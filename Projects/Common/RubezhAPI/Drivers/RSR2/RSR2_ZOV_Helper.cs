﻿using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RSR2_ZOV_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xF1,
				DriverType = GKDriverType.RSR2_ZOV,
				UID = new Guid("58795922-F023-4A90-BCEC-ACDB3CD0725F"),
				Name = "Оповещатель свето-звуковой взрывозащищенный",
				ShortName = "ЗОВ",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.Announcers
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			driver.AvailableStateBits.Add(GKStateBit.Off);
			driver.AvailableStateBits.Add(GKStateBit.TurningOn);
			driver.AvailableStateBits.Add(GKStateBit.TurningOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOffNow_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Задержка на выключение, с", 0, 0, 65535);

			var property1 = new GKDriverProperty()
			{
				No = 3,
				Name = "Состояние контакта для режима Удержание",
				Caption = "Состояние контакта для режима Удержание",
				Default = 0,
				IsLowByte = true,
				Mask = 0x03
			};
			GKDriversHelper.AddPropertyParameter(property1, "Выключено", 0);
			GKDriversHelper.AddPropertyParameter(property1, "Включено", 1);
			driver.Properties.Add(property1);

			driver.MeasureParameters.Add(new GKMeasureParameter { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true, IsNotVisible = true });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 2, Name = "Отсчет удержания, с", IsDelay = true, IsNotVisible = true });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 3, Name = "Отсчет задержки на выключение, с", IsDelay = true, IsNotVisible = true });

			return driver;
		}
	}
}