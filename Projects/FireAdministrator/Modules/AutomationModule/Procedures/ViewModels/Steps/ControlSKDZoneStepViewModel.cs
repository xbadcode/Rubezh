﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlSKDZoneStepViewModel: BaseStepViewModel
	{
		ControlSKDZoneArguments ControlSKDZoneArguments { get; set; }
		public ArithmeticParameterViewModel SKDZoneParameter { get; private set; }

		public ControlSKDZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlSKDZoneArguments = stepViewModel.Step.ControlSKDZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<SKDZoneCommandType>();
			SKDZoneParameter = new ArithmeticParameterViewModel(ControlSKDZoneArguments.SKDZoneParameter, stepViewModel.Update);
			SKDZoneParameter.ObjectType = ObjectType.SKDZone;
			SKDZoneParameter.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlSKDZoneArguments.SKDZoneCommandType;
			UpdateContent();
		}

		public ObservableCollection<SKDZoneCommandType> Commands { get; private set; }
		SKDZoneCommandType _selectedCommand;
		public SKDZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlSKDZoneArguments.SKDZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			SKDZoneParameter.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.SKDZone, false));
		}

		public override string Description
		{
			get
			{
				return "Зона: " + SKDZoneParameter.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
