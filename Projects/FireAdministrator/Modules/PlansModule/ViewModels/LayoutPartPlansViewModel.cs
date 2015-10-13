﻿using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;

namespace PlansModule.ViewModels
{
	public class LayoutPartPlansViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartPlansProperties _properties;

		public LayoutPartPlansViewModel(LayoutPartPlansProperties properties)
		{
			Title = "Планы";
			IconSource = LayoutPartDescription.IconPath + "CMap.png";
			_properties = properties ?? new LayoutPartPlansProperties();
			UpdateLayoutPart();
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyPlansPageViewModel(this);
			}
		}

		public string PlansTitle { get; private set; }

		public void UpdateLayoutPart()
		{
			switch (_properties.Type)
			{
				case LayoutPartPlansType.All:
					PlansTitle = "(Все планы)";
					break;
				case LayoutPartPlansType.Selected:
				case LayoutPartPlansType.Single:
					var names = GetPlanNames();
					PlansTitle = string.Format("({0})", string.Join(", ", names));
					break;
			}
			OnPropertyChanged(() => PlansTitle);
		}
		private List<string> GetPlanNames()
		{
			ClientManager.PlansConfiguration.Update();
			var map = new Dictionary<Guid, string>();
			ClientManager.PlansConfiguration.AllPlans.ForEach(item => map.Add(item.UID, item.Caption));
			return _properties.Plans.Select(item => map[item]).ToList();
		}
	}
}