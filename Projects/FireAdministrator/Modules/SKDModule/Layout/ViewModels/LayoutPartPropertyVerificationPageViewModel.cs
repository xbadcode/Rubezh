﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using Infrastructure.Common.Services.Layout;

namespace SKDModule.ViewModels
{
	public class LayoutPartPropertyVerificationPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartVerificationViewModel _layoutPartVerificationViewModel;

		public LayoutPartPropertyVerificationPageViewModel(LayoutPartVerificationViewModel layoutPartFilterViewModel)
		{
			_layoutPartVerificationViewModel = layoutPartFilterViewModel;

			Devices = new ObservableCollection<SKDDevice>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Reader)
				{
					Devices.Add(device);
				}
			}
		}

		public override string Header
		{
			get { return "Настройка верификации"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartSKDVerificationProperties)_layoutPartVerificationViewModel.Properties;
			SelectedDevice = Devices.FirstOrDefault(x => x.UID == properties.ReaderDeviceUID);
		}

		public ObservableCollection<SKDDevice> Devices { get; private set; }

		SKDDevice _selectedDevice;
		public SKDDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public override bool CanSave()
		{
			return SelectedDevice != null;
		}
		public override bool Save()
		{
			var properties = (LayoutPartSKDVerificationProperties)_layoutPartVerificationViewModel.Properties;

			properties.ReaderDeviceUID = SelectedDevice.UID;
			_layoutPartVerificationViewModel.UpdateLayoutPart();
			return true;
		}
	}
}