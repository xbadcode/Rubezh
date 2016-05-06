﻿using StrazhAPI;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BeeperTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((StrazhAPI.Models.BeeperType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (StrazhAPI.Models.BeeperType)value;
		}
	}
}