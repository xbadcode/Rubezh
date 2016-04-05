﻿using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementPolyline : ElementBasePolyline, IPrimitive
	{
		public ElementPolyline()
		{
			PresentationName = "Линия";
		}

		[DataMember]
		public bool ShowTooltip { get; set; }

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Polyline; }
		}

		#endregion
	}
}