﻿using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Events;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Linq;

namespace JournalModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public bool IsExistsInConfig { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public string ObjectImageSource { get; private set; }
		public string ObjectName { get; private set; }
		public XStateClass StateClass { get; private set; }

		CompositePresentationEvent<Guid> ShowObjectEvent;
		CompositePresentationEvent<Guid> ShowObjectDetailsEvent;

		public GKDevice Device { get; set; }
		GKZone Zone { get; set; }
		GKDirection Direction { get; set; }
		GKPumpStation PumpStation { get; set; }
		GKMPT MPT { get; set; }
		GKDelay Delay { get; set; }
		GKPim Pim { get; set; }
		GKGuardZone GuardZone { get; set; }
		GKSKDZone GKSKDZone { get; set; }
		GKDoor GKDoor { get; set; }
		Camera Camera { get; set; }

		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowObject);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowVideoCommand = new RelayCommand(OnShowVideo, CanShowVideo);

			JournalItem = journalItem;

			if (journalItem.JournalEventNameType != JournalEventNameType.NULL)
			{
				Name = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventNameType);
			}

			if (journalItem.JournalEventDescriptionType != JournalEventDescriptionType.NULL)
			{
				Description = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventDescriptionType);
				if (!string.IsNullOrEmpty(journalItem.DescriptionText))
					Description += " " + journalItem.DescriptionText;
			}
			else
			{
				Description = journalItem.DescriptionText;
			}

			IsExistsInConfig = true;
			_isVideoExist = journalItem.VideoUID != Guid.Empty;
			ObjectImageSource = JournalItem.GetImageSource(journalItem.JournalObjectType);
			StateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);

			switch (JournalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
					Device = GKManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (Device != null)
					{
						ObjectName = Device.GetGKDescriptorName(GKManager.DeviceConfiguration.GKNameGenerationType);
						ObjectImageSource = Device.Driver.ImageSource;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDeviceDetailsEvent>();
					}
					else
						ObjectImageSource = "/Controls;component/Images/Blank.png";
					break;

				case JournalObjectType.GKZone:
					Zone = GKManager.Zones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (Zone != null)
					{
						ObjectName = Zone.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKZoneEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKZoneDetailsEvent>();
					}
					break;

				case JournalObjectType.GKDirection:
					Direction = GKManager.Directions.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (Direction != null)
					{
						ObjectName = Direction.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKDirectionEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDirectionDetailsEvent>();
					}
					break;

				case JournalObjectType.GKPumpStation:
					PumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (PumpStation != null)
					{
						ObjectName = PumpStation.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKPumpStationEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKPumpStationDetailsEvent>();
					}
					break;

				case JournalObjectType.GKMPT:
					MPT = GKManager.MPTs.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (MPT != null)
					{
						ObjectName = MPT.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKMPTEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKMPTDetailsEvent>();
					}
					break;

				case JournalObjectType.GKDelay:
					Delay = GKManager.Delays.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (Delay != null)
					{
						ObjectName = Delay.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKDelayEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDelayDetailsEvent>();
					}
					else
					{
						Delay = GKManager.AutoGeneratedDelays.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (Delay != null)
						{
							ObjectName = Delay.PresentationName;
							ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKDelayEvent>();
							ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDelayDetailsEvent>();
							if (Delay.PumpStationUID != Guid.Empty)
							{
								PumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == Delay.PumpStationUID);
								if (PumpStation != null)
								{
									ObjectName += " (" + PumpStation.PresentationName + ")";
									ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKPumpStationEvent>();
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKPumpStationDetailsEvent>();
									ObjectImageSource = "/Controls;component/Images/BPumpStation.png";
									break;
								}
							}
						}
					}
					break;

				case JournalObjectType.GKPim:
					Pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (Pim != null)
					{
						ObjectName = Pim.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKPimEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKPIMDetailsEvent>();
						if (Pim.PumpStationUID != Guid.Empty)
						{
							PumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == Pim.PumpStationUID);
							if (PumpStation != null)
							{
								ObjectName += " (" + PumpStation.PresentationName + ")";
								ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKPumpStationEvent>();
								ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKPumpStationDetailsEvent>();
								ObjectImageSource = "/Controls;component/Images/BPumpStation.png";
								break;
							}
						}
						if (Pim.MPTUID != Guid.Empty)
						{
							MPT = GKManager.MPTs.FirstOrDefault(x => x.UID == Pim.MPTUID);
							if (MPT != null)
							{
								ObjectName += " (" + MPT.PresentationName + ")";
								ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKMPTEvent>();
								ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKMPTDetailsEvent>();
								ObjectImageSource = "/Controls;component/Images/BMPT.png";
								break;
							}
						}
					}
					break;

				case JournalObjectType.GKGuardZone:
					GuardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (GuardZone != null)
					{
						ObjectName = GuardZone.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKGuardZoneEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKGuardZoneDetailsEvent>();
					}
					break;

				case JournalObjectType.GKSKDZone:
					GKSKDZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (GKSKDZone != null)
					{
						ObjectName = GKSKDZone.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKSKDZoneEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKSKDZoneDetailsEvent>();
					}
					break;

				case JournalObjectType.GKDoor:
					GKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (GKDoor != null)
					{
						ObjectName = GKDoor.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKDoorEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDoorDetailsEvent>();
					}
					break;

				case JournalObjectType.Camera:
					Camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (Camera != null)
					{
						ObjectName = Camera.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowCameraEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowCameraDetailsEvent>();
					}
					break;

				case JournalObjectType.None:
				case JournalObjectType.GKUser:
					ObjectName = JournalItem.ObjectName != null ? JournalItem.ObjectName : "";
					break;
			}

			if (ObjectName == null)
			{
				ObjectName = JournalItem.ObjectName;
				IsExistsInConfig = false;
			}

			if (ObjectName == null)
				ObjectName = "<Нет в конфигурации>";

			if (JournalItem.JournalSubsystemType == JournalSubsystemType.SKD)
			{
				IsExistsInConfig = true;
				if ((int)JournalItem.JournalObjectType >= 11 && (int)JournalItem.JournalObjectType <= 19)
					ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowJournalHREvent>();

				if ((int)JournalItem.JournalObjectType >= 20 && (int)JournalItem.JournalObjectType <= 26)
					ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowJournalTimeTrackingEvent>();
			}

		}

		bool _isVideoExist;
		public bool IsVideoExist
		{
			get { return _isVideoExist; }
			set
			{
				_isVideoExist = value;
				OnPropertyChanged(() => IsVideoExist);
			}

		}
		public bool IsStateImage
		{
			get { return JournalItem != null && JournalItem.ObjectName != null && JournalItem.ObjectName.EndsWith("АМ-R2"); }
		}

		public bool CanShow
		{
			get { return CanShowObject() || CanShowOnPlan(); }
		}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else if (CanShowObject())
				OnShowObject();
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			if (ShowObjectEvent != null)
				ShowObjectEvent.Publish(JournalItem.ObjectUID);
		}
		bool CanShowObject()
		{
			return IsExistsInConfig && ShowObjectEvent != null;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			if (ShowObjectDetailsEvent != null)
				ShowObjectDetailsEvent.Publish(JournalItem.ObjectUID);
		}
		bool CanShowProperties()
		{
			return IsExistsInConfig && ShowObjectDetailsEvent != null;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			switch (JournalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
					if (Device != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(Device);
					}
					break;
				case JournalObjectType.GKPumpStation:
					if (PumpStation != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(Device);
					}
					break;
				case JournalObjectType.GKZone:
					if (Zone != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(Zone);
					}
					break;
				case JournalObjectType.GKDirection:
					if (Direction != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(Direction);
					}
					break;
				case JournalObjectType.GKMPT:
					if (MPT != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(MPT);
					}
					break;
				case JournalObjectType.GKGuardZone:
					if (GuardZone != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(GuardZone);
					}
					break;
				case JournalObjectType.GKSKDZone:
					if (GKSKDZone != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(GKSKDZone);
					}
					break;
				case JournalObjectType.GKDoor:
					if (GKDoor != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(GKDoor);
					}
					break;

				case JournalObjectType.GKDelay:
					if (Delay != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(Delay);
					}
					break;

				case JournalObjectType.Camera:
					if (Camera != null)
					{
						ShowOnPlanHelper.ShowObjectOnPlan(Camera);
					}
					break;
			}
		}
		bool CanShowOnPlan()
		{
			if (!IsExistsInConfig)
				return false;

			switch (JournalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
					if (Device != null)
					{
						return ShowOnPlanHelper.CanShowOnPlan(Device);
					}
					break;
				case JournalObjectType.GKZone:
					if (Zone != null)
					{
						return ShowOnPlanHelper.CanShowOnPlan(Zone);
					}
					break;
				case JournalObjectType.GKDirection:
					if (Direction != null)
					{
						return ShowOnPlanHelper.CanShowOnPlan(Direction);
					}
					break;
				case JournalObjectType.GKMPT:
					if (MPT != null)
					{
						return ShowOnPlanHelper.CanShowOnPlan(MPT);
					}
					break;
				case JournalObjectType.GKGuardZone:
					if (GuardZone != null)
					{
						return ShowOnPlanHelper.CanShowOnPlan(GuardZone);
					}
					break;
				case JournalObjectType.GKSKDZone:
					if (GKSKDZone != null)
					{
						return ShowOnPlanHelper.CanShowOnPlan(GKSKDZone);
					}
					break;
				case JournalObjectType.GKDoor:
					if (GKDoor != null)
					{
						return ShowOnPlanHelper.CanShowOnPlan(GKDoor);
					}
					break;
				case JournalObjectType.GKDelay:
					if (Delay != null)
					{
						return ShowOnPlanHelper.CanShowOnPlan(Delay);
					}
					break;
				case JournalObjectType.Camera:
					if (Camera != null)
					{
						return ShowOnPlanHelper.CanShowOnPlan(Camera);
					}
					break;
			}
			return false;
		}

		public RelayCommand ShowVideoCommand { get; private set; }
		void OnShowVideo()
		{
			var videoViewModel = new VideoViewModel(JournalItem.VideoUID, JournalItem.CameraUID);
			if (videoViewModel.HasVideo)
			{
				DialogService.ShowModalWindow(videoViewModel);
			}
			else
				MessageBoxService.ShowError(videoViewModel.ErrorInformation);
		}
		bool CanShowVideo()
		{
			return IsVideoExist;
		}
	}
}