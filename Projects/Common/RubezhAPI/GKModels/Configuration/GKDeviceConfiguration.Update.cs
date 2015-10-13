﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhClient;
using System.Diagnostics;

namespace RubezhAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void UpdateConfiguration()
		{
			ValidateVersion();
			Update();

			foreach (var device in Devices)
			{
				device.Driver = GKManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					//MessageBoxService.Show("Ошибка при сопоставлении драйвера устройств ГК");
				}
				if (device.DriverType == GKDriverType.GK)
					device.IntAddress = 0;
			}
			Reorder();

			InitializeProperties();
			Invalidate();
			CopyMPTProperties();
		}

		void InitializeProperties()
		{
			foreach (var device in Devices)
			{
				if (device.Properties == null)
					device.Properties = new List<GKProperty>();
				foreach (var property in device.Properties)
				{
					property.DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				}
				device.Properties.RemoveAll(x => x.DriverProperty == null);

				foreach (var property in device.DeviceProperties)
				{
					property.DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				}
				device.DeviceProperties.RemoveAll(x => x.DriverProperty == null);
				device.InitializeDefaultProperties();
			}
		}

		void Invalidate()
		{
			ClearAllReferences();
			InitializeLogic();
			InitializeCodes();
			InitializeReflection();
			InitializeOPC();
			UpdateGKChildrenDescription();
			Invalidation();
		}

		void ClearAllReferences()
		{
			foreach (var device in Devices)
			{
				device.ClearClauseDependencies();
				device.Zones = new List<GKZone>();
				device.GuardZones = new List<GKGuardZone>();
				device.Directions = new List<GKDirection>();
				if (device.GKReflectionItem != null)
				{
					device.GKReflectionItem.Delays = new List<GKDelay>();
					device.GKReflectionItem.Devices = new List<GKDevice>();
					device.GKReflectionItem.Diretions = new List<GKDirection>();
					device.GKReflectionItem.GuardZones = new List<GKGuardZone>();
					device.GKReflectionItem.NSs = new List<GKPumpStation>();
					device.GKReflectionItem.Zones = new List<GKZone>();
					device.GKReflectionItem.MPTs = new List<GKMPT>();
					device.Door = null;
				}
			}
			foreach (var zone in Zones)
			{
				zone.ClearClauseDependencies();
				zone.Devices = new List<GKDevice>();
				zone.Directions = new List<GKDirection>();
				zone.DevicesInLogic = new List<GKDevice>();
			}
			foreach (var direction in Directions)
			{
				direction.ClearClauseDependencies();
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.ClearClauseDependencies();
				pumpStation.NSDevices = new List<GKDevice>();
			}
			foreach (var mpt in MPTs)
			{
				mpt.ClearClauseDependencies();
			}
			foreach (var delay in Delays)
			{
				delay.ClearClauseDependencies();
			}
		}

		void InitializeLogic()
		{
			foreach (var device in Devices)
			{
				InvalidateOneLogic(device, device.Logic);
				if (device.NSLogic != null)
					InvalidateOneLogic(device, device.NSLogic);
			}
		}

		void Invalidation()
		{
			Stopwatch st = new Stopwatch();

			foreach (var device in Devices)
			{
				device.Invalidate();
			}

			foreach (var mpt in MPTs)
			{
				mpt.Invalidate();
			}
			st.Start();
			Doors.ForEach(x => x.Invalidate());
			st.Stop();
			foreach (var pump in PumpStations)
			{
				pump.Invalidate();
			}
			st.Reset();
			foreach (var guardZone in GuardZones)
			{
				guardZone.Invalidate();
			}
			st.Stop();
			foreach (var delay in Delays)
			{
				delay.Invalidate();
			}

			foreach (var directory in Directions)
			{
				directory.Invalidate();
			}
	
		}
		
		public void InvalidateOneLogic(GKDevice device, GKLogic logic)
		{
			InvalidateInputObjectsBaseLogic(device, logic);
			foreach (var clause in logic.OnClausesGroup.Clauses)
			{
				foreach (var clauseZone in clause.Zones)
				{
					if (!clauseZone.DevicesInLogic.Contains(device))
						clauseZone.DevicesInLogic.Add(device);
				}
				foreach (var clauseDirection in clause.Directions)
				{
					device.Directions.Add(clauseDirection);
				}
			}
			foreach (var clause in device.Logic.OffClausesGroup.Clauses)
			{
				foreach (var clauseZone in clause.Zones)
				{
					if (!clauseZone.DevicesInLogic.Contains(device))
						clauseZone.DevicesInLogic.Add(device);
				}
				foreach (var clauseDirection in clause.Directions)
				{
					device.Directions.Add(clauseDirection);
				}
			}
			foreach (var clause in device.Logic.StopClausesGroup.Clauses)
			{
				foreach (var clauseZone in clause.Zones)
				{
					if (!clauseZone.DevicesInLogic.Contains(device))
						clauseZone.DevicesInLogic.Add(device);
				}
				foreach (var clauseDirection in clause.Directions)
				{
					device.Directions.Add(clauseDirection);
				}
			}
		}

		

		public void InvalidateInputObjectsBaseLogic(GKBase gkBase, GKLogic logic)
		{
			logic.OnClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.OnClausesGroup);
			logic.OffClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.OffClausesGroup);
			logic.StopClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.StopClausesGroup);
			logic.OnNowClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.OnNowClausesGroup);
			logic.OffNowClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.OffNowClausesGroup);
		}

	   public GKClauseGroup InvalidateOneInputObjectsBaseLogic(GKBase gkBase, GKClauseGroup clauseGroup)
		{
			var result = new GKClauseGroup();
			result.ClauseJounOperationType = clauseGroup.ClauseJounOperationType;
			var groups = new List<GKClauseGroup>();
			foreach (var group in clauseGroup.ClauseGroups)
			{
				var _clauseGroup = InvalidateOneInputObjectsBaseLogic(gkBase, group);
				if (_clauseGroup.Clauses.Count + _clauseGroup.ClauseGroups.Count > 0)
					groups.Add(_clauseGroup);
			}
			result.ClauseGroups = groups;

			foreach (var clause in clauseGroup.Clauses)
			{
				clause.Devices = new List<GKDevice>();
				clause.Zones = new List<GKZone>();
				clause.GuardZones = new List<GKGuardZone>();
				clause.Directions = new List<GKDirection>();
				clause.MPTs = new List<GKMPT>();
				clause.Delays = new List<GKDelay>();
				clause.Doors = new List<GKDoor>();
				clause.PumpStations = new List<GKPumpStation>();

				var deviceUIDs = new List<Guid>();
				foreach (var deviceUID in clause.DeviceUIDs)
				{
					var clauseDevice = Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (clauseDevice != null && !clauseDevice.IsNotUsed)
					{
						deviceUIDs.Add(deviceUID);
						clause.Devices.Add(clauseDevice);
					}
				}
				clause.DeviceUIDs = deviceUIDs;

				var zoneUIDs = new List<Guid>();
				foreach (var zoneUID in clause.ZoneUIDs)
				{
					var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						zoneUIDs.Add(zoneUID);
						clause.Zones.Add(zone);
					}
				}
				clause.ZoneUIDs = zoneUIDs;

				var guardZoneUIDs = new List<Guid>();
				foreach (var guardZoneUID in clause.GuardZoneUIDs)
				{
					var guardZone = GuardZones.FirstOrDefault(x => x.UID == guardZoneUID);
					if (guardZone != null)
					{
						guardZoneUIDs.Add(guardZoneUID);
						clause.GuardZones.Add(guardZone);
					}
				}
				clause.GuardZoneUIDs = guardZoneUIDs;

				var directionUIDs = new List<Guid>();
				foreach (var directionUID in clause.DirectionUIDs)
				{
					var direction = Directions.FirstOrDefault(x => x.UID == directionUID);
					if (direction != null)
					{
						directionUIDs.Add(directionUID);
						clause.Directions.Add(direction);
					}
				}
				clause.DirectionUIDs = directionUIDs;

				var mptUIDs = new List<Guid>();
				foreach (var mptUID in clause.MPTUIDs)
				{
					var mpt = MPTs.FirstOrDefault(x => x.UID == mptUID);
					if (mpt != null)
					{
						mptUIDs.Add(mptUID);
						clause.MPTs.Add(mpt);
					}
				}
				clause.MPTUIDs = mptUIDs;

				var delayUIDs = new List<Guid>();
				foreach (var delayUID in clause.DelayUIDs)
				{
					var delay = Delays.FirstOrDefault(x => x.UID == delayUID);
					if (delay != null)
					{
						delayUIDs.Add(delayUID);
						clause.Delays.Add(delay);
					}
				}
				clause.DelayUIDs = delayUIDs;

				var doorUIDs = new List<Guid>();
				foreach (var doorUID in clause.DoorUIDs)
				{
					var door = Doors.FirstOrDefault(x => x.UID == doorUID);
					if (door != null)
					{
						doorUIDs.Add(doorUID);
						clause.Doors.Add(door);
					}
				}
				clause.DoorUIDs = doorUIDs;

				var pumpStationsUIDs = new List<Guid>();
				foreach (var pumpStationUID in clause.PumpStationsUIDs)
				{
					var pumpStation = PumpStations.FirstOrDefault(x => x.UID == pumpStationUID);
					if (pumpStation != null)
					{
						pumpStationsUIDs.Add(pumpStationUID);
						clause.PumpStations.Add(pumpStation);
					}
				}
				clause.PumpStationsUIDs = pumpStationsUIDs;

				if (clause.HasObjects())
					result.Clauses.Add(clause);
			}
			return result;
		}

		void InitializeGuardZones()
		{
			foreach (var guardZone in GuardZones)
			{
				var guardZoneDevices = new List<GKGuardZoneDevice>();
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					var device = Devices.FirstOrDefault(x => x.UID == guardZoneDevice.DeviceUID);
					if (device != null)
					{
						if (device.DriverType == GKDriverType.RSR2_GuardDetector || device.DriverType == GKDriverType.RSR2_GuardDetectorSound || device.DriverType == GKDriverType.RSR2_AM_1 || device.DriverType == GKDriverType.RSR2_MAP4 || device.DriverType == GKDriverType.RSR2_CodeReader || device.DriverType == GKDriverType.RSR2_CardReader)
						{
							guardZoneDevice.Device = device;
							guardZoneDevices.Add(guardZoneDevice);
							device.GuardZones.Add(guardZone);
						}
						if (device.DriverType == GKDriverType.RSR2_CodeReader || device.DriverType == GKDriverType.RSR2_CardReader)
						{
							InvalidateGKCodeReaderSettingsPart(guardZoneDevice.CodeReaderSettings.SetGuardSettings);
							InvalidateGKCodeReaderSettingsPart(guardZoneDevice.CodeReaderSettings.ResetGuardSettings);
							InvalidateGKCodeReaderSettingsPart(guardZoneDevice.CodeReaderSettings.ChangeGuardSettings);
							InvalidateGKCodeReaderSettingsPart(guardZoneDevice.CodeReaderSettings.AlarmSettings);
						}
					}
				}
				guardZone.GuardZoneDevices = guardZoneDevices;
			}
		}

		public void InvalidateGKCodeReaderSettingsPart(GKCodeReaderSettingsPart codeReaderSettingsPart)
		{
			var codeUIDs = new List<Guid>();
			foreach (var codeUID in codeReaderSettingsPart.CodeUIDs)
			{
				var code = Codes.FirstOrDefault(x => x.UID == codeUID);
				if (code != null)
				{
					codeUIDs.Add(codeUID);
				}
			}
			codeReaderSettingsPart.CodeUIDs = codeUIDs;
		}

		void InitializeCodes()
		{
			foreach (var code in Codes)
			{
			}
		}

		void InitializeReflection()
		{
			foreach (var device in Devices)
			{
				if (device.Driver.HasMirror)
				{
					if (device.GKReflectionItem != null)
					{
						var zoneUIDs = new List<Guid>();
						var guardzoneUIDs = new List<Guid>();
						var delayUIDs = new List<Guid>();
						var deviceUIDs = new List<Guid>();
						var pumpUIDs = new List<Guid>();
						var MPTUIDs = new List<Guid>();
						var directionUIDs = new List<Guid>();

						foreach (var guardzoneUID in device.GKReflectionItem.GuardZoneUIDs)
						{
							var zone = GuardZones.FirstOrDefault(x => x.UID == guardzoneUID);
							if (zone != null)
							{
								guardzoneUIDs.Add(guardzoneUID);
								device.GKReflectionItem.GuardZones.Add(zone);
							}
						}
						device.GKReflectionItem.GuardZoneUIDs = guardzoneUIDs;

						foreach (var zoneUID in device.GKReflectionItem.ZoneUIDs)
						{
							var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
							if (zone != null)
							{
								zoneUIDs.Add(zoneUID);
								device.GKReflectionItem.Zones.Add(zone);
							}
						}
						device.GKReflectionItem.ZoneUIDs = zoneUIDs;

						foreach (var delayUID in device.GKReflectionItem.DelayUIDs)
						{
							var delay = Delays.FirstOrDefault(x => x.UID == delayUID);
							if (delay != null)
							{
								delayUIDs.Add(delayUID);
								device.GKReflectionItem.Delays.Add(delay);
							}
						}
						device.GKReflectionItem.DelayUIDs = delayUIDs;

						foreach (var deviceUID in device.GKReflectionItem.DeviceUIDs)
						{
							var _delice = Devices.FirstOrDefault(x => x.UID == deviceUID);
							if (_delice != null)
							{
								deviceUIDs.Add(deviceUID);
								device.GKReflectionItem.Devices.Add(_delice);
							}
						}
						device.GKReflectionItem.DeviceUIDs = deviceUIDs;

						foreach (var pumpUID in device.GKReflectionItem.NSUIDs)
						{
							var pumps = PumpStations.FirstOrDefault(x => x.UID == pumpUID);
							if (pumps != null)
							{
								pumpUIDs.Add(pumpUID);
								device.GKReflectionItem.NSs.Add(pumps);
							}
						}
						device.GKReflectionItem.NSUIDs = pumpUIDs;

						foreach (var mptUID in device.GKReflectionItem.MPTUIDs)
						{
							var mpts = MPTs.FirstOrDefault(x => x.UID == mptUID);
							if (mpts != null)
							{
								MPTUIDs.Add(mptUID);
								device.GKReflectionItem.MPTs.Add(mpts);
							}
						}
						device.GKReflectionItem.MPTUIDs = MPTUIDs;

						foreach (var directionUID in device.GKReflectionItem.DiretionUIDs)
						{
							var directions = Directions.FirstOrDefault(x => x.UID == directionUID);
							if (directions != null)
							{
								directionUIDs.Add(directionUID);
								device.GKReflectionItem.Diretions.Add(directions);
							}
						}
						device.GKReflectionItem.DiretionUIDs = directionUIDs;
					}
				}
			}
		}

		void InitializeOPC()
		{
			OPCSettings.ZoneUIDs = Zones.Where(x => OPCSettings.ZoneUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.DelayUIDs = Delays.Where(x => OPCSettings.DelayUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.DeviceUIDs = Devices.Where(x => OPCSettings.DeviceUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.DiretionUIDs = Directions.Where(x => OPCSettings.DiretionUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.GuardZoneUIDs = GuardZones.Where(x => OPCSettings.GuardZoneUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.MPTUIDs = MPTs.Where(x => OPCSettings.MPTUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.NSUIDs = PumpStations.Where(x => OPCSettings.NSUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.DoorUIDs = Doors.Where(x => OPCSettings.DoorUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
		}			

		void UpdateGKChildrenDescription()
		{
			foreach (var gkControllerDevice in RootDevice.Children)
			{
				UpdateGKPredefinedName(gkControllerDevice);
			}
		}

		public void UpdateGKPredefinedName(GKDevice device)
		{
			if (device.DriverType == GKDriverType.GK && device.Children.Count >= 15)
			{
				if (device.Children.Count >= 21)
				{
					device.Children[0].PredefinedName = "Неисправность";
					device.Children[1].PredefinedName = "Пожар 1";
					device.Children[2].PredefinedName = "Пожар 2";
					device.Children[3].PredefinedName = "Внимание";
					device.Children[4].PredefinedName = "Включение ПУСК";
					device.Children[5].PredefinedName = "Тест";
					device.Children[6].PredefinedName = "Отключение";
					device.Children[7].PredefinedName = "Автоматика отключена";
					device.Children[8].PredefinedName = "Звук отключен";
					device.Children[9].PredefinedName = "Останов пуска";
					device.Children[10].PredefinedName = "Реле 1";
					device.Children[11].PredefinedName = "Реле 2";
					device.Children[12].PredefinedName = "Реле 3";
					device.Children[13].PredefinedName = "Реле 4";
					device.Children[14].PredefinedName = "Реле 5";
					device.Children[15].PredefinedName = "Тревога";
					device.Children[16].PredefinedName = "Резерв 1";
					device.Children[17].PredefinedName = "Резерв 2";
					device.Children[18].PredefinedName = "Резерв 3";
					device.Children[19].PredefinedName = "Резерв 4";
					device.Children[20].PredefinedName = "Резерв 5";

				}
			}
		}

		void CopyMPTProperties()
		{
			foreach (var mpt in MPTs)
			{
				foreach (var mptDevice in mpt.MPTDevices)
				{
					SetIsMPT(mptDevice);
				}
			}
		}

		public void SetIsMPT(GKMPTDevice mptDevice)
		{
			if (mptDevice.Device != null)
			{
				mptDevice.Device.IsInMPT = true;
				GKManager.ChangeLogic(mptDevice.Device, new GKLogic());
				mptDevice.Device.ZoneUIDs = new List<Guid>();
				mptDevice.Device.Zones.Clear();
			}
		}

		public void SetMPTDefaultProperty(GKDevice device, GKMPTDeviceType mptDeviceType)
		{
			if (device != null)
			{
				switch (device.DriverType)
				{
					case GKDriverType.RSR2_AM_1:
						SetDeviceProperty(device, "Конфигурация", 1);
						break;

					case GKDriverType.RSR2_OPS:
					case GKDriverType.RSR2_OPZ:
					case GKDriverType.RSR2_OPK:
						SetDeviceProperty(device, "Задержка на включение, с", 0);
						SetDeviceProperty(device, "Время удержания, с", 0);
						SetDeviceProperty(device, "Задержка на выключение, с", 0);
						SetDeviceProperty(device, "Состояние для модуля Выключено", 0);
						SetDeviceProperty(device, "Состояние для режима Удержания", 4);
						if (mptDeviceType == GKMPTDeviceType.DoNotEnterBoard || mptDeviceType == GKMPTDeviceType.ExitBoard)
						{
							SetDeviceProperty(device, "Состояние для режима Включено", 32);
						}
						if (mptDeviceType == GKMPTDeviceType.Speaker || mptDeviceType == GKMPTDeviceType.AutomaticOffBoard)
						{
							SetDeviceProperty(device, "Состояние для режима Включено", 16);
						}
						break;

					case GKDriverType.RSR2_MVK8:
						SetDeviceProperty(device, "Задержка на включение, с", 0);
						SetDeviceProperty(device, "Задержка на выключение, с", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Выключено", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Удержания", 4);
						SetDeviceProperty(device, "Контроль", 3);
						SetDeviceProperty(device, "Норма питания, 0.1В", 80);
						if (mptDeviceType == GKMPTDeviceType.DoNotEnterBoard || mptDeviceType == GKMPTDeviceType.ExitBoard)
						{
							SetDeviceProperty(device, "Время удержания, с", 0);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 32);
						}

						if (mptDeviceType == GKMPTDeviceType.Bomb)
						{
							SetDeviceProperty(device, "Время удержания, с", 2);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 0);
						}

						if (mptDeviceType == GKMPTDeviceType.Speaker || mptDeviceType == GKMPTDeviceType.AutomaticOffBoard)
						{
							SetDeviceProperty(device, "Время удержания, с", 0);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 16);
						}
						break;

					case GKDriverType.RSR2_RM_1:
						SetDeviceProperty(device, "Задержка на включение, с", 0);
						SetDeviceProperty(device, "Задержка на выключение, с", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Выключено", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Удержания", 4);
						if (mptDeviceType == GKMPTDeviceType.DoNotEnterBoard || mptDeviceType == GKMPTDeviceType.ExitBoard)
						{
							SetDeviceProperty(device, "Время удержания, с", 0);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 32);
						}
						if (mptDeviceType == GKMPTDeviceType.Bomb)
						{
							SetDeviceProperty(device, "Время удержания, с", 2);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 0);
						}
						if (mptDeviceType == GKMPTDeviceType.Speaker || mptDeviceType == GKMPTDeviceType.AutomaticOffBoard)
						{
							SetDeviceProperty(device, "Время удержания, с", 0);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 16);
						}
						break;
				}
			}
		}

		void SetDeviceProperty(GKDevice device, string propertyName, int value)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == propertyName);
			if (property == null)
			{
				property = new GKProperty()
				{
					Name = propertyName,
					DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == propertyName)
				};
				device.Properties.Add(property);
			}
			property.Value = (ushort)value;
		}
	}
}