﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using PlansModule.Events;
using PlansModule.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;
using PlansModule.Designer.Designer;
using Infrustructure.Plans.Events;

namespace PlansModule.Designer
{
	public class DesignerCanvas : CommonDesignerCanvas
	{
		public Plan Plan { get; set; }
		public PlanDesignerViewModel PlanDesignerViewModel { get; set; }
		private Point? dragStartPoint = null;
		public bool IsPointAdding = false;
		List<ElementBase> initialElements;

		public DesignerCanvas()
			: base(ServiceFactory.Events)
		{
			AllowDrop = true;
			Background = new SolidColorBrush(Colors.DarkGray);
			Width = 100;
			Height = 100;

			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			PreviewMouseDown += new MouseButtonEventHandler(On_PreviewMouseDown);
			DataContext = this;
		}

		public override double Zoom
		{
			get { return PlanDesignerViewModel.Zoom; }
		}
		public override double PointZoom
		{
			get { return PlanDesignerViewModel.DeviceZoom; }
		}

		public void RemoveAllSelected()
		{
			if (SelectedElements.Count == 0)
				return;

			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Publish(new List<ElementBase>(SelectedElements));
			for (int i = Items.Count() - 1; i >= 0; i--)
			{
				var designerItem = Children[i] as DesignerItem;
				if (designerItem.IsSelected)
					Children.Remove(designerItem);
			}
			ServiceFactory.SaveService.PlansChanged = true;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Source == this)
			{
				this.dragStartPoint = new Point?(e.GetPosition(this));
				this.DeselectAll();
				e.Handled = true;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (e.LeftButton != MouseButtonState.Pressed)
			{
				this.dragStartPoint = null;
			}

			if (this.dragStartPoint.HasValue)
			{
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
				if (adornerLayer != null)
				{
					var rubberbandAdorner = new RubberbandAdorner(this, this.dragStartPoint);
					if (rubberbandAdorner != null)
					{
						adornerLayer.Add(rubberbandAdorner);
					}
				}

				e.Handled = true;
			}
		}

		protected override void OnDrop(DragEventArgs e)
		{
			base.OnDrop(e);
			var elementBase = e.Data.GetData("DESIGNER_ITEM") as ElementBase;
			elementBase.SetDefault();

			Point position = e.GetPosition(this);
			elementBase.Position = position;

			if (elementBase is IElementZone)
			{
				var zonePropertiesViewModel = new ZonePropertiesViewModel(elementBase as IElementZone);
				if (DialogService.ShowModalWindow(zonePropertiesViewModel) == false)
				{
					e.Handled = true;
					return;
				}
			}

			var designerItem = AddElement(elementBase);
			if (designerItem != null)
			{
				this.DeselectAll();
				designerItem.IsSelected = true;
				PlanDesignerViewModel.MoveToFrontCommand.Execute();

				ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(new List<ElementBase>() { elementBase });
				ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(new List<ElementBase>() { elementBase });
			}

			e.Handled = true;
		}
		public DesignerItem AddElement(ElementBase elementBase)
		{
			if (elementBase is ElementRectangle)
				Plan.ElementRectangles.Add(elementBase as ElementRectangle);
			else if (elementBase is ElementEllipse)
				Plan.ElementEllipses.Add(elementBase as ElementEllipse);
			else if (elementBase is ElementPolygon)
				Plan.ElementPolygons.Add(elementBase as ElementPolygon);
			else if (elementBase is ElementPolyline)
				Plan.ElementPolylines.Add(elementBase as ElementPolyline);
			else if (elementBase is ElementTextBlock)
				Plan.ElementTextBlocks.Add(elementBase as ElementTextBlock);
			else if (elementBase is ElementRectangleZone)
				Plan.ElementRectangleZones.Add(elementBase as ElementRectangleZone);
			else if (elementBase is ElementPolygonZone)
				Plan.ElementPolygonZones.Add(elementBase as ElementPolygonZone);
			else if (elementBase is ElementSubPlan)
				Plan.ElementSubPlans.Add(elementBase as ElementSubPlan);
			else if (elementBase is ElementDevice)
			{
				var elementDevice = elementBase as ElementDevice;
				Helper.SetDevice(elementDevice);
				Plan.ElementDevices.Add(elementDevice);
			}

			return Create(elementBase);
		}

