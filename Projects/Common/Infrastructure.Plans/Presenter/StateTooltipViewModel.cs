﻿using Common;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace Infrastructure.Plans.Presenter
{
	public class StateTooltipViewModel<T> : BaseViewModel
		where T : IStateProvider
	{
		public IDeviceState State { get; protected set; }
		public T Item { get; protected set; }

		public StateTooltipViewModel(T item)
		{
			Item = item;
			if (item != null)
			{
				State = item.StateClass;
			}
		}

		public virtual void OnStateChanged()
		{
			OnPropertyChanged(() => Item);
			OnPropertyChanged(() => State);
		}
	}
}
