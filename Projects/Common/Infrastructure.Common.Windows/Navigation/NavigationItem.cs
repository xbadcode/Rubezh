﻿using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Infrastructure.Common.Navigation
{
	public class NavigationItem : BaseViewModel
	{
		public ReadOnlyCollection<NavigationItem> Childs { get; private set; }
		public ShellViewModel Context { get; set; }

		public NavigationItem(string title, string icon = null, IList<NavigationItem> childs = null, PermissionType? permission = null)
		{
			Title = title;
			Icon = icon;
			Childs = new ReadOnlyCollection<NavigationItem>(childs ?? new List<NavigationItem>());
			PermissionPredicate = null;
			IsVisible = true;
			IsSelectionAllowed = false;
			SupportMultipleSelect = true;
		}

		public virtual void Execute()
		{
		}

		public Predicate<NavigationItem> PermissionPredicate { get; set; }
		public virtual bool CheckPermission()
		{
			return PermissionPredicate == null || PermissionPredicate(this);
		}

		string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged(() => Title);
			}
		}

		string _icon;
		public string Icon
		{
			get { return _icon; }
			set
			{
				_icon = value;
				OnPropertyChanged("Icon");
			}
		}

		bool _isVisible;
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				OnPropertyChanged("IsVisible");
			}
		}

		bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				_isExpanded = value;
				if (IsExpanded && Parent != null)
					Parent.IsExpanded = true;
				OnPropertyChanged("IsExpanded");
			}
		}

		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				if (IsSelected && Parent != null)
					Parent.IsExpanded = true;
				OnPropertyChanged("IsSelected");
			}
		}

		NavigationItem _parent;
		public NavigationItem Parent
		{
			get { return _parent; }
			set
			{
				_parent = value;
				OnPropertyChanged("Parent");
			}
		}

		PermissionType? _permission = null;
		public PermissionType? Permission
		{
			get { return _permission; }
			set
			{
				_permission = value;
				OnPropertyChanged("Permission");
			}
		}

		bool _isSelectionAllowed;
		public bool IsSelectionAllowed
		{
			get { return _isSelectionAllowed; }
			set
			{
				_isSelectionAllowed = value;
				OnPropertyChanged("IsSelectionAllowed");
			}
		}

		public bool SupportMultipleSelect { get; set; }
	}
}