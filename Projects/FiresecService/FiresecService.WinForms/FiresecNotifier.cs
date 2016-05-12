﻿using FiresecService.Presenters;
using System;

namespace FiresecService
{
	public class FiresecNotifier : IFiresecNotifier
	{
		public void UILog(string message, bool isError = false)
		{
			UILogger.Log(message, isError);
		}

		public void BalloonShowFromServer(string text)
		{
		}

		public void OnPoll(Guid clientUID)
		{
			MainPresenter.Current.OnPoll(clientUID);
		}

		public void AddClient(RubezhAPI.Models.ClientCredentials clientCredentials)
		{
			MainPresenter.Current.AddClient(clientCredentials);
		}

		public void RemoveClient(Guid uid)
		{
			MainPresenter.Current.RemoveClient(uid);
		}

		public void AddServerTask(ServerTask serverTask)
		{
			MainPresenter.Current.AddTask(serverTask);
		}

		public void RemoveServerTask(ServerTask serverTask)
		{
			MainPresenter.Current.RemoveTask(serverTask);
		}

		public void SetLocalAddress(string address)
		{
			
		}

		public void SetRemoteAddress(string address)
		{
			MainPresenter.SetRemoteAddress(address);
		}
	}
}
