﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace GKWebService.DataProviders
{
	internal static class InernalConverter
	{
		/// <summary>
		///     Преобразует XAML Drawing/DrawingGroup в png
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="drawing"></param>
		/// <returns></returns>
		public static byte[] XamlDrawingToPngImageBytes(int width, int height, Drawing drawing) {
			var bitmapEncoder = new PngBitmapEncoder();

			// The image parameters...
			double dpiX = 96;
			double dpiY = 96;

			// The Visual to use as the source of the RenderTargetBitmap.
			var drawingVisual = new DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen()) {
				drawingContext.DrawDrawing(drawing);
			}

			var bounds = drawingVisual.ContentBounds;

			var targetBitmap = new RenderTargetBitmap(
				width * 10, height * 10, dpiX, dpiY,
				PixelFormats.Pbgra32);
			drawingVisual.Transform = new ScaleTransform(width * 10 / bounds.Width, height * 10 / bounds.Height);

			targetBitmap.Render(drawingVisual);

			// Encoding the RenderBitmapTarget as an image.
			bitmapEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));

			byte[] values;
			using (var str = new MemoryStream()) {
				bitmapEncoder.Save(str);
				values = str.ToArray();
			}
			return values;
		}

		/// <summary>
		///     Преобразует Xaml-canvas в png изображение
		/// </summary>
		/// <param name="surface">XAML Canvas</param>
		/// <returns>Bitmap, rendered from XAML</returns>
		public static Bitmap XamlCanvasToPngBitmap(Canvas surface) {
			// Save current canvas transform
			var transform = surface.LayoutTransform;
			// reset current transform (in case it is scaled or rotated)
			surface.LayoutTransform = null;

			// Get the size of canvas
			var size = new System.Windows.Size(surface.Width, surface.Height);
			// Measure and arrange the surface
			// VERY IMPORTANT
			surface.Measure(size);
			surface.Arrange(new Rect(size));

			// Create a render bitmap and push the surface to it
			var renderBitmap =
				new RenderTargetBitmap(
					(int)size.Width,
					(int)size.Height,
					96d,
					96d,
					PixelFormats.Pbgra32);
			renderBitmap.Render(surface);

			Bitmap bmp;
			// Create a file stream for saving image
			using (var stream = new MemoryStream()) {
				// Use png encoder for our data
				var encoder = new PngBitmapEncoder();
				// push the rendered bitmap to it
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				// save the data to the stream
				encoder.Save(stream);

				bmp = new Bitmap(stream);
			}
			return bmp;
		}

		public static string PointsToPath(PointCollection points, bool isClosed) {
			var enumerable = points.ToArray();
			if (!enumerable.Any()) {
				return string.Empty;
			}

			var start = enumerable[0];
			var segments = new List<LineSegment>();
			for (var i = 1; i < enumerable.Length; i++) {
				segments.Add(new LineSegment(new Point(enumerable[i].X, enumerable[i].Y), true));
			}
			var figure = new PathFigure(new Point(start.X, start.Y), segments, isClosed);
			var geometry = new PathGeometry();
			geometry.Figures.Add(figure);
			return geometry.ToString();
		}

		public static MemoryStream GenerateStreamFromString(string value) {
			return new MemoryStream(Encoding.Unicode.GetBytes(value ?? ""));
		}

		public static object XamlFromString(string source) {
			return XamlServices.Load(GenerateStreamFromString(source));
		}

		public static System.Drawing.Color ConvertColor(Color source) {
			return System.Drawing.Color.FromArgb(source.A, source.R, source.G, source.B);
		}
	}
}