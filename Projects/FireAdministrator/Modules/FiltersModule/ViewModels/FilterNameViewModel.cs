﻿using System.Linq;
using System.Reflection;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using Infrastructure.Common.TreeList;
using Controls.Converters;

namespace FiltersModule.ViewModels
{
	public class FilterNameViewModel : TreeNodeViewModel<FilterNameViewModel>
	{
		public FilterNameViewModel(JournalSubsystemType journalSubsystemType)
		{
			JournalSubsystemType = journalSubsystemType;
			IsSubsystem = true;
			Name = journalSubsystemType.ToDescription();
			var converter = new JournalSubsystemTypeToIconConverter();
			ImageSource = (string)converter.Convert(journalSubsystemType, typeof(JournalSubsystemType), null, null);
		}

		public FilterNameViewModel(JournalEventNameType journalEventNameType)
		{
			JournalEventNameType = journalEventNameType;

			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				EventNameAttribute[] descriptionAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventNameAttribute eventDescriptionAttribute = descriptionAttributes[0];
					Name = eventDescriptionAttribute.Name;
					JournalSubsystemType = eventDescriptionAttribute.JournalSubsystemType;
					StateClass = eventDescriptionAttribute.StateClass;
					if (StateClass == XStateClass.Norm)
						ImageSource = null;
					else
						ImageSource = "/Controls;component/StateClassIcons/" + StateClass.ToString() + ".png";
				}
			}
			IsSubsystem = false;
		}

		public JournalEventNameType JournalEventNameType { get; private set; }
		public string Name { get; private set; }
		public string ImageSource { get; private set; }
		public XStateClass StateClass { get; private set; }
		public JournalSubsystemType JournalSubsystemType { get; private set; }
		public bool IsSubsystem { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);

				if (IsSubsystem)
				{
					foreach (var child in Children)
					{
						child.SetIsChecked(value);
					}
				}
				else if (Parent != null)
				{
					var isAllChecked = Parent.Children.All(x => x.IsChecked == true);
					Parent.SetIsChecked(isAllChecked);
				}
			}
		}

		public void SetIsChecked(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}
	}
}