﻿#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xaml;
using GKWebService.DataProviders.Plan;
using GKWebService.DataProviders.Resources;
using GKWebService.Models.Plan.PlanElement.Hint;
using GKWebService.Utils;
using ImageMagick;
using Infrustructure.Plans.Elements;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

#endregion

namespace GKWebService.Models.Plan.PlanElement
{
	public class PlanElement : Shape
	{
		#region Properties

		public string Name { get; set; }

		public ElementHint Hint { get; set; }

		public IEnumerable<Shape> ChildElements { get; set; }

		#endregion

		#region Methods

		public static bool HasProperty(object objectToCheck, string properyName) {
			var type = objectToCheck.GetType();
			return type.GetProperty(properyName) != null;
		}
		public static object GetProperty(object objectToCheck, string properyName) {
			var type = objectToCheck.GetType();
			return type.GetProperty(properyName).GetValue(objectToCheck);
		}

		public static PlanElement FromRectangle(ElementBaseRectangle elem) {
			if (elem is ElementEllipse) {
				return null;
			}
			bool showState = false;
			if (HasProperty(elem, "ShowState")) {
				showState = (bool)GetProperty(elem, "ShowState");
			}
			if (elem is ElementRectangleGKMPT) {
				showState = true;
			}
			if (!showState) {
				if (elem is ElementRectangle) {
					return FromRectangleSimple(elem, false);
				}
				return FromRectangleSimple(elem, true);
			}
			// Получаем прямоугольник, в который вписан текст
			// Получаем элемент текста
			var textElement = new ElementTextBlock {
				FontBold = true,
				FontFamilyName = "Arial",
				FontItalic = true,
				Text = "Неизвестно",
				FontSize = 18,
				ForegroundColor = Colors.Black,
				WordWrap = false,
				BorderThickness = 1,
				Stretch = true,
				TextAlignment = 1,
				VerticalAlignment = 1,
				PresentationName = elem.PresentationName,
				UID = elem.UID,
				Height = elem.Height,
				Width = elem.Width
			};
			var planElementText = FromTextElement(
				textElement, new System.Windows.Size(elem.Width, elem.Height), elem.Left, elem.Top, false);
			// Получаем элемент прямоугольника, в который вписан текст
			var planElementRect = FromRectangleSimple(elem, false);
			// Очищаем элементы от групповой информации
			planElementText.Hint = null;
			planElementText.HasOverlay = false;
			planElementText.Id = Guid.Empty;
			planElementRect.Hint = null;
			planElementRect.HasOverlay = false;
			planElementRect.Id = Guid.Empty;
			// Задаем групповой элемент
			var planElement = new PlanElement {
				ChildElements = new[] { planElementRect, planElementText },
				Id = elem.UID,
				Hint = GetElementHint(elem),
				Type = ShapeTypes.Group.ToString(),
				Name = elem.PresentationName,
				Width = elem.Width,
				Height = elem.Height,
				HasOverlay = true,
				BorderMouseOver = InernalConverter.ConvertColor(Colors.Orange),
				X = elem.Left,
				Y = elem.Top
			};

			return planElement;
		}

