﻿using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class PositionTranslator : OrganizationElementTranslator<DataAccess.Position, Position, PositionFilter>
	{
		public PositionTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override OperationResult CanSave(Position item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name && x.OrganizationUID == item.OrganizationUID && x.UID != item.UID);
			if (sameName)
				return new OperationResult("Попытка добавления должности с совпадающим именем");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Position item)
		{
			if (Context.Employees.Any(x => x.PositionUID == item.UID && x.OrganizationUID == item.OrganizationUID && !x.IsDeleted))
				return new OperationResult("Не могу удалить должность, пока она указана у действующих сотрудников");
			bool sameName = Table.Any(x => x.Name == item.Name && x.OrganizationUID == item.OrganizationUID && x.UID != item.UID);
			if (sameName)
				return new OperationResult("Попытка добавления должности с совпадающим именем");
			return base.CanSave(item);
		}

		protected override Position Translate(DataAccess.Position tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.PhotoUID = tableItem.PhotoUID;
			return result;
		}

		protected override void TranslateBack(DataAccess.Position tableItem, Position apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.PhotoUID = apiItem.PhotoUID;
		}

		protected override Expression<Func<DataAccess.Position, bool>> IsInFilter(PositionFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Position>();
			result = result.And(base.IsInFilter(filter));
			var names = filter.Names;
			if (names != null && names.Count != 0)
				result = result.And(e => names.Contains(e.Name));
			return result;
		}
	}
}