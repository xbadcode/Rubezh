﻿using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public partial class GKManager
	{
		public static void ChangeDeviceZones(GKDevice device, List<GKZone> zones)
		{
			foreach (var zone in device.Zones)
			{
				zone.Devices.Remove(device);
				zone.OnChanged();
			}
			device.Zones.Clear();
			device.ZoneUIDs.Clear();
			foreach (var zone in zones)
			{
				device.Zones.Add(zone);
				device.ZoneUIDs.Add(zone.UID);
				zone.Devices.Add(device);
				zone.OnChanged();
			}
			device.OnChanged();
		}

		public static void ChangeDeviceGuardZones(GKDevice device, List<GKDeviceGuardZone> deviceGuardZones)
		{
			foreach (var guardZone in device.GuardZones)
			{
				guardZone.GuardZoneDevices.RemoveAll(x => x.Device == device);
				guardZone.OnChanged();
			}
			device.GuardZones.Clear();
			foreach (var deviceGuardZone in deviceGuardZones)
			{
				device.GuardZones.Add(deviceGuardZone.GuardZone);

				var gkGuardZoneDevice = new GKGuardZoneDevice();
				gkGuardZoneDevice.Device = device;
				gkGuardZoneDevice.DeviceUID = device.UID;
				if (deviceGuardZone.ActionType != null)
					gkGuardZoneDevice.ActionType = deviceGuardZone.ActionType.Value;
				gkGuardZoneDevice.CodeReaderSettings = deviceGuardZone.CodeReaderSettings;
				deviceGuardZone.GuardZone.GuardZoneDevices.Add(gkGuardZoneDevice);
				deviceGuardZone.GuardZone.OnChanged();
			}
			device.ChangedLogic();
			device.OnChanged();
		}

		public static void AddDeviceToZone(GKDevice device, GKZone zone)
		{
			if (!device.Zones.Contains(zone))
			{
				device.Zones.Add(zone);
			}
			if (!device.ZoneUIDs.Contains(zone.UID))
				device.ZoneUIDs.Add(zone.UID);
			if (!device.InputDependentElements.Contains(zone))
				device.InputDependentElements.Add(zone);
			if (!zone.OutputDependentElements.Contains(device))
				zone.OutputDependentElements.Add(device);
			zone.Devices.Add(device);
			zone.OnChanged();
			device.OnChanged();
		}

		public static void AddDeviceToGuardZone(GKDevice device, GKGuardZone guardZone ,GKGuardZoneDevice guardZoneDevice = null)
		{
			if (guardZoneDevice!= null)
			guardZone.GuardZoneDevices.Add(guardZoneDevice);
			if (!device.GuardZones.Contains(guardZone))
			{
				device.GuardZones.Add(guardZone);
			}
			if (!device.InputDependentElements.Contains(guardZone))
				device.InputDependentElements.Add(guardZone);
			if (!guardZone.OutputDependentElements.Contains(device))
				guardZone.OutputDependentElements.Add(device);
			guardZone.OnChanged();
			device.OnChanged();
		}

		public static void RemoveDeviceFromZone(GKDevice device, GKZone zone)
		{
			if (zone != null)
			{
				device.Zones.Remove(zone);
				device.ZoneUIDs.Remove(zone.UID);
				zone.Devices.Remove(device);
				zone.OutputDependentElements.Remove(device);
				device.InputDependentElements.Remove(zone);
				zone.OnChanged();
				device.OnChanged();
			}
		}

		public static void RemoveDeviceFromGuardZone(GKDevice device, GKGuardZone guardZone)
		{
			if (guardZone != null)
			{
				guardZone.GuardZoneDevices.RemoveAll(x => x.DeviceUID == device.UID);
				device.GuardZones.RemoveAll(x => x.UID == guardZone.UID);
				guardZone.OutputDependentElements.RemoveAll(x => x.UID == device.UID);
				device.InputDependentElements.RemoveAll(x => x.UID == guardZone.UID);
				device.OnChanged();
			}
		}

		public static void AddDevice(GKDevice device)
		{
			device.InitializeDefaultProperties();
		}

		/// <summary>
		/// Удаление устройства
		/// </summary>
		/// <param name="device"></param>
		public static List<GKDevice> RemoveDevice(GKDevice device)
		{
			var allDevices = device.AllChildrenAndSelf;
			foreach (var deviceItem in device.AllChildrenAndSelf)
			{
				//var parentDevice = device.Parent;
				foreach (var zone in deviceItem.Zones)
				{
					zone.Devices.Remove(deviceItem);
					zone.OnChanged();
				}

				deviceItem.Parent.Children.Remove(deviceItem);
				Devices.Remove(deviceItem);

				deviceItem.InputDependentElements.ForEach(x =>
				{
					x.OutputDependentElements.Remove(deviceItem);
					if (x is GKGuardZone)
						x.Invalidate(GKManager.DeviceConfiguration);
				});

				deviceItem.OutputDependentElements.ForEach(x =>
				{
					x.InputDependentElements.Remove(deviceItem);
					x.Invalidate(GKManager.DeviceConfiguration);
					x.OnChanged();
				});
			}
			return allDevices;
		}

		#region RebuildRSR2Addresses
		public static void RebuildRSR2Addresses(GKDevice parentDevice)
		{
			var kauParent = parentDevice.KAUParent;
			if (kauParent != null)
			{
				foreach (var shliefDevice in kauParent.Children)
				{
					RebuildRSR2Addresses_Children = new List<GKDevice>();
					RebuildRSR2Addresses_AddChild(shliefDevice);

					byte currentAddress = 1;
					foreach (var device in RebuildRSR2Addresses_Children)
					{
						device.IntAddress = currentAddress;
						if (!device.Driver.IsGroupDevice)
						{
							currentAddress++;
						}
						device.OnChanged();
					}

					RebuildRSR2Addresses_Children.FindAll(x => x.Driver.IsGroupDevice).ForEach(x => x.OnChanged());
				}
			}

			var mirrorParent = parentDevice.MirrorParent;
			if (mirrorParent != null)
			{
				int currentAddress = 1;
				foreach (var device in mirrorParent.Children.Where(x=> x.Driver.HasMirror))
				{
					device.IntAddress = currentAddress;
					if (!device.Driver.IsGroupDevice)
					{
						currentAddress++;
					}
					device.OnChanged();
				}
			}
		}

		static List<GKDevice> RebuildRSR2Addresses_Children;
		static void RebuildRSR2Addresses_AddChild(GKDevice device)
		{
			if (device.DriverType != GKDriverType.RSR2_MVP_Part && device.DriverType != GKDriverType.RSR2_KAU_Shleif)
				RebuildRSR2Addresses_Children.Add(device);

			foreach (var child in device.Children)
			{
				RebuildRSR2Addresses_AddChild(child);
			}
		}
		#endregion

		public static bool ChangeDriver(GKDevice device, GKDriver driver)
		{
			var kauShleifParent = device.KAUShleifParent;
			if (kauShleifParent != null)
			{
				var maxAddress = 0;
				if (kauShleifParent.Children.Count > 0)
				{
					maxAddress = kauShleifParent.Children.Max(x => x.IntAddress);
				}
				if (maxAddress + (driver.GroupDeviceChildrenCount > 0 ? driver.GroupDeviceChildrenCount : 1) - 1 > 255)
				{
					return false;
				}
			}

			foreach (var gkBase in device.OutputDependentElements)
			{
				gkBase.InputDependentElements.Remove(device);
				gkBase.OnChanged();
			}
			foreach (var gkBase in device.InputDependentElements)
			{
				gkBase.OutputDependentElements.Remove(device);
				gkBase.OnChanged();
			}
			if (device.Children != null)
			{
				device.Children.ForEach(x =>
				{
					GKManager.Devices.Remove(x);
					x.OutputDependentElements.ForEach(y =>
					{
						y.InputDependentElements.Remove(x);
						y.ChangedLogic();
						y.OnChanged();
					});
					x.InputDependentElements.ForEach(y =>
					{
						y.OutputDependentElements.Remove(x);
						if (y is GKGuardZone)
						{
							y.Invalidate(GKManager.DeviceConfiguration);
						}
						if (y is GKZone)
						{
							GKManager.Zones.ForEach(zone =>
							{
								if (zone == y)
									zone.Devices.Remove(x);
							});
						}
					});
				});
			}

			var changeZone = !(device.Driver.HasZone && driver.HasLogic);
			device.Driver = driver;
			device.DriverUID = driver.UID;
			if (driver.IsRangeEnabled)
				device.IntAddress = driver.MinAddress;

			device.Children.Clear();
			AddAutoCreateChildren(device);

			if (changeZone)
			{
				RemoveDeviceFromZone(device, null);
				device.Zones.ForEach(x => x.Devices.Remove(device));
				SetDeviceLogic (device, new GKLogic());
			}

			device.Properties = new List<GKProperty>();
			Guid oldUID = device.UID;
			device.UID = Guid.NewGuid();

			device.OnUIDChanged(oldUID, device.UID);

			device.OutputDependentElements.ForEach(x =>
			{
				x.Invalidate(GKManager.DeviceConfiguration);
				x.OnChanged();
			});

			device.InputDependentElements.ForEach(x =>
			{
				if (x is GKGuardZone)
				{
					x.Invalidate(GKManager.DeviceConfiguration);
					x.OnChanged();
				}
				x.UpdateLogic(GKManager.DeviceConfiguration);
				x.OnChanged();
			});
			device.Zones = new List<GKZone>();
			device.ZoneUIDs = new List<Guid>();
			device.GuardZones = new List<GKGuardZone>();
			device.InputDependentElements = new List<GKBase>();
			device.OutputDependentElements = new List<GKBase>();
			device.IsInMPT = false;
			RebuildRSR2Addresses(device);
			return true;
		}

		public static void RemoveSKDZone(GKSKDZone zone)
		{
			SKDZones.Remove(zone);
			zone.OnChanged();
		}

		public static void EditSKDZone(GKSKDZone zone)
		{
			zone.OnChanged();
		}
	}
}