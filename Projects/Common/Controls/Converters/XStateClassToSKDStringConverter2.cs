﻿using FiresecAPI;
using FiresecAPI.GK;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class XStateClassToSKDStringConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateClass = (XStateClass)value;
			switch (stateClass)
			{
				case XStateClass.On:
					return "Открыто";
				case XStateClass.Off:
					return "Закрыто";
				case XStateClass.TurningOff:
					return "Закрывается";
				case XStateClass.Fire1:
					return "Тревога";
				case XStateClass.Attention:
					return "Взлом";
			}
			return stateClass.ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}