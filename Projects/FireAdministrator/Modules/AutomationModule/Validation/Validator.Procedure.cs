﻿using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using FiresecAPI.Automation;
using AutomationModule.ViewModels;
using System;
using System.Linq;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		Procedure Procedure { get; set; }

		void ValidateProcedureName()
		{
			var nameList = new List<string>();
			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				Procedure = procedure;
				foreach (var procedureStep in procedure.Steps)
					ValidateStep(procedureStep);

				if (nameList.Contains(procedure.Name))
					Errors.Add(new ProcedureValidationError(procedure, "Процедура с таким именем уже существует " + procedure.Name, ValidationErrorLevel.CannotSave));
				nameList.Add(procedure.Name);
				
				var varList = new List<string>();
				foreach (var variable in procedure.Variables)
				{
					if (varList.Contains(variable.Name))
						Errors.Add(new VariableValidationError(variable, "Переменная с таким именем уже существует " + variable.Name, ValidationErrorLevel.CannotSave));
					varList.Add(variable.Name);
				}

				var argList = new List<string>();
				foreach (var argument in procedure.Arguments)
				{
					if (argList.Contains(argument.Name))
						Errors.Add(new VariableValidationError(argument, "Аргумент с таким именем уже существует " + argument.Name, ValidationErrorLevel.CannotSave));
					argList.Add(argument.Name);
				}
			}
		}

		void ValidateStep(ProcedureStep step)
		{
			switch (step.ProcedureStepType)
			{
				case ProcedureStepType.PlaySound:
					ValidateStepArguments(step.SoundArguments, step);
					break;

				case ProcedureStepType.ShowMessage:
					ValidateStepArguments(step.ShowMessageArguments, step);
					break;

				case ProcedureStepType.GetString:
					ValidateStepArguments(step.GetStringArguments, step);
					break;

				case ProcedureStepType.Arithmetics:
					ValidateStepArguments(step.ArithmeticArguments, step);
					break;

				case ProcedureStepType.If:
					ValidateStepArguments(step.ConditionArguments, step);
					break;

				case ProcedureStepType.AddJournalItem:
					ValidateStepArguments(step.JournalArguments, step);
					break;

				case ProcedureStepType.FindObjects:
					ValidateStepArguments(step.FindObjectArguments, step);
					break;

				case ProcedureStepType.Foreach:
					ValidateStepArguments(step.ForeachArguments, step);
					break;

				case ProcedureStepType.Pause:
					ValidateStepArguments(step.PauseArguments, step);
					break;

				case ProcedureStepType.ProcedureSelection:
					ValidateStepArguments(step.ProcedureSelectionArguments, step);
					break;

				case ProcedureStepType.Exit:
					ValidateStepArguments(step.ExitArguments, step);
					break;

				case ProcedureStepType.PersonInspection:
					ValidateStepArguments(step.PersonInspectionArguments, step);
					break;

				case ProcedureStepType.SetValue:
					ValidateStepArguments(step.SetValueArguments, step);
					break;

				case ProcedureStepType.IncrementValue:
					ValidateStepArguments(step.IncrementValueArguments, step);
					break;

				case ProcedureStepType.ControlGKDevice:
					ValidateStepArguments(step.ControlGKDeviceArguments, step);
					break;

				case ProcedureStepType.ControlSKDDevice:
					ValidateStepArguments(step.ControlSKDDeviceArguments, step);
					break;

				case ProcedureStepType.ControlGKFireZone:
					ValidateStepArguments(step.ControlGKFireZoneArguments, step);
					break;

				case ProcedureStepType.ControlGKGuardZone:
					ValidateStepArguments(step.ControlGKGuardZoneArguments, step);
					break;

				case ProcedureStepType.ControlDirection:
					ValidateStepArguments(step.ControlDirectionArguments, step);
					break;

				case ProcedureStepType.ControlDoor:
					ValidateStepArguments(step.ControlDoorArguments, step);
					break;

				case ProcedureStepType.ControlSKDZone:
					ValidateStepArguments(step.ControlSKDZoneArguments, step);
					break;

				case ProcedureStepType.ControlCamera:
					ValidateStepArguments(step.ControlCameraArguments, step);
					break;

				case ProcedureStepType.GetObjectField:
					ValidateStepArguments(step.GetObjectFieldArguments, step);
					break;

				case ProcedureStepType.SendEmail:
					ValidateStepArguments(step.SendEmailArguments, step);
					break;

				case ProcedureStepType.RunProgramm:
					ValidateStepArguments(step.RunProgrammArguments, step);
					break;
			}
		}

		void ValidateStepArguments(object stepArguments, ProcedureStep step)
		{
			var props = stepArguments.GetType().GetProperties();
			foreach (var prop in props)
			{
				if (prop.PropertyType == typeof(ArithmeticParameter))
				{
					var arithmeticParameter = (ArithmeticParameter)prop.GetValue(stepArguments, null);
					if (arithmeticParameter.VariableType != VariableType.IsValue && arithmeticParameter.VariableUid == Guid.Empty)
					{
						Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
						return;
					}
				}
			}
		}
	}
}