﻿using System;
using System.Collections.Generic;
using System.IO;
using Common;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		#region Employee
		public OperationResult<List<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<ShortEmployee>>>(() => FiresecService.GetEmployeeList(filter));
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Employee>>(() => FiresecService.GetEmployeeDetails(uid));
		}
		public OperationResult<bool> SaveEmployee(Employee item, bool isNew)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployee(item, isNew));
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedEmployee(uid, name));
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return SafeContext.Execute(() => FiresecService.GetTimeTracks(filter, startDate, endDate));
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeContext.Execute<Stream>(() => FiresecService.GetTimeTracksStream(filter, startDate, endDate));
			return result;
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployeeDepartment(uid, departmentUid, name));
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid? positionUid, string name)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployeePosition(uid, positionUid, name));
		}
		public OperationResult RestoreEmployee(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreEmployee(uid, name));
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<ShortDepartment>>>(() => FiresecService.GetDepartmentList(filter));
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Department>>(() => FiresecService.GetDepartmentDetails(uid));
		}
		public OperationResult<bool> SaveDepartment(Department item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveDepartment(item, isNew));
		}
		public OperationResult MarkDeletedDepartment(ShortDepartment item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDepartment(item));
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartmentChief(uid, chiefUID, name));
		}
		public OperationResult RestoreDepartment(ShortDepartment item)
		{
			return SafeContext.Execute(() => FiresecService.RestoreDepartment(item));
		}
		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<List<Guid>>>(() => FiresecService.GetChildEmployeeUIDs(uid));
		}
		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<List<Guid>>>(() => FiresecService.GetParentEmployeeUIDs(uid));
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<ShortPosition>>>(() => FiresecService.GetPositionList(filter));
		}

		public OperationResult<List<Guid>> GetPositionEmployees(Guid uid)
		{
			return SafeContext.Execute<OperationResult<List<Guid>>>(() => FiresecService.GetPositionEmployees(uid));
		}
		
		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Position>>(() => FiresecService.GetPositionDetails(uid));
		}
		public OperationResult<bool> SavePosition(Position item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SavePosition(item, isNew));
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedPosition(uid, name));
		}
		public OperationResult RestorePosition(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestorePosition(uid, name));
		}
		#endregion

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() => FiresecService.GetMinJournalDateTime());
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<JournalItem>>>(() => FiresecService.GetFilteredJournalItems(filter));
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.AddJournalItem(journalItem));
		}
		public OperationResult<List<JournalItem>> GetArchivePage(ArchiveFilter filter, int page)
		{
			return SafeContext.Execute<OperationResult<List<JournalItem>>>(() => FiresecService.GetArchivePage(filter, page));
		}
		public OperationResult<int> GetArchiveCount(ArchiveFilter filter)
		{
			return SafeContext.Execute<OperationResult<int>>(() => FiresecService.GetArchiveCount(filter));
		}
		#endregion

		#region Card
        public OperationResult<List<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<SKDCard>>>(() => FiresecService.GetCards(filter));
		}
		public OperationResult<SKDCard> GetSingleCard(Guid uid)
		{
			return SafeContext.Execute<OperationResult<SKDCard>>(() => FiresecService.GetSingleCard(uid));
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
            return SafeContext.Execute<OperationResult<List<SKDCard>>>(() => FiresecService.GetEmployeeCards(employeeUID));
		}
		public OperationResult<bool> AddCard(SKDCard item, string employeeName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.AddCard(item, employeeName));
		}
		public OperationResult<bool> EditCard(SKDCard item, string employeeName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.EditCard(item, employeeName));
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.DeleteCardFromEmployee(item, employeeName, reason));
		}
		public OperationResult DeletedCard(SKDCard card)
		{
			return SafeContext.Execute(() => FiresecService.DeletedCard(card));
		}
		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeContext.Execute(() => FiresecService.SaveCardTemplate(item));
		}
        #endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<AccessTemplate>>>(() => FiresecService.GetAccessTemplates(filter));
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveAccessTemplate(item, isNew));
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAccessTemplate(uid, name));
		}
		public OperationResult RestoreAccessTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreAccessTemplate(uid, name));
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<Organisation>>>(() => FiresecService.GetOrganisations(filter));
		}
		public OperationResult<bool> SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveOrganisation(item, isNew));
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedOrganisation(uid, name));
		}
		public OperationResult AddOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.AddOrganisationDoor(item, doorUID));
		}
		public OperationResult RemoveOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RemoveOrganisationDoor(item, doorUID));
		}
		public OperationResult SaveOrganisationUsers(Organisation item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationUsers(item));
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<OrganisationDetails>>(() => FiresecService.GetOrganisationDetails(uid));
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationChief(uid, chiefUID, name));
		}
		public OperationResult SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationHRChief(uid, chiefUID, name));
		}
		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreOrganisation(uid, name));
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.IsAnyOrganisationItems(uid));
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<AdditionalColumnType>>>(() => FiresecService.GetAdditionalColumnTypes(filter));
		}
		public OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveAdditionalColumnType(item, isNew));
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAdditionalColumnType(uid, name));
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreAdditionalColumnType(uid, name));
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			return SafeContext.Execute<OperationResult<NightSettings>>(() => FiresecService.GetNightSettingsByOrganisation(organisationUID));
		}
		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveNightSettings(nightSettings));
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<ShortPassCardTemplate>>>(() => FiresecService.GetPassCardTemplateList(filter));
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<PassCardTemplate>>(() => FiresecService.GetPassCardTemplateDetails(uid));
		}
		public OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SavePassCardTemplate(item, isNew));
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedPassCardTemplate(uid, name));
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestorePassCardTemplate(uid, name));
		}
		#endregion

		public OperationResult GenerateEmployeeDays()
		{
			return SafeContext.Execute(() => FiresecService.GenerateEmployeeDays());
		}

        public OperationResult GenerateJournal()
        {
            return SafeContext.Execute(() => FiresecService.GenerateJournal());
        }

		public OperationResult GenerateTestData(bool isAscending)
		{
			return SafeContext.Execute(() => FiresecService.GenerateTestData(isAscending));
		}

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveJournalVideoUID(journalItemUID, videoUID, cameraUID));
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveJournalCameraUID(journalItemUID, CameraUID));
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules()
		{
			return SafeContext.Execute<OperationResult<List<GKSchedule>>>(() => FiresecService.GetGKSchedules());
		}

		public OperationResult<bool> SaveGKSchedule(GKSchedule item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveGKSchedule(item, isNew));
		}

		public OperationResult<bool> DeleteGKSchedule(GKSchedule item)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.DeleteGKSchedule(item));
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules()
		{
			return SafeContext.Execute<OperationResult<List<GKDaySchedule>>>(() => FiresecService.GetGKDaySchedules());
		}

		public OperationResult<bool> SaveGKDaySchedule(GKDaySchedule item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveGKDaySchedule(item, isNew));
		}

		public OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule item)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.DeleteGKDaySchedule(item));
		}
		#endregion

		#region Export
		public OperationResult ExportOrganisation(ExportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ExportOrganisation(filter));
		}
		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ImportOrganisation(filter));
		}
		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ExportOrganisationList(filter));
		}
		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ImportOrganisationList(filter));
		}
		public OperationResult ExportJournal(JournalExportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ExportJournal(filter));
		}
		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ExportConfiguration(filter));
		}
		#endregion

		#region CurrentConsumption
		public OperationResult SaveCurrentConsumption(CurrentConsumption item)
		{
			return SafeContext.Execute(() => FiresecService.SaveCurrentConsumption(item));
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter item)
		{
			return SafeContext.Execute(() => FiresecService.GetCurrentConsumption(item));
		}
		#endregion
	}
}