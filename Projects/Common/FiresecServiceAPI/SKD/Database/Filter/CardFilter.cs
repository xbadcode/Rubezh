﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class CardFilter:FilterBase
    {
        [DataMember]
		public List<Guid> Uids { get; set; }
		[DataMember]
		public List<Guid> EmployeeUids { get; set; }
		
		public CardFilter()
        {
            Uids = new List<Guid>();
			EmployeeUids = new List<Guid>();
        }
    }
}
