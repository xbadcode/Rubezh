﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class TotalViewModel : BaseViewModel
	{
		public TotalViewModel(TimeTrackType timeTrackType, TimeSpan timeSpan)
		{
			TimeTrackType = timeTrackType;
			TimeSpan = timeSpan;
		}

		public TimeTrackType TimeTrackType { get; private set; }
		public TimeSpan TimeSpan { get; private set; }
	}
}