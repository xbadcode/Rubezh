﻿using GKModule.ViewModels;
using GKProcessor;
using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Linq;

namespace GKModuleTest
{
	[TestFixture]
	public class ConfigurationCompareTest
	{
		GKDeviceConfiguration LocalConfiguration { get; set; }
		GKDeviceConfiguration RemoteConfiguration { get; set; }
		GKDevice LocalGkDevice { get; set; }
		GKDevice RemoteGkDevice { get; set; }
		GKDevice LocalAlsDevice { get; set; }
		GKDevice RemoteAlsDevice { get; set; }
		void Initialize()
		{
			LocalConfiguration = CreateConfiguration();
			RemoteConfiguration = CreateConfiguration();
			LocalGkDevice = LocalConfiguration.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			RemoteGkDevice = RemoteConfiguration.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			LocalAlsDevice = LocalConfiguration.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			RemoteAlsDevice = RemoteConfiguration.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
		}
		[Test]
		public void CompareSKDZones()
		{
			Initialize();
			var uid1 = Guid.NewGuid();
			var uid2 = Guid.NewGuid();
			var localSkdZone1 = CreateSKDZone(LocalConfiguration, "Зона 1", 1, uid1);
			var localSkdZone2 = CreateSKDZone(LocalConfiguration, "Зона 2", 2, uid2);
			var remoteSkdZone1 = CreateSKDZone(RemoteConfiguration, "Зона 1", 1, uid1);
			var remoteSkdZone2 = CreateSKDZone(RemoteConfiguration, "Зона 2", 2, uid2);
			var viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, LocalGkDevice);
			var localHasDifferences = viewModel.LocalObjectsViewModel.Objects.Any(x => x.HasDifferences);
			var remoteHasDifferences = viewModel.RemoteObjectsViewModel.Objects.Any(x => x.HasDifferences);
			Assert.IsFalse(localHasDifferences);
			Assert.IsFalse(remoteHasDifferences);

			localSkdZone1.Name = "Другое название зоны";
			viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, LocalGkDevice);
			var localObserveSkdZone = viewModel.LocalObjectsViewModel.Objects.FirstOrDefault(x => x.SKDZone != null && x.SKDZone.UID == localSkdZone1.UID);
			var remoteObserveSkdZone = viewModel.RemoteObjectsViewModel.Objects.FirstOrDefault(x => x.SKDZone != null && x.SKDZone.UID == remoteSkdZone1.UID);
			Assert.IsTrue(localObserveSkdZone.DifferenceDiscription == "Не совпадает название");
			Assert.IsTrue(remoteObserveSkdZone.DifferenceDiscription == "Не совпадает название");

