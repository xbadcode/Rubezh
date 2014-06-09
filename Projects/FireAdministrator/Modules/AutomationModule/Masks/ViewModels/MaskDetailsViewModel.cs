﻿using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class MaskDetailsViewModel : SaveCancelDialogViewModel
	{
		public Mask Mask { get; private set; }
		public MaskDetailsViewModel(Mask mask)
		{
			Title = "Свойства маски";
			Mask = mask;
			Name = mask.Name;
		}

		public MaskDetailsViewModel()
		{
			Title = "Добавить маску";
			Mask = new Mask();
			Name = Mask.Name;
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

		protected override bool Save()
		{
			Mask.Name = Name;
			return base.Save();
		}
	}
}
