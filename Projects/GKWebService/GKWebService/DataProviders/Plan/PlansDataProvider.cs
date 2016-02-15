﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using GKWebService.Models.Plan;
using GKWebService.Models.Plan.PlanElement;
using GKWebService.Utils;
using Infrastructure.Common.Services.Content;
using Infrustructure.Plans.Elements;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;

#endregion

namespace GKWebService.DataProviders.Plan
{
	/// <summary>
	///     Провайдер планов. Предоставляет планы в готовом для сериализации виде.
	/// </summary>
	public class PlansDataProvider
	{
		/// <summary>
		///     Загрузка корневого элемента плана (фон).
		/// </summary>
		/// <param name="plan">Объект плана.</param>
		/// <returns>Корневой элемент плана.</returns>
		private PlanElement LoadPlanRoot(RubezhAPI.Models.Plan plan) {
			return new PlanElement {
				Border = InternalConverterOld.ConvertColor(Colors.Black),
				BorderThickness = 0,
				Fill = InternalConverterOld.ConvertColor(plan.BackgroundColor),
				Id = plan.UID,
				Name = plan.Caption,
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

		/// <summary>
		///     Загрузка всех элементов плана (помимо корневого).
		/// </summary>
		/// <param name="plan">Объект плана.</param>
		/// <returns>Коллекция элементов плана.</returns>
		private IEnumerable<PlanElement> LoadPlanSubElements(RubezhAPI.Models.Plan plan) {
			//var rectangles = LoadRectangleElements(plan);
			//var polygons = LoadPolygonElements(plan);
			//var polylines = LoadPolyLineElements(plan);
			//var ellipses = LoadEllipseElements(plan);
			//var textBlocks = LoadStaticTextElements(plan);
			//var doors = LoadDoorElements(plan);
			var devices = LoadDeviceElements(plan);
			//return textBlocks.Concat(rectangles).Concat(ellipses).Concat(doors).Concat(devices);


			return devices;
		}

		/// <summary>
		///     Загрузка всех прямоугольных элементов плана, базирующихся на ElementBaseRectangle, кроме тех, которые реализуют
		///     IElementTextBlock
		/// </summary>
		/// <param name="plan">Объект плана.</param>
		/// <returns>Коллекция элементов плана.</returns>
		private IEnumerable<PlanElement> LoadRectangleElements(RubezhAPI.Models.Plan plan) {
			return
				plan.AllElements.Where(elem => elem is ElementBaseRectangle && !(elem is IElementTextBlock))
				    .Select(elem => PlanElement.FromRectangle(elem as ElementBaseRectangle))
				    .Where(elem => elem != null);
		}

		private IEnumerable<PlanElement> LoadPolyLineElements(RubezhAPI.Models.Plan plan) {
			return plan.ElementPolylines.Select(PlanElement.FromPolyline);
		}

		private IEnumerable<PlanElement> LoadPolygonElements(RubezhAPI.Models.Plan plan) {
			return plan.AllElements.Where(elem => elem is ElementBasePolygon).Select(elem => PlanElement.FromPolygon(elem as ElementBasePolygon));
		}

		private IEnumerable<PlanElement> LoadEllipseElements(RubezhAPI.Models.Plan plan) {
			return plan.ElementEllipses.Select(PlanElement.FromEllipse);
		}

		/// <summary>
		///     Преобразует статические текcтовые элементы
		///     (ElementTextBlock, ElementProcedure).
		/// </summary>
		/// <param name="plan">План</param>
		/// <returns>Элемент-группа, содержащий групповой элемент, внутри которого текст и прямоугольник.</returns>
		private IEnumerable<PlanElement> LoadStaticTextElements(RubezhAPI.Models.Plan plan) {
			var textBlockElements = plan.ElementTextBlocks;
			var procedureElements = plan.AllElements.OfType<ElementProcedure>();
			return textBlockElements.Select(
				PlanElement.FromTextBlock)
			                        .Where(elem => elem != null)
			                        .Union(procedureElements.Select(PlanElement.FromProcedure).Where(elem => elem != null));
		}

		private IEnumerable<PlanElement> LoadDeviceElements(RubezhAPI.Models.Plan plan) {
			return plan.ElementGKDevices.Select(PlanElement.FromDevice);
		}

		private IEnumerable<PlanElement> LoadDoorElements(RubezhAPI.Models.Plan plan) {
			return plan.ElementGKDoors.ToList().Select(PlanElement.FromGkDoor);
		}

		/// <summary>
		/// Обработка событий изменения состояния элементов.
		/// </summary>
		/// <param name="obj">Информация об изменении состояния.</param>
		private void OnServiceCallback(GKCallbackResult obj) {
			var states = obj.GKStates;
			Parallel.ForEach(states.DeviceStates, PlanElement.UpdateDeviceState);
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
			Drawing drawing = null;
			Canvas canvas = null;
			if (source.HasValue) {
				try {
					drawing = _contentService.GetDrawing(source.Value);
				}
				catch (Exception) {
					canvas = _contentService.GetObject<Canvas>(source.Value);
					if (canvas == null) {
						return string.Empty;
					}
				}
			}
			else {
				return string.Empty;
			}
			if (drawing == null) {
				return canvas == null ? string.Empty : InternalConverter.XamlCanvasToPngBase64(canvas, width, height);
			}
			drawing.Freeze();

			return InternalConverterOld.XamlDrawingToPngBase64String(width, height, drawing);
		}

		#region Deferred Loading

		/// <summary>
		///     Получить список планов в текущей конфигурации.
		/// </summary>
		/// <returns>Иерархический список планов.</returns>
		public IEnumerable<PlanSimpl> GetPlansList() {
			if (ClientManager.PlansConfiguration == null
			    || ClientManager.PlansConfiguration.Plans == null) {
				return null;
			}
			var plans = ClientManager.PlansConfiguration.Plans;
			return plans.Select(GetPlanInfo).ToList();
		}

		/// <summary>
		///     Рекурсивно получить основную информацию о плане и вложенных планах.
		/// </summary>
		/// <param name="plan">Объект плана.</param>
		/// <returns>Основная информация о плане, включая вложенные планы.</returns>
		private PlanSimpl GetPlanInfo(RubezhAPI.Models.Plan plan) {
			// Корень плана
			return new PlanSimpl {
				Name = plan.Caption,
				Uid = plan.UID,
				Description = plan.Description,
				Width = plan.Width,
				Height = plan.Height,
				NestedPlans = plan.Children != null ? plan.Children.Select(GetPlanInfo) : null,
				ParentUid = plan.Parent != null ? plan.Parent.UID : (Guid?)null,
				IsFolder = plan is PlanFolder
			};
		}

		/// <summary>
		///     Полная загрузка плана.
		/// </summary>
		/// <param name="planId">UID плана.</param>
		/// <returns>Готовая к сериализации полная информация о плане.</returns>
		public PlanSimpl GetPlan(Guid planId) {
			var plan = ClientManager.PlansConfiguration.Plans.FirstOrDefault(p => p.UID == planId);
			// Корень плана
			if (plan == null) {
				throw new KeyNotFoundException(string.Format("План с ID {0} не найден, либо недоступен.", planId));
			}

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

			return planToAdd;
		}

		#endregion

		#region ctor, props

		private static PlansDataProvider _instance;
		private readonly ContentService _contentService;

		private PlansDataProvider() {
			_contentService = new ContentService("Sergey_GKOPC");
			SafeFiresecService.GKCallbackResultEvent += OnServiceCallback;
		}

		public static PlansDataProvider Instance {
			get {
				if (_instance != null) {
					return _instance;
				}
				return _instance = new PlansDataProvider();
			}
		}

		#endregion
	}
}