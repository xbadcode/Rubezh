﻿using System;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using GKOPCServer.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using GKProcessor;
using Infrastructure.Common.Services;
using Microsoft.Practices.Prism.Events;
using XFiresecAPI;

namespace GKOPCServer
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run()
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

			WindowThread = new Thread(new ThreadStart(OnWorkThread));
			WindowThread.Priority = ThreadPriority.Highest;
			WindowThread.SetApartmentState(ApartmentState.STA);
			WindowThread.IsBackground = true;
			WindowThread.Start();
			MainViewStartedEvent.WaitOne();

			UILogger.Log("Соединение с сервером");
			for (int i = 1; i <= 10; i++)
			{
				var message = FiresecManager.Connect(ClientType.OPC, ConnectionSettingsManager.ServerAddress, GlobalSettingsHelper.GlobalSettings.Login, GlobalSettingsHelper.GlobalSettings.Password);
				if (message == null)
					break;
				Thread.Sleep(5000);
				if (i == 10)
				{
					UILogger.Log("Ошибка соединения с сервером: " + message);
					return;
				}
			}

			InitializeGK();
			GKOPCManager.Start();
			UILogger.Log("Готово");
		}

		static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel, false, false);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
			}
			MainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			FiresecManager.Disconnect();
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}

			System.Environment.Exit(1);
		}

		static void InitializeGK()
		{
			ServiceFactoryBase.Events = new EventAggregator();
			GKDBHelper.CanAdd = false;

			UILogger.Log("Загрузка конфигурации с сервера");
			FiresecManager.GetConfiguration("GKOPC/Configuration");

			UILogger.Log("Создание драйверов");
			GKDriversCreator.Create();

			UILogger.Log("Обновление конфигурации");
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DescriptorsManager.Create();
			DescriptorsManager.CreateDynamicObjectsInXManager();
			UILogger.Log("Получение состояний объектов");
			InitializeStates();

			UILogger.Log("Старт мониторинга");
			if (!GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.Start();
			}

			SafeFiresecService.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			SafeFiresecService.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);

			GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);
		}

		static void InitializeStates()
		{
			var gkStates = FiresecManager.FiresecService.GKGetStates();
			CopyGKStates(gkStates);
		}

		static void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				CopyGKStates(gkCallbackResult.GKStates);
			});
		}

		static void CopyGKStates(GKStates gkStates)
		{
			foreach (var remoteDeviceState in gkStates.DeviceStates)
			{
				var device = XManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
				if (device != null)
				{
					remoteDeviceState.CopyTo(device.State);
				}
			}
			foreach (var remoteZoneState in gkStates.ZoneStates)
			{
				var zone = XManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
				if (zone != null)
				{
					remoteZoneState.CopyTo(zone.State);
				}
			}
			foreach (var remoteDirectionState in gkStates.DirectionStates)
			{
				var direction = XManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
				if (direction != null)
				{
					remoteDirectionState.CopyTo(direction.State);
				}
			}
			foreach (var remotePumpStationState in gkStates.PumpStationStates)
			{
				var pumpStation = XManager.PumpStations.FirstOrDefault(x => x.UID == remotePumpStationState.UID);
				if (pumpStation != null)
				{
					remotePumpStationState.CopyTo(pumpStation.State);
				}
			}
			foreach (var delayState in gkStates.DelayStates)
			{
				var delay = XManager.Delays.FirstOrDefault(x => x.UID == delayState.UID);
				if (delay == null)
					delay = XManager.Delays.FirstOrDefault(x => x.PresentationName == delayState.PresentationName);
				if (delay != null)
				{
					delayState.CopyTo(delay.State);
				}
			}
			foreach (var remotePimState in gkStates.PimStates)
			{
				var pim = XManager.Pims.FirstOrDefault(x => x.UID == remotePimState.UID);
				if (pim == null)
					pim = XManager.Pims.FirstOrDefault(x => x.PresentationName == remotePimState.PresentationName);
				if (pim != null)
				{
					remotePimState.CopyTo(pim.State);
				}
			}
		}
	}
}