﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;
using RubezhAPI.Journal;
using GKWebService.Controllers;
using RubezhAPI.GK;
using System.Drawing;
using RubezhClient;
using System.Reflection;

namespace GKWebService.Models
{
   public class JournalModel
    {
		public string DeviceDate { get; set; }
		public string SystemDate { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
		public string ObjectName { get; set; }
		public string ObjectImageSource { get; set; }
        public string User { get; set; }
		public Guid ObjectUid { get; set; }
		public string Color { get; set; }
		public JournalObjectType ObjectType { get; set; }
		public string Subsystem { get; set; }
		public string EventImage { get; set; }
		public bool CanShow { get; set; }
		public JournalModel(JournalItem journalItem)
		{
			Desc = journalItem.JournalEventDescriptionType.ToDescription();
			SystemDate = journalItem.SystemDateTime.ToString();
			Name = journalItem.JournalEventNameType.ToDescription();
			DeviceDate = journalItem.DeviceDateTime.ToString();
			User = journalItem.UserName;
			ObjectUid = journalItem.ObjectUID;
			Color = GetStateColor(journalItem);
			ObjectType = journalItem.JournalObjectType;
			ObjectImageSource = "/Content/Image/Images/Blank.png";
			Subsystem = journalItem.JournalSubsystemType.ToDescription();
			EventImage = GetEventImage(journalItem.JournalEventNameType);
			GetObject(journalItem);
		}

		void GetObject(JournalItem journalItem)
		{
			switch (journalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
					var device = GKManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (device != null)
					{
						ObjectName = device.GetGKDescriptorName(GKManager.DeviceConfiguration.GKNameGenerationType);
						ObjectImageSource = device.Driver.ImageSource.Replace("/Controls;component/", "/Content/Image/");
					}
					break;

				case JournalObjectType.GKZone:
					var zone = GKManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (zone != null)
					{
						ObjectName = zone.PresentationName;
					}
					ObjectImageSource = "/Content/Image/Images/Zone.png";
					break;

				case JournalObjectType.GKDirection:
					var direction = GKManager.Directions.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (direction != null)
					{
						ObjectName = direction.PresentationName;
					}
					ObjectImageSource = "/Content/Image/Images/BDirection.png";
					break;

				case JournalObjectType.GKPumpStation:
					var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (pumpStation != null)
					{
						ObjectName = pumpStation.PresentationName;
					}
					ObjectImageSource = "/Content/Image/Images/BPumpStation.png";
					break;

				case JournalObjectType.GKMPT:
					var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (mpt != null)
					{
						ObjectName = mpt.PresentationName;
					}
					ObjectImageSource = "/Content/Image/Images/BMPT.png";
					break;

				case JournalObjectType.GKDelay:
					var delay = GKManager.Delays.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (delay != null)
					{
						ObjectName = delay.PresentationName;
						ObjectImageSource = "/Content/Image/Images/Delay.png";
					}
					else
					{
						delay = GKManager.AutoGeneratedDelays.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
						if (delay != null)
						{
							ObjectName = delay.PresentationName;
							if (delay.PumpStationUID != Guid.Empty)
							{
								var delayPumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == delay.PumpStationUID);
								if (delayPumpStation != null)
								{
									ObjectName += " (" + delayPumpStation.PresentationName + ")";
									ObjectImageSource = "/Content/Image/Images/BPumpStation.png";
									break;
								}
							}
						}
					}
					break;

				case JournalObjectType.GKPim:
					var pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (pim != null)
					{
						ObjectName = pim.PresentationName;
						ObjectImageSource = "/Content/Image/Images/Pim.png";
						if (pim.PumpStationUID != Guid.Empty)
						{
							var pimPumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == pim.PumpStationUID);
							if (pimPumpStation != null)
							{
								ObjectName += " (" + pimPumpStation.PresentationName + ")";
								ObjectImageSource = "/Content/Image/Images/BPumpStation.png";
								break;
							}
						}
						if (pim.MPTUID != Guid.Empty)
						{
							var pimMPT = GKManager.MPTs.FirstOrDefault(x => x.UID == pim.MPTUID);
							if (pimMPT != null)
							{
								ObjectName += " (" + pimMPT.PresentationName + ")";
								ObjectImageSource = "/Content/Image/Images/BMPT.png";
								break;
							}
						}
					}
					break;

				case JournalObjectType.GKGuardZone:
					var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (guardZone != null)
					{
						ObjectName = guardZone.PresentationName;
					}
					ObjectImageSource = "/Content/Image/Images/GuardZone.png";
					break;

				case JournalObjectType.GKSKDZone:
					var gkSKDZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (gkSKDZone != null)
					{
						ObjectName = gkSKDZone.PresentationName;
					}
					ObjectImageSource = "/Content/Image/Images/Zone.png";
					break;

				case JournalObjectType.GKDoor:
					var gkDoor = GKManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (gkDoor != null)
					{
						ObjectName = gkDoor.PresentationName;
					}
					ObjectImageSource = "/Content/Image/Images/Door.png";
					break;

				case JournalObjectType.VideoDevice:
					var camera = RubezhClient.ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (camera != null)
					{
						ObjectName = camera.PresentationName;
					}
					ObjectImageSource = "/Content/Image/Images/BVideo.png";
					break;

				case JournalObjectType.None:
				case JournalObjectType.GKUser:
					ObjectName = journalItem.ObjectName != null ? journalItem.ObjectName : "";
					break;
			}
			CanShow = journalItem.JournalObjectType != JournalObjectType.None && journalItem.JournalObjectType != JournalObjectType.GKUser;
			if (ObjectName == null)
			{
				ObjectName = journalItem.ObjectName;
			}

			if (ObjectName == null)
				ObjectName = "<Нет в конфигурации>";
		}

		string GetStateColor(JournalItem journalItem)
		{
			var stateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
			switch (stateClass)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.ConnectionLost:
				case XStateClass.TechnologicalRegime:
				case XStateClass.HasNoLicense:
					return ColorTranslator.ToHtml(System.Drawing.Color.Gray);

				case XStateClass.Fire2:
				case XStateClass.Fire1:
					return ColorTranslator.ToHtml(System.Drawing.Color.Red);

				case XStateClass.Attention:
					return ColorTranslator.ToHtml(System.Drawing.Color.Orange);

				case XStateClass.Failure:
					return ColorTranslator.ToHtml(System.Drawing.Color.Pink);

				case XStateClass.Service:
					return ColorTranslator.ToHtml(System.Drawing.Color.Yellow);

				case XStateClass.Ignore:
					return ColorTranslator.ToHtml(System.Drawing.Color.Yellow);

				case XStateClass.On:
					return ColorTranslator.ToHtml(System.Drawing.Color.LightBlue);

				case XStateClass.AutoOff:
					return ColorTranslator.ToHtml(System.Drawing.Color.Yellow);

				case XStateClass.Test:
				case XStateClass.Norm:
					return ColorTranslator.ToHtml(System.Drawing.Color.Transparent);

				default:
					return ColorTranslator.ToHtml(System.Drawing.Color.Transparent);
			}
		}

	   string GetEventImage(JournalEventNameType journalEventNameType)
		{
			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				EventNameAttribute[] descriptionAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventNameAttribute eventNameAttribute = descriptionAttributes[0];
					Name = eventNameAttribute.Name;
					var stateClass = eventNameAttribute.StateClass;
					if (stateClass != XStateClass.Norm)
						return "/Content/Image/StateClasses/" + stateClass.ToString() + ".png";
				}
			}
			return "/Content/Image/Images/blank.png";
		}
    }
}