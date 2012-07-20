﻿using Infrastructure;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using PlansModule.Designer.DesignerItems;

namespace PlansModule.Designer
{
	public static class DesignerItemFactory
	{
		public static DesignerItem Create(ElementBase element)
		{
			var args = new DesignerItemFactoryEventArgs(element);
			ServiceFactory.Events.GetEvent<DesignerItemFactoryEvent>().Publish(args);
			if (args.DesignerItem != null)
				return args.DesignerItem;
			switch (element.Type)
			{
				case ElementType.Point:
					return new DesignerItemPoint(element);
				case ElementType.Rectangle:
					return new DesignerItemRectangle(element);
				case ElementType.Polygon:
				case ElementType.Polyline:
					return new DesignerItemShape(element);
			}
			return new DesignerItemBase(element);
		}
	}
}
