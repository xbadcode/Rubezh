﻿using Infrustructure.Plans.Elements;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementPolygon : ElementBasePolygon, IPrimitive
	{
		public ElementPolygon()
		{
			PresentationName = "Многоугольник";
		}

		public override ElementBase Clone()
		{
			ElementPolygon elementBase = new ElementPolygon();
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Polygon; }
		}

		#endregion IPrimitive Members
	}
}