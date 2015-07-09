﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class OrganisationDoor
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }

		public Guid DoorUID { get; set; }
	}
}