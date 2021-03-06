﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemeDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ScheduleScheme>
	{
		RubezhAPI.SKD.Organisation Organisation;
		ScheduleSchemesViewModel _parentViewModel;
		public ObservableCollection<ScheduleSchemeType> ScheduleSchemeTypes { get; set; }
		ScheduleSchemeType _selectedScheduleSchemeType;
		public ScheduleSchemeType SelectedScheduleSchemeType
		{
			get { return _selectedScheduleSchemeType; }
			set
			{
				_selectedScheduleSchemeType = value;
				OnPropertyChanged(() => SelectedScheduleSchemeType);
				OnPropertyChanged(() => CanSelectDayInterval);
			}
		}
		public bool IsNew { get; private set; }
		public ScheduleScheme Model { get; private set; }
		

		public ScheduleSchemeDetailsViewModel()	{ }

		public bool Initialize(Organisation organisation, ScheduleScheme model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			_parentViewModel = parentViewModel as ScheduleSchemesViewModel;
			ScheduleSchemeTypes = new ObservableCollection<ScheduleSchemeType>();
			foreach (ScheduleSchemeType scheduleSchemeType in Enum.GetValues(typeof(ScheduleSchemeType)))
			{
				ScheduleSchemeTypes.Add(scheduleSchemeType);
			}
			IsNew = model == null;
			var name = string.Empty;
			if (IsNew)
			{
				Name = "Новый график работы";
				Description = "";
				Title = "Новый график работы";
				SelectedScheduleSchemeType = ScheduleSchemeType.Week;
				Model = new ScheduleScheme()
				{
					OrganisationUID = Organisation.UID
				};
				var v = _parentViewModel.GetDayIntervals(Organisation.UID).FirstOrDefault(x => x.Name == "Выходной");
				Model.DayIntervals.Add(new ScheduleDayInterval(){Number =1, ScheduleSchemeUID=Guid.NewGuid(), DayIntervalName = v.Name, DayIntervalUID = v.UID});
			}
			else
			{
				Title = "Редактирование графика работы";
				Model = ScheduleSchemeHelper.GetSingle(model.UID);
				Name = Model.Name;
				Description = Model.Description;
			}
			return true;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public ObservableCollection<DayInterval> DayIntervals
		{
			get 
			{
				if (_parentViewModel != null)
					return _parentViewModel.GetDayIntervals(Organisation.UID);
				return new ObservableCollection<DayInterval>();
			}
		}

		DayInterval _selectedDayInterval;
		public DayInterval SelectedDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				_selectedDayInterval = value;
				OnPropertyChanged(() => SelectedDayInterval);
			}
		}
		
		public bool CanSelectDayInterval
		{
			get { return IsNew && SelectedScheduleSchemeType == ScheduleSchemeType.Week; }
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}
		protected override bool Save()
		{
			Model.Name = Name;
			Model.Description = Description;
			if (IsNew)
			{
				Model.DayIntervals = new List<ScheduleDayInterval>();
				switch (SelectedScheduleSchemeType)
				{
					case ScheduleSchemeType.Month:
							Model.DaysCount = 31;
						break;
					case ScheduleSchemeType.SlideDay:
						Model.DaysCount = 1;
						break;
					default:
						Model.DaysCount = 7;
						break;
				}
                var dayIntervals = _parentViewModel.GetDayIntervals(Organisation.UID);
				for (int i = 0; i < Model.DaysCount; i++)
				{
					var scheduleDayInterval = new ScheduleDayInterval()
					{
						Number = i,
						ScheduleSchemeUID = Model.UID,
					};
					var dayInterval = SelectedDayInterval != null && CanSelectDayInterval && Model.DaysCount - i > 2 ? SelectedDayInterval : DayIntervals.FirstOrDefault(x => x.Name == "Выходной");
					scheduleDayInterval.DayIntervalUID = dayInterval.UID;
					scheduleDayInterval.DayIntervalName = dayInterval.Name;
					Model.DayIntervals.Add(scheduleDayInterval);
				}
				Model.Type = SelectedScheduleSchemeType;
			}
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			return ScheduleSchemeHelper.Save(Model, IsNew);
		}
	}
}