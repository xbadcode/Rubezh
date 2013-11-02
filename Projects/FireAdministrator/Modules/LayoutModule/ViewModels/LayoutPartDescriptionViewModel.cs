﻿using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.TreeList;

namespace LayoutModule.ViewModels
{
	public class LayoutPartDescriptionViewModel : TreeNodeViewModel<LayoutPartDescriptionViewModel>
	{
		public ILayoutPartDescription LayoutPartDescription { get; private set; }
		public LayoutPartDescriptionViewModel(ILayoutPartDescription layoutPartDescription)
		{
			LayoutPartDescription = layoutPartDescription;
			_isPresented = true;
		}

		public string Name
		{
			get { return LayoutPartDescription.Name; }
		}
		public string Description
		{
			get { return LayoutPartDescription.Description; }
		}
		public string ImageSource
		{
			get { return LayoutPartDescription.ImageSource; }
		}
		private bool _isPresented;
		public bool IsPresented
		{
			get { return _isPresented; }
			set
			{
				_isPresented = value;
				OnPropertyChanged(() => IsPresented);
			}
		}
	}
}