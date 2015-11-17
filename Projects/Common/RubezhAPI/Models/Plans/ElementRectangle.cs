﻿using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementRectangle : ElementBaseRectangle, IPrimitive
	{
		public ElementRectangle()
		{
			PresentationName = "Прямоугольник";
			ShowTooltip = true;
		}

		[DataMember]
		public bool ShowTooltip { get; set; }

		#region IPrimitive Members

		[XmlIgnore]
		public virtual Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Rectangle; }
		}

		#endregion
	}
}