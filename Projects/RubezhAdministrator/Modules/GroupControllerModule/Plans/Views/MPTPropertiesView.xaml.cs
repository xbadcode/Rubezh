﻿using System.Windows.Controls;

namespace GKModule.Plans.Views
{
	public partial class MPTPropertiesView : UserControl
	{
		public MPTPropertiesView()
		{
			InitializeComponent();
		}
		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null && !dataGrid.IsMouseOver)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}