﻿using System;

namespace RubezhAPI.SKD
{
	public class TimeTrackTotal
	{
		public TimeTrackTotal(TimeTrackType timeTrackType)
		{
			TimeTrackType = timeTrackType;
		}

		public TimeTrackType TimeTrackType { get; set; }
		public TimeSpan TimeSpan { get; set; }
	}
}