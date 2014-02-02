﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDSystemConfiguration
	{
		public SKDSystemConfiguration()
		{
			JournalFilters = new List<SKDJournalFilter>();
		}

		[DataMember]
		public Guid ReaderDeviceUID { get; set; }

		[DataMember]
		public bool ShowEmployeeCardID { get; set; }

		[DataMember]
		public bool ShowEmployeeName { get; set; }

		[DataMember]
		public bool ShowEmployeePassport { get; set; }

		[DataMember]
		public bool ShowEmployeeTime { get; set; }

		[DataMember]
		public bool ShowEmployeeNo { get; set; }

		[DataMember]
		public bool ShowEmployeePosition { get; set; }

		[DataMember]
		public bool ShowEmployeeShedule { get; set; }

		[DataMember]
		public bool ShowEmployeeDepartment { get; set; }

		[DataMember]
		public bool ShowGuestCardID { get; set; }

		[DataMember]
		public bool ShowGuestName { get; set; }

		[DataMember]
		public bool ShowGuestWhere { get; set; }

		[DataMember]
		public bool ShowGuestConvoy { get; set; }

		[DataMember]
		public List<SKDJournalFilter> JournalFilters { get; set; }
	}
}