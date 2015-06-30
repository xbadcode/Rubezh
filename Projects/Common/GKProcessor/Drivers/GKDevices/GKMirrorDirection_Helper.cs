﻿using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class GKMirrorDirection_Helper
	{

		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x998,
				DriverType = GKDriverType.RSR2_GKMirrorDirection,
				UID = new Guid("19AD5199-53DF-40F5-9E35-99726476FD49"),
				Name = "Направления",
				ShortName = "Направления",
				CanEditAddress = true,
				HasAddress = false,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
				HasMirror = true,
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);

			return driver;
		}

	}
}
