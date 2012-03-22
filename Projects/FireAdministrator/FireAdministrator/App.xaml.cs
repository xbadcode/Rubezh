﻿using System;
using System.Windows;
using Controls.MessageBox;
using FiresecClient;

namespace FireAdministrator
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if ! DEBUG
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            var bootstrapper = new Bootsrapper();
            bootstrapper.Initialize();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBoxService.ShowException(e.ExceptionObject as Exception);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            FiresecManager.Disconnect();
            VideoModule.VideoModule.Close();
        }
    }
}