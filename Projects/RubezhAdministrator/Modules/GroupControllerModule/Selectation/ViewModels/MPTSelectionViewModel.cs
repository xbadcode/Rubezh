﻿using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class MPTSelectionViewModel : SaveCancelDialogViewModel
	{
		public MPTSelectionViewModel(GKMPT mpt)
		{
			Title = "Выбор направления";
			MPTs = new ObservableCollection<MPTViewModel>();
			GKManager.MPTs.ForEach(x => MPTs.Add(new MPTViewModel(x)));
			if (mpt != null)
				SelectedMPT = MPTs.FirstOrDefault(x => x.MPT.UID == mpt.UID);
			if (SelectedMPT == null)
				SelectedMPT = MPTs.FirstOrDefault();
		}

		public ObservableCollection<MPTViewModel> MPTs { get; private set; }

		MPTViewModel _selectedMPT;
		public MPTViewModel SelectedMPT
		{
			get { return _selectedMPT; }
			set
			{
				_selectedMPT = value;
				OnPropertyChanged(() => SelectedMPT);
			}
		}
	}
}