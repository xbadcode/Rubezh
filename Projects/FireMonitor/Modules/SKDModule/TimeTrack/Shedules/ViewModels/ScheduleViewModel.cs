﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleViewModel : OrganisationElementViewModel<ScheduleViewModel, Schedule>, IEditingViewModel, IDoorsParent
	{
		private bool _isInitialized;
		
		public ScheduleViewModel() { }
		
		public override void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			base.InitializeOrganisation(organisation, parentViewModel);
			_isInitialized = false;
		}
		
		public override void InitializeModel(Organisation organisation, Schedule model, ViewPartViewModel parentViewModel)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			base.InitializeModel(organisation, model, parentViewModel);
			_isInitialized = false;
			Update();
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (!IsOrganisation)
				{
					ScheduleZones = new SortableObservableCollection<ScheduleZoneViewModel>();
					foreach (var employeeScheduleZone in Model.Zones)
					{
						var scheduleZoneViewModel = new ScheduleZoneViewModel(employeeScheduleZone);
						ScheduleZones.Add(scheduleZoneViewModel);
					}
					ScheduleZones.Sort(x => x.No);
					SelectedScheduleZone = ScheduleZones.FirstOrDefault();
				}
			}
		}
		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Scheme);
		}

		public string Scheme
		{
			get
			{
				if (!IsOrganisation)
				{
					var schemes = ScheduleSchemeHelper.Get(new ScheduleSchemeFilter()
					{
						OrganisationUIDs = new List<Guid>() { Organisation.UID },
						Type = ScheduleSchemeType.Month | ScheduleSchemeType.SlideDay | ScheduleSchemeType.Week,
						WithDays = false,
					});
					var scheme = schemes.FirstOrDefault(item => item.UID == Model.ScheduleSchemeUID);
					if (scheme != null)
					{
						return scheme.Name + " (" + scheme.Type.ToDescription() + ")";
					}
				}
				return null;
			}
		}

		public SortableObservableCollection<ScheduleZoneViewModel> ScheduleZones { get; private set; }

		private ScheduleZoneViewModel _selectedScheduleZone;
		public ScheduleZoneViewModel SelectedScheduleZone
		{
			get { return _selectedScheduleZone; }
			set
			{
				_selectedScheduleZone = value;
				OnPropertyChanged(() => SelectedScheduleZone);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var scheduleZoneDetailsViewModel = new ScheduleZoneDetailsViewModel(Model, Organisation);
			if (DialogService.ShowModalWindow(scheduleZoneDetailsViewModel) && AddSave(scheduleZoneDetailsViewModel.ScheduleZone))
			{
				var scheduleZone = scheduleZoneDetailsViewModel.ScheduleZone;
				var scheduleZoneViewModel = new ScheduleZoneViewModel(scheduleZone);
				ScheduleZones.Add(scheduleZoneViewModel);
				Sort();
				SelectedScheduleZone = scheduleZoneViewModel;
			}
		}
		bool CanAdd()
		{
			return !IsDeleted && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Schedules_Edit);
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			if (DeleteSave(SelectedScheduleZone.Model))
			{
				Model.Zones.Remove(SelectedScheduleZone.Model);
				ScheduleZones.Remove(SelectedScheduleZone);
			}
		}
		bool CanDelete()
		{
			return SelectedScheduleZone != null && ScheduleZones.Count > 1 && !IsDeleted && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Schedules_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var scheduleZoneDetailsViewModel = new ScheduleZoneDetailsViewModel(Model, Organisation, SelectedScheduleZone.Model);
			if (DialogService.ShowModalWindow(scheduleZoneDetailsViewModel) && EditSave(SelectedScheduleZone.Model))
			{
				var selectedScheduleZone = SelectedScheduleZone;
				SelectedScheduleZone.Update();
				Sort();
				SelectedScheduleZone = selectedScheduleZone;
			}
		}
		bool CanEdit()
		{
			return SelectedScheduleZone != null && !IsDeleted && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Schedules_Edit);
		}

		public void UpdateCardDoors(IEnumerable<Guid> doorUIDs)
		{
			if (ScheduleZones == null)
				return;
			var zonesToRemove = ScheduleZones.Where(x => !doorUIDs.Any(y => y == x.Model.DoorUID)).ToList();
			if (DeleteManySave(zonesToRemove.Select(x => x.Model)))
			{
				foreach (var item in zonesToRemove)
				{
					ScheduleZones.Remove(item);
				}
			}
			
		}

		public bool AddSave(ScheduleZone zone)
		{
			Model.Zones.Add(zone);
			return ScheduleHelper.Save(Model, false);
		}

		public bool EditSave(ScheduleZone zone)
		{
			Model.Zones.RemoveAll(x => x.UID == zone.UID);
			Model.Zones.Add(zone);
			return ScheduleHelper.Save(Model, false);
		}

		public bool DeleteSave(ScheduleZone zone)
		{
			Model.Zones.RemoveAll(x => x.UID == zone.UID);
			return ScheduleHelper.Save(Model, false);
		}

		public bool DeleteManySave(IEnumerable<ScheduleZone> zones)
		{
			Model.Zones.RemoveAll(x => zones.Any(y => y.UID == x.UID));
			return ScheduleHelper.Save(Model, false);
		}
	}
}