		public static PlanElement FromPolygon(ElementBasePolygon elem)
		{
				

			var rect = elem.GetRectangle();
			bool showState = false;
			if (HasProperty(elem, "ShowState")) {
				showState = (bool)GetProperty(elem, "ShowState");
			}
			if (elem is ElementPolygonGKMPT) {
				showState = true;
			}
			if (!showState) {
				return FromPolygonSimple(elem, !(elem is ElementPolygon));
			}
			// Получаем прямоугольник, в который вписан текст
			// Получаем элемент текста
			var textElement = new ElementTextBlock {
				FontBold = true,
				FontFamilyName = "Arial",
				FontItalic = true,
				Text = "Неизвестно",
				FontSize = 18,
				ForegroundColor = Colors.Black,
				WordWrap = false,
				BorderThickness = 1,
				Stretch = true,
				TextAlignment = 1,
				VerticalAlignment = 1,
				PresentationName = elem.PresentationName,
				UID = elem.UID,
				Height = rect.Height,
				Width = rect.Width
			};
			var planElementText = FromTextElement(
				textElement, new System.Windows.Size(rect.Width, rect.Height), rect.Left, rect.Top, false);
			// Получаем элемент прямоугольника, в который вписан текст
			var planElementRect = FromPolygonSimple(elem, false);
			// Очищаем элементы от групповой информации
			planElementText.Hint = null;
			planElementText.HasOverlay = false;
			planElementText.Id = Guid.Empty;
			planElementRect.Hint = null;
			planElementRect.HasOverlay = false;
			planElementRect.Id = Guid.Empty;
			// Задаем групповой элемент
			var planElement = new PlanElement {
				ChildElements = new[] { planElementRect, planElementText },
				Id = elem.UID,
				Hint = GetElementHint(elem),
				Type = ShapeTypes.Group.ToString(),
				Width = rect.Width,
				Height = rect.Height,
				HasOverlay = true,
				BorderMouseOver = InernalConverter.ConvertColor(Colors.Orange),
				X = rect.Left,
				Y = rect.Top,
				Name = elem.PresentationName
			};

			return planElement;
		}

		private static PlanElement FromRectangleSimple(ElementBaseRectangle elem, bool mouseOver) {
			
				var result = System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
				{
					Debug.WriteLine(string.Format("App thread is {0}, with appartment = {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.GetApartmentState().ToString()));
					return elem.GetRectangle();
				});
			var rect = result;
			var pt = new PointCollection {
				rect.TopLeft,
				rect.TopRight,
				rect.BottomRight,
				rect.BottomLeft
			};

			bool showHint = true;

			if (HasProperty(elem, "ShowTooltip")) {
				showHint = (bool)GetProperty(elem, "ShowTooltip");
			}
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(pt, PathKind.ClosedLine),
				Border = InernalConverter.ConvertColor(elem.BorderColor),
				Fill = InernalConverter.ConvertColor(elem.BackgroundColor),
				BorderMouseOver = InernalConverter.ConvertColor(Colors.Orange),
				FillMouseOver = InernalConverter.ConvertColor(elem.BackgroundColor),
				Name = elem.PresentationName,
				Id = elem.UID,
				Hint = showHint ? GetElementHint(elem) : null,
				BorderThickness = elem.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = mouseOver
			};
			return shape;
		}

		public static PlanElement FromEllipse(ElementEllipse item) {

var result = System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
				{
					Debug.WriteLine(string.Format("App thread is {0}, with appartment = {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.GetApartmentState().ToString()));
					return item.GetRectangle();
				});				
			var rect = result;
			var pt = new PointCollection {
				rect.TopLeft,
				rect.TopRight,
				rect.BottomRight,
				rect.BottomLeft
			};
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(pt, PathKind.Ellipse),
				Border = InernalConverter.ConvertColor(item.BorderColor),
				Fill = InernalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InernalConverter.ConvertColor(item.BorderColor),
				FillMouseOver = InernalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = item.ShowTooltip ? GetElementHint(item) : null,
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};
			return shape;
		}

		public static PlanElement FromPolygonSimple(ElementBasePolygon item, bool mouseOver) {
			bool showHint = true;

			if (HasProperty(item, "ShowTooltip")) {
				showHint = (bool)GetProperty(item, "ShowTooltip");
			}
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(item.Points, PathKind.ClosedLine),
				Border = InernalConverter.ConvertColor(item.BorderColor),
				Fill = InernalConverter.ConvertColor(item.BackgroundColor),
				BorderMouseOver = InernalConverter.ConvertColor(Colors.Orange),
				FillMouseOver = InernalConverter.ConvertColor(item.BackgroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				BorderThickness = item.BorderThickness,
				Hint = showHint ? GetElementHint(item) : null,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = mouseOver
			};
			return shape;
		}

		public static PlanElement FromPolyline(ElementPolyline elem) {
			var shape = new PlanElement {
				Path = InernalConverter.PointsToPath(elem.Points, PathKind.Line),
				Border = InernalConverter.ConvertColor(elem.BorderColor),
				Fill = System.Drawing.Color.Transparent,
				BorderMouseOver = InernalConverter.ConvertColor(elem.BorderColor),
				FillMouseOver = InernalConverter.ConvertColor(elem.BackgroundColor),
				Name = elem.PresentationName,
				Id = elem.UID,
				Hint = elem.ShowTooltip ? GetElementHint(elem) : null,
				BorderThickness = elem.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};
			return shape;
		}

