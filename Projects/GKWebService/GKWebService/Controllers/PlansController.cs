﻿using System;
using System.Linq;
using System.Web.Mvc;
using GKWebService.DataProviders;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Practices.Unity;

namespace GKWebService.Controllers
{
    public class PlansController : Controller
    {
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = int.MaxValue
            };
        }

        public ActionResult GetPlans()
        {
            
            var result = Json(PlansDataProvider.Instance.Plans, JsonRequestBehavior.AllowGet);
            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<PlansRTStatusUpdaterHub>();
            var dependencyResolver = GlobalHost.DependencyResolver;
            var connectionManager = dependencyResolver.Resolve<IConnectionManager>();
            var hubContext = connectionManager.GetHubContext<PlansRTStatusUpdaterHub>();

            hubContext.Clients.All.Test("Message from server", "Successfully connected to SignalR");
           
            return result;
        }

        public ActionResult GetPlan(Guid planGuid)
        {
            var plan =
                PlansDataProvider.Instance.Plans.FirstOrDefault(p => p.Uid == planGuid);


            if (plan != null)
            {
                var result = Json(plan, JsonRequestBehavior.AllowGet);
                return result;
            }
            else return HttpNotFound(string.Format("План с ID {0} не найден", planGuid));
        } 
    }
}