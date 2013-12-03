﻿using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XAUParameter
	{
		[DataMember]
		public byte No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool IsDelay { get; set; }

		[DataMember]
		public bool IsHighByte { get; set; }

		[DataMember]
		public bool IsLowByte { get; set; }

		[DataMember]
		public string InternalName { get; set; }

		[DataMember]
		public double? Multiplier { get; set; }
	}
}