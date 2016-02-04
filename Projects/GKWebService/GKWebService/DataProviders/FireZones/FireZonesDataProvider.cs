﻿using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System.Linq;
using GKWebService.Models.FireZone;
using GKWebService.Utils;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Controls.Converters;
using GKWebService.Models;

namespace GKWebService.DataProviders.FireZones
{
    public class FireZonesDataProvider
    {
        /// <summary>
        /// Инстанс провайдера данных
        /// </summary>
        public static FireZonesDataProvider Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                return _instance = new FireZonesDataProvider();
            }
        }

        /// <summary>
        /// Метод, предоставляющий данные 
        /// для вкладки "Пожарные зоны"
        /// </summary>
        /// <returns>Список зон с соответствующими устройствами</returns>
        public List<FireZone> GetFireZones()
        {
            //Формируем список зон со всеми необходимыми данными
            List<FireZone> list = new List<FireZone>();

            //Проходимся по списку всех пожарных зон
            foreach (var currentZone in GKManager.Zones)
            {
                //Получаем зону
                var zone = currentZone;
                //Заносим в нее статус
                var gkStates = ClientManager.FiresecService.GKGetStates();
                foreach (var remoteZoneState in gkStates.ZoneStates)
                {
                    if (currentZone.UID == remoteZoneState.UID)
                    {
                        remoteZoneState.CopyTo(zone.State);
                        break;
                    }
                }

                list.Add(new FireZone
                {
                    StateIcon = "/Content/Image/Icon/GKStateIcons/" + Convert.ToString(zone.State.StateClasses[0]) + ".png",
                    Name = zone.DescriptorPresentationName,
                    Fire1Count = zone.Fire1Count,
                    Fire2Count = zone.Fire2Count,
                    ImageSource = "/Content/Image/" + zone.ImageSource.Replace("/Controls;component/", ""),
                    Uid = zone.UID, 
                    No = zone.No, 
                    StateColor = "'#" + new XStateClassToColorConverter2().Convert(zone.State.StateClass, null, null, null).ToString().Substring(3) + "'",
                    StateMessage = zone.State.StateClass.ToDescription(),
                    CanTurnOff = Convert.ToString(zone.State.StateClasses[0]) == "Norm",
                    CanTurnOn = Convert.ToString(zone.State.StateClasses[0]) == "Ignore",
                    CanReset = Convert.ToString(zone.State.StateClasses[0]) == "Norm", 
                    GKDescriptorNo = zone.GKDescriptorNo
                });
            }
            return list;
        }


        public List<DeviceNode> GetDevicesByZone(int zoneNumber)
        {
            List<DeviceNode> listTree = new List<DeviceNode>();

            DeviceNode data = new DeviceNode();

            int level = 0;

            if (GKManager.Zones.Count - 1 < zoneNumber)
            {
                return null;
            }

            foreach (var remoteDevice in GKManager.Zones[zoneNumber].Devices)
            {
				data.DeviceList.Add(new Device(remoteDevice)
                {
                    Level = level
                });
            }

            listTree.Add(data);
            var device = GKManager.Zones[zoneNumber].Devices.FirstOrDefault();

            while (device != null && device.Parent != null)
            {
                level++;
                DeviceNode item = new DeviceNode();
                device = device.Parent;
				item.DeviceList.Add(new Device(device)
				{
					Level = level
				});
                listTree.Add(item);
            }

            listTree.Reverse();
            return listTree;
        }

        /// <summary>
        /// private instance of DataProvider
        /// </summary>
        private static FireZonesDataProvider _instance;
    }
}