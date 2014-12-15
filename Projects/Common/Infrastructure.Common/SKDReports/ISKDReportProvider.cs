﻿
using FiresecAPI.Models;
namespace Infrastructure.Common.SKDReports
{
	public interface ISKDReportProvider
	{
		string Name { get; }
		string Title { get; }
		PermissionType? Permission { get; }
		int Index { get; }
		string IconSource { get; }
		SKDReportGroup? Group { get; }
	}
}