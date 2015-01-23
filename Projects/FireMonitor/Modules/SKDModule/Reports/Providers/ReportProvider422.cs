﻿using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider422 : FilteredSKDReportProvider<ReportFilter422>
	{
		public ReportProvider422()
			: base("Report422", "422. Отчет по графикам работы", 422, SKDReportGroup.TimeTracking)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Сотрудник" },
					{ "c02", "Организация" },
					{ "c03", "Отдел" },
					{ "c04", "Должность" },
					{ "c05", "График" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
					new SchedulePageViewModel(),
				},
			};
		}
	}
}