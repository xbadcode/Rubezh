﻿using System.Collections.Generic;
using System.Linq;
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
			GKManager.DeviceConfiguration.UpdateConfiguration();
			GKManager.DeviceConfiguration.PrepareDescriptors();

			GkDatabases = new List<GkDatabase>();
			KauDatabases = new List<KauDatabase>();

			foreach (var device in GKManager.Devices.Where(x => x.DriverType == GKDriverType.GK))
			{
				var gkDatabase = new GkDatabase(device);
				GkDatabases.Add(gkDatabase);

				foreach (var kauDevice in device.Children.Where(x => x.Driver.IsKau))
				{
					var kauDatabase = new KauDatabase(kauDevice);
					gkDatabase.KauDatabases.Add(kauDatabase);
					KauDatabases.Add(kauDatabase);
				}
			}

			KauDatabases.ForEach(x => x.BuildObjects());
			GkDatabases.ForEach(x => x.BuildObjects());
			CreateDynamicObjectsInGKManager();
		}

		static void CreateDynamicObjectsInGKManager()
		{
			GKManager.AutoGeneratedDelays = new List<GKDelay>();
			GKManager.AutoGeneratedPims = new List<GKPim>();

			foreach (var gkDatabase in GkDatabases)
			{
				foreach (var descriptor in gkDatabase.Descriptors)
				{
					if (descriptor is DelayDescriptor)
					{
						var delay = descriptor.GKBase as GKDelay;
						if (delay != null)
						{
							descriptor.GKBase.InternalState = new GKDelayInternalState(delay);
							descriptor.GKBase.State = new GKState(delay);

							if (delay.IsAutoGenerated)
							{
								GKManager.AutoGeneratedDelays.Add(delay);
							}
						}
					}
					if (descriptor is PimDescriptor)
					{
						var pim = descriptor.GKBase as GKPim;
						if (pim != null)
						{
							descriptor.GKBase.InternalState = new GKPimInternalState(pim);
							descriptor.GKBase.State = new GKState(pim);

							if (pim != null && pim.IsAutoGenerated)
							{
								GKManager.AutoGeneratedPims.Add(pim);
							}
						}
					}
				}
			}
		}

		public static bool Check()
		{
			foreach (var kauDatabase in KauDatabases)
			{
				var result = kauDatabase.Check().ToList();
				if (result.Count > 0)
				{
					return false;
				}
			}
			foreach (var gkDatabase in GkDatabases)
			{
				var result = gkDatabase.Check().ToList();
				if (result.Count > 0)
				{
					return false;
				}
			}
			return true;
		}
	}
}