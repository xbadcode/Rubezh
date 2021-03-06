﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class CardFilter : OrganisationFilterBase
	{
		public CardFilter()
			: base()
		{
			DeactivationType = LogicalDeletationType.All;
			EndDate = DateTime.Now.Date;
		}

		[DataMember]
		public LogicalDeletationType DeactivationType { get; set; }

		[DataMember]
		public bool IsWithEndDate { get; set; }
		
		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }
	}
}