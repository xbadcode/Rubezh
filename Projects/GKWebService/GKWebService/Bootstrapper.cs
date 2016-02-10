﻿using System;
using System.Linq;
using System.Threading;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKProcessor;
using Infrastructure.Common;
using System.Diagnostics;
using RubezhAPI.Journal;

namespace GKWebService
{
	public static class Bootstrapper
	{
		public static void Run() {
		    SubscribeOnServiceStateEvents();

			for (int i = 1; i <= 10; i++)
			{
				var message = ClientManager.Connect(ClientType.WebService, ConnectionSettingsManager.ServerAddress, GlobalSettingsHelper.GlobalSettings.AdminLogin, "");
				if (message == null)
					break;
				Thread.Sleep(5000);
				if (i == 10)
				{
					//UILogger.Log("Ошибка соединения с сервером: " + message);
					return;
				}
			}
            
			InitServer();
		}

	    private static void InitServer() {
	        InitializeGk();
	        ClientManager.StartPoll();
	    }

	    private static void SubscribeOnServiceStateEvents() {
	        SafeFiresecService.ConfigurationChangedEvent += SafeFiresecServiceOnConfigurationChangedEvent;
            SafeFiresecService.ConnectionAppeared += SafeFiresecServiceOnConnectionAppeared;
	    }

	    private static void SafeFiresecServiceOnConnectionAppeared() {
            //InitServer();
            //PlansUpdater.Instance.
	    }

	    private static void SafeFiresecServiceOnConfigurationChangedEvent() {
            //InitServer();
	    }

	    static void InitializeGk()
		{
			ClientManager.GetConfiguration("Sergey_GKOPC/Configuration");
            
			GKDriversCreator.Create();
			GKManager.UpdateConfiguration();
			GKManager.CreateStates();
			DescriptorsManager.Create();
			InitializeStates();

			SafeFiresecService.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGkCallbackResult);
			SafeFiresecService.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGkCallbackResult);

			//SafeFiresecService.NewJournalItemEvent -= new Action<JournalItem>(OnNewJournalItem);
			//SafeFiresecService.NewJournalItemEvent += new Action<JournalItem>(OnNewJournalItem);

			ShowAllObjects();
		}

		static void InitializeStates()
		{
			var gkStates = ClientManager.FiresecService.GKGetStates();
			CopyGkStates(gkStates);
		}

		static void OnGkCallbackResult(GKCallbackResult gkCallbackResult)
		{
			CopyGkStates(gkCallbackResult.GKStates);
		}

		static void CopyGkStates(GKStates gkStates)
		{
			foreach (var remoteDeviceState in gkStates.DeviceStates)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
				if (device != null)
				{
					remoteDeviceState.CopyTo(device.State);
				}
			}
			foreach (var remoteZoneState in gkStates.ZoneStates)
			{
				var zone = GKManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
				if (zone != null)
				{
					remoteZoneState.CopyTo(zone.State);
				}
			}
			foreach (var remoteDirectionState in gkStates.DirectionStates)
			{
				var direction = GKManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
				if (direction != null)
				{
					remoteDirectionState.CopyTo(direction.State);
				}
			}
			foreach (var remotePumpStationState in gkStates.PumpStationStates)
			{
				var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == remotePumpStationState.UID);
				if (pumpStation != null)
				{
					remotePumpStationState.CopyTo(pumpStation.State);
				}
			}
			foreach (var delayState in gkStates.DelayStates)
			{
				var delay = GKManager.Delays.FirstOrDefault(x => x.UID == delayState.UID);
				if (delay == null)
					delay = GKManager.Delays.FirstOrDefault(x => x.PresentationName == delayState.PresentationName);
				if (delay != null)
				{
					delayState.CopyTo(delay.State);
				}
			}
			foreach (var remotePimState in gkStates.PimStates)
			{
				var pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == remotePimState.UID);
				if (pim == null)
					pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.PresentationName == remotePimState.PresentationName);
				if (pim != null)
				{
					remotePimState.CopyTo(pim.State);
				}
			}
			foreach (var mptState in gkStates.MPTStates)
			{
				var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == mptState.UID);
				if (mpt != null)
				{
					mptState.CopyTo(mpt.State);
				}
			}
			foreach (var guardZoneState in gkStates.GuardZoneStates)
			{
				var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == guardZoneState.UID);
				if (guardZone != null)
				{
					guardZoneState.CopyTo(guardZone.State);
				}
			}
			foreach (var doorState in gkStates.DoorStates)
			{
				var door = GKManager.MPTs.FirstOrDefault(x => x.UID == doorState.UID);
				if (door != null)
				{
					doorState.CopyTo(door.State);
				}
			}
		}

		static void OnNewJournalItem(JournalItem journalItem)
		{
			var journalItemViewModel = new JournalItemViewModel(journalItem);
			Trace.WriteLine("");
		}

		static void ShowAllObjects()
		{
            //foreach(var device in GKManager.Devices)
            //{
            //    Trace.WriteLine(device.PresentationName);
            //}
            //foreach (var zone in GKManager.Zones)
            //{
            //    Trace.WriteLine(zone.PresentationName);
            //}
            //foreach (var direction in GKManager.Directions)
            //{
            //    Trace.WriteLine(direction.PresentationName);
            //}
            //foreach (var delay in GKManager.Delays)
            //{
            //    Trace.WriteLine(delay.PresentationName);
            //}
            //foreach (var mpt in GKManager.MPTs)
            //{
            //    Trace.WriteLine(mpt.PresentationName);
            //}
            //foreach (var pumpStation in GKManager.PumpStations)
            //{
            //    Trace.WriteLine(pumpStation.PresentationName);
            //}
            //foreach (var guardZone in GKManager.GuardZones)
            //{
            //    Trace.WriteLine(guardZone.PresentationName);
            //}
            //foreach (var door in GKManager.Doors)
            //{
            //    Trace.WriteLine(door.PresentationName);
            //}
		}
	}
}