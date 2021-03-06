﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class GlobalSettings
	{
		public GlobalSettings()
		{
			RemoteAddress = "localhost";
			RemotePort = 8000;
			Login = "adm";
			Password = "";
			AutoConnect = true;
			DoNotOverrideFS1 = true;
			Server_EnableRemoteConnections = false;

			IsGKAsAService = false;
			UseSKD = false;

			FS_RemoteAddress = "localhost";
			FS_Port = 211;
			FS_Login = "adm";
			FS_Password = "";

			SetDefaultModules();

			Monitor_F1_Enabled = false;
			Monitor_F2_Enabled = true;
			Monitor_F3_Enabled = true;
			Monitor_F4_Enabled = true;
			Monitor_IsControlMPT = false;
			Administrator_IsExpertMode = false;
			IgnoredErrors = new List<string>();
		}

		[DataMember]
		public string RemoteAddress { get; set; }

		[DataMember]
		public int RemotePort { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public bool AutoConnect { get; set; }

		[DataMember]
		public bool IsGKAsAService { get; set; }

		[DataMember]
		public bool UseSKD { get; set; }

		[DataMember]
		public bool DoNotAutoconnectAdm { get; set; }

		[DataMember]
		public bool RunRevisor { get; set; }

		[DataMember]
		public bool DoNotOverrideFS1 { get; set; }

		[DataMember]
		public string FS_RemoteAddress { get; set; }

		[DataMember]
		public int FS_Port { get; set; }

		[DataMember]
		public string FS_Login { get; set; }

		[DataMember]
		public string FS_Password { get; set; }

		[DataMember]
		public bool Server_EnableRemoteConnections { get; set; }

		[DataMember]
		public List<string> ModuleItems { get; set; }

		[DataMember]
		public bool Monitor_F1_Enabled { get; set; }
		[DataMember]
		public bool Monitor_F2_Enabled { get; set; }
		[DataMember]
		public bool Monitor_F3_Enabled { get; set; }
		[DataMember]
		public bool Monitor_F4_Enabled { get; set; }
		[DataMember]
		public bool Monitor_IsControlMPT { get; set; }
		[DataMember]
		public bool Monitor_HidePlansTree { get; set; }

		[DataMember]
		public bool Administrator_IsExpertMode { get; set; }
		[DataMember]
		public bool Administrator_HidePlanAlignInstruments { get; set; }

		[DataMember]
		public List<string> IgnoredErrors { get; set; }

		[DataMember]
		public bool IsLogicAllowed { get; set; }

		public void SetDefaultModules()
		{
			ModuleItems = new List<string>();
			
			ModuleItems.Add("PlansModule.dll");
			ModuleItems.Add("PlansModule.Kursk.dll");
			ModuleItems.Add("SecurityModule.dll");
			ModuleItems.Add("SoundsModule.dll");
			ModuleItems.Add("SettingsModule.dll");
			ModuleItems.Add("GKModule.dll");
			ModuleItems.Add("VideoModule.dll");
			ModuleItems.Add("DiagnosticsModule.dll");
			ModuleItems.Add("ReportsModule.dll");
			ModuleItems.Add("AutomationModule.dll");
			//ModuleItems.Add("SKDModule.dll");

			//ModuleItems.Add("DevicesModule.dll");
			//ModuleItems.Add("LibraryModule.dll");
			//ModuleItems.Add("FiltersModule.dll");
			//ModuleItems.Add("InstructionsModule.dll");
			//ModuleItems.Add("NotificationModule.dll");
			//ModuleItems.Add("AlarmModule.dll");
			//ModuleItems.Add("JournalModule.dll");
			//ModuleItems.Add("OPCModule.dll");
		}

		public bool IsDebug
		{
			get
			{
#if DEBUG
				return true;
#endif
				return false;
			}
		}
	}
}