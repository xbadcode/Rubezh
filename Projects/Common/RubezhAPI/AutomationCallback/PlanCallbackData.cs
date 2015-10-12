﻿using System;
using System.Runtime.Serialization;
using RubezhAPI.Automation;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	public class PlanCallbackData : AutomationCallbackData
	{
		[DataMember]
		public Guid PlanUid { get; set; }

		[DataMember]
		public Guid ElementUid { get; set; }

		[DataMember]
		public ElementPropertyType ElementPropertyType { get; set; }

		[DataMember]
		public object Value { get; set; }
	}
}
