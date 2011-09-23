﻿using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;

namespace FiresecService
{
    public static class ConfigurationConverter
    {
        public static Firesec.CoreConfiguration.config FiresecConfiguration { get; set; }
        public static DeviceConfiguration DeviceConfiguration { get; set; }

        public static void Convert()
        {
            FiresecConfiguration = FiresecInternalClient.GetCoreConfig();
            DeviceConfiguration = new DeviceConfiguration();
            ConvertConfiguration();

            ConfigurationFileManager.SetDeviceConfiguration(DeviceConfiguration);
            ConfigurationFileManager.SetSecurityConfiguration(FiresecManager.SecurityConfiguration);

            var plans = FiresecInternalClient.GetPlans();
            var plansConfiguration = PlansConverter.Convert(plans);

            ConfigurationFileManager.SetPlansConfiguration(plansConfiguration);

            FiresecManager.DeviceConfiguration = DeviceConfiguration;
        }

        static void ConvertConfiguration()
        {
            DeviceConfiguration = new DeviceConfiguration();
            ZoneConverter.Convert();
            DirectionConverter.Convert();
            GuardUserConverter.Convert();
            SecurityConverter.Convert();
            DeviceConverter.Convert();
        }

        public static void ConvertBack(DeviceConfiguration deviceConfiguration)
        {
            deviceConfiguration.Update();

            foreach (var device in deviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
            }
            FiresecConfiguration = FiresecInternalClient.GetCoreConfig();
            FiresecConfiguration.part = null;
            FiresecConfiguration.secGUI = null;
            FiresecConfiguration.secObjType = null;
            FiresecConfiguration.user = null;
            FiresecConfiguration.userGroup = null;
            FiresecConfiguration.zone = null;

            ZoneConverter.ConvertBack(deviceConfiguration);
            DeviceConverter.ConvertBack(deviceConfiguration);
            DirectionConverter.ConvertBack(deviceConfiguration);
            GuardUserConverter.ConvertBack(deviceConfiguration);
        }
    }
}