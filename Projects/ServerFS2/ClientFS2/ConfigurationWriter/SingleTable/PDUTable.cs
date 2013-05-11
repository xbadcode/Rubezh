﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class PDUTable : TableBase
	{
		public Device Device { get; set; }

		public PDUTable(DevicePDUDirection devicePDUDirection, PanelDatabase panelDatabase)
			: base(null, devicePDUDirection.PDUGroupDevice.Device.DottedPresentationNameAndAddress)
		{
			Device = devicePDUDirection.PDUGroupDevice.Device;
			BytesDatabase = new BytesDatabase(Device.DottedPresentationNameAndAddress);
			BytesDatabase.AddByte(Device.AddressOnShleif, "Адрес");
			BytesDatabase.AddByte((Device.ShleifNo - 1), "Шлейф");
			BytesDatabase.AddByte(Device.Parent.IntAddress, "Адрес прибора");
			var deviceCode = DriversHelper.GetCodeForFriver(Device.Driver.DriverType);
			BytesDatabase.AddByte(deviceCode, "Тип ИУ");
			var option = 0;
			if (devicePDUDirection.PDUGroupDevice.IsInversion)
				option = 128;
			BytesDatabase.AddByte(option, "Опции");
			BytesDatabase.AddByte(devicePDUDirection.Device.IntAddress, "Направление");
			BytesDatabase.AddByte(0, "Пустой байт");

			TableBase tableBase = null;
			foreach (var tableGroup in panelDatabase.PanelDatabase2.DevicesTableGroups)
			{
				tableBase = tableGroup.Tables.FirstOrDefault(x => x.UID == Device.UID);
				if (tableBase != null)
				{
					break;
				}
			}
			var offset = tableBase.BytesDatabase.ByteDescriptions.FirstOrDefault().Offset;
			var offsetBytes = BitConverter.GetBytes(offset);
			for (int i = 0; i < 4; i++)
			{
				BytesDatabase.AddByte(offsetBytes[i], "Смещение");
			}

			BytesDatabase.AddByte(devicePDUDirection.PDUGroupDevice.OnDelay, "Задержка на включение");
			BytesDatabase.AddByte(devicePDUDirection.PDUGroupDevice.OffDelay, "Задержка на выключение");
			for (int i = 0; i < 9; i++)
			{
				BytesDatabase.AddByte(255, "ПДУ привязки");
			}
		}
	}
}