﻿using System;
using Common;
using Firesec;
using FiresecAPI;
using Infrastructure.Common;
using FSAgentClient;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static FiresecDriver FiresecDriver { get; private set; }
		public static FSAgent FSAgent { get; private set; }

        static public OperationResult<bool> InitializeFiresecDriver(string FS_Address, int FS_Port, string FS_Login, string FS_Password, bool isPing)
        {
            try
            {
				FSAgent = new FSAgent(AppSettingsManager.FSAgentServerAddress);
				FSAgent.Start();

                FiresecDriver = new FiresecDriver();
                var result = FiresecDriver.Connect(FSAgent, FS_Address, FS_Port, FS_Login, FS_Password, isPing);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.InitializeFiresecDriver");
                LoadingErrorManager.Add(e);
                return new OperationResult<bool>(e.Message);
            }
        }

        public static void SynchrinizeJournal()
        {
            try
            {
                var result = FiresecService.FiresecService.GetJournalLastId();
                if (result.HasError)
                {
                    LoadingErrorManager.Add("Ошибка при получении индекса последней записи с сервера");
                }

                var journalRecords = FiresecDriver.Watcher.SynchrinizeJournal(result.Result);
                if (journalRecords.Count > 0)
                {
                    FiresecService.AddJournalRecords(journalRecords);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.SynchrinizeJournal");
                LoadingErrorManager.Add(e);
            }
        }
    }
}