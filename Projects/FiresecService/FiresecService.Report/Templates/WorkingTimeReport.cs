﻿using FiresecService.Report.DataSources;
using Localization.FiresecService.Report.Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using System;
using System.Data;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class WorkingTimeReport : BaseReport
	{
		public WorkingTimeReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Альбомная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return true; }
		}

		public override string ReportTitle
		{
			get { return CommonResources.InfoAboutWorkTime; }
		}

		protected bool AllowOnlyAcceptedOvertime { get; set; }

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<WorkingTimeReportFilter>();

			var employeeFilter = dataProvider.GetEmployeeFilter(filter);
			var employees = dataProvider.GetEmployees(employeeFilter, filter.IsDefault);
			if (filter.IsSearch)
			{
				employeeFilter.FirstName = filter.FirstName;
				employeeFilter.SecondName = filter.SecondName;
				employeeFilter.LastName = filter.LastName;
			}
			var timeTrackResult = dataProvider.DatabaseService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo);

			var dataSet = new WorkingTimeDataSet();
			foreach (var employee in employees)
			{
				var dataRow = dataSet.Data.NewDataRow();

				dataRow.Employee = employee.Name;
				dataRow.Department = employee.Department;
				dataRow.Position = employee.Position;

				var timeTrackEmployeeResult = timeTrackResult.Result.TimeTrackEmployeeResults.FirstOrDefault(x => x.ShortEmployee.UID == employee.UID);
				if (timeTrackEmployeeResult != null)
				{
					var totalScheduleDay = default(double);
					var totalScheduleNight = default(double);
					var totalPresence = default(double);
					var totalOvertime = default(double);
					var totalNight = default(double);
					var totalDocumentOvertime = default(double);
					var totalDocumentAbsence = default(double);
					var totalAbsence = default(double);
					var documentPresenceTotal = default(double);
					var documentAbsenceReasTotal = default(double);
					foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
					{
						dayTimeTrack.Calculate();
						totalScheduleNight += dayTimeTrack.GetNightTotalTime();

						totalScheduleDay += dayTimeTrack.SlideTime != default(TimeSpan)
											? dayTimeTrack.SlideTime.TotalHours
											: dayTimeTrack.PlannedTimeTrackParts.Where(x => x.TimeTrackPartType != TimeTrackType.Break).Aggregate(default(double), (current, plannedPart) => current + plannedPart.Delta.TotalHours);

						var presence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Presence);
						if (presence != null)
						{
							totalPresence += presence.TimeSpan.TotalHours;
						}

						var overtime = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime);
						if (overtime != null)
						{
							totalOvertime += overtime.TimeSpan.TotalHours;
						}

						var night = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Night);
						if (night != null)
						{
							totalNight += night.TimeSpan.TotalHours;
						}

						var documentOvertime = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentOvertime);
						if (documentOvertime != null)
						{
							totalDocumentOvertime += documentOvertime.TimeSpan.TotalHours;
						}

						var documentAbsence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentAbsence);
						if (documentAbsence != null)
						{
							totalDocumentAbsence += documentAbsence.TimeSpan.TotalHours;
						}

						var balanceTotal = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Balance);
						if (balanceTotal != null)
						{
						}

						var absence = dayTimeTrack.Totals.Where(x => x.TimeTrackType == TimeTrackType.Absence
																|| x.TimeTrackType == TimeTrackType.EarlyLeave
																|| x.TimeTrackType == TimeTrackType.Late)
																.Select(x => x.TimeSpan.TotalHours);
						if (absence != null)
						{
							totalAbsence += absence.Aggregate(default(double), (s, i) => s + i);
						}

						var documentPresence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentPresence);
						if (documentPresence != null)
						{
							documentPresenceTotal += documentPresence.TimeSpan.TotalHours;
						}

						var documentAbsenceReas = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentAbsenceReasonable);
						if (documentAbsenceReas != null)
						{
							documentAbsenceReasTotal += documentAbsenceReas.TimeSpan.TotalHours;
						}
					}

					dataRow.ScheduleDay = totalScheduleDay.ToString("f1");
					dataRow.ScheduleNight = totalScheduleNight.ToString("f1");
					dataRow.RealPresence = (totalPresence + totalNight).ToString("f1");
					dataRow.RealNightTime = totalNight.ToString("f1");
					dataRow.TotalAbsence = totalAbsence.ToString("f1");
					dataRow.TotalNonAcceptedOvertime = totalOvertime.ToString("f1");
					dataRow.DocumentPresence = documentPresenceTotal.ToString("f1");
					dataRow.DocumentAbsenceReasonable = documentAbsenceReasTotal.ToString("f1");
					dataRow.DocumentAbsence = totalDocumentAbsence.ToString("f1");
					dataRow.DocumentOvertime = totalDocumentOvertime.ToString("f1");
					dataRow.TotalBalance = GetBalanceWithoutNonAcceptedOvertime(totalDocumentOvertime, totalAbsence, totalDocumentAbsence, totalOvertime, filter.AllowOnlyAcceptedOvertime).ToString("f1");
				}
				dataRow.TotalBalanceHeaderName = filter.AllowOnlyAcceptedOvertime
												? CommonResources.BalanceWithoutTimeout
												: CommonResources.BalanceWithTimeout;
				dataSet.Data.Rows.Add(dataRow);
			}
			return dataSet;
		}

		protected override void BeforeReportPrint()
		{
			base.BeforeReportPrint();

			this.EmployeeHeaderCell.Text = CommonResources.Employee;
			this.DepartmentHeaderCell.Text = CommonResources.Department;
			this.PositionHeaderCell.Text = CommonResources.Position;
			this.WorkingTimeHeaderCell.Text = CommonResources.WorkTimeSchedule;
			this.RealTotalTimeHeaderCell.Text = CommonResources.FactWorkTime;
			this.NonAcceptedAdjustmentsHeaderCell.Text = CommonResources.ScheduleDeviationNotConfirmed;
			this.DocumentsTimeHeaderCell.Text = CommonResources.DocumentTime;
			this.WorkingTimeHeaderCell2.Text = CommonResources.Tottally;
			this.WorkingTimeHeaderCell3.Text = CommonResources.IncludeNightTime;
			this.RealTotalTimeHeaderCell2.Text = CommonResources.Tottally;
			this.RealTotalTimeHeaderCell3.Text = CommonResources.IncludeNightWork;
			this.NonAcceptedAdjustmentsHeaderCell2.Text = CommonResources.AbsenceInclEarlyLeaveLate;
			this.NonAcceptedAdjustmentsHeaderCell3.Text = CommonResources.TimeoverOutSchedule;
			this.DocumentsTimeHeaderCell2.Text = CommonResources.Presence;
			this.DocumentsTimeHeaderCell3.Text = CommonResources.ReasonAbsence;
			this.DocumentsTimeHeaderCell4.Text = CommonResources.NotReasonAbsence;
			this.DocumentsTimeHeaderCell5.Text = CommonResources.Overtime;
		}

		private double GetBalanceWithoutNonAcceptedOvertime(double totalDocumentOvertime, double totalAbsence, double totalDocumentAbsence, double totalOvertime, bool allowOnlyAcceptedOvertime)
		{
			return allowOnlyAcceptedOvertime
				? totalDocumentOvertime - (totalAbsence + totalDocumentAbsence)
				: (totalOvertime + totalDocumentOvertime) - (totalAbsence + totalDocumentAbsence);

		}
	}
}