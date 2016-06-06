﻿using System.Collections.Generic;
using System.Linq;
using Localization.SKD.Common;
using Localization.SKD.ViewModels;
using StrazhAPI.Models.Layouts;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using StrazhAPI.SKD;
using StrazhAPI.GK;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class LayoutPartVerificationViewModel : LayoutPartTitleViewModel
	{
		LayoutPartReferenceProperties _properties;

		public LayoutPartVerificationViewModel(LayoutPartReferenceProperties properties)
		{
			Title = CommonResources.Verification;
			IconSource = LayoutPartDescription.IconPath + "BLevels.png";
			_properties = properties ?? new LayoutPartReferenceProperties();
			DeviceViewModel deviceViewModel = null;
			var skdDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == _properties.ReferenceUID);
			if (skdDevice != null)
			{
				deviceViewModel = new DeviceViewModel(skdDevice);
			}
			if (deviceViewModel != null)
			{
				UpdateLayoutPart(deviceViewModel.NameAndAddress);
			}
			else
			{
				UpdateLayoutPart(CommonViewModels.UnknownDevice);
			}
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyVerificationPageViewModel(this);
			}
		}

		string _filterTitle;
		public string FilterTitle
		{
			get { return _filterTitle; }
			set
			{
				_filterTitle = value;
				OnPropertyChanged(() => FilterTitle);
			}
		}

		public void UpdateLayoutPart(string name)
		{
			FilterTitle = name;
		}
	}
}