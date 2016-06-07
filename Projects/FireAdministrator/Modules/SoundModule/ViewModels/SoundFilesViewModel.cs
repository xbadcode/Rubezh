﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using StrazhAPI.Automation;

namespace SoundsModule.ViewModels
{
	public class SoundFilesViewModel : BaseViewModel
	{
		public SoundFilesViewModel()
		{
			PlaySoundCommand = new RelayCommand(OnPlaySound);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			Initialize();
		}

		private void Initialize()
		{
			Sounds = new ObservableCollection<SoundFileViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds == null)
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds = new List<AutomationSound>();
			foreach (var sound in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds)
			{
				var soundFileViewModel = new SoundFileViewModel(sound);
				Sounds.Add(soundFileViewModel);
			}
			SelectedSound = Sounds.FirstOrDefault();
		}

		ObservableCollection<SoundFileViewModel> _sounds;
		public ObservableCollection<SoundFileViewModel> Sounds
		{
			get { return _sounds; }
			set
			{
				_sounds = value;
				OnPropertyChanged(() => Sounds);
			}
		}

		SoundFileViewModel _selectedSound;
		public SoundFileViewModel SelectedSound
		{
			get { return _selectedSound; }
			set
			{
				_selectedSound = value;
				OnPropertyChanged(() => SelectedSound);
			}
		}

		bool _isNowPlaying;
		public bool IsNowPlaying
		{
			get { return _isNowPlaying; }
			set
			{
				_isNowPlaying = value;
				OnPropertyChanged(() => IsNowPlaying);
			}
		}

		public ICommand PlaySoundCommand { get; private set; }
		void OnPlaySound()
		{
			if (IsNowPlaying == false)
			{
				AlarmPlayerHelper.Play(FiresecClient.FileHelper.GetSoundFilePath
					(Path.Combine(ServiceFactoryBase.ContentService.ContentFolder, SelectedSound.Sound.Uid.ToString())), false);
				IsNowPlaying = false;
			}
			else
			{
				AlarmPlayerHelper.Stop();
				IsNowPlaying = false;
			}
		}

		public ICommand AddCommand { get; private set; }
		void OnAdd()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Звуковой файл |*.wav",
				DefaultExt = "Звуковой файл |*.wav"
			};
			if (openFileDialog.ShowDialog().Value)
			{
				var sound = new AutomationSound
				{
					Name = Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName),
					Uid = ServiceFactoryBase.ContentService.AddContent(openFileDialog.FileName)
				};
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Add(sound);
				ServiceFactory.SaveService.AutomationChanged = true;
				var soundFileViewModel = new SoundFileViewModel(sound);
				Sounds.Add(soundFileViewModel);
				SelectedSound = soundFileViewModel;
			}
		}

		bool CanAdd()
		{
			return true;
		}

		public ICommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (!MessageBoxService.ShowConfirmation("Удалить выбранный звук из системы?"))
				return;

			var index = Sounds.IndexOf(SelectedSound);
			FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Remove(SelectedSound.Sound);
			Sounds.Remove(SelectedSound);
			index = Math.Min(index, Sounds.Count - 1);
			if (index > -1)
				SelectedSound = Sounds[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public ICommand EditCommand { get; private set; }
		void OnEdit()
		{
			var soundFileDetailsViewModel = new SoundFileDetailsViewModel(SelectedSound.Sound);
			if (DialogService.ShowModalWindow(soundFileDetailsViewModel))
			{
				SelectedSound.Sound = soundFileDetailsViewModel.Sound;
				SelectedSound.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedSound != null;
		}

		public void Select(Guid soundUid)
		{
			if (soundUid != Guid.Empty)
			{
				SelectedSound = Sounds.FirstOrDefault(item => item.Sound.Uid == soundUid);
			}
		}
	}
}