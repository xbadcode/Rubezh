﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RubezhAPI.Models;

namespace GKWebService.DataProviders
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public class ConfirmCommandAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			var isAuthenticated = base.AuthorizeCore(httpContext);
			if (isAuthenticated)
			{
				if (ClientManager.CheckPermission(PermissionType.Oper_MayNotConfirmCommands))
				{
					return true;
				}

				if (httpContext.Request.Headers == null ||
				    httpContext.Request.Headers["Password"] == null)
				{
					return false;
				}

				var password = httpContext.Request.Headers["Password"];

				if (!ClientManager.CheckPass(httpContext.User.Identity.Name, password))
				{
					return false;
				}
			}
			return isAuthenticated;
		}
	}
}


//	string cookieName = FormsAuthentication.FormsCookieName;
//	if (!httpContext.User.Identity.IsAuthenticated ||
//		httpContext.Request.Cookies == null ||
//		httpContext.Request.Cookies[cookieName] == null)
//	{
//		return false;
//	}

//	var authCookie = httpContext.Request.Cookies[cookieName];
//	var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

//	string webServiceToken = authTicket.UserData;

//	IPrincipal userPrincipal = ...create some custom implementation
//								  and store the web service token as property

//// Inject the custom principal in the HttpContext
//	httpContext.User = userPrincipal;
