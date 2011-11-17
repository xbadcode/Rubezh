﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrastructure;
using PlansModule.Events;
using FiresecAPI.Models;

namespace PlansModule.Designer
{
    public class PolygonResizeChrome : Canvas
    {
        public static PolygonResizeChrome Current { get; private set; }
        static List<PolygonResizeChrome> Polygons = new List<PolygonResizeChrome>();

        ContentControl _designerItem;
        Polygon _polygon;
        List<Thumb> thumbs = new List<Thumb>();
        List<ElementBase> initialElements;

        public static void ClearActivePolygons()
        {
            Polygons.Clear();
        }
        public static void ResetActivePolygons()
        {
            foreach (var polygonResizeChrome in Polygons)
            {
                polygonResizeChrome.Initialize();
            }
        }

        static PolygonResizeChrome()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PolygonResizeChrome), new FrameworkPropertyMetadata(typeof(PolygonResizeChrome)));
        }

        public PolygonResizeChrome(ContentControl designerItem)
        {
            Current = this;
            Polygons.Add(this);

            _designerItem = designerItem;
            _polygon = designerItem.Content as Polygon;

            Initialize();
        }

        public void Initialize()
        {
            ArrangeSize();

            this.Children.Clear();
            thumbs.Clear();

            foreach (var point in _polygon.Points)
            {
                var thumb = new Thumb()
                {
                    Width = 10,
                    Height = 10,
                    Margin = new Thickness(-5, -5, 0, 0),
                    Focusable = true
                };
                Canvas.SetLeft(thumb, point.X);
                Canvas.SetTop(thumb, point.Y);
                this.Children.Add(thumb);
                thumb.DragStarted += new DragStartedEventHandler(thumb_DragStarted);
                thumb.DragDelta += new DragDeltaEventHandler(_thumb_DragDelta);
                thumb.DragCompleted += new DragCompletedEventHandler(thumb_DragCompleted);
                thumb.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(thumb_PreviewKeyDown);
                thumbs.Add(thumb);
            }
        }

        void thumb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var thumb = sender as Thumb;
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                thumbs.Remove(thumb);
                ResetPolygonPoints();
                PlansModule.HasChanges = true;
                Initialize();
            }
        }

        void thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            DesignerItem designerItem = _designerItem as DesignerItem;
            ElementBasePolygon elementPolygon = designerItem.ElementBase as ElementBasePolygon;

            initialElements = new List<ElementBase>();
            elementPolygon.Left = Canvas.GetLeft(designerItem);
            elementPolygon.Top = Canvas.GetTop(designerItem);
            elementPolygon.PolygonPoints = new System.Windows.Media.PointCollection((designerItem.Content as Polygon).Points);
            initialElements.Add(elementPolygon.Clone());
        }

        void thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ServiceFactory.Events.GetEvent<ElementPositionChangedEvent>().Publish(_designerItem);
            ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(initialElements);
        }

        private void _thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var currentThumb = sender as Thumb;

            Canvas.SetLeft(currentThumb, Canvas.GetLeft(currentThumb) + e.HorizontalChange);
            Canvas.SetTop(currentThumb, Canvas.GetTop(currentThumb) + e.VerticalChange);

            ArrangeSize();
            ResetPolygonPoints();
            PlansModule.HasChanges = true;
        }

        void ResetPolygonPoints()
        {
            _polygon.Points.Clear();
            foreach (var thumb in thumbs)
            {
                _polygon.Points.Add(new Point(Canvas.GetLeft(thumb), Canvas.GetTop(thumb)));
            }
        }

        public void ArrangeSize()
        {
            double minLeft = double.MaxValue;
            double minTop = double.MaxValue;
            double maxLeft = 0;
            double maxTop = 0;
            foreach (var point in _polygon.Points)
            {
                minLeft = Math.Min(point.X, minLeft);
                minTop = Math.Min(point.Y, minTop);
                maxLeft = Math.Max(point.X, maxLeft);
                maxTop = Math.Max(point.Y, maxTop);
            }

            Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) + minLeft);
            Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) + minTop);

            foreach (var thumb in thumbs)
            {
                Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) - minLeft);
                Canvas.SetTop(thumb, Canvas.GetTop(thumb) - minTop);
            }

            var points = new PointCollection();
            foreach (var point in _polygon.Points)
            {
                points.Add(new Point(point.X - minLeft, point.Y - minTop));
            }
            _polygon.Points = points;

            _designerItem.Width = maxLeft - minLeft;
            _designerItem.Height = maxTop - minTop;
        }
    }
}
