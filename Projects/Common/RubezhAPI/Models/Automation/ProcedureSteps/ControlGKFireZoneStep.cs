﻿using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlGKFireZoneStep : ProcedureStep
	{
		public ControlGKFireZoneStep()
		{
			GKFireZoneArgument = new Argument();
		}

		[DataMember]
		public Argument GKFireZoneArgument { get; set; }

		[DataMember]
		public ZoneCommandType ZoneCommandType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlGKFireZone; } }
		public override Argument[] Arguments
		{
			get { return new Argument[] { GKFireZoneArgument }; }
		}
	}

	public enum ZoneCommandType
	{
		[Description("Перевести в отключенный режим")]
		Ignore,

		[Description("Перевести в автоматический режим")]
		ResetIgnore,

		[Description("Сбросить")]
		Reset
	}
}