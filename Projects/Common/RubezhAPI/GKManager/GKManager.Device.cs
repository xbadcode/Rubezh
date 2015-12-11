﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RubezhAPI.GK;
using System.Collections.ObjectModel;

namespace RubezhAPI
{
	public partial class GKManager
	{
		public static GKDevice CopyDevice(GKDevice device, bool fullCopy , bool paste = false)
		{
			var newDevice = new GKDevice();
			if (fullCopy)
			{
				newDevice.UID = device.UID;
			}
			CopyDevice(device, newDevice);
			if (paste)
			{
				foreach (var guardZone in GKManager.GuardZones)
				{
					var guardZones = newDevice.GuardZones.FirstOrDefault(x => x.UID == guardZone.UID);
					if (guardZones != null)
						guardZone.GuardZoneDevices.AddRange(guardZones.GuardZoneDevices);
				}
			}
			return newDevice;
		}

		public static GKDevice CopyDevice(GKDevice deviceFrom, GKDevice deviceTo)
		{
			deviceTo.DriverUID = deviceFrom.DriverUID;
			deviceTo.Driver = deviceFrom.Driver;
			deviceTo.IntAddress = deviceFrom.IntAddress;
			deviceTo.Description = deviceFrom.Description;
			deviceTo.ProjectAddress = deviceFrom.ProjectAddress;
			deviceTo.PredefinedName = deviceFrom.PredefinedName;
			deviceTo.InputDependentElements = deviceFrom.InputDependentElements;
			deviceTo.OutputDependentElements = deviceFrom.OutputDependentElements;

			deviceTo.Properties = new List<GKProperty>();
			foreach (var property in deviceFrom.Properties)
			{
				deviceTo.Properties.Add(new GKProperty()
				{
					Name = property.Name,
					Value = property.Value,
					DriverProperty = property.DriverProperty,
					StringValue = property.StringValue,
				});
			}

			deviceTo.ZoneUIDs = deviceFrom.ZoneUIDs.ToList();
			deviceTo.Zones = deviceFrom.Zones.ToList();
			deviceTo.GuardZones = deviceFrom.GuardZones.ToList();

			deviceTo.Logic.OnClausesGroup = deviceFrom.Logic.OnClausesGroup.Clone();
			deviceTo.Logic.OffClausesGroup = deviceFrom.Logic.OffClausesGroup.Clone();
			deviceTo.Logic.StopClausesGroup = deviceFrom.Logic.StopClausesGroup.Clone();
			deviceTo.Logic.OnNowClausesGroup = deviceFrom.Logic.OnNowClausesGroup.Clone();
			deviceTo.Logic.OffNowClausesGroup = deviceFrom.Logic.OffNowClausesGroup.Clone();

			deviceTo.Children = new List<GKDevice>();
			foreach (var childDevice in deviceFrom.Children)
			{
				var newChildDevice = CopyDevice(childDevice, false);
				newChildDevice.Parent = deviceTo;
				deviceTo.Children.Add(newChildDevice);
			}

			deviceTo.PlanElementUIDs = new List<Guid>();
			foreach (var deviceElementUID in deviceFrom.PlanElementUIDs)
				deviceTo.PlanElementUIDs.Add(deviceElementUID);

			var newGuardZone = new List<GKGuardZone>();
			foreach (var zone in deviceTo.GuardZones)
			{
				var guardZoneDevice = zone.GuardZoneDevices.FirstOrDefault(x => x.DeviceUID == deviceFrom.UID);
				if (guardZoneDevice != null)
				{
					var newZone = new GKGuardZone {UID = zone.UID };
					var GuardZoneDevice = new GKGuardZoneDevice()
					{
						DeviceUID = deviceTo.UID,
						Device = deviceTo,
						ActionType = guardZoneDevice.ActionType,
						CodeReaderSettings = guardZoneDevice.CodeReaderSettings,
					};
					newZone.GuardZoneDevices.Add(GuardZoneDevice);
					newGuardZone.Add(newZone);
				}
			}
			deviceTo.GuardZones = new List<GKGuardZone>(newGuardZone);
			return deviceTo;
		}

		public static GKDevice AddChild(GKDevice parentDevice, GKDevice previousDevice, GKDriver driver, int intAddress, bool isStartList = false)
		{
			var device = new GKDevice()
			{
				DriverUID = driver.UID,
				Driver = driver,
				IntAddress = intAddress,
				Parent = previousDevice ==null ? parentDevice : previousDevice,
			};
			device.InitializeDefaultProperties();

			if (previousDevice == null || parentDevice == previousDevice)
			{
				if (isStartList)
				{
					parentDevice.Children.Insert(0, device);
				}
				else
					parentDevice.Children.Add(device);
			}
			else
			{
				if (previousDevice != null)
				{
					var index = previousDevice.Children.IndexOf(parentDevice);
					previousDevice.Children.Insert(index + 1, device);
				}
				else
				{
					var index = parentDevice.Children.IndexOf(previousDevice);
					parentDevice.Children.Insert(index + 1, device);
				}
			}

			if (driver.DriverType == GKDriverType.GK || driver.DriverType == GKDriverType.RSR2_GKMirror)
			{
				var indicatorsGroupDriver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKIndicatorsGroup);
				var relaysGroupDriver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKRelaysGroup);
				var indicatorDriver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKIndicator);
				var releDriver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKRele);

				AddChild(device, null, indicatorsGroupDriver, 1);
				AddChild(device, null, relaysGroupDriver, 1);

				for (byte i = 2; i <= 11; i++)
				{
					AddChild(device.Children[0], null, indicatorDriver, i);
				}
				for (byte i = 12; i <= 16; i++)
				{
					AddChild(device.Children[1], null, releDriver, i);
				}
				for (byte i = 17; i <= 22; i++)
				{
					AddChild(device.Children[0], null, indicatorDriver, i);
				}
				DeviceConfiguration.UpdateGKPredefinedName(device);
			}
			else
			{
				AddAutoCreateChildren(device);
			}
			return device;
		}

		public static void AddAutoCreateChildren(GKDevice device)
		{
			foreach (var autoCreateDriverType in device.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == autoCreateDriverType);
				for (byte i = autoCreateDriver.MinAddress; i <= autoCreateDriver.MaxAddress; i++)
				{
					AddChild(device, null, autoCreateDriver, i);
				}
			}

			if ( device.Driver.IsGroupDevice && device.Children.Count == 0)
			{
				var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == device.Driver.GroupDeviceChildType);

				for (byte i = 0; i < device.Driver.GroupDeviceChildrenCount; i++)
				{
					 AddChild(device, null, driver, device.IntAddress + i);
				}
			}
		}


		public static void SetDeviceLogic(GKDevice device, GKLogic logic, bool isNs = false)
		{
			if (isNs)
				device.NSLogic = logic;
			else
				device.Logic = logic;

			device.ChangedLogic();
			device.OnChanged();
		}

		public static bool IsValidIpAddress(GKDevice device)
		{
			if (device.DriverType == GKDriverType.GK)
			{
				const string pattern = @"^([01]\d\d?|[01]?[1-9]\d?|2[0-4]\d|25[0-3])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])$";
				var address = device.GetGKIpAddress();
				if (string.IsNullOrEmpty(address) || !Regex.IsMatch(address, pattern))
				{
					return false;
				}
			}
			return true;
		}
	}
}