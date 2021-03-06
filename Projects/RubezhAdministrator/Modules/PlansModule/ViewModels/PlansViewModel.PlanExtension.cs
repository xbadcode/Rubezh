﻿using Controls.Converters;
using Infrastructure.Common;
using Infrastructure.Common.Validation;
using Infrastructure.Plans;
using Infrastructure.Plans.Designer;
using Infrastructuret.Plans;
using PlansModule.Validation;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel
	{
		private List<IPlanExtension<Plan>> _planExtensions;

		public void RegisterExtension(IPlanExtension<Plan> planExtension)
		{
			if (!_planExtensions.Contains(planExtension))
			{
				_planExtensions.Add(planExtension);
				planExtension.ExtensionRegistered(DesignerCanvas);
				ElementsViewModel.Update();
				DesignerCanvas.Toolbox.RegisterInstruments(planExtension.Instruments);
			}
		}
		public void ElementAdded(ElementBase element)
		{
			foreach (var planExtension in _planExtensions)
				if (planExtension.ElementAdded(SelectedPlan.Plan, element))
					break;
		}
		public void ElementRemoved(ElementBase element)
		{
			foreach (var planExtension in _planExtensions)
				if (planExtension.ElementRemoved(SelectedPlan.Plan, element))
					break;
		}
		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			foreach (var planExtension in _planExtensions)
				planExtension.RegisterDesignerItem(designerItem);
		}
		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var planExtension in _planExtensions)
				foreach (var element in planExtension.LoadPlan(plan))
					yield return element;
		}
		public IEnumerable<IValidationError> Validate()
		{
			if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(ValidationErrorType.NotBoundedElements))
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				{
					foreach (var element in BasePlanExtension.FindUnbinded(plan.ElementRectangleSubPlans))
						yield return new PlanElementValidationError(CreateElementError(plan.UID, element));
					foreach (var element in BasePlanExtension.FindUnbinded(plan.ElementPolygonSubPlans))
						yield return new PlanElementValidationError(CreateElementError(plan.UID, element));
				}

			foreach (var planExtension in _planExtensions)
				foreach (var error in planExtension.Validate())
					yield return new PlanElementValidationError(error);
		}
		ElementError CreateElementError(Guid planUid, ElementBase element)
		{
			return new ElementError()
			{
				PlanUID = planUid,
				Error = "Несвязанная ссылка на план",
				Element = element,
				IsCritical = false,
				ImageSource = "/Controls;component/Images/CMap.png"
			};
		}

		private List<TabItem> _tabPages;
		public List<TabItem> TabPages
		{
			get { return _tabPages; }
			set
			{
				_tabPages = value;
				OnPropertyChanged(() => TabPages);
			}
		}
		private int _selectedTabIndex;
		public int SelectedTabIndex
		{
			get { return _selectedTabIndex; }
			set
			{
				_selectedTabIndex = value;
				OnPropertyChanged(() => SelectedTabIndex);
			}
		}

		private void CreatePages()
		{
			var layers = new TabItem()
			{
				Header = "Слои",
				Content = ElementsViewModel
			};
			Binding visibilityBinding = new Binding("SelectedPlan");
			visibilityBinding.Source = this;
			visibilityBinding.Converter = new NullToVisibilityConverter();
			layers.SetBinding(TabItem.VisibilityProperty, visibilityBinding);

			TabPages = new List<TabItem>()
			{
				layers
			};
			SelectedTabIndex = -1;
		}
		private void UpdateTabIndex()
		{
			SelectedTabIndex = SelectedPlan == null ? -1 : 0;
		}

		private void ExtensionAttached()
		{
			foreach (var planExtension in _planExtensions)
				planExtension.ExtensionAttached();
		}
	}
}