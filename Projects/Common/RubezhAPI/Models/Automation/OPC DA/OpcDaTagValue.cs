﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	[KnownType(typeof(OpcDaTag))]
	public class OpcDaTagValue: OpcDaTag
	{
		#region Properties
		
		[DataMember]
		public string ServerName {get; set;}
		[DataMember]
		public Guid ServerId { get; set; }

		private Object _Value;
		[DataMember]
		public Object Value 
		{
			get { return _Value; }
			set { _Value = value; }
		}
		[DataMember]
		public Int16 Quality { get; set; }
		[DataMember]
		public DateTime Timestamp { get; set; }
		[DataMember]
		public bool OperationResult { get; set; }

		#endregion
	}
}