﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using XFiresecAPI;
using Infrustructure.Plans.Elements;
using FiresecClient;
using FiresecAPI;

namespace GKModule.Plans
{
	internal class PlanMonitor
	{
		private Plan _plan;
		private Action _callBack;
		private List<XDeviceState> _xdeviceStates;
		private List<XZoneState> _xzoneStates;

		public PlanMonitor(Plan plan, Action callBack)
		{
			_plan = plan;
			_callBack = callBack;
			_xdeviceStates = new List<XDeviceState>();
			_xzoneStates = new List<XZoneState>();
			Initialize();
		}
		private void Initialize()
		{
			_plan.ElementXDevices.ForEach(item => Initialize(item));
			_plan.ElementRectangleXZones.ForEach(item => Initialize(item));
			_plan.ElementPolygonXZones.ForEach(item => Initialize(item));
		}
		private void Initialize(ElementXDevice element)
		{
			var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == element.XDeviceUID);
			if (device != null)
			{
				_xdeviceStates.Add(device.DeviceState);
				device.DeviceState.StateChanged += _callBack;
			}
		}
		private void Initialize(IElementZone element)
		{
			if (element.ZoneUID != Guid.Empty)
			{
				var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == element.ZoneUID);
				if (zone != null)
				{
					_xzoneStates.Add(zone.ZoneState);
					zone.ZoneState.StateChanged += _callBack;
				}
			}
		}

		public StateType GetState()
		{
			var result = StateType.No;
			foreach (var state in _xdeviceStates)
			{
				var stateType = state.GetStateType();
				if (stateType < result)
					result = stateType;
			}
			foreach (var state in _xzoneStates)
			{
				var stateType = state.GetStateType();
				if (stateType < result)
					result = stateType;
			}
			return result;
		}
	}
}