			remoteSkdZone1.Name = "Другое название зоны";
			localSkdZone1.Name = "Зона 1";
			viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, LocalGkDevice);
			localObserveSkdZone = viewModel.LocalObjectsViewModel.Objects.FirstOrDefault(x => x.SKDZone != null && x.SKDZone.UID == localSkdZone1.UID);
			remoteObserveSkdZone = viewModel.RemoteObjectsViewModel.Objects.FirstOrDefault(x => x.SKDZone != null && x.SKDZone.UID == remoteSkdZone1.UID);
			Assert.IsTrue(localObserveSkdZone.DifferenceDiscription == "Не совпадает название");
			Assert.IsTrue(remoteObserveSkdZone.DifferenceDiscription == "Не совпадает название");
		}
		[Test]
		public void ComparePumpStations()
		{
			Initialize();
			CreatePumpStation(LocalConfiguration);
			CreatePumpStation(RemoteConfiguration);
			var viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, LocalGkDevice);
			var localHasDifferences = viewModel.LocalObjectsViewModel.Objects.Any(x => x.HasDifferences);
			var remoteHasDifferences = viewModel.RemoteObjectsViewModel.Objects.Any(x => x.HasDifferences);
			Assert.IsFalse(localHasDifferences);
			Assert.IsFalse(remoteHasDifferences);

			LocalConfiguration.PumpStations.Clear();
			viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, LocalGkDevice);
			localHasDifferences = viewModel.LocalObjectsViewModel.Objects.Any(x => x.HasDifferences);
			remoteHasDifferences = viewModel.RemoteObjectsViewModel.Objects.Any(x => x.HasDifferences);
			Assert.IsTrue(localHasDifferences);
			Assert.IsTrue(remoteHasDifferences);
			var localPumpStation = viewModel.LocalObjectsViewModel.Objects.FirstOrDefault(x => x.ObjectType == ObjectType.PumpStation);
			var remotePumpStation = viewModel.RemoteObjectsViewModel.Objects.FirstOrDefault(x => x.ObjectType == ObjectType.PumpStation);
			Assert.IsTrue(localPumpStation.DifferenceDiscription == "Отсутствует в локальной конфигурации");
			Assert.IsTrue(localPumpStation.IsAbsent && !localPumpStation.IsPresent);
			Assert.IsTrue(remotePumpStation.DifferenceDiscription == "Отсутствует в локальной конфигурации");
			Assert.IsTrue(!remotePumpStation.IsAbsent && remotePumpStation.IsPresent);

			Initialize();
			CreatePumpStation(LocalConfiguration);
			viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, LocalGkDevice);
			localHasDifferences = viewModel.LocalObjectsViewModel.Objects.Any(x => x.HasDifferences);
			remoteHasDifferences = viewModel.RemoteObjectsViewModel.Objects.Any(x => x.HasDifferences);
			Assert.IsTrue(localHasDifferences);
			Assert.IsTrue(remoteHasDifferences);
			localPumpStation = viewModel.LocalObjectsViewModel.Objects.FirstOrDefault(x => x.ObjectType == ObjectType.PumpStation);
			remotePumpStation = viewModel.RemoteObjectsViewModel.Objects.FirstOrDefault(x => x.ObjectType == ObjectType.PumpStation);
			Assert.IsTrue(localPumpStation.DifferenceDiscription == "Отсутствует в конфигурации прибора");
			Assert.IsTrue(localPumpStation.IsAbsent == false && localPumpStation.IsPresent == true);
			Assert.IsTrue(remotePumpStation.DifferenceDiscription == "Отсутствует в конфигурации прибора");
			Assert.IsTrue(remotePumpStation.IsAbsent == true && remotePumpStation.IsPresent == false);
		}
		GKDeviceConfiguration CreateConfiguration()
		{
			var newConfiguration = new GKDeviceConfiguration();
			GKDriversCreator.Create();
			var drivers = GKManager.Drivers;
			var systemDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
			var gkDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			var kauDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU);
			var alsDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);

			var alsDevice = new GKDevice { DriverUID = alsDriver.UID };

			var kauDevice = new GKDevice { DriverUID = kauDriver.UID };
			kauDevice.Children.Add(alsDevice);

			var gkDevice = new GKDevice { DriverUID = gkDriver.UID };
			gkDevice.Children.Add(kauDevice);

			newConfiguration.RootDevice = new GKDevice { DriverUID = systemDriver.UID };
			newConfiguration.RootDevice.Children.Add(gkDevice);
			newConfiguration.UpdateConfiguration();
			return newConfiguration;
		}
		GKDevice CreateCardReader()
		{
			var cardReaderDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_CardReader);
			var cardReaderDevice = new GKDevice { DriverUID = cardReaderDriver.UID };
			return cardReaderDevice;
		}
		GKDevice CreateAm1(GKDevice alsDevice)
		{
			var am1Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_AM_1);
			var am1Device = new GKDevice { DriverUID = am1Driver.UID };
			alsDevice.Children.Add(am1Device);
			return am1Device;
		}
		GKSKDZone CreateSKDZone(GKDeviceConfiguration deviceConfiguration, string name, int no, Guid uid)
		{
			var cardReader = CreateCardReader();
			var skdZone = new GKSKDZone
			{
				Name = name,
				No = no,
				UID = uid
			};
			deviceConfiguration.SKDZones.Add(skdZone);
			return skdZone;
		}
		GKDoor CreateDoor(GKDeviceConfiguration deviceConfiguration, string name, int no, GKSKDZone enterZone, GKSKDZone exitZone)
		{
			var cardReaderDevice = CreateCardReader();
			var alsDevice = deviceConfiguration.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			var am1Device = CreateAm1(alsDevice);
			var logic = CreateLogic(am1Device);

			var door = new GKDoor
			{
				Name = "Точка доступа",
				No = 1,
				OpenRegimeLogic = logic,
				CloseRegimeLogic = logic,
				NormRegimeLogic = logic,
				EnterZoneUID = enterZone.UID,
				ExitZoneUID = exitZone.UID,
				EnterDevice = cardReaderDevice,
				EnterDeviceUID = cardReaderDevice.UID,
				ExitDevice = cardReaderDevice,
				ExitDeviceUID = cardReaderDevice.UID
			};
			return door;
		}
		GKPumpStation CreatePumpStation(GKDeviceConfiguration deviceConfiguration)
		{
			var alsDevice = deviceConfiguration.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			var bushDrenazhDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_Bush_Drenazh);
			var bushDrenazhDevice = new GKDevice { DriverUID = bushDrenazhDriver.UID };
			alsDevice.Children.Add(bushDrenazhDevice);
			var am1Device = CreateAm1(alsDevice);
			var logic = CreateLogic(am1Device);
			var pumpStation = new GKPumpStation
			{
				Name = "Насосная станция",
				No = 1,
				StartLogic = logic,
				StopLogic = logic,
				AutomaticOffLogic = logic
			};
			pumpStation.NSDevices.Add(bushDrenazhDevice);
			pumpStation.NSDeviceUIDs.Add(bushDrenazhDevice.UID);
			deviceConfiguration.PumpStations.Add(pumpStation);
			return pumpStation;
		}
		GKLogic CreateLogic(GKDevice device)
		{
			var clause = new GKClause() { ClauseOperationType = ClauseOperationType.AnyDevice, StateType = GKStateBit.Fire1 };
			clause.Devices.Add(device);
			clause.DeviceUIDs.Add(device.UID);
			var clauseGroup = new GKClauseGroup();
			clauseGroup.Clauses.Add(clause);
			var logic = new GKLogic();
			logic.OnClausesGroup = clauseGroup;
			return logic;
		}
	}
}