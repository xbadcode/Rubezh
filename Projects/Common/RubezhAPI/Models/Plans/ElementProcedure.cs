﻿using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementProcedure : ElementBaseRectangle, IElementTextBlock, IPrimitive, IElementReference
	{
		public ElementProcedure()
		{
			ProcedureUID = Guid.Empty;
			Stretch = false;
			TextAlignment = 0;
			VerticalAlignment = 0;
			WordWrap = false;
			BorderThickness = 0;
			BackgroundColor = Colors.Transparent;

			Text = "Процедура";
			PresentationName = "Процедура";
			ForegroundColor = Colors.Black;
			FontSize = 10;
			TextAlignment = 0;
			FontFamilyName = "Arial";
			FontItalic = false;
			FontBold = false;
			Height = 22;
			Width = 52;
		}

		[DataMember]
		public Guid ProcedureUID { get; set; }

		[DataMember]
		public string Text { get; set; }
		[DataMember]
		public Color ForegroundColor { get; set; }
		[DataMember]
		public double FontSize { get; set; }
		[DataMember]
		public bool FontItalic { get; set; }
		[DataMember]
		public bool FontBold { get; set; }
		[DataMember]
		public string FontFamilyName { get; set; }
		[DataMember]
		public bool Stretch { get; set; }
		[DataMember]
		public int TextAlignment { get; set; }
		[DataMember]
		public int VerticalAlignment { get; set; }
		[DataMember]
		public bool WordWrap { get; set; }

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		public Guid ItemUID
		{
			get { return ProcedureUID; }
			set { ProcedureUID = value; }
		}

		#endregion

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Primitive.Procedure; }
		}

		#endregion
	}
}