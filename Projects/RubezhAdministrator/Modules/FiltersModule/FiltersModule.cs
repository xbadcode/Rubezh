using System;
using System.Collections.Generic;
using FilterModule.Validation;
using FiltersModule.Events;
using FiltersModule.ViewModels;
using RubezhAPI;
using RubezhAPI.Models.Layouts;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;

namespace FiltersModule
{
	public class FilterModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		FiltersViewModel FiltersViewModel;

		public override void CreateViewModels()
		{
			FiltersViewModel = new FiltersViewModel();
		}

		public override void Initialize()
		{
			FiltersViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowFiltersEvent, Guid>(FiltersViewModel, ModuleType.ToDescription(), "Filter"),
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Filters; }
		}

		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Journal, 120, "Журнал", "Панель журнал", "BBook.png") { Factory = (p) => new LayoutPartJournalViewModel(p as LayoutPartJournalProperties), };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Archive, 121, "Архив", "Панель архив", "BArchive.png", false);
		}

		#endregion
	}
}