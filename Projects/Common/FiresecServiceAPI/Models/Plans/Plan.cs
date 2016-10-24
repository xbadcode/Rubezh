﻿using Common;
using StrazhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	[KnownType(typeof(Plan))]
	[KnownType(typeof(PlanFolder))]
	[KnownType(typeof(ElementRectangleTank))]
	[KnownType(typeof(ElementCamera))]
	[KnownType(typeof(ElementProcedure))]
	[XmlInclude(typeof(PlanFolder)), XmlInclude(typeof(PlanFolder)), XmlInclude(typeof(ElementRectangleTank)), XmlInclude(typeof(ElementCamera)), XmlInclude(typeof(ElementProcedure))]
	public class Plan : IElementBackground, IElementRectangle
	{
		public Plan()
		{
			UID = Guid.NewGuid();
			Children = new List<Plan>();
			ElementSubPlans = new List<ElementSubPlan>();
			Caption = "Новый план";
			Width = 297;
			Height = 210;
			BackgroundColor = Colors.White;
			ImageType = ResourceType.Image;
			ClearElements();
		}

		public void ClearElements()
		{
			ElementSubPlans = new List<ElementSubPlan>();
			ElementRectangles = new List<ElementRectangle>();
			ElementEllipses = new List<ElementEllipse>();
			ElementTextBlocks = new List<ElementTextBlock>();
			ElementPolygons = new List<ElementPolygon>();
			ElementPolylines = new List<ElementPolyline>();
			ElementSKDDevices = new List<ElementSKDDevice>();
			ElementRectangleSKDZones = new List<ElementRectangleSKDZone>();
			ElementPolygonSKDZones = new List<ElementPolygonSKDZone>();
			ElementDoors = new List<ElementDoor>();
			ElementRectangleOPCZones = new List<ElementRectangleOPCZone>();
			ElementPolygonOPCZones = new List<ElementPolygonOPCZone>();

			ElementExtensions = new List<ElementBase>();
		}

		[XmlIgnore]
		public Plan Parent { get; set; }

		[DataMember]
		public string Caption { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<Plan> Children { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public double Width { get; set; }

		[DataMember]
		public double Height { get; set; }

		[DataMember]
		public Color BackgroundColor { get; set; }

		[DataMember]
		public Guid? BackgroundImageSource { get; set; }

		[DataMember]
		public string BackgroundSourceName { get; set; }

		[DataMember]
		public ResourceType ImageType { get; set; }

		[DataMember]
		public List<ElementRectangle> ElementRectangles { get; set; }

		[DataMember]
		public List<ElementEllipse> ElementEllipses { get; set; }

		[DataMember]
		public List<ElementTextBlock> ElementTextBlocks { get; set; }

		[DataMember]
		public List<ElementPolygon> ElementPolygons { get; set; }

		[DataMember]
		public List<ElementPolyline> ElementPolylines { get; set; }

		[DataMember]
		public List<ElementSubPlan> ElementSubPlans { get; set; }

		[DataMember]
		public List<ElementSKDDevice> ElementSKDDevices { get; set; }

		[DataMember]
		public List<ElementRectangleSKDZone> ElementRectangleSKDZones { get; set; }

		[DataMember]
		public List<ElementRectangleOPCZone> ElementRectangleOPCZones { get; set; }

		[DataMember]
		public List<ElementPolygonSKDZone> ElementPolygonSKDZones { get; set; }

		[DataMember]
		public List<ElementPolygonOPCZone> ElementPolygonOPCZones { get; set; }

		[DataMember]
		public List<ElementDoor> ElementDoors { get; set; }

		[DataMember]
		public List<ElementBase> ElementExtensions { get; set; }


		/// <summary>
		/// Свойство содержит коллекцию всех элементов плана.
		/// Служит для корректной работы функционала, который позволяет отображать на плане фигуру, соответствующую выбранному элементу (например, выбранную процедуру),
		/// даже если на данный момент выбран тот план, где фигуры, соответствующей выбранному элементу, не существует (но существует на других планах).
		/// </summary>
		[XmlIgnore]
		public List<ElementBase> ElementUnion
		{
			get
			{
				var union = new List<ElementBase>();
				union.AddRange(ElementDoors);
				union.AddRange(ElementExtensions);
				union.AddRange(ElementPolygonSKDZones);
				union.AddRange(ElementRectangleSKDZones);
				union.AddRange(ElementSKDDevices);
				union.AddRange(ElementSubPlans);
				union.AddRange(ElementPolygonOPCZones);
				union.AddRange(ElementRectangleOPCZones);
				return union;
			}
		}

		[XmlIgnore]
		public IEnumerable<ElementBase> SimpleElements
		{
			get
			{
				foreach (var element in ElementRectangles)
					yield return element;
				foreach (var element in ElementEllipses)
					yield return element;
				foreach (var element in ElementPolylines)
					yield return element;
				foreach (var element in ElementPolygons)
					yield return element;
				foreach (var element in ElementTextBlocks)
					yield return element;
			}
		}

		[XmlIgnore]
		public IEnumerable<ElementBase> AllElements
		{
			get
			{
				foreach (var element in ElementUnion)
					yield return element;
				foreach (var element in SimpleElements)
					yield return element;
			}
		}

		[XmlIgnore]
		public bool AllowTransparent
		{
			get { return true; }
		}
	}
}