﻿using System.Collections.Generic;
using FiresecAPI;
using SKDDriver;
using System;
using System.Linq;
using XFiresecAPI;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		SKDDatabaseService _skdService = new SKDDatabaseService();
		
		#region IFiresecService Members
		
		#region Get
		public OperationResult<IEnumerable<Employee>> GetEmployees(EmployeeFilter filter)
		{
			return _skdService.GetEmployees(filter);
		}
		public OperationResult<IEnumerable<Department>> GetDepartments(DepartmentFilter filter)
		{
			return _skdService.GetDepartments(filter);
		}
		public OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter)
		{
			return _skdService.GetPositions(filter);
		}
		public  OperationResult<IEnumerable<SKDJournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return _skdService.GetSKDJournalItems(filter);
		}
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return _skdService.GetCards(filter);
		}
		public OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter)
		{
			return _skdService.GetCardZones(filter);
		}
		public OperationResult<IEnumerable<Organization>> GetOrganizations(OrganizationFilter filter)
		{
			return _skdService.GetOrganizations(filter);
		}
		public OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter)
		{
			return _skdService.GetDocuments(filter);
		}
		public OperationResult<IEnumerable<GUD>> GetGUDs(GUDFilter filter)
		{
			return _skdService.GetGUDs(filter);
		}
		#endregion

		#region Save
		public OperationResult SaveEmployees(IEnumerable<Employee> Employees)
		{
			return _skdService.SaveEmployees(Employees);
		}
		public OperationResult SaveDepartments(IEnumerable<Department> Departments)
		{
			return _skdService.SaveDepartments(Departments);
		}
		public OperationResult SavePositions(IEnumerable<Position> Positions)
		{
			return _skdService.SavePositions(Positions);
		}
		public OperationResult SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			return _skdService.SaveSKDJournalItems(journalItems);
		}
		public OperationResult SaveCards(IEnumerable<SKDCard> items)
		{
			return _skdService.SaveCards(items);
		}
		public OperationResult SaveCardZones(IEnumerable<CardZone> items)
		{
			return _skdService.SaveCardZones(items);
		}
		public OperationResult SaveOrganizations(IEnumerable<Organization> Organizations)
		{
			return _skdService.SaveOrganizations(Organizations);
		}
		public OperationResult SaveDocuments(IEnumerable<Document> items)
		{
			return _skdService.SaveDocuments(items);
		}
		public OperationResult SaveGUDs(IEnumerable<GUD> items)
		{
			return _skdService.SaveGUDs(items);
		}
		#endregion

		#region MarkDeleted
		public OperationResult MarkDeletedEmployees(IEnumerable<Employee> Employees)
		{
			return _skdService.MarkDeletedEmployees(Employees);
		}
		public OperationResult MarkDeletedDepartments(IEnumerable<Department> Departments)
		{
			return _skdService.MarkDeletedDepartments(Departments);
		}
		public OperationResult MarkDeletedPositions(IEnumerable<Position> Positions)
		{
			return _skdService.MarkDeletedPositions(Positions);
		}
		public OperationResult MarkDeletedSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			return _skdService.MarkDeletedSKDJournalItems(journalItems);
		}
		public OperationResult MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			return _skdService.MarkDeletedCards(items);
		}
		public OperationResult MarkDeletedCardZones(IEnumerable<CardZone> items)
		{
			return _skdService.MarkDeletedCardZones(items);
		}
		public OperationResult MarkDeletedOrganizations(IEnumerable<Organization> Organizations)
		{
			return _skdService.MarkDeletedOrganizations(Organizations);
		}
		public OperationResult MarkDeletedDocuments(IEnumerable<Document> items)
		{
			return _skdService.MarkDeletedDocuments(items);
		}
		public OperationResult MarkDeletedGUDs(IEnumerable<GUD> items)
		{
			return _skdService.MarkDeletedGUDs(items);
		}
		#endregion
		#endregion

		#region Devices
		public OperationResult<string> SKDGetDeviceInfo(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x=>x.UID == deviceUID);
			if(device != null)
			{
				return new OperationResult<string>() { Result = SKDProcessorManager.SKDGetDeviceInfo(device, UserName) };
			}
			return new OperationResult<string>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return new OperationResult<bool>() { Result = SKDProcessorManager.SKDSyncronyseTime(device, UserName) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDWriteConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return SKDProcessorManager.GKWriteConfiguration(device, UserName);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return SKDProcessorManager.GKUpdateFirmware(device, fileName, UserName);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public void SKDSetRegimeOpen(Guid deviceUID)
		{

		}

		public void SKDSetRegimeClose(Guid deviceUID)
		{

		}

		public void SKDSetRegimeControl(Guid deviceUID)
		{

		}

		public void SKDSetRegimeConversation(Guid deviceUID)
		{

		}

		public void SKDOpenDevice(Guid deviceUID)
		{

		}

		public void SKDCloseDevice(Guid deviceUID)
		{

		}

		public void SKDAllowReader(Guid deviceUID)
		{

		}

		public void SKDDenyReader(Guid deviceUID)
		{

		}
		#endregion
	}
}