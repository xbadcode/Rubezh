﻿using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using System;
using Infrastructure.Common;
using RubezhAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDoors()
		{
			ValidateDoorNoEquality();
			ValidateDoorSameDevices();

			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				ValidateDoorHasNoDevices(door);
				ValidateDoorHasWrongDevices(door);
				ValidateLockControlDevice(door);
				ValidateLockProperties(door);
				ValidateLockLogic(door);
			}
		}

		/// <summary>
		/// Валидация уникальности номеров ТД
		/// </summary>
		void ValidateDoorNoEquality()
		{
			var nos = new HashSet<int>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				if (!nos.Add(door.No))
					Errors.Add(new DoorValidationError(door, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Валидация того, что в рамках каждой ТД не может быть повторяющихся устройства
		/// Исключение делается для замка на вход и замка на выход. Устройства для замков могут совпадать с устройствами на вход или выход
		/// Также устройства, уже учавствующие в ТД, не могут учавствовать в других ТД
		/// </summary>
		void ValidateDoorSameDevices()
		{
			var deviceUIDs = new HashSet<Guid>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				var doorDeviceUIDs = new HashSet<Guid>();

				if (door.EnterDevice != null)
				{
					if (!doorDeviceUIDs.Add(door.EnterDevice.UID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.EnterDevice.PresentationName + " может учасвствовать только в устройстве на вход или в устройстве на выход ", ValidationErrorLevel.CannotWrite));
				}
				if (door.ExitDevice != null)
				{
					if (!doorDeviceUIDs.Add(door.ExitDevice.UID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.ExitDevice.PresentationName + " не может быть одновременно устройством на вход и устройством на выход ", ValidationErrorLevel.CannotWrite));
				}
				if (door.LockDevice != null)
				{
					doorDeviceUIDs.Add(door.LockDevice.UID);
				}

				if (door.LockDeviceExit != null)
				{
					doorDeviceUIDs.Add(door.LockDeviceExit.UID);
					if (door.LockDevice != null && door.LockDeviceExit.UID.Equals(door.LockDevice.UID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.LockDeviceExit.PresentationName + " не может быть одновременно реле на вход и реле на выход", ValidationErrorLevel.CannotWrite));
				}
				if (door.EnterButton != null)
				{
					if (!doorDeviceUIDs.Add(door.EnterButtonUID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.EnterButton.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite));
				}
				if (door.ExitButton != null)
				{
					if (!doorDeviceUIDs.Add(door.ExitButtonUID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.ExitButton.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite));
				}
				if (door.LockControlDeviceExit != null)
				{
					if (!doorDeviceUIDs.Add(door.LockControlDeviceExitUID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.LockControlDeviceExit.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite));
				}
				if (door.LockControlDevice != null)
				{
					if (!doorDeviceUIDs.Add(door.LockControlDevice.UID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.LockControlDevice.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite));
				}

				foreach (var doorDeviceUID in doorDeviceUIDs)
				{
					if (!deviceUIDs.Add(doorDeviceUID))
					{
						var device = GKManager.Devices.FirstOrDefault(x => x.UID == doorDeviceUID);
						if (device != null)
						{
							Errors.Add(new DoorValidationError(door, "Устройство " + device.PresentationName + " уже участвует в другой точке доступа", ValidationErrorLevel.CannotWrite));
						}
					}
				}
			}
		}

		void ValidateLockControlDevice(GKDoor door)
		{
			if (door.AntipassbackOn && door.DoorType != GKDoorType.Barrier)
			{
				if (door.LockControlDevice == null)
				{
					if (door.DoorType == GKDoorType.Turnstile)
						Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует датчик проворота", ValidationErrorLevel.CannotWrite));
					else
						Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует датчик контроля двери", ValidationErrorLevel.CannotWrite));
				}
				if (door.LockControlDeviceExit == null)
				{
					if (door.DoorType == GKDoorType.AirlockBooth)
						Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует датчик контроля двери на выход", ValidationErrorLevel.CannotWrite));
				}
				if (door.EnterZoneUID == Guid.Empty)
					Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует зона на вход", ValidationErrorLevel.CannotWrite));
				if (door.DoorType != GKDoorType.OneWay && door.ExitZoneUID == Guid.Empty)
					Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует зона на выход", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Валидация того, что ТД данного типа подключены все необходимые устройства
		/// </summary>
		/// <param name="door"></param>
		void ValidateDoorHasNoDevices(GKDoor door)
		{
			if (door.EnterDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключено устройство на вход", ValidationErrorLevel.CannotWrite));
			}
			if (door.ExitDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключено устройство на выход", ValidationErrorLevel.CannotWrite));
			}
			if (door.LockDevice == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth || door.DoorType == GKDoorType.Turnstile)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключено реле на вход", ValidationErrorLevel.CannotWrite));
				if (door.DoorType == GKDoorType.Barrier)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключено реле на открытие", ValidationErrorLevel.CannotWrite));
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключен замок", ValidationErrorLevel.CannotWrite));
			}
			if (door.LockDeviceExit == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth || door.DoorType == GKDoorType.Turnstile)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключено реле на выход", ValidationErrorLevel.CannotWrite));
				if (door.DoorType == GKDoorType.Barrier)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключено реле на закрытие", ValidationErrorLevel.CannotWrite));
			}
			if (door.EnterButton == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключена кнопка на вход", ValidationErrorLevel.CannotWrite));
			}
			if (door.ExitButton == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключена кнопка на выход", ValidationErrorLevel.CannotWrite));
			}
			if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.HasFlag(ValidationErrorType.SensorNotConnected))
			{
				if (door.LockControlDevice == null)
				{
					if (door.DoorType != GKDoorType.Barrier)
						Errors.Add(new DoorValidationError(door, "У точки доступа отсутствует датчик контроля двери на на вход", ValidationErrorLevel.Warning));
				}
				if (door.DoorType == GKDoorType.AirlockBooth)
				{
					if (door.LockControlDeviceExit == null)
						Errors.Add(new DoorValidationError(door, "У точки доступа отсутствует датчик контроля двери на выход", ValidationErrorLevel.Warning));
				}
			}
		}

		void ValidateDoorOtherLock(GKDoor door)
		{
			if (door.EnterDevice != null && door.ExitDevice != null && door.LockDevice != null)
			{
				if (door.LockDevice.DriverType == GKDriverType.RSR2_CardReader || door.LockDevice.DriverType == GKDriverType.RSR2_CodeReader)
				{
					if (door.EnterDevice.UID != door.LockDevice.UID && door.ExitDevice.UID != door.LockDevice.UID)
						Errors.Add(new DoorValidationError(door, "Устройство Замок должно совпадать с устройством на вход или выход", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateDoorHasWrongDevices(GKDoor door)
		{
			if (door.EnterDevice != null && door.EnterDevice.DriverType != GKDriverType.RSR2_CardReader && door.EnterDevice.DriverType != GKDriverType.RSR2_CodeReader)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на вход", ValidationErrorLevel.CannotWrite));
			}

			if (door.ExitDevice != null)
			{
				if (door.DoorType == GKDoorType.OneWay && door.ExitDevice.DriverType != GKDriverType.RSR2_AM_1)
				{
					Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite));
				}
				if (door.DoorType == GKDoorType.TwoWay && door.ExitDevice.DriverType != GKDriverType.RSR2_CardReader && door.ExitDevice.DriverType != GKDriverType.RSR2_CodeReader)
				{
					Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite));
				}
			}

			if (door.LockDevice != null && door.LockDevice.DriverType != GKDriverType.RSR2_RM_1 && door.LockDevice.DriverType != GKDriverType.RSR2_MVK8 && door.LockDevice.DriverType != GKDriverType.RSR2_CardReader && door.LockDevice.DriverType != GKDriverType.RSR2_CodeReader)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на замок", ValidationErrorLevel.CannotWrite));
			}

			if (door.LockControlDevice != null && door.LockControlDevice.DriverType != GKDriverType.RSR2_AM_1)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на датчик контроля двери", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Валидация того, что устройства, учавствующие в ТД, имеют правильно заданные параметры
		/// </summary>
		/// <param name="door"></param>
		void ValidateLockProperties(GKDoor door)
		{
			if (door.LockDevice != null)
			{
				switch (door.LockDevice.DriverType)
				{
					case GKDriverType.RSR2_RM_1:
					case GKDriverType.RSR2_MVK8:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с").Value != 0)
							Errors.Add(new DeviceValidationError(door.LockDevice, "Парамер 'Задержка на включение, с' устройства, участвующего в ТД, должен быть '0'", ValidationErrorLevel.CannotWrite));
						break;

					case GKDriverType.RSR2_CodeReader:
					case GKDriverType.RSR2_CardReader:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с").Value != 0)
							Errors.Add(new DeviceValidationError(door.LockDevice, "Парамер 'Задержка на включение, с' устройства, участвующего в ТД, должен быть '0'", ValidationErrorLevel.CannotWrite));
						break;
				}

				switch (door.LockDevice.DriverType)
				{
					case GKDriverType.RSR2_CodeReader:
					case GKDriverType.RSR2_CardReader:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Наличие реле").Value != 2)
							Errors.Add(new DeviceValidationError(door.LockDevice, "Парамер 'Наличие реле' устройства, участвующего в ТД, должен быть 'Есть'", ValidationErrorLevel.CannotWrite));
						break;
				}
			}
		}

		/// <summary>
		/// Валидация того, что устройство замок не должно иметь настроенной логики срабатывания
		/// </summary>
		/// <param name="door"></param>
		void ValidateLockLogic(GKDoor door)
		{
			if (door.LockDevice != null)
			{
				if (door.LockDevice.Logic.GetObjects().Count > 0)
				{
					Errors.Add(new DoorValidationError(door, "Устройство Замок не должно иметь настроенную логику срабатывания", ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}