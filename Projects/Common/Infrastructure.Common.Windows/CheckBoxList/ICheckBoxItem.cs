﻿namespace Infrastructure.Common.CheckBoxList
{
	public interface ICheckBoxItem
	{
		bool IsChecked { get; set; }
		ICheckBoxItemList ItemsList { get; set; }
		void SetFromParent(bool value);
	}
}