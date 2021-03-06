﻿using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using RubezhClient;

namespace GKWebService.DataProviders.SKD
{
    public static class DepartmentHelper
	{
		public static bool Save(Department department, bool isNew)
		{
			var result = ClientManager.RubezhService.SaveDepartment(department, isNew);
			return Common.ThrowErrorIfExists(result);
		}

		public static bool MarkDeleted(ShortDepartment item)
		{
			var result = ClientManager.RubezhService.MarkDeletedDepartment(item);
			return Common.ThrowErrorIfExists(result);
		}

		public static bool Restore(ShortDepartment item)
		{
			var result = ClientManager.RubezhService.RestoreDepartment(item);
			return Common.ThrowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> Get(DepartmentFilter filter)
		{
			var result = ClientManager.RubezhService.GetDepartmentList(filter);
			return Common.ThrowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.RubezhService.GetDepartmentList(new DepartmentFilter
				{
					OrganisationUIDs = new List<Guid> { organisationUID },
				});
			return Common.ThrowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> GetByCurrentUser()
		{
			return Get(new DepartmentFilter() { User = ClientManager.CurrentUser });
		}

		public static Department GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var result = ClientManager.RubezhService.GetDepartmentDetails(uid.Value);
			return Common.ThrowErrorIfExists(result);
		}

		public static bool SaveChief(ShortDepartment model, Guid? chiefUID)
		{
			return SaveChief(model.UID, chiefUID, model.Name);
		}

		public static bool SaveChief(Guid uid, Guid? chiefUID, string name)
		{
			var result = ClientManager.RubezhService.SaveDepartmentChief(uid, chiefUID, name);
			return Common.ThrowErrorIfExists(result);
		}

		public static ShortDepartment GetSingleShort(Guid uid)
		{
			var filter = new DepartmentFilter();
			filter.UIDs.Add(uid);
			var operationResult = ClientManager.RubezhService.GetDepartmentList(filter);
			return Common.ThrowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<Guid> GetChildEmployeeUIDs(Guid uid)
		{
			var operationResult = ClientManager.RubezhService.GetChildEmployeeUIDs(uid);
			return Common.ThrowErrorIfExists(operationResult);
		}

		public static IEnumerable<Guid> GetParentEmployeeUIDs(Guid uid)
		{
			var operationResult = ClientManager.RubezhService.GetParentEmployeeUIDs(uid);
			return Common.ThrowErrorIfExists(operationResult);
		}
	}
}
