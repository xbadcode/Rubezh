﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AppServerSettings
	{
		public AppServerSettings()
		{
			ServiceAddress = "localhost";
			ServicePort = 8000;
			ReportServicePort = 8800;
			EnableRemoteConnections = false;
			UseHasp = false;
			DBServerName = "SQLEXPRESS";
			CreateNewDBOnOversize = true;
			EnableOfflineLog = true;
		}

		[DataMember]
		public string ServiceAddress { get; set; }

		[DataMember]
		public int ServicePort { get; set; }

		[DataMember]
		public int ReportServicePort { get; set; }

		[DataMember]
		public bool EnableRemoteConnections { get; set; }

		[DataMember]
		public bool UseHasp { get; set; }

		[DataMember]
		public string DBServerName { get; set; }

		[DataMember]
		public bool CreateNewDBOnOversize { get; set; }

		public bool EnableOfflineLog { get; set; }
	}
}