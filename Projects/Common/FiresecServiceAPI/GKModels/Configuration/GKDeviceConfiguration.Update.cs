﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecClient;

namespace FiresecAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void UpdateConfiguration()
		{
			if (RootDevice == null)
			{
				var systemDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
				if (systemDriver != null)
				{
					RootDevice = new GKDevice()
					{
						DriverUID = systemDriver.UID,
						Driver = systemDriver
					};
				}
				else
				{
					Logger.Error("GKManager.SetEmptyConfiguration systemDriver = null");
				}
			}
			ValidateVersion();

			Update();
			foreach (var device in Devices)
			{
				device.Driver = GKManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					//MessageBoxService.Show("Ошибка при сопоставлении драйвера устройств ГК");
				}
			}
			Reorder();

			InitializeProperties();
			Invalidate();
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
			UpdateGKChildrenDescription();
		}

		void ClearAllReferences()
		{
			foreach (var device in Devices)
			{
				device.ClearClauseDependencies();
			}
		}

		void InitializeLogic()
		{
			foreach (var device in Devices)
			{
				InvalidateOneLogic(device, device.Logic);
			}
		}

		public void InvalidateOneLogic(GKDevice device, GKLogic logic)
		{
			InvalidateInputObjectsBaseLogic(device, logic);
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

				var deviceUIDs = new List<Guid>();
				foreach (var deviceUID in clause.DeviceUIDs)
				{
					var clauseDevice = Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (clauseDevice != null && !clauseDevice.IsNotUsed)
					{
						deviceUIDs.Add(deviceUID);
						clause.Devices.Add(clauseDevice);
						if (!gkBase.ClauseInputDevices.Contains(clauseDevice))
							gkBase.ClauseInputDevices.Add(clauseDevice);
					}
				}
				clause.DeviceUIDs = deviceUIDs;

				if (clause.HasObjects())
					result.Clauses.Add(clause);
			}
			return result;
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
	}
}