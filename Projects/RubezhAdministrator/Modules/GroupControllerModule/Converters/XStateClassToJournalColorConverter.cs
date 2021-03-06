﻿using System;
using System.Windows.Data;
using System.Windows.Media;
using RubezhAPI.GK;

namespace GKModule.Converters
{
	public class XStateClassToJournalColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((XStateClass)value)
			{
				case XStateClass.Fire2:
					return Brushes.Red;

				case XStateClass.Fire1:
					return Brushes.Red;

				case XStateClass.Attention:
					return Brushes.Yellow;

				case XStateClass.Failure:
					return Brushes.Pink;

				case XStateClass.Service:
					return Brushes.Yellow;

				case XStateClass.Ignore:
					return Brushes.Yellow;

				case XStateClass.Unknown:
					return Brushes.Gray;

				case XStateClass.On:
					return Brushes.LightBlue;

				case XStateClass.AutoOff:
					return Brushes.Yellow;

				case XStateClass.Test:
				case XStateClass.Info:
					return Brushes.Transparent;

				case XStateClass.Norm:
					return Brushes.Transparent;

				default:
					return Brushes.Transparent;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}