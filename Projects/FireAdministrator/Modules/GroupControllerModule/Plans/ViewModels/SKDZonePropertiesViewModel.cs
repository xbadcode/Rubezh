﻿using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class SKDZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		IElementZone IElementZone;
		ElementBase ElementBase { get; set; }

		public SKDZonePropertiesViewModel(IElementZone element)
		{
			IElementZone = element;
			ElementBase = element as ElementBase;
			var position = ElementBase.GetPosition();
			Left = (int)(position.X * _sensivityFactor);
			Top = (int)(position.Y * _sensivityFactor);
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: СКД Зона";
			Zones = new ObservableCollection<GKSKDZone>(GKManager.SKDZones);
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
		public ObservableCollection<GKSKDZone> Zones { get; private set; }

		GKSKDZone _selectedZone;
		public GKSKDZone SelectedZone
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
			var createSKDZoneEventArg = new CreateGKSKDZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKSKDZoneEvent>().Publish(createSKDZoneEventArg);
			if (createSKDZoneEventArg.Zone != null)
				GKPlanExtension.Instance.RewriteItem(IElementZone, createSKDZoneEventArg.Zone);
			if (!createSKDZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKSKDZoneEvent>().Publish(SelectedZone.UID);
			Zones = new ObservableCollection<GKSKDZone>(GKManager.SKDZones);
			OnPropertyChanged(() => Zones);
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			ElementBase.SetPosition(new System.Windows.Point((double)Left / _sensivityFactor, (double)Top / _sensivityFactor));
			GKPlanExtension.Instance.RewriteItem(IElementZone, SelectedZone);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
	}
}