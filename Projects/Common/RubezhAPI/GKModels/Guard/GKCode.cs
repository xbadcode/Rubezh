﻿using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Код ГК
	/// </summary>
	[DataContract]
	public class GKCode : GKBase
	{
		public GKCode()
		{
			Name = "Новый код";
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Code; } }

		/// <summary>
		/// Пароль
		/// </summary>
		[DataMember]
		public int Password { get; set; }

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Code.png"; }
		}
	}
}