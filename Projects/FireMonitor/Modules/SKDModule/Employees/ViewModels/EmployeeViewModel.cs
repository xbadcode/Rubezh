﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : OrganisationElementViewModel<EmployeeViewModel, ShortEmployee>
	{
		public string DepartmentName
		{
			get
			{
				var isWithDeleted = (ParentViewModel as EmployeesViewModel).IsWithDeleted;

				if (!IsOrganisation && (isWithDeleted || !IsDepartmentDeleted))
					return IsOrganisation ? string.Empty : Model.DepartmentName;

				return string.Empty;
			}
		}
		public string PositionName
		{
			get
			{
				var isWithDeleted = (ParentViewModel as EmployeesViewModel).IsWithDeleted;

				if (!IsOrganisation && (isWithDeleted || !IsPositionDeleted))
					return Model.PositionName;

				return string.Empty;
			}
		}
		public string FirstName
		{
			get { return IsOrganisation ? string.Empty : Model.FirstName; }
		}
		public string SecondName
		{
			get { return IsOrganisation ? string.Empty : Model.SecondName; }
		}
		public string LastName
		{
			get { return IsOrganisation ? string.Empty : Model.LastName; }
		}
		public Country? Country
		{
			get { return IsOrganisation ? null : Model.Country; }
		}
		public bool HasCountry
		{
			get
			{
				return Country.HasValue;
			}
		}

		public string Phone
		{
			get { return IsOrganisation ? string.Empty : Model.Phone; }
		}
		public string OrganisationName
		{
			get { return IsOrganisation ? string.Empty : Model.OrganisationName; }
		}
		public bool IsDepartmentDeleted
		{
			get { return !IsOrganisation && (Model.IsDepartmentDeleted || IsOrganisationDeleted); }
		}
		public bool IsPositionDeleted
		{
			get { return !IsOrganisation && (Model.IsPositionDeleted || IsOrganisationDeleted); }
		}

		public string[] AdditionalColumnValues
		{
			get
			{
				var additionalColumnTypes = (ParentViewModel as EmployeesViewModel).AdditionalColumnTypes;
				var result = new string[additionalColumnTypes.Count];
				if (IsOrganisation)
					return result;
				var i = 0;
				foreach (var additionalColumnType in additionalColumnTypes)
				{
					var textColumn = Model.TextColumns.FirstOrDefault(x => x.ColumnTypeUID == additionalColumnType.UID);
					var columnValue = textColumn != null ? textColumn.Text : "";
					result[i] = columnValue;
					i++;
				}
				return result;
			}
		}

		public override void InitializeModel(Organisation organisation, ShortEmployee model, ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);

			Update();
		}

		public void InitializeCards()
		{
			EmployeeCardsViewModel = new EmployeeCardsViewModel(this);
		}

		public EmployeeCardsViewModel EmployeeCardsViewModel { get; private set; }


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
				var details = EmployeeHelper.GetDetails(Model.UID);
				if (details != null)
					photo = details.Photo;
			}
			Photo = new PhotoColumnViewModel(photo);
			OnPropertyChanged(() => Photo);
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => FirstName);
			OnPropertyChanged(() => SecondName);
			OnPropertyChanged(() => LastName);
			OnPropertyChanged(() => Country);
			OnPropertyChanged(() => HasCountry);
			OnPropertyChanged(() => OrganisationName);
			OnPropertyChanged(() => Phone);
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => DepartmentName);
			OnPropertyChanged(() => PositionName);
			OnPropertyChanged(() => AdditionalColumnValues);
			OnPropertyChanged(() => IsDepartmentDeleted);
			OnPropertyChanged(() => IsPositionDeleted);
			if (IsOrganisation)
			{
				foreach (var child in Children)
				{
					child.Update();
				}
			}
		}

		public bool IsGuest { get { return !IsOrganisation && Model.Type == PersonType.Guest; } }

		public PersonType PersonType
		{
			get { return (ParentViewModel as EmployeesViewModel).PersonType; }
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

		public IEnumerable<SKDCard> Cards { get { return EmployeeCardsViewModel != null ? EmployeeCardsViewModel.Cards.Select(x => x.Card) : new List<SKDCard>(); } }
	}
}