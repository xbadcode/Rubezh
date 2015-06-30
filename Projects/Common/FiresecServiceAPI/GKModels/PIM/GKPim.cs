﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Программно имитируемый модуль ГК
	/// </summary>
	[DataContract]
	public class GKPim : GKBase
	{
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Pim; } }
		public override void Update(GKDevice device)
		{

		}

		public override void Update(GKDirection direction)
		{

		}
		/// <summary>
		/// Признак автогенерации
		/// </summary>
		[DataMember]
		public bool IsAutoGenerated { get; set; }

		/// <summary>
		/// Идентификатор МПТ
		/// </summary>
		[DataMember]
		public Guid MPTUID { get; set; }

		/// <summary>
		/// Идентификатор НС
		/// </summary>
		[DataMember]
		public Guid PumpStationUID { get; set; }

		/// <summary>
		/// Идентификатор Охранной зоны
		/// </summary>
		[DataMember]
		public Guid GuardZoneUID { get; set; }

		/// <summary>
		/// Идентификатор Точки Доступа
		/// </summary>
		[DataMember]
		public Guid DoorUID { get; set; }

		public override string PresentationName
		{
			get { return Name; }
		}

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Pim.png"; }
		}
	}
}