﻿using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class ScheduleDayInterval : SKDModelBase
	{
		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public string DayIntervalName { get; set; }

		[DataMember]
		public Guid DayIntervalUID { get; set; } 

		[DataMember]
		public Guid ScheduleSchemeUID { get; set; }
	}
}