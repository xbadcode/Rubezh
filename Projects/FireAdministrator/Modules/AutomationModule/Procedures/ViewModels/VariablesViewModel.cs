﻿using FiresecAPI.Automation;
using FiresecAPI.Models.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class VariablesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public Procedure Procedure { get; private set; }

		public VariablesViewModel(Procedure procedure)
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, () => SelectedVariable != null);
			EditCommand = new RelayCommand(OnEdit, () => SelectedVariable != null);
			Menu = new VariablesMenuViewModel(this);
			Procedure = procedure;

			Variables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in procedure.Variables)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
		}

		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		ObservableCollection<VariableViewModel> _variables;
		public ObservableCollection<VariableViewModel> Variables
		{
			get { return _variables; }
			set
			{
				_variables = value;
				OnPropertyChanged(() => Variables);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var variableDetailsViewModel = new VariableDetailsViewModel(null, Resources.Language.Procedures.ViewModels.VariablesViewModel.LocalVariable
                                                                            , Resources.Language.Procedures.ViewModels.VariablesViewModel.AddLocalVariable);
			if (!DialogService.ShowModalWindow(variableDetailsViewModel)) return;

			if (IsExist(variableDetailsViewModel.Variable))
			{
				MessageBoxService.ShowError(Resources.Language.Procedures.ViewModels.VariablesViewModel.VariableExistError);
				return;
			}

			var varialbeViewModel = new VariableViewModel(variableDetailsViewModel.Variable);
			Procedure.Variables.Add(varialbeViewModel.Variable);
			Variables.Add(varialbeViewModel);
			SelectedVariable = varialbeViewModel;
			SelectedVariable.Update();
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		private bool IsExist(IVariable variable)
		{
			return Variables.Any(x => string.Equals(x.Variable.Name, variable.Name));
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Procedure.Variables.Remove(SelectedVariable.Variable);
			Variables.Remove(SelectedVariable);
			SelectedVariable = Variables.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
            var variableDetailsViewModel = new VariableDetailsViewModel(SelectedVariable.Variable, Resources.Language.Procedures.ViewModels.VariablesViewModel.LocalVariable
                                                                                                , Resources.Language.Procedures.ViewModels.VariablesViewModel.EditLocalVariable);
			if (DialogService.ShowModalWindow(variableDetailsViewModel))
			{
				PropertyCopy.Copy(variableDetailsViewModel.Variable, SelectedVariable.Variable);
				SelectedVariable.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Select(Guid variableUid)
		{
			if (variableUid != Guid.Empty)
			{
				SelectedVariable = Variables.FirstOrDefault(item => item.Variable.UID == variableUid);
			}
		}
	}
}