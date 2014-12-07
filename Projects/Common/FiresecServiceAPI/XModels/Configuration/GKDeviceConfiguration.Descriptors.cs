﻿using System.Collections.Generic;
using System.Linq;
using FiresecClient;

namespace FiresecAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void PrepareDescriptors()
		{
			PrepareZones();
			PrepareInputOutputDependences();
			PrepareDirections();
			PreparePumpStations();
			PrepareMPTs();
			PrepareDelays();
			PrepareGuardZones();
			PrepareCodes();
			PrepareDoors();
		}

		void PrepareZones()
		{
			foreach (var zone in Zones)
			{
				zone.KauDatabaseParent = null;
				zone.GkDatabaseParent = null;

				var gkParents = new HashSet<GKDevice>();
				foreach (var device in zone.Devices)
				{
					var gkParent = device.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
					gkParents.Add(gkParent);
				}

				var gkControllerDevice = gkParents.FirstOrDefault();
				if (gkControllerDevice != null)
				{
					zone.GkDatabaseParent = gkControllerDevice;
				}
			}
		}

		void PrepareInputOutputDependences()
		{
			foreach (var device in Devices)
			{
				device.ClearDescriptor();
			}
			foreach (var zone in Zones)
			{
				zone.ClearDescriptor();
			}
			foreach (var direction in Directions)
			{
				direction.ClearDescriptor();
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.ClearDescriptor();
			}
			foreach (var mpt in MPTs)
			{
				mpt.ClearDescriptor();
			}
			foreach (var delay in Delays)
			{
				delay.ClearDescriptor();
			}
			foreach (var guardZone in GuardZones)
			{
				guardZone.ClearDescriptor();
			}
			foreach (var code in Codes)
			{
				code.ClearDescriptor();
			}
			foreach (var door in Doors)
			{
				door.ClearDescriptor();
			}

			foreach (var device in Devices)
			{
				LinkLogic(device, device.Logic.OnClausesGroup);
				LinkLogic(device, device.Logic.OffClausesGroup);
				LinkLogic(device, device.Logic.StopClausesGroup);
			}

			foreach (var zone in Zones)
			{
				foreach (var device in zone.Devices)
				{
					LinkGKBases(zone, device);
				}
				LinkGKBases(zone, zone);
			}

			foreach (var direction in Directions)
			{
				LinkLogic(direction, direction.Logic.OnClausesGroup);
				LinkLogic(direction, direction.Logic.OffClausesGroup);
			}

			foreach (var pumpStation in PumpStations)
			{
				LinkLogic(pumpStation, pumpStation.StartLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.StopLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.AutomaticOffLogic.OnClausesGroup);
			}

			foreach (var mpt in MPTs)
			{
				LinkLogic(mpt, mpt.StartLogic.OnClausesGroup);
			}

			foreach (var delay in Delays)
			{
				LinkLogic(delay, delay.Logic.OnClausesGroup);
				LinkLogic(delay, delay.Logic.OffClausesGroup);
			}

			foreach (var guardZone in GuardZones)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					LinkGKBases(guardZone, guardZoneDevice.Device);
					if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetector || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
					{
						LinkGKBases(guardZoneDevice.Device, guardZone);
					}
				}
				LinkGKBases(guardZone, guardZone);
			}

			foreach (var door in Doors)
			{
				if (door.EnterDevice != null)
					LinkGKBases(door, door.EnterDevice);
				if (door.ExitDevice != null)
					LinkGKBases(door, door.ExitDevice);
				if (door.LockDevice != null)
					LinkGKBases(door.LockDevice, door);
				if (door.LockControlDevice != null)
					LinkGKBases(door, door.LockControlDevice);
			}
		}

		void LinkLogic(GKBase gkBase, GKClauseGroup clauseGroup)
		{
			if (clauseGroup.Clauses != null)
			{
				foreach (var clause in clauseGroup.Clauses)
				{
					foreach (var clauseDevice in clause.Devices)
						LinkGKBases(gkBase, clauseDevice);
					foreach (var zone in clause.Zones)
						LinkGKBases(gkBase, zone);
					foreach (var guardZone in clause.GuardZones)
						LinkGKBases(gkBase, guardZone);
					foreach (var direction in clause.Directions)
						LinkGKBases(gkBase, direction);
					foreach (var mpt in clause.MPTs)
						LinkGKBases(gkBase, mpt);
					foreach (var delay in clause.Delays)
						LinkGKBases(gkBase, delay);
				}
			}
			if (clauseGroup.ClauseGroups != null)
			{
				foreach (var group in clauseGroup.ClauseGroups)
				{
					LinkLogic(gkBase, group);
				}
			}
		}

		void PrepareDirections()
		{
			foreach (var direction in Directions)
			{
				direction.KauDatabaseParent = null;
				direction.GkDatabaseParent = null;

				var inputDirection = direction.ClauseInputDirections.FirstOrDefault();
				if (inputDirection != null)
				{
					direction.GkDatabaseParent = inputDirection.GkDatabaseParent;
				}

				var inputZone = direction.ClauseInputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.GkDatabaseParent != null)
						direction.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = direction.ClauseInputDevices.FirstOrDefault();
				if (inputDevice != null)
				{
					direction.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}
			}
		}

		void PreparePumpStations()
		{
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.KauDatabaseParent = null;
				pumpStation.GkDatabaseParent = null;

				var inputZone = pumpStation.ClauseInputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.GkDatabaseParent != null)
						pumpStation.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = pumpStation.ClauseInputDevices.FirstOrDefault();
				if (inputDevice != null)
				{
					pumpStation.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}

				var outputDevice = pumpStation.NSDevices.FirstOrDefault();
				if (outputDevice != null)
				{
					pumpStation.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}
			}
		}

		void PrepareMPTs()
		{
			foreach (var mpt in MPTs)
			{
				mpt.KauDatabaseParent = null;
				mpt.GkDatabaseParent = null;

				var inputZone = mpt.ClauseInputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.GkDatabaseParent != null)
						mpt.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = mpt.ClauseInputDevices.FirstOrDefault();
				if (inputDevice != null)
				{
					mpt.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}

				var outputDevice = mpt.Devices.FirstOrDefault();
				if (outputDevice != null)
				{
					mpt.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}
			}
		}

		void PrepareDelays()
		{
			foreach (var delay in Delays)
			{
				delay.KauDatabaseParent = null;
				delay.GkDatabaseParent = null;

				var inputDirection = delay.ClauseInputDirections.FirstOrDefault();
				if (inputDirection != null)
				{
					delay.GkDatabaseParent = inputDirection.GkDatabaseParent;
				}

				var inputZone = delay.ClauseInputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.GkDatabaseParent != null)
						delay.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = delay.ClauseInputDevices.FirstOrDefault();
				if (inputDevice != null)
				{
					delay.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}
			}
		}

		void PrepareGuardZones()
		{
			foreach (var guardZone in GuardZones)
			{
				guardZone.KauDatabaseParent = null;
				guardZone.GkDatabaseParent = null;

				var gkParents = new HashSet<GKDevice>();
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					var gkParent = guardZoneDevice.Device.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
					gkParents.Add(gkParent);
				}

				var gkControllerDevice = gkParents.FirstOrDefault();
				if (gkControllerDevice != null)
				{
					guardZone.GkDatabaseParent = gkControllerDevice;
				}
			}
		}

		void PrepareCodes()
		{
			foreach (var code in Codes)
			{
				code.KauDatabaseParent = null;
				code.GkDatabaseParent = GKManager.Devices.FirstOrDefault(x=>x.DriverType == GKDriverType.GK);
			}
		}

		void PrepareDoors()
		{
			foreach (var door in Doors)
			{
				door.KauDatabaseParent = null;
				door.GkDatabaseParent = door.EnterDevice != null ? door.EnterDevice.GKParent : null;
			}
		}

		public static void LinkGKBases(GKBase value, GKBase dependsOn)
		{
			AddInputOutputObject(value.InputGKBases, dependsOn);
			AddInputOutputObject(dependsOn.OutputGKBases, value);
		}

		static void AddInputOutputObject(List<GKBase> objects, GKBase newObject)
		{
			if (!objects.Contains(newObject))
				objects.Add(newObject);
		}
	}
}