﻿using System.Collections.ObjectModel;
using Common;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows.ViewModels;

namespace ReportsModule.ViewModels
{
	public class SKDReportFilterViewModel : SaveCancelDialogViewModel
	{
		public SKDReportFilter Filter { get; private set; }
		FilterModel _model;

		public SKDReportFilterViewModel(SKDReportFilter filter, FilterModel model)
		{
			Title = "Настройки отчета";
			_model = model;
			Filter = filter;
			Pages = new ObservableCollection<FilterContainerViewModel>();
			Pages.Add(new FilterMainPageViewModel(model, Filter, LoadFilter, UpdateFilter));
			model.Pages.ForEach(page => { if (page.IsActive) Pages.Add(page); });
			if (model.AllowSort)
				Pages.Add(new FilterSortPageViewModel(model.Columns));
			CommandPanel = model.CommandsViewModel;
			LoadFilter(Filter);
		}

		ObservableCollection<FilterContainerViewModel> _pages;
		public ObservableCollection<FilterContainerViewModel> Pages
		{
			get { return _pages; }
			set
			{
				_pages = value;
				OnPropertyChanged(() => Pages);
			}
		}

		protected override bool Save()
		{
			UpdateFilter(Filter);
			return base.Save();
		}
		void LoadFilter(SKDReportFilter filter)
		{
			using (new WaitWrapper())
			{
				if (_model.MainViewModel != null)
					_model.MainViewModel.LoadFilter(filter);
				if (_model.CommandsViewModel != null)
					_model.CommandsViewModel.LoadFilter(filter);
				Pages.ForEach(page => page.LoadFilter(filter));
			}
		}
		void UpdateFilter(SKDReportFilter filter)
		{
			if (_model.MainViewModel != null)
				_model.MainViewModel.UpdateFilter(filter);
			if (_model.CommandsViewModel != null)
				_model.CommandsViewModel.UpdateFilter(filter);
			Pages.ForEach(page => page.UpdateFilter(filter));
		}
	}
}