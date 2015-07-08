﻿using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public GKDevice Device { get; private set; }
		
		public DeviceViewModel(GKDevice device)
		{
			Device = device;
		}
	}
}