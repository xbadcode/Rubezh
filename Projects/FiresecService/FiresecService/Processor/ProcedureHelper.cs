﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using FiresecService.Service;
using Infrastructure.Common.Video.RVI_VSS;
using ValueType = FiresecAPI.Automation.ValueType;

namespace FiresecService.Processor
{
	public static class ProcedureHelper
	{

		static ProcedureHelper()
		{
			WinFormsPlayers = new List<WinFormsPlayer>();
		}

		public static void Calculate(ProcedureStep procedureStep, Procedure procedure, List<Argument> arguments)
		{
			var arithmeticArguments = procedureStep.ArithmeticArguments;
			var variable1 = GetValue(arithmeticArguments.Variable1, procedure, arguments);
			var variable2 = GetValue(arithmeticArguments.Variable2, procedure, arguments);
			int result = 0;
			if (arithmeticArguments.ArithmeticType == ArithmeticType.Add)
				result = variable1 + variable2;
			if (arithmeticArguments.ArithmeticType == ArithmeticType.Sub)
				result = variable1 - variable2;
			if (arithmeticArguments.ArithmeticType == ArithmeticType.Multi)
				result = variable1 * variable2;
			if ((arithmeticArguments.ArithmeticType == ArithmeticType.Div) && (variable2 != 0))
				result = variable1 / variable2;

			if (arithmeticArguments.Result.ValueType == ValueType.IsGlobalVariable)
			{
				var globalVariable = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables
					.FirstOrDefault(x => x.Uid == arithmeticArguments.Result.GlobalVariableUid);
				if (globalVariable != null)
					globalVariable.Value = result;
			}

			if (arithmeticArguments.Result.ValueType == ValueType.IsLocalVariable)
			{
				var localVariable = procedure.Variables.FirstOrDefault(x => x.Uid == arithmeticArguments.Result.VariableUid) ??
					procedure.Arguments.FirstOrDefault(x => x.Uid == arithmeticArguments.Result.VariableUid);
				if (localVariable != null)
					localVariable.IntValue = result;
			}
		}

		static int GetValue(ArithmeticParameter arithmeticParameter, Procedure procedure, List<Argument> arguments)
		{
			if (arithmeticParameter.ValueType == ValueType.IsGlobalVariable)
			{
				var globalVariable = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables
					.FirstOrDefault(
						x => x.Uid == arithmeticParameter.GlobalVariableUid);
				if (globalVariable != null)
					return globalVariable.Value;
			}
			if (arithmeticParameter.ValueType == ValueType.IsLocalVariable)
			{
				var localVariable = procedure.Variables.FirstOrDefault(x => x.Uid == arithmeticParameter.VariableUid) ??
					procedure.Arguments.FirstOrDefault(x => x.Uid == arithmeticParameter.VariableUid);
				if (localVariable != null)
				{
					var argument = arguments.FirstOrDefault(x => x.ArgumentUid == localVariable.Uid);
					if (argument != null)
						return argument.IntValue;
					return localVariable.IntValue;
				}
			}
			return arithmeticParameter.Value;
		}

		public static void FindObjects(ProcedureStep procedureStep, Procedure procedure)
		{
			var findObjectArguments = procedureStep.FindObjectArguments;
			var variable = procedure.Variables.FirstOrDefault(x => x.Uid == findObjectArguments.ResultUid);
			if (findObjectArguments.JoinOperator == JoinOperator.Or)
				FindObjectsOr(variable, findObjectArguments.FindObjectConditions);
			else
				FindObjectsAnd(variable, findObjectArguments.FindObjectConditions);
		}

		static void InitializeProperties(ref int intPropertyValue, ref string stringPropertyValue, ref Guid itemUid, FindObjectCondition findObjectCondition, object item)
		{
			if (item is XDevice)
			{
				switch (findObjectCondition.DevicePropertyType)
				{
					case DevicePropertyType.ShleifNo:
						intPropertyValue = (item as XDevice).ShleifNo;
						break;
					case DevicePropertyType.IntAddress:
						intPropertyValue = (item as XDevice).IntAddress;
						break;
					case DevicePropertyType.DeviceState:
						intPropertyValue = (int)(item as XDevice).State.StateClass;
						break;
					case DevicePropertyType.Name:
						stringPropertyValue = (item as XDevice).PresentationName.Trim();
						break;
				}
				itemUid = (item as XDevice).UID;
			}

			if (item is XZone)
			{
				switch (findObjectCondition.ZonePropertyType)
				{
					case ZonePropertyType.No:
						intPropertyValue = (item as XZone).No;
						break;
					case ZonePropertyType.ZoneType:
						intPropertyValue = (int)(item as XZone).ObjectType;
						break;
					case ZonePropertyType.Name:
						stringPropertyValue = (item as XZone).Name.Trim();
						break;
				}
				itemUid = (item as XZone).UID;
			}

			if (item is XDirection)
			{
				switch (findObjectCondition.DirectionPropertyType)
				{
					case DirectionPropertyType.No:
						intPropertyValue = (item as XDirection).No;
						break;
					case DirectionPropertyType.Delay:
						intPropertyValue = (int)(item as XDirection).Delay;
						break;
					case DirectionPropertyType.Hold:
						intPropertyValue = (int)(item as XDirection).Hold;
						break;
					case DirectionPropertyType.DelayRegime:
						intPropertyValue = (int)(item as XDirection).DelayRegime;
						break;
					case DirectionPropertyType.Name:
						stringPropertyValue = (item as XDirection).Name.Trim();
						break;
				}
				itemUid = (item as XDirection).UID;
			}
		}

