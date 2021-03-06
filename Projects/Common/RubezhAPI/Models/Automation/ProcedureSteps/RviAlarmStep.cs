﻿using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class RviAlarmStep : ProcedureStep
	{
		public RviAlarmStep()
		{
			NameArgument = new Argument();
			NameArgument.StringValue = "";
		}

		[DataMember]
		public Argument NameArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.RviAlarm; } }
		public override Argument[] Arguments
		{
			get { return new Argument[] { NameArgument }; }
		}
	}
}