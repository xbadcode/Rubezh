﻿using System;
using System.IO;
using Common;
using DevExpress.XtraReports.UI;
using FiresecAPI.Enums;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.Templates;

namespace FiresecService.Report.Export
{
	public class ReportExporter
	{
		private readonly ReportExportFilter _filter;

		public ReportExporter(ReportExportFilter filter)
		{
			_filter = filter;
		}

		public void Execute()
		{
			var skdReportFilter = InitializeFilterFromExport(_filter);
			var report = ReportFactory.CreateReport(_filter.ReportType, skdReportFilter);
			ExportReport(report, _filter.ReportFormat, GetPath(report, _filter));
		}

		protected SKDReportFilter InitializeFilterFromExport(ReportExportFilter filter)
		{
			var tmpFilter = filter.ReportFilter;

			var archive = tmpFilter as IReportFilterArchive;
			if (archive != null)
				archive.UseArchive = filter.IsShowArchive;

			var period = tmpFilter as IReportFilterPeriod;
			if (period != null)
			{
				period.PeriodType = filter.ReportPeriodType;
				if (filter.ReportPeriodType == ReportPeriodType.Arbitrary)
				{
					period.DateTimeFrom = filter.StartDate;
					period.DateTimeTo = filter.EndDate;
				}
			}

			var reportFilter = tmpFilter as CardsReportFilter;
			if (reportFilter != null)
			{
				reportFilter.UseExpirationDate = filter.IsUseExpirationDate;
				if (filter.IsUseExpirationDate)
				{
					reportFilter.ExpirationType = filter.ReportEndDateType;

					if (filter.ReportEndDateType == EndDateType.Arbitrary)
						reportFilter.ExpirationDate = filter.EndDate;
				}
			}

			var zonesReportFilter = tmpFilter as EmployeeZonesReportFilter;
			if (zonesReportFilter != null)
			{
				zonesReportFilter.UseCurrentDate = filter.IsUseDateTimeNow;
				zonesReportFilter.ReportDateTime = filter.IsUseDateTimeNow ? DateTime.Now : filter.StartDate;
			}

			return tmpFilter;
		}

		protected string GetPath(BaseReport report, ReportExportFilter filter)
		{
			return Path.Combine(filter.Path, GetReportName(report, filter));
		}

		private string GetReportName(BaseReport report, ReportExportFilter filter)
		{
			var result = report.GetType().Name;

			if (filter.IsFilterNameInHeader)
				result += " (" + filter.ReportFilter.Name + ")";
			if (filter.IsUseDateInFileName)
			{
				var time = DateTime.Now;
				result += string.Format("_{0}-{1}-{2}_{3}_{4}_{5}", time.Day, time.Month, time.Year, time.Hour, time.Minute, time.Second);
			}

			return result + EnumHelper.GetEnumDescription(filter.ReportFormat).Replace("*", string.Empty);
		}

		private void ExportReport(XtraReport report, ReportFormatEnum reportFormat, string path)
		{
			try
			{
				switch (reportFormat)
				{
					case ReportFormatEnum.Csv:
						report.ExportToCsv(path);
						break;
					case ReportFormatEnum.Html:
						report.ExportToHtml(path);
						break;
					case ReportFormatEnum.Mht:
						report.ExportToMht(path);
						break;
					case ReportFormatEnum.Pdf:
						report.ExportToPdf(path);
						break;
					case ReportFormatEnum.Png:
						report.ExportToImage(path);
						break;
					case ReportFormatEnum.Rtf:
						report.ExportToRtf(path);
						break;
					case ReportFormatEnum.Txt:
						report.ExportToText(path);
						break;
					case ReportFormatEnum.Xls:
						report.ExportToXls(path);
						break;
					case ReportFormatEnum.Xlsx:
						report.ExportToXlsx(path);
						break;
						//	case ReportFormatEnum.Xps:
						//		report.PrintingSystem.ExportToXps(path, new DevExpress.XtraPrinting.XpsExportOptions());
						//		break;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}
		}
	}
}