		static void InitializeItems(ref IEnumerable<object> items, ref Variable result)
		{
			result.ObjectsUids = new List<Guid>();
			if (result.ObjectType == ObjectType.Device)
			{
				items = new List<XDevice>(XManager.DeviceConfiguration.Devices);
				result.ObjectsUids = new List<Guid>(XManager.DeviceConfiguration.Devices.Select(x => x.UID));
			}
			if (result.ObjectType == ObjectType.Zone)
			{
				items = new List<XZone>(XManager.Zones);
				result.ObjectsUids = new List<Guid>(XManager.Zones.Select(x => x.UID));
			}
			if (result.ObjectType == ObjectType.Direction)
			{
				items = new List<XDirection>(XManager.Directions);
				result.ObjectsUids = new List<Guid>(XManager.Directions.Select(x => x.UID));
			}
		}

		static void FindObjectsOr(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
		{
			IEnumerable<object> items = new List<object>();
			InitializeItems(ref items, ref result);
			var resultObjects = new List<object>();
			int intPropertyValue = 0;
			string stringPropertyValue = "";
			var itemUid = new Guid();

			foreach (var findObjectCondition in findObjectConditions)
			{
				foreach (var item in items)
				{
					InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, findObjectCondition, item);
					if (resultObjects.Contains(item))
						continue;
					if (((findObjectCondition.PropertyType == PropertyType.Integer) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue < findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue >= findObjectCondition.IntValue)))) ||
						((findObjectCondition.PropertyType == PropertyType.String) &&
						(((findObjectCondition.StringConditionType == StringConditionType.StartsWith) && (stringPropertyValue.StartsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.EndsWith) && (stringPropertyValue.EndsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.Contains) && (stringPropertyValue.Contains(findObjectCondition.StringValue))))))
					{ resultObjects.Add(item); result.ObjectsUids.Add(itemUid); }
				}
			}
		}

		static void FindObjectsAnd(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
		{
			IEnumerable<object> items = new List<object>();
			InitializeItems(ref items, ref result);
			var resultObjects = new List<object>(items);
			var tempObjects = new List<object>(resultObjects);
			int intPropertyValue = 0;
			string stringPropertyValue = "";
			var itemUid = new Guid();

			foreach (var findObjectCondition in findObjectConditions)
			{
				foreach (var item in resultObjects)
				{
					InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, findObjectCondition, item);
					if (((findObjectCondition.PropertyType == PropertyType.Integer) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue >= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue < findObjectCondition.IntValue)))) ||
						((findObjectCondition.PropertyType == PropertyType.String) &&
						(((findObjectCondition.StringConditionType == StringConditionType.StartsWith) && (!stringPropertyValue.StartsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.EndsWith) && (!stringPropertyValue.EndsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.Contains) && (!stringPropertyValue.Contains(findObjectCondition.StringValue))))))
					{ tempObjects.Remove(item); result.ObjectsUids.Remove(itemUid); }
				}
				resultObjects = new List<object>(tempObjects);
			}
		}

		public static void ControlGKDevice(ProcedureStep procedureStep)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == procedureStep.ControlGKDeviceArguments.DeviceUid);
			if (device == null)
				return;
			FiresecServiceManager.SafeFiresecService.GKExecuteDeviceCommand(device.BaseUID, procedureStep.ControlGKDeviceArguments.Command);
		}

		public static List<WinFormsPlayer> WinFormsPlayers { get; private set; }
		public static void ControlCamera(ProcedureStep procedureStep)
		{
			var camera = ConfigurationCashHelper.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == procedureStep.ControlCameraArguments.CameraUid);
			if(camera == null)
				return;
			if (procedureStep.ControlCameraArguments.CameraCommandType == CameraCommandType.StartRecord)
			{
				if (WinFormsPlayers.Any(x => x.Camera == camera))
					return;
				var winFormsPlayer = new WinFormsPlayer();
				winFormsPlayer.Camera = camera;
				winFormsPlayer.Connect(camera);
				if(winFormsPlayer.StartRecord(camera, camera.ChannelNumber))
					WinFormsPlayers.Add(winFormsPlayer);
			}
			else
			{
				var winFormsPlayer = WinFormsPlayers.FirstOrDefault(x => x.Camera == camera);
				if (winFormsPlayer != null)
				{
					winFormsPlayer.StopRecord();
					WinFormsPlayers.Remove(winFormsPlayer);
				}
			}
		}
	}
}