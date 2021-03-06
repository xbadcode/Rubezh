﻿using GKWebService.DataProviders;
using GKWebService.Models;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
	[Authorize]
	public class MPTsController : Controller
    {
        // GET: MPTs
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult MPTDetails()
		{
			return View();
		}

		public JsonResult GetMPTsData()
		{
			GKManager.MPTs.Select(x =>new MPTModel(x));
			return Json(GKManager.MPTs.Select(x => new MPTModel(x)).OrderBy(x=> x.No), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetMPTDevices(Guid id)
		{
			var data = new List<Device>();
			var device = GKManager.MPTs.FirstOrDefault(x => x.UID == id);
			if (device != null)
				device.MPTDevices.ForEach(x => data.Add(new Device(x.Device) { MPTDeviceType = x.MPTDeviceType.ToDescription() }));

			return Json(data, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetAutomaticState(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.RubezhService.GKSetAutomaticRegime(mpt, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetManualState(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.RubezhService.GKSetManualRegime(mpt, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetIgnoreState(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.RubezhService.GKSetIgnoreRegime(mpt, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOn(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.RubezhService.GKTurnOn(mpt, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOnNow(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.RubezhService.GKTurnOnNow(mpt, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOff(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.RubezhService.GKTurnOff(mpt, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult ForbidStart(Guid id)
		{
			var mpt = GKManager.MPTs.FirstOrDefault(d => d.UID == id);
			if (mpt != null)
			{
				ClientManager.RubezhService.GKStop(mpt, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

    }
}