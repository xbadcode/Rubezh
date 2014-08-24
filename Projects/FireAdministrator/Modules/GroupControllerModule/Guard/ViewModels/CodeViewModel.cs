﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class CodeViewModel : BaseViewModel
	{
		public CodeViewModel(XCode code)
		{
			Code = code;
			AddZoneCommand = new RelayCommand(OnAddZone, CanAdd);
			RemoveZoneCommand = new RelayCommand(OnRemoveZone, CanRemove);

			Zones = new ObservableCollection<GuardZoneViewModel>();
			SourceZones = new ObservableCollection<GuardZoneViewModel>();

			foreach (var guardZone in XManager.DeviceConfiguration.GuardZones)
			{
				var zoneViewModel = new GuardZoneViewModel(guardZone);
				if (Code.GuardZoneUIDs.Contains(guardZone.BaseUID))
					Zones.Add(zoneViewModel);
				else
					SourceZones.Add(zoneViewModel);
			}

			SelectedZone = Zones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		XCode _code;
		public XCode Code
		{
			get { return _code; }
			set
			{
				_code = value;
				OnPropertyChanged(() => Code);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Code);
		}

		public ObservableCollection<GuardZoneViewModel> Zones { get; private set; }

		GuardZoneViewModel _selectedZone;
		public GuardZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public ObservableCollection<GuardZoneViewModel> SourceZones { get; private set; }

		GuardZoneViewModel _selectedSourceZone;
		public GuardZoneViewModel SelectedSourceZone
		{
			get { return _selectedSourceZone; }
			set
			{
				_selectedSourceZone = value;
				OnPropertyChanged(() => SelectedSourceZone);
			}
		}
		bool CanAdd()
		{
			return SelectedSourceZone != null;
		}

		public RelayCommand AddZoneCommand { get; private set; }
		void OnAddZone()
		{
			int oldIndex = SourceZones.IndexOf(SelectedSourceZone);

			Code.GuardZoneUIDs.Add(SelectedSourceZone.Zone.BaseUID);
			Zones.Add(SelectedSourceZone);
			SourceZones.Remove(SelectedSourceZone);

			if (SourceZones.Count > 0)
				SelectedSourceZone = SourceZones[System.Math.Min(oldIndex, SourceZones.Count - 1)];

			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand RemoveZoneCommand { get; private set; }
		void OnRemoveZone()
		{
			int oldIndex = Zones.IndexOf(SelectedZone);

			Code.GuardZoneUIDs.Remove(SelectedZone.Zone.BaseUID);
			SourceZones.Add(SelectedZone);
			Zones.Remove(SelectedZone);

			if (Zones.Count > 0)
				SelectedZone = Zones[System.Math.Min(oldIndex, Zones.Count - 1)];

			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanRemove()
		{
			return SelectedZone != null;
		}
	}
}