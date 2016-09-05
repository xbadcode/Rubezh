﻿using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Employees.Models;
using System;

namespace SKDModule.Employees.ViewModels.DialogWindows
{
	public class PrintingTemplatesDialogViewModel : SaveCancelDialogViewModel
	{
		private readonly Guid _organisationId;

		public ReportSettings Settings { get; set; }

		public PrintingTemplatesDialogViewModel(Guid organisationId)
		{
			_organisationId = organisationId;
			Settings = new ReportSettings();
			LoadTemplateNames();
		}

		private void LoadTemplateNames()
		{
			if (Settings == null)
				throw new InvalidOperationException("Settings is null");

			Settings.LoadTemplatesInOrganisation(_organisationId);
		}
	}
}