﻿using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace PlansModule.Converters
{
    public class StateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var icon = StateToIcon((StateType) value);
            if (icon != null)
                return FiresecClient.FileHelper.GetIconFilePath(icon + ".png");
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        string StateToIcon(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.Fire:
                    return "DS_Critical";

                case StateType.Attention:
                    return "DS_Warning";

                case StateType.Failure:
                    return "DS_Error";

                case StateType.Service:
                    return "DS_ServiceRequired";

                case StateType.Off:
                    return "DS_Mute";

                case StateType.Unknown:
                    return "DS_Unknown";

                case StateType.Info:
                    return "DS_Normal";

                case StateType.Norm:
                    return null;

                default:
                    return null;
            }
        }
    }
}