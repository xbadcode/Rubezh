﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;

namespace GKModule.ViewModels
{
	public class AlarmViewModel : BaseViewModel
	{
		public Alarm Alarm { get; private set; }

		public AlarmViewModel(Alarm alarm)
		{
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			TurnOnAutomaticCommand = new RelayCommand(OnTurnOnAutomatic, CanTurnOnAutomatic);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			Alarm = alarm;
			InitializePlans();
		}

		public string ObjectName
		{
			get
			{
				if (Alarm.Device != null)
					return Alarm.Device.PresentationName;
				if (Alarm.Door != null)
					return Alarm.Door.PresentationName;
				return null;
			}
		}

		public string ImageSource
		{
			get
			{
				if (Alarm.Device != null)
					return Alarm.Device.Driver.ImageSource;
				if (Alarm.Door != null)
					return "/Controls;component/Images/Door.png";
				return null;
			}
		}

		public XStateClass ObjectStateClass
		{
			get
			{
				if (Alarm.Device != null)
					return Alarm.Device.State.StateClass;
				if (Alarm.Door != null)
					return Alarm.Door.State.StateClass;
				return XStateClass.Norm;
			}
		}

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();

			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				ElementBase elementBase;
				if (Alarm.Device != null)
				{
					elementBase = plan.ElementGKDevices.FirstOrDefault(x => x.DeviceUID == Alarm.Device.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Device = Alarm.Device;
						Plans.Add(alarmPlanViewModel);
					}
				}
				if (Alarm.Door != null)
				{
					elementBase = plan.ElementGKDoors.FirstOrDefault(x => x.DoorUID == Alarm.Door.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Door = Alarm.Door;
						Plans.Add(alarmPlanViewModel);
					}
				}
			}
		}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else
				OnShowObject();
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			if (Alarm.Device != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(Alarm.Device.UID);
			}
			if (Alarm.Door != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKDoorEvent>().Publish(Alarm.Door.UID);
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Alarm.Device != null)
			{
				ShowOnPlanHelper.ShowDevice(Alarm.Device);
			}
			if (Alarm.Door != null)
			{
				ShowOnPlanHelper.ShowDoor(Alarm.Door);
			}
		}
		bool CanShowOnPlan()
		{
			if (Alarm.Device != null)
			{
				return ShowOnPlanHelper.CanShowDevice(Alarm.Device);
			}
			if (Alarm.Door != null)
			{
				return ShowOnPlanHelper.CanShowDoor(Alarm.Door);
			}
			return false;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (Alarm.Device != null)
				{
					FiresecManager.FiresecService.GKReset(Alarm.Device);
				}
			}
		}
		bool CanReset()
		{
			if (Alarm.Device != null)
			{
				if (Alarm.Device.DriverType == GKDriverType.RSR2_MAP4)
				{
					return Alarm.Device.State.StateClasses.Contains(XStateClass.Fire2) || Alarm.Device.State.StateClasses.Contains(XStateClass.Fire1);
				}
			}
			return false;
		}
		public bool CanResetCommand
		{
			get { return CanReset(); }
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (Alarm.Device != null)
				{
					if (Alarm.Device.State.StateClasses.Contains(XStateClass.Ignore))
					{
						FiresecManager.FiresecService.GKSetAutomaticRegime(Alarm.Device);
					}
				}

				if (Alarm.Door != null)
				{
					if (Alarm.Door.State.StateClasses.Contains(XStateClass.Ignore))
					{
						FiresecManager.FiresecService.GKSetAutomaticRegime(Alarm.Door);
					}
				}
			}
		}
		bool CanResetIgnore()
		{
			if (Alarm.AlarmType != GKAlarmType.Ignore)
				return false;

			if (!FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices))
				return false;

			if (Alarm.Device != null)
			{
				if (Alarm.Device.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			return false;
		}
		public bool CanResetIgnoreCommand
		{
			get { return CanResetIgnore(); }
		}

		public RelayCommand TurnOnAutomaticCommand { get; private set; }
		void OnTurnOnAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (Alarm.Device != null)
				{
					if (Alarm.Device.State.StateClasses.Contains(XStateClass.AutoOff))
					{
						FiresecManager.FiresecService.GKSetAutomaticRegime(Alarm.Device);
					}
				}
			}
		}
		bool CanTurnOnAutomatic()
		{
			if (Alarm.AlarmType == GKAlarmType.AutoOff)
			{
				if (Alarm.Device != null)
				{
					return Alarm.Device.Driver.IsControlDevice && Alarm.Device.State.StateClasses.Contains(XStateClass.AutoOff);
				}
			}
			return false;
		}
		public bool CanTurnOnAutomaticCommand
		{
			get { return CanTurnOnAutomatic(); }
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showArchiveEventArgs = new ShowArchiveEventArgs()
			{
				GKDevice = Alarm.Device,
				GKDoor = Alarm.Door,
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			if (Alarm.Device != null)
			{
				DialogService.ShowWindow(new DeviceDetailsViewModel(Alarm.Device));
			}
			if (Alarm.Door != null)
			{
				DialogService.ShowWindow(new DoorDetailsViewModel(Alarm.Door));
			}
		}
		bool CanShowProperties()
		{
			return Alarm.Device != null || Alarm.Door != null;
		}
		public bool CanShowPropertiesCommand
		{
			get { return CanShowProperties(); }
		}
	}
}