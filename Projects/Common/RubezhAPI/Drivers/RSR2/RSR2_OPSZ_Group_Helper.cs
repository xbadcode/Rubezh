﻿using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	class RSR2_OPSZ_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_OPSZ,
				UID = new Guid("7e03462f-d397-449c-a7b7-48abed3579d1"),
				Name = "Оповещатель cвето-звуковой",
				ShortName = "ОПСЗ",
				GroupDeviceChildrenCount = 2,
				IsGroupDevice = true,
				DriverClassification = GKDriver.DriverClassifications.Announcers
			};
			driver.AutoCreateChildren.Add(GKDriverType.RSR2_OPKS);
			driver.AutoCreateChildren.Add(GKDriverType.RSR2_OPKZ);
			return driver;
		}
	}
}