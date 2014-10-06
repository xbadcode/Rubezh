﻿using System;
using System.Runtime.Serialization;
using FiresecAPI.Journal;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKJournalItem
	{
		public GKJournalItem()
		{
			DeviceDateTime = DateTime.Now;
			SystemDateTime = DateTime.Now;
			ObjectStateClass = XStateClass.Norm;
			JournalObjectType = GKJournalObjectType.System;
		}

		[DataMember]
		public GKJournalObjectType JournalObjectType { get; set; }
		[DataMember]
		public DateTime DeviceDateTime { get; set; }
		[DataMember]
		public DateTime SystemDateTime { get; set; }
		[DataMember]
		public int? GKJournalRecordNo { get; set; }

		[DataMember]
		public JournalEventNameType JournalEventNameType { get; set; }
		[DataMember]
		public JournalEventDescriptionType JournalEventDescriptionType { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public Guid ObjectUID { get; set; }
		[DataMember]
		public string ObjectName { get; set; }
		[DataMember]
		public int ObjectState { get; set; }
		[DataMember]
		public ushort GKObjectNo { get; set; }
		[DataMember]
		public string GKIpAddress { get; set; }
		[DataMember]
		public XStateClass ObjectStateClass { get; set; }

		[DataMember]
		public ushort ControllerAddress { get; set; }
		[DataMember]
		public string AdditionalDescription { get; set; }

		[DataMember]
		public ushort DescriptorType { get; set; }
		[DataMember]
		public ushort DescriptorAddress { get; set; }

		[DataMember]
		public string UserName { get; set; }
		[DataMember]
		public GKSubsystemType SubsystemType { get; set; }
	}
}