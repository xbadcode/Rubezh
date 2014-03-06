﻿using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class DepartmentTranslator : OrganizationTranslatorBase<DataAccess.Department, Department, DepartmentFilter>
	{
		public DepartmentTranslator(Table<DataAccess.Department> table, DataAccess.SKUDDataContext context)
			: base(table, context)
		{
			
		}

		protected override OperationResult CanSave(Department item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganizationUid == item.OrganizationUid &&
				x.UID != item.UID && 
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Отдел с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Department item)
		{
			bool isHasEmployees = Context.Employee.Any(x => !x.IsDeleted && x.DepartmentUid == item.UID);
			if (isHasEmployees)
				return new OperationResult("Не могу удалить отдел, пока он указан содержит действующих сотрудников");

			if(item.ChildDepartmentUIDs.IsNotNullOrEmpty())
				return new OperationResult("Не могу удалить отдел, пока он содержит дочерние отделы");
			return base.CanSave(item);
		}

		protected override Department Translate(DataAccess.Department tableItem)
		{
			var result = base.Translate(tableItem);

			var phoneUIDs = new List<Guid>();
			foreach (var phone in Context.Phone.Where(x => !x.IsDeleted && x.DepartmentUid == tableItem.UID))
			{
				phoneUIDs.Add(phone.UID);
			} 
			var childDepartmentUIDs = new List<Guid>();
			foreach (var department in Context.Department.Where(x => !x.IsDeleted && x.ParentDepartmentUid == tableItem.UID))
			{
				childDepartmentUIDs.Add(department.UID);
			}

			tableItem.Department2.ToList().ForEach(x => childDepartmentUIDs.Add(x.UID));
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.ParentDepartmentUID = tableItem.ParentDepartmentUid;
			result.ChildDepartmentUIDs = childDepartmentUIDs;
			result.ContactEmployeeUID = tableItem.ContactEmployeeUid;
			result.AttendantEmployeeUID = tableItem.AttendantUid;
			result.PhoneUIDs = phoneUIDs;
			result.PhotoUID = tableItem.PhotoUID;
			return result;
		}

		protected override void TranslateBack(DataAccess.Department tableItem, Department apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.ParentDepartmentUid = apiItem.ParentDepartmentUID;
			tableItem.ContactEmployeeUid = apiItem.ContactEmployeeUID;
			tableItem.AttendantUid = apiItem.AttendantEmployeeUID;
			tableItem.PhotoUID = apiItem.PhotoUID;
		}
	}
}
