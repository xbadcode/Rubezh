﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;
using FiresecMetadata;

namespace ServiseProcessor
{
    public class ConfigToFiresec
    {
        public Firesec.CoreConfig.config Convert(CurrentConfiguration configuration)
        {
            SetZones(configuration);

            Device rootDevice = configuration.RootDevice;
            Firesec.CoreConfig.devType rootInnerDevice = DeviceToInnerDevice(rootDevice);
            AddInnerDevice(rootDevice, rootInnerDevice);

            Services.CoreConfig.dev = new Firesec.CoreConfig.devType[1];
            Services.CoreConfig.dev[0] = rootInnerDevice;
            return Services.CoreConfig;
        }

        void AddInnerDevice(Device parentDevice, Firesec.CoreConfig.devType parentInnerDevice)
        {
            List<Firesec.CoreConfig.devType> childInnerDevices = new List<Firesec.CoreConfig.devType>();

            foreach (Device device in parentDevice.Children)
            {
                Firesec.CoreConfig.devType childInnerDevice = DeviceToInnerDevice(device);
                childInnerDevices.Add(childInnerDevice);
                AddInnerDevice(device, childInnerDevice);
            }
            parentInnerDevice.dev = childInnerDevices.ToArray();
        }

        Firesec.CoreConfig.devType DeviceToInnerDevice(Device device)
        {
            Firesec.CoreConfig.devType innerDevice = new Firesec.CoreConfig.devType();
            innerDevice.drv = Services.CoreConfig.drv.FirstOrDefault(x => x.id == device.DriverId).idx;
            innerDevice.addr = ConvertAddress(device);

            if (device.ZoneNo != null)
            {
                List<Firesec.CoreConfig.inZType> zones = new List<Firesec.CoreConfig.inZType>();
                zones.Add(new Firesec.CoreConfig.inZType() { idz = device.ZoneNo });
                innerDevice.inZ = zones.ToArray();
            }

            innerDevice.prop = AddProperties(device).ToArray();

            return innerDevice;
        }

        string ConvertAddress(Device device)
        {
            if (string.IsNullOrEmpty(device.Address))
                return "0";

            if (device.Address.Contains("."))
            {
                List<string> addresses = device.Address.Split(new char[] { '.' }, StringSplitOptions.None).ToList();

                int intShleifAddress = System.Convert.ToInt32(addresses[0]);
                int intAddress = System.Convert.ToInt32(addresses[1]);
                return (intShleifAddress * 256 + intAddress).ToString();
            }

            return device.Address;
        }

        void SetZones(CurrentConfiguration currentConfiguration)
        {
            Services.CoreConfig.zone = new Firesec.CoreConfig.zoneType[currentConfiguration.Zones.Count];
            for (int i = 0; i < currentConfiguration.Zones.Count; i++)
            {
                Services.CoreConfig.zone[i] = new Firesec.CoreConfig.zoneType();
                Services.CoreConfig.zone[i].name = currentConfiguration.Zones[i].Name;
                Services.CoreConfig.zone[i].idx = currentConfiguration.Zones[i].No;
                Services.CoreConfig.zone[i].no = currentConfiguration.Zones[i].No;
                if (!string.IsNullOrEmpty(currentConfiguration.Zones[i].Description))
                    Services.CoreConfig.zone[i].desc = currentConfiguration.Zones[i].Description;

                List<Firesec.CoreConfig.paramType> zoneParams = new List<Firesec.CoreConfig.paramType>();
                if (!string.IsNullOrEmpty(currentConfiguration.Zones[i].DetectorCount))
                {
                    Firesec.CoreConfig.paramType DetectorCountZoneParam = new Firesec.CoreConfig.paramType();
                    DetectorCountZoneParam.name = "FireDeviceCount";
                    DetectorCountZoneParam.type = "Int";
                    DetectorCountZoneParam.value = currentConfiguration.Zones[i].DetectorCount;
                    zoneParams.Add(DetectorCountZoneParam);
                }
                if (!string.IsNullOrEmpty(currentConfiguration.Zones[i].EvacuationTime))
                {
                    Firesec.CoreConfig.paramType EvacuationTimeZoneParam = new Firesec.CoreConfig.paramType();
                    EvacuationTimeZoneParam.name = "ExitTime";
                    EvacuationTimeZoneParam.type = "SmallInt";
                    EvacuationTimeZoneParam.value = currentConfiguration.Zones[i].EvacuationTime;
                    zoneParams.Add(EvacuationTimeZoneParam);
                }
                if (zoneParams.Count > 0)
                    Services.CoreConfig.zone[i].param = zoneParams.ToArray();
            }
        }

        List<Firesec.CoreConfig.propType> AddProperties(Device device)
        {
            List<Firesec.CoreConfig.propType> propertyList = new List<Firesec.CoreConfig.propType>();

            string driverName = DriversHelper.GetDriverNameById(device.DriverId);
            if (driverName != "Компьютер")
            {
                if (device.Properties != null)
                {
                    if (device.Properties.Count > 0)
                    {
                        foreach (Property deviceProperty in device.Properties)
                        {
                            if ((!string.IsNullOrEmpty(deviceProperty.Name)) && (!string.IsNullOrEmpty(deviceProperty.Value)))
                            {
                                Firesec.Metadata.drvType metadataDriver = Services.CurrentConfiguration.Metadata.drv.First(x => x.id == device.DriverId);
                                if (metadataDriver.propInfo != null)
                                {
                                    if (metadataDriver.propInfo.Any(x => x.name == deviceProperty.Name))
                                    {
                                        Firesec.CoreConfig.propType property = new Firesec.CoreConfig.propType();
                                        property.name = deviceProperty.Name;
                                        property.value = deviceProperty.Value;
                                        propertyList.Add(property);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return propertyList;
        }
    }
}
