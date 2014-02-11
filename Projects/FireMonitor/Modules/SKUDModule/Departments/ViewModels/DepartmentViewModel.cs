﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : TreeNodeViewModel<DepartmentViewModel>
	{
		public DepartmentViewModel(Department department)
		{
			Department = department;

			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public Department Department { get; private set; }

		public void Update()
		{
			OnPropertyChanged("Department");
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var departmentDetailsViewModel = new DepartmentDetailsViewModel();
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var department = departmentDetailsViewModel.Department;
				var departmentViewModel = new DepartmentViewModel(department);
				this.Department.ChildDepartmentUids.Add(department.Uid);
				this.AddChild(departmentViewModel);
				this.Update();
				DepartmentsViewModel.Current.Departments.Add(departmentViewModel);
			}
		}

		public RelayCommand AddToParentCommand { get; private set; }
		void OnAddToParent()
		{
			Parent.AddCommand.Execute();
		}
		public bool CanAddToParent()
		{
			return Parent != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var parent = Parent;
			if (parent != null)
			{
				var index = ZonesViewModel.Current.SelectedZone.VisualIndex;
				parent.Nodes.Remove(this);
				parent.Update();

				index = Math.Min(index, parent.ChildrenCount - 1);
				DepartmentsViewModel.Current.Departments.Remove(this);
				DepartmentsViewModel.Current.SelectedDepartment = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
		}
		bool CanRemove()
		{
			return Parent != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(this.Department);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				this.Department = departmentDetailsViewModel.Department;
				Update();
			}
		}
		public bool CanEdit()
		{
			return true;
		}
	}
}