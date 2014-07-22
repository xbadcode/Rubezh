﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;
using JournalModule.Events;

namespace JournalModule.ViewModels
{
	public class ArchiveSettingsViewModel : SaveCancelDialogViewModel
	{
		public ArchiveSettingsViewModel(ArchiveDefaultState archiveDefaultState)
		{
			Title = "Настройки";

			ArchiveDefaultState = new ArchiveDefaultState();
			ArchiveDefaultStates = new ObservableCollection<ArchiveDefaultStateViewModel>();
			foreach (ArchiveDefaultStateType item in Enum.GetValues(typeof(ArchiveDefaultStateType)))
			{
				if (item == ArchiveDefaultStateType.All)
					continue;
				ArchiveDefaultStates.Add(new ArchiveDefaultStateViewModel(item));
			}

			HoursCount = 1;
			DaysCount = 1;
			StartDate = ArchiveFirstDate;
			EndDate = NowDate;

			ServiceFactory.Events.GetEvent<ArchiveDefaultStateCheckedEvent>().Subscribe(OnArchiveDefaultStateCheckedEvent);

			var archiveDefaultStateViewModel = ArchiveDefaultStates.FirstOrDefault(x => x.ArchiveDefaultStateType == archiveDefaultState.ArchiveDefaultStateType);
			if (archiveDefaultStateViewModel != null)
				archiveDefaultStateViewModel.IsActive = true;
			switch (archiveDefaultState.ArchiveDefaultStateType)
			{
				case ArchiveDefaultStateType.LastHours:
					if (archiveDefaultState.Count.HasValue)
						HoursCount = archiveDefaultState.Count.Value;
					break;

				case ArchiveDefaultStateType.LastDays:
					if (archiveDefaultState.Count.HasValue)
						DaysCount = archiveDefaultState.Count.Value;
					break;

				case ArchiveDefaultStateType.FromDate:
					if (archiveDefaultState.StartDate.HasValue)
						StartDate = archiveDefaultState.StartDate.Value;
					break;

				case ArchiveDefaultStateType.RangeDate:
					if (archiveDefaultState.StartDate.HasValue)
						StartDate = archiveDefaultState.StartDate.Value;
					if (archiveDefaultState.EndDate.HasValue)
						EndDate = archiveDefaultState.EndDate.Value;
					break;

				case ArchiveDefaultStateType.All:
				default:
					break;
			}

			AdditionalColumns = new List<JournalColumnTypeViewModel>();
			foreach (JournalColumnType journalColumnType in Enum.GetValues(typeof(JournalColumnType)))
			{
				var journalColumnTypeViewModel = new JournalColumnTypeViewModel(journalColumnType);
				AdditionalColumns.Add(journalColumnTypeViewModel);
				if (archiveDefaultState.AdditionalJournalColumnTypes.Any(x => x == journalColumnType))
				{
					journalColumnTypeViewModel.IsChecked = true;
				}
			}

			PageSize = archiveDefaultState.PageSize;
		}

		public ObservableCollection<ArchiveDefaultStateViewModel> ArchiveDefaultStates { get; private set; }
		public ArchiveDefaultState ArchiveDefaultState { get; private set; }

		ArchiveDefaultStateType _checkedArchiveDefaultStateType;
		public ArchiveDefaultStateType CheckedArchiveDefaultStateType
		{
			get { return _checkedArchiveDefaultStateType; }
			set
			{
				_checkedArchiveDefaultStateType = value;
				OnPropertyChanged(() => CheckedArchiveDefaultStateType);
			}
		}

		public List<JournalColumnTypeViewModel> AdditionalColumns { get; private set; }
		public int HoursCount { get; set; }
		public int DaysCount { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public DateTime ArchiveFirstDate
		{
			get { return ArchiveViewModel.ArchiveFirstDate; }
		}

		public DateTime NowDate
		{
			get { return DateTime.Now; }
		}

		int _pageSize;
		public int PageSize
		{
			get { return _pageSize; }
			set
			{
				_pageSize = value;
				OnPropertyChanged(() => PageSize);
			}
		}

		protected override bool Save()
		{
			ArchiveDefaultState.ArchiveDefaultStateType = ArchiveDefaultStates.First(x => x.IsActive).ArchiveDefaultStateType;
			switch (ArchiveDefaultState.ArchiveDefaultStateType)
			{
				case ArchiveDefaultStateType.LastHours:
					ArchiveDefaultState.Count = HoursCount;
					break;

				case ArchiveDefaultStateType.LastDays:
					ArchiveDefaultState.Count = DaysCount;
					break;

				case ArchiveDefaultStateType.FromDate:
					ArchiveDefaultState.StartDate = StartDate;
					break;

				case ArchiveDefaultStateType.RangeDate:
					ArchiveDefaultState.StartDate = StartDate;
					ArchiveDefaultState.EndDate = EndDate;
					break;

				default:
					break;
			}
			ArchiveDefaultState.AdditionalJournalColumnTypes = new List<JournalColumnType>();
			foreach (var journalColumnTypeViewModel in AdditionalColumns)
			{
				if (journalColumnTypeViewModel.IsChecked)
					ArchiveDefaultState.AdditionalJournalColumnTypes.Add(journalColumnTypeViewModel.JournalColumnType);
			}

			if (PageSize < 10)
				PageSize = 10;
			if (PageSize > 1000)
				PageSize = 1000;
			ArchiveDefaultState.PageSize = PageSize;
			return base.Save();
		}

		void OnArchiveDefaultStateCheckedEvent(ArchiveDefaultStateViewModel archiveDefaultState)
		{
			foreach (var defaultState in ArchiveDefaultStates.Where(x => x != archiveDefaultState))
			{
				defaultState.IsActive = false;
			}
			CheckedArchiveDefaultStateType = archiveDefaultState.ArchiveDefaultStateType;
		}
	}
}