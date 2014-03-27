﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDirection : XBase, INamedBase
	{
		public XDirection()
		{
			UID = BaseUID;// Guid.NewGuid();
			DirectionZones = new List<XDirectionZone>();
			DirectionDevices = new List<XDirectionDevice>();
			DelayRegime = DelayRegime.Off;

			InputDevices = new List<XDevice>();
			InputZones = new List<XZone>();
			InputDirections = new List<XDirection>();
			InputMPTs = new List<XMPT>();
			InputDelays = new List<XDelay>();
			OutputDevices = new List<XDevice>();
			PlanElementUIDs = new List<Guid>();
		}

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Direction; } }

		public List<XDevice> InputDevices { get; set; }
		public List<XZone> InputZones { get; set; }
		public List<XDirection> InputDirections { get; set; }
		public List<XMPT> InputMPTs { get; set; }
		public List<XDelay> InputDelays { get; set; }
		public List<XDevice> OutputDevices { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public ushort No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ushort Delay { get; set; }

		[DataMember]
		public ushort Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		[DataMember]
		public List<XDirectionZone> DirectionZones { get; set; }

		[DataMember]
		public List<XDirectionDevice> DirectionDevices { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		public override string PresentationName
		{
			get { return No + "." + Name; }
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;
		public List<Guid> PlanElementUIDs { get; set; }
	}
}