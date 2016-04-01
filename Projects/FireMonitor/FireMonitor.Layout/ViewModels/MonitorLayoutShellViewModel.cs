using FireMonitor.ViewModels;
using FiresecAPI.AutomationCallback;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using LayoutModel = FiresecAPI.Models.Layouts.Layout;

namespace FireMonitor.Layout.ViewModels
{
	public class MonitorLayoutShellViewModel : MonitorShellViewModel
	{
		public MonitorLayoutShellViewModel(FiresecAPI.Models.Layouts.Layout layout)
			: base(ClientType.Monitor)
		{
			Layout = layout;
			LayoutContainer = new LayoutContainer(this, layout);
			LayoutContainer.LayoutChanging += LayoutChanging;
			ChangeUserCommand = new RelayCommand(OnChangeUser, CanChangeUser);
			ChangeLayoutCommand = new RelayCommand<LayoutModel>(OnChangeLayout, CanChangeLayout);
		}

		public LayoutContainer LayoutContainer { get; private set; }
		private void LayoutChanging(object sender, EventArgs e)
		{
			Layout = LayoutContainer.Layout;
			UpdateRibbon();
		}

		private void UpdateRibbon()
		{
			if (Layout.IsRibbonEnabled)
			{
				RibbonContent = new RibbonMenuViewModel();
				var ribbonViewModel = new RibbonViewModel
				{
					Content = RibbonContent,
				};
				ribbonViewModel.PopupOpened += (s, e) => UpdateRibbonItems();
				ribbonViewModel.LogoSource = "Logo";
				HeaderMenu = ribbonViewModel;
				AddRibbonItem();
				AllowLogoIcon = false;
			}
			else
			{
				HeaderMenu = null;
				AllowLogoIcon = true;
			}
			RibbonVisible = false;
		}
		private void UpdateRibbonItems()
		{
			//RibbonContent.Items[2][2].ImageSource = _soundViewModel.IsSoundOn ? "BSound" : "BMute";
			//RibbonContent.Items[2][2].ToolTip = _soundViewModel.IsSoundOn ? "Звук включен" : "Звук выключен";
			//RibbonContent.Items[2][2].Text = _soundViewModel.IsSoundOn ? "Выключить звук" : "Включить звук";
		}
		private void AddRibbonItem()
		{
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить пользователя", ChangeUserCommand, "BUser"));

			var ip = ConnectionSettingsManager.IsRemote ? null : FiresecManager.GetIP();
			var layouts = FiresecManager.LayoutsConfiguration.Layouts.Where(layout => layout.Users.Contains(FiresecManager.CurrentUser.UID) && (ip == null || layout.HostNameOrAddressList.Contains(ip))).OrderBy(item => item.Caption);
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить шаблон", new ObservableCollection<RibbonMenuItemViewModel>(layouts.Select(item => new RibbonMenuItemViewModel(item.Caption, ChangeLayoutCommand, item, "BLayouts", item.Description))), "BLayouts"));

			if (AllowClose)
				RibbonContent.Items.Add(new RibbonMenuItemViewModel("Выход", ApplicationCloseCommand, "BExit") { Order = int.MaxValue });
		}

		public RelayCommand ChangeUserCommand { get; private set; }
		private void OnChangeUser()
		{
			ApplicationService.ShutDown();
			Process.Start(Application.ResourceAssembly.Location);
		}
		private bool CanChangeUser()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Logout);
		}

		public RelayCommand<LayoutModel> ChangeLayoutCommand { get; private set; }
		private void OnChangeLayout(LayoutModel layout)
		{
			ApplicationService.CloseAllWindows();
			LayoutContainer.UpdateLayout(layout);
		}
		private bool CanChangeLayout(LayoutModel layout)
		{
			return layout != Layout;
		}

		public void ListenAutomationEvents()
		{
			SafeFiresecService.AutomationEvent -= OnAutomationCallback;
			SafeFiresecService.AutomationEvent += OnAutomationCallback;
		}
		private void OnAutomationCallback(AutomationCallbackResult automationCallbackResult)
		{
			if (automationCallbackResult.AutomationCallbackType == AutomationCallbackType.GetVisualProperty || automationCallbackResult.AutomationCallbackType == AutomationCallbackType.SetVisualProperty)
			{
				var visuaPropertyData = (VisualPropertyCallbackData)automationCallbackResult.Data;
				var layoutPart = LayoutContainer.LayoutParts.FirstOrDefault(item => item.UID == visuaPropertyData.LayoutPart);
				if (layoutPart != null)
				{
					var sendResponse = false;
					object value = null;
					ApplicationService.Invoke(() =>
					{
						switch (automationCallbackResult.AutomationCallbackType)
						{
							case AutomationCallbackType.GetVisualProperty:
								value = layoutPart.GetProperty(visuaPropertyData.Property);
								sendResponse = true;
								break;
							case AutomationCallbackType.SetVisualProperty:
								layoutPart.SetProperty(visuaPropertyData.Property, visuaPropertyData.Value);
								break;
						}
					});
					if (sendResponse)
						FiresecManager.FiresecService.ProcedureCallbackResponse(automationCallbackResult.CallbackUID, value);
				}
			}
			else if (automationCallbackResult.AutomationCallbackType == AutomationCallbackType.Dialog)
				LayoutDialogViewModel.Show((DialogCallbackData)automationCallbackResult.Data);
		}
	}
}