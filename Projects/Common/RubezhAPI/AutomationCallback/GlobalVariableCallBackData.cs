﻿using RubezhAPI.Automation;
using System;
using System.Runtime.Serialization;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	public class GlobalVariableCallBackData : AutomationCallbackData
	{
		[DataMember]
		public Guid? InitialClientUID { get; set; }

		[DataMember]
		public Guid VariableUID { get; set; }

		[DataMember]
		public ExplicitValue ExplicitValue { get; set; }
	}
}
