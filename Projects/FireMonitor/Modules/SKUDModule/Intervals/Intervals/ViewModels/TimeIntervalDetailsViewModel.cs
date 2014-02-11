﻿using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public EmployeeTimeInterval TimeInterval { get; private set; }

		public TimeIntervalDetailsViewModel(EmployeeTimeInterval timeInterval = null)
		{
			if (timeInterval == null)
			{
				Title = "Новый именованный интервал";
				timeInterval = new EmployeeTimeInterval()
				{
					Name = "Именованный интервал"
				};
			}
			else
			{
				Title = "Редактирование именованного интервала";
			}
			TimeInterval = timeInterval;
			Name = TimeInterval.Name;
			Description = TimeInterval.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Всегда" && Name != "Никогда";
		}

		protected override bool Save()
		{
			TimeInterval.Name = Name;
			TimeInterval.Description = Description;
			return true;
		}
	}
}