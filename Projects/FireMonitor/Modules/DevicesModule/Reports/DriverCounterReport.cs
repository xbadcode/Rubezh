﻿using System.Data;
using System.Linq;
using CodeReason.Reports;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Reports;

namespace DevicesModule.Reports
{
	internal class DriverCounterReport : ISingleReportProvider
	{
		#region ISingleReportProvider Members

		public ReportData GetData()
		{
			var data = new ReportData();

			DataTable table = new DataTable("Devices");
			table.Columns.Add("Driver");
			table.Columns.Add("Count");
			foreach (var driver in from Driver d in FiresecManager.Drivers orderby d.ShortName select d)
			{
				if (driver.IsAutoCreate || driver.DriverType == DriverType.Computer)
					continue;
				AddDrivers(driver, table);
			}
			data.DataTables.Add(table);
			return data;
		}

		#endregion

		#region IReportProvider Members

		public string Template
		{
			get { return "Reports/DriverCounterReport.xaml"; }
		}

		public string Title
		{
			get { return "Количество устройств по типам"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		#endregion

		private void AddDrivers(Driver driver, DataTable table)
		{
			var count = 0;
			foreach (var device in FiresecManager.Devices)
			{
				if (device.Driver.DriverType == driver.DriverType)
				{
					if (device.Parent.Driver.ChildAddressReserveRangeCount > 0)
						continue;
					count++;
				}
			}
			if (count > 0)
				table.Rows.Add(driver.ShortName, count);
		}
	}
}