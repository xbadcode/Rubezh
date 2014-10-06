﻿using System;
using System.Linq;
using FiresecAPI.SKD;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class ScheduleTranslator : WithShortTranslator<DataAccess.Schedule, Schedule, ScheduleFilter, ShortSchedule>
	{
		public ScheduleTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override IQueryable<DataAccess.Schedule> GetQuery(ScheduleFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Name);
		}

		protected override OperationResult CanSave(Schedule item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.OrganisationUID == item.OrganisationUID && x.UID != item.UID && !x.IsDeleted && x.Name == item.Name);
			if (hasSameName)
				return new OperationResult("График с таким же названием уже содержится в базе данных");
			return new OperationResult();
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.Employees.Any(item => !item.IsDeleted && item.ScheduleUID == uid))
				return new OperationResult("Невозможно удалить график работ, т.к. он назначен одному из сотрудников");
			return base.CanDelete(uid);
		}

		protected override Schedule Translate(DataAccess.Schedule tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.ScheduleSchemeUID = tableItem.ScheduleSchemeUID.HasValue ? tableItem.ScheduleSchemeUID.Value : Guid.Empty;
			result.IsIgnoreHoliday = tableItem.IsIgnoreHoliday;
			result.IsOnlyFirstEnter = tableItem.IsOnlyFirstEnter;
			result.AllowedLate = TimeSpan.FromSeconds(tableItem.AllowedLate);
			result.AllowedEarlyLeave = TimeSpan.FromSeconds(tableItem.AllowedEarlyLeave);
			result.Zones = DatabaseService.ScheduleZoneTranslator.TranslateAll(tableItem.ScheduleZones);
			return result;
		}

		protected override ShortSchedule TranslateToShort(DataAccess.Schedule tableItem)
		{
			return new ShortSchedule
			{
				UID = tableItem.UID,
				Name = tableItem.Name
			};
		}

		protected override void TranslateBack(DataAccess.Schedule tableItem, Schedule apiItem)
		{
			var scheduleScheme = apiItem.ScheduleSchemeUID == Guid.Empty ? null : Context.ScheduleSchemes.FirstOrDefault(item => item.UID == apiItem.ScheduleSchemeUID);
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.IsIgnoreHoliday = apiItem.IsIgnoreHoliday;
			tableItem.IsOnlyFirstEnter = apiItem.IsOnlyFirstEnter;
			tableItem.AllowedLate = (int)apiItem.AllowedLate.TotalSeconds;
			tableItem.AllowedEarlyLeave = (int)apiItem.AllowedEarlyLeave.TotalSeconds;
			if (scheduleScheme == null && apiItem.ScheduleSchemeUID != Guid.Empty)
				tableItem.ScheduleSchemeUID = apiItem.ScheduleSchemeUID;
			else
				tableItem.ScheduleScheme = scheduleScheme;
			DatabaseService.ScheduleZoneTranslator.Save(apiItem.Zones);
		}

		public string GetName(Guid? uid)
		{
			if (uid == null)
				return "";
			var tableItem = Table.FirstOrDefault(x => x.UID == uid.Value);
			if (tableItem == null)
				return "";
			return tableItem.Name;
		}
	}
}