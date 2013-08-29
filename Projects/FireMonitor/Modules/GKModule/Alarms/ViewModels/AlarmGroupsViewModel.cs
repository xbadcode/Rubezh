﻿using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common;

namespace GKModule.ViewModels
{
	public class AlarmGroupsViewModel : BaseViewModel
	{
		public static AlarmGroupsViewModel Current { get; private set; }

		public AlarmGroupsViewModel()
		{
			Current = this;
			ResetCommand = new RelayCommand(OnReset);
			AlarmGroups = new List<AlarmGroupViewModel>();
			foreach (XAlarmType alarmType in Enum.GetValues(typeof(XAlarmType)))
			{
				AlarmGroups.Add(new AlarmGroupViewModel(alarmType));
			}
		}

		public List<AlarmGroupViewModel> AlarmGroups { get; private set; }

		public void Update(List<Alarm> alarms)
		{
			foreach (var alarmGroup in AlarmGroups)
			{
				alarmGroup.Alarms = alarms.Where(x => x.AlarmType == alarmGroup.AlarmType).ToList<Alarm>();
				alarmGroup.Update();
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			AlarmsViewModel.Current.ResetAll();
		}
	}
}