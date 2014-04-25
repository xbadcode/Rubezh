﻿using System;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class PositionDetailsViewModel : SaveCancelDialogViewModel
	{
		public Position Position { get; private set; }
		public ShortPosition ShortPosition
		{
			get
			{
				return new ShortPosition
				{
					UID = Position.UID,
					Name = Position.Name,
					Description = Position.Description,
					OrganisationUID = Position.OrganisationUID
				};
			}
		}

		Organisation Organisation { get; set; }

		public PositionDetailsViewModel(Organisation orgnaisation, Guid? positionUID = null)
		{
			Organisation = orgnaisation;
			if (positionUID == null)
			{
				Title = "Создание должности";
				Position = new Position()
				{
					Name = "Новая должность",
					OrganisationUID = Organisation.UID
				};
			}
			else
			{
				Position = PositionHelper.GetDetails(positionUID);
				Title = string.Format("Свойства должности: {0}", Position.Name);
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = Position.Name;
			Description = Position.Description;
			if (Position.Photo != null)
				PhotoData = Position.Photo.Data;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged("Description");
				}
			}
		}

		byte[] _photoData;
		public byte[] PhotoData
		{
			get { return _photoData; }
			set
			{
				_photoData = value;
				OnPropertyChanged(() => PhotoData);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			Position.Name = Name;
			Position.Description = Description;
			if (Position.Photo == null)
				Position.Photo = new Photo();
			Position.Photo.Data = PhotoData;
			Position.OrganisationUID = Organisation.UID;
			return PositionHelper.Save(Position);
		}
	}
}