		public DesignerItem Create(ElementBase elementBase)
		{
			var designerItem = DesignerItemFactory.Create(elementBase);
			Children.Add(designerItem);
			designerItem.Redraw();
			SetZIndex(designerItem);
			return designerItem;
		}
		private void SetZIndex(DesignerItem designerItem)
		{
			int bigConstatnt = 100000;

			if (designerItem.Element is IElementZIndex)
				Panel.SetZIndex(designerItem, (designerItem.Element as IElementZIndex).ZIndex);

			if (designerItem.Element is ElementSubPlan)
				Panel.SetZIndex(designerItem, 1 * bigConstatnt);

			if (designerItem.Element is IElementZone)
			{
				Panel.SetZIndex(designerItem, 2 * bigConstatnt);
				IElementZone elementZone = designerItem.Element as IElementZone;
				Zone zone = Helper.GetZone(elementZone);
				if (zone != null)
				{
					if (zone.ZoneType == ZoneType.Fire)
						Panel.SetZIndex(designerItem, 3 * bigConstatnt);

					if (zone.ZoneType == ZoneType.Guard)
						Panel.SetZIndex(designerItem, 4 * bigConstatnt);
				}
			}

			if (designerItem.Element is ElementDevice)
				Panel.SetZIndex(designerItem, 5 * bigConstatnt);
		}

		void On_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (IsPointAdding)
			{
				//if (SelectedItems == null)
				//    return;
				//var selectedItem = SelectedItems.FirstOrDefault();
				//if (selectedItem == null)
				//    return;
				//if ((selectedItem.IsPolygon == false) && (selectedItem.IsPolyline == false))
				//    return;

				//IsPointAdding = false;

				//PointCollection pointCollection = null;
				//if (selectedItem.Content is Polygon)
				//{
				//    var polygon = selectedItem.Content as Polygon;
				//    pointCollection = polygon.Points;
				//}
				//if (selectedItem.Content is Polyline)
				//{
				//    var polyline = selectedItem.Content as Polyline;
				//    pointCollection = polyline.Points;
				//}

				//Point currentPoint = e.GetPosition(selectedItem);
				//var minDistance = double.MaxValue;
				//int minIndex = 0;
				//for (int i = 0; i < pointCollection.Count; i++)
				//{
				//    var polygonPoint = pointCollection[i];
				//    var distance = Math.Pow(currentPoint.X - polygonPoint.X, 2) + Math.Pow(currentPoint.Y - polygonPoint.Y, 2);
				//    if (distance < minDistance)
				//    {
				//        minIndex = i;
				//        minDistance = distance;
				//    }
				//}
				//minIndex = minIndex + 1;
				//Point point = e.GetPosition(selectedItem);
				//pointCollection.Insert(minIndex, point);

				//var designerItem = Items.FirstOrDefault(x => x.IsPointAdding);
				//designerItem.UpdatePolygonAdorner();

				//e.Handled = true;
			}
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var designerPropertiesViewModel = new DesignerPropertiesViewModel(Plan);
			if (DialogService.ShowModalWindow(designerPropertiesViewModel))
			{
				Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}

		public override void Update()
		{
			Width = Plan.Width;
			Height = Plan.Height;
			Background = new SolidColorBrush(Plan.BackgroundColor);
			if (Plan.BackgroundPixels != null)
				Background = PainterHelper.CreateBrush(Plan.BackgroundPixels);
		}

		public List<ElementBase> CloneSelectedElements()
		{
			initialElements = new List<ElementBase>();
			foreach (var designerItem in SelectedItems)
			{
				designerItem.UpdateElementProperties();
				initialElements.Add(designerItem.Element.Clone());
			}
			return initialElements;
		}

		public override void BeginChange()
		{
			initialElements = CloneSelectedElements();
		}
		public override void EndChange()
		{
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(initialElements);
		}

		public void UpdateZoom()
		{
			foreach (DesignerItem designerItem in Items)
				designerItem.UpdateZoom();
		}
		public void UpdateZoomPoint()
		{
			foreach (DesignerItem designerItem in Items)
				designerItem.UpdateZoomPoint();
		}
	}
}