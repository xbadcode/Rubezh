﻿using System.Security.Policy;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class GuardZoneViewModel : BaseViewModel
	{
		public GKGuardZone GuardZone { get; private set; }
		public GKState State
		{
			get { return GuardZone.State; }
		}

		public GuardZoneViewModel(GKGuardZone guardZone)
		{
			TurnOnCommand = new RelayCommand(OnTurnOn, CanControl);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow, CanControl);
			TurnOnInAutomaticCommand = new RelayCommand(OnTurnOnInAutomatic, CanControl);
			TurnOnNowInAutomaticCommand = new RelayCommand(OnTurnOnNowInAutomatic, CanControl);
			TurnOffCommand = new RelayCommand(OnTurnOff, CanControl);
			TurnOffInAutomaticCommand = new RelayCommand(OnTurnOffInAutomatic, CanControl);
			TurnOffNowInAutomaticCommand = new RelayCommand(OnTurnOffNowInAutomatic, CanControl);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);

			GuardZone = guardZone;
			State.StateChanged += OnStateChanged;
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => GuardZone);
			OnPropertyChanged(() => State.StateClasses);
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void OnShowOnPlanOrProperties()
		{
			if (CanShowOnPlan())
				ShowOnPlanHelper.ShowGuardZone(GuardZone);
			else
				DialogService.ShowWindow(new GuardZoneDetailsViewModel(GuardZone));
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		public void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowGuardZone(GuardZone);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowGuardZone(GuardZone);
		}


		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOn(GuardZone);
			}
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOnNow(GuardZone);
			}
		}

		public RelayCommand TurnOnInAutomaticCommand { get; private set; }
		void OnTurnOnInAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (!State.StateClasses.Contains(XStateClass.AutoOff))
					FiresecManager.FiresecService.GKTurnOnInAutomatic(GuardZone);
				else
					FiresecManager.FiresecService.GKTurnOn(GuardZone);
			}
		}

		public RelayCommand TurnOnNowInAutomaticCommand { get; private set; }
		void OnTurnOnNowInAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (!State.StateClasses.Contains(XStateClass.AutoOff))
					FiresecManager.FiresecService.GKTurnOnNowInAutomatic(GuardZone);
				else
					FiresecManager.FiresecService.GKTurnOnNow(GuardZone);
			}
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOff(GuardZone);
			}
		}

		public RelayCommand TurnOffInAutomaticCommand { get; private set; }
		void OnTurnOffInAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (!State.StateClasses.Contains(XStateClass.AutoOff))
					FiresecManager.FiresecService.GKTurnOffInAutomatic(GuardZone);
				else
					FiresecManager.FiresecService.GKTurnOff(GuardZone);
			}
		}

		public RelayCommand TurnOffNowInAutomaticCommand { get; private set; }
		void OnTurnOffNowInAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (!State.StateClasses.Contains(XStateClass.AutoOff))
					FiresecManager.FiresecService.GKTurnOffNowInAutomatic(GuardZone);
				else
					FiresecManager.FiresecService.GKTurnOffNow(GuardZone);
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKReset(GuardZone);
			}
		}
		bool CanReset()
		{
			return State.StateClasses.Contains(XStateClass.Fire1);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showArchiveEventArgs = new ShowArchiveEventArgs()
			{
				GKGuardZone = GuardZone
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new GuardZoneDetailsViewModel(GuardZone));
		}

		public bool CanControl()
		{
			if (State != null && !State.StateClasses.Contains(XStateClass.Ignore))
			{
				if (GuardZone.IsExtraProtected && FiresecManager.CheckPermission(PermissionType.Oper_ExtraGuardZone))
				{
					return FiresecManager.CheckPermission(PermissionType.Oper_ExtraGuardZone);
				}
				return !GuardZone.IsExtraProtected && FiresecManager.CheckPermission(PermissionType.Oper_GuardZone_Control);
			}
			return false;

		}
	}
}