﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class CardFilter : OrganisationFilterBase
	{
		public CardFilter()
		{
			DeactivationType = LogicalDeletationType.All;
			CardTypes = new List<CardType>();
			EndDate = DateTime.Now.Date;
		}

		[DataMember]
		public LogicalDeletationType DeactivationType { get; set; }

		[DataMember]
		public bool IsWithEndDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public List<CardType> CardTypes { get; set; }

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }

		[DataMember]
		public bool IsWithInactive { get; set; }

		/// <summary>
		/// Включить в выборку карты с шаблоном доступа, удовлетворяющим требованиям фильтра AccessTemplateFilter
		/// </summary>
		[DataMember]
		public AccessTemplateFilter AccessTemplateFilter { get; set; }

		/// <summary>
		/// Включить в выборку карты с пустым шаблоном доступа
		/// </summary>
		[DataMember]
		public bool WithEmptyAccessTemplate { get; set; }
	}
}