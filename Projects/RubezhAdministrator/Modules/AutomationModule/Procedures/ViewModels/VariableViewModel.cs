﻿using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class VariableViewModel : BaseViewModel
	{
		public Variable Variable { get; set; }

		public VariableViewModel(Variable variable)
		{
			Variable = variable;
		}

		public string ValueDescription
		{
			get { return Variable.ToString(); }
		}

		public string TypeDescription
		{
			get
			{
				if (Variable.ExplicitType == ExplicitType.Object)
					return Variable.ExplicitType.ToDescription() + " \\ " + Variable.ObjectType.ToDescription();
				if (Variable.ExplicitType == ExplicitType.Enum)
					return Variable.ExplicitType.ToDescription() + " \\ " + Variable.EnumType.ToDescription();
				return Variable.ExplicitType.ToDescription();
			}
		}

		public override string ToString()
		{
			if (Variable != null)
				return Variable.Name;
			return "";
		}

		public void Update()
		{
			OnPropertyChanged(() => Variable);
			OnPropertyChanged(() => ValueDescription);
			OnPropertyChanged(() => TypeDescription);
		}
	}
}