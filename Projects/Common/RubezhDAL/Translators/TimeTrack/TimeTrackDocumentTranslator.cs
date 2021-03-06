﻿using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public class TimeTrackDocumentTranslator
	{
		DatabaseContext Context;

		public TimeTrackDocumentTranslator(DbService databaseService)
		{
			Context = databaseService.Context;
		}

		public OperationResult<List<API.TimeTrackDocument>> Get(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return Get(employeeUID, startDateTime, endDateTime, Context.TimeTrackDocuments);
		}

		public OperationResult<List<API.TimeTrackDocument>> Get(Guid employeeUID, DateTime startDateTime, DateTime endDateTime, IEnumerable<TimeTrackDocument> tableItems)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableTimeTrackDocuments = tableItems.Where(x => x.EmployeeUID == employeeUID &&
					((x.StartDateTime.Date >= startDateTime && x.StartDateTime.Date <= endDateTime) ||
					 (x.EndDateTime.Date >= startDateTime && x.EndDateTime.Date <= endDateTime) ||
					 (startDateTime >= x.StartDateTime.Date && startDateTime <= x.EndDateTime.Date) ||
					 (endDateTime >= x.StartDateTime.Date && endDateTime <= x.EndDateTime.Date)));
				if (tableTimeTrackDocuments != null)
				{
					var timeTrackDocuments = new List<API.TimeTrackDocument>();
					foreach (var tableTimeTrackDocument in tableTimeTrackDocuments)
					{
						var timeTrackDocument = new API.TimeTrackDocument()
						{
							UID = tableTimeTrackDocument.UID,
							EmployeeUID = tableTimeTrackDocument.EmployeeUID.GetValueOrDefault(),
							StartDateTime = tableTimeTrackDocument.StartDateTime,
							EndDateTime = tableTimeTrackDocument.EndDateTime,
							DocumentCode = tableTimeTrackDocument.DocumentCode,
							Comment = tableTimeTrackDocument.Comment,
							DocumentDateTime = tableTimeTrackDocument.DocumentDateTime,
							DocumentNumber = tableTimeTrackDocument.DocumentNumber,
							FileName = tableTimeTrackDocument.FileName
						};
						timeTrackDocuments.Add(timeTrackDocument);
					}
					return timeTrackDocuments;
				}
				return new List<API.TimeTrackDocument>();
			});
		}

		public OperationResult<bool> AddTimeTrackDocument(API.TimeTrackDocument timeTrackDocument)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = new TimeTrackDocument();
				tableItem.UID = timeTrackDocument.UID;
				tableItem.EmployeeUID = timeTrackDocument.EmployeeUID;
				tableItem.StartDateTime = timeTrackDocument.StartDateTime.CheckDate();
				tableItem.EndDateTime = timeTrackDocument.EndDateTime.CheckDate();
				tableItem.DocumentCode = timeTrackDocument.DocumentCode;
				tableItem.Comment = timeTrackDocument.Comment;
				tableItem.DocumentDateTime = timeTrackDocument.DocumentDateTime.CheckDate();
				tableItem.DocumentNumber = timeTrackDocument.DocumentNumber;
				Context.TimeTrackDocuments.Add(tableItem);
				tableItem.FileName = timeTrackDocument.FileName;
				Context.SaveChanges();
				return true;
			});
		}
		public OperationResult<bool> EditTimeTrackDocument(API.TimeTrackDocument timeTrackDocument)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = (from x in Context.TimeTrackDocuments where x.UID.Equals(timeTrackDocument.UID) select x).FirstOrDefault();
				if (tableItem != null)
				{
					tableItem.EmployeeUID = timeTrackDocument.EmployeeUID;
					tableItem.StartDateTime = timeTrackDocument.StartDateTime.CheckDate();
					tableItem.EndDateTime = timeTrackDocument.EndDateTime.CheckDate();
					tableItem.DocumentCode = timeTrackDocument.DocumentCode;
					tableItem.Comment = timeTrackDocument.Comment;
					tableItem.DocumentDateTime = timeTrackDocument.DocumentDateTime.CheckDate();
					tableItem.DocumentNumber = timeTrackDocument.DocumentNumber;
					tableItem.FileName = timeTrackDocument.FileName;
					Context.SaveChanges();
				}
				return true;
			});
		}
		public OperationResult<bool> RemoveTimeTrackDocument(Guid uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Context.TimeTrackDocuments.Where(x => x.UID.Equals(uid)).Single();
				Context.TimeTrackDocuments.Remove(tableItem);
				Context.SaveChanges();
				return true;
			});
		}
	}
}