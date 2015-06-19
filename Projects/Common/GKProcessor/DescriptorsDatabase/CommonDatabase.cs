﻿using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public abstract class CommonDatabase
	{
		ushort currentChildNo = 1;
		protected List<GKDevice> Devices { get; set; }
		public List<GKPim> Pims { get; private set; }

		protected ushort NextDescriptorNo
		{
			get { return currentChildNo++; }
		}

		public DatabaseType DatabaseType { get; protected set; }
		public GKDevice RootDevice { get; protected set; }

		public CommonDatabase()
		{
			Devices = new List<GKDevice>();
			Pims = new List<GKPim>();
		}

		public void AddPim(GKPim pim)
		{
			if (!Pims.Contains(pim) && pim != null)
			{
				if (DatabaseType == DatabaseType.Gk)
				{
					pim.GKDescriptorNo = NextDescriptorNo;
					pim.GkDatabaseParent = RootDevice;
				}
				else
				{
					pim.KAUDescriptorNo = NextDescriptorNo;
					pim.KauDatabaseParent = RootDevice;
					pim.GkDatabaseParent = RootDevice.GKParent;
				}
				Pims.Add(pim);
			}
		}

		public abstract void BuildObjects();
	}
}