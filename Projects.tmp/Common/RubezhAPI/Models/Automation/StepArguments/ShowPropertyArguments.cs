﻿using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ShowPropertyArguments : UIArguments
	{
		public ShowPropertyArguments()
		{
			ObjectArgument = new Argument();
		}

		[DataMember]
		public Argument ObjectArgument { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }
	}
}