		/// <summary>
		/// Создает SVG-группу из ElementTextBlock
		/// </summary>
		/// <param name="elem">ElementTextBlock</param>
		/// <returns>групповой PlanElement</returns>
		public static PlanElement FromTextBlock(ElementTextBlock elem) {
			// Получаем прямоугольник, в который вписан текст
			// Получаем элемент текста
			var planElementText = FromTextElement(
				elem, new System.Windows.Size(elem.Width, elem.Height), elem.Left, elem.Top, elem.ShowTooltip);
			// Получаем элемент прямоугольника, в который вписан текст
			var planElementRect = FromRectangleSimple(elem, false);
			// Очищаем элементы от групповой информации
			planElementText.Hint = null;
			planElementText.HasOverlay = false;
			planElementText.Id = Guid.Empty;
			planElementRect.Hint = null;
			planElementRect.HasOverlay = false;
			planElementRect.Id = Guid.Empty;
			// Задаем групповой элемент
			var planElement = new PlanElement {
				ChildElements = new[] { planElementRect, planElementText },
				Id = planElementText.Id,
				Hint = GetElementHint(elem),
				Type = ShapeTypes.Group.ToString(),
				Width = elem.Width,
				Height = elem.Height,
				HasOverlay = false,
				X = elem.Left,
				Y = elem.Top
			};

			return planElement;
		}

		/// <summary>
		/// Создает SVG-группу из ElementProcedure
		/// </summary>
		/// <param name="elem">ElementProcedure</param>
		/// <returns></returns>
		public static PlanElement FromProcedure(ElementProcedure elem) {
			// Получаем элемент текста
			var planElementText = FromTextElement(
				elem, new System.Windows.Size(elem.Width, elem.Height), elem.Left, elem.Top, true);
			// Получаем элемент прямоугольника, в который вписан текст

			var planElementRect = FromRectangleSimple(elem, false);
			// Очищаем элементы от групповой информации
			planElementText.Hint = null;
			planElementText.HasOverlay = false;
			planElementRect.Hint = null;
			planElementRect.HasOverlay = false;
			// Задаем групповой элемент
			var planElement = new PlanElement {
				ChildElements = new[] { planElementRect, planElementText },
				Id = planElementText.Id,
				Hint = GetElementHint(elem),
				Type = ShapeTypes.Group.ToString(),
				Width = elem.Width,
				Height = elem.Height,
				Name = elem.PresentationName,
				HasOverlay = true,
				BorderMouseOver = InernalConverter.ConvertColor(Colors.Orange),
				X = elem.Left,
				Y = elem.Top
			};

			return planElement;
		}

