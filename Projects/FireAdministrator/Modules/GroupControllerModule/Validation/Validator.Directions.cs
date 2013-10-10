﻿using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public static partial class Validator
	{
		static void ValidateDirections()
		{
			ValidateDirectionNoEquality();

			foreach (var direction in XManager.Directions)
			{
				if (IsManyGK())
					ValidateDifferentGK(direction);
				if (ValidateEmptyDirection(direction))
				{
					if (MustValidate("В направлении отсутствуют входные устройства или зоны"))
						ValidateDirectionInputCount(direction);
					if (MustValidate("В направлении отсутствуют выходные устройства"))
						ValidateDirectionOutputCount(direction);

					ValidateEmptyZoneInDirection(direction);
				}
			}
		}

		static void ValidateDirectionNoEquality()
		{
			var directionNos = new HashSet<int>();
			foreach (var direction in XManager.Directions)
			{
				if (!directionNos.Add(direction.No))
					Errors.Add(new DirectionValidationError(direction, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidateDifferentGK(XDirection direction)
		{
			var devices = new List<XDevice>();
			devices.AddRange(direction.InputDevices);
			devices.AddRange(direction.OutputDevices);
			foreach (var zone in direction.InputZones)
			{
				devices.AddRange(zone.Devices);
			}

			if (AreDevicesInSameGK(devices))
				Errors.Add(new DirectionValidationError(direction, "Направление содержит объекты устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		static void ValidateDirectionInputCount(XDirection direction)
		{
			if (direction.InputDevices.Count + direction.InputZones.Count == 0)
				Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют входные устройства или зоны", ValidationErrorLevel.CannotWrite));
		}

		static void ValidateDirectionOutputCount(XDirection direction)
		{
			var pumpStationCount = 0;
			foreach (var device in XManager.Devices)
			{
				if (device.Driver.DriverType == XDriverType.PumpStation)
				{
					if (device.PumpStationProperty.DirectionUIDs.Contains(direction.UID))
						pumpStationCount++;
				}
			}
			if (direction.OutputDevices.Count == 0 && pumpStationCount == 0)
				Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют выходные устройства", ValidationErrorLevel.CannotWrite));
		}

		static bool ValidateEmptyDirection(XDirection direction)
		{
			var count = direction.InputZones.Count + direction.InputDevices.Count + direction.OutputDevices.Count;
			if (count == 0)
			{
				Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют входные или выходные объекты", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}

		static void ValidateEmptyZoneInDirection(XDirection direction)
		{
			foreach (var zone in direction.InputZones)
			{
				if (zone.Devices.Count == 0)
				{
					Errors.Add(new DirectionValidationError(direction, "В направление входит пустая зона " + zone.PresentationName, ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}