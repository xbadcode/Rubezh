﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ServerTask
	{
		[DataMember]
		public string Name { get; set; }
	}
}