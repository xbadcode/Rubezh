﻿using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using System;

namespace GKModule.ViewModels
{
	public class SchedulePartViewModel : BaseViewModel
	{
		public int Index { get; private set; }
		GKSchedule Schedule;

		public SchedulePartViewModel(GKSchedule schedule, Guid uid, int index)
		{
			Schedule = schedule;
			Index = index;
			AvailableDaySchedules = new ObservableCollection<GKDaySchedule>();
			foreach (var dayInterval in GKManager.DeviceConfiguration.DaySchedules)
			{
				AvailableDaySchedules.Add(dayInterval);
			}
			_selectedDaySchedule = AvailableDaySchedules.FirstOrDefault(x => x.UID == uid);
			if (_selectedDaySchedule == null)
				_selectedDaySchedule = AvailableDaySchedules.FirstOrDefault();
			Update();
		}

		public string Name { get; private set; }
		public ObservableCollection<GKDaySchedule> AvailableDaySchedules { get; private set; }

		GKDaySchedule _selectedDaySchedule;
		public GKDaySchedule SelectedDaySchedule
		{
			get { return _selectedDaySchedule; }
			set
			{
				if (value == null)
					SelectedDaySchedule = AvailableDaySchedules.FirstOrDefault();
				else
				{
					_selectedDaySchedule = value;
					OnPropertyChanged(() => SelectedDaySchedule);
					Schedule.DayScheduleUIDs[Index] = SelectedDaySchedule.UID;
					ServiceFactory.SaveService.GKChanged = true;
				}
			}
		}

		public void Update()
		{
			Name = string.Format("{0}", Index + 1);
			_selectedDaySchedule = AvailableDaySchedules.FirstOrDefault(x => x.UID == Schedule.DayScheduleUIDs[Index]);
			if (_selectedDaySchedule == null)
				_selectedDaySchedule = AvailableDaySchedules.FirstOrDefault();
			OnPropertyChanged(() => SelectedDaySchedule);
			OnPropertyChanged(() => Name);
		}
	}
}