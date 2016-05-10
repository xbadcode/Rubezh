﻿using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class GuardZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		IElementZone IElementZone;
		ElementBaseRectangle ElementBaseRectangle { get; set; }
		public bool CanEditPosition { get; private set; }
		public GuardZonePropertiesViewModel(IElementZone element)
		{
			IElementZone = element;
			ElementBaseRectangle = element as ElementBaseRectangle;
			CanEditPosition = ElementBaseRectangle != null;
			if (CanEditPosition)
			{
				Left = (int)(ElementBaseRectangle.Left * _sensivityFactor);
				Top = (int)(ElementBaseRectangle.Top * _sensivityFactor);
			}
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: Охранная зона";
			Zones = new ObservableCollection<GKGuardZone>(GKManager.GuardZones);
			if (element.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.UID == element.ZoneUID);
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}

		public ObservableCollection<GKGuardZone> Zones { get; private set; }

		GKGuardZone _selectedZone;
		public GKGuardZone SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			var createGuardZoneEventArg = new CreateGKGuardZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKGuardZoneEvent>().Publish(createGuardZoneEventArg);
			if (createGuardZoneEventArg.Zone != null)
				GKPlanExtension.Instance.RewriteItem(IElementZone, createGuardZoneEventArg.Zone);
			if (!createGuardZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKGuardZoneEvent>().Publish(SelectedZone.UID);
			Zones = new ObservableCollection<GKGuardZone>(GKManager.GuardZones);
			OnPropertyChanged(() => Zones);
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}
		protected override bool Save()
		{
			ElementBaseRectangle.Left = (double)Left / _sensivityFactor;
			ElementBaseRectangle.Top = (double)Top / _sensivityFactor;
			GKPlanExtension.Instance.RewriteItem(IElementZone, SelectedZone);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
	}
}