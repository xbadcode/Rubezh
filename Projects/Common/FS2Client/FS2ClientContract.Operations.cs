﻿using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;

namespace FS2Client
{
	public partial class FS2ClientContract
	{
		#region Main
		public List<FS2Callbac> Poll(Guid clientUID)
		{
			return SafeOperationCall(() => { return FS2Contract.Poll(clientUID); }, "Poll");
		}

		public void CancelPoll(Guid clientUID)
		{
			SafeOperationCall(() => { FS2Contract.CancelPoll(clientUID); }, "CancelPoll");
		}

		public void CancelProgress()
		{
			SafeOperationCall(() => { FS2Contract.CancelProgress(); }, "CanceProgress");
		}
		#endregion

		#region Common
		public OperationResult<DeviceConfiguration> GetDeviceConfiguration()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDeviceConfiguration(); }, "GetDeviceConfiguration");
		}

		public OperationResult<DriversConfiguration> GetDriversConfiguration()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDriversConfiguration(); }, "GetDriversConfiguration");
		}
		#endregion

		#region Monitor
		public OperationResult<List<DeviceState>> GetDeviceStates()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDeviceStates(); }, "GetDeviceStates");
		}

		public OperationResult<List<DeviceState>> GetDeviceParameters()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDeviceParameters(); }, "GetDeviceParameters");
		}

		public OperationResult<List<ZoneState>> GetZoneStates()
		{
			return SafeOperationCall(() => { return FS2Contract.GetZoneStates(); }, "GetZoneStates");
		}

		public OperationResult AddToIgnoreList(List<Guid> deviceUIDs, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.AddToIgnoreList(deviceUIDs, userName); }, "AddToIgnoreList");
		}

		public OperationResult RemoveFromIgnoreList(List<Guid> deviceUIDs, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.RemoveFromIgnoreList(deviceUIDs, userName); }, "RemoveFromIgnoreList");
		}

		public OperationResult SetZoneGuard(Guid zoneUID, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.SetZoneGuard(zoneUID, userName); }, "SetZoneGuard");
		}

		public OperationResult UnSetZoneGuard(Guid zoneUID, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.UnSetZoneGuard(zoneUID, userName); }, "UnSetZoneGuard");
		}

		public OperationResult SetDeviceGuard(Guid deviceUID, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.SetDeviceGuard(deviceUID, userName); }, "SetDeviceGuard");
		}

		public OperationResult UnSetDeviceGuard(Guid deviceUID, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.UnSetDeviceGuard(deviceUID, userName); }, "UnSetDeviceGuard");
		}

		public OperationResult ResetStates(List<PanelResetItem> panelResetItems, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.ResetStates(panelResetItems, userName); }, "ResetStates");
		}

		public OperationResult ExecuteCommand(Guid deviceUID, string methodName, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.ExecuteCommand(deviceUID, methodName, userName); }, "ExecuteCommand");
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return SafeOperationCall<bool>(() => { return FS2Contract.CheckHaspPresence(); }, "CheckHaspPresence");
		}
		#endregion

		#region Administrator
		public OperationResult SetNewConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.SetNewConfiguration(deviceConfiguration, userName); }, "SetNewConfiguration");
		}

		public OperationResult<bool> DeviceWriteConfiguration(Guid deviceUID, bool isUSB, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceWriteConfiguration(deviceUID, isUSB, userName); }, "DeviceWriteConfiguration");
		}

		public OperationResult<List<Guid>> DeviceWriteAllConfiguration(List<Guid> deviceUIDs, string userName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceWriteAllConfiguration(deviceUIDs, userName); }, "DeviceWriteAllConfiguration");
		}

		public OperationResult DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceSetPassword(deviceUID, isUSB, devicePasswordType, password); }, "DeviceSetPassword");
		}

		public OperationResult DeviceDatetimeSync(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceDatetimeSync(deviceUID, isUSB); }, "DeviceDatetimeSync");
		}

		public OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetInformation(deviceUID, isUSB); }, "DeviceGetInformation");
		}

		public OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetSerialList(deviceUID); }, "DeviceGetSerialList");
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceVerifyFirmwareVersion(deviceUID, isUSB, fileName); }, "DeviceVerifyFirmwareVersion");
		}

		public OperationResult DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceUpdateFirmware(deviceUID, isUSB, fileName); }, "DeviceUpdateFirmware");
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfiguration(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceReadConfiguration(deviceUID, isUSB); }, "DeviceReadConfiguration");
		}

		public OperationResult<FS2JournalItemsCollection> DeviceReadJournal(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceReadJournal(deviceUID, isUSB); }, "DeviceReadJournal");
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceAutoDetectChildren(deviceUID, fastSearch); }, "DeviceAutoDetectChildren");
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceGetCustomFunctions(DriverType driverType)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetCustomFunctions(driverType); }, "DeviceGetCustomFunctions");
		}

		public OperationResult DeviceExecuteCustomFunction(Guid deviceUID, bool isUSB, string functionName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceExecuteCustomFunction(deviceUID, isUSB, functionName); }, "DeviceExecuteCustomFunction");
		}

		public OperationResult<List<GuardUser>> DeviceGetGuardUsers(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetGuardUsers(deviceUID); }, "DeviceGetGuardUsers");
		}

		public OperationResult DeviceSetGuardUsers(Guid deviceUID, List<GuardUser> guardUser)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceSetGuardUsers(deviceUID, guardUser); }, "DeviceSetGuardUsers");
		}

		public OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetMDS5Data(deviceUID); }, "DeviceGetMDS5Data");
		}

		public OperationResult SetAuParameters(Guid deviceUID, List<Property> properties)
		{
			return SafeOperationCall(() => { return FS2Contract.SetAuParameters(deviceUID, properties); }, "SetAuParameters");
		}

		public OperationResult<List<Property>> GetAuParameters(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.GetAuParameters(deviceUID); }, "GetAuParameters");
		}
		#endregion

		#region Journal
		public OperationResult<List<FS2JournalItem>> GetFilteredJournal(JournalFilter journalFilter)
		{
			return SafeOperationCall(() => { return FS2Contract.GetFilteredJournal(journalFilter); }, "GetFilteredJournal");
		}

		public OperationResult<List<FS2JournalItem>> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return SafeOperationCall(() => { return FS2Contract.GetFilteredArchive(archiveFilter); }, "GetFilteredArchive");
		}

		public OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return SafeOperationCall(() => { return FS2Contract.BeginGetFilteredArchive(archiveFilter); }, "BeginGetFilteredArchive");
		}

		public OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDistinctDescriptions(); }, "GetDistinctDescriptions");
		}

		public OperationResult<DateTime> GetArchiveStartDate()
		{
			return SafeOperationCall(() => { return FS2Contract.GetArchiveStartDate(); }, "GetArchiveStartDate");
		}

		public OperationResult AddJournalRecords(List<FS2JournalItem> journalItems)
		{
			return SafeOperationCall(() => { return FS2Contract.AddJournalRecords(journalItems); }, "AddJournalRecords");
		}
		#endregion
	}
}