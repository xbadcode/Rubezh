﻿using Resurs.Reports;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resurs.ViewModels
{
	public class ChangeFlowFilterViewModel : ReportFilterViewModel
	{
		
		public DateTime MinDate { get { return new DateTime(1900, 1, 1); } }
		public DateTime MaxDate { get { return DateTime.Today.AddDays(1).AddSeconds(-1); } }
		public bool IsDatePickerEnabled { get { return SelectedReportPeriod == ReportPeriodType.Arbitrary; } }
		public ChangeFlowFilterViewModel()
		{
			Title = "Настройки отчета расхода счетчика";
			Devices = DbCache.GetAllChildren(DbCache.RootDevice).Where(x => x.DeviceType == DeviceType.Counter).ToList();
			SelectedDevice = Devices.FirstOrDefault();
			StartDate = DateTime.Today;
			EndDate = DateTime.Today.AddDays(1).AddSeconds(-1);
			SelectedReportPeriod = ReportPeriodType.Arbitrary;
		}
		bool _isNotShowCounters;
		public bool IsNotShowCounters 
		{
			get { return _isNotShowCounters; }
			set
			{
				_isNotShowCounters = value;
				OnPropertyChanged(() => IsNotShowCounters);
			}
		}
		DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				Filter.StartDate = _startDate;
				OnPropertyChanged(() => StartDate);
			}
		}
		DateTime _endDate;
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = value;
				if (SelectedReportPeriod == ReportPeriodType.Arbitrary)
					Filter.EndDate = _endDate.AddDays(1).AddSeconds(-1);
				else
					Filter.EndDate = _endDate;
				OnPropertyChanged(() => EndDate);
			}
		}
		public List<Device> Devices { get; private set; }
		Device _selectedDevice;
		public Device SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				Filter.Device = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}
		ReportPeriodType _selectedReportPeriod;
		public ReportPeriodType SelectedReportPeriod
		{
			get { return _selectedReportPeriod; }
			set
			{
				_selectedReportPeriod = value;
				OnPropertyChanged(() => SelectedReportPeriod);
				OnPropertyChanged(() => IsDatePickerEnabled);
				var today = DateTime.Today;
				switch (_selectedReportPeriod)
				{
					case ReportPeriodType.Day:
						StartDate = today.AddDays(-1);
						EndDate = today.AddSeconds(-1);
						break;
					case ReportPeriodType.Week:
						StartDate = today.AddDays(1 - (int)today.DayOfWeek).AddDays(-7);
						EndDate = StartDate.AddDays(7).AddSeconds(-1);
						break;
					case ReportPeriodType.Month:
						StartDate = new DateTime(today.Year, today.Month - 1, 1);
						EndDate = StartDate.AddDays(DateTime.DaysInMonth(StartDate.Year, StartDate.Month)).AddSeconds(-1);
						break;
				}
			}
		}
	}
}