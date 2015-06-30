﻿using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class ScheduleZoneViewModel : BaseObjectViewModel<ScheduleZone>
	{
		public ScheduleZoneViewModel(ScheduleZone scheduleZone)
			: base(scheduleZone)
		{
		}

		public string Name
		{
			get
			{
				var strazhZone = SKDManager.Zones.FirstOrDefault(x => x.UID == Model.ZoneUID);
				if (strazhZone != null)
					return strazhZone.Name;
			//		return strazhZone.PresentationName;

				var gkZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Model.ZoneUID);
				if (gkZone != null)
					return gkZone.Name;
			//		return gkZone.PresentationName;

				return "<Нет в конфигурации>";
			}
		}

		public int No
		{
			get 
			{
				var strazhZone = SKDManager.Zones.FirstOrDefault(x => x.UID == Model.ZoneUID);
				if (strazhZone != null)
					return strazhZone.No;

				var gkZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Model.ZoneUID);
				if (gkZone != null)
					return gkZone.No;

				return default(int);
			}
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Name);
		}
	}
}