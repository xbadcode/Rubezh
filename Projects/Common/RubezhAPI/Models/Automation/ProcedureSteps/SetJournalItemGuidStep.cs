﻿using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class SetJournalItemGuidStep : ProcedureStep
	{
		public SetJournalItemGuidStep()
		{
			ValueArgument = new Argument();
		}

		[DataMember]
		public Argument ValueArgument { get; set; }

		public override Argument[] Arguments
		{
			get { return new Argument[] { ValueArgument }; }
		}
	}
}