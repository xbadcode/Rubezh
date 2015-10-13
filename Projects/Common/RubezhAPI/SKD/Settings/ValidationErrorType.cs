﻿using System;
using System.ComponentModel;

namespace RubezhAPI
{
	[Flags]
	public enum ValidationErrorType
	{
		[Description("Устройство не подключено к зоне")]
		DeviceNotConnected = 1,

		[Description("Отсутствует логика срабатывания исполнительного устройства")]
		DeviceHaveNoLogic = 2,

		[Description("Количество подключенных к зоне датчиков")]
		ZoneSensorQuantity = 4,

		[Description("Несвязанные элементы плана")]
		NotBoundedElements = 8,

		[Description("Датчик не подключен к ТД")]
		SensorNotConnected = 16,
	}
}