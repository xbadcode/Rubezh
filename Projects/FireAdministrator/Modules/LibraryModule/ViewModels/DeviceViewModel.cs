﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        readonly Driver _driver;

        public DeviceViewModel(DeviceLibrary.Models.Device device)
        {
            Device = device;
            _driver = FiresecManager.Drivers.First(x => x.Id == Device.Id);
            if (Device.States == null)
            {
                SetDefaultStateTo(Device);
            }

            RemoveStateCommand = new RelayCommand(
                () => OnRemoveState(),
                (x) => SelectedStateViewModel != null &&
                     SelectedStateViewModel.State.Class != StateViewModel.DefaultClassId);
            AddStateCommand = new RelayCommand(OnAddState);
            AddAdditionalStateCommand = new RelayCommand(OnShowAdditionalStates);

            Initialize();
        }

        void Initialize()
        {
            StateViewModels = new ObservableCollection<StateViewModel>();
            foreach (var state in Device.States)
            {
                StateViewModels.Add(new StateViewModel(state, _driver));
            }
        }

        public DeviceLibrary.Models.Device Device { get; private set; }

        public string Name
        {
            get { return _driver.Name; }
        }

        public string ImageSource
        {
            get { return _driver.ImageSource; }
        }

        public string Id
        {
            get { return Device.Id; }
        }

        public ObservableCollection<StateViewModel> StateViewModels { get; private set; }

        StateViewModel _selectedStateViewModel;
        public StateViewModel SelectedStateViewModel
        {
            get { return _selectedStateViewModel; }
            set
            {
                _selectedStateViewModel = value;
                OnPropertyChanged("SelectedStateViewModel");
                OnPropertyChanged("CanvasesPresenter");
            }
        }

        public CanvasesPresenter CanvasesPresenter
        {
            get
            {
                if (SelectedStateViewModel == null) return null;

                var canvasesPresenter = new CanvasesPresenter(SelectedStateViewModel.State);
                if (!SelectedStateViewModel.IsAdditional)
                {
                    foreach (var stateViewModel in StateViewModels)
                    {
                        if (stateViewModel.IsAdditional &&
                            stateViewModel.IsChecked &&
                            stateViewModel.State.Class == SelectedStateViewModel.State.Class)
                        {
                            canvasesPresenter.AddCanvacesFrom(stateViewModel.State);
                        }
                    }
                }

                return canvasesPresenter;
            }
        }

        public static void SetDefaultStateTo(DeviceLibrary.Models.Device device)
        {
            device.States = new List<DeviceLibrary.Models.State>();
            device.States.Add(StateViewModel.GetDefaultStateWith());
        }

        public static DeviceLibrary.Models.Device GetDefaultDriverWith(string id)
        {
            var device = new DeviceLibrary.Models.Device();
            device.Id = id;
            SetDefaultStateTo(device);

            return device;
        }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddState()
        {
            var addStateViewModel = new StateDetailsViewModel(Device);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addStateViewModel))
            {
                Device.States.Add(addStateViewModel.SelectedItem.State);
                StateViewModels.Add(addStateViewModel.SelectedItem);
            }
        }

        public RelayCommand AddAdditionalStateCommand { get; private set; }
        void OnShowAdditionalStates()
        {
            var addAdditionalStateViewModel = new AdditionalStateDetailsViewModel(Device);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addAdditionalStateViewModel))
            {
                Device.States.Add(addAdditionalStateViewModel.SelectedItem.State);
                StateViewModels.Add(addAdditionalStateViewModel.SelectedItem);
            }
        }

        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveState()
        {
            var dialogResult = MessageBox.Show("Удалить выбранное состояние?",
                                                "Окно подтверждения",
                                                MessageBoxButton.OKCancel,
                                                MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.OK)
            {
                Device.States.Remove(SelectedStateViewModel.State);
                StateViewModels.Remove(SelectedStateViewModel);
            }
        }
    }
}