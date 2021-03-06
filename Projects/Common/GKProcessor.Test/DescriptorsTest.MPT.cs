﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhAPI.GK;
using RubezhAPI;

namespace GKProcessor.Test
{
	public partial class DescriptorsTest
	{
		[TestMethod]
		public void TestMPTOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var mpt = new GKMPT();
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice() { MPTDeviceType = GKMPTDeviceType.Bomb, DeviceUID = device2.UID });
			GKManager.MPTs.Add(mpt);
			Compile();

			CheckObjectLogicOnKau(mpt);
			CheckDeviceLogicOnKau(device2);
		}

		[TestMethod]
		public void TestMPTOnGK()
		{
			var device1 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var mpt = new GKMPT();
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice() { MPTDeviceType = GKMPTDeviceType.Bomb, DeviceUID = device2.UID });
			GKManager.MPTs.Add(mpt);
			Compile();

			CheckObjectLogicOnGK(mpt);
			CheckDeviceLogicOnGK(device2);
		}

		[TestMethod]
		public void TestMPTWithCodesOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			var mpt = new GKMPT();
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice() { MPTDeviceType = GKMPTDeviceType.HandStart, DeviceUID = device2.UID, CodeReaderSettings = new GKCodeReaderSettings() { MPTSettings = new GKCodeReaderSettingsPart() { CodeUIDs = {code.UID}} } });
			GKManager.MPTs.Add(mpt);
			Compile();

			CheckObjectLogicOnKau(mpt);
			Assert.IsNotNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == code));
		}

		[TestMethod]
		public void TestMPTWithCodesOnGK()
		{
			var device1 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			var mpt = new GKMPT();
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice() { MPTDeviceType = GKMPTDeviceType.HandStart, DeviceUID = device2.UID, CodeReaderSettings = new GKCodeReaderSettings() { MPTSettings = new GKCodeReaderSettingsPart() { CodeUIDs = { code.UID } } } });
			GKManager.MPTs.Add(mpt);
			Compile();

			CheckObjectLogicOnGK(mpt);
			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == code));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == code));
			Assert.IsNotNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == code));
		}

		[TestMethod]
		public void TestMPTWithAccessLevelsOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var mpt = new GKMPT();
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice() { MPTDeviceType = GKMPTDeviceType.HandStart, DeviceUID = device2.UID, CodeReaderSettings = new GKCodeReaderSettings() { MPTSettings = new GKCodeReaderSettingsPart() { AccessLevel = 1 } } });
			GKManager.MPTs.Add(mpt);
			Compile();

			CheckObjectLogicOnGK(mpt);
		}

		[TestMethod]
		public void TestMPTWithGlobalPimOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var mpt = new GKMPT();
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice { MPTDeviceType = GKMPTDeviceType.HandStart, DeviceUID = device2.UID, CodeReaderSettings = new GKCodeReaderSettings { MPTSettings = new GKCodeReaderSettingsPart() } });
			GKManager.MPTs.Add(mpt);
			Compile();

			var globalPimDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => (x.GKBase is GKPim) && (x.GKBase as GKPim).IsGlobalPim);
			Assert.IsNotNull(globalPimDescriptor);
			var mptDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => (x.GKBase == mpt));
			Assert.IsNotNull(mptDescriptor);
			Assert.IsTrue(mptDescriptor.Formula.FormulaOperations.Any(x => x.GKBaseSecondOperand == globalPimDescriptor.GKBase));
		}

		[TestMethod]
		public void TestMPTWithGlobalPimOnGk()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var mpt = new GKMPT();
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice { MPTDeviceType = GKMPTDeviceType.HandStart, DeviceUID = device2.UID, CodeReaderSettings = new GKCodeReaderSettings { MPTSettings = new GKCodeReaderSettingsPart{AccessLevel = 1} } });
			GKManager.MPTs.Add(mpt);
			Compile();

			var globalPimDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => (x.GKBase is GKPim) && (x.GKBase as GKPim).IsGlobalPim && x.GKBase.KauDatabaseParent == null);
			Assert.IsNotNull(globalPimDescriptor);
			var mptDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => (x.GKBase == mpt));
			Assert.IsNotNull(mptDescriptor);
			Assert.IsTrue(mptDescriptor.Formula.FormulaOperations.Any(x => x.GKBaseSecondOperand == globalPimDescriptor.GKBase));
		}
	}
}