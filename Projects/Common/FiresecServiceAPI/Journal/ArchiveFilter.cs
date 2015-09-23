﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Journal
{
	[DataContract]
	public class ArchiveFilter
	{
		public ArchiveFilter()
		{
			StartDate = DateTime.Now.AddDays(-1);
			EndDate = DateTime.Now;
			UseDeviceDateTime = false;
			JournalSubsystemTypes = new List<JournalSubsystemType>();
			JournalEventNameTypes = new List<JournalEventNameType>();
			JournalEventDescriptionTypes = new List<JournalEventDescriptionType>();
			JournalObjectTypes = new List<JournalObjectType>();
			ObjectUIDs = new List<Guid>();
			EmployeeUIDs = new List<Guid>();
            Users = new List<string>();
			SortType = Journal.ArchiveSortType.SystemDate;
        }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public bool UseDeviceDateTime { get; set; }

		[DataMember]
		public List<JournalSubsystemType> JournalSubsystemTypes { get; set; }

		[DataMember]
		public List<JournalEventNameType> JournalEventNameTypes { get; set; }

		[DataMember]
		public List<JournalEventDescriptionType> JournalEventDescriptionTypes { get; set; }

		[DataMember]
		public List<JournalObjectType> JournalObjectTypes { get; set; }

		[DataMember]
		public List<Guid> ObjectUIDs { get; set; }

		[DataMember]
		public List<Guid> EmployeeUIDs { get; set; }

        [DataMember]
        public List<string> Users { get; set; }

		[DataMember]
		public int PageSize { get; set; }

		[DataMember]
		public ArchiveSortType SortType { get; set; }

		[DataMember]
		public bool IsSortAsc { get; set; }
	}

	public enum ArchiveSortType
	{
		[Description("Дата в системе")]
		SystemDate = 0,
		[Description("Дата в приборе")]
		DeviceDate = 1,
		[Description("Сотрудник")]
		EmployeeUID = 2,
		[Description("Подсистема")]
		Subsystem = 3,
		[Description("Событие")]
		Name = 4,
		[Description("Уточнение")]
		Description = 5,
		[Description("Тип объекта")]
		ObjectType = 6,
		[Description("Объект")]
		ObjectName = 7,
		[Description("Пользователь")]
		UserName = 8,
		[Description("Камера")]
		CameraUID = 9,
		[Description("Пропуск")]
		CardNo = 10
	}
}