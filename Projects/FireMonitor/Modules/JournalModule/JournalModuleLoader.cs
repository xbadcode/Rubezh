﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using JournalModule.ViewModels;

namespace JournalModule
{
	public class JournalModuleLoader : ModuleBase
	{
		private NavigationItem _journalNavigationItem;
		JournalViewModel JournalViewModel;
		ArchiveViewModel ArchiveViewModel;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournalItem);
			JournalViewModel = new JournalViewModel();
			ArchiveViewModel = new ArchiveViewModel();
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
		void OnNewJournalItem(List<FiresecAPI.SKD.JournalItem> journalItems)
		{
			if (_journalNavigationItem == null || !_journalNavigationItem.IsSelected)
				UnreadJournalCount += journalItems.Count;
		}

		public override void Initialize()
		{
			JournalViewModel.Initialize();
			ArchiveViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_journalNavigationItem = new NavigationItem<ShowJournalEvent>(JournalViewModel, "Журнал событий", "/Controls;component/Images/book.png");
			UnreadJournalCount = 0;
			return new List<NavigationItem>()
			{
				_journalNavigationItem,
				new NavigationItem<ShowArchiveEvent>(ArchiveViewModel, "Архив", "/Controls;component/Images/archive.png")
			};
		}
		public override string Name
		{
			get { return "Журнал событий и Архив"; }
		}

		public override void AfterInitialize()
		{
			SafeFiresecService.NewJournalItemEvent -= new Action<FiresecAPI.SKD.JournalItem>(OnNewJournalItem);
			SafeFiresecService.NewJournalItemEvent += new Action<FiresecAPI.SKD.JournalItem>(OnNewJournalItem);
		}

		void OnNewJournalItem(FiresecAPI.SKD.JournalItem journalItem)
		{
			ApplicationService.Invoke(() =>
			{
				var journalItems = new List<JournalItem>();
				journalItems.Add(journalItem);
				ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Publish(journalItems);
			});
		}
	}
}