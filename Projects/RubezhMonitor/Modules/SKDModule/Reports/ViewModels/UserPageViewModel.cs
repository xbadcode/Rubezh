﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using RubezhAPI.Models;

namespace SKDModule.Reports.ViewModels
{
	public class UserPageViewModel : FilterContainerViewModel
	{
		public UserPageViewModel()
		{
			Title = "Пользователи";
			Users = new ObservableCollection<CheckedItemViewModel<User>>(ClientManager.SecurityConfiguration.Users.Select(item => new CheckedItemViewModel<User>(item)));
			SelectAllCommand = new RelayCommand(() => Users.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Users.ForEach(item => item.IsChecked = false));
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<CheckedItemViewModel<User>> Users { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var userFilter = filter as IReportFilterUser;
			if (userFilter == null)
				return;
			if (userFilter.Users == null)
				userFilter.Users = new List<string>();
			Users.ForEach(item => item.IsChecked = userFilter.Users.Contains(item.Item.Name));
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var userFilter = filter as IReportFilterUser;
			if (userFilter == null)
				return;
            userFilter.Users = Users.Where(item => item.IsChecked).Select(item => item.Item.Name).ToList();
		}
	}
}
