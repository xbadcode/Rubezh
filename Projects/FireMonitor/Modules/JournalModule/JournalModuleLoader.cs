using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.Journal;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using JournalModule.ViewModels;
using Infrastructure.Common.Reports;
using JournalModule.Reports;

namespace JournalModule
{
	public class JournalModuleLoader : ModuleBase, IReportProviderModule, ILayoutProviderModule
	{
		private NavigationItem _journalNavigationItem;
		JournalViewModel JournalViewModel;
		ArchiveViewModel ArchiveViewModel;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
			JournalViewModel = new JournalViewModel();
			ArchiveViewModel = new ArchiveViewModel();
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Unsubscribe(OnShowArchive);
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(OnShowArchive);
		}

		int _unreadJournalCount;
		private int UnreadJournalCount
		{
			get { return _unreadJournalCount; }
			set
			{
				_unreadJournalCount = value;
				if (_journalNavigationItem != null)
					_journalNavigationItem.Title = UnreadJournalCount == 0 ? "Журнал событий" : string.Format("Журнал событий {0}", UnreadJournalCount);
			}
		}

		void OnShowJournal(object obj)
		{
			UnreadJournalCount = 0;
			JournalViewModel.SelectedJournal = JournalViewModel.JournalItems.FirstOrDefault();
		}

		void OnShowArchive(List<Guid> objectUIDs)
		{
			if (objectUIDs != null)
			{
				ArchiveViewModel.Sort(objectUIDs);
			}
		}

		public override void Initialize()
		{
			JournalViewModel.Initialize();
			ArchiveViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_journalNavigationItem = new NavigationItem<ShowJournalEvent>(JournalViewModel, "Журнал событий", "Book");
			UnreadJournalCount = 0;
			return new List<NavigationItem>()
			{
				_journalNavigationItem,
				new NavigationItem<ShowArchiveEvent, List<Guid>>(ArchiveViewModel, "Архив", "Archive")
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Journal; }
		}

		#region IReportProviderModule Members
		public IEnumerable<IReportProvider> GetReportProviders()
		{
			return new List<IReportProvider>()
			{
#if DEBUG
				new JournalReport(),
#endif
			};
		}
		#endregion

		public override void AfterInitialize()
		{
			SafeFiresecService.NewJournalItemsEvent -= new Action<List<JournalItem>>(OnNewJournalItems);
			SafeFiresecService.NewJournalItemsEvent += new Action<List<JournalItem>>(OnNewJournalItems);

			var journalFilter = new JournalFilter();
			var result = ClientManager.FiresecService.GetFilteredJournalItems(journalFilter);
			if (!result.HasError)
			{
				JournalViewModel.SetJournalItems(result.Result);
			}
			ArchiveViewModel.Update();
		}

		void OnNewJournalItems(List<JournalItem> journalItems)
		{
			ApplicationService.Invoke(() =>
			{
				if (_journalNavigationItem == null || !_journalNavigationItem.IsSelected)
					UnreadJournalCount += journalItems.Count;

				ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Publish(journalItems);
			});
		}

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Journal, "Журнал событий", "Book.png", (p) =>
			{
				var layoutPartJournalProperties = p as LayoutPartReferenceProperties;
				var filter = ClientManager.SystemConfiguration.JournalFilters.FirstOrDefault(x => x.UID == layoutPartJournalProperties.ReferenceUID);
				if(filter == null)
					filter = new JournalFilter();

				var journalViewModel = new JournalViewModel(filter);
				journalViewModel.Initialize();
				var result = ClientManager.FiresecService.GetFilteredJournalItems(filter);
				if (!result.HasError)
				{
					journalViewModel.SetJournalItems(result.Result);
				}

				return journalViewModel;
			});
			yield return new LayoutPartPresenter(LayoutPartIdentities.Archive, "Архив", "Archive.png", (p) => ArchiveViewModel);
		}

		#endregion
	}
}