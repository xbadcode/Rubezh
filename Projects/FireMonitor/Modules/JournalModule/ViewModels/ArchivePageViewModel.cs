﻿using System.Collections.Generic;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class ArchivePageViewModel : BaseViewModel
	{
		IEnumerable<JournalItem> JournalItemsList;

		public ArchivePageViewModel(IEnumerable<JournalItem> journalItems)
		{
			JournalItemsList = journalItems;
		}

		public void Create()
		{
			JournalItems = new List<JournalItemViewModel>();
			if (JournalItemsList != null)
			{
				foreach (var journalItem in JournalItemsList)
				{
					var journalItemViewModel = new JournalItemViewModel(journalItem);
					JournalItems.Add(journalItemViewModel);
				}
			}
			//if (JournalItemsList != null)
			//{
			//	foreach (var journalItem in JournalItemsList)
			//	{
			//		var journalRecordViewModel = new JournalRecordViewModel(journalItem);
			//		JournalRecords.Add(journalRecordViewModel);
			//	}
			//}
		}

		public List<JournalItemViewModel> JournalItems { get; private set; }
	}
}