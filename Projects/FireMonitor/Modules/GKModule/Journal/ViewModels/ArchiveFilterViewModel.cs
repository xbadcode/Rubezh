﻿using System;
using System.Linq;
using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using FiresecAPI;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class ArchiveFilterViewModel : DialogViewModel
    {
        public ArchiveFilterViewModel(XArchiveFilter archiveFilter)
        {
            Title = "Настройки фильтра";
            ClearCommand = new RelayCommand(OnClear);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

            StartDate = archiveFilter.StartDate;
            EndDate = archiveFilter.EndDate;
            StartTime = archiveFilter.StartDate;
            EndTime = archiveFilter.EndDate;

            InitializeJournalItemTypes(archiveFilter);
            InitializeStateClasses(archiveFilter);
            InitializeGKAddresses(archiveFilter);
        }

        void InitializeJournalItemTypes(XArchiveFilter archiveFilter)
        {
            JournalItemTypes = new List<JournalItemTypeViewModel>();
            JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Device));
            JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Direction));
            JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.GK));
            JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.System));
            JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Zone));

            foreach (var journalItemType in archiveFilter.JournalItemTypes)
            {
                var JournalItemTypeViewModel = JournalItemTypes.FirstOrDefault(x => x.JournalItemType == journalItemType);
                if (JournalItemTypeViewModel != null)
                {
                    JournalItemTypeViewModel.IsChecked = true;
                }
            }
        }

        void InitializeStateClasses(XArchiveFilter archiveFilter)
        {
            StateClasses = new List<StateClassViewModel>();
            foreach (XStateClass stateClass in Enum.GetValues(typeof(XStateClass)))
            {
                var stateClassViewModel = new StateClassViewModel(stateClass);
                StateClasses.Add(stateClassViewModel);
            }

            foreach (var stateClass in archiveFilter.StateClasses)
            {
                var stateClassViewModel = StateClasses.FirstOrDefault(x => x.StateClass == stateClass);
                if (stateClassViewModel != null)
                {
                    stateClassViewModel.IsChecked = true;
                }
            }
        }

        void InitializeGKAddresses(XArchiveFilter archiveFilter)
        {
            GKAddresses = new List<GKAddressViewModel>();
            var addresses = GKDBHelper.GetGKIPAddresses();
            foreach (var address in addresses)
            {
                var addressViewModel = new GKAddressViewModel(address);
                GKAddresses.Add(addressViewModel);
            }

            foreach (var address in archiveFilter.GKAddresses)
            {
                var addressViewModel = GKAddresses.FirstOrDefault(x => x.Address == address);
                if (addressViewModel != null)
                {
                    addressViewModel.IsChecked = true;
                }
            }
        }

        public DateTime ArchiveFirstDate
        {
            get { return ArchiveViewModel.ArchiveFirstDate; }
        }

        public DateTime NowDate
        {
            get { return DateTime.Now; }
        }

        DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }
        public DateTime StartTime
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartTime");
            }
        }

        DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }
        public DateTime EndTime
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndTime");
            }
        }

        public List<JournalItemTypeViewModel> JournalItemTypes { get; private set; }
        public List<StateClassViewModel> StateClasses { get; private set; }
        public List<GKAddressViewModel> GKAddresses { get; private set; }

        public XArchiveFilter GetModel()
        {
            var archiveFilter = new XArchiveFilter()
            {
                StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hour, StartTime.Minute, StartTime.Second),
                EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hour, EndTime.Minute, EndTime.Second),
            };
            foreach (var journalItemType in JournalItemTypes)
            {
                if (journalItemType.IsChecked)
                    archiveFilter.JournalItemTypes.Add(journalItemType.JournalItemType);
            }
            foreach (var stateClass in StateClasses)
            {
                if (stateClass.IsChecked)
                    archiveFilter.StateClasses.Add(stateClass.StateClass);
            }
            foreach (var addresses in GKAddresses)
            {
                if (addresses.IsChecked)
                    archiveFilter.GKAddresses.Add(addresses.Address);
            }
            return archiveFilter;
        }

        public RelayCommand ClearCommand { get; private set; }
        void OnClear()
        {
            StartDate = StartTime = EndDate = EndTime = DateTime.Now;
            StartDate = StartDate.AddDays(-1);
        }
        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            if (StartDate > EndDate)
            {
                MessageBoxService.ShowWarning("Начальная дата должна быть меньше конечной");
                return;
            }
            Close(true);
        }
        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }

    public class JournalItemTypeViewModel : BaseViewModel
    {
        public JournalItemTypeViewModel(JournalItemType journalItemType)
        {
            JournalItemType = journalItemType;
            Name = journalItemType.ToDescription();
        }

        public JournalItemType JournalItemType { get; private set; }
        public string Name { get; private set; }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }

    public class StateClassViewModel : BaseViewModel
    {
        public StateClassViewModel(XStateClass stateClass)
        {
            StateClass = stateClass;
            Name = stateClass.ToDescription();
        }

        public XStateClass StateClass { get; private set; }
        public string Name { get; private set; }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }

    public class GKAddressViewModel : BaseViewModel
    {
        public GKAddressViewModel(string address)
        {
            Address = address;
        }

        public string Address { get; private set; }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
}