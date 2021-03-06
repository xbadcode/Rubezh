﻿using System.ComponentModel;

namespace RubezhAPI.SKD
{
	public enum IntervalType
	{
		[DescriptionAttribute("Дневные графики")]
		Time,

		[DescriptionAttribute("Недельные графики")]
		Weekly,

		[DescriptionAttribute("Суточные графики")]
		SlideDay,

		[DescriptionAttribute("Скользящие понедельные графики")]
		SlideWeekly
	}
}