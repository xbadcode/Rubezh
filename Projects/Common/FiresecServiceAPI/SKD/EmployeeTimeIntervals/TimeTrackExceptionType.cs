﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum TimeTrackExceptionType
	{
		[Description("Нет")]
		None,

		[Description("Больничный")]
		Hospital,

		[Description("Отпуск")]
		Vacation,

		[Description("Командировка")]
		Trip
	}
}