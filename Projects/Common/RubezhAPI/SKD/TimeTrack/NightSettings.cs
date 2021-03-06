﻿using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class NightSettings : SKDModelBase
	{
		[DataMember]
		public Guid OrganisationUID { get; set; }

		[DataMember]
		public TimeSpan NightStartTime { get; set; }

		[DataMember]
		public TimeSpan NightEndTime { get; set; }
	}
}