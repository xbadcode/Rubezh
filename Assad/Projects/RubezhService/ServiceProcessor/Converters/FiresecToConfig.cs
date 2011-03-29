﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;
using FiresecMetadata;
using Firesec;

namespace ServiseProcessor
{
    public class FiresecToConfig
    {
        CurrentConfiguration stateConfiguration;
        Firesec.CoreConfig.config firesecConfig;

        public CurrentConfiguration Convert(Firesec.CoreConfig.config firesecConfig)
        {
            this.firesecConfig = firesecConfig;
            stateConfiguration = new CurrentConfiguration();

            Services.AllDevices = new List<Device>();
            Services.CurrentStates = new CurrentStates();
            Services.CurrentStates.DeviceStates = new List<DeviceState>();
            Services.CurrentStates.ZoneStates = new List<ZoneState>();

            AddZones();

            Firesec.CoreConfig.devType rootInnerDevice = Services.CoreConfig.dev[0];
            Device rootDevice = new Device();
            rootDevice.Parent = null;
            SetInnerDevice(rootDevice, rootInnerDevice);
            Services.AllDevices.Add(rootDevice);
            Services.CurrentStates.DeviceStates.Add(CreateDeviceState(rootDevice));
            AddDevice(rootInnerDevice, rootDevice);

            stateConfiguration.RootDevice = rootDevice;

            return stateConfiguration;
        }

        void AddDevice(Firesec.CoreConfig.devType parentInnerDevice, Device parentDevice)
        {
            parentDevice.Children = new List<Device>();
            if (parentInnerDevice.dev != null)
            {
                foreach (Firesec.CoreConfig.devType innerDevice in parentInnerDevice.dev)
                {
                    Device device = new Device();
                    device.Parent = parentDevice;
                    SetInnerDevice(device, innerDevice);
                    
                    parentDevice.Children.Add(device);
                    Services.AllDevices.Add(device);
                    Services.CurrentStates.DeviceStates.Add(CreateDeviceState(device));
                    AddDevice(innerDevice, device);
                }
            }
        }

        DeviceState CreateDeviceState(Device device)
        {
            Firesec.Metadata.drvType metadataDriver = Services.CurrentConfiguration.Metadata.drv.First(x => x.id == device.DriverId);

            DeviceState deviceState = new DeviceState();
            deviceState.ChangeEntities = new ChangeEntities();
            deviceState.Path = device.Path;
            deviceState.PlaceInTree = device.PlaceInTree;

            deviceState.InnerStates = new List<InnerState>();
            foreach (Firesec.Metadata.stateType innerState in metadataDriver.state)
            {
                InnerState state = new InnerState()
                {
                    Id = innerState.id,
                    Name = innerState.name,
                    Priority = System.Convert.ToInt32(innerState.@class),
                    AffectChildren = innerState.affectChildren == "1" ? true : false
                };
                deviceState.InnerStates.Add(state);
            }

            return deviceState;
        }

        void SetInnerDevice(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            device.DriverId = firesecConfig.drv.FirstOrDefault(x => x.idx == innerDevice.drv).id;
            Firesec.Metadata.drvType metadataDriver = Services.CurrentConfiguration.Metadata.drv.First(x => x.id == device.DriverId);

            // addrMask="[8(1)-15(2)];[0(1)-7(255)]"
            device.Address = innerDevice.addr;
            if (metadataDriver.addrMask != null)
            {
                int intAddress = System.Convert.ToInt32(device.Address);
                if (intAddress > 255)
                {
                    int intShleifAddress = intAddress / 255;
                    int intSelfAddress = intAddress % 256;
                    device.Address = intShleifAddress.ToString() + "." + intSelfAddress.ToString();
                }
            }

            if (innerDevice.param != null)
            {
                if (innerDevice.param.Any(x => x.name == "DB$IDDevices"))
                {
                    device.DatabaseId = innerDevice.param.FirstOrDefault(x => x.name == "DB$IDDevices").value;
                }
            }

            device.Properties = new List<Property>();
            if (innerDevice.prop != null)
            {
                foreach (Firesec.CoreConfig.propType innerProperty in innerDevice.prop)
                {
                    Property deviceProperty = new Property();
                    deviceProperty.Name = innerProperty.name;
                    deviceProperty.Value = innerProperty.value;
                    device.Properties.Add(deviceProperty);
                }
            }

            device.Description = innerDevice.name;

            SetAddress(device, innerDevice);
            SetPath(device);
            SetPlaceInTree(device);
            SetZone(device, innerDevice);
        }

