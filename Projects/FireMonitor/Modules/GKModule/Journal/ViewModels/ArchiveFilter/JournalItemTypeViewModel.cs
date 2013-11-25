﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using GKProcessor;
using FiresecAPI;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class JournalItemTypeViewModel : BaseViewModel
	{
		public JournalItemTypeViewModel(JournalItemType journalItemType)
		{
			JournalItemType = journalItemType;
			Name = journalItemType.ToDescription();
		}

		public JournalItemType JournalItemType { get; private set; }
		public string Name { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}