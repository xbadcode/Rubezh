﻿using StrazhAPI.Plans.Elements;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementEllipse : ElementBaseRectangle, IPrimitive
	{
		public ElementEllipse()
		{
            PresentationName = Resources.Language.Models.Plans.ElementEllipse.PresentationName;
		}

		public override ElementBase Clone()
		{
			var elementBase = new ElementEllipse();
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Primitive.Ellipse; }
		}

		#endregion IPrimitive Members
	}
}