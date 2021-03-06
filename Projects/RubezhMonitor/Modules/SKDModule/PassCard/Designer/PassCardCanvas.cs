﻿using Infrastructure;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Painters;
using Infrastructure.Plans.Presenter;
using RubezhAPI.Plans.Elements;
using RubezhAPI.SKD;
using System.Collections.Generic;

namespace SKDModule.PassCard.Designer
{
	class PassCardCanvas : CommonDesignerCanvas
	{
		private PassCardTemplate _passCardTemplate;

		public PassCardCanvas()
			: base(ServiceFactory.Events)
		{
			PainterCache.Initialize(ServiceFactory.ContentService.GetBitmapContent, ServiceFactory.ContentService.GetDrawing, ServiceFactory.ContentService.GetVisual);
		}

		public override void BeginChange(IEnumerable<DesignerItem> designerItems)
		{
		}
		public override void BeginChange()
		{
		}
		public override void EndChange()
		{
		}

		public override void CreateDesignerItem(ElementBase element)
		{
			CreatePresenterItem(element);
		}
		public PresenterItem CreatePresenterItem(ElementBase elementBase)
		{
			var presenterItem = new PresenterItem(elementBase);
			Add(presenterItem);
			presenterItem.CreatePainter();
			return presenterItem;
		}

		public IEnumerable<PresenterItem> PresenterItems
		{
			get { return InternalItems<PresenterItem>(); }
		}

		public override void Update()
		{
			CanvasWidth = _passCardTemplate.Width;
			CanvasHeight = _passCardTemplate.Height;
			CanvasBackground = PainterCache.GetBrush(_passCardTemplate);
			CanvasBorder = PainterCache.GetPen(_passCardTemplate);
		}

		public void Initialize(PassCardTemplate passCardTemplate)
		{
			_passCardTemplate = passCardTemplate;
			Initialize();
		}
	}
}