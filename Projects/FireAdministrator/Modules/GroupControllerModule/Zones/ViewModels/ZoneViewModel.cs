﻿using RubezhAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public GKZone Zone { get; private set; }

		public ZoneViewModel(GKZone zone)
		{
			Zone = zone;
			Update();
		}

		VisualizationState _visualizetionState;
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public bool IsOnPlan
		{
			get { return Zone.PlanElementUIDs != null && Zone.PlanElementUIDs.Count > 0; }
		}

		public void Update()
		{
			OnPropertyChanged(() => Zone);
			if (Zone.PlanElementUIDs == null)
				return;
			_visualizetionState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}
	}
}