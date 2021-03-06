﻿using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		public GKDirection Direction { get; private set; }

		public DirectionViewModel(GKDirection direction)
		{
			ShowLogicCommand = new RelayCommand(OnShowLogic);
			Direction = direction;
			Direction.Changed += Update;
			Direction.PlanElementUIDsChanged += UpdateVisualizationState;
			Update();
		}

		public void Update()
		{
			UpdateVisualizationState();
			OnPropertyChanged(() => Direction);
			OnPropertyChanged(() => PresentationLogic);
		}
		void UpdateVisualizationState()
		{
			VisualizationState = Direction.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Direction.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
		}

		public string PresentationLogic
		{
			get
			{
				var presentationLogic = GKManager.GetPresentationLogic(Direction.Logic);
				IsLogicGrayed = string.IsNullOrEmpty(presentationLogic);
				if (string.IsNullOrEmpty(presentationLogic))
				{
					presentationLogic = "Нажмите для настройки логики";
				}
				return presentationLogic;
			}
		}

		bool _isLogicGrayed;
		public bool IsLogicGrayed
		{
			get { return _isLogicGrayed; }
			set
			{
				_isLogicGrayed = value;
				OnPropertyChanged(() => IsLogicGrayed);
			}
		}

		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
			DirectionsViewModel.Current.SelectedDirection = this;
			var logicViewModel = new LogicViewModel(Direction, Direction.Logic, true, hasStopClause: true);
			if (ServiceFactory.DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetDirectionLogic(Direction, logicViewModel.GetModel());
				OnPropertyChanged(() => PresentationLogic);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
			private set
			{
				_visualizationState = value;
				OnPropertyChanged(() => VisualizationState);
			}
		}
	}
}