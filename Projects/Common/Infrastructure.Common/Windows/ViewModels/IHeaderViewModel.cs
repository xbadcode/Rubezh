﻿namespace Infrastructure.Common.Windows.ViewModels
{
	public interface IHeaderViewModel
	{
		HeaderedWindowViewModel Content { get; set; }

		bool ShowIconAndTitle { get; set; }
	}
}