﻿using Common;
using FireMonitor.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		private string _login;
		private string _password;

		public bool Initialize()
		{
			bool result;
			LoadingErrorManager.Clear();
			AppConfigHelper.InitializeAppSettings();
		//	ServiceFactory.Initialize(new LayoutService(), new SecurityService(), new UiElementsVisibilityService());
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactoryBase.ResourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
			ServiceFactory.StartupService.Show();
			if (ServiceFactory.StartupService.PerformLogin(_login, _password))
			{
				var userChangedEventArgs = new UserChangedEventArgs
				{
					IsReconnect = false
				};
				ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Publish(userChangedEventArgs);
				_login = ServiceFactory.StartupService.Login;
				_password = ServiceFactory.StartupService.Password;
				try
				{
					// При получении от сервера команды на разрыв соединения выводим соответствующее предупреждение и завершаем работу
					SafeFiresecService.DisconnectClientCommandEvent += () =>
					{
						ApplicationService.Invoke(() => MessageBoxService.ShowWarning("Соединение было разорвано Сервером.\nРабота приложения будет завершена."));
						ApplicationService.ShutDown();
					};

					// При получении от сервера уведомления о смене лицензии выводим соответствующее предупреждение и завершаем работу
					//SafeFiresecService.LicenseChangedEvent += () =>
					//{
					//	ApplicationService.Invoke(() => MessageBoxService.ShowWarning("Соединение было разорвано Сервером в связи с изменением лицензии.\nРабота приложения будет завершена."));
					//	ApplicationService.ShutDown();
					//};

					// Получаем данные лицензии с Сервера
				//	ServiceFactory.UiElementsVisibilityService.Initialize(FiresecManager.FiresecService.GetLicenseData().Result);

					CreateModules();

					ServiceFactory.StartupService.ShowLoading("Чтение конфигурации", 15);
					ServiceFactory.StartupService.AddCount(GetModuleCount());

					ServiceFactory.StartupService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();

					ServiceFactory.StartupService.DoStep("Загрузка конфигурации с сервера");
					FiresecManager.GetConfiguration("Monitor/Configuration");

					BeforeInitialize(true);

					ServiceFactory.StartupService.DoStep("Старт полинга сервера");
					FiresecManager.StartPoll();

					ServiceFactory.StartupService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CheckPermission(PermissionType.Oper_Login))
					{
						ServiceFactory.StartupService.DoStep("Загрузка клиентских настроек");
						ClientSettings.LoadSettings();

						result = Run();
						SafeFiresecService.ConfigurationChangedEvent += () => ApplicationService.Invoke(OnConfigurationChanged);

						if (result)
						{
							AterInitialize();
						}
					}
					else
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();

						if (Application.Current != null)
							Application.Current.Shutdown();
						return false;
					}

					if (Process.GetCurrentProcess().ProcessName != "StrazhMonitor.vshost")
					{
						RegistrySettingsHelper.SetBool("isException", true);
					}
					ServiceFactory.StartupService.Close();
				}
				catch (StartupCancellationException)
				{
					throw;
				}
				catch (Exception e)
				{
					Logger.Error(e, "Bootstrapper.InitializeFs");
					MessageBoxService.ShowException(e);
					if (Application.Current != null)
						Application.Current.Shutdown();
					return false;
				}
			}
			else
			{
				if (Application.Current != null)
					Application.Current.Shutdown();
				return false;
			}
			return result;
		}
		protected virtual bool Run()
		{
			var result = true;
			var shell = CreateShell();
			((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
			if (!RunShell(shell))
				result = false;
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new UserViewModel());
			return result;
		}
		protected virtual ShellViewModel CreateShell()
		{
			return new MonitorShellViewModel();
		}

		protected virtual void OnConfigurationChanged()
		{
			var restartView = new RestartApplicationViewModel();
			var isRestart = DialogService.ShowModalWindow(restartView);
			if (isRestart)
				Restart();
			else
			{
				var timer = new DispatcherTimer();
				timer.Tick += (s, e) =>
				{
					timer.Stop();
					Restart();
				};
				timer.Interval = TimeSpan.FromSeconds(restartView.Total);
				timer.Start();
			}
		}
		private void Restart()
		{
			ApplicationService.ApplicationWindow.IsEnabled = false;
			ServiceFactoryBase.ContentService.Clear();
			FiresecManager.FiresecService.StopPoll();
			LoadingErrorManager.Clear();
			ApplicationService.CloseAllWindows();
			ServiceFactory.Layout.Close();
			ApplicationService.ShutDown();
			RestartApplication();
		}

		public void RestartApplication()
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = Application.ResourceAssembly.Location,
				Arguments = GetRestartCommandLineArguments()
			};
			Process.Start(processStartInfo);
		}
		protected virtual string GetRestartCommandLineArguments()
		{
			string commandLineArguments = null;
			if (_login != null && _password != null)
				commandLineArguments = "login='" + _login + "' password='" + _password + "'";
			return commandLineArguments;
		}
		public virtual void InitializeCommandLineArguments(string[] args)
		{
			if (args == null || args.Count() < 2) return;

			foreach (var arg in args)
			{
				if (arg.StartsWith("login='") && arg.EndsWith("'"))
				{
					_login = arg.Replace("login='", "");
					_login = _login.Replace("'", "");
				}

				if (arg.StartsWith("password='") && arg.EndsWith("'"))
				{
					_password = arg.Replace("password='", "");
					_password = _password.Replace("'", "");
				}
			}
		}
	}
}