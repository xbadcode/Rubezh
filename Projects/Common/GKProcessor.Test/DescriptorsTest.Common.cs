﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FiresecClient;
using FiresecAPI.GK;

namespace GKProcessor.Test
{
	[TestClass]
	public partial class DescriptorsTest
	{
		GKDevice gkDevice;
		GKDevice kauDevice1;
		GKDevice kauDevice2;

		GkDatabase GkDatabase;
		KauDatabase Kau1Database;
		KauDatabase Kau2Database;

		[TestInitialize]
		public void CreateConfiguration()
		{
			GKDriversCreator.Create();
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice() { DriverUID = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System).UID };
			gkDevice = GKManager.AddChild(systemDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice1 = GKManager.AddChild(gkDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice2 = GKManager.AddChild(gkDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);
		}

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddChild(device.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
		}

		void Compile()
		{
			DescriptorsManager.Create();
			Assert.IsTrue(DescriptorsManager.GkDatabases.Count == 1);
			Assert.IsTrue(DescriptorsManager.KauDatabases.Count == 2);
			GkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == gkDevice);
			Assert.IsNotNull(GkDatabase);
			Kau1Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice1);
			Assert.IsNotNull(Kau1Database);
			Kau2Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice2);
			Assert.IsNotNull(Kau2Database);
			Assert.IsTrue(DescriptorsManager.Check());
		}

		void CheckDeviceLogicOnGK(GKDevice device)
		{
			var device1GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			var device1Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device);
			Assert.IsTrue(device1GKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика устройства");
			Assert.IsTrue(device1Kau1Descriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика устройства");
		}

		void CheckDeviceLogicOnKau(GKDevice device)
		{
			var device1GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			var device1Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device);
			Assert.IsTrue(device1GKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика устройства");
			Assert.IsTrue(device1Kau1Descriptor.Formula.FormulaOperations.Count > 1, "На КАУ должна присутствовать логика устройства");
		}

		void CheckObjectLogicOnGK(GKBase gkBase)
		{
			var device1GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			Assert.IsTrue(device1GKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика объекта");
			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == gkBase), "На КАУ должен отсутствовать объект");
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == gkBase), "На КАУ должен отсутствовать объект");
		}

		void CheckObjectLogicOnKau(GKBase gkBase)
		{
			var device1GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			var device1Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			Assert.IsTrue(device1GKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика объекта");
			Assert.IsTrue(device1Kau1Descriptor.Formula.FormulaOperations.Count > 1, "На КАУ должна присутствовать логика объекта");
		}

		[TestMethod]
		public void TestZoneOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var zone1 = new GKZone();
			GKManager.Zones.Add(zone1);
			device1.ZoneUIDs.Add(zone1.UID);
			Compile();

			CheckObjectLogicOnKau(zone1);
		}

		[TestMethod]
		public void TestZoneOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var zone1 = new GKZone();
			GKManager.Zones.Add(zone1);
			device1.ZoneUIDs.Add(zone1.UID);
			device2.ZoneUIDs.Add(zone1.UID);
			Compile();

			CheckObjectLogicOnGK(zone1);
		}

		[TestMethod]
		public void TestDeviceLogicOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device1.UID }
			};
			device2.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckDeviceLogicOnKau(device2);
		}

		[TestMethod]
		public void TestDeviceLogicOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var device3 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID, device2.UID }
			};
			device3.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckDeviceLogicOnGK(device3);
		}

		[TestMethod]
		public void TestDirectionLogicOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var direction = new GKDirection();
			GKManager.Directions.Add(direction);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device1.UID }
			};
			direction.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckObjectLogicOnKau(direction);
		}

		[TestMethod]
		public void TestDirectionLogicOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var direction = new GKDirection();
			GKManager.Directions.Add(direction);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID, device2.UID }
			};
			direction.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckObjectLogicOnGK(direction);
		}

		[TestMethod]
		public void TestDelayLogicOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var delay = new GKDelay();
			GKManager.Delays.Add(delay);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device1.UID }
			};
			delay.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckObjectLogicOnKau(delay);
		}

		[TestMethod]
		public void TestDelayLogicOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var delay = new GKDelay();
			GKManager.Delays.Add(delay);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID, device2.UID }
			};
			delay.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckObjectLogicOnGK(delay);
		}

		[TestMethod]
		public void TestEmptyObjects()
		{
			var zone = new GKZone();
			GKManager.Zones.Add(zone);

			var direction = new GKDirection();
			GKManager.Directions.Add(direction);

			var delay = new GKDelay();
			GKManager.Delays.Add(delay);

			var guardZone = new GKGuardZone();
			GKManager.GuardZones.Add(guardZone);

			var pumpStation = new GKPumpStation();
			GKManager.PumpStations.Add(pumpStation);

			var mpt = new GKMPT();
			GKManager.MPTs.Add(mpt);

			var door = new GKDoor();
			GKManager.Doors.Add(door);

			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			Compile();

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == zone));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == zone));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == zone));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == direction));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == direction));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == direction));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == delay));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == delay));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == delay));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == guardZone));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == guardZone));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == guardZone));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == pumpStation));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == pumpStation));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == pumpStation));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == mpt));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == mpt));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == mpt));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == door));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == door));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == door));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == code));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == code));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == code));
		}

		[TestMethod]
		public void TestEmptyObjectsCrossReferences()
		{
			var direction1 = new GKDirection();
			GKManager.Directions.Add(direction1);

			var direction2 = new GKDirection();
			GKManager.Directions.Add(direction2);

			direction1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction1.UID } });
			direction2.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction2.UID } });
			Compile();

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == direction1));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == direction1));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == direction1));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == direction2));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == direction2));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == direction2));

			Compile();
		}
	}
}