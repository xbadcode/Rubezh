﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.GK;
using FiresecAPI.XModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public partial class DeviceViewModel
    {
        #region Capy and Paste
        public static XDriver DriverCopy;
        public static List<XProperty> PropertiesCopy;

        public RelayCommand CopyParamCommand { get; private set; }
        void OnCopy()
        {
            DriverCopy = Device.Driver;
            PropertiesCopy = new List<XProperty>();
            foreach (var property in Device.Properties)
            {
                var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
                if (driverProperty != null && driverProperty.IsAUParameter)
                {
                    var propertyCopy = new XProperty()
                    {
                        StringValue = property.StringValue,
                        Name = property.Name,
                        Value = property.Value
                    };
                    PropertiesCopy.Add(propertyCopy);
                }
            }
        }
        bool CanCopy()
        {
            return HasAUProperties;
        }

        public RelayCommand PasteParamCommand { get; private set; }
        void OnPaste()
        {
            CopyParametersFromBuffer(Device);
            PropertiesViewModel.Update();
            //UpdateDeviceParameterMissmatch();
        }
        bool CanPaste()
        {
            return DriverCopy != null && Device.Driver.DriverType == DriverCopy.DriverType;
        }

        public RelayCommand PasteAllParamCommand { get; private set; }
        void OnPasteAll()
        {
            foreach (var device in XManager.GetAllDeviceChildren(Device))
            {
                CopyParametersFromBuffer(device);
                var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
                if (deviceViewModel != null)
                    deviceViewModel.PropertiesViewModel.Update();
            }
            //UpdateDeviceParameterMissmatch();
        }
        bool CanPasteAll()
        {
            return Children.Count() > 0 && DriverCopy != null;
        }

        static void CopyParametersFromBuffer(XDevice device)
        {
            foreach (var property in PropertiesCopy)
            {
                var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == property.Name);
                if (deviceProperty != null)
                {
                    deviceProperty.Value = property.Value;
                }
            }
            ServiceFactory.SaveService.GKChanged = true;
        }
        #endregion

        #region Template
        public RelayCommand PasteTemplateCommand { get; private set; }
        void OnPasteTemplate()
        {
            var parameterTemplateSelectationViewModel = new ParameterTemplateSelectationViewModel();
            if (DialogService.ShowModalWindow(parameterTemplateSelectationViewModel))
            {
                CopyParametersFromTemplate(parameterTemplateSelectationViewModel.SelectedParameterTemplate, Device);
                PropertiesViewModel.Update();
            }
            //UpdateDeviceParameterMissmatch();
        }
        bool CanPasteTemplate()
        {
            return HasAUProperties;
        }

        public RelayCommand PasteAllTemplateCommand { get; private set; }
        void OnPasteAllTemplate()
        {
            var parameterTemplateSelectationViewModel = new ParameterTemplateSelectationViewModel();
            if (DialogService.ShowModalWindow(parameterTemplateSelectationViewModel))
            {
                var devices = XManager.GetAllDeviceChildren(Device);
                devices.Add(Device);
                foreach (var device in devices)
                {
                    CopyParametersFromTemplate(parameterTemplateSelectationViewModel.SelectedParameterTemplate, device);
                    var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
                    if (deviceViewModel != null)
                        deviceViewModel.PropertiesViewModel.Update();
                }
            }
            //UpdateDeviceParameterMissmatch();
        }
        bool CanPasteAllTemplate()
        {
            return Children.Count() > 0;
        }

        static void CopyParametersFromTemplate(XParameterTemplate parameterTemplate, XDevice device)
        {
            var deviceParameterTemplate = parameterTemplate.DeviceParameterTemplates.FirstOrDefault(x => x.XDevice.DriverUID == device.Driver.UID);
            if (deviceParameterTemplate != null)
            {
                foreach (var property in deviceParameterTemplate.XDevice.Properties)
                {
                    var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == property.Name);
                    if (deviceProperty != null)
                    {
                        deviceProperty.Value = property.Value;
                    }
                }
            }
        }
        #endregion

        public RelayCommand WriteCommand { get; private set; }
        public RelayCommand SyncFromSystemToDeviceCommand { get; private set; }
        void OnSyncFromSystemToDevice()
        {
            if (CheckNeedSave())
            {
                var devices = new List<XDevice>() { Device };
                if (Validate(devices))
                {
                    CopyFromSystemToDevice(Device);
                    PropertiesViewModel.Update();
                    WriteDevices(devices);
                }
            }
        }

        public RelayCommand WriteAllCommand { get; private set; }
        public RelayCommand SyncFromAllSystemToDeviceCommand { get; private set; }
        void SyncFromAllSystemToDevice()
        {
            if (CheckNeedSave())
            {
                var devices = GetRealChildren();
                devices.Add(Device);
                if (Validate(devices))
                {
                    foreach (var device in devices)
                    {
                        CopyFromSystemToDevice(device);
                        var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
                        if (deviceViewModel != null)
                            deviceViewModel.PropertiesViewModel.Update();
                    }
                    WriteDevices(devices);
                }
            }
        }

        public RelayCommand SyncFromDeviceToSystemCommand { get; private set; }
        void OnSyncFromDeviceToSystem()
        {
            if (CheckNeedSave())
            {
                CopyFromDeviceToSystem(Device);
                PropertiesViewModel.Update();
                //UpdateDeviceParameterMissmatch();
            }
        }

        public RelayCommand SyncFromAllDeviceToSystemCommand { get; private set; }
        void OnSyncFromAllDeviceToSystem()
        {
            if (CheckNeedSave())
            {
                var devices = GetRealChildren();
                devices.Add(Device);
                foreach (var device in devices)
                {
                    CopyFromDeviceToSystem(device);
                    var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
                    if (deviceViewModel != null)
                        deviceViewModel.PropertiesViewModel.Update();
                }
            }
        }

        bool CanSync()
        {
            return HasAUProperties;
        }

        bool CanSyncAll()
        {
            return Children.Count() > 0;
        }

        static void CopyFromDeviceToSystem(XDevice device)
        {
            device.Properties.RemoveAll(x => x.Name != "IPAddress");
            foreach (var property in device.DeviceProperties)
            {
                var clonedProperty = new XProperty
                {
                    Name = property.Name,
                    Value = property.Value
                };
                device.Properties.Add(clonedProperty);
            }
            ServiceFactory.SaveService.GKChanged = true;
        }

        static void CopyFromSystemToDevice(XDevice device)
        {
            device.DeviceProperties.Clear();
            foreach (var property in device.Properties)
            {
                var clonedProperty = new XProperty()
                {
                    Name = property.Name,
                    Value = property.Value
                };
                device.DeviceProperties.Add(clonedProperty);
            }
            ServiceFactory.SaveService.GKChanged = true;
        }

        bool Validate(IEnumerable<XDevice> devices)
        {
            foreach (var device in devices)
            {
                foreach (var property in device.Properties)
                {
                    var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
                    if (IsPropertyValid(property, driverProperty))
                    {
                        MessageBoxService.Show("Устройство " + device.PresentationDriverAndAddress + "\nЗначение параметра\n" + driverProperty.Caption + "\nдолжно быть целым числом " + "в диапазоне от " + driverProperty.Min.ToString() + " до " + driverProperty.Max.ToString(), "Firesec");
                        return false;
                    }
                }
            }
            return true;
        }

        static bool IsPropertyValid(XProperty property, XDriverProperty driverProperty)
        {
            int value;
            return
                    driverProperty != null &&
                    driverProperty.IsAUParameter &&
                    driverProperty.DriverPropertyType == XDriverPropertyTypeEnum.IntType &&
                    (!int.TryParse(Convert.ToString(property.Value), out value) ||
                    (value < driverProperty.Min || value > driverProperty.Max));
        }


        public RelayCommand ReadCommand { get; private set; }
        void OnRead()
        {
            if (CheckNeedSave())
            {
                ReadDevices(new List<XDevice> { Device });
                PropertiesViewModel.Update();
                ServiceFactory.SaveService.GKChanged = true;
            }
        }

        bool CanReadWrite()
        {
            return HasAUProperties;
        }

        public RelayCommand ReadAllCommand { get; private set; }
        void OnReadAll()
        {
            if (CheckNeedSave())
            {
                var devices = GetRealChildren();
                devices.Add(Device);
                ReadDevices(devices);
            }
        }

        bool CanReadWriteAll()
        {
            return Children.Count() > 0;
        }

        public bool HasAUProperties
        {
            get { return Device.Driver.Properties.Count(x => x.IsAUParameter) > 0; }
        }

        static void ReadDevices(IEnumerable<XDevice> devices)
        {
            ParametersHelper.ErrorLog = "";
            LoadingService.Show("Запрос параметров");
            DatabaseManager.Convert();
            var i = 0;
            LoadingService.AddCount(devices.Count());
            foreach (var device in devices)
            {
                i++;
                //FiresecDriverAuParametersHelper_Progress("Чтение параметров устройства " + device.PresentationDriverAndAddress, (i * 100) / devices.Count);
                ParametersHelper.GetSingleParameter(device);
            }
            LoadingService.Close();
            if (ParametersHelper.ErrorLog != "")
                MessageBoxService.ShowError("Ошибка при получении параметров следующих устройств:" + ParametersHelper.ErrorLog);
            //FiresecDriverAuParametersHelper_Progress("Чтение параметров устройства ", 0);
            ServiceFactory.SaveService.GKChanged = true;
        }

        static void WriteDevices(IEnumerable<XDevice> devices)
        {
            ParametersHelper.ErrorLog = "";
            LoadingService.Show("Запись параметров");
            DatabaseManager.Convert();
            var i = 0;
            LoadingService.AddCount(devices.Count());
            foreach (var device in devices)
            {
                i++;
                //FiresecDriverAuParametersHelper_Progress("Запись параметров в устройство " + device.PresentationDriverAndAddress, (i * 100) / devices.Count);
                ParametersHelper.SetSingleParameter(device);
                Thread.Sleep(100);
            }
            LoadingService.Close();
            if (ParametersHelper.ErrorLog != "")
                MessageBoxService.ShowError("Ошибка при записи параметров в следующие устройства:" + ParametersHelper.ErrorLog);
            //FiresecDriverAuParametersHelper_Progress("Запись параметров в устройство ", 0);
            ServiceFactory.SaveService.GKChanged = true;
        }

        public RelayCommand ResetAUPropertiesCommand { get; private set; }
        void OnResetAUProperties()
        {
            foreach (var property in Device.Properties)
            {
                var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
                if (driverProperty != null)
                {
                    property.Value = driverProperty.Default;
                }
            }
            PropertiesViewModel = new PropertiesViewModel(Device);
            OnPropertyChanged("PropertiesViewModel");
        }
    }
}
