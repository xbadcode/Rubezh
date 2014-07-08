﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class LockIntervalsViewModel : BaseViewModel
	{
		public LockIntervalsViewModel(SKDDoorConfiguration doorConfiguration)
		{
			DayIntervals = new ObservableCollection<DayIntervalViewModel>();
			for (int i = 0; i < 7; i++)
			{
				var doorDayInterval = doorConfiguration.DoorDayIntervalsCollection.DoorDayIntervals[i];
				var dayIntervalViewModel = new DayIntervalViewModel(i+1);
				DayIntervals.Add(dayIntervalViewModel);

				foreach (var doorDayIntervalPart in doorDayInterval.DoorDayIntervalParts)
				{
					var intervalPartViewModel = new IntervalPartViewModel();
					intervalPartViewModel.StartHour = doorDayIntervalPart.StartHour;
					intervalPartViewModel.StartMinute = doorDayIntervalPart.StartMinute;
					intervalPartViewModel.EndHour = doorDayIntervalPart.EndHour;
					intervalPartViewModel.EndMinute = doorDayIntervalPart.EndMinute;
					dayIntervalViewModel.IntervalParts.Add(intervalPartViewModel);
				}
			}
		}

		public ObservableCollection<DayIntervalViewModel> DayIntervals { get; private set; }

		public DoorDayIntervalsCollection GetModel()
		{
			var doorDayIntervalsCollection = new DoorDayIntervalsCollection();
			foreach (var dayInterval in DayIntervals)
			{
				var doorDayInterval = new DoorDayInterval();
				foreach (var interval in dayInterval.IntervalParts)
				{
					var doorDayIntervalPart = new DoorDayIntervalPart();
					doorDayIntervalPart.StartHour = interval.StartHour;
					doorDayIntervalPart.StartMinute = interval.StartMinute;
					doorDayIntervalPart.EndHour = interval.EndHour;
					doorDayIntervalPart.EndMinute = interval.EndMinute;
					doorDayInterval.DoorDayIntervalParts.Add(doorDayIntervalPart);
				}
				doorDayIntervalsCollection.DoorDayIntervals.Add(doorDayInterval);
			}
			return doorDayIntervalsCollection;
		}

		public class DayIntervalViewModel : BaseViewModel
		{
			public DayIntervalViewModel(int dayNo)
			{
				Name = IntToWeekDay(dayNo);
				IntervalParts = new ObservableCollection<IntervalPartViewModel>();
			}

			public string Name { get; private set; }
			public ObservableCollection<IntervalPartViewModel> IntervalParts { get; private set; }

			string IntToWeekDay(int dayNo)
			{
				switch (dayNo)
				{
					case 1:
						return "Понедельник";
					case 2:
						return "Вторник";
					case 3:
						return "Среда";
					case 4:
						return "Четверг";
					case 5:
						return "Пятница";
					case 6:
						return "Суббота";
					case 7:
						return "Воскресенье";
				}
				return "Неизвестный день";
			}
		}

		public class IntervalPartViewModel : BaseViewModel
		{
			public IntervalPartViewModel()
			{
				StartHours = new ObservableCollection<int>();
				StartMinutes = new ObservableCollection<int>();
				EndHours = new ObservableCollection<int>();
				EndMinutes = new ObservableCollection<int>();

				for (int i = 0; i <= 23; i++)
				{
					StartHours.Add(i);
					EndHours.Add(i);
				}
				for (int i = 0; i <= 59; i++)
				{
					StartMinutes.Add(i);
					EndMinutes.Add(i);
				}
			}

			public ObservableCollection<int> StartHours { get; private set; }
			public ObservableCollection<int> StartMinutes { get; private set; }
			public ObservableCollection<int> EndHours { get; private set; }
			public ObservableCollection<int> EndMinutes { get; private set; }

			public int StartHour { get; set; }
			public int StartMinute { get; set; }
			public int EndHour { get; set; }
			public int EndMinute { get; set; }
		}
	}
}