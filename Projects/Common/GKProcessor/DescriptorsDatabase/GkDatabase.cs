﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class GkDatabase : CommonDatabase
	{
		List<GKDoor> Doors { get; set; }
		public List<KauDatabase> KauDatabases { get; set; }

		public GkDatabase(GKDevice gkControllerDevice)
		{
			Doors = new List<GKDoor>();
			KauDatabases = new List<KauDatabase>();
			DatabaseType = DatabaseType.Gk;
			RootDevice = gkControllerDevice;

			AddDevice(gkControllerDevice);
			foreach (var device in gkControllerDevice.Children)
			{
				if (device.DriverType == GKDriverType.GKIndicator || device.DriverType == GKDriverType.GKRele)
				{
					AddDevice(device);
				}
			}
			Devices.ForEach(x => x.GkDatabaseParent = RootDevice);
		}

		void AddDevice(GKDevice device)
		{
			if (!Devices.Contains(device))
			{
				Devices.Add(device);
			}
		}

		public override void BuildObjects()
		{
			Descriptors = new List<BaseDescriptor>();

			AddKauDevices();

			foreach (var device in Devices)
			{
				var deviceDescriptor = new DeviceDescriptor(device, DatabaseType);
				Descriptors.Add(deviceDescriptor);
			}

			AddKauLogicObjects();

			foreach (var zone in GKManager.Zones.Where(x => x.GkDatabaseParent == RootDevice && x.KauDatabaseParent == null))
			{
				var zoneDescriptor = new ZoneDescriptor(zone, DatabaseType.Gk);
				Descriptors.Add(zoneDescriptor);
			}

			foreach (var guardZone in GKManager.GuardZones.Where(x => x.GkDatabaseParent == RootDevice && x.KauDatabaseParent == null))
			{
				var guardZoneDescriptor = new GuardZoneDescriptor(guardZone, DatabaseType.Gk);
				Descriptors.Add(guardZoneDescriptor);

				if (guardZoneDescriptor.GuardZonePimDescriptor != null)
				{
					Descriptors.Add(guardZoneDescriptor.GuardZonePimDescriptor);
				}
			}

			foreach (var direction in GKManager.Directions.Where(x => x.GkDatabaseParent == RootDevice && x.KauDatabaseParent == null))
			{
				var directionDescriptor = new DirectionDescriptor(direction, DatabaseType.Gk);
				Descriptors.Add(directionDescriptor);
			}

			foreach (var delay in GKManager.Delays.Where(x => x.GkDatabaseParent == RootDevice && x.KauDatabaseParent == null))
			{
				var delayDescriptor = new DelayDescriptor(delay, DatabaseType.Gk);
				Descriptors.Add(delayDescriptor);
			}

			foreach (var pumpStation in GKManager.PumpStations.Where(x => x.GkDatabaseParent == RootDevice && x.KauDatabaseParent == null))
			{
				var pumpStationDescriptor = new PumpStationDescriptor(this, pumpStation, DatabaseType.Gk);
				Descriptors.Add(pumpStationDescriptor);

				var pumpStationCreator = new PumpStationCreator(this, pumpStation, DatabaseType.Gk);
				pumpStationCreator.Create();
			}

			foreach (var mpt in GKManager.DeviceConfiguration.MPTs.Where(x => x.GkDatabaseParent == RootDevice && x.KauDatabaseParent == null))
			{
				var mptDescriptor = new MPTDescriptor(this, mpt, DatabaseType.Gk);
				Descriptors.Add(mptDescriptor);

				var mptCreator = new MPTCreator(mpt);
			}

			foreach (var code in GKManager.DeviceConfiguration.Codes.Where(x => x.GkDatabaseParent == RootDevice && x.KauDatabaseParent == null))
			{
				var codeDescriptor = new CodeDescriptor(code, DatabaseType.Gk);
				Descriptors.Add(codeDescriptor);
			}

			foreach (var door in GKManager.Doors)
			{
				var doorDescriptor = new DoorDescriptor(door);
				Descriptors.Add(doorDescriptor);
				if (doorDescriptor.DoorPimDescriptorEnter != null)
				{
					Descriptors.Add(doorDescriptor.DoorPimDescriptorEnter);
				}
				if (doorDescriptor.DoorPimDescriptorExit != null)
				{
					Descriptors.Add(doorDescriptor.DoorPimDescriptorExit);
				}
			}

			ushort no = 1;
			foreach (var descriptor in Descriptors)
			{
				descriptor.GKBase.GKDescriptorNo = no++;
				descriptor.DatabaseType = DatabaseType.Gk;
			}
			foreach (var descriptor in Descriptors)
			{
				descriptor.Build();
				if (!descriptor.Formula.IsGeneratedOutside)
				{
					descriptor.BuildFormula();
				}
				descriptor.Formula.Resolve(DatabaseType);
				descriptor.FormulaBytes = descriptor.Formula.GetBytes();
				descriptor.InitializeAllBytes();
			}
		}

		void AddKauDevices()
		{
			foreach (var kauDatabase in KauDatabases)
			{
				foreach (var descriptor in kauDatabase.Descriptors)
				{
					var gkBase = descriptor.GKBase;
					gkBase.GkDatabaseParent = RootDevice;
					if (gkBase is GKDevice)
					{
						GKDevice device = gkBase as GKDevice;
						AddDevice(device);
					}
				}
			}
		}

		void AddKauLogicObjects()
		{
			foreach (var kauDatabase in KauDatabases)
			{
				foreach (var descriptor in kauDatabase.Descriptors)
				{
					var gkBase = descriptor.GKBase;
					gkBase.GkDatabaseParent = RootDevice;

					if(gkBase is GKZone)
						Descriptors.Add(new ZoneDescriptor(gkBase as GKZone, DatabaseType));

					if (gkBase is GKDirection)
						Descriptors.Add(new DirectionDescriptor(gkBase as GKDirection, DatabaseType));

					if (gkBase is GKDelay)
						Descriptors.Add(new DelayDescriptor(gkBase as GKDelay, DatabaseType));

					if (gkBase is GKPumpStation)
						Descriptors.Add(new PumpStationDescriptor(this, gkBase as GKPumpStation, DatabaseType));

					if (gkBase is GKMPT)
						Descriptors.Add(new MPTDescriptor(this, gkBase as GKMPT, DatabaseType));

					if (gkBase is GKPim)
						Descriptors.Add(new PimDescriptor(gkBase as GKPim, DatabaseType));

					if (gkBase is GKCode)
						Descriptors.Add(new CodeDescriptor(gkBase as GKCode, DatabaseType));

					if (gkBase is GKGuardZone)
						Descriptors.Add(new GuardZoneDescriptor(gkBase as GKGuardZone, DatabaseType));
				}
			}
		}
	}
}