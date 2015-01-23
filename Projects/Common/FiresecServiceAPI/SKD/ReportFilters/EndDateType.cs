﻿using System.ComponentModel;

namespace FiresecAPI.SKD.ReportFilters
{
	public enum EndDateType
	{
		[Description("Сутки")]
		Day,

		[Description("Неделя")]
		Week,

		[Description("Месяц")]
		Month,

		[Description("Произвольный период")]
		Arbitrary
	}
}