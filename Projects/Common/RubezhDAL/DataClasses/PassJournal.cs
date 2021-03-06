﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubezhDAL.DataClasses
{
	public class PassJournal
	{
		[Key]
		public Guid UID { get; set; }
		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }

		public Guid ZoneUID { get; set; }

		public DateTime EnterTime { get; set; }

		public DateTime? ExitTime { get; set; }
	}
}
