﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Common;
using Resurs.Service;
using Resurs.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Theme;
using System.Windows.Forms;
using ResursRunner;
using Microsoft.Win32;
using Resurs.Views;
using ResursDAL;

namespace Resurs
{
	public static class Bootstrapper
	{
		public static MainViewModel MainViewModel;
		public static MainView MainView;

		public static void Run(bool showWindow)
		{
			try
			{
				AddToAutorun();
				ThemeHelper.LoadThemeFromRegister();
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Main/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Devices/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Consumers/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Reports/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Users/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "JournalEvents/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Tariffs/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Deposits/DataTemplates/Dictionary.xaml"));
				try
				{
					App.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
					Activate(showWindow);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
					BalloonHelper.Show("АРМ Ресурс", "Ошибка во время загрузки");
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				Close();
			}
		}

		public static void Activate()
		{
			Activate(true);
		}
		public static void Activate(bool showWindow)
		{
			if (showWindow && DBCash.CurrentUser == null)
			{
				var startupViewModel = new StartupViewModel();
				if (!DialogService.ShowModalWindow(startupViewModel))
					return;
			}
			
			if (MainViewModel == null)
				MainViewModel = new MainViewModel();
			
			if (MainView == null)
			{
				MainView = new MainView();
				MainView.DataContext = MainViewModel;
			}

			if (showWindow)
			{
				MainViewModel.UpdateTabsIsVisible();
				MainView.WindowState = System.Windows.WindowState.Normal;
				MainView.Show();
				MainView.Activate();
				MainView.ShowInTaskbar = true;
			}
		}

		static void AddToAutorun()
		{
			try
			{
				using (var registryKey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run"))
				{
					if (registryKey != null)
						registryKey.SetValue("Resurs", string.Format("\"{0}\" -hide", Application.ExecutablePath));
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при попытке добавления программы в автозагрузку");
			}
			
		}

		public static void Close()
		{
			System.Environment.Exit(1);
			Process.GetCurrentProcess().Kill();
		}
	}
}