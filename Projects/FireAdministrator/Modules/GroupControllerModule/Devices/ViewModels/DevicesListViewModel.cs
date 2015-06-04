﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DevicesListViewModel : DialogViewModel
	{
		public DevicesListViewModel()
		{
			Title = "Список устройств";
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in GKManager.Devices)
			{
				if (device.IsRealDevice)
				{
					var deviceViewModel = new DeviceViewModel(device);
					Devices.Add(deviceViewModel);
				}
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
				SelectedDeviceDescriptorName = value.Device.GetGKDescriptorName(GKManager.DeviceConfiguration.GKNameGenerationType);
			}
		}

		string _selectedDeviceDescriptorName;
		public string SelectedDeviceDescriptorName
		{
			get { return _selectedDeviceDescriptorName; }
			set
			{
				_selectedDeviceDescriptorName = value;
				OnPropertyChanged(() => SelectedDeviceDescriptorName);
			}
		}
	}
}