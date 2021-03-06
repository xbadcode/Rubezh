﻿using GKWebService.DataProviders;
using GKWebService.Models;
using GKWebService.Models.PumpStation;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
	[Authorize]
	public class PumpStationsController : Controller
    {
         //GET: PunpStations
		public ActionResult PumpStations()
		{
			return View();
		}

		public ActionResult PumpStationDetails()
		{
			return View();
		}

		public JsonResult GetPumpStations()
		{
			return Json(GKManager.PumpStations.Select(x=> new PumpStation(x)).OrderBy(x=> x.No), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetPumpStationDevices(Guid id)
		{
			var data = new List<Device>();
			var punpstation = GKManager.PumpStations.FirstOrDefault(x => x.UID == id);
			if (punpstation != null)
			{
				punpstation.NSDeviceUIDs.ForEach(x =>
				{
					var device = GKManager.Devices.FirstOrDefault(y => y.UID == x);
					if (device != null)
						data.Add(new Device(device));
				});
			}
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetAutomaticState(Guid id)
		{
			var pumpStation = GKManager.PumpStations.FirstOrDefault(d => d.UID == id);
			if (pumpStation != null)
			{
				ClientManager.RubezhService.GKSetAutomaticRegime(pumpStation, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetManualState(Guid id)
		{
			var pumpStation = GKManager.PumpStations.FirstOrDefault(d => d.UID == id);
			if (pumpStation != null)
			{
				ClientManager.RubezhService.GKSetManualRegime(pumpStation, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetIgnoreState(Guid id)
		{
			var pumpStation = GKManager.PumpStations.FirstOrDefault(d => d.UID == id);
			if (pumpStation != null)
			{
				ClientManager.RubezhService.GKSetIgnoreRegime(pumpStation, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOn(Guid id)
		{
			var pumpStation = GKManager.PumpStations.FirstOrDefault(d => d.UID == id);
			if (pumpStation != null)
			{
				ClientManager.RubezhService.GKTurnOn(pumpStation, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOnNow(Guid id)
		{
			var pumpStation = GKManager.PumpStations.FirstOrDefault(d => d.UID == id);
			if (pumpStation != null)
			{
				ClientManager.RubezhService.GKTurnOnNow(pumpStation, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOff(Guid id)
		{
			var pumpStation = GKManager.PumpStations.FirstOrDefault(d => d.UID == id);
			if (pumpStation != null)
			{
				ClientManager.RubezhService.GKTurnOff(pumpStation, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult ForbidStart(Guid id)
		{
			var pumpStation = GKManager.PumpStations.FirstOrDefault(d => d.UID == id);
			if (pumpStation != null)
			{
				ClientManager.RubezhService.GKStop(pumpStation, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}
    }
}