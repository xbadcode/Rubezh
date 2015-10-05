﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace Resurs.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			DevicesViewModel = new DevicesViewModel();
			ApartmentsViewModel = new ApartmentsViewModel();
			ReportsViewModel = new ReportsViewModel();
			UsersViewModel = new UsersViewModel();
			JournalEventsViewModel = new JournalEventsViewModel();
			TariffsViewModel = new TariffsViewModel();
		}

		public DevicesViewModel DevicesViewModel { get; private set; }
		public ApartmentsViewModel ApartmentsViewModel { get; private set; }
		public ReportsViewModel ReportsViewModel { get; private set; }
		public UsersViewModel UsersViewModel { get; private set; }
		public JournalEventsViewModel JournalEventsViewModel { get; private set; }
		public TariffsViewModel TariffsViewModel { get; private set; }
    }
}