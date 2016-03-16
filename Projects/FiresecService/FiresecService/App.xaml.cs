﻿using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecService;
using FiresecService.ViewModels;
using FiresecService.Views;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Theme;
using System;
using System.Diagnostics;
using System.Windows;
using KeyGenerator;

namespace FiresecServiceRunner
{
	public partial class App
	{
		private const string SignalId = "{59CFC4B4-BA41-4F34-9C41-1CA3851D7019}";
		private const string WaitId = "{6023CC31-322E-4A74-86FD-E851C2E6C20C}";

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ThemeHelper.LoadThemeFromRegister();

			try
			{
				PatchManager.Patch();
			}
			catch (Exception)
			{
				MessageBox.Show("Не удалось подключиться к базе данных");
				Current.MainWindow.Close();
			}

			var licenseService = new LicenseManager();

			if (!licenseService.IsValidExistingKey())
			{
				//TODO:block connections
			}

			using (new DoubleLaunchLocker(SignalId, WaitId, true))
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
				AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
				try
				{
					Bootstrapper.Run(licenseService);
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "App.OnStartup");
					BalloonHelper.ShowFromServer("Ошибка во время загрузки");
					return;
				}
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			ProcedureRunner.Terminate();
			base.OnExit(e);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
			BalloonHelper.ShowFromServer("Перезагрузка");
			var processStartInfo = new ProcessStartInfo
			{
				FileName = ResourceAssembly.Location
			};
			Process.Start(processStartInfo);
			Bootstrapper.Close();
			Current.MainWindow.Close();
			Current.Shutdown();
		}

		private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
		}
	}
}