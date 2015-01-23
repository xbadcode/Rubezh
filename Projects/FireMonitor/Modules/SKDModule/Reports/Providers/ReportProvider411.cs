﻿using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider411 : FilteredSKDReportProvider<ReportFilter411>
	{
		public ReportProvider411()
            : base("Report411", "411. Сведения о пропусках", 411, SKDReportGroup.HR)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Статус" },
					{ "c02", "Номер" },
					{ "c03", "Организация" },
					{ "c04", "Отдел" },
					{ "c05", "Должность" },
					{ "c06", "Сотрудник" },
					{ "c07", "Срок действия" },
				},
				MainViewModel = new PassCardMainPageViewModel(),
				Pages = new List<FilterContainerViewModel>()
				{
					new PassCardTypePageViewModel(),
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}