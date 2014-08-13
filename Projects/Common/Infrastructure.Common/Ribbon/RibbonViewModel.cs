﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Ribbon
{
	public class RibbonViewModel : BaseViewModel
	{
		public event EventHandler PopupOpened;

		private RibbonMenuViewModel _content;
		public RibbonMenuViewModel Content
		{
			get { return _content; }
			set
			{
				_content = value;
				OnPropertyChanged(() => Content);
			}
		}

		private bool _isOpened;
		public bool IsOpened
		{
			get { return _isOpened; }
			set
			{
				_isOpened = value;
				OnPropertyChanged(() => IsOpened);
				if (IsOpened && PopupOpened != null)
					PopupOpened(this, EventArgs.Empty);
			}
		}
		
	}
}