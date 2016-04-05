﻿using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ShowPropertyStep : UIStep
	{
		public ShowPropertyStep()
		{
			ObjectArgument = new Argument();
			LayoutFilter.Add(System.Guid.Empty);
		}

		[DataMember]
		public Argument ObjectArgument { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ShowProperty; } }
	}
}