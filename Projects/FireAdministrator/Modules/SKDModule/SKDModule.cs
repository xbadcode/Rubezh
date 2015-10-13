using System;
using System.Collections.Generic;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrustructure.Plans.Events;
using SKDModule.Validation;
using SKDModule.ViewModels;
using RubezhAPI.SKD;

namespace SKDModule
{
	public class SKDModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		public override void CreateViewModels()
		{
		}

		public override void Initialize()
		{
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.SKD; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Layout/DataTemplates/Dictionary.xaml"));
		}

		#region IValidationModule Members
		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
		#endregion

		#region ILayoutDeclarationModule Members
		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDVerification, 304, "Верификация", "Панель верификация", "BTree.png") { Factory = (p) => new LayoutPartVerificationViewModel(p as LayoutPartReferenceProperties), };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.SKDHR, 305, "Картотека", "Панель картотека", "BLevels.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDTimeTracking, 310, "УРВ", "Панель учета рабочеговремени", "BTree.png");
		}
		#endregion
	}
}