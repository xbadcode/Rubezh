﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract()]
	public class ElementPolygonGKDelay : ElementBasePolygon, IPrimitive, IElementDelay, IElementReference
	{
		public ElementPolygonGKDelay()
		{
			PresentationName = "Задержка";
		}

		[DataMember()]
		public Guid DelayUID { get; set; }

		[DataMember()]
		public bool ShowState { get; set; }

		[DataMember]
		public bool ShowDelay { get; set; }

		#region IPrimitive Members

		[XmlIgnore()]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.PolygonZone; }
		}

		#endregion IPrimitive Members

		public void SetZLayer(int zlayer)
		{
			ZLayer = zlayer;
		}

		#region IElementReference Members

		[XmlIgnore]
		public Guid ItemUID
		{
			get { return this.DelayUID; }
			set { this.DelayUID = value; }
		}

		#endregion
	}
}