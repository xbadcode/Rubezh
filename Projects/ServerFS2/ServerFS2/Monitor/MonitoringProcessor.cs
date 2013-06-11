﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using ServerFS2;
using FS2Api;

namespace ServerFS2.Monitor
{
	public static class MonitoringProcessor
	{
		static List<MonitoringDevice> MonitoringDevices = new List<MonitoringDevice>();
		public static bool DoMonitoring{get; set;}
		static DateTime StartTime;
		static object locker = new object();

		static MonitoringProcessor()
		{
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices.Where(x => DeviceStatesManager.IsMonitoringable(x) && x.IntAddress == 15))
			{
				MonitoringDevices.Add(new MonitoringDevice(device));
			}
			DoMonitoring = false;
			ServerHelper.UsbRunnerBase.NewResponse += new Action<Response>(UsbRunner_NewResponse);
		}

		public static void StartMonitoring()
		{
			if (!DoMonitoring)
			{
				StartTime = DateTime.Now;
				DoMonitoring = true;
				var thread = new Thread(OnRun);
				thread.Start();
			}
		}

		public static void StopMonitoring()
		{
			DoMonitoring = false;
		}

		static void OnRun()
		{
			MonitoringDevices.ForEach(x => x.ToInitializingState());
			MonitoringDevices.Where(x => !x.IsInitialized).ToList().ForEach(x => x.Initialize());
			while (true)
			{
				if (DoMonitoring)
				{
					MonitoringDevices.ForEach(x => x.CheckTasks());
						DeviceStatesManager.UpdateDeviceStateOnPanelState(monitoringDevice.Panel);
				}
			}
		}
					foreach (var monitoringDevice in MonitoringDevices.Where(x => x.ResetStateIds != null && x.ResetStateIds.Count > 0))
					{

		static void UsbRunner_NewResponse(Response response)
						ServerHelper.ResetOnePanelStates(monitoringDevice.Panel, monitoringDevice.ResetStateIds);
						monitoringDevice.ResetStateIds = new List<string>();
						DeviceStatesManager.UpdatePanelState(monitoringDevice.Panel);
					}
		{
			MonitoringDevice monitoringDevice = null;
			Request request = null;
			lock (MonitoringDevice.Locker)
			{
				foreach (var monitoringDevicesItem in MonitoringDevices.ToList())
				{
					foreach (var requestsItem in monitoringDevicesItem.Requests.ToList())
					{
						if (requestsItem != null && requestsItem.Id == response.Id)
						{
							request = requestsItem;
							monitoringDevice = monitoringDevicesItem;
						}
					}
				}
			}
			if (request != null)
			{
				monitoringDevice.ReqestResponsed(request, response);
			}
		}

		public static void WriteStats()
		{
			var timeSpan = DateTime.Now - StartTime;
			Trace.WriteLine("betweenCyclesSpan " + MonitoringDevice.betweenCyclesSpan);
			Trace.WriteLine("betweenDevicesSpan " + MonitoringDevice.betweenDevicesSpan);
			Trace.WriteLine("testTime " + timeSpan);
			foreach (var monitoringDevice in MonitoringDevices)
			{
				Trace.WriteLine(monitoringDevice.Panel.PresentationAddress + " " + (monitoringDevice.AnsweredCount / timeSpan.TotalSeconds).ToString() + " " + monitoringDevice.AnsweredCount + "/" + monitoringDevice.UnAnsweredCount);
			}
		}

		public static void AddStateToReset(Device device, DriverState state)
		{
			foreach (var monitoringDevice in MonitoringDevices)
			{
				if (monitoringDevice.Panel == device)
					monitoringDevice.StatesToReset.Add(state);
			}
		}

		public static void AddPanelResetItems(List<PanelResetItem> panelResetItems)
		{
			foreach (var panelResetItem in panelResetItems)
			{
				var parentPanel = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == panelResetItem.PanelUID);
				if (parentPanel != null)
				{
					AddStateToReset2(parentPanel, panelResetItem.Ids.ToList());
				}
			}
		}

		public static void AddStateToReset2(Device device, List<string> resetStateIds)
		{
			foreach (var monitoringDevice in MonitoringDevices)
			{
				if (monitoringDevice.Panel == device)
					monitoringDevice.ResetStateIds = resetStateIds;
			}
		}

		public static void AddTaskIgnore(Device device)
		{
			foreach (var monitoringDevice in MonitoringDevices)
			{
				if (monitoringDevice.Panel == device.ParentPanel)
				{
					monitoringDevice.DevicesToIgnore = new List<Device>() { device };
				}
			}
		}

		public static void AddTaskResetIgnore(Device device)
		{
			foreach (var monitoringDevice in MonitoringDevices)
			{
				if (monitoringDevice.Panel == device.ParentPanel)
				{
					monitoringDevice.DevicesToResetIgnore = new List<Device>() { device };
				}
			}
		}

		
	}
}