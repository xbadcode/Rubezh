﻿using System;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;
using System.Collections.Generic;

namespace SKDDriver.DataClasses
{
	public class HolidayTranslator : OrganisationItemTranslatorBase<Holiday, API.Holiday, API.HolidayFilter>
	{
        public HolidayTranslator(DbService context)
            : base(context)
        {
            AsyncTranslator = new HolidayAsyncTranslator(this);
        }

        public HolidayAsyncTranslator AsyncTranslator { get; private set; }

		public override DbSet<Holiday> Table
		{
			get { return Context.Holidays; }
		}

		public override API.Holiday Translate(Holiday tableItem)
		{
			var result = base.Translate(tableItem);
            if (result == null)
                return null;
			result.Date = tableItem.Date;
			result.TransferDate = tableItem.TransferDate;
			result.Type = (API.HolidayType)tableItem.Type;
			result.Reduction = TimeSpan.FromMilliseconds(tableItem.Reduction);
			return result;
		}

		public override void TranslateBack(API.Holiday apiItem, Holiday tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.Date = apiItem.Date;
			tableItem.TransferDate = apiItem.TransferDate.CheckDate();
			tableItem.Type = (int)apiItem.Type;
			tableItem.Reduction = (int)apiItem.Reduction.TotalMilliseconds;
		}

		public override System.Linq.IQueryable<Holiday> GetFilteredTableItems(API.HolidayFilter filter, IQueryable<Holiday> tableItems)
		{
			return base.GetFilteredTableItems(filter, tableItems).Where(x => filter.Year == 0 || x.Date.Year == filter.Year);
		}

		protected override FiresecAPI.OperationResult<bool> CanSave(API.Holiday item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			if (item.Reduction.TotalHours > 2)
				return OperationResult<bool>.FromError("Величина сокращения не может быть больше двух часов");
			int year = item.Date.Year;
			if (Table.Any(x => x.UID != item.UID 
				&& x.OrganisationUID == item.OrganisationUID 
				&& x.Date.Year == year
				&& !x.IsDeleted
				&& x.Date == item.Date))
				return OperationResult<bool>.FromError("Дата сокращённого дня совпадает с введенной ранее");
			bool hasSameName = Table.Any(x => x.OrganisationUID == item.OrganisationUID && x.UID != item.UID && !x.IsDeleted && x.Name == item.Name 
				&& x.Date.Year == year);
			if (hasSameName)
				return OperationResult<bool>.FromError("Сокращённый день с таким же названием уже содержится в базе данных");
			return new OperationResult<bool>();
		}
	}

    public class HolidayAsyncTranslator : AsyncTranslator<Holiday, API.Holiday, API.HolidayFilter>
    {
        public HolidayAsyncTranslator(HolidayTranslator translator) : base(translator as ITranslatorGet<Holiday, API.Holiday, API.HolidayFilter>) { }
        public override List<API.Holiday> GetCollection(DbCallbackResult callbackResult)
        {
            return callbackResult.Holidays;
        }
    }
}
		