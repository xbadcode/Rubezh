﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient;
using System.Runtime.Serialization;
using FiresecAPI;

namespace GKProcessor
{
	public static class GKProcessorManager
	{
		public static void OnStartProgress(string name, int count, bool canCancel = true)
		{
			if (DoProgress != null)
				StartProgress(name, count, canCancel);
		}
		public static event Action<string, int, bool> StartProgress;

		public static void OnDoProgress(string name)
		{
			if (DoProgress != null)
				DoProgress(name);
		}
		public static event Action<string> DoProgress;

		public static void OnStopProgress()
		{
			if (StopProgress != null)
				StopProgress();
		}
		public static event Action StopProgress;

		public static void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			if (gkCallbackResult.JournalItems.Count + gkCallbackResult.DeviceStates.Count + gkCallbackResult.ZoneStates.Count + gkCallbackResult.DirectionStates.Count > 0)
			{
				if (GKCallbackResultEvent != null)
					GKCallbackResultEvent(gkCallbackResult);
			}
		}
		public static event Action<GKCallbackResult> GKCallbackResultEvent;

		#region Operations
		public static OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device)
		{
			AddGKMessage("Чтение конфигурации из прибора", "", XStateClass.Info, device, true);
			var descriptorReader = device.Driver.IsKauOrRSR2Kau ? (DescriptorReaderBase)new KauDescriptorsReaderBase() : new GkDescriptorsReaderBase();
			descriptorReader.ReadConfiguration(device);
			return new OperationResult<XDeviceConfiguration> { HasError = !string.IsNullOrEmpty(descriptorReader.Error), Error = descriptorReader.Error, Result = descriptorReader.DeviceConfiguration };
		}

		public static void GKUpdateFirmware(XDevice device, string fileName)
		{
			AddGKMessage("Обновление ПО прибора", "", XStateClass.Info, device, true);
			FirmwareUpdateHelper.Update(device, fileName);
		}

		public static bool GKSyncronyseTime(XDevice device)
		{
			AddGKMessage("Синхронизация времени", "", XStateClass.Info, device, true);
			return DeviceBytesHelper.WriteDateTime(device);
		}

		public static string GKGetDeviceInfo(XDevice device)
		{
			AddGKMessage("Запрос информации об устройсве", "", XStateClass.Info, device, true);
			return DeviceBytesHelper.GetDeviceInfo(device);
		}

		public static OperationResult<int> GKGetJournalItemsCount(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 6, 64);
			if (sendResult.HasError)
			{
				return new OperationResult<int>("Ошибка связи с устройством");
			}
			var journalParser = new JournalParser(device, sendResult.Bytes);
			var result = journalParser.JournalItem.GKJournalRecordNo.Value;
			return new OperationResult<int>() { Result = result };
		}

		public static OperationResult<JournalItem> GKReadJournalItem(XDevice device, int no)
		{
			var data = BitConverter.GetBytes(no).ToList();
			var sendResult = SendManager.Send(device, 4, 7, 64, data);
			if (sendResult.HasError)
			{
				return new OperationResult<JournalItem>("Ошибка связи с устройством");
			}
			var journalParser = new JournalParser(device, sendResult.Bytes);
			return new OperationResult<JournalItem>() { Result = journalParser.JournalItem };
		}

		public static OperationResult<bool> GKSetSingleParameter(XDevice device)
		{
			var error = ParametersHelper.SetSingleParameter(device);
			return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
		}

		public static OperationResult<bool> GKGetSingleParameter(XDevice device)
		{
			var error = ParametersHelper.GetSingleParameter(device);
			return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
		}

		public static GKStates GKGetStates()
		{
			var gkStates = new GKStates();
			foreach (var device in XManager.Devices)
			{
				gkStates.DeviceStates.Add(device.DeviceState);
			}
			foreach (var direction in XManager.Directions)
			{
				gkStates.DirectionStates.Add(direction.DirectionState);
			}
			foreach (var zone in XManager.Zones)
			{
				gkStates.ZoneStates.Add(zone.ZoneState);
			}
			return gkStates;
		}

		public static void GKExecuteDeviceCommand(XDevice device, XStateBit stateBit)
		{
			Watcher.SendControlCommand(device, stateBit);
			AddGKMessage("Команда оператора", stateBit.ToDescription(), XStateClass.Info, device);
		}

		public static void GKReset(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Reset);
			AddGKMessage("Команда оператора", "Сброс", XStateClass.Info, xBase);
		}

		public static void GKResetFire1(XZone zone)
		{
			Watcher.SendControlCommand(zone, 0x02);
			AddGKMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
		}

		public static void GKResetFire2(XZone zone)
		{
			Watcher.SendControlCommand(zone, 0x03);
			AddGKMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
		}

		public static void GKSetAutomaticRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic);
			AddGKMessage("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, xBase);
		}

		public static void GKSetManualRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual);
			AddGKMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
		}

		public static void GKSetIgnoreRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off);
			AddGKMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
		}

		public static void GKTurnOn(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual);
			AddGKMessage("Команда оператора", "Включить", XStateClass.Info, xBase);
		}

		public static void GKTurnOnNow(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual);
			AddGKMessage("Команда оператора", "Включить немедленно", XStateClass.Info, xBase);
		}

		public static void GKTurnOff(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual);
			AddGKMessage("Команда оператора", "Выключить", XStateClass.Info, xBase);
		}

		public static void GKTurnOffNow(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual);
			AddGKMessage("Команда оператора", "Выключить немедленно", XStateClass.Info, xBase);
		}

		public static void GKStop(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Stop_InManual);
			AddGKMessage("Команда оператора", "Остановка пуска", XStateClass.Info, xBase);
		}
		#endregion

		#region JournalItem Callback
		public static void AddGKMessage(string message, string description, XStateClass stateClass, XBase xBase, bool isAdministrator = false)
		{
			Guid uid = Guid.Empty;
			var journalItemType = JournalItemType.System;
			if (xBase != null)
			{
				if (xBase is XDevice)
				{
					uid = (xBase as XDevice).UID;
					journalItemType = JournalItemType.Device;
				}
				if (xBase is XZone)
				{
					uid = (xBase as XZone).UID;
					journalItemType = JournalItemType.Zone;
				}
				if (xBase is XDirection)
				{
					uid = (xBase as XDirection).UID;
					journalItemType = JournalItemType.Direction;
				}
				if (xBase is XDelay)
				{
					uid = (xBase as XDelay).UID;
					journalItemType = JournalItemType.Delay;
				}
			}

			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalItemType = journalItemType,
				StateClass = stateClass,
				Name = message,
				Description = description,
				ObjectUID = uid,
				ObjectStateClass = XStateClass.Norm,
				//UserName = FiresecManager.CurrentUser.Name,
				SubsystemType = XSubsystemType.System
			};
			if (xBase != null)
			{
				journalItem.ObjectName = xBase.PresentationName;
				journalItem.GKObjectNo = (ushort)xBase.GKDescriptorNo;
			}

			GKDBHelper.Add(journalItem);
			OnNewJournalItem(journalItem, isAdministrator);
		}

		public static event Action<JournalItem, bool> NewJournalItem;
		static void OnNewJournalItem(JournalItem journalItem, bool isAdministrator)
		{
			if (NewJournalItem != null)
				NewJournalItem(journalItem, isAdministrator);
		}
		#endregion
	}
}