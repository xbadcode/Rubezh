﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class DepartmentTranslator : OrganisationItemTranslatorBase<Department, API.Department, API.DepartmentFilter>
	{
		DataContractSerializer _serializer;
		public DepartmentShortTranslator ShortTranslator { get; private set; }
		public DepartmentAsyncTranslator AsyncTranslator { get; private set; }
		public DepartmentSynchroniser Synchroniser { get; private set; } 
		public DepartmentTranslator(DbService context)
			: base(context)
		{
			_serializer = new DataContractSerializer(typeof(API.Department));
			ShortTranslator = new DepartmentShortTranslator(this);
            AsyncTranslator = new DepartmentAsyncTranslator(ShortTranslator);
			Synchroniser = new DepartmentSynchroniser(Table, DbService);
        }

        

		public override DbSet<Department> Table
		{
			get { return Context.Departments; }
		}

		public override System.Linq.IQueryable<Department> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.Photo).Include(x => x.ChildDepartments);
		}
		
		public override API.Department Translate(Department tableItem)
		{
			var result = base.Translate(tableItem);
            if (result == null)
                return null;
			result.Photo = result.Photo = tableItem.Photo != null ? tableItem.Photo.Translate() : null;
			result.ParentDepartmentUID = tableItem.ParentDepartmentUID;
			result.ChildDepartmentUIDs = tableItem.ChildDepartments.Select(x => x.UID).ToList();
			result.ChiefUID = tableItem.ChiefUID.GetValueOrDefault();
			result.Phone = tableItem.Phone;
			return result;
		}

		public override void TranslateBack(API.Department apiItem, Department tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.Photo = Photo.Create(apiItem.Photo);
			tableItem.ParentDepartmentUID = apiItem.ParentDepartmentUID;
			tableItem.ChiefUID = apiItem.ChiefUID.EmptyToNull();
			tableItem.Phone = apiItem.Phone;
		}

		protected override void ClearDependentData(Department tableItem)
		{
			if (tableItem.Photo != null)
				Context.Photos.Remove(tableItem.Photo);
		}

		protected override OperationResult<bool> CanSave(API.Department item)
		{
			if (item == null)
				return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
			if (item.OrganisationUID == Guid.Empty)
				return OperationResult<bool>.FromError("Не указана организация");
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.ParentDepartmentUID == item.ParentDepartmentUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return OperationResult<bool>.FromError("Запись с таким же названием уже существует");
			else
				return new OperationResult<bool>(true);
		}

		protected override void AfterDelete(Department tableItem)
		{
			base.AfterDelete(tableItem);
			MarkDeletedByParent(tableItem.UID, tableItem.RemovalDate.GetValueOrDefault());
		}

		protected override void BeforeRestore(Department tableItem)
		{
			base.BeforeRestore(tableItem);
			RestoreByChild(tableItem);
		}

		void MarkDeletedByParent(Guid uid, DateTime removalDate)
		{
			var items = new List<Department>();
			var currentItems = Table.Where(x => x.ParentDepartmentUID == uid).ToList();
			items.AddRange(currentItems);
			while (currentItems.Count > 0)
			{
				var childrenItems = new List<Department>();
				foreach (var item in currentItems)
				{
					var childItems = Table.Where(x => x.ParentDepartmentUID == item.UID).ToList();
					childrenItems.AddRange(childItems);
					items.AddRange(childItems);
				}
				currentItems = childrenItems;
			}
			foreach (var item in items)
			{
				item.IsDeleted = true;
				item.RemovalDate = removalDate;
			}
		}

		void RestoreByChild(Department tableItem)
		{
			var items = new List<Department>();
			var parent = tableItem;
			while (parent != null)
			{
				parent = Table.FirstOrDefault(x => x.UID == parent.ParentDepartmentUID);
				if(parent != null)
					items.Add(parent);
			}
			foreach (var item in items)
			{
				item.IsDeleted = true;
				item.RemovalDate = tableItem.RemovalDate;
			}
		}

		public OperationResult SaveChief(Guid uid, Guid? chiefUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
                if (tableItem == null)
                    return new OperationResult("Запись не найдена");
                tableItem.ChiefUID = chiefUID != null ? chiefUID.Value.EmptyToNull() : null;
                Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			try
			{
				var result = new List<Guid>();
                var tableItem = Table.Include(x => x.ChildDepartments.Select(y => y.Employees)).FirstOrDefault(x => x.UID == uid);
                result.AddRange(tableItem.Employees.Select(x => x.UID));
                result.AddRange(tableItem.ChildDepartments.SelectMany(x => x.Employees.Select(y => y.UID)));
				return new OperationResult<List<Guid>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<Guid>>.FromError(e.Message);
			}
		}

		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			try
			{
				var result = new List<Guid>();
                var parentItem = Table.Include(x => x.ChildDepartments.Select(y => y.Employees)).FirstOrDefault(x => x.UID == uid);
                bool isRoot = true;
                while (isRoot)
                {
                    isRoot = parentItem.ParentDepartment != null;
                    result.AddRange(parentItem.Employees.Select(x => x.UID));
                    parentItem = parentItem.ParentDepartment;
                }
				return new OperationResult<List<Guid>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<Guid>>.FromError(e.Message);
			}
		}
	}

	public class DepartmentShortTranslator : OrganisationShortTranslatorBase<Department, API.ShortDepartment, API.Department, API.DepartmentFilter>
	{
		public DepartmentShortTranslator(DepartmentTranslator translator) : base(translator) { }

		public override IQueryable<Department> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.ChildDepartments);
		}

		public override API.ShortDepartment TranslateToShort(Department tableItem)
		{
			var result = base.TranslateToShort(tableItem);
            if (result == null)
                return null;
			result.ChiefUID = tableItem.ChiefUID.GetValueOrDefault();
			result.Phone = tableItem.Phone;
			result.ParentDepartmentUID = tableItem.ParentDepartmentUID;
			result.ChildDepartments = new System.Collections.Generic.Dictionary<Guid, string>();
			var childDepartments = tableItem.ChildDepartments.Select(x => new { x.UID, x.Name });
			foreach (var item in childDepartments)
			{
				result.ChildDepartments.Add(item.UID, item.Name);
			}
			return result;
		}
	}

    public class DepartmentAsyncTranslator : AsyncTranslator<Department, API.ShortDepartment, API.DepartmentFilter>
    {
        public DepartmentAsyncTranslator(DepartmentShortTranslator translator) : base(translator as ITranslatorGet<Department, API.ShortDepartment, API.DepartmentFilter>) { }
        public override List<API.ShortDepartment> GetCollection(DbCallbackResult callbackResult)
        {
            return callbackResult.Departments;
        }
    }
}
