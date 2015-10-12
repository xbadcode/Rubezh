﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using System;
using System.Collections.Generic;

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
				if (Alarm.Zone != null)
					return Alarm.Zone.PresentationName;
				if (Alarm.GuardZone != null)
					return Alarm.GuardZone.PresentationName;
				if (Alarm.Direction != null)
					return Alarm.Direction.PresentationName;
				if (Alarm.Door != null)
					return Alarm.Door.PresentationName;
				if (Alarm.Delay != null)
					return Alarm.Delay.PresentationName;
				if (Alarm.Mpt != null)
					return Alarm.Mpt.PresentationName;
				return null;
			}
		}

		public string ImageSource
		{
			get
			{
				if (Alarm.Device != null)
					return Alarm.Device.Driver.ImageSource;
				if (Alarm.Zone != null)
					return "/Controls;component/Images/Zone.png";
				if (Alarm.GuardZone != null)
					return "/Controls;component/Images/GuardZone.png";
				if (Alarm.Direction != null)
					return "/Controls;component/Images/Blue_Direction.png";
				if (Alarm.Door != null)
					return "/Controls;component/Images/Door.png";
				if (Alarm.Delay != null)
					return "/Controls;component/Images/Delay.png";
				if (Alarm.Mpt != null)
					return "/Controls;component/Images/Mpt.png";
				return null;
			}
		}

		public XStateClass ObjectStateClass
		{
			get
			{
				if (Alarm.Device != null)
					return Alarm.Device.State.StateClass;
				if (Alarm.Zone != null)
					return Alarm.Zone.State.StateClass;
				if (Alarm.GuardZone != null)
					return Alarm.GuardZone.State.StateClass;
				if (Alarm.Direction != null)
					return Alarm.Direction.State.StateClass;
				if (Alarm.Door != null)
					return Alarm.Door.State.StateClass;
				if (Alarm.Delay != null)
					return Alarm.Delay.State.StateClass;
				if (Alarm.Mpt != null)
					return Alarm.Mpt.State.StateClass;
				return XStateClass.Norm;
			}
		}

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();

			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
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
				if (Alarm.Zone != null)
				{
					elementBase = plan.ElementRectangleGKZones.FirstOrDefault(x => x.ZoneUID == Alarm.Zone.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Zone = Alarm.Zone;
						Plans.Add(alarmPlanViewModel);
						continue;
					}

					elementBase = plan.ElementPolygonGKZones.FirstOrDefault(x => x.ZoneUID == Alarm.Zone.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Zone = Alarm.Zone;
						Plans.Add(alarmPlanViewModel);
					}
				}
				if (Alarm.GuardZone != null)
				{
					elementBase = plan.ElementRectangleGKGuardZones.FirstOrDefault(x => x.ZoneUID == Alarm.GuardZone.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.GuardZone = Alarm.GuardZone;
						Plans.Add(alarmPlanViewModel);
						continue;
					}

					elementBase = plan.ElementPolygonGKGuardZones.FirstOrDefault(x => x.ZoneUID == Alarm.GuardZone.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.GuardZone = Alarm.GuardZone;
						Plans.Add(alarmPlanViewModel);
					}
				}
				if (Alarm.Delay != null)
				{
					elementBase = plan.ElementRectangleGKDelays.FirstOrDefault(x => x.DelayUID == Alarm.Delay.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Delay = Alarm.Delay;
						Plans.Add(alarmPlanViewModel);
						continue;
					}
					elementBase = plan.ElementPolygonGKDelays.FirstOrDefault(x => x.DelayUID == Alarm.Delay.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Delay = Alarm.Delay;
						Plans.Add(alarmPlanViewModel);
						continue;
					}
				}
				if (Alarm.Direction != null)
				{
					elementBase = plan.ElementRectangleGKDirections.FirstOrDefault(x => x.DirectionUID == Alarm.Direction.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Direction = Alarm.Direction;
						Plans.Add(alarmPlanViewModel);
						continue;
					}

					elementBase = plan.ElementPolygonGKDirections.FirstOrDefault(x => x.DirectionUID == Alarm.Direction.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Direction = Alarm.Direction;
						Plans.Add(alarmPlanViewModel);
					}
				}
				if (Alarm.Mpt != null)
				{
					elementBase = plan.ElementRectangleGKMPTs.FirstOrDefault(x => x.MPTUID == Alarm.Mpt.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.MPT = Alarm.Mpt;
						Plans.Add(alarmPlanViewModel);
						continue;
					}

					elementBase = plan.ElementPolygonGKMPTs.FirstOrDefault(x => x.MPTUID == Alarm.Mpt.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.MPT = Alarm.Mpt;
						Plans.Add(alarmPlanViewModel);
					}
				}
				if (Alarm.Delay != null)
				{
					elementBase = plan.ElementRectangleGKDelays.FirstOrDefault(x => x.DelayUID == Alarm.Delay.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Delay = Alarm.Delay;
						Plans.Add(alarmPlanViewModel);
						continue;
					}

					elementBase = plan.ElementPolygonGKDelays.FirstOrDefault(x => x.DelayUID == Alarm.Delay.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Delay = Alarm.Delay;
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
			if (Alarm.Zone != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKZoneEvent>().Publish(Alarm.Zone.UID);
			}
			if (Alarm.GuardZone != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKGuardZoneEvent>().Publish(Alarm.GuardZone.UID);
			}
			if (Alarm.Direction != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKDirectionEvent>().Publish(Alarm.Direction.UID);
			}
			if (Alarm.Mpt != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKMPTEvent>().Publish(Alarm.Mpt.UID);
			}
			if (Alarm.Delay != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKDelayEvent>().Publish(Alarm.Delay.UID);
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
			if (Alarm.Zone != null)
			{
				ShowOnPlanHelper.ShowZone(Alarm.Zone);
			}
			if (Alarm.GuardZone != null)
			{
				ShowOnPlanHelper.ShowGuardZone(Alarm.GuardZone);
			}
			if (Alarm.Direction != null)
			{
				ShowOnPlanHelper.ShowDirection(Alarm.Direction);
			}
			if (Alarm.Mpt != null)
			{
				ShowOnPlanHelper.ShowMPT(Alarm.Mpt);
			}
			if (Alarm.Delay != null)
			{
				ShowOnPlanHelper.ShowDelay(Alarm.Delay);
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
			if (Alarm.Zone != null)
			{
				return ShowOnPlanHelper.CanShowZone(Alarm.Zone);
			}
			if (Alarm.GuardZone != null)
			{
				return ShowOnPlanHelper.CanShowGuardZone(Alarm.GuardZone);
			}
			if (Alarm.Direction != null)
			{
				return ShowOnPlanHelper.CanShowDirection(Alarm.Direction);
			}
			if (Alarm.Mpt != null)
			{
				return ShowOnPlanHelper.CanShowMPT(Alarm.Mpt);
			}
			if (Alarm.Delay != null)
			{
				return ShowOnPlanHelper.CanShowDelay(Alarm.Delay);
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
				if (Alarm.Zone != null)
				{
					switch (Alarm.AlarmType)
					{
						case GKAlarmType.Fire1:
							ClientManager.FiresecService.GKResetFire1(Alarm.Zone);
							break;

						case GKAlarmType.Fire2:
							ClientManager.FiresecService.GKResetFire2(Alarm.Zone);
							break;
					}
				}
				if (Alarm.GuardZone != null)
				{
					switch (Alarm.AlarmType)
					{
						case GKAlarmType.GuardAlarm:
							ClientManager.FiresecService.GKReset(Alarm.GuardZone);
							break;
					}
				}
				if (Alarm.Device != null)
				{
					ClientManager.FiresecService.GKReset(Alarm.Device);
				}
				if (Alarm.Door != null)
				{
					switch (Alarm.AlarmType)
					{
						case GKAlarmType.GuardAlarm:
							ClientManager.FiresecService.GKReset(Alarm.Door);
							break;
					}
				}
			}
		}
		bool CanReset()
		{
			if (Alarm.Zone != null)
			{
				return (Alarm.AlarmType == GKAlarmType.Fire1 || Alarm.AlarmType == GKAlarmType.Fire2);
			}
			if (Alarm.GuardZone != null)
			{
				return (Alarm.AlarmType == GKAlarmType.GuardAlarm);
			}
			if (Alarm.Device != null)
			{
				if (Alarm.Device.DriverType == GKDriverType.RSR2_MAP4)
				{
					return Alarm.Device.State.StateClasses.Contains(XStateClass.Fire2) || Alarm.Device.State.StateClasses.Contains(XStateClass.Fire1);
				}
			}
			if (Alarm.Door != null)
			{
				return (Alarm.AlarmType == GKAlarmType.GuardAlarm);
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
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Device);
					}
				}

				if (Alarm.Zone != null)
				{
					if (Alarm.Zone.State.StateClasses.Contains(XStateClass.Ignore))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Zone);
					}
				}

				if (Alarm.GuardZone != null)
				{
					if (Alarm.GuardZone.State.StateClasses.Contains(XStateClass.Ignore))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.GuardZone);
					}
				}

				if (Alarm.Direction != null)
				{
					if (Alarm.Direction.State.StateClasses.Contains(XStateClass.Ignore))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Direction);
					}
				}

				if (Alarm.Mpt != null)
				{
					if (Alarm.Mpt.State.StateClasses.Contains(XStateClass.Ignore))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Mpt);
					}
				}

				if (Alarm.Delay != null)
				{
					if (Alarm.Delay.State.StateClasses.Contains(XStateClass.Ignore))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Delay);
					}
				}

				if (Alarm.Door != null)
				{
					if (Alarm.Door.State.StateClasses.Contains(XStateClass.Ignore))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Door);
					}
				}
			}
		}
		bool CanResetIgnore()
		{
			if (Alarm.AlarmType != GKAlarmType.Ignore)
				return false;

			if (Alarm.Device != null)
			{
				if (Alarm.Device.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
					return true;
			}

			if (Alarm.Zone != null)
			{
				if (Alarm.Zone.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
					return true;
			}

			if (Alarm.GuardZone != null)
			{
				if (Alarm.GuardZone.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control))
					return true;
			}

			if (Alarm.Mpt != null)
			{
				if (Alarm.Mpt.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
					return true;
			}

			if (Alarm.Delay != null)
			{
				if (Alarm.Delay.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
					return true;
			}

			if (Alarm.Direction != null)
			{
				if (Alarm.Direction.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
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
					if (Alarm.Device.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Device);
					}
				}
				if (Alarm.Direction != null)
				{
					if (Alarm.Direction.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Direction);
					}
				}
				if (Alarm.Delay != null)
				{
					if (Alarm.Delay.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Delay);
					}
				}
				if (Alarm.Mpt != null)
				{
					if (Alarm.Mpt.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(Alarm.Mpt);
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
				if (Alarm.Direction != null)
				{
					return Alarm.Direction.State.StateClasses.Contains(XStateClass.AutoOff);
				}
				if (Alarm.Delay != null)
				{
					return Alarm.Delay.State.StateClasses.Contains(XStateClass.AutoOff);
				}
				if (Alarm.Mpt != null)
				{
					return Alarm.Mpt.State.StateClasses.Contains(XStateClass.AutoOff);
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
			var uids = new List<Guid>();
			if (Alarm.Device != null)
				uids.Add(Alarm.Device.UID);
			if (Alarm.Zone != null)
				uids.Add(Alarm.Zone.UID);
			if (Alarm.GuardZone != null)
				uids.Add(Alarm.GuardZone.UID);
			if (Alarm.Direction != null)
				uids.Add(Alarm.Direction.UID);
			if (Alarm.Mpt != null)
				uids.Add(Alarm.Mpt.UID);
			if (Alarm.Delay != null)
				uids.Add(Alarm.Delay.UID);
			if (Alarm.Door != null)
				uids.Add(Alarm.Door.UID);
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(uids);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			if (Alarm.Device != null)
			{
				DialogService.ShowWindow(new DeviceDetailsViewModel(Alarm.Device));
			}
			if (Alarm.Zone != null)
			{
				DialogService.ShowWindow(new ZoneDetailsViewModel(Alarm.Zone));
			}
			if (Alarm.GuardZone != null)
			{
				DialogService.ShowWindow(new GuardZoneDetailsViewModel(Alarm.GuardZone));
			}
			if (Alarm.Direction != null)
			{
				DialogService.ShowWindow(new DirectionDetailsViewModel(Alarm.Direction));
			}
			if (Alarm.Mpt != null)
			{
				DialogService.ShowWindow(new MPTDetailsViewModel(Alarm.Mpt));
			}
			if (Alarm.Delay != null)
			{
				DialogService.ShowWindow(new DelayDetailsViewModel(Alarm.Delay));
			}
			if (Alarm.Door != null)
			{
				DialogService.ShowWindow(new DoorDetailsViewModel(Alarm.Door));
			}
		}
		bool CanShowProperties()
		{
			return Alarm.Device != null || Alarm.Zone != null || Alarm.GuardZone != null || Alarm.Direction != null || Alarm.Mpt != null || Alarm.Delay != null || Alarm.Door != null;
		}
		public bool CanShowPropertiesCommand
		{
			get { return CanShowProperties(); }
		}
	}
}