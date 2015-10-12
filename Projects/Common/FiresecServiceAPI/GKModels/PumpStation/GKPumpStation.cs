﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;
using RubezhClient;
using System.Linq;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Насосная станция ГК
	/// </summary>
	[DataContract]
	public class GKPumpStation : GKBase
	{
		public GKPumpStation()
		{
			Delay = 30;
			Hold = 600;
			DelayRegime = DelayRegime.Off;
			NSPumpsCount = 1;
			NSDeltaTime = 5;
			StartLogic = new GKLogic();
			StopLogic = new GKLogic();
			AutomaticOffLogic = new GKLogic();

			NSDevices = new List<GKDevice>();
			NSDeviceUIDs = new List<Guid>();

			Pim = new GKPim()
			{
				IsAutoGenerated = true,
				PumpStationUID = UID,
				UID = GuidHelper.CreateOn(UID, 0)
			};
		}

		public override void Invalidate()
		{
			var nsDevicesUIDs = new List<Guid>();
			NSDevices = new List<GKDevice>();
			foreach (var NSDevicesUID in NSDeviceUIDs)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == NSDevicesUID);
				if (device != null)
				{
					nsDevicesUIDs.Add(NSDevicesUID);
					NSDevices.Add(device);
					AddDependentElement(device);
				}
			}

			NSDeviceUIDs = nsDevicesUIDs;

			UpdateLogic();

			StartLogic.GetObjects().ForEach(x => AddDependentElement(x));
			
			StopLogic.GetObjects().ForEach(x => AddDependentElement(x));

			AutomaticOffLogic.GetObjects().ForEach(x => AddDependentElement(x));
			
		}

		public override void UpdateLogic()
		{
			GKManager.DeviceConfiguration.InvalidateInputObjectsBaseLogic(this, StartLogic);
			GKManager.DeviceConfiguration.InvalidateInputObjectsBaseLogic(this, StopLogic);
			GKManager.DeviceConfiguration.InvalidateInputObjectsBaseLogic(this, AutomaticOffLogic);
		}

		[XmlIgnore]
		public override bool IsLogicOnKau { get; set; }

		[XmlIgnore]
		public GKPim Pim { get; private set; }
		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.PumpStation; } }
		[XmlIgnore]
		public List<GKDevice> NSDevices { get; set; }

		/// <summary>
		/// Время задержки
		/// </summary>
		[DataMember]
		public ushort Delay { get; set; }

		ushort _hold;
		/// <summary>
		/// Время удержания
		/// </summary>		
		[DataMember]
		public ushort Hold { get; set; }

		/// <summary>
		/// Режим после удержания
		/// </summary>
		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		/// <summary>
		/// Количество основных насосов
		/// </summary>
		[DataMember]
		public int NSPumpsCount { get; set; }

		/// <summary>
		/// Время разновременного пуска
		/// </summary>
		[DataMember]
		public int NSDeltaTime { get; set; }

		/// <summary>
		/// Идентификаторы устройств, входящих в НС
		/// </summary>
		[DataMember]
		public List<Guid> NSDeviceUIDs { get; set; }

		/// <summary>
		/// Логика включения
		/// </summary>
		[DataMember]
		public GKLogic StartLogic { get; set; }

		/// <summary>
		/// Логика выключения
		/// </summary>
		[DataMember]
		public GKLogic StopLogic { get; set; }

		/// <summary>
		/// Логика отключения автоматики
		/// </summary>
		[DataMember]
		public GKLogic AutomaticOffLogic { get; set; }

		[XmlIgnore]
		public override string PresentationName
		{
			get 
			{
				var presentationName = "0" + No + "." + Name;
				if (Pim != null)
					Pim.Name = presentationName;
				return presentationName; 
			}
		}

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/BPumpStation.png"; }
		}
	}
}