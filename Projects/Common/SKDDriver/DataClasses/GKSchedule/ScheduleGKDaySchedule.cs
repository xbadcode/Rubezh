﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class ScheduleGKDaySchedule
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? ScheduleUID { get; set; }
		public GKSchedule Schedule { get; set; }

		public Guid? DayScheduleUID { get; set; }
		public GKDaySchedule DaySchedule { get; set; }
	}
}