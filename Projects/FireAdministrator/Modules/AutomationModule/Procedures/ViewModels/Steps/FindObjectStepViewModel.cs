﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;
using FiresecAPI.GK;

namespace AutomationModule.ViewModels
{
	public class FindObjectStepViewModel : BaseViewModel, IStepViewModel
	{
		FindObjectArguments FindObjectArguments { get; set; }
		Procedure Procedure { get; set; }
		public ObservableCollection<FindObjectConditionViewModel> FindObjectConditions { get; private set; }

		public FindObjectStepViewModel(FindObjectArguments findObjectArguments, Procedure procedure)
		{
			FindObjectArguments = findObjectArguments;
			Procedure = procedure;
			UpdateContent();
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<FindObjectConditionViewModel>(OnRemove);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);
		}

		public RelayCommand AddCommand { get; private set; }
		public void OnAdd()
		{
			var findObjectCondition = new FindObjectCondition();
			var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, SelectedVariable.Variable);
			FindObjectArguments.FindObjectConditions.Add(findObjectCondition);
			FindObjectConditions.Add(findObjectConditionViewModel);			
			OnPropertyChanged(() => FindObjectConditions);
			UpdateContent();
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand<FindObjectConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(FindObjectConditionViewModel findObjectConditionViewModel)
		{
			FindObjectConditions.Remove(findObjectConditionViewModel);
			FindObjectArguments.FindObjectConditions.Remove(findObjectConditionViewModel.FindObjectCondition);
			OnPropertyChanged(() => FindObjectConditions);
			UpdateContent();
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand ChangeJoinOperatorCommand { get; private set; }
		void OnChangeJoinOperator()
		{
			JoinOperator = JoinOperator == JoinOperator.And ? JoinOperator.Or : JoinOperator.And;
		}

		public bool IsJoinOperatorVisible
		{
			get { return FindObjectConditions.Count > 1; }
		}

		public void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure);
			Variables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in allVariables.FindAll(x => ((x.ValueType == ValueType.Object) && (x.IsList))))
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == FindObjectArguments.ResultUid);
			JoinOperator = FindObjectArguments.JoinOperator;
			FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
			if (SelectedVariable != null)
				foreach (var findObjectCondition in FindObjectArguments.FindObjectConditions)
				{
					var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, SelectedVariable.Variable);
					FindObjectConditions.Add(findObjectConditionViewModel);
				}
			else
				FindObjectArguments.FindObjectConditions = new List<FindObjectCondition>();
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => SelectedVariable);
			OnPropertyChanged(() => FindObjectConditions);
			OnPropertyChanged(() => IsJoinOperatorVisible);
		}

		public string Description
		{
			get { return ""; }
		}

		public JoinOperator JoinOperator
		{
			get { return FindObjectArguments.JoinOperator; }
			set
			{
				FindObjectArguments.JoinOperator = value;
				OnPropertyChanged(() => JoinOperator);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public ObservableCollection<VariableViewModel> Variables { get; private set; }
		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (value != null)
				{
					if (FindObjectArguments.ResultUid != value.Variable.Uid)
					{
						FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
						FindObjectArguments.FindObjectConditions = new List<FindObjectCondition>();
						OnPropertyChanged(() => FindObjectConditions);
					}
					FindObjectConditionViewModel.Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(value.ObjectType));
					FindObjectConditionViewModel.Types = new ObservableCollection<string>(ProcedureHelper.ObjectTypeToTypesList(value.ObjectType));
					FindObjectArguments.ResultUid = value.Variable.Uid;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}
	}

	public class FindObjectConditionViewModel : BaseViewModel
	{
		public FindObjectCondition FindObjectCondition { get; private set; }
		Variable ResultVariable { get; set; }

		public FindObjectConditionViewModel(FindObjectCondition findObjectCondition, Variable resultVariable)
		{
			FindObjectCondition = findObjectCondition;
			ResultVariable = resultVariable;
			SelectedProperty = FindObjectCondition.Property;
			SelectedConditionType = FindObjectCondition.ConditionType;
			IntValue = FindObjectCondition.IntValue;
			StringValue = FindObjectCondition.StringValue;
		}

		public static ObservableCollection<Property> Properties { get; set; }
		public Property SelectedProperty
		{
			get { return FindObjectCondition.Property; }
			set
			{
				FindObjectCondition.Property = value;
				ConditionTypes = new ObservableCollection<ConditionType>(ProcedureHelper.ObjectTypeToConditionTypesList(ValueType));
				OnPropertyChanged(() => SelectedProperty);
				OnPropertyChanged(() => ValueType);
				OnPropertyChanged(() => ConditionTypes);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public ObservableCollection<ConditionType> ConditionTypes { get; private set; }
		public ConditionType SelectedConditionType
		{
			get { return FindObjectCondition.ConditionType; }
			set
			{
				FindObjectCondition.ConditionType = value;
				OnPropertyChanged(() => SelectedConditionType);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public static ObservableCollection<string> Types { get; set; }
		public string SelectedType
		{
			get { return FindObjectCondition.Type; }
			set
			{
				FindObjectCondition.Type = value;
				OnPropertyChanged(() => SelectedType);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public int IntValue
		{
			get { return FindObjectCondition.IntValue; }
			set
			{
				FindObjectCondition.IntValue = value;
				OnPropertyChanged(() => IntValue);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public string StringValue
		{
			get { return FindObjectCondition.StringValue; }
			set
			{
				FindObjectCondition.StringValue = value;
				OnPropertyChanged(() => StringValue);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public ValueType ValueType
		{
			get
			{
				if (SelectedProperty == Property.Description)
					return ValueType.String;
				if (SelectedProperty == Property.Type)
					return ValueType.Object;
				return ValueType.Integer;
			}
		}
	}
}
