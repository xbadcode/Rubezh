﻿using GKModule.Plans.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GKModule.Plans.InstrumentAdorners
{
	public class DelayPolygonAdorner : BasePolygonAdorner
	{
		public DelayPolygonAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Shape CreateRubberband()
		{
			return new Polygon();
		}
		protected override PointCollection Points
		{
			get { return ((Polygon)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement(RubezhAPI.PointCollection points)
		{
			var element = new ElementPolygonGKDelay { Points = points };
			var propertiesViewModel = new DelayPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}