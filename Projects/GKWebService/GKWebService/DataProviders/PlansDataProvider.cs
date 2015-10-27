﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GKWebService.Models;
using Infrastructure.Common.Services.Content;
using Infrustructure.Plans.Elements;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;

#endregion

namespace GKWebService.DataProviders
{
	public class PlansDataProvider
	{
		public void LoadPlans() {
			if (ClientManager.PlansConfiguration == null
			    || ClientManager.PlansConfiguration.Plans == null) {
				return;
			}
			var plans = ClientManager.PlansConfiguration.Plans;
			Plans = new List<PlanSimpl>();
			foreach (var plan in plans) {
				LoadPlan(plan);
			}
			SafeFiresecService.GKCallbackResultEvent += OnServiceCallback;
		}

		private void LoadPlan(Plan plan) {
			// Корень плана
			var planToAdd = new PlanSimpl {
				Name = plan.Caption,
				Uid = plan.UID,
				Description = plan.Description,
				Width = plan.Width,
				Height = plan.Height,
				Elements = new List<PlanElement>()
			};

			// Добавляем сам план с фоном и все элементы
			var planRootElement = LoadPlanRoot(plan);
			planToAdd.Elements.Add(planRootElement);
			var planSubElements = LoadPlanSubElements(plan);
			planToAdd.Elements.AddRange(planSubElements);

			Plans.Add(planToAdd);
		}

		private PlanElement LoadPlanRoot(Plan plan) {
			return new PlanElement {
				Border = InernalConverter.ConvertColor(Colors.Black),
				BorderThickness = 0,
				Fill = InernalConverter.ConvertColor(plan.BackgroundColor),
				Id = plan.UID,
				Name = plan.Caption,
				Hint = plan.Description,
				Path =
					"M 0 0 L " + plan.Width + " 0 L " + plan.Width +
					" " + plan.Height +
					" L 0 " + plan.Height + " L 0 0 z",
				Type = ShapeTypes.Plan.ToString(),
				Image = RenderPlanBackgound(
					plan.BackgroundImageSource,
					Convert.ToInt32(plan.Width),
					Convert.ToInt32(plan.Height)),
				Width = plan.Width,
				Height = plan.Height
			};
		}

		private IEnumerable<PlanElement> LoadPlanSubElements(Plan plan) {
			var rectangles = LoadRectangleElements(plan);
			var polygons = LoadPolygonElements(plan);
			var polylines = LoadPolyLineElements(plan);
			//var doors = LoadDoorElements(plan);
			var devices = plan.ElementGKDevices.Select(PlanElement.FromDevice);
			return rectangles.Concat(polygons).Concat(polylines).Concat(devices);
		}

		private IEnumerable<PlanElement> LoadPolygonElements(Plan plan) {
			var polygons =
				(from rect in plan.ElementPolygons
				 select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKZones
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKDelays
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKDirections
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKGuardZones
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKMPTs
						select rect as ElementBasePolygon)
					.Union
					(
						from rect in plan.ElementPolygonGKSKDZones
						select rect as ElementBasePolygon);

			// Конвертим зоны-полигоны
			return polygons.Select(PlanElement.FromPolygon);
		}

		private IEnumerable<PlanElement> LoadRectangleElements(Plan plan) {
			var rectangles =
				(from rect in plan.ElementRectangles
				 select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKZones
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKDelays
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKDirections
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKGuardZones
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKMPTs
						select rect as ElementBaseRectangle)
					.Union
					(
						from rect in plan.ElementRectangleGKSKDZones
						select rect as ElementBaseRectangle);

			// Конвертим зоны-прямоугольники
			return rectangles.ToList().Select(PlanElement.FromRectangle);
		}

		private IEnumerable<PlanElement> LoadPolyLineElements(Plan plan) {
			var polylines = (from line in plan.ElementPolylines
			                 select line as ElementBasePolyline);

			// Конвертим зоны-полигоны
			return polylines.Select(PlanElement.FromPolyline);
		}

		private void OnServiceCallback(GKCallbackResult obj) {
			var states = obj.GKStates;
			foreach (var state in states.DeviceStates) {
				PlanElement.UpdateDeviceState(state);
			}
			//foreach (var state in states.DelayStates) {
			//}
			//foreach (var state in states.DirectionStates) {
			//}
			//foreach (var state in states.DoorStates) {
			//}
			//foreach (var state in states.GuardZoneStates) {
			//}
			//foreach (var state in states.MPTStates) {
			//}
			//foreach (var state in states.PumpStationStates) {
			//}
			//foreach (var state in states.SKDZoneStates) {
			//}
			//foreach (var state in states.ZoneStates) {
			//}
		}

		/// <summary>
		///     Получает преобразованное в Base64String png-изображение фона плана
		/// </summary>
		/// <param name="source">GUID плана</param>
		/// <param name="width">Ширина плана</param>
		/// <param name="height">Высота плана</param>
		/// <returns></returns>
		private string RenderPlanBackgound(Guid? source, int width, int height) {
			Drawing drawing;
			if (source.HasValue) {
				drawing = _contentService.GetDrawing(source.Value);
			}
			else {
				return string.Empty;
			}
			drawing.Freeze();

			return InernalConverter.XamlDrawingToPngBase64String(width, height, drawing);
		}

		#region ctor, props

		private static PlansDataProvider _instance;
		private readonly ContentService _contentService;

		private PlansDataProvider() {
			Plans = new List<PlanSimpl>();
			_contentService = new ContentService("GKOPC");
		}

		public static PlansDataProvider Instance {
			get {
				if (_instance != null) {
					return _instance;
				}
				return _instance = new PlansDataProvider();
			}
		}

		public List<PlanSimpl> Plans { get; set; }

		#endregion
	}
}
