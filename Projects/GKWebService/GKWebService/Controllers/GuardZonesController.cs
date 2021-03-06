﻿using GKWebService.DataProviders;
using GKWebService.Models;
using GKWebService.Models.GuardZones;
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
	public class GuardZonesController : Controller
	{
		// GET: GuardZones
		public ActionResult Guard()
		{
			return View();
		}


		public ActionResult GuardZoneDetails()
		{
			return View();
		}

		[HttpGet]
		public JsonResult GetGuardZones()
		{
			List<GuardZone> guardZones = new List<GuardZone>();
			GKManager.GuardZones.ForEach(x => guardZones.Add(new GuardZone(x)));
			return Json(guardZones.OrderBy(x => x.No), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetGuardZoneDevices(Guid id)
		{
			List<Device> data = new List<Device>();
			var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == id);
			if (guardZone != null)
				guardZone.GuardZoneDevices.ForEach(x => data.Add(new Device(x.Device) {ActionType = !x.Device.Driver.IsCardReaderOrCodeReader? x.ActionType.ToDescription(): string.Empty}));
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetAutomaticState(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.RubezhService.GKSetAutomaticRegime(guardZone, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetManualState(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.RubezhService.GKSetManualRegime(guardZone, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetIgnoreState(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.RubezhService.GKSetIgnoreRegime(guardZone, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOn(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.RubezhService.GKTurnOn(guardZone, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOnNow(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.RubezhService.GKTurnOnNow(guardZone, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOff(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.RubezhService.GKTurnOff(guardZone, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOffNow(Guid id)
		{
			var guardZone = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (guardZone != null)
			{
				ClientManager.RubezhService.GKTurnOffNow(guardZone, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult Reset(Guid id)
		{
			var mpt = GKManager.GuardZones.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.RubezhService.GKReset(mpt, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}
	}
}