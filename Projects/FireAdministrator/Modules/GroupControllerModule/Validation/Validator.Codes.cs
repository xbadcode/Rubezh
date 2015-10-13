﻿using System.Collections.Generic;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using System.Linq;
using System;
using GKModule.Events;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateCodes()
		{
			ValidateCommon(GKManager.DeviceConfiguration.Codes);
			ValidateCodePropertiesEquality();
		}

		/// <summary>
		/// Валидация уникальности названий и паролей кодов
		/// </summary>
		void ValidateCodePropertiesEquality()
		{
			var names = new HashSet<string>();
			var passwords = new HashSet<int>();

			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (!names.Add(code.Name))
					Errors.Add(new CodeValidationError(code, "Дублируется название кода", ValidationErrorLevel.CannotWrite));

				if (!passwords.Add(code.Password))
					Errors.Add(new CodeValidationError(code, "Дублируется пароль кода", ValidationErrorLevel.CannotWrite));
			}
		}
	}
}