		/// <summary>
		/// Рендерит IElementTextBlock
		/// </summary>
		/// <param name="item">Элемент</param>
		/// <param name="size">Размеры контейнера</param>
		/// <param name="left">Координата X</param>
		/// <param name="top">Координата Y</param>
		/// <param name="showHint">Показывать всплывающую подсказку.</param>
		/// <returns></returns>
		private static PlanElement FromTextElement(IElementTextBlock item, System.Windows.Size size, double left, double top, bool showHint) {
			var fontFamily = new System.Windows.Media.FontFamily(item.FontFamilyName);
			var fontStyle = item.FontItalic ? FontStyles.Italic : FontStyles.Normal;
			var fontWeight = item.FontBold ? FontWeights.Bold : FontWeights.Normal;
			var text = new FormattedText(
				item.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
				new Typeface(fontFamily, fontStyle, fontWeight, FontStretches.Normal), item.FontSize, new SolidColorBrush(item.ForegroundColor)) {
				Trimming = TextTrimming.WordEllipsis,
				MaxLineCount = item.WordWrap ? int.MaxValue : 1
			};
			// Делаем первый рендер без трансформаций
			var pathGeometry = text.BuildGeometry(new Point(left + item.BorderThickness, top + item.BorderThickness));
			// Добавляем Scale-трансформацию, если включен Stretch, либо Translate-трансформацию
			if (item.Stretch) {
				var scaleFactorX = (size.Width - item.BorderThickness * 2) / text.Width;
				var scaleFactorY = (size.Height - item.BorderThickness * 2) / text.Height;
				pathGeometry.Transform = new ScaleTransform(
					scaleFactorX, scaleFactorY, left + item.BorderThickness, top + item.BorderThickness);
			}
			else {
				double offsetX = 0;
				double offsetY = 0;
				switch (item.TextAlignment) {
					case 1: {
							offsetX = size.Width - item.BorderThickness * 2 - text.Width;
							break;
						}
					case 2: {
							offsetX = (size.Width - item.BorderThickness * 2 - text.Width) / 2;
							break;
						}
				}
				switch (item.VerticalAlignment) {
					case 1: {
							offsetY = (size.Height - item.BorderThickness * 2 - text.Height) / 2;
							break;
						}
					case 2: {
							offsetY = size.Height - item.BorderThickness * 2 - text.Height;
							break;
						}
				}

				pathGeometry.Transform = new TranslateTransform(offsetX, offsetY);
			}
			// Делаем финальный рендер
			var path = pathGeometry.GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture).Substring(2);

			var shape = new PlanElement {
				Path = path,
				Border = InernalConverter.ConvertColor(Colors.Transparent),
				Fill = InernalConverter.ConvertColor(item.ForegroundColor),
				Name = item.PresentationName,
				Id = item.UID,
				Hint = (item as ElementBase) != null && showHint ? GetElementHint((ElementBase)item) : null,
				BorderThickness = 0,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};
			return shape;
		}

		public static PlanElement FromGkDoor(ElementGKDoor item) {
			var strings = EmbeddedResourceLoader.LoadResource("GKWebService.Content.SvgIcons.GKDoor.txt")
												.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
var result = System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
				{
					Debug.WriteLine(string.Format("App thread is {0}, with appartment = {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.GetApartmentState().ToString()));
					return item.GetRectangle();
			});
			var rect = result;
			var geometry = Geometry.Parse(strings[0]);
			var flatten = geometry.GetFlattenedPathGeometry();
			flatten.Transform = new TranslateTransform(rect.TopLeft.X, rect.TopRight.Y);
			KnownColor color1;
			Enum.TryParse(strings[1], true, out color1);
			KnownColor color2;
			Enum.TryParse(strings[2], true, out color2);
			var shape1 = new PlanElement {
				Path = flatten.GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture),
				Border = System.Drawing.Color.FromKnownColor(color1),
				Fill = System.Drawing.Color.FromKnownColor(color2),
				BorderMouseOver = InernalConverter.ConvertColor(Colors.Transparent),
				FillMouseOver = InernalConverter.ConvertColor(Colors.Transparent),
				Name = item.PresentationName,
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};
			geometry = Geometry.Parse(strings[3]);
			flatten = geometry.GetFlattenedPathGeometry();
			flatten.Transform = new TranslateTransform(rect.TopLeft.X, rect.TopRight.Y);
			Enum.TryParse(strings[4], true, out color1);
			Enum.TryParse(strings[5], true, out color2);

			var shape2 = new PlanElement {
				Path = flatten.GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture),
				Border = System.Drawing.Color.FromKnownColor(color1),
				Fill = System.Drawing.Color.FromKnownColor(color2),
				BorderMouseOver = InernalConverter.ConvertColor(Colors.Transparent),
				FillMouseOver = InernalConverter.ConvertColor(Colors.Transparent),
				Name = item.PresentationName,
				BorderThickness = item.BorderThickness,
				Type = ShapeTypes.Path.ToString(),
				HasOverlay = false
			};

