﻿using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDelay : XBase
	{
		public XDelay()
		{
			UID = BaseUID;// Guid.NewGuid();
			DelayState = new XDelayState()
			{
				Delay = this
			};
		}

		public XDelayState DelayState { get; set; }
		public override XBaseState GetXBaseState() { return DelayState; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ushort DelayTime { get; set; }

		[DataMember]
		public ushort SetTime { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		public override string PresentationName
		{
			get { return Name; }
		}

		public override string DescriptorInfo
		{
			get { return "Задержка " + PresentationName; }
		}
	}
}