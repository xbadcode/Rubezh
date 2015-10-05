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
using FiresecAPI;
using Infrastructure.Common.Theme;
using System.Windows.Forms;
using ResursRunner;

namespace Resurs
{
	public static class Bootstrapper
	{
		static MainViewModel MainViewModel;

		public static void Run(bool showWindow)
		{
			try
			{
				ThemeHelper.LoadThemeFromRegister();
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Main/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Devices/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Apartments/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Reports/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Users/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "JournalEvents/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "Tariffs/DataTemplates/Dictionary.xaml"));
				try
				{
					App.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

					var startupViewModel = new StartupViewModel();
					if (!DialogService.ShowModalWindow(startupViewModel))
						return;

					var mainView = new Resurs.Views.MainView();
					var mainViewModel = new MainViewModel();
					mainView.DataContext = mainViewModel;
					if (showWindow)
					{
						mainView.Show();
					}
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

		public static void Close()
		{
			System.Environment.Exit(1);
			Process.GetCurrentProcess().Kill();
		}
	}
}