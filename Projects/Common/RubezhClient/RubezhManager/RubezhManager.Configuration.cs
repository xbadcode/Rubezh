﻿using Common;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RubezhAPI.Plans.Interfaces;

namespace RubezhClient
{
	public partial class ClientManager
	{
		public static PlansConfiguration PlansConfiguration
		{
			get { return ConfigurationCash.PlansConfiguration; }
			set { ConfigurationCash.PlansConfiguration = value; }
		}

		public static SystemConfiguration SystemConfiguration { get; set; }
		public static SecurityConfiguration SecurityConfiguration { get; set; }
		public static LayoutsConfiguration LayoutsConfiguration { get; set; }

		public static void UpdateFiles()
		{
			try
			{
				FileHelper.Synchronize();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.UpdateFiles");
				LoadingErrorManager.Add(e);
			}
		}

		public static void CopyStream(Stream input, Stream output)
		{
			var buffer = new byte[8 * 1024];
			int length;
			while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, length);
			}
			output.Close();
		}

		public static void GetConfiguration(string configurationFolderName)
		{
			try
			{
				var serverConfigDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");
				var configDirectory = AppDataFolderHelper.GetLocalFolder(configurationFolderName);
				var contentDirectory = Path.Combine(configDirectory, "Content");
				if (Directory.Exists(configDirectory))
				{
					Directory.Delete(configDirectory, true);
				}
				Directory.CreateDirectory(configDirectory);
				Directory.CreateDirectory(contentDirectory);
				if (ServiceFactoryBase.ContentService != null)
					ServiceFactoryBase.ContentService.Invalidate();

				if (ConnectionSettingsManager.IsRemote)
				{
					var configFileName = Path.Combine(configDirectory, "Config.fscp");
					var configFileStream = File.Create(configFileName);
					var stream = RubezhService.GetConfig();
					CopyStream(stream, configFileStream);
					LoadFromZipFile(configFileName);

					var result = RubezhService.GetSecurityConfiguration();
					if (!result.HasError && result.Result != null)
					{
						SecurityConfiguration = result.Result;
					}
				}
				else
				{
					foreach (var fileName in Directory.GetFiles(serverConfigDirectory))
					{
						var file = Path.GetFileName(fileName);
						File.Copy(fileName, Path.Combine(configDirectory, file), true);
					}
					foreach (var fileName in Directory.GetFiles(Path.Combine(serverConfigDirectory, "Content")))
					{
						var file = Path.GetFileName(fileName);
						File.Copy(fileName, Path.Combine(contentDirectory, file), true);
					}

					if (File.Exists(serverConfigDirectory + "\\..\\SecurityConfiguration.xml"))
					{
						File.Copy(serverConfigDirectory + "\\..\\SecurityConfiguration.xml", Path.Combine(configDirectory, "SecurityConfiguration.xml"), true);
					}

					LoadConfigFromDirectory(configDirectory);
					SecurityConfiguration = ZipSerializeHelper.DeSerialize<SecurityConfiguration>(Path.Combine(configDirectory, "SecurityConfiguration.xml"));
				}
				UpdateConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.GetConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		public static void UpdateConfiguration()
		{
			try
			{
				if (LayoutsConfiguration == null)
					LayoutsConfiguration = new LayoutsConfiguration();
				LayoutsConfiguration.Update();
				PlansConfiguration.Update();
				SystemConfiguration.UpdateConfiguration();
				GKManager.UpdateConfiguration();
				GKManager.CreateStates();
				UpdatePlansConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.UpdateConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		public static void UpdatePlansConfiguration()
		{
			try
			{
				GKManager.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.Zones.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.Directions.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.MPTs.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.GuardZones.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.SKDZones.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.PumpStations.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.Delays.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });

				SystemConfiguration.Cameras.ForEach(x => x.PlanElementUIDs = new List<Guid>());
				SystemConfiguration.AutomationConfiguration.Procedures.ForEach(x => x.PlanElementUIDs = new List<Guid>());

				var cameraMap = new Dictionary<Guid, Camera>();
				foreach (var camera in SystemConfiguration.Cameras)
				{
					if (!cameraMap.ContainsKey(camera.UID))
						cameraMap.Add(camera.UID, camera);
				}

				var procedureMap = new Dictionary<Guid, Procedure>();
				foreach (var procedure in ClientManager.SystemConfiguration.AutomationConfiguration.Procedures)
				{
					if (!procedureMap.ContainsKey(procedure.Uid))
						procedureMap.Add(procedure.Uid, procedure);
				}

				var planMap = new Dictionary<Guid, Plan>();
				PlansConfiguration.AllPlans.ForEach(plan => planMap.Add(plan.UID, plan));
				foreach (var plan in PlansConfiguration.AllPlans)
				{
					for (int i = plan.ElementGKDevices.Count(); i > 0; i--)
					{
						var elementGKDevice = plan.ElementGKDevices[i - 1];
						var device = GKManager.Devices.Find(x => x.UID == elementGKDevice.DeviceUID);
						//elementGKDevice.UpdateZLayer();
						if (device != null)
							device.PlanElementUIDs.Add(elementGKDevice.UID);
					}
					foreach (var zone in plan.ElementPolygonGKZones)
					{
						var _zone = GKManager.Zones.Find(x => x.UID == zone.ZoneUID);
						//UpdateSKDZoneType(zone, _zone);
						if (_zone != null)
							_zone.PlanElementUIDs.Add(zone.UID);
					}
					foreach (var zone in plan.ElementRectangleGKZones)
					{
						var _zone = GKManager.Zones.Find(x => x.UID == zone.ZoneUID);
						//UpdateSKDZoneType(zone, _zone);
						if (_zone != null)
							_zone.PlanElementUIDs.Add(zone.UID);
					}

					foreach (var zoneSKD in plan.ElementPolygonGKSKDZones)
					{
						var skd = GKManager.SKDZones.Find(x => x.UID == zoneSKD.ZoneUID);
						//UpdateSKDZoneType(zoneSKD, skd);
						if (skd != null)
							skd.PlanElementUIDs.Add(zoneSKD.UID);
					}

					foreach (var zoneSKD in plan.ElementRectangleGKSKDZones)
					{
						var skd = GKManager.SKDZones.Find(x => x.UID == zoneSKD.ZoneUID);
						//UpdateSKDZoneType(zoneSKD, skd);
						if (skd != null)
							skd.PlanElementUIDs.Add(zoneSKD.UID);
					}
					foreach (var guardZone in plan.ElementPolygonGKGuardZones)
					{
						var zone = GKManager.GuardZones.Find(x => x.UID == guardZone.ZoneUID);
						//UpdateZoneType(guardZone, zone);
						if (zone != null)
							zone.PlanElementUIDs.Add(guardZone.UID);
					}
					foreach (var guardZone in plan.ElementRectangleGKGuardZones)
					{
						var zone = GKManager.GuardZones.Find(x => x.UID == guardZone.ZoneUID);
						//UpdateZoneType(guardZone, zone);
						if (zone != null)
							zone.PlanElementUIDs.Add(guardZone.UID);
					}
					foreach (var delay in plan.ElementRectangleGKDelays)
					{
						var delayGK = GKManager.Delays.Find(x => x.UID == delay.DelayUID);
						//UpdateDelayType(delay, delayGK);
						if (delayGK != null)
							delayGK.PlanElementUIDs.Add(delay.UID);
					}
					foreach (var delay in plan.ElementPolygonGKDelays)
					{
						var delayGK = GKManager.Delays.Find(x => x.UID == delay.DelayUID);
						//UpdateDelayType(delay, delayGK);
						if (delayGK != null)
							delayGK.PlanElementUIDs.Add(delay.UID);
					}
					foreach (var pumpStation in new IElementPumpStation[0]
						.Concat(plan.ElementPolygonGKPumpStations)
						.Concat(plan.ElementRectangleGKPumpStations))
					{
						var pump =  GKManager.PumpStations.Find(x => x.UID == pumpStation.PumpStationUID);
						//UpdatePumpStationType(pumpStation, pump);
						if (pump != null)
							pump.PlanElementUIDs.Add(pumpStation.UID);
					}
					foreach (var direction in plan.ElementRectangleGKDirections)
					{
						var directionGK = GKManager.Directions.Find(x => x.UID == direction.DirectionUID);
						//UpdateDirectionType(direction, directionGK);
						if (directionGK != null)
							directionGK.PlanElementUIDs.Add(direction.UID);
					}
					foreach (var direction in plan.ElementPolygonGKDirections)
					{
						var directionGK = GKManager.Directions.Find(x => x.UID == direction.DirectionUID);
						//UpdateDirectionType(direction, directionGK);
						if (directionGK != null)
							directionGK.PlanElementUIDs.Add(direction.UID);
					}
					foreach (var mpt in plan.ElementRectangleGKMPTs)
					{
						var mptGK = GKManager.MPTs.Find(x => x.UID == mpt.MPTUID);
						//UpdateMPTType(mpt, mptGK);
						if (mptGK != null)
							mptGK.PlanElementUIDs.Add(mpt.UID);
					}
					foreach (var mpt in plan.ElementPolygonGKMPTs)
					{
						var mptGK = GKManager.MPTs.Find(x => x.UID == mpt.MPTUID);
						//UpdateMPTType(mpt, mptGK);
						if (mptGK != null)
							mptGK.PlanElementUIDs.Add(mpt.UID);
					}
					for (int i = plan.ElementGKDoors.Count(); i > 0; i--)
					{
						var elementGKDoor = plan.ElementGKDoors[i - 1];
						var doorGK = GKManager.Doors.Find(x => x.UID == elementGKDoor.DoorUID);
						//elementGKDoor.UpdateZLayer();
						if (doorGK != null)
							doorGK.PlanElementUIDs.Add(elementGKDoor.UID);
					}

					for (int i = plan.ElementExtensions.Count(); i > 0; i--)
					{
						var elementExtension = plan.ElementExtensions[i - 1];
						//elementExtension.UpdateZLayer();
						var elementCamera = elementExtension as ElementCamera;

						if (elementCamera != null && cameraMap.ContainsKey(elementCamera.CameraUID))
							cameraMap[elementCamera.CameraUID].PlanElementUIDs.Add(elementExtension.UID);
						else if (elementExtension is ElementProcedure)
						{
							var elementProcedure = (ElementProcedure)elementExtension;
							if (procedureMap.ContainsKey(elementProcedure.ProcedureUID))
								procedureMap[elementProcedure.ProcedureUID].PlanElementUIDs.Add(elementExtension.UID);
						}
					}

					foreach (var subplan in plan.ElementRectangleSubPlans)
						UpdateSubPlan(subplan, subplan.PlanUID != Guid.Empty && planMap.ContainsKey(subplan.PlanUID) ? planMap[subplan.PlanUID] : null);
					foreach (var subplan in plan.ElementPolygonSubPlans)
						UpdateSubPlan(subplan, subplan.PlanUID != Guid.Empty && planMap.ContainsKey(subplan.PlanUID) ? planMap[subplan.PlanUID] : null);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.UpdatePlansConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		//private static void UpdateZoneType(IElementZone elementZone, GKGuardZone zone)
		//{
		//	elementZone.SetZLayer(zone == null ? 20 : 40);
		//	elementZone.BackgroundColor = zone == null ? Colors.Black : Colors.Brown;
		//}
		
		//private static void UpdateSKDZoneType(IElementZone elementZone, GKBase zone)
		//{
		//	elementZone.SetZLayer(zone == null  ? 50 : 60);
		//	elementZone.BackgroundColor = zone == null ? Colors.Black : Colors.Green;
		//}
		//private static void UpdateDelayType(IElementDelay elementGKDelay, GKDelay gkDelay)
		//{
		//	elementGKDelay.BackgroundColor = gkDelay == null ? Colors.Black : Colors.LightBlue;
		//}
		//private static void UpdatePumpStationType(IElementPumpStation elementGKPumpStation, GKPumpStation gkPumpStation)
		//{
		//	elementGKPumpStation.BackgroundColor = (gkPumpStation == null) ? Colors.Black : Colors.Cyan;
		//}
		//private static void UpdateDirectionType(IElementDirection elementGKDirection, GKDirection gkDirection)
		//{
		//	elementGKDirection.SetZLayer(gkDirection == null ? 10 : 11);
		//	elementGKDirection.BackgroundColor = gkDirection == null ? Colors.Black : Colors.LightBlue;
		//}
		//private static void UpdateMPTType(IElementMPT elementGKMPT, GKMPT gkMPT)
		//{
		//	elementGKMPT.SetZLayer(gkMPT == null ? 10 : 11);
		//	elementGKMPT.BackgroundColor = gkMPT == null ? Colors.Black : Colors.LightBlue;
		//}
		private static void UpdateSubPlan(ElementBase elementSubPlan, Plan plan)
		{
			elementSubPlan.BackgroundColor = plan == null ? Colors.Black : Colors.Green;
		}

		public static void InvalidateContent()
		{
			var uids = new HashSet<Guid?>();
			foreach (var plan in PlansConfiguration.AllPlans)
			{
				uids.Add(plan.BackgroundImageSource);
				uids.Add(plan.BackgroundSVGImageSource);
				plan.AllElements.ForEach(x => uids.Add(x.BackgroundImageSource));
				plan.AllElements.ForEach(x => uids.Add(x.BackgroundSVGImageSource));
			}
			foreach (var layout in LayoutsConfiguration.Layouts)
			{
				foreach (var part in layout.Parts)
				{
					if (part.Properties is LayoutPartImageProperties)
					{
						var layoutPartImageProperties = part.Properties as LayoutPartImageProperties;
						uids.Add(layoutPartImageProperties.ReferenceUID);
						uids.Add(layoutPartImageProperties.ReferenceSVGUID);
					}
				}
			}
			SystemConfiguration.AutomationConfiguration.AutomationSounds.ForEach(x => uids.Add(x.Uid));
			uids.Remove(null);
			uids.Remove(Guid.Empty);
			ServiceFactoryBase.ContentService.RemoveAllBut(uids.Select(x => x.Value.ToString()).ToList());
		}
	}
}