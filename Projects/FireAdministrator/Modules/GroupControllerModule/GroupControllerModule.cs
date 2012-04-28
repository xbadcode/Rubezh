﻿using FiresecClient;
using GKModule.Converter;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace GKModule
{
    public class GroupControllerModule
    {
        static DevicesViewModel _devicesViewModel;
        static ZonesViewModel _zonesViewModel;

        public GroupControllerModule()
        {
            if (ServiceFactory.AppSettings.ShowGK == false)
                return;

            ServiceFactory.Events.GetEvent<ShowXDevicesEvent>().Unsubscribe(OnShowXDevices);
            ServiceFactory.Events.GetEvent<ShowXZonesEvent>().Unsubscribe(OnShowXZones);
            ServiceFactory.Events.GetEvent<ShowXDevicesEvent>().Subscribe(OnShowXDevices);
            ServiceFactory.Events.GetEvent<ShowXZonesEvent>().Subscribe(OnShowXZones);

            DriversConverter.Convert();
            XManager.DeviceConfiguration = FiresecManager.FiresecService.GetXDeviceConfiguration();
            XManager.UpdateConfiguration();

            //XManager.SetEmptyConfiguration();

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void CreateViewModels()
        {
            if (ServiceFactory.AppSettings.ShowGK == false)
                return;

            _devicesViewModel = new DevicesViewModel();
            _zonesViewModel = new ZonesViewModel();
        }

        static void OnShowXDevices(object obj)
        {
            ServiceFactory.Layout.Show(_devicesViewModel);
        }

        static void OnShowXZones(object obj)
        {
            ServiceFactory.Layout.Show(_zonesViewModel);
        }
    }
}