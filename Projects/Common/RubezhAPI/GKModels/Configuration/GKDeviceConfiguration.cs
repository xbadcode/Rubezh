﻿using Common;
using RubezhAPI.OPC;
using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Конфигурация ГК
	/// </summary>
	[DataContract]
	public partial class GKDeviceConfiguration : VersionedConfiguration
	{
		public GKDeviceConfiguration()
		{
			Devices = new List<GKDevice>();
			Zones = new List<GKZone>();
			Directions = new List<GKDirection>();
			PumpStations = new List<GKPumpStation>();
			MPTs = new List<GKMPT>();
			Delays = new List<GKDelay>();
			GuardZones = new List<GKGuardZone>();
			Codes = new List<GKCode>();
			Doors = new List<GKDoor>();
			SKDZones = new List<GKSKDZone>();
			OPCSettings = new OPCSettings();
			
			ParameterTemplates = new List<GKParameterTemplate>();
			GKNameGenerationType = GKNameGenerationType.DriverTypePlusAddressPlusDescription;
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		/// <summary>
		/// Устройство верхнего уровня
		/// </summary>
		[DataMember]
		public GKDevice RootDevice { get; set; }

		/// <summary>
		/// Пожарные зоны
		/// </summary>
		[DataMember]
		public List<GKZone> Zones { get; set; }

		/// <summary>
		/// Направления
		/// </summary>
		[DataMember]
		public List<GKDirection> Directions { get; set; }

		/// <summary>
		/// Насосные станции
		/// </summary>
		[DataMember]
		public List<GKPumpStation> PumpStations { get; set; }

		/// <summary>
		/// Модули пожаротушения
		/// </summary>
		[DataMember]
		public List<GKMPT> MPTs { get; set; }

		/// <summary>
		/// Задержки
		/// </summary>
		[DataMember]
		public List<GKDelay> Delays { get; set; }

		/// <summary>
		/// Коды
		/// </summary>
		[DataMember]
		public List<GKCode> Codes { get; set; }

		/// <summary>
		/// Охранные зоны
		/// </summary>
		[DataMember]
		public List<GKGuardZone> GuardZones { get; set; }

		/// <summary>
		/// Точки доступа
		/// </summary>
		[DataMember]
		public List<GKDoor> Doors { get; set; }

		/// <summary>
		/// Зоны СКД
		/// </summary>
		[DataMember]
		public List<GKSKDZone> SKDZones { get; set; }

		/// <summary>
		/// Шаблоны параметров устройств
		/// </summary>
		[DataMember]
		public List<GKParameterTemplate> ParameterTemplates { get; set; }

		/// <summary>
		/// Тип генерации названия компонентка в ГК
		/// </summary>
		[DataMember]
		public GKNameGenerationType GKNameGenerationType { get; set; }

		/// <summary>
		/// Писать только конфигурацию ГК
		/// </summary>
		[DataMember]
		public bool OnlyGKDeviceConfiguration { get; set; }

		[DataMember]
		public OPCSettings OPCSettings { get; set; }

		public void Update()
		{
			Devices = new List<GKDevice>();
			if (RootDevice != null)
			{
				RootDevice.Parent = null;
				Devices.Add(RootDevice);
				AddChild(RootDevice);
			}
		}

		void AddChild(GKDevice parentDevice)
		{
			foreach (var device in parentDevice.Children)
			{
				device.Parent = parentDevice;
				Devices.Add(device);
				AddChild(device);
			}
		}

		[XmlIgnore]
		public List<GKZone> SortedZones
		{
			get
			{
				return (
				from GKZone zone in Zones
				orderby zone.No
				select zone).ToList();
			}
		}

		public void Reorder()
		{
			foreach (var device in Devices)
			{
				if (device.DriverType == GKDriverType.RSR2_KAU || device.DriverType == GKDriverType.GKMirror)
				{
					GKManager.RebuildRSR2Addresses(device);
				}
				device.SynchronizeChildren();
			}
		}

		public override bool ValidateVersion()
		{
			bool result = true;

			if (RootDevice == null)
			{
				var systemDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
				if (systemDriver != null)
				{
					RootDevice = new GKDevice()
					{
						DriverUID = systemDriver.UID,
						Driver = systemDriver
					};
				}
				else
				{
					Logger.Error("ValidateVersion.RootDevice = null systemDriver = null");
				}
			}

			return result;
		}
	}
}