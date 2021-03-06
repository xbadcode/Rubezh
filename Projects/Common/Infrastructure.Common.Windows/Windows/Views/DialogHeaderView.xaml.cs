﻿using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows.Views
{
	public partial class DialogHeaderView : UserControl
	{
		public DialogHeaderView()
		{
			InitializeComponent();
		}

		private void OnCloseButton(object sender, RoutedEventArgs e)
		{
			(((DialogHeaderViewModel)DataContext).Content).Close(false);
		}
	}
}