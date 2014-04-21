﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class NamedInterval : OrganizationElementBase
	{
		public NamedInterval()
		{
			TimeIntervals = new List<TimeInterval>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public TimeSpan SlideTime { get; set; }

		[DataMember]
		public List<TimeInterval> TimeIntervals { get; set; }
	}
}