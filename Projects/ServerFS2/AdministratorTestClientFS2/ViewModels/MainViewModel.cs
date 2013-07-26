﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using ServerFS2.ConfigurationWriter;
using ServerFS2.Processor;
using System.Diagnostics;
using Microsoft.Win32;
using ServerFS2.Service;
using FS2Api;
using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.IO;

namespace AdministratorTestClientFS2.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
		public ZonesViewModel ZonesViewModel { get; private set; }
		FS2Contract FS2Contract = new FS2Contract();

		public MainViewModel()
		{
			CancelProgressCommand = new RelayCommand(OnCancelProgress);
			SendRequestCommand = new RelayCommand(OnSendRequest);
			AutoDetectDeviceCommand = new RelayCommand(OnAutoDetectDevice);
			ReadConfigurationCommand = new RelayCommand(OnReadConfiguration, CanReadConfiguration);
			ReadJournalCommand = new RelayCommand(OnReadJournal, CanReadJournal);
			GetInformationCommand = new RelayCommand(OnGetInformation, CanGetInformation);
			SynchronizeTimeCommand = new RelayCommand(OnSynchronizeTime, CanSynchronizeTime);
			SetPasswordCommand = new RelayCommand(OnSetPassword, CanSetPassword);
			RunOtherFunctionsCommand = new RelayCommand(OnRunOtherFunctions, CanRunOtherFunctions);
			UpdateFirmwhareCommand = new RelayCommand(OnUpdateFirmwhare, CanUpdateFirmwhare);
			WriteConfigurationCommand = new RelayCommand(OnWriteConfiguration, CanWriteConfiguration);
			GetDeviceStatusCommand = new RelayCommand(OnGetDeviceStatus, CanGetResetDeviceStatus);
			TestCommand = new RelayCommand(OnTest);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			ZonesViewModel.Initialize();
			ProgressInfos = new ObservableRangeCollection<FS2ProgressInfo>();
			CallbackManager.ProgressEvent += new System.Action<FS2Api.FS2ProgressInfo>(CallbackManager_ProgressEvent);
		}

		#region Progress
		public ObservableRangeCollection<FS2ProgressInfo> ProgressInfos { get; private set; }

		void CallbackManager_ProgressEvent(FS2ProgressInfo fs2ProgressInfos)
		{
			Application.Current.Dispatcher.Invoke(new Action(
				() =>
				{
					ProgressInfos.Insert(0, fs2ProgressInfos);
					if (ProgressInfos.Count > 1000)
						ProgressInfos.RemoveAt(1000);
				}
				));
			Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
		}

		public RelayCommand CancelProgressCommand { get; private set; }
		private void OnCancelProgress()
		{
			FS2Contract.CancelProgress();
		}
		#endregion

		List<byte> _status;
		List<byte> Status
		{
			get { return _status; }
			set
			{
				_status = value;
				if (_status != null)
					StatusString = TraceHelper.TraceBytes(value);
				else
					StatusString = "Нет сведений о статусе прибора";
			}
		}

		string _statusString;
		public string StatusString
		{
			get { return _statusString; }
			set
			{
				_statusString = value;
				OnPropertyChanged("StatusString");
			}
		}

		bool _isUsbDevice;
		public bool IsUsbDevice
		{
			get { return _isUsbDevice; }
			set
			{
				_isUsbDevice = value;
				OnPropertyChanged("IsUsbDevice");
			}
		}

		string _textBoxRequest;
		public string TextBoxRequest
		{
			get { return _textBoxRequest; }
			set
			{
				_textBoxRequest = value;
				OnPropertyChanged("TextBoxRequest");
			}
		}

		string _textBoxResponse;
		public string TextBoxResponse
		{
			get { return _textBoxResponse; }
			set
			{
				_textBoxResponse = value;
				OnPropertyChanged("TextBoxResponse");
			}
		}

		public RelayCommand SendRequestCommand { get; private set; }
		void OnSendRequest()
		{
			var bytes = TextBoxRequest.Split().Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
			var response = USBManager.Send(DevicesViewModel.SelectedDevice.Device, bytes);
			TextBoxResponse += BytesHelper.BytesToString(response.Bytes);
		}

		public RelayCommand AutoDetectDeviceCommand { get; private set; }
		void OnAutoDetectDevice()
		{
			var device = AutoDetectOperationHelper.AutoDetectDevice(DevicesViewModel.SelectedDevice.Device);
			var autoDetectedDevicesViewModel = new DevicesViewModel(device);
			DialogService.ShowModalWindow(autoDetectedDevicesViewModel);
		}

		public RelayCommand ReadJournalCommand { get; private set; }
		void OnReadJournal()
		{
			var fs2JournalItemsCollection = MainManager.DeviceReadJournal(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);
			var journalViewModel = new JournalViewModel(fs2JournalItemsCollection.FireJournalItems);
			DialogService.ShowModalWindow(journalViewModel);
			if (fs2JournalItemsCollection.SecurityJournalItems.Count > 0)
			{
				journalViewModel = new JournalViewModel(fs2JournalItemsCollection.SecurityJournalItems);
				DialogService.ShowModalWindow(journalViewModel);
			}
		}
		bool CanReadJournal()
		{
			return DeviceValidation(DevicesViewModel.SelectedDevice);
		}

		public RelayCommand ReadConfigurationCommand { get; private set; }
		void OnReadConfiguration()
		{
			var remoteDeviceConfiguration = MainManager.DeviceReadConfiguration(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);
			DialogService.ShowModalWindow(new DeviceConfigurationViewModel(DevicesViewModel.SelectedDevice.Device.UID, remoteDeviceConfiguration));
		}
		bool CanReadConfiguration()
		{
			return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.Driver.IsPanel));
		}
		public RelayCommand GetInformationCommand { get; private set; }
		void OnGetInformation()
		{
			var result = MainManager.DeviceGetInformation(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);
			DialogService.ShowModalWindow(new InformationViewModel(result));
		}
		bool CanGetInformation()
		{
			var selectedDevice = DevicesViewModel.SelectedDevice;
			return ((selectedDevice != null) && ((selectedDevice.Device.Driver.IsPanel) || (selectedDevice.Device.Driver.DriverType == DriverType.MS_1 || selectedDevice.Device.Driver.DriverType == DriverType.MS_2)));
		}

		public RelayCommand SynchronizeTimeCommand { get; private set; }
		void OnSynchronizeTime()
		{
			MainManager.DeviceDatetimeSync(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);
		}
		bool CanSynchronizeTime()
		{
			return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.Driver.IsPanel));
		}

		public RelayCommand RunOtherFunctionsCommand { get; private set; }
		void OnRunOtherFunctions()
		{
			DialogService.ShowModalWindow(new OtherFunctionViewModel(DevicesViewModel.SelectedDevice.Device));
		}
		bool CanRunOtherFunctions()
		{
			if (DevicesViewModel.SelectedDevice == null)
				return false;
			var driverType = DevicesViewModel.SelectedDevice.Device.Driver.DriverType;
			return ((driverType == DriverType.IndicationBlock) || (driverType == DriverType.PDU) ||
			        (driverType == DriverType.PDU_PT));
		}

		public RelayCommand SetPasswordCommand { get; private set; }
		void OnSetPassword()
		{
			DialogService.ShowModalWindow(new PasswordViewModel(DevicesViewModel.SelectedDevice.Device));
		}
		bool CanSetPassword()
		{
			return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.Driver.IsPanel));
		}

		public RelayCommand UpdateFirmwhareCommand { get; private set; }
		void OnUpdateFirmwhare()
		{
			//var openFileDialog = new OpenFileDialog()
			//{
			//    Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.FSCF)|*.FSCF|All files (*.*)|*.*"
			//};
			//if (openFileDialog.ShowDialog() == true)
			//{
			//    var fileName = openFileDialog.FileName;
			//    var message = MainManager.DeviceVerifyFirmwareVersion(DevicesViewModel.SelectedDevice.Device, IsUsbDevice, fileName);
			//    MessageBoxService.Show(message);
			//    MainManager.DeviceUpdateFirmware(DevicesViewModel.SelectedDevice.Device, IsUsbDevice, fileName);
			//}
			MainManager.DeviceUpdateFirmware(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);
		}
		bool CanUpdateFirmwhare()
		{
			return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.Driver.IsPanel));
		}

		public RelayCommand WriteConfigurationCommand { get; private set; }
		void OnWriteConfiguration()
		{
			MainManager.DeviceWriteConfiguration(DevicesViewModel.SelectedDevice.Device, IsUsbDevice, null);

		}
		bool CanWriteConfiguration()
		{
			return DevicesViewModel.SelectedDevice != null;
		}

		public RelayCommand GetDeviceStatusCommand { get; private set; }
		void OnGetDeviceStatus()
		{
			Status = ServerHelper.GetDeviceStatus(DevicesViewModel.SelectedDevice.Device);
		}
		bool CanGetResetDeviceStatus()
		{
			return DeviceValidation(DevicesViewModel.SelectedDevice);
		}


		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			var firmwareFileName = Path.Combine(AppDataFolderHelper.GetFolder("Server"), "frm.fscf");
			var hexInfo = FirmwareUpdateOperationHelper.GetHexInfo(firmwareFileName, DevicesViewModel.SelectedDevice.Device.Driver.ShortName + ".hex");
			return;
		}

		bool DeviceValidation(DeviceViewModel selectedDeivice)
		{
			return (selectedDeivice != null) && (selectedDeivice.Device.Driver.IsPanel);
		}
	}
}