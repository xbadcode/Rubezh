﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Localization.SKD.ViewModels;
using SKDModule.Events;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentAccessTemplateSelectionViewModel : ItemsSelectionBaseViewModel<AccessTemplate>
	{
		#region <Конструктор>

		public DepartmentAccessTemplateSelectionViewModel()
		{
			Title = CommonViewModels.ChooseAccessTemplate;
			ReleaseItemCommandText = CommonViewModels.UnpinAccessTemplate;
			AddItemCommandText = CommonViewModels.AddNewAccessTemplate;
		}
		
		#endregion </Конструктор>

		#region <Методы>

		protected override void InitializeItems(Guid organisationUID, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			Items = new ObservableCollection<AccessTemplate>(AccessTemplateHelper.Get(new AccessTemplateFilter
			{
				LogicalDeletationType = logicalDeletationType,
				OrganisationUIDs = new List<Guid> { organisationUID },
				UserUID = FiresecManager.CurrentUser.UID
			}));
		}

		protected override void OnAddItem()
		{
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel();
			accessTemplateDetailsViewModel.Initialize(_organisationUID);
			if (DialogService.ShowModalWindow(accessTemplateDetailsViewModel))
			{
				Items.Add(accessTemplateDetailsViewModel.Model);
				ServiceFactoryBase.Events.GetEvent<NewAccessTemplateEvent>().Publish(accessTemplateDetailsViewModel.Model);
			}
		}

		#endregion </Методы>
	}
}