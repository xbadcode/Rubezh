﻿using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RSR2_Button_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xF0,
				DriverType = GKDriverType.RSR2_Button,
				UID = new Guid("CCBABF42-F134-47E6-AF5F-51D82E5F77A5"),
				Name = "Устройство дистанционного пуска",
				ShortName = "УДП",
				HasZone = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.Other
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			return driver;
		}
	}
}