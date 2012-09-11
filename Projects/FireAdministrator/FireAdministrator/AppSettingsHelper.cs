﻿using System;
using System.Configuration;
using Infrastructure;

namespace FireAdministrator
{
    public static class AppSettingsHelper
    {
        public static void InitializeAppSettings()
        {
            var appSettings = new AppSettings()
            {
                FS_Address = ConfigurationManager.AppSettings["FS_Address"] as string,
                FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string),
                FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string,
                FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string,

                ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string,
                LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string,
                UseLocalConnection = Convert.ToBoolean(ConfigurationManager.AppSettings["UseLocalConnection"] as string),
                Theme = ConfigurationManager.AppSettings["Theme"] as string
            };
#if DEBUG
            appSettings.IsDebug = true;
#endif
            ServiceFactory.AppSettings = appSettings;
        }
    }
}