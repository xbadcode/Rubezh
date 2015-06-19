﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DeviceControls;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;

namespace GKModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan, XStateClass>
	{
		public static MapSource Cache { get; private set; }

		private Dictionary<Plan, PlanMonitor> _monitors;
		public PlanPresenter()
		{
			Cache = new MapSource();
			Cache.Add<GKSKDZone>(() => GKManager.SKDZones);
			Cache.Add<GKDevice>(() => GKManager.Devices);
			Cache.Add<GKDoor>(() => GKManager.Doors);

			ServiceFactory.Events.GetEvent<ShowGKDeviceOnPlanEvent>().Subscribe(OnShowGKDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKSKDZoneOnPlanEvent>().Subscribe(OnShowGKSKDZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKDoorOnPlanEvent>().Subscribe(OnShowGKDoorOnPlan);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			_monitors = new Dictionary<Plan, PlanMonitor>();
		}

		#region IPlanPresenter<Plan> Members

		public void SubscribeStateChanged(Plan plan, Action callBack)
		{
			Cache.BuildAllSafe();
			if (_monitors.ContainsKey(plan))
				_monitors[plan].AddCallBack(callBack);
			else
				_monitors.Add(plan, new PlanMonitor(plan, callBack));
		}

		public NamedStateClass GetNamedStateClass(Plan plan)
		{
			return _monitors.ContainsKey(plan) ? _monitors[plan].GetNamedStateClass() : new NamedStateClass();
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementGKDevices.Where(x => x.DeviceUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementRectangleGKSKDZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHiddenZone))
				yield return element;
			foreach (var element in plan.ElementPolygonGKSKDZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHiddenZone))
				yield return element;
			foreach (var element in plan.ElementGKDoors.Where(x => x.DoorUID != Guid.Empty))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementGKDevice)
				presenterItem.OverridePainter(new GKDevicePainter(presenterItem));
			else if (presenterItem.Element is ElementPolygonGKSKDZone || presenterItem.Element is ElementRectangleGKSKDZone)
				presenterItem.OverridePainter(new GKSKDZonePainter(presenterItem));
			else if (presenterItem.Element is ElementGKDoor)
				presenterItem.OverridePainter(new GKDoorPainter(presenterItem));
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("GKDevice.ExtensionAttached.BuildMap: {0}"))
				Cache.BuildAllSafe();
		}

		#endregion

		public void Initialize()
		{
			_monitors.Clear();
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}

		private void OnShowGKDeviceOnPlan(GKDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementGKDevices)
					if (element.DeviceUID == device.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}

		private void OnShowGKSKDZoneOnPlan(GKSKDZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleGKSKDZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonGKSKDZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
		private void OnShowGKDoorOnPlan(GKDoor door)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementGKDoors)
					if (element.DoorUID == door.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
	}
}