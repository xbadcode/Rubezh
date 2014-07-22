﻿using System;
using System.Windows.Data;
using Infrastructure.Models;

namespace GKModule.Converters
{
	public class JournalColumnTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var journalColumnType = (XJournalColumnType)value;
			switch (journalColumnType)
			{
				case XJournalColumnType.GKIpAddress:
					return "/Controls;component/GKIcons/GK.png";

				case XJournalColumnType.SubsystemType:
					return "/Controls;component/Images/PC.png";

				case XJournalColumnType.UserName:
					return "/Controls;component/Images/PCUser.png";
				
				default:
					return "/Controls;component/Images/blank.png";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}