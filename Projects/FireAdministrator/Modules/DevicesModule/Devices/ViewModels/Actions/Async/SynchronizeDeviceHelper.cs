﻿using System;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public static class SynchronizeDeviceHelper
    {
		static Device _device;
        static bool _isUsb;
        static OperationResult<bool> _operationResult;

        public static void Run(Device device, bool isUsb)
        {
			_device = device;
            _isUsb = isUsb;

            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressAndDriver + ". Установка времени");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.SynchronizeDevice(_device, _isUsb);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
                return;
            }
            MessageBoxService.Show("Операция завершилась успешно");
        }
    }
}
