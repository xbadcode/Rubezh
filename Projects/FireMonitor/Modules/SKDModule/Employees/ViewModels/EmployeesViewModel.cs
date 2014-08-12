﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : ViewPartViewModel
	{
		public PersonType PersonType { get; private set; }
		public List<ShortAdditionalColumnType> AdditionalColumnTypes { get; private set; }
		EmployeeFilter Filter;
		HRViewModel _hrViewModel;

		public EmployeesViewModel(HRViewModel hrViewModel)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			_hrViewModel = hrViewModel;
		}

		public void Initialize(EmployeeFilter filter)
		{
			Filter = filter;
			InitializeInternal();
		}	

		void InitializeInternal()
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;
			var employees = EmployeeHelper.Get(Filter);
			PersonType = Filter.PersonType;
			AllEmployees = new List<EmployeeViewModel>();
			Organisations = new List<EmployeeViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new EmployeeViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllEmployees.Add(organisationViewModel);
				foreach (var employee in employees)
				{
					if (employee.OrganisationUID == organisation.UID)
					{
						var employeeViewModel = new EmployeeViewModel(organisation, employee);
						organisationViewModel.AddChild(employeeViewModel);
						AllEmployees.Add(employeeViewModel);
					}
				}
			}
			OnPropertyChanged(() => Organisations);
			SelectedEmployee = Organisations.FirstOrDefault();
			InitializeAdditionalColumns();
		}

		public List<EmployeeViewModel> Organisations { get; private set; }
		List<EmployeeViewModel> AllEmployees { get; set; }

		public void Select(Guid employeeUID)
		{
			if (employeeUID != Guid.Empty)
			{
				var employeeViewModel = AllEmployees.FirstOrDefault(x => x.ShortEmployee != null && x.ShortEmployee.UID == employeeUID);
				if (employeeViewModel != null)
					employeeViewModel.ExpandToThis();
				SelectedEmployee = employeeViewModel;
			}
		}

		EmployeeViewModel _selectedEmployee;
		public EmployeeViewModel SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged(() => SelectedEmployee);
				OnPropertyChanged(() => IsEmployeeSelected);
				if (SelectedEmployee != null)
					SelectedEmployee.UpdatePhoto();
			}
		}

		public bool IsEmployeeSelected
		{
			get { return SelectedEmployee != null && !SelectedEmployee.IsOrganisation; }
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(PersonType, SelectedEmployee.Organisation, _hrViewModel);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				var employeeViewModel = new EmployeeViewModel(SelectedEmployee.Organisation, employeeDetailsViewModel.ShortEmployee);
				AllEmployees.Add(employeeViewModel);
				var organisationViewModel = Organisations.FirstOrDefault(x => x.Organisation.UID == SelectedEmployee.Organisation.UID);
				organisationViewModel.AddChild(employeeViewModel);
				SelectedEmployee = employeeViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedEmployee != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(PersonType, SelectedEmployee.Organisation, _hrViewModel, SelectedEmployee.ShortEmployee);
			var employeeUID = SelectedEmployee.ShortEmployee.UID;
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				SelectedEmployee.Update(employeeDetailsViewModel.ShortEmployee, AdditionalColumnTypes);
				SelectedEmployee.UpdatePhoto();
			}
		}
		bool CanEdit()
		{
			return SelectedEmployee != null && SelectedEmployee.Parent != null && !SelectedEmployee.IsOrganisation;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			EmployeeViewModel OrganisationViewModel = SelectedEmployee;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedEmployee.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var employee = SelectedEmployee.ShortEmployee;
			var removeResult = EmployeeHelper.MarkDeleted(employee.UID);
			if (!removeResult)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedEmployee);
			OrganisationViewModel.RemoveChild(SelectedEmployee);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedEmployee = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedEmployee = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedEmployee != null && !SelectedEmployee.IsOrganisation;
		}

		public void InitializeAdditionalColumns()
		{
			AdditionalColumnNames = new ObservableCollection<string>();
			var columnTypes = AdditionalColumnTypeHelper.GetByCurrentUser();
			if (columnTypes == null)
				return;
			columnTypes = columnTypes.Where(x => x.DataType == AdditionalColumnDataType.Text);
			AdditionalColumnTypes = columnTypes != null ? columnTypes.ToList() : new List<ShortAdditionalColumnType>();
			foreach (var additionalColumnType in AdditionalColumnTypes)
			{
				//if (additionalColumnType.DataType == AdditionalColumnDataType.Text && additionalColumnType.IsInGrid)
				if (additionalColumnType.DataType == AdditionalColumnDataType.Text)
					AdditionalColumnNames.Add(additionalColumnType.Name);
			}
			foreach (var employee in AllEmployees.Where(x => !x.IsOrganisation))
			{
				employee.UpdateColumnValues(AdditionalColumnTypes);
			}
		}

		public ObservableCollection<string> AdditionalColumnNames { get; private set; }
	}
}