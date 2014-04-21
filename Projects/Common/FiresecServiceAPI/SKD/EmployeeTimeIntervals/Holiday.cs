﻿using System;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class Holiday : OrganizationElementBase
	{
		public Holiday()
		{
			Type = HolidayType.Holiday;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public HolidayType Type { get; set; }

		[DataMember]
		public DateTime Date { get; set; }

		[DataMember]
		public DateTime? TransferDate { get; set; }

		[DataMember]
		public TimeSpan Reduction { get; set; }
	}
}