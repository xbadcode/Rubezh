﻿using System;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using JournalModule.ViewModels;
using ReportsModule.Models;

namespace ReportsModule.Reports
{
	public class ReportJournal : BaseReportGeneric<ReportJournalModel>
	{
		public ReportJournal()
		{
			base.ReportFileName = "JournalCrystalReport.rpt";
			ReportArchiveFilter = new ReportArchiveFilter();
		}

		public ReportJournal(ArchiveFilterViewModel archiveFilterViewModel)
		{
			base.ReportFileName = "JournalCrystalReport.rpt";
			ReportArchiveFilter = new ReportArchiveFilter(archiveFilterViewModel);
		}

		public override void LoadData()
		{
			DataList = new List<ReportJournalModel>();
			foreach (var journalRecord in ReportArchiveFilter.JournalRecords)
			{
				DataList.Add(new ReportJournalModel()
				{
					DeviceTime = journalRecord.DeviceTime.ToString(),
					SystemTime = journalRecord.SystemTime.ToString(),
					ZoneName = journalRecord.ZoneName,
					Description = journalRecord.Description,
					Device = journalRecord.DeviceName,
					Panel = journalRecord.PanelName,
					User = journalRecord.User
				});
			}
			StartDate = ReportArchiveFilter.StartDate;
			EndDate = ReportArchiveFilter.EndDate;
		}

		public DateTime EndDate { get; set; }
		public DateTime StartDate { get; set; }
		public ReportArchiveFilter ReportArchiveFilter { get; set; }

		public override void LoadCrystalReportDocument(ReportDocument reportDocument)
		{
			base.LoadCrystalReportDocument(reportDocument);
			reportDocument.SetParameterValue("StartDate", StartDate.ToString());
			reportDocument.SetParameterValue("EndDate", EndDate.ToString());
		}
	}

	public class ReportArchiveFilter
	{
		public ReportArchiveFilter()
		{
			SetFilter();
			Initialize();
		}

		public ReportArchiveFilter(ArchiveFilterViewModel archiveFilterViewModel)
		{
			SetFilter(archiveFilterViewModel);
			Initialize();
		}

		void Initialize()
		{
			JournalRecords = new List<JournalRecord>();
			LoadArchive();
		}

		public readonly DateTime ArchiveFirstDate = FiresecManager.FiresecService.GetArchiveStartDate().Result;
		public List<JournalRecord> JournalRecords { get; set; }
		public ArchiveFilter ArchiveFilter { get; set; }
		public bool IsFilterOn { get; set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		void SetFilter(ArchiveFilterViewModel archiveFilterViewModel)
		{
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDate;
			EndDate = archiveFilterViewModel.EndDate;
		}

		void SetFilter()
		{
			var archiveFilter = new ArchiveFilter() { StartDate = ArchiveFirstDate, EndDate = DateTime.Now, UseSystemDate = false };
			var archiveFilterViewModel = new ArchiveFilterViewModel(archiveFilter);
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDate;
			EndDate = archiveFilterViewModel.EndDate;
		}

		public void LoadArchive()
		{
			JournalRecords = new List<JournalRecord>();
			OperationResult<List<JournalRecord>> operationResult;
			operationResult = FiresecManager.FiresecService.GetFilteredArchive(ArchiveFilter);
			if (operationResult.HasError == false)
			{
				foreach (var journalRecord in operationResult.Result)
				{
					JournalRecords.Add(journalRecord);
				}
			}
		}
	}
}