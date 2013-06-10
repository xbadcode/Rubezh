﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.ConfigurationWriter;
using System.Diagnostics;
using FiresecAPI;
using Common;

namespace ServerFS2
{
	public class JournalParser
	{
		public FSInternalJournal FSInternalJournal { get; private set; }
		public FS2JournalItem FS2JournalItem { get; private set; }
		public List<byte> Bytes { get; private set; }

		public FS2JournalItem Parce(List<byte> bytes)
		{
			FSInternalJournal = new FSInternalJournal();
			FS2JournalItem = new FS2JournalItem();

			string bytesString = "";
			foreach (var byteItem in bytes)
			{
				bytesString += byteItem.ToString("X2") + " ";
			}
			FS2JournalItem.BytesString = bytesString;


			if (!IsValidInput(bytes))
				return null;

			FSInternalJournal.PanelAddress = bytes[5];
			FS2JournalItem.PanelAddress = bytes[5];
			FSInternalJournal.ShleifNo = bytes[24] + 1;

			bytes.RemoveRange(0, 7);
			Bytes = bytes;

			FSInternalJournal.EventCode = bytes[0];
			FSInternalJournal.AdditionalEventCode = bytes[5];

			FSInternalJournal.DeviceType = bytes[7];
			FSInternalJournal.AddressOnShleif = bytes[8];
			FSInternalJournal.State = bytes[9];

			FSInternalJournal.ZoneNo = BytesHelper.ExtractShort(bytes, 10);
			FSInternalJournal.DescriptorNo = BytesHelper.ExtractTriple(bytes, 12);

			var timeBytes = bytes.GetRange(1, 4);
			FS2JournalItem.DeviceTime = TimeParceHelper.ParceDateTime(timeBytes);
			FS2JournalItem.SystemTime = DateTime.Now;
			FS2JournalItem.EventCode = FSInternalJournal.EventCode;
			FS2JournalItem.EventChoiceNo = FSInternalJournal.AdditionalEventCode;
			FS2JournalItem.Description = GetEventName();
			FS2JournalItem.StateType = GetEventStateType();

			FS2JournalItem.PanelDevice = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.IntAddress == FS2JournalItem.PanelAddress && x.Driver.IsPanel);
			if (FS2JournalItem.PanelDevice != null)
			{
				FS2JournalItem.PanelUID = FS2JournalItem.PanelDevice.UID;
				FS2JournalItem.PanelName = FS2JournalItem.PanelDevice.DottedPresentationNameAndAddress;

				var intAddress = FSInternalJournal.AddressOnShleif + 256 * FSInternalJournal.ShleifNo;
				FS2JournalItem.DeviceAddress = FSInternalJournal.AddressOnShleif;
				FS2JournalItem.Device = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.IntAddress == intAddress && x.ParentPanel == FS2JournalItem.PanelDevice);
				if (FS2JournalItem.Device != null)
				{
					FS2JournalItem.DeviceUID = FS2JournalItem.Device.UID;
					FS2JournalItem.DeviceName = FS2JournalItem.Device.DottedPresentationNameAndAddress;
				}

				FS2JournalItem.SubsystemType = GetSubsystemType(FS2JournalItem.PanelDevice);
			}
			if (FSInternalJournal.DeviceType == 1)
				FS2JournalItem.DeviceName = "АСПТ " + (FSInternalJournal.ShleifNo - 1) + ".";

			InitializeDetalization();
			InitializeZone();
			InitializeGuardEvents();

			FS2JournalItem.EventClass = GetIntEventClass();