        void SetAddress(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            device.PresentationAddress = device.Address;
            string DriverName = DriversHelper.GetDriverNameById(device.DriverId);

            switch (DriverName)
            {
                case "Компьютер":
                    device.Address = "Компьютер";
                    device.PresentationAddress = "";
                    break;

                case "Насосная Станция":
                    device.Address = "Насосная Станция";
                    device.PresentationAddress = "";
                    break;

                case "Жокей-насос":
                    device.Address = "Жокей-насос";
                    device.PresentationAddress = "";
                    break;

                case "Компрессор":
                    device.Address = "Компрессор";
                    device.PresentationAddress = "";
                    break;

                case "Дренажный насос":
                    device.Address = "Дренажный насос";
                    device.PresentationAddress = "";
                    break;

                case "Насос компенсации утечек":
                    device.Address = "Насос компенсации утечек";
                    device.PresentationAddress = "";
                    break;

                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                    if (innerDevice.prop != null)
                    {
                        if (innerDevice.prop.Any(x => x.name == "SerialNo"))
                            device.Address = innerDevice.prop.FirstOrDefault(x => x.name == "SerialNo").value;
                    }
                    else
                        device.Address = "0";
                    device.PresentationAddress = "";
                    break;
            }
        }

        void SetPath(Device device)
        {
            string currentPath = device.DriverId + ":" + device.Address;
            if (device.Parent != null)
            {
                device.Path = device.Parent.Path + @"/" + currentPath;
            }
            else
            {
                device.Path = currentPath;
            }
        }

        void SetPlaceInTree(Device device)
        {
            if (device.Parent == null)
            {
                device.PlaceInTree = "";
            }
            else
            {
                string localPlaceInTree = (device.Parent.Children.Count - 1).ToString();
                if (device.Parent.PlaceInTree == "")
                {
                    device.PlaceInTree = localPlaceInTree;
                }
                else
                {
                    device.PlaceInTree = device.Parent.PlaceInTree + @"\" + localPlaceInTree;
                }
            }
        }

        void SetZone(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            if (innerDevice.inZ != null)
            {
                string zoneIdx = innerDevice.inZ[0].idz;
                string zoneNo = firesecConfig.zone.FirstOrDefault(x => x.idx == zoneIdx).no;
                device.ZoneNo = zoneNo;
            }
            if (innerDevice.prop != null)
            {
                if (innerDevice.prop.Any(x => x.name == "ExtendedZoneLogic"))
                {
                    string zoleLogicstring = innerDevice.prop.FirstOrDefault(x => x.name == "ExtendedZoneLogic").value;
                    Firesec.ZoneLogic.expr ZoneLogic = FiresecClient.GetZoneLogic(zoleLogicstring);
                    if (ZoneLogic != null)
                    {
                        string logic = "";

                        foreach (Firesec.ZoneLogic.clauseType clause in ZoneLogic.clause)
                        {
                            if (clause.joinOperator != null)
                            {
                                switch (clause.joinOperator)
                                {
                                    case "and":
                                        logic += " и ";
                                        break;
                                    case "or":
                                        logic += " или ";
                                        break;
                                }
                            }

                            string stringState = "";
                            switch (clause.state)
                            {
                                case "0":
                                    stringState = "Включение автоматики";
                                    break;

                                case "1":
                                    stringState = "Тревога";
                                    break;

                                case "2":
                                    stringState = "Пожар";
                                    break;

                                case "5":
                                    stringState = "Внимание";
                                    break;

                                case "6":
                                    stringState = "Включение модуля пожаротушения";
                                    break;
                            }

                            string stringOperation = "";
                            switch (clause.operation)
                            {
                                case "and":
                                    stringOperation = "во всех зонах из";
                                    break;

                                case "or":
                                    stringOperation = "в любой зонах из";
                                    break;
                            }

                            logic += "состояние " + stringState + " " + stringOperation + " [";

                            foreach (string zoneNo in clause.zone)
                            {
                                logic += zoneNo + ", ";
                            }
                            logic += "]";
                        }
                    }
                }
            }
        }

        void AddZones()
        {
            stateConfiguration.Zones = new List<Zone>();

            if (firesecConfig.zone != null)
            {
                foreach (Firesec.CoreConfig.zoneType innerZone in firesecConfig.zone)
                {
                    Zone zone = new Zone();
                    zone.Name = innerZone.name;
                    zone.No = innerZone.no;
                    zone.Description = innerZone.desc;
                    if (innerZone.param != null)
                    {
                        if (innerZone.param.Any(x => x.name == "ExitTime"))
                            zone.EvacuationTime = innerZone.param.FirstOrDefault(x => x.name == "ExitTime").value;
                        if (innerZone.param.Any(x => x.name == "FireDeviceCount"))
                            zone.DetectorCount = innerZone.param.FirstOrDefault(x => x.name == "FireDeviceCount").value;
                    }
                    stateConfiguration.Zones.Add(zone);
                }
            }
        }
    }
}
