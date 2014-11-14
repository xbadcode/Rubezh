﻿using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public static class DescriptorsManager
	{
		public static List<KauDatabase> KauDatabases { get; private set; }
		public static List<GkDatabase> GkDatabases { get; private set; }

		public static void Create()
		{
			GKManager.UpdateConfiguration();
			GKManager.PrepareDescriptors();

			GkDatabases = new List<GkDatabase>();
			KauDatabases = new List<KauDatabase>();

			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == GKDriverType.GK)
				{
					var gkDatabase = new GkDatabase(device);
					GkDatabases.Add(gkDatabase);

					foreach (var kauDevice in device.Children)
					{
						if (kauDevice.Driver.IsKauOrRSR2Kau)
						{
							var kauDatabase = new KauDatabase(kauDevice);
							gkDatabase.KauDatabases.Add(kauDatabase);
							KauDatabases.Add(kauDatabase);
						}
					}
				}
			}

			KauDatabases.ForEach(x => x.BuildObjects());
			GkDatabases.ForEach(x => x.BuildObjects());
			CreateDynamicObjectsInGKManager();
		}

		public static void CreateDynamicObjectsInGKManager()
		{
			GKManager.AutoGeneratedDelays = new List<GKDelay>();
			GKManager.AutoGeneratedPims = new List<GKPim>();
			foreach (var gkDatabase in GkDatabases)
			{
				foreach (var delay in gkDatabase.Delays)
				{
					delay.InternalState = new GKDelayInternalState(delay);
					delay.State = new GKState(delay);
					GKManager.AutoGeneratedDelays.Add(delay);
				}
				foreach (var pim in gkDatabase.Pims)
				{
					pim.InternalState = new GKPimInternalState(pim);
					pim.State = new GKState(pim);
					GKManager.AutoGeneratedPims.Add(pim);
				}
			}
		}
	}
}