﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace AutomationModule.ViewModels
{
	public class SoundStepViewModel : BaseStepViewModel
	{
		public SoundArguments SoundArguments { get; private set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }

		public SoundStepViewModel(SoundArguments soundArguments, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			UpdateDescriptionHandler = updateDescriptionHandler;
			SoundArguments = soundArguments;
			UpdateContent();
		}

		public override string Description
		{
			get { return SelectedSound == null ? "нет" : SelectedSound.Name; }
		}

		public override void UpdateContent()
		{
			Sounds = new ObservableCollection<SoundViewModel>();
			foreach (var sound in FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds)
			{
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
			}
			if (FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Any(x => x.Uid == SoundArguments.SoundUid))
				SelectedSound = Sounds.FirstOrDefault(x => x.Sound.Uid == SoundArguments.SoundUid);
			else
				SelectedSound = null;

			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(SoundArguments.ProcedureLayoutCollection);
			OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
			OnPropertyChanged(() => Sounds);
		}

		public ObservableCollection<SoundViewModel> Sounds { get; private set; }

		SoundViewModel _selectedSound;
		public SoundViewModel SelectedSound
		{
			get { return _selectedSound; }
			set
			{
				_selectedSound = value;
				if (value != null)
					SoundArguments.SoundUid = value.Sound.Uid;
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();

				OnPropertyChanged(() => SelectedSound);
			}
		}
	}
}