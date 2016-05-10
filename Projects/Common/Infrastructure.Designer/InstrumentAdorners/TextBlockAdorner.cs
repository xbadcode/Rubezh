﻿using Infrastructure.Common.Windows;
using Infrastructure.Designer.ElementProperties.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class TextBlockAdorner : RectangleAdorner
	{
		public TextBlockAdorner(BaseDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementTextBlock() { Left = left, Top = top };
			var propertiesViewModel = new TextBlockPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}