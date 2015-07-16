﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class MirrorUser 
	{
		public MirrorUser()
		{
			DateEndAccess = DateTime.Now.AddYears(1);
		}

		public string Name { get; set;}

		public GKCardType Type { get; set;}

		public string Password { get; set; }

		public DateTime DateEndAccess { get; set; }

	}
}
