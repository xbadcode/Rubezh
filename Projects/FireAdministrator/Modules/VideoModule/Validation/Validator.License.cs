﻿using RubezhClient;
using Infrastructure.Common.Validation;
using FiresecLicense;

namespace VideoModule.Validation
{
	public partial class Validator
	{
		void ValidateLicense()
		{
			if (FiresecLicenseManager.CurrentLicenseInfo.HasVideo)
				return;

			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
				Errors.Add(new VideoValidationError(camera, "Отсутствует лицензия модуля \"GLOBAL Видео\"", ValidationErrorLevel.Warning));
		}
	}
}