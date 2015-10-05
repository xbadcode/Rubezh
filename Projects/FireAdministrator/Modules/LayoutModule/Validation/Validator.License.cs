﻿using FiresecClient;
using Infrastructure.Common.Validation;
using Infrastructure.Client.Layout;
using FiresecAPI.Models.Layouts;
using System.Linq;
using FiresecLicense;
using System.Collections.Generic;
using System;

namespace LayoutModule.Validation
{
	public partial class Validator
	{
		void ValidateLicense()
		{
			foreach (var layout in FiresecManager.LayoutsConfiguration.Layouts)
			{
				var layoutLicenses = GetLayoutLicenses(layout);
				if (layoutLicenses.Any())
					Errors.Add(new LayoutValidationError(layout, "Макет содержит элементы, требующие наличия лицензии модуля(ей): " + String.Join(", ", layoutLicenses), ValidationErrorLevel.Warning));
			}
		}

		IEnumerable<string> GetLayoutLicenses(Layout layout)
		{
			if (!FiresecLicenseManager.CurrentLicenseInfo.HasFirefighting && layout.Parts.Any(x => 
				x.DescriptionUID == LayoutPartIdentities.PumpStations || 
				x.DescriptionUID == LayoutPartIdentities.MPTs))
				yield return "\"GLOBAL Пожаротушение\"";

			if (!FiresecLicenseManager.CurrentLicenseInfo.HasGuard && layout.Parts.Any(x => 
				x.DescriptionUID == LayoutPartIdentities.GuardZones))
				yield return "\"GLOBAL Охрана\"";

			if (!FiresecLicenseManager.CurrentLicenseInfo.HasSKD && layout.Parts.Any(x => 
				x.DescriptionUID == LayoutPartIdentities.Doors ||
				x.DescriptionUID == LayoutPartIdentities.GKSKDZones ||
				x.DescriptionUID == LayoutPartIdentities.SKDVerification ||
				x.DescriptionUID == LayoutPartIdentities.SKDHR ||
				x.DescriptionUID == LayoutPartIdentities.SKDTimeTracking))
				yield return "\"GLOBAL Доступ\"";

			if (!FiresecLicenseManager.CurrentLicenseInfo.HasVideo && layout.Parts.Any(x =>
				x.DescriptionUID == LayoutPartIdentities.CamerasList ||
				x.DescriptionUID == LayoutPartIdentities.CameraVideo ||
				x.DescriptionUID == LayoutPartIdentities.MultiCamera))
				yield return "\"GLOBAL Видео\"";
		}
	}
}