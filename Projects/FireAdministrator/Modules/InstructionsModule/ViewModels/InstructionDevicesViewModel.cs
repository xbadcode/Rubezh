﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionDevicesViewModel : SaveCancelDialogContent
    {
        public InstructionDevicesViewModel(List<Guid> instructionDevicesList)
        {
            Title = "Выбор устройства";

            InstructionDevicesList = new List<Guid>(instructionDevicesList);
            InstructionDevices = new ObservableCollection<DeviceViewModel>();
            AvailableDevices = new ObservableCollection<DeviceViewModel>();

            UpdateDevices();

            AddOneCommand = new RelayCommand(OnAddOne, CanAddOne);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
        }

        void UpdateDevices()
        {
            var availableDevices = new HashSet<Device>();
            var instructionDevices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if ((device.Driver.Category == DeviceCategoryType.Sensor) ||
                    (device.Driver.Category == DeviceCategoryType.Effector) ||
                    (device.Driver.Category == DeviceCategoryType.Device))
                {
                    if (InstructionDevicesList.Contains(device.UID))
                    {
                        device.AllParents.ForEach(x => { instructionDevices.Add(x); });
                        instructionDevices.Add(device);
                    }
                    else
                    {
                        device.AllParents.ForEach(x => { availableDevices.Add(x); });
                        availableDevices.Add(device);
                    }
                }
            }

            InstructionDevices.Clear();
            BuildTree(instructionDevices, InstructionDevices);

            AvailableDevices.Clear();
            BuildTree(availableDevices, AvailableDevices);

            if (InstructionDevices.IsNotNullOrEmpty())
            {
                CollapseChild(InstructionDevices[0]);
                ExpandChild(InstructionDevices[0]);
                SelectedInstructionDevice = InstructionDevices[0];
            }
            else
            {
                SelectedInstructionDevice = null;
            }

            if (AvailableDevices.IsNotNullOrEmpty())
            {
                CollapseChild(AvailableDevices[0]);
                ExpandChild(AvailableDevices[0]);
                SelectedAvailableDevice = AvailableDevices[0];
            }
            else
            {
                SelectedAvailableDevice = null;
            }
        }

        void BuildTree(HashSet<Device> hashSetDevices, ObservableCollection<DeviceViewModel> devices)
        {
            foreach (var device in hashSetDevices)
            {
                var deviceViewModel = new DeviceViewModel(device, devices);
                deviceViewModel.IsExpanded = true;
                if ((device.Driver.Category == DeviceCategoryType.Sensor) ||
                    (device.Driver.Category == DeviceCategoryType.Effector) ||
                    (device.Driver.Category == DeviceCategoryType.Device))
                    deviceViewModel.IsBold = true;
                devices.Add(deviceViewModel);
            }

            foreach (var device in devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }
        }

        void CollapseChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = false;

            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        void ExpandChild(DeviceViewModel parentDeviceViewModel)
        {
            if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }

        public List<Guid> InstructionDevicesList { get; set; }
        public ObservableCollection<DeviceViewModel> AvailableDevices { get; set; }
        public ObservableCollection<DeviceViewModel> InstructionDevices { get; set; }

        DeviceViewModel _selectedAvailableDevice;
        public DeviceViewModel SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        DeviceViewModel _selectedInstructionDevice;
        public DeviceViewModel SelectedInstructionDevice
        {
            get { return _selectedInstructionDevice; }
            set
            {
                _selectedInstructionDevice = value;
                OnPropertyChanged("SelectedInstructionDevice");
            }
        }

        public bool CanAddOne()
        {
            return ((SelectedAvailableDevice != null) && (SelectedAvailableDevice.IsBold));
        }

        public bool CanAddAll()
        {
            return (AvailableDevices.IsNotNullOrEmpty());
        }

        public bool CanRemoveOne()
        {
            return ((SelectedInstructionDevice != null) && (SelectedInstructionDevice.IsBold));
        }

        public bool CanRemoveAll()
        {
            return (InstructionDevices.IsNotNullOrEmpty());
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            InstructionDevicesList.Add(SelectedAvailableDevice.UID);
            UpdateDevices();
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var deviceViewModel in AvailableDevices)
            {
                if (deviceViewModel.IsBold)
                    InstructionDevicesList.Add(deviceViewModel.UID);
            }
            UpdateDevices();
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            InstructionDevicesList.Clear();
            UpdateDevices();
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            InstructionDevicesList.Remove(SelectedInstructionDevice.UID);
            UpdateDevices();
        }
    }
}