﻿using System;
using System.Windows;
using System.Windows.Shapes;
using Controls;
using Infrastructure;
using Infrastructure.Common;
using Shell = FireMonitor;

namespace FireMonitor.Layout
{
	public partial class App : Shell.App
	{
		protected override Shell.Bootstrapper CreateBootstrapper()
		{
			return new Bootstrapper();
		}

		[STAThread]
		private static void Main()
		{
			if (DateTime.Now.Date > new DateTime(2015, 12, 31))
			{
				MessageBox.Show(Layout.Properties.Resources.DemoLimitStartupDateMessage, "СТРАЖ", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			ServiceFactory.StartupService.Run();
			var app = new App();
			var resourceLocater = new ResourceDescription(typeof(UIBehavior).Assembly, "Themes/Styles.xaml");
			app.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = resourceLocater.Source });
			app.Resources.Add(typeof(Rectangle), new Style(typeof(Rectangle)));
			app.Run();
		}
	}
}