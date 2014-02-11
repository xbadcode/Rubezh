﻿using System;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
{
	public static partial class Validator
	{
		static void ValidateSlideWeklyIntervals()
		{
			ValidateSlideWeeklyIntervalEquality();
			foreach (var slideWeeklyInterval in SKDManager.SKDConfiguration.SlideWeeklyIntervals)
			{
				if (string.IsNullOrEmpty(slideWeeklyInterval.Name))
				{
					Errors.Add(new SlideWeeklyIntervalValidationError(slideWeeklyInterval, "Отсутствует название интервала", ValidationErrorLevel.CannotWrite));
				}
				if (slideWeeklyInterval.WeeklyIntervalUIDs.Count == 0)
				{
					Errors.Add(new SlideWeeklyIntervalValidationError(slideWeeklyInterval, "Отсутствуют составляющие части интервала", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		static void ValidateSlideWeeklyIntervalEquality()
		{
			var slideWeeklyIntervals = new HashSet<string>();
			foreach (var slideWeeklyInterval in SKDManager.SKDConfiguration.SlideWeeklyIntervals)
			{
				if (!slideWeeklyIntervals.Add(slideWeeklyInterval.Name))
				{
					Errors.Add(new SlideWeeklyIntervalValidationError(slideWeeklyInterval, "Дублируется название интервала", ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}