using Common;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Plans;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Painters;
using PlansModule.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System.Collections.Generic;
using System.Linq;

namespace PlansModule
{
	public class PlansModuleLoader : ModuleBase, ILayoutProviderModule
	{
		private List<IPlanPresenter<Plan, XStateClass>> _planPresenters;
		private List<PlansViewModel> _plansViewModels;
		private PlansViewModel _plansViewModel;
		private NavigationItem _planNavigationItem;

		public PlansModuleLoader()
		{
			_plansViewModels = new List<PlansViewModel>();
			_planPresenters = new List<IPlanPresenter<Plan, XStateClass>>();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Subscribe(OnRegisterPlanPresenter);
		}
		public override void CreateViewModels()
		{
			PainterCache.UseTransparentImage = false;
			EventService.RegisterEventAggregator(ServiceFactory.Events);
			_plansViewModel = new PlansViewModel(_planPresenters);
		}

		public override int Order
		{
			get { return 100; }
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Plans; }
		}
		public override void Initialize()
		{
			ClientManager.UpdatePlansConfiguration();
			ClientManager.PlansConfiguration.Update();
			using (new TimeCounter("PlansModuleLoader.Initialize: {0}"))
			{
				using (new TimeCounter("\tPlansModuleLoader.CacheBrushes: {0}"))
					foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
					{
						PainterCache.CacheBrush(plan);
						foreach (var elementBase in PlanEnumerator.Enumerate(plan))
							PainterCache.CacheBrush(elementBase);
					}
				ClientManager.InvalidatePlans();
			}
			_planNavigationItem.IsVisible = ClientManager.PlansConfiguration.Plans.Any(x => !x.IsNotShowPlan);
			_plansViewModel.Initialize();
			_plansViewModels.ForEach(item => item.Initialize());
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_planNavigationItem = new NavigationItem<ShowPlansEvent>(_plansViewModel, ModuleType.ToDescription(), "Map");
			return new List<NavigationItem>() { _planNavigationItem };
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Plans, "Планы", "CMap.png", CreatePlansViewModel);
		}
		#endregion

		private BaseViewModel CreatePlansViewModel(ILayoutProperties properties)
		{
			var plansViewModel = new PlansViewModel(_planPresenters, properties as LayoutPartPlansProperties);
			plansViewModel.Initialize();
			_plansViewModels.Add(plansViewModel);
			return plansViewModel;
		}
		private void OnRegisterPlanPresenter(IPlanPresenter<Plan, XStateClass> planPresenter)
		{
			if (!_planPresenters.Contains(planPresenter))
				_planPresenters.Add(planPresenter);
		}
	}
}