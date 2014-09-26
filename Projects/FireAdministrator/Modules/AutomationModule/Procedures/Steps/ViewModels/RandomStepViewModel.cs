﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class RandomStepViewModel : BaseStepViewModel
	{
		public RandomArguments RandomArguments { get; private set; }
		public ArgumentViewModel MaxValueParameter { get; private set; }
		public ArgumentViewModel ResultParameter { get; private set; }

		public RandomStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			RandomArguments = stepViewModel.Step.RandomArguments;
			MaxValueParameter = new ArgumentViewModel(RandomArguments.MaxValueParameter, stepViewModel.Update);
			MaxValueParameter.ExplicitType = ExplicitType.Integer;
			ResultParameter = new ArgumentViewModel(RandomArguments.ResultParameter, stepViewModel.Update, false);
			ResultParameter.ExplicitType = ExplicitType.Integer;
		}

		public override void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList);
			MaxValueParameter.Update(allVariables);
			ResultParameter.Update(allVariables);
		}

		public override string Description
		{
			get 
			{
				return "Максимальное значение: " + MaxValueParameter.Description;
			}
		}
	}
}