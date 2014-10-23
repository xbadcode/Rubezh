﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// График работ
	/// </summary>
	[DataContract]
	public class GKSchedule : ModelBase
	{
		public GKSchedule()
		{
			DayScheduleUIDs = new List<Guid>();
		}

		/// <summary>
		/// Тип графика
		/// </summary>
		[DataMember]
		public GKScheduleType GKScheduleType { get; set; }

		/// <summary>
		/// Список составных идентификаторов дневных графиков
		/// </summary>
		[DataMember]
		public List<Guid> DayScheduleUIDs { get; set; }
	}
}