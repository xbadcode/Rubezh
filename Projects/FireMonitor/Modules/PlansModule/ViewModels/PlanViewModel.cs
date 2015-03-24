﻿using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.TreeList;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using Common;

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeNodeViewModel<PlanViewModel>
	{
		PlansViewModel _plansViewModel;
		public Plan Plan { get; private set; }
		public PlanFolder PlanFolder { get; private set; }

		public PlanViewModel(PlansViewModel plansViewModel, Plan plan)
		{
			_selfNamedStateClass = new NamedStateClass();
			_namedStateClass = new NamedStateClass();
			_plansViewModel = plansViewModel;
			Plan = plan;
			PlanFolder = plan as PlanFolder;
		}

		NamedStateClass _namedStateClass;
		public NamedStateClass NamedStateClass
		{
			get { return _namedStateClass; }
			set
			{
				_namedStateClass = value;
				ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Publish(Plan.UID);
				OnPropertyChanged(() => NamedStateClass);
			}
		}

		NamedStateClass _selfNamedStateClass;
		public NamedStateClass SelfNamedStateClass
		{
			get { return _selfNamedStateClass; }
			set
			{
				_selfNamedStateClass = value;
				OnPropertyChanged(() => SelfNamedStateClass);
				UpdateState();
			}
		}

		void UpdateState()
		{
			NamedStateClass = SelfNamedStateClass;
			foreach (var child in Children)
			{
				if (child.NamedStateClass.StateClass < NamedStateClass.StateClass)
					NamedStateClass = child.NamedStateClass;
			}
			if (Parent != null)
				Parent.UpdateState();
		}

		public void RegisterPresenter(IPlanPresenter<Plan, XStateClass> planPresenter)
		{
			planPresenter.SubscribeStateChanged(Plan, StateChanged);
			StateChanged();
		}
		void StateChanged()
		{
			var minNamedStateClass = new NamedStateClass();
			foreach (var planPresenter in _plansViewModel.PlanPresenters)
			{
				var namedStateClass = (NamedStateClass)planPresenter.GetNamedStateClass(Plan);
				if (namedStateClass.StateClass < minNamedStateClass.StateClass)
				{
					minNamedStateClass = namedStateClass;
				}
			}
			if (minNamedStateClass.StateClass == XStateClass.No || minNamedStateClass.StateClass == XStateClass.Off)
			{
				minNamedStateClass.StateClass = XStateClass.Norm;
				minNamedStateClass.Name = "Норма";
			}
			SelfNamedStateClass = minNamedStateClass;
		}

		public bool IsFolder
		{
			get { return PlanFolder != null; }
		}
	}
}