			var planElement = new PlanElement {
				ChildElements = new[] { shape1, shape2 },
				Id = item.UID,
				Hint = GetElementHint(item),
				Type = ShapeTypes.Group.ToString(),
				Width = 30,
				Height = 30,
				Name = item.PresentationName,
				HasOverlay = true,
				BorderMouseOver = InernalConverter.ConvertColor(Colors.Orange),
				X = rect.Left,
				Y = rect.Top
			};
			return planElement;
		}

		public static PlanElement FromDevice(ElementGKDevice item) {
			// Находим девайс и набор его состояний
			var device =
				GKManager.Devices.FirstOrDefault(d => d.UID == item.DeviceUID);
			if (device == null) {
				return null;
			}

			// Создаем элемент плана
			// Ширину и высоту задаем 500, т.к. знаем об этом
			var shape = new PlanElement {
				Id = item.DeviceUID,
				SubElementId = item.DeviceUID.ToString(),
				Name = item.PresentationName,
				Image = GetDeviceStatePic(device, device.State),
				X = item.Left - 7,
				Y = item.Top - 7,
				Height = 14,
				Width = 14,
				Type = ShapeTypes.GkDevice.ToString(),
				HasOverlay = false
			};
			var planElement = new PlanElement {
				ChildElements = new[] { shape },
				Id = item.DeviceUID,
				SubElementId = item.DeviceUID.ToString() + "GroupElement",
				Hint = GetElementHint(item),
				Type = ShapeTypes.Group.ToString(),
				Width = 14,
				Height = 14,
				HasOverlay = true,
				Name = item.PresentationName,
				BorderMouseOver = InernalConverter.ConvertColor(Colors.Orange),
				X = item.Left - 7,
				Y = item.Top - 7,
			};
			return planElement;
		}

		public static void UpdateDeviceState(GKState state) {
			if (state.BaseObjectType != GKBaseObjectType.Device) {
				return;
			}
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == state.UID);
			// Получаем обновленную картинку устройства
			var getPictureTask = Task.Factory.StartNewSta(()=> GetDeviceStatePic(device, state));
			Task.WaitAll();
			var pic = getPictureTask.Result;
			if (pic == null) {
				return;
			}

			// Получаем названия для состояний
			var stateClasses = new string[state.StateClasses.Count];
			for (var index = 0; index < state.StateClasses.Count; index++) {
				var stateClass = state.StateClasses[index];
				stateClasses[index] = InernalConverter.GetStateClassName(stateClass);
			}

			ElementGKDevice elemDevice = null;

			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
			{
				foreach (var elementGkDevice in plan.ElementGKDevices)
				{
					if (elementGkDevice.UID == state.UID)
					{
						elemDevice = elementGkDevice;
						break;
					}
				}
			}

			// Собираем обновление для передачи
			var statusUpdate = new {
				Id = state.UID,
				Picture = pic,
				Name = elemDevice != null ? elemDevice.PresentationName : device.PresentationName,
				HintPic = elemDevice != null ? GetElementHintIcon(elemDevice).Item1 : null,
				StateClass = InernalConverter.GetStateClassName(state.StateClass),
				StateClasses = stateClasses,
				state.AdditionalStates
			};
			PlansUpdater.Instance.UpdateDeviceState(statusUpdate);
		}

		private static string GetDeviceStatePic(GKDevice device, GKState state) {
			var deviceConfig =
				GKManager.DeviceLibraryConfiguration.GKDevices.FirstOrDefault(d => d.DriverUID == device.DriverUID);
			if (deviceConfig == null) {
				return null;
			}
			var stateWithPic =
				deviceConfig.States.FirstOrDefault(s => s.StateClass == state.StateClass) ??
				deviceConfig.States.FirstOrDefault(s => s.StateClass == XStateClass.No);
			if (stateWithPic == null) {
				return null;
			}

			// Перебираем кадры в состоянии и генерируем gif картинку
			byte[] bytes;
			using (var collection = new MagickImageCollection()) {
				foreach (var frame in stateWithPic.Frames) {
					var frame1 = frame;
					Canvas surface;
					var imageBytes = Encoding.Unicode.GetBytes(frame1.Image ?? "");
					using (var stream = new MemoryStream(imageBytes)) {
						surface = (Canvas)XamlServices.Load(stream);
					}
					var pngBitmap = surface != null ? InernalConverter.XamlCanvasToPngBitmap(surface) : null;
					if (pngBitmap == null) {
						continue;
					}
					var img = new MagickImage(pngBitmap) {
						AnimationDelay = frame.Duration / 10,
						HasAlpha = true,
						AlphaColor = MagickColor.Transparent,
						BackgroundColor = MagickColor.Transparent
					};
					collection.Add(img);
				}
				if (collection.Count == 0) {
					return string.Empty;
				}
				var path = string.Format(@"C:\tmpImage{0}.gif", Guid.NewGuid());
				collection.Write(path);
				using (var fstream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					using (var stream = new MemoryStream()) {
						fstream.CopyTo(stream);
						bytes = stream.ToArray();
					}
				}
				File.Delete(path);
			}
			return Convert.ToBase64String(bytes);
		}

		private static ElementHint GetElementHint(ElementBase element) {
			var hint = new ElementHint();

			var asDoor = element as ElementGKDoor;
			if (asDoor != null) {
				var door = GKManager.Doors.FirstOrDefault(d => d.UID == asDoor.DoorUID);
				if (door != null) {
					if (door.PresentationName != null) {
						hint.StateHintLines.Add(new HintLine { Text = door.PresentationName, Icon = GetElementHintIcon(asDoor).Item1 });
					}
				}
			}

			var asZone = element as IElementZone;
			if (asZone != null) {
				if (element is ElementRectangleGKZone) {
					var zone = GKManager.Zones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null && zone.PresentationName != null) {
						hint.StateHintLines.Add(
							new HintLine {
								Text = zone.PresentationName,
								Icon = new Func<string>(
								() => {
									var imagePath = zone.ImageSource.Replace("/Controls;component/", "");
									var imageData = GetImageResource(imagePath);
									return imageData.Item1;
								}).Invoke()
							});

					}
				}
				if (element is ElementPolygonGKZone) {
					var zone = GKManager.Zones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null) {
						if (zone.PresentationName != null) {
							hint.StateHintLines.Add(
								new HintLine {
									Text = zone.PresentationName,
									Icon = new Func<string>(
									() => {
										var imagePath = zone.ImageSource.Replace("/Controls;component/", "");
										var imageData = GetImageResource(imagePath);
										return imageData.Item1;
									}).Invoke()
								});

						}
					}
				}
				if (element is ElementRectangleGKGuardZone
					|| element is ElementPolygonGKGuardZone) {
					var zone = GKManager.GuardZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null) {
						if (zone.PresentationName != null) {
							hint.StateHintLines.Add(new HintLine {
								Text = zone.PresentationName,
								Icon = new Func<string>(
									() => {
										var imagePath = zone.ImageSource.Replace("/Controls;component/", "");
										var imageData = GetImageResource(imagePath);
										return imageData.Item1;
									}).Invoke()
							});
						}
					}
				}
				if (element is ElementRectangleGKSKDZone
					|| element is ElementPolygonGKSKDZone) {
					var zone = GKManager.SKDZones.FirstOrDefault(z => z.UID == asZone.ZoneUID);
					if (zone != null) {
						if (zone.PresentationName != null) {
							hint.StateHintLines.Add(new HintLine {
								Text = zone.PresentationName,
								Icon = new Func<string>(
									() => {
										var imagePath = zone.ImageSource.Replace("/Controls;component/", "");
										var imageData = GetImageResource(imagePath);
										return imageData.Item1;
									}).Invoke()
							});
						}
					}
				}
			}
			var asMpt = element as IElementMPT;
			if (asMpt != null) {
				var mpt = GKManager.MPTs.FirstOrDefault(m => m.UID == asMpt.MPTUID);
				if (mpt != null) {
					if (mpt.PresentationName != null) {
						hint.StateHintLines.Add(new HintLine {
							Text = mpt.PresentationName,
							Icon = new Func<string>(
									() => {
										var imagePath = "Images/MPT.png";
										var imageData = GetImageResource(imagePath);
										return imageData.Item1;
									}).Invoke()
						});
					}
				}
			}
			var asDelay = element as IElementDelay;
			if (asDelay != null) {
				var delay = GKManager.Delays.FirstOrDefault(m => m.UID == asDelay.DelayUID);
				if (delay != null) {
					if (delay.PresentationName != null) {
						hint.StateHintLines.Add(new HintLine {
							Text = delay.PresentationName,
							Icon = new Func<string>(
									() => {
										var imagePath = delay.ImageSource.Replace("/Controls;component/", "");
										var imageData = GetImageResource(imagePath);
										return imageData.Item1;
									}).Invoke()
						});
					}
				}
			}
			var asDirection = element as IElementDirection;
			if (asDirection != null) {
				var direction = GKManager.Directions.FirstOrDefault(
					d => d.UID == asDirection.DirectionUID);
				if (direction != null) {
					if (direction.PresentationName != null) {
						hint.StateHintLines.Add(new HintLine {
							Text = direction.PresentationName,
							Icon = new Func<string>(
									() => {
										var imagePath = direction.ImageSource.Replace("/Controls;component/", "");
										var imageData = GetImageResource(imagePath);
										return imageData.Item1;
									}).Invoke()
						});
					}
				}
			}
			var asDevice = element as ElementGKDevice;
			if (asDevice != null) {
				var device = GKManager.Devices.FirstOrDefault(
					d => d.UID == asDevice.DeviceUID);
				if (device != null
					&& device.PresentationName != null) {
					hint.StateHintLines.Add(new HintLine { Text = device.PresentationName, Icon = GetElementHintIcon(asDevice).Item1 });
				}
			}
			return hint;
		}
		private static Tuple<string, Size> GetElementHintIcon(ElementBasePoint item) {
			var imagePath = string.Empty;

			var gkDevice = item as ElementGKDevice;
			if (gkDevice != null) {
				var device =
					GKManager.Devices.FirstOrDefault(d => d.UID == gkDevice.DeviceUID);
				if (device == null) {
					return null;
				}

				var deviceConfig =
					GKManager.DeviceLibraryConfiguration.GKDevices.FirstOrDefault(d => d.DriverUID == device.DriverUID);
				if (deviceConfig == null) {
					return null;
				}

				var stateWithPic =
					deviceConfig.States.FirstOrDefault(s => s.StateClass == device.State.StateClass) ??
					deviceConfig.States.FirstOrDefault(s => s.StateClass == XStateClass.No);
				imagePath = device.ImageSource.Replace("/Controls;component/", "");
			}
			var gkDoor = item as ElementGKDoor;
			if (gkDoor != null) {
				var door =
					GKManager.Doors.FirstOrDefault(d => d.UID == gkDoor.DoorUID);
				if (door == null) {
					return null;
				}

				imagePath = door.ImageSource.Replace("/Controls;component/", "");
			}

			var imageData = GetImageResource(imagePath);

			return imageData;
		}

		/// <summary>
		///     Получение иконок для хинтов из ресурсов проекта Controls
		/// </summary>
		/// <param name="resName">Путь к ресурсу формата GKIcons/RSR2_Bush_Fire.png</param>
		/// <returns></returns>
		private static Tuple<string, Size> GetImageResource(string resName) {
			var assembly = Assembly.GetAssembly(typeof(Controls.AlarmButton));
			var name =
				assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".resources", StringComparison.Ordinal));
			var resourceStream = assembly.GetManifestResourceStream(name);
			if (resourceStream == null) {
				return new Tuple<string, Size>("", new Size());
			}
			byte[] values;
			using (var reader = new ResourceReader(resourceStream)) {
				string type;
				reader.GetResourceData(resName.ToLowerInvariant(), out type, out values);
			}

			// Получили массив байтов ресурса, теперь преобразуем его в png bitmap, а потом снова в массив битов
			// уже корректного формата, после чего преобразуем его в base64string для удобства обратного преобразования
			// на клиенте

			const int offset = 4;
			var size = BitConverter.ToInt32(values, 0);
			var value1 = new Bitmap(new MemoryStream(values, offset, size));
			byte[] byteArray;
			using (var stream = new MemoryStream()) {
				value1.Save(stream, ImageFormat.Png);
				stream.Close();

				byteArray = stream.ToArray();
			}

			return new Tuple<string, Size>(Convert.ToBase64String(byteArray), value1.Size);
		}
		#endregion
	}
}
