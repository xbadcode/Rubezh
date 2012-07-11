﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Designer;
using PlansModule.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using PlansModule.Designer.Designer;
using Infrustructure.Plans.Events;

namespace PlansModule.ViewModels
{
	public class ElementsViewModel : BaseViewModel
	{
		public ElementsViewModel(DesignerCanvas designerCanvas)
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementAdded);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementAdded);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
			DesignerCanvas = designerCanvas;

			Elements = new ObservableCollection<ElementBaseViewModel>();
			Update();
		}

		DesignerCanvas DesignerCanvas;
		public ObservableCollection<ElementBaseViewModel> Elements { get; set; }
		ElementBaseViewModel ElementDevices;
		ElementBaseViewModel ElementZones;
		ElementBaseViewModel ElementSubPlans;
		ElementBaseViewModel ElementElements;

		ElementBaseViewModel _selectedElement;
		public ElementBaseViewModel SelectedElement
		{
			get { return _selectedElement; }
			set
			{
				_selectedElement = value;
				OnPropertyChanged("SelectedElement");
			}
		}

		void OnPlansChanged(Guid planUID)
		{
			Update();
		}

		public void Update()
		{
			AllElements = new List<ElementViewModel>();
			Elements.Clear();
			Elements.Add(ElementDevices = new ElementGroupViewModel(Elements, this, "Устройства", "Device"));
			Elements.Add(ElementZones = new ElementGroupViewModel(Elements, this, "Зоны", "Zone"));
			Elements.Add(ElementSubPlans = new ElementGroupViewModel(Elements, this, "Подпланы", "SubPlan"));
			Elements.Add(ElementElements = new ElementGroupViewModel(Elements, this, "Элементы", "Element"));

			foreach (var designerItem in DesignerCanvas.Items)
			{
				AddDesignerItem(designerItem);
			}

			CollapseChild(ElementDevices);
			ExpandChild(ElementDevices);
			CollapseChild(ElementZones);
			ExpandChild(ElementZones);
			CollapseChild(ElementSubPlans);
			ExpandChild(ElementSubPlans);
			CollapseChild(ElementElements);
			ExpandChild(ElementElements);
		}

		#region ElementSelection

		public List<ElementViewModel> AllElements;

		public void Select(Guid elementUID)
		{
			var elementViewModel = AllElements.FirstOrDefault(x => x.ElementUID == elementUID);
			if (elementViewModel != null)
				elementViewModel.ExpantToThis();
			SelectedElement = elementViewModel;
		}

		#endregion

		void UpdateHasChildren()
		{
			ElementDevices.OnPropertyChanged("HasChildren");
			ElementZones.OnPropertyChanged("HasChildren");
			ElementSubPlans.OnPropertyChanged("HasChildren");
			ElementElements.OnPropertyChanged("HasChildren");
		}

		void AddElement(ElementBaseViewModel parentElementViewModel, ElementViewModel elementViewModel)
		{
			elementViewModel.ElementType = (parentElementViewModel as ElementGroupViewModel).ElementType;

			elementViewModel.Parent = parentElementViewModel;
			parentElementViewModel.Children.Add(elementViewModel);
			var indexOf = Elements.IndexOf(parentElementViewModel);
			Elements.Insert(indexOf + 1, elementViewModel);
			AllElements.Add(elementViewModel);
		}

		void AddDesignerItem(DesignerItem designerItem)
		{
			string name = "";
			var elementBase = designerItem.Element;

			if (elementBase is ElementDevice)
			{
				ElementDevice elementDevice = elementBase as ElementDevice;
				Device device = Helper.GetDevice(elementDevice);
				name = device.DottedAddress + " " + device.Driver.ShortName;
				AddElement(ElementDevices, new ElementViewModel(Elements, designerItem, name));
			}
			if (elementBase is IElementZone)
			{
				IElementZone elementZone = elementBase as IElementZone;
				Zone zone = Helper.GetZone(elementZone);
				name = zone == null ? "Несвязанная зона" : zone.PresentationName;
				AddElement(ElementZones, new ElementViewModel(Elements, designerItem, name));
			}
			if (elementBase is ElementSubPlan)
			{
				ElementSubPlan elementSubPlan = elementBase as ElementSubPlan;
				Plan plan = Helper.GetPlan(elementSubPlan);
				name = plan == null ? "Несвязанный подплан" : plan.Caption;
				AddElement(ElementSubPlans, new ElementViewModel(Elements, designerItem, name));
			}
			if (elementBase is ElementEllipse)
			{
				AddElement(ElementElements, new ElementViewModel(Elements, designerItem, "Эллипс"));
			}
			if (elementBase is ElementPolygon)
			{
				AddElement(ElementElements, new ElementViewModel(Elements, designerItem, "Многоугольник"));
			}
			if (elementBase is ElementPolyline)
			{
				AddElement(ElementElements, new ElementViewModel(Elements, designerItem, "Линия"));
			}
			if (elementBase is ElementRectangle)
			{
				AddElement(ElementElements, new ElementViewModel(Elements, designerItem, "Прямоугольник"));
			}
			if (elementBase is ElementTextBlock)
			{
				AddElement(ElementElements, new ElementViewModel(Elements, designerItem, "Надпись"));
			}
		}

		void OnElementAdded(List<ElementBase> elements)
		{
			foreach (var elementBase in elements)
			{
				var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.Element.UID == elementBase.UID);
				if (designerItem != null)
				{
					AddDesignerItem(designerItem);
				}
			}
			UpdateHasChildren();
		}

		void OnElementRemoved(List<ElementBase> elements)
		{
			foreach (var elementBase in elements)
			{
				var element = AllElements.FirstOrDefault(x => x.ElementUID == elementBase.UID);
				if (element != null && element.Parent != null)
				{
					if ((element.Parent != null) && (element.Parent.Children != null))
					{
						element.Parent.Children.Remove(element);
					}
					element.Parent = null;
					Elements.Remove(element);
				}
			}
			UpdateHasChildren();
		}

		void OnElementChanged(List<ElementBase> elements)
		{
		}

		void OnElementSelected(ElementBase element)
		{
			Select(element.UID);
		}

		void CollapseChild(ElementBaseViewModel parentElementViewModel)
		{
			parentElementViewModel.IsExpanded = false;

			foreach (var elementViewModel in parentElementViewModel.Children)
			{
				CollapseChild(elementViewModel);
			}
		}

		void ExpandChild(ElementBaseViewModel parentElementViewModel)
		{
			parentElementViewModel.IsExpanded = true;
			foreach (var elementViewModel in parentElementViewModel.Children)
			{
				ExpandChild(elementViewModel);
			}
		}
	}
}