﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : TreeNodeViewModel<EmployeeViewModel>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public ShortEmployee ShortEmployee { get; set; }
		public string Name 
		{
			get 
			{
				if (IsOrganisation)
					return Organisation.Name;
				else
					return ShortEmployee.LastName + " " + ShortEmployee.FirstName + " " + ShortEmployee.SecondName; 
			} 
		}
		public Guid? DepartmentPhotoUID { get; set; }
		public Guid? PositionPhotoUID { get; set; }
		public string AppointedString { get; private set; }
		public string DismissedString { get; private set; }
		public string DepartmentName { get; private set; }
		public string PositionName { get; private set; }

		public EmployeeViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsExpanded = true;
		}

		public EmployeeViewModel(Organisation organisation, ShortEmployee employee)
		{
			Organisation = organisation;
			ShortEmployee = employee;
			IsOrganisation = false;
			
			AddCardCommand = new RelayCommand(OnAddCard);
			SelectEmployeeCommand = new RelayCommand(OnSelectEmployee);

			Initialize();
		}

		private void Initialize()
		{
			Cards = new ObservableCollection<EmployeeCardViewModel>();
			foreach (var item in ShortEmployee.Cards)
				Cards.Add(new EmployeeCardViewModel(Organisation, this, item));
			SelectedCard = Cards.FirstOrDefault();

			AppointedString = ShortEmployee.Appointed;
			DismissedString = ShortEmployee.Dismissed;
			DepartmentName = ShortEmployee.DepartmentName;
			PositionName = ShortEmployee.PositionName;
		}

		public void Update(ShortEmployee employee)
		{
			ShortEmployee = employee;
			Initialize();
			OnPropertyChanged(() => ShortEmployee);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => DepartmentName);
			OnPropertyChanged(() => PositionName);
			OnPropertyChanged(() => AppointedString);
			OnPropertyChanged(() => DismissedString);
		}

		public PhotoColumnViewModel Photo { get; private set; }

		public void UpdatePhoto()
		{
			Photo photo = null;
			if (IsOrganisation)
			{
				var details = OrganisationHelper.GetDetails(Organisation.UID);
				if (details != null)
					photo = details.Photo;
			}
			else
			{
				var details = EmployeeHelper.GetDetails(ShortEmployee.UID);
				if (details != null)
					photo = details.Photo;
			}
			Photo = new PhotoColumnViewModel(photo);
			OnPropertyChanged(() => Photo);
		}

		public ObservableCollection<string> AdditionalColumnValues { get; set; }

		#region Cards
		public ObservableCollection<EmployeeCardViewModel> Cards { get; private set; }

		EmployeeCardViewModel _selectedCard;
		public EmployeeCardViewModel SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged(() => SelectedCard);
			}
		}

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			if (Cards.Count > 100)
			{
				MessageBoxService.ShowWarning("У сотрудника не может быть более 100 пропусков");
				return;
			}
			var cardDetailsViewModel = new EmployeeCardDetailsViewModel(Organisation);
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				card.HolderUID = ShortEmployee.UID;
				var saveResult = CardHelper.Add(card);
				if (!saveResult)
					return;
				var cardViewModel = new EmployeeCardViewModel(Organisation, this, card);
				Cards.Add(cardViewModel);
				SelectedCard = cardViewModel;
				SelectCard(cardViewModel);
			}
		}

		bool _isEmployeeSelected = true;
		public bool IsEmployeeSelected
		{
			get { return _isEmployeeSelected; }
			set
			{
				_isEmployeeSelected = value;
				OnPropertyChanged(() => IsEmployeeSelected);
			}
		}

		bool _isCardSelected;
		public bool IsCardSelected
		{
			get { return _isCardSelected; }
			set
			{
				_isCardSelected = value;
				OnPropertyChanged(() => IsCardSelected);
			}
		}

		public RelayCommand SelectEmployeeCommand { get; private set; }
		public void OnSelectEmployee()
		{
			IsEmployeeSelected = !IsOrganisation;
			IsCardSelected = false;
			foreach (var card in Cards)
			{
				card.IsCardSelected = false;
			}
		}

		public void SelectCard(EmployeeCardViewModel employeeCardViewModel)
		{
			IsEmployeeSelected = false;
			IsCardSelected = true;
			foreach (var card in Cards)
			{
				card.IsCardSelected = false;
			}
			employeeCardViewModel.IsCardSelected = true;
			SelectedCard = employeeCardViewModel;
		}
		#endregion
	}
}