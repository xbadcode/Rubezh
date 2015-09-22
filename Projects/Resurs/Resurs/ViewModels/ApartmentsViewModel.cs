﻿using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Processor;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ApartmentsViewModel : BaseViewModel
	{
		public ApartmentsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);

			BuildTree();
			if (RootApartment != null)
			{
				RootApartment.IsExpanded = true;
				SelectedApartment = RootApartment;
				foreach (var child in RootApartment.Children)
				{
					if (true)
					{
						child.IsExpanded = true;
					}
				}
			}

			foreach (var apartment in AllApartments)
			{
				if (true)
					apartment.ExpandToThis();
			}

			OnPropertyChanged(() => RootApartments);
		}

		ApartmentViewModel _selectedApartment;
		public ApartmentViewModel SelectedApartment
		{
			get { return _selectedApartment; }
			set
			{
				_selectedApartment = value;
				OnPropertyChanged(() => SelectedApartment);
			}
		}

		ApartmentViewModel _rootApartment;
		public ApartmentViewModel RootApartment
		{
			get { return _rootApartment; }
			private set
			{
				_rootApartment = value;
				OnPropertyChanged(() => RootApartment);
			}
		}

		public ApartmentViewModel[] RootApartments
		{
			get { return new[] { RootApartment }; }
		}

		void BuildTree()
		{
			RootApartment = AddApartmentInternal(DBCash.RootApartment, null);
			FillAllApartments();
		}

		public ApartmentViewModel AddApartment(Apartment apartment, ApartmentViewModel parentApartmentViewModel)
		{
			var apartmentViewModel = AddApartmentInternal(apartment, parentApartmentViewModel);
			FillAllApartments();
			return apartmentViewModel;
		}
		private ApartmentViewModel AddApartmentInternal(Apartment apartment, ApartmentViewModel parentApartmentViewModel)
		{
			var apartmentViewModel = new ApartmentViewModel(apartment);
			if (parentApartmentViewModel != null)
				parentApartmentViewModel.AddChild(apartmentViewModel);

			foreach (var childApartment in apartment.Children)
				AddApartmentInternal(childApartment, apartmentViewModel);
			return apartmentViewModel;
		}

		public List<ApartmentViewModel> AllApartments;

		public void FillAllApartments()
		{
			AllApartments = new List<ApartmentViewModel>();
			AddChildPlainApartments(RootApartment);
		}

		void AddChildPlainApartments(ApartmentViewModel parentViewModel)
		{
			AllApartments.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainApartments(childViewModel);
		}

		public void Select(Guid apartmentUID)
		{
			if (apartmentUID != Guid.Empty)
			{
				FillAllApartments();
				var apartmentViewModel = AllApartments.FirstOrDefault(x => x.Apartment.UID == apartmentUID);
				if (apartmentViewModel != null)
					apartmentViewModel.ExpandToThis();
				SelectedApartment = apartmentViewModel;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var apartmentDetailsViewModel = new ApartmentDetailsViewModel(null);
			if (DialogService.ShowModalWindow(apartmentDetailsViewModel))
			{
				var apartmentViewModel = new ApartmentViewModel(apartmentDetailsViewModel.Apartment);
				SelectedApartment.AddChild(apartmentViewModel);
				SelectedApartment.IsExpanded = true;
				AllApartments.Add(apartmentViewModel);
				SelectedApartment = apartmentViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedApartment != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var apartmentDetailsViewModel = new ApartmentDetailsViewModel(SelectedApartment.Apartment);
			if (DialogService.ShowModalWindow(apartmentDetailsViewModel))
			{
				var apartmentViewModel = new ApartmentViewModel(apartmentDetailsViewModel.Apartment);
				SelectedApartment.Update(apartmentDetailsViewModel.Apartment);
			}
		}
		bool CanEdit()
		{
			return SelectedApartment != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var selectedApartment = SelectedApartment;
			var parent = selectedApartment.Parent;
			if (parent != null)
			{
				var index = selectedApartment.VisualIndex;
				parent.Nodes.Remove(selectedApartment);
				index = Math.Min(index, parent.ChildrenCount - 1);
				foreach (var childApartmentViewModel in selectedApartment.GetAllChildren(true))
				{
					AllApartments.Remove(childApartmentViewModel);
				}
				SelectedApartment = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
		}
		bool CanRemove()
		{
			return SelectedApartment != null && SelectedApartment.Parent != null;
		}
	}
}