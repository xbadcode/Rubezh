﻿using System;
using System.Collections.Generic;
using System.IO;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> Reconnect(Guid uid, string userName, string password)
		{
			return SafeOperationCall(() => { return FiresecService.Reconnect(uid, userName, password); }, "Reconnect");
		}
		 
		public void Disconnect(Guid uid)
		{
			SafeOperationCall(() => { FiresecService.Disconnect(uid); }, "Disconnect");
		}

		public List<CallbackResult> Poll(Guid uid)
		{
			return SafeOperationCall(() => { return FiresecService.Poll(uid); }, "Poll");
		}

		public void NotifyClientsOnConfigurationChanged()
		{
			SafeOperationCall(() => { FiresecService.NotifyClientsOnConfigurationChanged(); }, "NotifyClientsOnConfigurationChanged");
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); }, "GetSecurityConfiguration");
		}

		public T GetConfiguration<T>(string filename)
			where T : VersionedConfiguration, new()
		{
			var config = new T();
			return SafeOperationCall(() => config, filename);
		}

		public List<string> GetFileNamesList(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetFileNamesList(directory); }, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetDirectoryHash(directory); }, "GetDirectoryHash");
		}

		public System.IO.Stream GetServerAppDataFile(string dirAndFileName)
		{
			return SafeOperationCall(() => { return FiresecService.GetServerAppDataFile(dirAndFileName); }, "GetServerAppDataFile");
		}

		public Stream GetConfig()
		{
			return SafeOperationCall(() => { return FiresecService.GetConfig(); }, "GetConfig");
		}

		public void SetRemoteConfig(Stream stream)
		{
			SafeOperationCall(() => { FiresecService.SetRemoteConfig(stream); }, "SetRemoteConfig");
		}

		public void SetLocalConfig()
		{
			SafeOperationCall(() => { FiresecService.SetLocalConfig(); }, "SetLocalConfig");
		}

		public string Test(string arg)
		{
			return SafeOperationCall(() => { return FiresecService.Test(arg); }, "Test");
		}

		public string Ping()
		{
			return SafeOperationCall(() => { return FiresecService.Ping(); }, "Ping");
		}

		public List<ServerTask> GetServerTasks()
		{
			return SafeOperationCall(() => { return FiresecService.GetServerTasks(); }, "GetServerTasks");
		}

		public OperationResult ResetDB()
		{
			return SafeOperationCall(() => { return FiresecService.ResetDB(); }, "ResetDB");
		}
	}
}