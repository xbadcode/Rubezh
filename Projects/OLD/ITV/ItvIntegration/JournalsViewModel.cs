﻿using System;
using System.Collections.Generic;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient.Itv;
using System.Collections.ObjectModel;
using System.Windows;

namespace ItvIntegration
{
    public class JournalsViewModel : BaseViewModel
    {
        public JournalsViewModel()
        {
			JournalRecords = new ObservableCollection<JournalRecord>();
			ItvManager.NewJournalRecord += new Action<JournalRecord>((x) => { SafeCall(() => { OnNewJournalRecord(x); }); });
        }

		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		void OnNewJournalRecord(JournalRecord journalRecord)
		{
			JournalRecords.Add(journalRecord);
		}

        public ObservableCollection<JournalRecord> JournalRecords { get; private set; }
    }
}