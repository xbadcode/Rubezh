﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;
using System;
using Infrastructure;
using GKModule.Devices.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{

		public NewDeviceViewModel(DeviceViewModel deviceViewModel)
		{			
			Title = "Добавление устройства";

			if (deviceViewModel.Device.Parent != null && deviceViewModel.Device.Parent.DriverType == GKDriverType.GKMirror)
				deviceViewModel = deviceViewModel.Parent;

			ParentDeviceViewModel = deviceViewModel;
			ParentDevice = ParentDeviceViewModel.Device;
			AddedDevices = new List<DeviceViewModel>();
			
			if (ParentDevice.IsConnectedToKAU)
				{
					RealParentDevice = ParentDevice.MVPPartParent ?? ParentDevice.KDPartParent ?? ParentDevice.MRKParent ?? ParentDevice.KAUShleifParent;
					Drivers = new ObservableCollection<GKDriver>(SortDrivers().Where(x => RealParentDevice.Driver.Children.Contains(x.DriverType)));
					TypedDrivers = new ObservableCollection<NewTypedDeviceViewModel>(Drivers.Select(x => new NewTypedDeviceViewModel(x)));
					SelectedDriver = TypedDrivers.FirstOrDefault();
					MinAddress = 1;
					MaxAddress = 255;
					BuildTree(TypedDrivers);
				}
				else
				{
					Drivers = new ObservableCollection<GKDriver>(SortDrivers().Where(x => ParentDevice.Driver.Children.Contains(x.DriverType)));
					TypedDrivers = new ObservableCollection<NewTypedDeviceViewModel>(Drivers.Select(x => new NewTypedDeviceViewModel(x)));
					SelectedDriver = TypedDrivers.FirstOrDefault();
					MinAddress = SelectedDriver.Driver.MinAddress;
					MaxAddress = SelectedDriver.Driver.MaxAddress;
				}

		Count = 1;
		}
		GKDevice RealParentDevice;
		DeviceViewModel ParentDeviceViewModel;
		GKDevice ParentDevice;
		public List<DeviceViewModel> AddedDevices { get; private set; }
		public ObservableCollection<GKDriver> Drivers { get; private  set; }
		public ObservableCollection<NewTypedDeviceViewModel> TypedDrivers { get; private set; }

		public int MaxAddress { get; private set; }
		public int MinAddress { get; private set; }
		public bool AddInStartlList { get; set; }

		NewTypedDeviceViewModel _selectedDriver;
		public NewTypedDeviceViewModel SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged(() => SelectedDriver);
			}
		}

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged(() => Count);
			}
		}

		public List<GKDriver> SortDrivers()
		{
			var driverCounters = new List<DriverCounter>();
			foreach (var driver in GKManager.Drivers)
			{
				var driverCounter = new DriverCounter()
				{
					Driver = driver,
					Count = 0
				};
				driverCounters.Add(driverCounter);
			}
			foreach (var device in GKManager.Devices)
			{
				var driverCounter = driverCounters.FirstOrDefault(x => x.Driver == device.Driver);
				if (driverCounter != null)
				{
					driverCounter.Count++;
				}
			}
			var sortedDrivers = driverCounters.OrderByDescending(x => x.Count).Select(x => x.Driver);
			return sortedDrivers.ToList();
		}

		public bool ShowCheckBox { get { return ParentDevice.DriverType == GKDriverType.RSR2_KAU_Shleif || ParentDevice.DriverType == GKDriverType.RSR2_MVP_Part || ParentDevice.DriverType == GKDriverType.RSR2_KDKR_Part; } }
		public bool CreateDevices()
		{
			AddedDevices = new List<DeviceViewModel>();
			var startAddress = RealParentDevice == null ? GKManager.GetAddress(ParentDevice.Children.Where(x => x.Driver.HasAddress)) : GKManager.GetAddress(RealParentDevice.KAUShleifParent.AllChildren);

			if (RealParentDevice != null)
			{
					if (Count * Math.Max(1, (int)SelectedDriver.Driver.GroupDeviceChildrenCount) + startAddress > 255)
					{
						ServiceFactory.MessageBoxService.ShowWarning("При добавлении количество устройств на АЛС максимально допустимое значения в 255");
						return false;
					}
			}
			else
			{
				if (Count + startAddress > SelectedDriver.Driver.MaxAddress && SelectedDriver.Driver.HasAddress)
				{
					ServiceFactory.MessageBoxService.ShowWarning("При добавлении устройств количество будет превышать максимально допустимое значения в " + SelectedDriver.Driver.MaxAddress.ToString());
					return false;
				}
			}

			for (int i = 1; i <= Count; i++)
			{
				var address = RealParentDevice == null ? startAddress + i : 0;
				if (RealParentDevice == null || RealParentDevice == ParentDevice)
				{
					GKDevice device = GKManager.AddDevice(ParentDevice, SelectedDriver.Driver, address, AddInStartlList ? 0 : (int?)null);
					AddedDevices.Add(NewDeviceHelper.AddDevice(device, ParentDeviceViewModel, isStartList: AddInStartlList));
				}
				else
				{
					var index = RealParentDevice.Children.IndexOf(ParentDevice) + 1;
					GKDevice device = GKManager.AddDevice(RealParentDevice, SelectedDriver.Driver, address, index);
					var addedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel, false);
					AddedDevices.Insert(0, addedDevice);
				}
			}
			if (RealParentDevice != null)
				GKManager.RebuildRSR2Addresses(ParentDevice);

			return true;
		}

		protected override bool CanSave()
		{
			return (SelectedDriver != null && SelectedDriver.Driver != null);
		}

		protected override bool Save()
		{

			var result = CreateDevices();
			if (result)
			{
				ParentDeviceViewModel.Update();
				GKManager.DeviceConfiguration.Update();
			}
			return result;
		}


		public bool IsNotGKOrMirror
		{
			get
			{
				if (ParentDevice.DriverType != GKDriverType.GK && ParentDevice.DriverType != GKDriverType.GKMirror)
				{
					return true;
				}
				return false;
			}
		}

		class DriverCounter
		{
			public GKDriver Driver { get; set; }
			public int Count { get; set; }
		}

		public ObservableCollection<NewTypedDeviceViewModel> RootDrivers { get; private set; }

		void BuildTree(ObservableCollection<NewTypedDeviceViewModel> typedDrivers)
		{
			RootDrivers = new ObservableCollection<NewTypedDeviceViewModel>();

			var accessControl = new NewTypedDeviceViewModel(GKDriver.TypesOfBranches.AccessControl);
			RootDrivers.Add(accessControl);

			var actuatingDevice = new NewTypedDeviceViewModel(GKDriver.TypesOfBranches.ActuatingDevice);
			RootDrivers.Add(actuatingDevice);

			var announcers = new NewTypedDeviceViewModel(GKDriver.TypesOfBranches.Announcers);
			RootDrivers.Add(announcers);

			var controlCabinet = new NewTypedDeviceViewModel(GKDriver.TypesOfBranches.ControlCabinet);
			RootDrivers.Add(controlCabinet);

			var fireDetector = new NewTypedDeviceViewModel(GKDriver.TypesOfBranches.FireDetector);
			RootDrivers.Add(fireDetector);

			var intruderDetector = new NewTypedDeviceViewModel(GKDriver.TypesOfBranches.IntruderDetector);
			RootDrivers.Add(intruderDetector);

			var other = new NewTypedDeviceViewModel(GKDriver.TypesOfBranches.Other);
			RootDrivers.Add(other);

			var radioChannel = new NewTypedDeviceViewModel(GKDriver.TypesOfBranches.RadioChannel);
			RootDrivers.Add(radioChannel);

			foreach (var typedDriver in typedDrivers)
			{
				switch (typedDriver.Driver.TypeOfBranche)
				{
					case GKDriver.TypesOfBranches.AccessControl:
						accessControl.AddChild(typedDriver);
						break;
					case GKDriver.TypesOfBranches.ActuatingDevice:
						actuatingDevice.AddChild(typedDriver);
						break;
					case GKDriver.TypesOfBranches.Announcers:
						announcers.AddChild(typedDriver);
						break;
					case GKDriver.TypesOfBranches.ControlCabinet:
						controlCabinet.AddChild(typedDriver);
						break;
					case GKDriver.TypesOfBranches.FireDetector:
						fireDetector.AddChild(typedDriver);
						break;
					case GKDriver.TypesOfBranches.IntruderDetector:
						intruderDetector.AddChild(typedDriver);
						break;
					case GKDriver.TypesOfBranches.Other:
						other.AddChild(typedDriver);
						break;
					case GKDriver.TypesOfBranches.RadioChannel:
						radioChannel.AddChild(typedDriver);
						break;
				}
			}
		}
	}
}