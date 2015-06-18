﻿using System;

namespace SKDDriver.DataClasses
{
	public class DbService : IDisposable
	{
		public SKDDbContext Context;

		public GKScheduleTranslator GKScheduleTranslator { get; private set; }
		public GKDayScheduleTranslator GKDayScheduleTranslator { get; private set; }
		public PassJournalTranslator PassJournalTranslator { get; private set; }
		public JournalTranslator JournalTranslator { get; private set; }
		public AccessTemplateTranslator AccessTemplateTranslator { get; private set; }
		public AdditionalColumnTypeTranslator AdditionalColumnTypeTranslator { get; private set; }
		public CardTranslator CardTranslator { get; private set; }
		public CurrentConsumptionTranslator CurrentConsumptionTranslator { get; private set; }
		public DayIntervalTranslator DayIntervalTranslator { get; private set; }
		public DepartmentTranslator DepartmentTranslator { get; private set; }
		public EmployeeTranslator EmployeeTranslator { get; private set; }
		public HolidayTranslator HolidayTranslator { get; private set; }
		public NightSettingTranslator NightSettingTranslator { get; private set; }
		public OrganisationTranslator OrganisationTranslator { get; private set; }
		public PassCardTemplateTranslator PassCardTemplateTranslator { get; private set; }
		public PositionTranslator PositionTranslator { get; private set; }
		public ScheduleTranslator ScheduleTranslator { get; private set; }
		public ScheduleSchemeTranslator ScheduleSchemeTranslator { get; private set; }
		public TimeTrackingTranslator TimeTrackingTranslator { get; private set; }

		public static bool IsAbort
		{
			get { return JournalTranslator.IsAbort; }
			set { JournalTranslator.IsAbort = value; }
		}

		public DbService()
		{
			Context = new SKDDbContext("SKDDbContext", ContextType.PostgreSQL);
			GKScheduleTranslator = new GKScheduleTranslator(this);
			GKDayScheduleTranslator = new GKDayScheduleTranslator(this);
			PassJournalTranslator = new PassJournalTranslator(this);
			JournalTranslator = new JournalTranslator(this);
			AccessTemplateTranslator = new AccessTemplateTranslator(this);
			//AdditionalColumnTypeTranslator = new AdditionalColumnTypeTranslator(this);
			CardTranslator = new CardTranslator(this);
			//CurrentConsumptionTranslator = new CurrentConsumptionTranslator(this);
			DayIntervalTranslator = new DayIntervalTranslator(this);
			//DepartmentTranslator = new DepartmentTranslator(this);
			//EmployeeTranslator = new EmployeeTranslator(this);
			//HolidayTranslator = new HolidayTranslator(this);
			//NightSettingTranslator = new NightSettingTranslator(this);
			//OrganisationTranslator = new OrganisationTranslator(this);
			//PassCardTemplateTranslator = new PassCardTemplateTranslator(this);
			//PositionTranslator = new PositionTranslator(this);
			ScheduleTranslator = new ScheduleTranslator(this);
			ScheduleSchemeTranslator = new ScheduleSchemeTranslator(this);
			//TimeTrackingTranslator = new TimeTrackingTranslator(this);
		}

		public void Dispose()
		{
			Context.Dispose();
		}
	}

	public class AdditionalColumnTypeTranslator { }
	public class CurrentConsumptionTranslator { }
	public class DepartmentTranslator { }
	public class EmployeeTranslator { }
	public class HolidayTranslator { }
	public class NightSettingTranslator { }
	public class OrganisationTranslator { }
	public class PassCardTemplateTranslator { }
	public class PositionTranslator { }
	public class TimeTrackingTranslator { }

}