﻿using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class DepartmentsFilterViewModel : OrganisationBaseViewModel<ShortDepartment, DepartmentFilter, DepartmentFilterItemViewModel, DepartmentDetailsViewModel>, IHRFilterTab
	{
		public DepartmentsFilterViewModel()
			: base()
		{
			SelectAllCommand = new RelayCommand(OnSelectAll);
			SelectNoneCommand = new RelayCommand(OnSelectNone);
		}

		public override void Initialize(DepartmentFilter filter)
		{
			var emptyFilter = new DepartmentFilter { LogicalDeletationType = filter.LogicalDeletationType, OrganisationUIDs = filter.OrganisationUIDs };
			base.Initialize(emptyFilter);
			var models = Organisations.SelectMany(x => x.Children);
			SetSelected(models, filter.UIDs ?? new List<Guid>());
		}

		public void Initialize(List<Guid> uids, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			var filter = new DepartmentFilter { LogicalDeletationType = logicalDeletationType, UIDs = uids };
			Initialize(filter);
		}


		public void Initialize(List<Guid> uids, List<Guid> organisationUIDs, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			var filter = new DepartmentFilter { LogicalDeletationType = logicalDeletationType, UIDs = uids, OrganisationUIDs = organisationUIDs };
			Initialize(filter);
		}

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			var models = Organisations.SelectMany(x => x.GetAllChildren(false));
			foreach (var model in models)
				model.IsChecked = true;
		}

		public RelayCommand SelectNoneCommand { get; private set; }
		void OnSelectNone()
		{
			var models = Organisations.SelectMany(x => x.GetAllChildren(false));
			foreach (var model in models)
				model.IsChecked = false;
		}

		protected override IEnumerable<ShortDepartment> GetModels(DepartmentFilter filter)
		{
			return DepartmentHelper.Get(filter);
		}
		protected override IEnumerable<ShortDepartment> GetModelsByOrganisation(Guid organisationUID)
		{
			return DepartmentHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(ShortDepartment model)
		{
			return DepartmentHelper.MarkDeleted(model);
		}
		protected override bool Restore(ShortDepartment model)
		{
			return DepartmentHelper.Restore(model);
		}
		protected override bool Add(ShortDepartment item)
		{
			throw new NotImplementedException();
		}

		protected override void InitializeModels(IEnumerable<ShortDepartment> models)
		{
			foreach (var organisation in Organisations)
			{
				foreach (var model in models)
				{
					if (model.OrganisationUID == organisation.Organisation.UID && (model.ParentDepartmentUID == null || model.ParentDepartmentUID == Guid.Empty))
					{
						var itemViewModel = new DepartmentFilterItemViewModel();
						itemViewModel.InitializeModel(organisation.Organisation, model, this);
						organisation.AddChild(itemViewModel);
						AddChildren(itemViewModel, models);
					}
				}
			}
		}

		void AddChildren(DepartmentFilterItemViewModel parentViewModel, IEnumerable<ShortDepartment> models)
		{
			if (parentViewModel.Model.ChildDepartments != null && parentViewModel.Model.ChildDepartments.Count > 0)
			{
				var children = models.Where(x => x.ParentDepartmentUID == parentViewModel.Model.UID);
				foreach (var child in children)
				{
					var itemViewModel = new DepartmentFilterItemViewModel();
					itemViewModel.InitializeModel(parentViewModel.Organisation, child, this);
					parentViewModel.AddChild(itemViewModel);
					AddChildren(itemViewModel, models);
				}
			}
		}

		public List<Guid> UIDs { get { return GetSelected(Organisations.SelectMany(x => x.Children)).ToList(); } }
		private IEnumerable<Guid> GetSelected(IEnumerable<DepartmentFilterItemViewModel> departments)
		{
			foreach (var department in departments)
			{
				if (department.IsChecked)
					yield return department.Model.UID;
				foreach (var subdepartment in GetSelected(department.Children))
					yield return subdepartment;
			}
		}
		private void SetSelected(IEnumerable<DepartmentFilterItemViewModel> departments, List<Guid> selected)
		{
			foreach (var department in departments)
			{
				if(selected.Contains(department.Model.UID))
				{
					department.IsChecked = true;
					department.ExpandToThis();
				}
				SetSelected(department.Children, selected);
			}
		}

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_Departments_Etit; }
		}

		void IHRFilterTab.Update(List<Guid> organisationUids, bool isWithDeleted)
		{
			_filter.OrganisationUIDs = organisationUids;
			_filter.UIDs = UIDs;
			_filter.LogicalDeletationType = isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;
			Initialize(_filter);
		}
	}
}