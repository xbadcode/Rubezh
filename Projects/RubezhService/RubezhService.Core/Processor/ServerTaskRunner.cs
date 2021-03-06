﻿using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RubezhService
{
	public static class ServerTaskRunner
	{
		static List<ServerTask> ServerTasks = new List<ServerTask>();

		static Thread Thread;
		static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
		public static void Start()
		{
			Thread = new Thread(OnRun);
			Thread.Start();
		}

		public static void Stop()
		{
			foreach (var serverTask in ServerTasks)
			{
				if (serverTask.ProgressCallback != null)
				{
					serverTask.ProgressCallback.IsCanceled = true;
					Notifier.RemoveServerTask(serverTask);
				}
			}

			if (AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if (Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(2));
				}
			}
		}

		public static void SetNewConfig()
		{
			Stop();
			Start();
		}

		static void OnRun()
		{
			AutoResetEvent = new AutoResetEvent(false);
			while (true)
			{
				if (AutoResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
				{
					return;
				}
				var serverTask = ServerTasks.FirstOrDefault();
				if (serverTask != null)
				{
					serverTask.Action();
					Notifier.RemoveServerTask(serverTask);
					ServerTasks.Remove(serverTask);
				}
			}
		}

		public static void Add(GKProgressCallback progressCallback, string name, Action action)
		{
			var serverTask = new ServerTask() { Action = action, ProgressCallback = progressCallback, Name = name };
			ServerTasks.Add(serverTask);
			Notifier.AddServerTask(serverTask);
		}
	}

	public class ServerTask
	{
		public ServerTask()
		{
			UID = Guid.NewGuid();
		}

		public Guid UID { get; private set; }
		public string Name { get; set; }
		public Action Action { get; set; }
		public GKProgressCallback ProgressCallback { get; set; }
	}
}