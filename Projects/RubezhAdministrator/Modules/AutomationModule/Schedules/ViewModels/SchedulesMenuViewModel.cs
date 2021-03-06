﻿using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SchedulesMenuViewModel : BaseViewModel
	{
		public SchedulesMenuViewModel(SchedulesViewModel context)
		{
			Context = context;
		}

		public SchedulesViewModel Context { get; private set; }
	}
}