			FS2JournalItem.UserName = "Usr";
			return FS2JournalItem;
		}

		bool IsValidInput(List<byte> bytes)
		{
			return true;
			return //bytes.Count == 39 &&
			bytes[6] == 0x41 &&
			bytes[8] == 0xC4;
		}

		void InitializeDetalization()
		{
			if (FSInternalJournal.ShleifNo == 0x83)
				FS2JournalItem.Detalization += "Выход: " + FSInternalJournal.ShleifNo + "\n";
			if (FSInternalJournal.ShleifNo == 0x0F)
				FS2JournalItem.Detalization += "АЛС: " + FSInternalJournal.ShleifNo + "\n";

			if (FSInternalJournal.EventCode == 0x0D && FS2JournalItem.StateByte == 0x20)
			{
				FS2JournalItem.Detalization += "база (сигнатура) повреждена или отсутствует\n";
			}

			FS2JournalItem.Detalization += GetDetalizationForConnectionLost();
			FS2JournalItem.Detalization += GetEventDetalization();
		}

		string GetEventDetalization()
		{
			try
			{
				string result = "";
				if (FS2JournalItem.DeviceUID != Guid.Empty && FSInternalJournal.DeviceType != 0)
				{
					var stringTableType = MetadataHelper.GetDeviceTableNo(FS2JournalItem.Device);
					if (stringTableType != null)
					{
						var metadataEvent = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == "$" + FSInternalJournal.EventCode.ToString("X2"));
						if (metadataEvent.detailsFor != null)
						{
							var metadataDetailsFor = metadataEvent.detailsFor.FirstOrDefault(x => x.tableType == stringTableType);
							if (metadataDetailsFor != null)
							{
								var metadataDictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == metadataDetailsFor.dictionary);
								var bitState = new BitArray(new int[] { FSInternalJournal.State });
								foreach (var bit in metadataDictionary.bit)
									if (bitState.Get(Convert.ToInt32(bit.no)))
										result += metadataDictionary.bit.FirstOrDefault(x => x.no == bit.no).value + "\n";
							}
						}
					}
				}
				return result;
			}
			catch
			{
				return "Детализация не прочитана";
			}
		}

		static SubsystemType GetSubsystemType(Device panelDevice)
		{
			switch (panelDevice.Driver.DriverType)
			{
				case DriverType.Rubezh_2OP:
				case DriverType.USB_Rubezh_2OP:
					return SubsystemType.Guard;

				case DriverType.Rubezh_10AM:
				case DriverType.Rubezh_2AM:
				case DriverType.Rubezh_4A:
				case DriverType.USB_Rubezh_2AM:
				case DriverType.USB_Rubezh_4A:
					return SubsystemType.Fire;

				default:
					return SubsystemType.Other;
			}
		}

		string GetDetalizationForConnectionLost()
		{
			if (FSInternalJournal.EventCode == 0x85)
			{
				switch (FSInternalJournal.DeviceType)
				{
					case 3:
						return "Прибор: Рубеж-БИ Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 7:
						return "Прибор: Рубеж-ПДУ Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 100:
						return "Устройство: МС-3 Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 101:
						return "Устройство: МС-4 Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 102:
						return "Устройство: УОО-ТЛ Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					default:
						return "Неизв. устр." + "(" + FSInternalJournal.DeviceType.ToString() + ") Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
				}
			}
			return "";
		}

		void InitializeGuardEvents()
		{
			if (FSInternalJournal.EventCode == 0x28)
			{
				switch (Bytes[17])
				{
					case 0:
						{
							FS2JournalItem.Detalization += "команда с компьютера\n";
							if (Bytes[16] == 0)
								FS2JournalItem.Detalization += "через USB\n";
							else
								FS2JournalItem.Detalization += "через канал МС " + Bytes[16] + "\n";
							break;
						}
					case 3:
						FS2JournalItem.Detalization += "Прибор: Рубеж-БИ Адрес:" + Bytes[16] + "\n";
						break;
					case 7:
						FS2JournalItem.Detalization += "Прибор: Рубеж-ПДУ Адрес:" + Bytes[16] + "\n";
						break;
					case 9:
						FS2JournalItem.Detalization += "Прибор: Рубеж-ПДУ-ПТ Адрес:" + Bytes[16] + "\n";
						break;
					case 100:
						FS2JournalItem.Detalization += "Устройство: МС-3 Адрес:" + Bytes[16] + "\n";
						break;
					case 101:
						FS2JournalItem.Detalization += "Устройство: МС-4 Адрес:" + Bytes[16] + "\n";
						break;
					case 102:
						FS2JournalItem.Detalization += "Устройство: УОО-ТЛ Адрес:" + Bytes[16] + "\n";
						break;
					default:
						FS2JournalItem.Detalization += "Неизв. устр." + "(" + Bytes[17] + ") Адрес:" + Bytes[16] + "\n";
						break;
				}
				if (FS2JournalItem.DeviceCategory == 0x00)
					FS2JournalItem.DeviceCategory = 0x75;
			}
		}

		void InitializeZone()
		{
			var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == FSInternalJournal.ZoneNo);
			if (zone != null)
			{
				FS2JournalItem.ZoneName = zone.No + "." + zone.Name;
				FS2JournalItem.ZoneNo = zone.No;
			}
		}

		string GetEventName()
		{
			var eventName = MetadataHelper.GetEventMessage(FSInternalJournal.EventCode);
			var firstIndex = eventName.IndexOf("[");
			var lastIndex = eventName.IndexOf("]");
			if (firstIndex != -1 && lastIndex != -1)
			{
				var firstPart = eventName.Substring(0, firstIndex);
				var secondPart = eventName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
				var thirdPart = eventName.Substring(lastIndex + 1);
				var secondParts = secondPart.Split('/');
				if (FSInternalJournal.AdditionalEventCode < secondParts.Count())
				{
					var choise = secondParts[FSInternalJournal.AdditionalEventCode];
					return firstPart + choise + thirdPart;
				}
			}
			return eventName;
		}

		StateType GetEventStateType()
		{
			try
			{
				var stringStateType = MetadataHelper.GetEventStateClassString(FSInternalJournal.EventCode, FSInternalJournal.AdditionalEventCode);
				var intStateType = Int32.Parse(stringStateType);
				return (StateType)intStateType;
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalParser.GetEventStateType");
				return StateType.Norm;

			}
		}

		int GetIntEventClass()
		{
			var stringCode = "$" + FSInternalJournal.EventCode.ToString("X2");
			var metadataEvent = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == stringCode);
			if (metadataEvent == null)
				return -1;

			if (metadataEvent.detailsFor != null)
			{
			}

			var stringEventClass = "";
			if (metadataEvent.eventClassAll != null)
			{
				stringEventClass = metadataEvent.eventClassAll;
			}
			else
			{
				stringEventClass = MetadataHelper.GetAdditionalEventClass(metadataEvent, FSInternalJournal.AdditionalEventCode);
			}

			if (!string.IsNullOrEmpty(stringEventClass))
			{
				try
				{
					return int.Parse(stringEventClass);
				}
				catch { }
			}
			return -1;
		}

		public static FS2JournalItem CustomJournalItem(Device panel, string description)
		{
			return new FS2JournalItem
			{
				DeviceTime = DateTime.Now,
				SystemTime = DateTime.Now,
				Description = description,
				PanelName = panel.DottedPresentationNameAndAddress,
				PanelUID = panel.UID,
				SubsystemType = GetSubsystemType(panel),
			};
		}
	}
}