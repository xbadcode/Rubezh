﻿using System;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;
using System.Collections.Generic;

namespace SKDDriver.DataClasses
{
	public class DayIntervalTranslator : OrganisationItemTranslatorBase<DayInterval, API.DayInterval, API.DayIntervalFilter>
	{
		int _daySeconds = 86400;

		public DayIntervalTranslator(DbService context)
			: base(context)
		{
			AsyncTranslator = new DayIntervalAsyncTranslator(this);
		}

		public DayIntervalAsyncTranslator AsyncTranslator { get; private set; }

		public override DbSet<DayInterval> Table
		{
			get { return Context.DayIntervals; }
		}

		public override IQueryable<DayInterval> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.DayIntervalParts);
		}

		public override API.DayInterval Translate(DayInterval tableItem)
		{
			var result = base.Translate(tableItem);
			if (result == null)
				return null;
			result.SlideTime = TimeSpan.FromSeconds(tableItem.SlideTime);
			result.DayIntervalParts = tableItem.DayIntervalParts.Select(x => TranslatePart(x)).ToList();
			return result;
		}

		public override void TranslateBack(API.DayInterval apiItem, DayInterval tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.SlideTime = (int)apiItem.SlideTime.TotalSeconds;
			tableItem.DayIntervalParts = apiItem.DayIntervalParts.Select(x => TranslatePartBack(x)).ToList();
		}

		API.DayIntervalPart TranslatePart(DayIntervalPart tableItem)
		{
			var apiItem = new API.DayIntervalPart { UID = tableItem.UID, DayIntervalUID = tableItem.DayIntervalUID.Value };
			apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime);
			apiItem.Number = tableItem.Number;
			if (tableItem.EndTime >= _daySeconds)
			{
				apiItem.TransitionType = API.DayIntervalPartTransitionType.Night;
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime - _daySeconds);
			}
			else
			{
				apiItem.TransitionType = API.DayIntervalPartTransitionType.Day;
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime);
			}
			return apiItem;
		}

		DayIntervalPart TranslatePartBack(API.DayIntervalPart apiItem)
		{
			var tableItem = new DayIntervalPart();
			tableItem.UID = apiItem.UID;
			tableItem.BeginTime = (int)apiItem.BeginTime.TotalSeconds;
			tableItem.EndTime = (int)apiItem.EndTime.TotalSeconds;
			tableItem.Number = apiItem.Number;
			if (apiItem.TransitionType == API.DayIntervalPartTransitionType.Night)
			{
				tableItem.EndTime += _daySeconds;
			}
			return tableItem;
		}

		protected override OperationResult<bool> CanSave(API.DayInterval dayInterval)
		{
			if (dayInterval == null)
				return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
			if (dayInterval.UID == Guid.Empty)
				return OperationResult<bool>.FromError("Не указана организация");
			bool hasSameName = Table.Any(x => x.OrganisationUID==dayInterval.OrganisationUID&&x.Name == dayInterval.Name &&
				x.UID != dayInterval.UID);
			if (hasSameName)
				return OperationResult<bool>.FromError("Запись с таким же названием уже существует");
			var intervals = dayInterval.DayIntervalParts;
			foreach (var item in intervals)
			{
				var beginTime = item.BeginTime;
				var endTime = item.EndTime;
				if (item.TransitionType != API.DayIntervalPartTransitionType.Day)
					endTime = endTime.Add(TimeSpan.FromSeconds(_daySeconds));
				if (beginTime == endTime)
					return OperationResult<bool>.FromError("Интервал не может иметь нулевую длительность");
				var otherIntervals = intervals.Where(x => x.UID != item.UID);
				if (otherIntervals.Count() == 0 && item.BeginTime.TotalSeconds >= _daySeconds)
					return OperationResult<bool>.FromError("Последовательность интервалов не может начинаться со следующего дня");
				if (beginTime > endTime)
					return OperationResult<bool>.FromError("Время окончания интервала должно быть позже времени начала");
				foreach (var interval in otherIntervals)
				{
					var otherBeginTime = interval.BeginTime;
					var otherEndTime = interval.EndTime;
					if (interval.TransitionType!=API.DayIntervalPartTransitionType.Day)
						otherEndTime = otherEndTime.Add(TimeSpan.FromSeconds(_daySeconds));
					if ((otherBeginTime >= beginTime && otherBeginTime <= endTime) || (otherEndTime >= beginTime && otherEndTime <= endTime))
						return OperationResult<bool>.FromError("Последовательность интервалов не должна быть пересекающейся");
				}
			}
			return new OperationResult<bool>(true);
		}

		protected override void ClearDependentData(DayInterval tableItem)
		{
			Context.DayIntervalParts.RemoveRange(tableItem.DayIntervalParts);
		}
	}

	public class DayIntervalAsyncTranslator : AsyncTranslator<DayInterval, API.DayInterval, API.DayIntervalFilter>
	{
		public DayIntervalAsyncTranslator(DayIntervalTranslator translator) : base(translator as ITranslatorGet<DayInterval, API.DayInterval, API.DayIntervalFilter>) { }
		public override List<API.DayInterval> GetCollection(DbCallbackResult callbackResult)
		{
			return callbackResult.DayIntervals;
		}
	}
}