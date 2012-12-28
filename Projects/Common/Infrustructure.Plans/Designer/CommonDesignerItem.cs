﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using System.Windows.Media;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerItem : ContentControl, INotifyPropertyChanged
	{
		public const int BigConstatnt = 100000;

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public event EventHandler ItemPropertyChanged;

		public ElementBase Element { get; protected set; }
		public IPainter Painter { get; protected set; }
		public Visual Presenter { get; protected set; }

		public event Action<CommonDesignerItem> UpdateProperties;

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}

		public CommonDesignerItem(ElementBase element)
		{
			ResetElement(element);
			ContextMenuOpening += (s, e) => CreateContextMenu();
		}

		public virtual void ResetElement(ElementBase element)
		{
			Element = element;
			DataContext = Element;
			Painter = PainterFactory.Create(Element);
		}

		public virtual void SetLocation()
		{
			var rect = Element.GetRectangle();
			if (ItemWidth != rect.Width)
				ItemWidth = rect.Width;
			if (ItemHeight != rect.Height)
				ItemHeight = rect.Height;
			if (Canvas.GetLeft(this) != rect.Left)
				Canvas.SetLeft(this, rect.Left);
			if (Canvas.GetTop(this) != rect.Top)
				Canvas.SetTop(this, rect.Top);
		}
		public virtual void Redraw()
		{
			RedrawContent();
			SetLocation();
		}
		public void RedrawContent()
		{
			MinWidth = Element.BorderThickness;
			MinHeight = Element.BorderThickness;
			if (Element is ElementBaseShape)
			{
				MinWidth += 3;
				MinHeight += 3;
			}
			Presenter = Painter == null ? null : Painter.Draw(Element);
			OnPropertyChanged("Content");
		}
		public void SetZIndex()
		{
			Panel.SetZIndex(this, Element.ZIndex + Element.ZLayer * BigConstatnt);
		}

		public virtual double ItemWidth
		{
			get { return Width - Element.BorderThickness; }
			set { Width = value + Element.BorderThickness; }
		}
		public virtual double ItemHeight
		{
			get { return Height - Element.BorderThickness; }
			set { Height = value + Element.BorderThickness; }
		}

		public virtual void UpdateElementProperties()
		{
			OnUpdateProperties();
		}
		protected void OnUpdateProperties()
		{
			if (UpdateProperties != null)
				UpdateProperties(this);
		}

		protected abstract void CreateContextMenu();

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		protected void OnDesignerItemPropertyChanged()
		{
			if (ItemPropertyChanged != null)
				ItemPropertyChanged(this, EventArgs.Empty);
		}
	}
}