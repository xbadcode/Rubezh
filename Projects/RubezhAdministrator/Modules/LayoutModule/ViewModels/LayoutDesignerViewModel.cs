﻿using Common;
using Controls.Extentions;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models.Layouts;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace LayoutModule.ViewModels
{
	public class LayoutDesignerViewModel : BaseViewModel
	{
		public static LayoutDesignerViewModel Instance { get; private set; }

		Layout _layout;
		XmlLayoutSerializer _serializer;
		bool _loading;
		bool _currentLayoutChanged;
		public LayoutDesignerViewModel(LayoutElementsViewModel layoutElementsViewModel)
		{
			_currentLayoutChanged = false;
			_loading = false;
			Instance = this;
			LayoutElementsViewModel = layoutElementsViewModel;
			Update();
			LayoutParts = new ObservableCollection<LayoutPartViewModel>();
		}

		public LayoutElementsViewModel LayoutElementsViewModel { get; private set; }

		ObservableCollection<LayoutPartViewModel> _layoutParts;
		public ObservableCollection<LayoutPartViewModel> LayoutParts
		{
			get { return _layoutParts; }
			set
			{
				if (_layoutParts != null)
					_layoutParts.CollectionChanged -= LayoutPartsChanged;
				_layoutParts = value;
				_layoutParts.CollectionChanged += LayoutPartsChanged;
				OnPropertyChanged(() => LayoutParts);
			}
		}

		LayoutPartViewModel _activeLayoutPart;
		public LayoutPartViewModel ActiveLayoutPart
		{
			get { return _activeLayoutPart; }
			set
			{
				_activeLayoutPart = value;
				OnPropertyChanged(() => ActiveLayoutPart);
			}
		}

		DockingManager _manager;
		public DockingManager Manager
		{
			get { return _manager; }
			set
			{
				if (Manager != null)
				{
					Manager.DocumentClosing -= LayoutPartClosing;
					Manager.LayoutConfigurationChanged -= LayoutConfigurationChanged;
				}
				_manager = value;
				if (_serializer != null)
				{
					_serializer.LayoutSerializationCallback -= LayoutSerializationCallback;
					_serializer = null;
				}
				if (_manager != null)
					using (new TimeCounter("Attach to LayoutManager = {0}"))
					{
						Manager.LayoutConfigurationChanged += LayoutConfigurationChanged;
						Manager.DocumentClosing += LayoutPartClosing;
						Manager.LayoutUpdateStrategy = new LayoutUpdateStrategy();
						_serializer = new XmlLayoutSerializer(Manager);
						_serializer.LayoutSerializationCallback += LayoutSerializationCallback;
						if (_layout != null)
							Update(_layout);
					}
			}
		}

		public void Update(Layout layout)
		{
			using (new WaitWrapper())
			{
				if (_layout != null)
					SaveLayout();
				_layout = layout;
				_currentLayoutChanged = false;
				if (_layout != null && Manager != null)
				{
					var layoutParts = new ObservableCollection<LayoutPartViewModel>();
					foreach (var layoutPart in _layout.Parts)
						layoutParts.Add(new LayoutPartViewModel(layoutPart));
					LayoutParts = layoutParts;
					_loading = true;
					Manager.GridSplitterWidth = _layout.SplitterSize;
					Manager.GridSplitterHeight = _layout.SplitterSize;
					Manager.GridSplitterBackground = new SolidColorBrush(_layout.SplitterColor.ToWindowsColor());
					Manager.BorderBrush = new SolidColorBrush(_layout.BorderColor.ToWindowsColor());
					Manager.BorderThickness = new Thickness(_layout.BorderThickness);
					Manager.Background = new SolidColorBrush(_layout.BackgroundColor.ToWindowsColor());
					Manager.Padding = new Thickness(_layout.Padding);
					Manager.Layout = new LayoutRoot();
					if (!string.IsNullOrEmpty(_layout.Content))
						using (var tr = new StringReader(_layout.Content))
							_serializer.Deserialize(tr);
					_loading = false;
					ActiveLayoutPart = LayoutParts.FirstOrDefault();
				}
			}
		}
		public void Update()
		{

		}
		public void SaveLayout()
		{
			if (_layout != null && _currentLayoutChanged)
				using (var tw = new StringWriter())
				{
					_serializer.Serialize(tw);
					_layout.Content = tw.ToString();
				}
		}
		public void LayoutConfigurationChanged(bool withLayout = true)
		{
			if (!_loading)
			{
				ServiceFactory.SaveService.LayoutsChanged = true;
				if (withLayout)
					_currentLayoutChanged = true;
			}
		}

		void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(e.Model.ContentId))
				e.Content = LayoutParts.FirstOrDefault(item => item.UID == Guid.Parse(e.Model.ContentId));
		}

		void LayoutConfigurationChanged(object sender, EventArgs e)
		{
			LayoutConfigurationChanged();
		}

		void LayoutPartClosing(object sender, DocumentClosingEventArgs e)
		{
			var layoutPartViewModel = e.Document.Content as LayoutPartViewModel;
			if (layoutPartViewModel != null)
			{
				LayoutParts.Remove(layoutPartViewModel);
				e.Cancel = true;
			}
		}

		void LayoutPartsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (LayoutPartViewModel layoutPartViewModel in e.NewItems)
					{
						_layout.Parts.Add(layoutPartViewModel.LayoutPart);
						layoutPartViewModel.LayoutPartDescriptionViewModel.Count++;
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					if (_layout != null)
					{
						foreach (LayoutPartViewModel layoutPartViewModel in e.OldItems)
						{
							_layout.Parts.Remove(layoutPartViewModel.LayoutPart);
							layoutPartViewModel.LayoutPartDescriptionViewModel.Count--;
						}
					}
					break;
			}
		}

		public void AddLayoutPart(LayoutPartDescriptionViewModel layoutPartDescriptionViewModel, bool dragging)
		{
			var layoutPartViewModel = new LayoutPartViewModel(layoutPartDescriptionViewModel);
			LayoutParts.Add(layoutPartViewModel);
			ActiveLayoutPart = layoutPartViewModel;
			if (dragging)
				Manager.StartDragging(layoutPartViewModel);
		}
	}
}