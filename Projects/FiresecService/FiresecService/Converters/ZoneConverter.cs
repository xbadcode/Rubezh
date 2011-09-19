﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Firesec.CoreConfiguration;

namespace FiresecService.Converters
{
    public static class ZoneConverter
    {
        public static void Convert()
        {
            ConfigurationConverter.DeviceConfiguration.Zones = new List<Zone>();

            if (ConfigurationConverter.FiresecConfiguration.zone != null)
            {
                foreach (var innerZone in ConfigurationConverter.FiresecConfiguration.zone)
                {
                    var zone = new Zone()
                    {
                        Name = innerZone.name,
                        No = innerZone.no,
                        Description = innerZone.desc
                    };

                    if (innerZone.shape != null)
                    {
                        zone.ShapeIds = new List<string>();
                        foreach (var shape in innerZone.shape)
                        {
                            zone.ShapeIds.Add(shape.id);
                        }
                    }

                    if (innerZone.param != null)
                    {
                        var zoneTypeParam = innerZone.param.FirstOrDefault(x => x.name == "ZoneType");
                        if (zoneTypeParam != null)
                        {
                            zone.ZoneType = (zoneTypeParam.value == "0") ? ZoneType.Fire : ZoneType.Guard;
                        }

                        var exitTimeParam = innerZone.param.FirstOrDefault(x => x.name == "ExitTime");
                        if (exitTimeParam != null)
                            zone.EvacuationTime = exitTimeParam.value;

                        var fireDeviceCountParam = innerZone.param.FirstOrDefault(x => x.name == "FireDeviceCount");
                        if (fireDeviceCountParam != null)
                            zone.DetectorCount = fireDeviceCountParam.value;

                        var autoSetParam = innerZone.param.FirstOrDefault(x => x.name == "AutoSet");
                        if (autoSetParam != null)
                            zone.AutoSet = autoSetParam.value;

                        var delayParam = innerZone.param.FirstOrDefault(x => x.name == "Delay");
                        if (delayParam != null)
                            zone.Delay = delayParam.value;

                        var skippedParam = innerZone.param.FirstOrDefault(x => x.name == "Skipped");
                        if (skippedParam != null)
                            zone.Skipped = skippedParam.value == "1" ? true : false;

                        var guardZoneTypeParam = innerZone.param.FirstOrDefault(x => x.name == "GuardZoneType");
                        if (guardZoneTypeParam != null)
                        {
                            zone.GuardZoneType = (GuardZoneType)int.Parse(guardZoneTypeParam.value);
                        }
                    }
                    ConfigurationConverter.DeviceConfiguration.Zones.Add(zone);
                }
            }
        }

        public static void ConvertBack(DeviceConfiguration deviceConfiguration)
        {
            var innerZones = new List<zoneType>();
            foreach (var zone in deviceConfiguration.Zones)
            {
                var innerZone = new zoneType()
                {
                    name = zone.Name,
                    idx = zone.No,
                    no = zone.No
                };

                if (string.IsNullOrEmpty(zone.Description) == false)
                    innerZone.desc = zone.Description;

                var zoneParams = new List<paramType>();

                zoneParams.Add(new paramType()
                {
                    name = "ZoneType",
                    type = "Int",
                    value = (zone.ZoneType == ZoneType.Fire) ? "0" : "1"
                });

                if (string.IsNullOrEmpty(zone.DetectorCount) == false)
                {
                    zoneParams.Add(new paramType()
                    {
                        name = "FireDeviceCount",
                        type = "Int",
                        value = zone.DetectorCount
                    });
                }

                if (string.IsNullOrEmpty(zone.EvacuationTime) == false)
                {
                    zoneParams.Add(new paramType()
                    {
                        name = "ExitTime",
                        type = "SmallInt",
                        value = zone.EvacuationTime
                    });
                }

                if (string.IsNullOrEmpty(zone.AutoSet) == false)
                {
                    zoneParams.Add(new paramType()
                    {
                        name = "AutoSet",
                        type = "Int",
                        value = zone.AutoSet
                    });
                }

                if (string.IsNullOrEmpty(zone.Delay) == false)
                {
                    zoneParams.Add(new paramType()
                    {
                        name = "Delay",
                        type = "Int",
                        value = zone.Delay
                    });
                }

                if (true)
                {
                    zoneParams.Add(new paramType()
                    {
                        name = "Skipped",
                        type = "Int",
                        value = zone.Skipped ? "1" : "0"
                    });
                }

                string GuardZoneTypeParamString = ((int)zone.GuardZoneType).ToString();

                zoneParams.Add(new paramType()
                {
                    name = "GuardZoneType",
                    value = GuardZoneTypeParamString
                });

                if (zoneParams.Count > 0)
                    innerZone.param = zoneParams.ToArray();

                innerZones.Add(innerZone);
            }

            if (innerZones.Count > 0)
                ConfigurationConverter.FiresecConfiguration.zone = innerZones.ToArray();
            else
                ConfigurationConverter.FiresecConfiguration.zone = null;
        }
    }
}