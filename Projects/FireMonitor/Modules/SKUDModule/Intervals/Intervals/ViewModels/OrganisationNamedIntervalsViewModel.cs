﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.Intervals.Common.ViewModels;
using Organisation = FiresecAPI.Organisation;

namespace SKDModule.ViewModels
{
	public class OrganisationNamedIntervalsViewModel : OrganisationIntervalViewModel<NamedIntervalViewModel, NamedInterval>
	{
		private NamedInterval _intervalToCopy;

		public OrganisationNamedIntervalsViewModel(Organisation organization)
			: base(organization)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		protected override NamedIntervalViewModel CreateViewModel(NamedInterval model)
		{
			return new NamedIntervalViewModel(model);
		}

		private void OnAdd()
		{
			var namedIntervalDetailsViewModel = new NamedIntervalDetailsViewModel(this);
			if (DialogService.ShowModalWindow(namedIntervalDetailsViewModel) && NamedIntervalHelper.Save(namedIntervalDetailsViewModel.NamedInterval))
			{
				var namedIntervalViewModel = new NamedIntervalViewModel(namedIntervalDetailsViewModel.NamedInterval);
				ViewModels.Add(namedIntervalViewModel);
				SelectedViewModel = namedIntervalViewModel;
			}
		}

		private void OnDelete()
		{
			if (NamedIntervalHelper.MarkDeleted(SelectedViewModel.Model))
				ViewModels.Remove(SelectedViewModel);
		}
		private bool CanDelete()
		{
			return SelectedViewModel != null;
		}

		private void OnEdit()
		{
			var namedInrervalDetailsViewModel = new NamedIntervalDetailsViewModel(this, SelectedViewModel.Model);
			if (DialogService.ShowModalWindow(namedInrervalDetailsViewModel))
			{
				NamedIntervalHelper.Save(SelectedViewModel.Model);
				SelectedViewModel.Update();
			}
		}
		private bool CanEdit()
		{
			return SelectedViewModel != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_intervalToCopy = CopyInterval(SelectedViewModel.Model);
		}
		private bool CanCopy()
		{
			return SelectedViewModel != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_intervalToCopy);
			NamedIntervalHelper.Save(newInterval);
			var timeInrervalViewModel = new NamedIntervalViewModel(newInterval);
			ViewModels.Add(timeInrervalViewModel);
			SelectedViewModel = timeInrervalViewModel;
		}
		private bool CanPaste()
		{
			return _intervalToCopy != null;
		}

		private NamedInterval CopyInterval(NamedInterval source)
		{
			var copy = new NamedInterval();
			copy.Name = source.Name;
			copy.Description = source.Description;
			copy.SlideTime = source.SlideTime;
			foreach (var timeInterval in source.TimeIntervals)
			{
				var copyNamedIntervalPart = new TimeInterval()
				{
					BeginTime = timeInterval.BeginTime,
					EndTime = timeInterval.EndTime,
					IntervalTransitionType = timeInterval.IntervalTransitionType,
				};
				copy.TimeIntervals.Add(copyNamedIntervalPart);
			}
			return copy;
		}
	}
}