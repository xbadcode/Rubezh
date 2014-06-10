﻿using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ScheduleViewModel : BaseViewModel
	{
		public const string DefaultName = "<нет>";
		public AutomationSchedule Schedule { get; set; }

		public ScheduleViewModel(AutomationSchedule schedule)
		{
			Schedule = schedule;
		}

		static ScheduleViewModel()
		{
			Years = new ObservableCollection<int> { -1 };
			for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 100; i++)
			{
				Years.Add(i);
			}
			Months = new ObservableCollection<int> { -1 };
			for (int i = 1; i <= 12; i++)
			{
				Months.Add(i);				
			}
			Days = new ObservableCollection<int> { -1 };
			for (int i = 1; i <= 31 ; i++)
			{
				Days.Add(i);				
			}
			Hours = new ObservableCollection<int> { -1 };
			for (int i = 0; i <= 24; i++)
			{
				Hours.Add(i);				
			}
			Minutes = new ObservableCollection<int> { -1 };
			for (int i = 0; i <= 59; i++)
			{
				Minutes.Add(i);				
			}
			Seconds = new ObservableCollection<int> { -1 };
			for (int i = 0; i <= 59; i++)
			{
				Seconds.Add(i);				
			}
			DaysOfWeek = new ObservableCollection<DayOfWeekType>
				{
					DayOfWeekType.Any, DayOfWeekType.Monday, DayOfWeekType.Tuesday, DayOfWeekType.Wednesday,
					DayOfWeekType.Thursday, DayOfWeekType.Friday, DayOfWeekType.Saturday, DayOfWeekType.Sunday
				};			
		}

		public string Name
		{
			get { return Schedule.Name; }
			set
			{
				Schedule.Name = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged("Name");
			}
		}

		public static ObservableCollection<int> Years { get; private set; }
		public static ObservableCollection<int> Months { get; private set; }
		public static ObservableCollection<int> Days { get; private set; }
		public static ObservableCollection<int> Hours { get; private set; }
		public static ObservableCollection<int> Minutes { get; private set; }
		public static ObservableCollection<int> Seconds { get; private set; }
		public static ObservableCollection<DayOfWeekType> DaysOfWeek { get; private set; }

		public int SelectedYear
		{
			get { return Schedule.Year; }
			set
			{
				Schedule.Year = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedYear);
			}
		}

		public int SelectedMonth
		{
			get { return Schedule.Month; }
			set
			{
				Schedule.Month = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedMonth);
			}
		}

		public int SelectedDay
		{
			get { return Schedule.Day; }
			set
			{
				Schedule.Day = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedDay);
			}
		}

		public int SelectedHour
		{
			get { return Schedule.Hour; }
			set
			{
				Schedule.Hour = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedHour);
			}
		}

		public int SelectedMinute
		{
			get { return Schedule.Minute; }
			set
			{
				Schedule.Minute = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedMinute);
			}
		}

		public int SelectedSecond
		{
			get { return Schedule.Second; }
			set
			{
				Schedule.Second = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedSecond);
			}
		}

		public DayOfWeekType SelectedDayOfWeek
		{
			get { return Schedule.DayOfWeek; }
			set
			{
				Schedule.DayOfWeek = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedDayOfWeek);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Schedule);
			OnPropertyChanged(() => Name);
		}
	}
}