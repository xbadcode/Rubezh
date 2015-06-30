﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class NightSetting
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }

		public int NightStartTime { get; set; }

		public int NightEndTime { get; set; }
	}
}
