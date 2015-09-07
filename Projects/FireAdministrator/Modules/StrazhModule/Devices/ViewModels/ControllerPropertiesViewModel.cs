﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure;
using System.ComponentModel;
using Infrastructure.Events;
using System.Threading;
using System.Collections.ObjectModel;

namespace StrazhModule.ViewModels
{
	public class ControllerPropertiesViewModel : SaveCancelDialogViewModel
	{
		public SKDDevice Device { get; private set; }
		public SKDDeviceInfo DeviceInfo { get; private set; }

		public ControllerPropertiesViewModel(SKDDevice device, SKDDeviceInfo deviceInfo)
		{
			Title = "Конфигурация контроллера";
			Device = device;
			DeviceInfo = deviceInfo;

			ResetCommand = new RelayCommand(OnReset);
			RebootCommand = new RelayCommand(OnReboot);
			RewriteAllCardsCommand = new RelayCommand(OnRewriteAllCards);
			RewriteConfigurationCommand = new RelayCommand(OnRewriteConfiguration);
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (!MessageBoxService.ShowQuestion(
					"При сбросе настроек все параметры контроллера будут установлены на заводские, а записанные карты доступа и пропуска будут стерты. Продолжить?",
					null, MessageBoxImage.Warning))
				return;
			var result = FiresecManager.FiresecService.SKDResetController(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции", result.Error);
			}
		}

		public RelayCommand RebootCommand { get; private set; }
		void OnReboot()
		{
			var result = FiresecManager.FiresecService.SKDRebootController(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Выполняется перезагрузка контроллера. Контроллер будет доступен через несколько секунд");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции", result.Error);
			}
		}

		public RelayCommand RewriteAllCardsCommand { get; private set; }
		void OnRewriteAllCards()
		{
			var thread = new Thread(() =>
			{
				var result = FiresecManager.FiresecService.SKDRewriteAllCards(Device);

				ApplicationService.Invoke(new Action(() =>
				{
					if (result.HasError)
					{
						LoadingService.Close();
						MessageBoxService.ShowWarning(result.Error);
					}
				}));
			});
			thread.Name = "DeviceCommandsViewModel OnWriteTimeSheduleConfiguration";
			thread.Start();
		}

		public RelayCommand RewriteConfigurationCommand { get; private set; }
		void OnRewriteConfiguration()
		{
			if (!CheckNeedSave())
				return;

			if (!ValidateConfiguration())
				return;

			// Записываем графики доступа
			WriteTimeSheduleConfiguration();

			// Записываем графики замков
			// TODO: Запись графиков замков

			// Записываем пароли замков
			// TODO: Запись паралей замков
		}

		/// <summary>
		/// Проверяет текущую конфигурацию на наличие ошибок
		/// </summary>
		/// <returns>true - ошибок не обнаружено,
		/// false - есть ошибки</returns>
		private bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.HasErrors(ModuleType.SKD))
			{
				if (validationResult.CannotSave(ModuleType.SKD) || validationResult.CannotWrite(ModuleType.SKD))
				{
					MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Проверяет необходимость сохранения конфигурации перед выполнением операций, зависящих от состояния текущей конфигурации
		/// </summary>
		/// <returns>true - текущая конфигурация уже сохранена (нет изменений),
		/// false - текущая конфигурация еще не сохранена (есть изменения) </returns>
		private bool CheckNeedSave()
		{
			if (ServiceFactory.SaveService.SKDChanged)
			{
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Применить сейчас?"))
				{
					var cancelEventArgs = new CancelEventArgs();
					ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Publish(cancelEventArgs);
				}
				return false;
			}
			return true;
		}

		void WriteTimeSheduleConfiguration()
		{
			var thread = new Thread(() =>
			{
				var result = FiresecManager.FiresecService.SKDWriteTimeSheduleConfiguration(Device);

				ApplicationService.Invoke(new Action(() =>
				{
					if (result.HasError)
					{
						LoadingService.Close();
						MessageBoxService.ShowWarning(result.Error);
					}

					var oldHasMismatch = HasMismatch;
					if (ClientSettings.SKDMissmatchSettings.HasMissmatch(Device.UID))
					{
						if (result.HasError)
						{
							ClientSettings.SKDMissmatchSettings.Set(Device.UID);
						}
						else
						{
							ClientSettings.SKDMissmatchSettings.Reset(Device.UID);
						}
					}
					OnPropertyChanged(() => HasMismatch);
					if (HasMismatch != oldHasMismatch)
						ServiceFactory.SaveService.SKDChanged = true;
				}));
			});
			thread.Name = "ControllerPropertiesViewModel OnWriteTimeSheduleConfiguration";
			thread.Start();
		}

		private bool HasMismatch
		{
			get
			{
				//foreach (var device in SKDManager.Devices)
				//{
				//	if (device.Driver.IsController)
				//	{
				//		if (ClientSettings.SKDMissmatchSettings.HasMissmatch(device.UID))
				//			return true;
				//	}
				//}
				return false;
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}
}