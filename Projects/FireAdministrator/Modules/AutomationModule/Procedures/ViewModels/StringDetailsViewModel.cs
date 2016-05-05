﻿using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class StringDetailsViewModel : SaveCancelDialogViewModel
	{
		public StringDetailsViewModel(string stringValue)
		{
		    Title = Resources.Language.Procedures.ViewModels.StringDetailsViewModel.Title;
			StringValue = stringValue;
		}

		string _stringValue;
		public string StringValue
		{
			get { return _stringValue; }
			set
			{
				_stringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}
	}
}
