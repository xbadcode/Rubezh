﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using API = FiresecAPI.SKD;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace SKDDriver.DataClasses
{
	public class EmployeeTranslator : OrganisationItemTranslatorBase<Employee, API.Employee, API.EmployeeFilter>
	{
		public EmployeeShortTranslator ShortTranslator { get; private set; }
		public EmployeeAsyncTranslator AsyncTranslator { get; private set; }
		public EmployeeSynchroniser Synchroniser { get; private set; }

		public EmployeeTranslator(DbService dbService)
			: base(dbService)
		{
			ShortTranslator = new EmployeeShortTranslator(this);
			AsyncTranslator = new EmployeeAsyncTranslator(ShortTranslator);
			Synchroniser = new EmployeeSynchroniser(Table, DbService);
		}

		public override DbSet<Employee> Table
		{
			get { return Context.Employees; }
		}

		public override IQueryable<Employee> GetTableItems()
		{
			return base.GetTableItems()
				.Include(x => x.Photo)
				.Include(x => x.Position)
				.Include(x => x.Department)
				.Include(x => x.Schedule)
				.Include(x => x.Escort)
				.Include(x => x.AdditionalColumns.Select(additionalColumn => additionalColumn.AdditionalColumnType))
				.Include(x => x.AdditionalColumns.Select(additionalColumn => additionalColumn.Photo));
		}

		public override IQueryable<Employee> GetFilteredTableItems(API.EmployeeFilter filter, IQueryable<Employee> tableItems)
		{
			return base.GetFilteredTableItems(filter, tableItems).Where(x =>
				(filter.DepartmentUIDs.Count() == 0 ||
					(x.DepartmentUID != null && filter.DepartmentUIDs.Contains(x.DepartmentUID.Value))) &&
				(!filter.IsEmptyDepartment ||
					x.DepartmentUID == null || x.Department.IsDeleted) &&
				(filter.PositionUIDs.Count() == 0 ||
					(x.PositionUID != null && filter.PositionUIDs.Contains(x.PositionUID.Value))) &&
				(!filter.IsEmptyPosition ||
					x.PositionUID == null || x.Position.IsDeleted) &&
				(filter.ScheduleUIDs.Count() == 0 ||
					(x.ScheduleUID != null && filter.ScheduleUIDs.Contains(x.ScheduleUID.Value))) &&
				(string.IsNullOrEmpty(filter.FirstName) ||
					(x.FirstName.Contains(filter.FirstName))) &&
				(string.IsNullOrEmpty(filter.SecondName) ||
					(x.SecondName.Contains(filter.SecondName))) &&
				(string.IsNullOrEmpty(filter.LastName) ||
					(x.LastName.Contains(filter.LastName))) &&
				x.Type == (int)filter.PersonType
				);
		}

		protected override IEnumerable<API.Employee> GetAPIItems(IQueryable<Employee> tableItems)
		{
			return tableItems.Select(tableItem => new API.Employee
			{
				UID = tableItem.UID,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				FirstName = tableItem.FirstName,
				SecondName = tableItem.SecondName,
				LastName = tableItem.LastName,
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				PositionUID = tableItem.Position != null ? tableItem.Position.UID : Guid.Empty,
				PositionName = tableItem.Position != null ? tableItem.Position.Name : null,
				IsPositionDeleted = tableItem.Position != null ? tableItem.Position.IsDeleted : false,
				DepartmentUID = tableItem.Department != null ? tableItem.Department.UID : Guid.Empty,
				DepartmentName = tableItem.Department != null ? tableItem.Department.Name : null,
				IsDepartmentDeleted = tableItem.Department != null ? tableItem.Department.IsDeleted : false,
				ScheduleUID = tableItem.Schedule != null ? tableItem.Schedule.UID : Guid.Empty,
				ScheduleName = tableItem.Schedule != null ? tableItem.Schedule.Name : null,
				IsScheduleDeleted = tableItem.Schedule != null ? tableItem.Schedule.IsDeleted : false,
				ScheduleStartDate = tableItem.ScheduleStartDate,
				Photo = tableItem.Photo != null ? new API.Photo { UID = tableItem.Photo.UID, Data = tableItem.Photo.Data } : null,
				AdditionalColumns = tableItem.AdditionalColumns.Select(x => new API.AdditionalColumn
				{
					UID = x.UID,
					EmployeeUID = x.EmployeeUID,
					AdditionalColumnTypeUID = x.AdditionalColumnType != null ? x.AdditionalColumnType.UID : Guid.Empty,
					ColumnName = x.AdditionalColumnType != null ? x.AdditionalColumnType.Name : null,
					DataType = x.AdditionalColumnType != null ? (AdditionalColumnDataType)x.AdditionalColumnType.DataType : AdditionalColumnDataType.Text,
					Photo = x.Photo != null ? new API.Photo { UID = x.Photo.UID, Data = x.Photo.Data } : null,
					TextData = x.TextData
				}).ToList(),
				Type = (API.PersonType)tableItem.Type,
				TabelNo = tableItem.TabelNo,
				CredentialsStartDate = tableItem.CredentialsStartDate,
				EscortUID = tableItem.EscortUID,
                EscortName = tableItem.Escort != null ? tableItem.Escort.LastName + " " + tableItem.Escort.FirstName + " " + tableItem.Escort.SecondName : null,
				DocumentNumber = tableItem.DocumentNumber,
				BirthDate = tableItem.BirthDate,
				BirthPlace = tableItem.BirthPlace,
				DocumentGivenBy = tableItem.DocumentGivenBy,
				DocumentGivenDate = tableItem.DocumentGivenDate,
				DocumentValidTo = tableItem.DocumentValidTo,
				Gender = (API.Gender)tableItem.Gender,
				DocumentDepartmentCode = tableItem.DocumentDepartmentCode,
				Citizenship = tableItem.Citizenship,
				DocumentType = (API.EmployeeDocumentType)tableItem.DocumentType,
				LastEmployeeDayUpdate = tableItem.LastEmployeeDayUpdate,
				Phone = tableItem.Phone
			});
		}

		public override void TranslateBack(API.Employee apiItem, Employee tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.FirstName = apiItem.FirstName;
			tableItem.SecondName = apiItem.SecondName;
			tableItem.LastName = apiItem.LastName;
			tableItem.PositionUID = apiItem.PositionUID.EmptyToNull();
			tableItem.DepartmentUID = apiItem.DepartmentUID.EmptyToNull();
			tableItem.ScheduleUID = apiItem.ScheduleUID.EmptyToNull();
			tableItem.ScheduleStartDate = apiItem.ScheduleStartDate.CheckDate();
			tableItem.Photo = Photo.Create(apiItem.Photo);
			tableItem.AdditionalColumns = apiItem.AdditionalColumns.Select(x => new AdditionalColumn
			{
				UID = x.UID,
				EmployeeUID = x.EmployeeUID,
				AdditionalColumnTypeUID = x.AdditionalColumnTypeUID.EmptyToNull(),
				Photo = Photo.Create(x.Photo),
				TextData = x.TextData
			}).ToList();
			tableItem.Type = (int)apiItem.Type;
			tableItem.TabelNo = apiItem.TabelNo;
			tableItem.CredentialsStartDate = apiItem.CredentialsStartDate;
			tableItem.EscortUID = apiItem.EscortUID;
			tableItem.BirthDate = apiItem.BirthDate.CheckDate();
			tableItem.BirthPlace = apiItem.BirthPlace;
			tableItem.DocumentNumber = apiItem.DocumentNumber;
			tableItem.DocumentGivenBy = apiItem.DocumentGivenBy;
			tableItem.DocumentValidTo = apiItem.DocumentValidTo.CheckDate();
			tableItem.DocumentGivenDate = apiItem.DocumentGivenDate.CheckDate();
			tableItem.DocumentDepartmentCode = tableItem.DocumentDepartmentCode;
			tableItem.DocumentType = (int)apiItem.DocumentType;
			tableItem.Gender = (int)apiItem.Gender;
			tableItem.Citizenship = apiItem.Citizenship;
			tableItem.LastEmployeeDayUpdate = apiItem.LastEmployeeDayUpdate.CheckDate();
			tableItem.Phone = apiItem.Phone;
		}

		protected override void ClearDependentData(Employee tableItem)
		{
			Context.AdditionalColumns.RemoveRange(tableItem.AdditionalColumns);
			if (tableItem.Photo != null)
				Context.Photos.Remove(tableItem.Photo);
		}

		public OperationResult SaveDepartment(Guid uid, Guid? departmentUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem == null)
					return new OperationResult("Запись не найдена");
				tableItem.DepartmentUID = departmentUID != null ? departmentUID.Value.EmptyToNull() : null;
				Context.SaveChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public OperationResult SavePosition(Guid uid, Guid? positionUID)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem == null)
					return new OperationResult("Запись не найдена");
				tableItem.PositionUID = positionUID != null ? positionUID.Value.EmptyToNull() : null;
				Context.SaveChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		protected override OperationResult<bool> CanSave(API.Employee item)
		{
			if (item == null)
				return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
			if (item.OrganisationUID == Guid.Empty)
				return OperationResult<bool>.FromError("Не указана организация");
			bool hasSameName = Table.Any(x => x.FirstName == item.FirstName &&
				x.FirstName == item.FirstName &&
				x.SecondName == item.SecondName &&
				x.LastName == item.LastName &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return OperationResult<bool>.FromError("Запись с таким же названием уже существует");
			else
				return new OperationResult<bool>(true);
		}
	}

	public class EmployeeShortTranslator : OrganisationShortTranslatorBase<Employee, API.ShortEmployee, API.Employee, API.EmployeeFilter>
	{
		public EmployeeShortTranslator(EmployeeTranslator translator) : base(translator as ITranslatorGet<Employee, API.Employee, API.EmployeeFilter>) { }

		public override IQueryable<Employee> GetTableItems()
		{
			return base.GetTableItems()
				.Include(x => x.Position)
				.Include(x => x.Department)
				.Include(x => x.AdditionalColumns.Select(additionalColumn => additionalColumn.AdditionalColumnType));
		}

		protected override IEnumerable<ShortEmployee> GetAPIItems(IQueryable<Employee> employees)
		{
			return employees.Select(employee =>
				new API.ShortEmployee
				{
					UID = employee.UID,
					FirstName = employee.FirstName,
					SecondName = employee.SecondName,
					LastName = employee.LastName,
					Description = employee.Description,
					DepartmentName = employee.Department != null ? employee.Department.Name : null,
					IsDepartmentDeleted = employee.Department != null && employee.Department.IsDeleted,
					PositionName = employee.Position != null ? employee.Position.Name : null,
					IsPositionDeleted = employee.Position != null && employee.Position.IsDeleted,
					Type = (API.PersonType)employee.Type,
					TextColumns = employee.AdditionalColumns.Where(x => x.AdditionalColumnType.DataType == 0).
						Select(x => new TextColumn { ColumnTypeUID = x.AdditionalColumnType.UID, Text = x.TextData }).ToList(),
					CredentialsStartDate = employee.CredentialsStartDate.ToString(),
					TabelNo = employee.TabelNo,
					OrganisationUID = employee.OrganisationUID != null ? employee.OrganisationUID.Value : Guid.Empty,
					OrganisationName = employee.Organisation.Name,
					Phone = employee.Phone,
					IsDeleted = employee.IsDeleted,
					RemovalDate = employee.RemovalDate != null ? employee.RemovalDate.Value : new DateTime(),
					LastEmployeeDayUpdate = employee.LastEmployeeDayUpdate,
					ScheduleUID = employee.ScheduleUID != null ? employee.ScheduleUID.Value : Guid.Empty,
				});
		}

		public ShortEmployee Translate(Employee employee)
		{
			return GetAPIItems(new List<Employee> { employee }.AsQueryable()).FirstOrDefault();
		}
	}

	public class EmployeeAsyncTranslator : AsyncTranslator<Employee, API.ShortEmployee, EmployeeFilter>
	{
		public EmployeeAsyncTranslator(EmployeeShortTranslator translator) : base(translator as ITranslatorGet<Employee, API.ShortEmployee, EmployeeFilter>) { }
		public override List<ShortEmployee> GetCollection(DbCallbackResult callbackResult)
		{
			return callbackResult.Employees;
		}
	}
}