using System.Collections.Generic;
using System.Linq;
using RubezhAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Ribbon;
using Infrastructure.Events;
using SettingsModule.ViewModels;
using System.Collections.ObjectModel;
using System;
using Infrastructure.Common.Theme;

namespace SettingsModule
{
	public class SettingsModule : ModuleBase
	{
		SettingsViewModel SettingsViewModel;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<EditValidationEvent>().Subscribe(OnEditValidation);

			SettingsViewModel = new SettingsViewModel();
			ChangeThemeCommand = new RelayCommand<Theme>(OnChangeTheme, CanChangeTheme);
		}

		public override void Initialize()
		{
			SettingsViewModel.Initialize();
		}
		public override void AfterInitialize()
		{
			base.AfterInitialize();
			ServiceFactory.RibbonService.AddRibbonItems(new RibbonMenuItemViewModel(ModuleType.ToDescription(), new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Параметры", SettingsViewModel.ShowSettingsCommand, "BSettings"),
				new RibbonMenuItemViewModel("Сообщения об ошибках", SettingsViewModel.ShowErrorsFilterCommand, "BJournal"),
				new RibbonMenuItemViewModel("Выбор темы", 
					new ObservableCollection<RibbonMenuItemViewModel>(Enum.GetValues(typeof(Theme)).Cast<Theme>().Select(t=>new RibbonMenuItemViewModel(t.ToDescription(), ChangeThemeCommand, t, "BLayouts"))),
					"BLayouts"),
			}, "BSettings", "Настройка приложения") { Order = int.MaxValue - 1 });
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return null;
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Settings; }
		}

		void OnEditValidation(object obj)
		{
			SettingsViewModel.ShowErrorsFilterCommand.Execute();
		}

		public RelayCommand<Theme> ChangeThemeCommand { get; private set; }
		private void OnChangeTheme(Theme theme)
		{
			ThemeHelper.SetThemeIntoRegister(theme);
			ThemeHelper.LoadThemeFromRegister();
		}
		private bool CanChangeTheme(Theme theme)
		{
			return theme.ToString() != ThemeHelper.CurrentTheme;
		}
	}
}