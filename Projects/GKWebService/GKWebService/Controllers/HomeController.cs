﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GKWebService.Models;
using GKWebService.Models.GK;
using GKWebService.Utils;
using Infrastructure.Common;
using RubezhAPI.Models;
using RubezhClient;

namespace GKWebService.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		// GET: Home
		[AllowAnonymous]
		public ActionResult Index()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public JsonResult Login(LoginData loginData)
		{
			var error = ClientManager.Connect(loginData.userName, loginData.password);

			if (string.IsNullOrEmpty(error))
			{
				var authTicket = new FormsAuthenticationTicket(
					2,
					loginData.userName,
					DateTime.Now,
					DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
					false,
					string.Empty //clientCredentials.ClientUID.ToString()
					);
				var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket))
				{
					HttpOnly = true
				};
				Response.SuppressFormsAuthenticationRedirect = true;
				Response.AppendCookie(authCookie);
			}

			return Json(new { success = string.IsNullOrEmpty(error), message = error });
		}

		public JsonResult CheckPass(string password)
		{
			var result = ClientManager.CheckPass(HttpContext.User.Identity.Name, password);
			return Json(new { result }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult TryGetCurrentUserName()
		{
			// если веб-сервер перезапустили, то просим пользователя залогиниться заново
			return Json(new { userName = User.Identity.Name }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetCurrentUserPermissions()
		{
			//var permissions = ClientManager.CurrentUser.PermissionStrings;
			// на случай если в ОЗ поменяется наменование конкретного права permission произойдёт ошибка компиляции
			var permissions = new List<string>();
			if (ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
				permissions.Add("Oper_Device_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_Full_Door_Control))
				permissions.Add("Oper_Full_Door_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_Door_Control))
				permissions.Add("Oper_Door_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
				permissions.Add("Oper_Zone_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control))
				permissions.Add("Oper_GuardZone_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
				permissions.Add("Oper_MPT_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
				permissions.Add("Oper_Delay_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
				permissions.Add("Oper_Directions_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_NS_Control))
				permissions.Add("Oper_NS_Control");
			if (ClientManager.CheckPermission(PermissionType.Oper_MayNotConfirmCommands))
				permissions.Add("Oper_MayNotConfirmCommands");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Employees_View))
				permissions.Add("Oper_SKD_Employees_View");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Guests_View))
				permissions.Add("Oper_SKD_Guests_View");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Positions_View))
				permissions.Add("Oper_SKD_Positions_View");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Departments_View))
				permissions.Add("Oper_SKD_Departments_View");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_AdditionalColumns_View))
				permissions.Add("Oper_SKD_AdditionalColumns_View");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Cards_View))
				permissions.Add("Oper_SKD_Cards_View");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_AccessTemplates_View))
				permissions.Add("Oper_SKD_AccessTemplates_View");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_PassCards_View))
				permissions.Add("Oper_SKD_PassCards_View");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Organisations_View))
				permissions.Add("Oper_SKD_Organisations_View");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Employees_Edit))
				permissions.Add("Oper_SKD_Employees_Edit");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Guests_Edit))
				permissions.Add("Oper_SKD_Guests_Edit");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Departments_Etit))
				permissions.Add("Oper_SKD_Departments_Etit");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Positions_Etit))
				permissions.Add("Oper_SKD_Positions_Etit");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_AccessTemplates_Etit))
				permissions.Add("Oper_SKD_AccessTemplates_Etit");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Employees_Edit_CardType))
				permissions.Add("Oper_SKD_Employees_Edit_CardType");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Cards_Etit))
				permissions.Add("Oper_SKD_Cards_Etit");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Organisations_Doors))
				permissions.Add("Oper_SKD_Organisations_Doors");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Organisations_Users))
				permissions.Add("Oper_SKD_Organisations_Users");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Organisations_AddRemove))
				permissions.Add("Oper_SKD_Organisations_AddRemove");
			if (ClientManager.CheckPermission(PermissionType.Oper_SKD_Organisations_Edit))
				permissions.Add("Oper_SKD_Organisations_Edit");

			return Json(new { permissions }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult LogOut()
		{
			FormsAuthentication.SignOut();
            return Json("ok", JsonRequestBehavior.AllowGet);
		}

		[AllowAnonymous]
		[ErrorHandler]
		public JsonResult GetNavigationItems()
		{
			var gkModelLoader = new GKModuleLoader();
			var items = gkModelLoader.CreateNavigation();
			gkModelLoader.Initialize();
			return Json(items, JsonRequestBehavior.AllowGet);
		}

		public ActionResult RestartDetails()
		{
			return View();
		}

		public ActionResult MPTs()
		{
			return View();
		}

		[HttpPost]
		public JsonResult Logon(string login, string password)
		{
			string error = null;

			if (!login.Equals("admin") || !password.Equals("admin"))
			{
				error = "Неверный логин или пароль";
			}

			return Json(new { Success = error == null, Message = error });
		}
	}
}