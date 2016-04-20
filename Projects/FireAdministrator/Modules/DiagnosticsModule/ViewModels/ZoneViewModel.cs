﻿using RubezhAPI.Models;
using Infrastructure.Common.TreeList;
using RubezhAPI.GK;
using RubezhAPI.Hierarchy;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class ZoneViewModel : ItemBaseViewModel<GKZone>
	{
		public ZoneViewModel()
		{
		}

		public ZoneViewModel(GKZone zone)
			: base(zone)
		{
		}

		public void Update()
		{
			OnPropertyChanged(() => Item);
			OnPropertyChanged(() => Description);
		}

		public string Description
		{
			get { return Item.Description; }
		}
	}
}