﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class AdditionalColumnTypeHelper
	{
		public static bool Save(AdditionalColumnType AdditionalColumnType, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SaveAdditionalColumnType(AdditionalColumnType, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(AdditionalColumnType item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(AdditionalColumnType item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedAdditionalColumnType(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.RestoreAdditionalColumnType(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static AdditionalColumnType GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = FiresecManager.FiresecService.GetAdditionalColumnTypes(new AdditionalColumnTypeFilter
			{
				UIDs = new List<System.Guid> { uid.Value }, LogicalDeletationType = LogicalDeletationType.All
			});
			var result = Common.ShowErrorIfExists(operationResult);
			if (result != null && result.Count > 0)
				return result.FirstOrDefault();
			return null;
		}

		public static IEnumerable<AdditionalColumnType> Get(AdditionalColumnTypeFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetAdditionalColumnTypes(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<AdditionalColumnType> GetByCurrentUser()
		{
			return Get(new AdditionalColumnTypeFilter() { UserUID = FiresecManager.CurrentUser.UID });
		}

		public static IEnumerable<AdditionalColumnType> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetAdditionalColumnTypes(new AdditionalColumnTypeFilter
				{
					OrganisationUIDs = new List<System.Guid> { organisationUID }
				});
			return Common.ShowErrorIfExists(result);
		}
}
}