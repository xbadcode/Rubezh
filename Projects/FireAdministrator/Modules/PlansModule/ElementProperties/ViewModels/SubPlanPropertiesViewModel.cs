﻿using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans;
using PlansModule.Designer;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using RubezhClient;
using System.Collections.ObjectModel;

namespace PlansModule.ViewModels
{
	public class SubPlanPropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		ElementBase ElementBase { get; set; }
		public bool CanEditPosition { get; private set; }
		private IElementSubPlan _elementSubPlan;

		public SubPlanPropertiesViewModel(IElementSubPlan element)
		{
			Title = "Свойства фигуры: Ссылка на план";
			_elementSubPlan = element;
			ElementBase = element as ElementBase;
			CanEditPosition = ElementBase != null;
			if (CanEditPosition)
			{
				var position = ElementBase.GetPosition();
				Left = (int)(position.X * _sensivityFactor);
				Top = (int)(position.Y * _sensivityFactor);
			}
			Initialize();
		}
		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}
		private void Initialize()
		{
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in ClientManager.PlansConfiguration.Plans)
				AddPlan(plan, null);

			for (int i = 0; i < Plans.Count; i++)
			{
				Plans[i].CollapseChildren();
				Plans[i].ExpandChildren();
			}
		}
		private void AddPlan(Plan planFolder, PlanViewModel parentPlanViewModel)
		{
			var planViewModel = new PlanViewModel(planFolder);
			if (parentPlanViewModel == null)
				Plans.Add(planViewModel);
			else
				parentPlanViewModel.AddChild(planViewModel);
			var plan = planFolder as Plan;
			if (plan != null && plan.UID == _elementSubPlan.PlanUID)
				SelectedPlan = planViewModel;

			foreach (var childPlan in planFolder.Children)
				AddPlan(childPlan, planViewModel);
		}

		public ObservableCollection<PlanViewModel> Plans { get; set; }

		PlanViewModel _selectedPlan;
		public PlanViewModel SelectedPlan
		{
			get { return _selectedPlan; }
			set
			{
				_selectedPlan = value;
				OnPropertyChanged(() => SelectedPlan);
			}
		}

		protected override bool CanSave()
		{
			return base.CanSave() && (SelectedPlan == null || SelectedPlan.PlanFolder == null);
		}
		protected override bool Save()
		{
			if (CanEditPosition)
				ElementBase.SetPosition(new System.Windows.Point((double)Left / _sensivityFactor, (double)Top / _sensivityFactor));
			Helper.SetSubPlan(_elementSubPlan, SelectedPlan == null ? null : SelectedPlan.Plan);
			return base.Save();
		}
	}
}