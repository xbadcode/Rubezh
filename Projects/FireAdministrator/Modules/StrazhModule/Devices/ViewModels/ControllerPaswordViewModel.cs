﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Strazh.ViewModels;
using StrazhAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;

namespace StrazhModule.ViewModels
{
	public class ControllerPaswordViewModel : SaveCancelDialogViewModel
	{
		DeviceViewModel DeviceViewModel { get; set; }
		bool HasChanged { get; set; }

		public ControllerPaswordViewModel(DeviceViewModel deviceViewModel)
		{
			Title = CommonViewModels.ControllerPasword_Title;
			DeviceViewModel = deviceViewModel;
			SetPasswordCommand = new RelayCommand(OnSetPassword);
			AvailableLogins = new ObservableCollection<string>();
			AvailableLogins.Add("admin");
			AvailableLogins.Add("system");
			SelectedLogin = AvailableLogins.FirstOrDefault();

			var loginProperty = DeviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Login");
			if (loginProperty != null)
			{
				SelectedLogin = AvailableLogins.FirstOrDefault(x => x == loginProperty.StringValue);
			}
			var passwordProperty = DeviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Password");
			if (passwordProperty != null)
			{
				OldPassword = passwordProperty.StringValue;
			}
		}

		public ObservableCollection<string> AvailableLogins { get; private set; }

		string _selectedLogin;
		public string SelectedLogin
		{
			get { return _selectedLogin; }
			set
			{
				_selectedLogin = value;
				OnPropertyChanged(() => SelectedLogin);
			}
		}

		string _oldPassword;
		public string OldPassword
		{
			get { return _oldPassword; }
			set
			{
				_oldPassword = value;
				OnPropertyChanged(() => OldPassword);
			}
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		string _passwordConfirmation;
		public string PasswordConfirmation
		{
			get { return _passwordConfirmation; }
			set
			{
				_passwordConfirmation = value;
				OnPropertyChanged(() => PasswordConfirmation);
			}
		}

		public RelayCommand SetPasswordCommand { get; private set; }
		void OnSetPassword()
		{
			if (String.IsNullOrEmpty(Password))
			{
				MessageBoxService.ShowWarning(CommonViewModels.ControllerPasword_EmptyPass);
				return;
			}
			if (Password != PasswordConfirmation)
			{
				MessageBoxService.ShowWarning(CommonViewModels.ControllerPasword_PassNotConfirm);
				return;
			}

			var result = FiresecManager.FiresecService.SetControllerPassword(DeviceViewModel.Device, SelectedLogin, OldPassword, Password);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				HasChanged = true;
			}
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				var loginProperty = DeviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Login");
				if (loginProperty == null)
				{
                    MessageBoxService.ShowWarning(CommonViewModels.ControllerPasword_NoLogin);
					return false;
				}
				var passwordProperty = DeviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Password");
				if (passwordProperty == null)
				{
                    MessageBoxService.ShowWarning(CommonViewModels.ControllerPasword_NoPass);
					return false;
				}

				if (loginProperty.StringValue == SelectedLogin)
				{
                    if (MessageBoxService.ShowQuestion(CommonViewModels.ControllerPasword_Save))
					{
						passwordProperty.StringValue = Password;
						DeviceViewModel.UpdateProperties();
						ServiceFactory.SaveService.SKDChanged = true;
					}
				}
			}
			return base.Save();
		}
	}
}