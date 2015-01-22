﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Linq;
using SKDDriver;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using System.Collections.Generic;

namespace FiresecService.Report.Templates
{
	public partial class Report431 : BaseSKDReport
	{
        public Report431()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
            get { return "Список точек доступа"; }
		}
		protected override DataSet CreateDataSet()
		{
            return new DataSet431();
		}
        protected override void DataSourceRequered()
        {
            base.DataSourceRequered();
            FillTestData();
        }
    }
}