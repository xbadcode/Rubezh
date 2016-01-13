﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Infrastructure;
using Infrastructure.Events;
using RubezhAPI.Journal;
using RubezhAPI.GK;
using System.Windows;
using RubezhClient;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using RubezhAPI;

namespace FireMonitor
{
	internal class AutoActivationWatcher
	{
		public void Run()
		{
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournals);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournals);
		}

		void OnNewJournals(List<JournalItem> journalItems)
		{
			AutoActivate();
			if (ClientSettings.AutoActivationSettings.IsPlansAutoActivation)
				foreach (var journalItem in journalItems)
				{
					var globalStateClass = GKManager.GetMinStateClass();
					var stateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
					if (stateClass <= globalStateClass || (globalStateClass != XStateClass.Fire1 && globalStateClass != XStateClass.Fire2 && globalStateClass != XStateClass.Attention))
						switch (journalItem.JournalObjectType)
						{
							case JournalObjectType.GKDevice:
								var device = GKManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowDevice(device))
									ShowOnPlanHelper.ShowDevice(device);
								break;
							case JournalObjectType.GKZone:
								var zone = GKManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowZone(zone))
									ShowOnPlanHelper.ShowZone(zone);
								break;
							case JournalObjectType.GKDelay:
								var delay = GKManager.Delays.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowDelay(delay))
									ShowOnPlanHelper.ShowDelay(delay);
								break;
							case JournalObjectType.GKDirection:
								var direction = GKManager.Directions.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowDirection(direction))
									ShowOnPlanHelper.ShowDirection(direction);
								break;
							case JournalObjectType.GKGuardZone:
								var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowGuardZone(guardZone))
									ShowOnPlanHelper.ShowGuardZone(guardZone);
								break;
							case JournalObjectType.GKMPT:
								var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowMPT(mpt))
									ShowOnPlanHelper.ShowMPT(mpt);
								break;
							case JournalObjectType.VideoDevice:
								var camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowCamera(camera))
									ShowOnPlanHelper.ShowCamera(camera);
								break;
						}
				}
		}

		void AutoActivate()
		{
			if (ClientSettings.AutoActivationSettings.IsAutoActivation)
			{
				var window = DialogService.GetActiveWindow();
				if (window != null && !window.IsActive)
				{
					if (ApplicationService.ApplicationWindow != null)
						ApplicationService.ApplicationWindow.WindowState = WindowState.Maximized;
					window.Activate();
				}
			}
		}
	}
}