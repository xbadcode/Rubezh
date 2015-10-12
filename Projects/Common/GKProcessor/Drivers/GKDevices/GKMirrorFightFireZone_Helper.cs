﻿using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class GKMirrorFightFireZone_Helper
	{
	
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x998,
				DriverType = GKDriverType.RSR2_GKMirrorFightFireZone,
				UID = new Guid("C796BFE3-A1B9-4507-8B18-09DD20B16FB2"),
				Name = "Зоны пожаротушения",
				ShortName = "Зоны пожаротушения",
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
				HasMirror = true,
				MinAddress = 1,
				MaxAddress = 2000
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);

			return driver;
		}
	}
}
