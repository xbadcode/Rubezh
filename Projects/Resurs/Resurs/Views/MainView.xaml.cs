﻿using System;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.Windows;
using Resurs.Service;

namespace Resurs.Views
{
	public partial class MainView : Window
	{
		public MainView()
		{
			InitializeComponent();
			NotifyIconService.Start(OnShow, OnClose);
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			ShowInTaskbar = true;
		}

		void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
			ShowInTaskbar = false;
		}

		void OnShow(object sender, EventArgs e)
		{
			WindowState = WindowState.Normal;
			Show();
			Activate();
			ShowInTaskbar = true;
		}
		void OnClose(object sender, EventArgs e)
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить АРМ Ресурс?"))
			{
				Close();
				NotifyIconService.Stop();
				Bootstrapper.Close();
			}
		}
	}
}