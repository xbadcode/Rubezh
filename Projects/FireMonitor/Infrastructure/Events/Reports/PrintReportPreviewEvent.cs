﻿using Infrastructure.Common.Windows.Reports;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events.Reports
{
	public class PrintReportPreviewEvent : CompositePresentationEvent<IReportProvider>
	{
	}
}