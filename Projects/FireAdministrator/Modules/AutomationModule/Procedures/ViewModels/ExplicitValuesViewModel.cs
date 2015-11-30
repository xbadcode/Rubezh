﻿using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using RubezhAPI.Automation;
using Infrastructure.Common;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ExplicitValuesViewModel : BaseViewModel
	{
		public ExplicitValueViewModel ExplicitValue { get; private set; }
		public ObservableCollection<ExplicitValueViewModel> ExplicitValues { get; private set; }

		public ExplicitValuesViewModel()
		{
			EditListCommand = new RelayCommand(OnEditList);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);
			ExplicitValue = new ExplicitValueViewModel();
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
		}

		public ExplicitValuesViewModel(ExplicitValue explicitValue, List<ExplicitValue> explicitValues, bool isList, ExplicitType explicitType, EnumType enumType, ObjectType objectType)
		{
			EditListCommand = new RelayCommand(OnEditList);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);
			IsList = isList;
			ExplicitType = explicitType;
			EnumType = enumType;
			ObjectType = objectType;
			var newExplicitValue = new ExplicitValue();
			PropertyCopy.Copy(explicitValue, newExplicitValue);
			ExplicitValue = new ExplicitValueViewModel(newExplicitValue);
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
			foreach (var explicitVal in explicitValues)
			{
				var newExplicitVal = new ExplicitValue();
				PropertyCopy.Copy(explicitVal, newExplicitVal);
				ExplicitValues.Add(new ExplicitValueViewModel(newExplicitVal));
			}
		}

		bool _isList;
		public bool IsList
		{
			get { return _isList; }
			set
			{
				_isList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		ExplicitType _explicitType;
		public ExplicitType ExplicitType
		{
			get { return _explicitType; }
			set
			{
				if(_explicitType != value)
				{
					ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
					ExplicitValue = new ExplicitValueViewModel();
				}
				_explicitType = value;
				OnPropertyChanged(() => ExplicitValue);
				OnPropertyChanged(() => ExplicitValues);
				OnPropertyChanged(() => ExplicitType);
			}
		}

		EnumType _enumType;
		public EnumType EnumType
		{
			get { return _enumType; }
			set
			{
				if (_enumType != value)
				{
					ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
					ExplicitValue = new ExplicitValueViewModel();
				}
				_enumType = value;
				OnPropertyChanged(() => ExplicitValue);
				OnPropertyChanged(() => ExplicitValues);
				OnPropertyChanged(() => EnumType);
			}
		}

		ObjectType _objectType;
		public ObjectType ObjectType
		{
			get { return _objectType; }
			set
			{
				if (_objectType != value)
				{
					ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
					ExplicitValue = new ExplicitValueViewModel();
				}
				_objectType = value;
				OnPropertyChanged(() => ExplicitValue);
				OnPropertyChanged(() => ExplicitValues);
				OnPropertyChanged(() => ObjectType);
			}
		}

		public RelayCommand EditListCommand { get; private set; }
		void OnEditList()
		{
			var explicitValues = ExplicitValues.ToList();
			if (ExplicitType == ExplicitType.Object)
				if (!ProcedureHelper.SelectObjects(ObjectType, ref explicitValues))
					return;
			if (explicitValues != null)
				ExplicitValues = new ObservableCollection<ExplicitValueViewModel>(explicitValues);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand<ExplicitValueViewModel> RemoveCommand { get; private set; }
		void OnRemove(ExplicitValueViewModel explicitValueViewModel)
		{
			ExplicitValues.Remove(explicitValueViewModel);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand<ExplicitValueViewModel> ChangeCommand { get; private set; }
		void OnChange(ExplicitValueViewModel explicitValueViewModel)
		{
			if (IsList)
				{
				var explicitValues = ExplicitValues.ToList();
				ProcedureHelper.SelectObjects(ObjectType, ref explicitValues);
				if (explicitValues != null)
					ExplicitValues = new ObservableCollection<ExplicitValueViewModel>(explicitValues);
			}
			else
				ProcedureHelper.SelectObject(ObjectType, ExplicitValue);
			OnPropertyChanged(() => ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}
	}
}
