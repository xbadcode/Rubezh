﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GKImitator.Processor
{
	[DataContract]
	public class ImitatorJournalItem
	{
		public ImitatorJournalItem()
		{
			DateTime = DateTime.Now;
			ObjectDeviceType = 0;
			ObjectDeviceAddress = 0;
		}

		public ImitatorJournalItem(byte source, byte nameCode, byte descriptionCode, byte yesNoCode)
		{
			Source = source;
			NameCode = nameCode;
			DescriptionCode = descriptionCode;
			YesNoCode = yesNoCode;
		}

		[DataMember]
		public int GkNo { get; set; }
		[DataMember]
		public int GkObjectNo { get; set; }
		[DataMember]
		public int UNUSED_KauNo { get; set; }

		[DataMember]
		public DateTime DateTime { get; set; }

		[DataMember]
		public short UNUSED_KauAddress { get; set; }
		[DataMember]
		public byte Source { get; set; } // Controller = 0, Device = 1, Object = 2
		[DataMember]
		public byte NameCode { get; set; }

		[DataMember]
		public byte YesNoCode { get; set; }
		[DataMember]
		public byte DescriptionCode { get; set; }

		[DataMember]
		public short ObjectNo { get; set; }
		[DataMember]
		public short ObjectDeviceType { get; set; }
		[DataMember]
		public short ObjectDeviceAddress { get; set; }
		[DataMember]
		public int ObjectFactoryNo { get; set; }
		[DataMember]
		public int ObjectState { get; set; }

		public List<byte> ToBytes()
		{
			var result = new List<byte>();
			result.AddRange(BitConverter.GetBytes(GkNo));
			result.AddRange(BitConverter.GetBytes(GkObjectNo));
			for (int i = 0; i < 24; i++)
			{
				result.Add(0);
			}
			result.AddRange(BitConverter.GetBytes(UNUSED_KauNo));

			result.Add((byte)DateTime.Day);
			result.Add((byte)DateTime.Month);
			result.Add((byte)(DateTime.Year - 2000));
			result.Add((byte)DateTime.Hour);
			result.Add((byte)DateTime.Minute);
			result.Add((byte)DateTime.Second);

			result.AddRange(BitConverter.GetBytes(UNUSED_KauAddress));
			result.Add(Source);
			result.Add(NameCode);

			result.Add(YesNoCode);
			result.Add(DescriptionCode);

			result.AddRange(BitConverter.GetBytes((short)0));
			result.AddRange(BitConverter.GetBytes(ObjectNo));
			result.AddRange(BitConverter.GetBytes(ObjectDeviceType));
			result.AddRange(BitConverter.GetBytes(ObjectDeviceAddress));
			result.AddRange(BitConverter.GetBytes(ObjectFactoryNo));
			result.AddRange(BitConverter.GetBytes(ObjectState));

			return result;
		}
	}
}