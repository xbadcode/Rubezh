﻿using System;
using System.Linq;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhClient;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsDBMissmatchDuringMonitoring = false;
		JournalEventDescriptionType DBMissmatchDuringMonitoringReason;

		bool _isJournalAnyDBMissmatch = false;
		bool IsJournalAnyDBMissmatch
		{
			get { return _isJournalAnyDBMissmatch; }
			set
			{
				if (_isJournalAnyDBMissmatch != value)
				{
					_isJournalAnyDBMissmatch = value;
					var journalItem = new JournalItem()
					{
						JournalEventNameType = value ? JournalEventNameType.База_данных_прибора_не_соответствует_базе_данных_ПК : JournalEventNameType.База_данных_прибора_соответствует_базе_данных_ПК,
					};
					//var gkIpAddress = GKManager.GetIpAddress(GkDatabase.RootDevice);
					//if (!string.IsNullOrEmpty(gkIpAddress))
					//    journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("IP-адрес ГК", gkIpAddress.ToString()));
					AddJournalItem(journalItem);
				}
			}
		}

		void GetAllStates()
		{
			IsDBMissmatchDuringMonitoring = false;
			GKProgressCallback progressCallback = GKProcessorManager.StartProgress("Опрос объектов ГК", "", GkDatabase.Descriptors.Count, false, GKProgressClientType.Monitor);

			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Опрос состояний объектов"))
			{
				for (int i = 0; i < GkDatabase.Descriptors.Count; i++)
				{
					var descriptor = GkDatabase.Descriptors[i];
					gkLifecycleManager.Progress(i + 1, GkDatabase.Descriptors.Count);

					LastUpdateTime = DateTime.Now;
					GetState(descriptor.GKBase);
					if (!IsConnected)
					{
						break;
					}
					GKProcessorManager.DoProgress(descriptor.GKBase.DescriptorPresentationName, progressCallback);

					WaitIfSuspending();
					if (IsStopping)
						return;
				}
			}
			GKProcessorManager.StopProgress(progressCallback);

			CheckTechnologicalRegime();
			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Передача состояний объектов"))
			{
				NotifyAllObjectsStateChanged();
				IsJournalAnyDBMissmatch = IsDBMissmatchDuringMonitoring;
			}
		}

		void CheckDelays()
		{
			foreach (var device in GKManager.Devices)
			{
				if (!device.Driver.IsGroupDevice && device.Driver.IsReal && device.AllParents.Any(x => x.DriverType == GKDriverType.RSR2_KAU))
				{
					CheckDelay(device);
				}
			}
			foreach (var direction in GKManager.Directions)
			{
				CheckDelay(direction);
			}
			foreach (var pumpStation in GKManager.PumpStations)
			{
				CheckDelay(pumpStation);
			}
			foreach (var mpt in GKManager.MPTs)
			{
				CheckDelay(mpt);
			}
			foreach (var delay in GKManager.Delays)
			{
				CheckDelay(delay);
			}
			foreach (var delay in GKManager.AutoGeneratedDelays)
			{
				CheckDelay(delay);
			}
			foreach (var guardZone in GKManager.GuardZones)
			{
				CheckDelay(guardZone);
			}
		}

		void CheckDelay(GKBase gkBase)
		{
			if (gkBase.InternalState == null)
				return;
			var mustGetState = false;
			if (gkBase.InternalState.StateClasses.Contains(XStateClass.TurningOn))
				mustGetState = (DateTime.Now - gkBase.InternalState.LastDateTime).TotalMilliseconds > 500;
			else if (gkBase.InternalState.StateClasses.Contains(XStateClass.On))
				mustGetState = gkBase.InternalState.ZeroHoldDelayCount < 10 && (DateTime.Now - gkBase.InternalState.LastDateTime).TotalMilliseconds > 500;
			else if (gkBase.InternalState.StateClasses.Contains(XStateClass.TurningOff))
				mustGetState = (DateTime.Now - gkBase.InternalState.LastDateTime).TotalMilliseconds > 500;
			if (mustGetState)
			{
				var onDelay = gkBase.InternalState.OnDelay;
				var holdDelay = gkBase.InternalState.HoldDelay;
				var offDelay = gkBase.InternalState.OffDelay;

				GetDelays(gkBase);

				if (onDelay != gkBase.InternalState.OnDelay || holdDelay != gkBase.InternalState.HoldDelay || offDelay != gkBase.InternalState.OffDelay)
					OnObjectStateChanged(gkBase);

				if (gkBase.InternalState.StateClass == XStateClass.On && holdDelay == 0)
					gkBase.InternalState.ZeroHoldDelayCount++;
				else
					gkBase.InternalState.ZeroHoldDelayCount = 0;
			}
		}

		bool GetDelays(GKBase gkBase)
		{
			gkBase.InternalState.LastDateTime = DateTime.Now;

			SendResult sendResult = null;
			if (gkBase.KauDatabaseParent != null)
			{
				sendResult = SendManager.Send(gkBase.KauDatabaseParent, 2, 12, 32, BytesHelper.ShortToBytes(gkBase.KAUDescriptorNo));
				if (sendResult.HasError || sendResult.Bytes.Count != 32)
				{
					gkBase.InternalState.ZeroHoldDelayCount = 10;
					return false;
				}
			}
			else
			{
				sendResult = SendManager.Send(gkBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(gkBase.GKDescriptorNo));
				if (sendResult.HasError || sendResult.Bytes.Count != 68)
				{
					gkBase.InternalState.ZeroHoldDelayCount = 10;
					ConnectionChanged(false);
					return false;
				}
			}

			ConnectionChanged(true);
			var descriptorStateHelper = new DescriptorStateHelper();
			descriptorStateHelper.Parse(sendResult.Bytes, gkBase);

			gkBase.InternalState.OnDelay = descriptorStateHelper.OnDelay;
			gkBase.InternalState.HoldDelay = descriptorStateHelper.HoldDelay;
			gkBase.InternalState.OffDelay = descriptorStateHelper.OffDelay;
			return true;
		}

		void GetState(GKBase gkBase, bool delaysOnly = false)
		{
			var sendResult = SendManager.Send(gkBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(gkBase.GKDescriptorNo));
			if (sendResult.HasError || sendResult.Bytes.Count != 68)
			{
				ConnectionChanged(false);
				return;
			}
			ConnectionChanged(true);
			var descriptorStateHelper = new DescriptorStateHelper();
			descriptorStateHelper.Parse(sendResult.Bytes, gkBase);
			CheckDBMissmatch(gkBase, descriptorStateHelper);

			gkBase.InternalState.LastDateTime = DateTime.Now;
			if (!delaysOnly)
			{
				gkBase.InternalState.StateBits = descriptorStateHelper.StateBits;
				gkBase.InternalState.AdditionalStates = descriptorStateHelper.AdditionalStates;
			}
			gkBase.InternalState.OnDelay = descriptorStateHelper.OnDelay;
			gkBase.InternalState.HoldDelay = descriptorStateHelper.HoldDelay;
			gkBase.InternalState.OffDelay = descriptorStateHelper.OffDelay;
		}

		void CheckDBMissmatch(GKBase gkBase, DescriptorStateHelper descriptorStateHelper)
		{
			bool isMissmatch = false;
			if (gkBase is GKDevice)
			{
				var device = gkBase as GKDevice;
				if (device.Driver.DriverTypeNo != descriptorStateHelper.TypeNo)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_устройства;
				}

				ushort physicalAddress = (ushort)device.IntAddress;
				if (device.Driver.IsDeviceOnShleif)
					physicalAddress = (ushort)((device.ShleifNo - 1) * 256 + device.IntAddress);
				if (device.DriverType != GKDriverType.GK && device.DriverType != GKDriverType.RSR2_KAU && device.DriverType != GKDriverType.GKMirror
					&& device.Driver.HasAddress && device.Driver.IsReal && physicalAddress != descriptorStateHelper.PhysicalAddress)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_физический_адрес_устройства;
				}

				var nearestDescriptorNo = 0;
				if (device.KauDatabaseParent != null)
					nearestDescriptorNo = device.KAUDescriptorNo;
				else if (device.GkDatabaseParent != null)
					nearestDescriptorNo = device.GKDescriptorNo;
				if (nearestDescriptorNo != descriptorStateHelper.AddressOnController)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_адрес_на_контроллере;
				}
			}
			if (gkBase is GKZone)
			{
				if (descriptorStateHelper.TypeNo != 0x100)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_зоны;
				}
			}
			if (gkBase is GKDirection)
			{
				if (descriptorStateHelper.TypeNo != 0x106)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_направления;
				}
			}
			if (gkBase is GKPumpStation)
			{
				if (descriptorStateHelper.TypeNo != 0x106)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_НС;
				}
			}
			if (gkBase is GKMPT)
			{
				if (descriptorStateHelper.TypeNo != 0x106)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_МПТ;
				}
			}
			if (gkBase is GKDelay)
			{
				if (descriptorStateHelper.TypeNo != 0x101)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_Задержки;
				}
			}
			if (gkBase is GKPim)
			{
				if (descriptorStateHelper.TypeNo != 0x107)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_ПИМ;
				}
			}
			if (gkBase is GKGuardZone)
			{
				if (descriptorStateHelper.TypeNo != 0x108)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_охранной_зоны;
				}
			}
			if (gkBase is GKCode)
			{
				if (descriptorStateHelper.TypeNo != 0x109)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_кода;
				}
			}
			if (gkBase is GKDoor)
			{
				if (descriptorStateHelper.TypeNo != 0x104)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_кода;
				}
			}

			var description = gkBase.GetGKDescriptorName(GKManager.DeviceConfiguration.GKNameGenerationType);
			if (description != descriptorStateHelper.Description)
			{
				isMissmatch = true;
				DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_описание_компонента;
			}

			gkBase.InternalState.IsDBMissmatchDuringMonitoring = isMissmatch;
			if (isMissmatch)
			{
				IsDBMissmatchDuringMonitoring = true;
			}
		}

		#region TechnologicalRegime
		bool CheckTechnologicalRegime()
		{
			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Проверка технологического режима"))
			{
				gkLifecycleManager.AddItem(GkDatabase.RootDevice.PresentationName);
				var isInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(GkDatabase.RootDevice);
				gkLifecycleManager.AddItem("Обновление состояний технологического режима");
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					descriptor.GKBase.InternalState.IsInTechnologicalRegime = isInTechnologicalRegime;
				}

				if (!isInTechnologicalRegime)
				{
					foreach (var kauDatabase in GkDatabase.KauDatabases)
					{
						gkLifecycleManager.AddItem(kauDatabase.RootDevice.PresentationName);
						var isKAUInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(kauDatabase.RootDevice);
						gkLifecycleManager.AddItem("Обновление состояний технологического режима");
						foreach (var device in kauDatabase.RootDevice.AllChildrenAndSelf)
						{
							device.InternalState.IsInTechnologicalRegime = isKAUInTechnologicalRegime;
						}
					}
				}
				return isInTechnologicalRegime;
			}
		}
		#endregion

		void NotifyAllObjectsStateChanged()
		{
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == GkDatabase.RootDevice.UID);
			if (gkControllerDevice != null)
			{
				GKCallbackResult.GKStates.DeviceStates.RemoveAll(x => x.Device.GkDatabaseParent == gkControllerDevice);

				foreach (var device in gkControllerDevice.AllChildrenAndSelf)
				{
					OnObjectStateChanged(device, false);
				}
				foreach (var device in gkControllerDevice.AllChildrenAndSelf.Where(x => x.Driver.IsGroupDevice || x.DriverType == GKDriverType.RSR2_KAU_Shleif))
				{
					OnObjectStateChanged(device);
				}
				foreach (var zone in GKManager.Zones.Where(x => x.GkDatabaseParent == gkControllerDevice))
				{
					OnObjectStateChanged(zone);
				}
				foreach (var direction in GKManager.Directions.Where(x => x.GkDatabaseParent == gkControllerDevice))
				{
					OnObjectStateChanged(direction);
				}
				foreach (var pumpStation in GKManager.PumpStations.Where(x => x.GkDatabaseParent == gkControllerDevice))
				{
					OnObjectStateChanged(pumpStation);
				}
				foreach (var mpt in GKManager.MPTs.Where(x => x.GkDatabaseParent == gkControllerDevice))
				{
					OnObjectStateChanged(mpt);
				}
				foreach (var delay in GKManager.Delays.Where(x => x.GkDatabaseParent == gkControllerDevice))
				{
					OnObjectStateChanged(delay);
				}
				foreach (var delay in GKManager.AutoGeneratedDelays.Where(x => x.GkDatabaseParent == gkControllerDevice))
				{
					OnObjectStateChanged(delay);
				}
				foreach (var pim in GKManager.AutoGeneratedPims.Where(x => x.GkDatabaseParent == gkControllerDevice))
				{
					OnObjectStateChanged(pim);
				}
				foreach (var guardZone in GKManager.GuardZones.Where(x => x.GkDatabaseParent == gkControllerDevice))
				{
					OnObjectStateChanged(guardZone);
				}
				foreach (var door in GKManager.Doors.Where(x => x.GkDatabaseParent == gkControllerDevice))
				{
					OnObjectStateChanged(door);
				}
			}
		}
	}
}