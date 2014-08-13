﻿using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeViewModel : TreeNodeViewModel<AdditionalColumnTypeViewModel>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public ShortAdditionalColumnType AdditionalColumnType { get; private set; }
		public string IsInGrid 
		{
			get 
			{
				if (IsOrganisation)
					return "";
				return AdditionalColumnType.IsInGrid ? "Отображается" : "Не отображается"; 
			} 
		}

		public AdditionalColumnTypeViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public AdditionalColumnTypeViewModel(Organisation organisation, ShortAdditionalColumnType additionalColumnType)
		{
			Organisation = organisation;
			AdditionalColumnType = additionalColumnType;
			IsOrganisation = false;
			Name = additionalColumnType.Name;
			Description = additionalColumnType.Description;
		}

		public void Update(ShortAdditionalColumnType additionalColumnType)
		{
			AdditionalColumnType = additionalColumnType;
			Name = additionalColumnType.Name;
			Description = additionalColumnType.Description;
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => IsInGrid);
		}
	}
}