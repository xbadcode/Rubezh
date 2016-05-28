﻿using StrazhAPI.Plans.Elements;
using StrazhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementSubPlan : ElementBaseRectangle, IPrimitive, IElementReference
	{
		public ElementSubPlan()
		{
			PresentationName = "Ссылка на план";
		}

		[DataMember]
		public Guid PlanUID { get; set; }

		[DataMember]
		public string Caption { get; set; }

		public override ElementBase Clone()
		{
			ElementSubPlan elementBase = new ElementSubPlan();
			Copy(elementBase);
			return elementBase;
		}

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementSubPlan)element).PlanUID = PlanUID;
			((ElementSubPlan)element).Caption = Caption;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return StrazhAPI.Plans.Elements.Primitive.SubPlan; }
		}

		#endregion IPrimitive Members

		#region IElementReference Members

		public Guid ItemUID
		{
			get { return PlanUID; }
			set { PlanUID = value; }
		}

		#endregion IElementReference Members
	}
}