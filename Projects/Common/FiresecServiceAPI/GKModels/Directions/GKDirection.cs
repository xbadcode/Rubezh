﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Направление ГК
	/// </summary>
	[DataContract]
	public class GKDirection : GKBase, IPlanPresentable
	{
		public GKDirection()
		{
			DelayRegime = DelayRegime.On;
			Logic = new GKLogic();

			InputDevices = new List<GKDevice>();
			InputZones = new List<GKZone>();
			OutputDevices = new List<GKDevice>();
			PlanElementUIDs = new List<Guid>();
            DescriptorName = this.GetGKDescriptorName(FiresecClient.GKManager.DeviceConfiguration.GKNameGenerationType);
		}

		public void OnRemoved()
		{
			var newOutputObjects = new List<GKBase>(OutputObjects);
			foreach (var outputObject in newOutputObjects)
			{
				outputObject.Update(this);
			}
		}

		public override void Update(GKDevice device)
		{
			Logic.GetAllClauses().FindAll(x => x.Devices.Contains(device)).ForEach(y => { y.Devices.Remove(device); y.DeviceUIDs.Remove(device.UID); });
			UnLinkObject(device);
			OnChanged();
		}

		public override void Update(GKDirection direction)
		{
			Logic.GetAllClauses().FindAll(x => x.Directions.Contains(direction)).ForEach(y => { y.Directions.Remove(direction); y.DirectionUIDs.Remove(direction.UID); });
			UnLinkObject(direction);
			OnChanged();
		}

        public string DescriptorName { get; set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Direction; } }

		[XmlIgnore]
		public List<GKDevice> InputDevices { get; set; }
		[XmlIgnore]
		public List<GKZone> InputZones { get; set; }
		[XmlIgnore]
		public List<GKDevice> OutputDevices { get; set; }

		/// <summary>
		/// Задержка на включение
		/// </summary>
		[DataMember]
		public ushort Delay { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public ushort Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		/// <summary>
		/// Логика включения
		/// </summary>
		[DataMember]
		public GKLogic Logic { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/BDirection.png"; }
		}
	}
}