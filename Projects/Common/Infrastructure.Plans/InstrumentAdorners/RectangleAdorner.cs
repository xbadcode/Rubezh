﻿using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace Infrastructure.Plans.InstrumentAdorners
{
	public class RectangleAdorner : BaseRectangleAdorner
	{
		public RectangleAdorner(BaseDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			return new ElementRectangle() { Left = left, Top = top };
		}
	}
}