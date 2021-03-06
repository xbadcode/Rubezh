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
using RubezhAPI.Models;
using RubezhAPI.GK;
using RubezhAPI;

namespace JournalModule
{
	public class JournalModuleLoader : ModuleBase, IReportProviderModule, ILayoutProviderModule
	{
		NavigationItem _journalNavigationItem;
		JournalViewModel _journalViewModel;
		ArchiveViewModel _archiveViewModel;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
			_journalViewModel = new JournalViewModel();
			_archiveViewModel = new ArchiveViewModel();
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Unsubscribe(OnShowArchive);
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(OnShowArchive);
		}

		int _unreadJournalCount;

		int UnreadJournalCount
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
			_journalViewModel.SelectedJournal = _journalViewModel.JournalItems.FirstOrDefault();
		}

		void OnShowArchive(List<Guid> objectUIDs)
		{
			if (objectUIDs != null)
			{
				_archiveViewModel.Sort(objectUIDs);
			}
		}

		public override void Initialize()
		{
			_journalViewModel.Initialize();
			_archiveViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_journalNavigationItem = new NavigationItem<ShowJournalEvent>(_journalViewModel, "Журнал событий", "Book");
			UnreadJournalCount = 0;
			return new List<NavigationItem>()
			{
				_journalNavigationItem,
				new NavigationItem<ShowArchiveEvent, List<Guid>>(_archiveViewModel, "Архив", "Archive")
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
			SafeRubezhService.JournalItemsEvent -= new Action<List<JournalItem>, bool>(OnJournalItems);
			SafeRubezhService.JournalItemsEvent += new Action<List<JournalItem>, bool>(OnJournalItems);
			
			_journalViewModel.SetJournalItems();
			_archiveViewModel.Update();
		}

		void OnJournalItems(List<JournalItem> journalItems, bool isNew)
		{
			ApplicationService.Invoke(() =>
			{
				if (isNew)
				{
					if (_journalNavigationItem == null || !_journalNavigationItem.IsSelected)
						UnreadJournalCount += journalItems.Count;

					ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Publish(journalItems);

					foreach (var journalItem in journalItems)
					{
						var stateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
						if (ClientManager.CheckPermission(PermissionType.Oper_NoAlarmConfirm) == false)
						{
							if (((journalItem.JournalObjectType == JournalObjectType.GKZone || journalItem.JournalObjectType == JournalObjectType.GKDirection) &&
								(stateClass == XStateClass.Fire1 || stateClass == XStateClass.Fire2 || stateClass == XStateClass.Attention)) ||
								((journalItem.JournalObjectType == JournalObjectType.GKGuardZone || journalItem.JournalObjectType == JournalObjectType.GKDoor) && stateClass == XStateClass.Fire1))
							{
								var confirmationViewModel = new ConfirmationViewModel(journalItem);
								DialogService.ShowWindow(confirmationViewModel);
							}
						}
					}
				}
				else
					ServiceFactory.Events.GetEvent<UpdateJournalItemsEvent>().Publish(journalItems);

			});
		}

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Journal, "Журнал событий", "BBook.png", (p) =>
			{
				var layoutPartJournalProperties = p as LayoutPartJournalProperties;
				var filter = ClientManager.SystemConfiguration.JournalFilters.FirstOrDefault(x => x.UID == layoutPartJournalProperties.FilterUID);
				if (filter == null)
					filter = new JournalFilter();

				var journalViewModel = new JournalViewModel(filter);
				journalViewModel.IsVisibleBottomPanel = layoutPartJournalProperties.IsVisibleBottomPanel;
				journalViewModel.Initialize();
				journalViewModel.SetJournalItems();
				
				return journalViewModel;
			});
			yield return new LayoutPartPresenter(LayoutPartIdentities.Archive, "Архив", "BArchive.png", (p) => _archiveViewModel);
		}

		#endregion
	}
}