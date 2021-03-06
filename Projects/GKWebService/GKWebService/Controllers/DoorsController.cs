﻿using GKWebService.DataProviders;
using GKWebService.Models.Door;
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
	public class DoorsController : Controller
    {
        // GET: Doors
        public ActionResult Door()
        {
            return View();
        }

		public ActionResult DoorDetails()
		{
			return View();
		}

		public JsonResult GetDoors()
		{
			var data = new List<Door>();
			GKManager.Doors.ForEach(x => data.Add(new Door(x)));
			return Json(data.OrderBy(x=> x.No), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetAutomaticState(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKSetAutomaticRegime(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetManualState(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKSetManualRegime(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetIgnoreState(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKSetIgnoreRegime(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOn(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKTurnOn(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOffNow(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKTurnOffNow(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult TurnOff(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKTurnOff(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult Reset(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKReset(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetRegimeNorm(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKSetAutomaticRegime(door, ClientManager.CurrentUser.Name);
				ClientManager.RubezhService.GKTurnOffInAutomatic(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();

		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetRegimeOpen(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKSetManualRegime(door, ClientManager.CurrentUser.Name);
				ClientManager.RubezhService.GKTurnOn(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}

		[HttpPost]
		[ConfirmCommand]
		public JsonResult SetRegimeClose(Guid id)
		{
			var door = GKManager.Doors.FirstOrDefault(d => d.UID == id);
			if (door != null)
			{
				ClientManager.RubezhService.GKSetManualRegime(door, ClientManager.CurrentUser.Name);
				ClientManager.RubezhService.GKTurnOff(door, ClientManager.CurrentUser.Name);
			}

			return new JsonResult();
		}
    }
}