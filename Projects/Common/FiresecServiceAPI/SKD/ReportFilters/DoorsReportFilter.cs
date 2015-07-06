﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	public class DoorsReportFilter : SKDReportFilter, IReportFilterOrganisation, IReportFilterZoneWithDirection, IReportFilterDoor
	{
		#region IReportFilterOrganisation Members

		[DataMember]
		public List<Guid> Organisations { get; set; }

		#endregion IReportFilterOrganisation Members

		#region IReportFilterZone Members

		[DataMember]
		public List<Guid> Zones { get; set; }

		#endregion IReportFilterZone Members

		#region IReportFilterDoor Members

		[DataMember]
		public List<Guid> Doors { get; set; }

		#endregion IReportFilterDoor Members

		#region IReportFilterZoneWithDirection Members

		[DataMember]
		public bool ZoneIn { get; set; }

		[DataMember]
		public bool ZoneOut { get; set; }

		#endregion IReportFilterZoneWithDirection Members
	}
}