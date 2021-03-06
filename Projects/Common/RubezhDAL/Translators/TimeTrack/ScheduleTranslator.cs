﻿using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public class ScheduleTranslator : OrganisationItemTranslatorBase<Schedule, API.Schedule, API.ScheduleFilter>
	{
		public ScheduleTranslator(DbService context)
			: base(context) { }
		public override DbSet<Schedule> Table
		{
			get { return Context.Schedules; }
		}

		public override IQueryable<Schedule> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.ScheduleZones);
		}

		public override void TranslateBack(API.Schedule apiItem, Schedule tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.IsIgnoreHoliday = apiItem.IsIgnoreHoliday;
			tableItem.IsOnlyFirstEnter = apiItem.IsOnlyFirstEnter;
			tableItem.AllowedLateTimeSpan = apiItem.AllowedLate;
			tableItem.AllowedEarlyLeaveTimeSpan = apiItem.AllowedEarlyLeave;
			tableItem.ScheduleSchemeUID = apiItem.ScheduleSchemeUID != Guid.Empty ? (Guid?)apiItem.ScheduleSchemeUID : null;
			tableItem.ScheduleZones = apiItem.Zones.Select(x => new ScheduleZone
			{
				DoorUID = x.DoorUID,
				ScheduleUID = x.ScheduleUID,
				UID = x.UID,
				ZoneUID = x.ZoneUID
			}).ToList();
		}

		protected override void ClearDependentData(Schedule tableItem)
		{
			Context.ScheduleZones.RemoveRange(tableItem.ScheduleZones);
		}

		protected override IEnumerable<API.Schedule> GetAPIItems(IQueryable<Schedule> tableItems)
		{
			return tableItems.Select(tableItem => new API.Schedule
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				ScheduleSchemeUID = tableItem.ScheduleSchemeUID.HasValue ? tableItem.ScheduleSchemeUID.Value : Guid.Empty,
				IsIgnoreHoliday = tableItem.IsIgnoreHoliday,
				IsOnlyFirstEnter = tableItem.IsOnlyFirstEnter,
				AllowedLate = tableItem.AllowedLateTimeSpan,
				AllowedEarlyLeave = tableItem.AllowedEarlyLeaveTimeSpan,
				Zones = tableItem.ScheduleZones.Select(x => new API.ScheduleZone
				{
					DoorUID = x.DoorUID,
					ScheduleUID = x.ScheduleUID.Value,
					UID = x.UID,
					ZoneUID = x.ZoneUID
				}).ToList()
			});
		}
	}
}