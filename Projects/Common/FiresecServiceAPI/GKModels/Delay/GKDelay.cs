﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Задержка ГК
	/// </summary>
	[DataContract]
	public class GKDelay : GKBase
	{
		public GKDelay()
		{
			Logic = new GKLogic();
			DelayRegime = DelayRegime.On;
            DescriptorName = this.GetGKDescriptorName(FiresecClient.GKManager.DeviceConfiguration.GKNameGenerationType);
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

		/// <summary>
		/// Задержка на включение
		/// </summary>
		[DataMember]
		public ushort DelayTime { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public ushort Hold { get; set; }

		/// <summary>
		/// Режим после окончания удержания
		/// </summary>
		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		/// <summary>
		/// Логика включения
		/// </summary>
		[DataMember]
		public GKLogic Logic { get; set; }

		/// <summary>
		/// Признак автогенерации
		/// </summary>
		[DataMember]
		public bool IsAutoGenerated { get; set; }

		/// <summary>
		/// Идентификатор НС
		/// </summary>
		[DataMember]
		public Guid PumpStationUID { get; set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Delay; } }

		[XmlIgnore]
		public override string PresentationName
		{
			get { return Name; }
		}

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Delay.png"; }
		}
        public string DescriptorName { get; set; }
	}
}