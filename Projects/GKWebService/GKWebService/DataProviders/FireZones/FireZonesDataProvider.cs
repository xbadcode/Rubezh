﻿using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System.Linq;

namespace GKWebService.DataProviders.FireZones
{
    public class FireZonesDataProvider
    {
        private FireZonesDataProvider()
        {
            ClientManager.GetConfiguration("GKOPC/Configuration");
		}

        public GKZone GetFireZones()
        {
            var gkStates = ClientManager.FiresecService.GKGetStates();
            
            foreach (var remoteZoneState in gkStates.ZoneStates)
            {
                GKZone zone = GKManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
                if (zone != null)
                {
                    remoteZoneState.CopyTo(zone.State);
                    FireZonesUpdater.Instance.StartTestBroadcast();
                    return zone;
                }
            }
            return null;
        }
       

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

        private static FireZonesDataProvider _instance;
    }
}