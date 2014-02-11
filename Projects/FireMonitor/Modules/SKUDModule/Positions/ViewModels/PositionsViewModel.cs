﻿using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;

namespace SKDModule.ViewModels
{
	public class PositionsViewModel : ViewPartViewModel
	{
		public PositionsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			Positions = new ObservableCollection<PositionViewModel>();
			var positions = FiresecManager.GetPositions(null);
			foreach (var position in positions)
			{
				var positionViewModel = new PositionViewModel(position);
				Positions.Add(positionViewModel);
			}
			SelectedPosition = Positions.FirstOrDefault();
		}

		ObservableCollection<PositionViewModel> _positions;
		public ObservableCollection<PositionViewModel> Positions
		{
			get { return _positions; }
			set
			{
				_positions = value;
				OnPropertyChanged("Positions");
			}
		}

		PositionViewModel _selectedPosition;
		public PositionViewModel SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				OnPropertyChanged("SelectedPosition");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel();
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				var position = positionDetailsViewModel.Position;
				var positionViewModel = new PositionViewModel(position);
				Positions.Add(positionViewModel);
				SelectedPosition = positionViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = Positions.IndexOf(SelectedPosition);
			Positions.Remove(SelectedPosition);
			index = Math.Min(index, Positions.Count - 1);
			if (index > -1)
				SelectedPosition = Positions[index];
		}
		bool CanRemove()
		{
			return SelectedPosition != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(SelectedPosition.Position);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				SelectedPosition.Update(positionDetailsViewModel.Position);
			}
		}
		bool CanEdit()
		{
			return SelectedPosition != null;
		}
	}
}