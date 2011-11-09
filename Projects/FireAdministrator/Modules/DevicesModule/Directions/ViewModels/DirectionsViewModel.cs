﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DirectionsViewModel : RegionViewModel
    {
        public DirectionsViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanEditOrDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditOrDelete);
            AddCommand = new RelayCommand(OnAdd);
        }

        public void Initialize()
        {
            Directions = new ObservableCollection<DirectionViewModel>(
                from direction in FiresecManager.DeviceConfiguration.Directions
                select new DirectionViewModel(direction));

            if (Directions.Count > 0)
                SelectedDirection = Directions[0];
        }

        public ObservableCollection<DirectionViewModel> Directions { get; private set; }

        DirectionViewModel _selectedDirection;
        public DirectionViewModel SelectedDirection
        {
            get { return _selectedDirection; }
            set
            {
                _selectedDirection = value;
                OnPropertyChanged("SelectedDirection");
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.DeviceConfiguration.Directions.Remove(SelectedDirection.Direction);
            Directions.Remove(SelectedDirection);

            DevicesModule.HasChanges = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var directionDetailsViewModel = new DirectionDetailsViewModel(SelectedDirection.Direction);
            if (ServiceFactory.UserDialogs.ShowModalWindow(directionDetailsViewModel))
            {
                SelectedDirection.Update();

                DevicesModule.HasChanges = true;
            }
        }

        bool CanEditOrDelete()
        {
            return SelectedDirection != null;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var directionDetailsViewModel = new DirectionDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(directionDetailsViewModel))
            {
                FiresecManager.DeviceConfiguration.Directions.Add(directionDetailsViewModel.Direction);
                Directions.Add(new DirectionViewModel(directionDetailsViewModel.Direction));

                DevicesModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new DirectionsMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}