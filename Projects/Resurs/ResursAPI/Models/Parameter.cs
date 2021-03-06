﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResursAPI
{
	public class Parameter
	{
		public Parameter()
		{
			UID = Guid.NewGuid();
		}
		public void Initialize(DriverParameter driverParameter)
		{
			DriverParameter = driverParameter;
			DriverParameterUID = driverParameter.UID;
			switch (driverParameter.ParameterType)
			{
				case ParameterType.Enum:
					if (IntValue == null)
						IntValue = driverParameter.EnumDefaultItem;
					break;
				case ParameterType.String:
					if (StringValue == null)
						StringValue = driverParameter.StringDefaultValue;
					break;
				case ParameterType.Int:
					if (IntValue == null)
						IntValue = driverParameter.IntDefaultValue > driverParameter.IntMinValue ? driverParameter.IntDefaultValue : driverParameter.IntMinValue;
					break;
				case ParameterType.Double:
					if (DoubleValue == null)
						DoubleValue = driverParameter.DoubleDefaultValue > driverParameter.DoubleMinValue ? driverParameter.DoubleDefaultValue : driverParameter.DoubleMinValue;
					break;
				case ParameterType.Bool:
					if(BoolValue == null)
						BoolValue = driverParameter.BoolDefaultValue;
					break;
				case ParameterType.DateTime:
					if (DateTimeValue == null)
						DateTimeValue = driverParameter.DateTimeDefaultValue > driverParameter.DateTimeMinValue ? driverParameter.DateTimeDefaultValue : driverParameter.DateTimeMinValue;
					break;
				default:
					break;
			}
		}
		public string Validate()
		{
			if(DriverParameter == null)
				return "Отсутствует тип параметра";
			switch (DriverParameter.ParameterType)
			{
				case ParameterType.Enum:
					if (IntValue == null)
						return "Значение параметра не задано";
					if(!DriverParameter.ParameterEnumItems.Select(x => x.Value).Any(x => x == IntValue.Value))
						return "Недопустимое значение параметра";
					break;
				case ParameterType.String:
					if (StringValue != null && DriverParameter.RegEx != null && !Regex.IsMatch(StringValue, DriverParameter.RegEx))
						return "Недопустимое значение параметра";
					break;
				case ParameterType.Int:
					if (IntValue == null)
						return "Значение параметра не задано";
					if (DriverParameter.IntMinValue != null && IntValue < DriverParameter.IntMinValue.Value)
						return "Значение параметра не должно быть меньше чем " + DriverParameter.IntMinValue.Value;
					if (DriverParameter.IntMaxValue != null && IntValue > DriverParameter.IntMaxValue.Value)
						return "Значение параметра не должно быть больше чем " + DriverParameter.IntMaxValue.Value;
					break;
				case ParameterType.Double:
					if (DoubleValue == null)
						return "Значение параметра не задано";
					if (DriverParameter.DoubleMinValue != null && DoubleValue < DriverParameter.DoubleMinValue.Value)
						return "Значение параметра не должно быть меньше чем " + DriverParameter.DoubleMinValue.Value;
					if (DriverParameter.DoubleMaxValue != null && DoubleValue > DriverParameter.DoubleMaxValue.Value)
						return "Значение параметра не должно быть больше чем " + DriverParameter.DoubleMaxValue.Value;
					break;
				case ParameterType.Bool:
					break;
				case ParameterType.DateTime:
					if (DateTimeValue == null)
						return "Значение параметра не задано";
					if (DriverParameter.DateTimeMinValue != null && DateTimeValue < DriverParameter.DateTimeMinValue.Value)
						return "Значение параметра не должно быть меньше чем " + DriverParameter.DateTimeMinValue.Value;
					if (DriverParameter.DateTimeMaxValue != null && DateTimeValue > DriverParameter.DateTimeMaxValue.Value)
						return "Значение параметра не должно быть больше чем " + DriverParameter.DateTimeMaxValue.Value;
					break;
				default:
					return "Отсутствует тип параметра";
			}
			return null;
		}
		[Key]
		public Guid UID { get; set; }
		public Device Device { get; set; }
		[NotMapped]
		public DriverParameter DriverParameter { get; set; }
		public Guid DriverParameterUID { get; set; }
		public int? IntValue { get; set; }
		public double? DoubleValue { get; set; }
		public bool? BoolValue { get; set; }
		[MaxLength(4000)]
		public string StringValue { get; set; }
		public DateTime? DateTimeValue { get; set; }
		public string GetStringValue()
		{
			switch (DriverParameter.ParameterType)
			{
				case ParameterType.Enum:
					if(IntValue == null)
						return "";
					var enumItem = DriverParameter.ParameterEnumItems.FirstOrDefault(x => x.Value == IntValue);
					if(enumItem == null)
						return "";
					return enumItem.Name;	
				case ParameterType.String:
					return StringValue;
				case ParameterType.Int:
					if(IntValue == null)
						return "";
					return IntValue.Value.ToString();
				case ParameterType.Double:
					if (DoubleValue == null)
						return "";
					return DoubleValue.Value.ToString();
				case ParameterType.Bool:
					if (BoolValue == null)
						return "";
					return BoolValue.Value ? "Да" : "Нет";
				case ParameterType.DateTime:
					if (DateTimeValue == null)
						return "";
					return DateTimeValue.Value.ToString();
				default:
					return "";
			}
		}
		[NotMapped]
		public ValueType ValueType
		{
			get
			{
				switch (DriverParameter.ParameterType)
				{
					case ParameterType.Enum:
						if (IntValue != null)
						{
							switch (DriverParameter.IntType)
							{
								case IntType.Int:
									return IntValue.Value;
								case IntType.Int16:
									return Convert.ToInt16(IntValue.Value);
								case IntType.Int32:
									return Convert.ToInt32(IntValue.Value);
								case IntType.UInt32:
									return Convert.ToUInt32(IntValue.Value);
								case IntType.UInt16:
									return Convert.ToUInt16(IntValue.Value);
								default:
									break;
							}

						}
						break;
					case ParameterType.String:
						return new ParameterStringContainer 
						{ 
							Value = StringValue, 
							RegEx = DriverParameter.RegEx 
						};
					case ParameterType.Int:
						if (IntValue != null)
						{
							switch (DriverParameter.IntType)
							{
								case IntType.Int:
									return IntValue.Value;
								case IntType.Int16:
									return Convert.ToInt16(IntValue.Value);
								case IntType.Int32:
									return Convert.ToInt32(IntValue.Value);
								case IntType.UInt32:
									return Convert.ToUInt32(IntValue.Value);
								case IntType.UInt16:
									return Convert.ToUInt16(IntValue.Value);
								default:
									break;
							}
						}
						break;
					case ParameterType.Double:
						if (DoubleValue != null)
							return DoubleValue.Value;
						break;
					case ParameterType.Bool:
						return BoolValue;
					case ParameterType.DateTime:
						if (DateTimeValue != null)
							return DateTimeValue.Value;
						break;
				}
				throw new ArgumentNullException("Не найдено значение параметра");
			}
		}
		public void SetValue(ValueType valueType)
		{
			switch (DriverParameter.ParameterType)
			{
				case ParameterType.Enum:
					IntValue = Convert.ToInt32(valueType);
					break;
				case ParameterType.String:
					var parameterStringContainer = (ParameterStringContainer)valueType;
					StringValue = parameterStringContainer.Value;
					break;
				case ParameterType.Int:
					IntValue = Convert.ToInt32(valueType);
					break;
				case ParameterType.Double:
					DoubleValue = Convert.ToDouble(valueType);
					break;
				case ParameterType.Bool:
					BoolValue = (bool)valueType;
					break;
				case ParameterType.DateTime:
					DateTimeValue = (DateTime)valueType;
					break;
			}
		}

		public static bool IsEqual(Parameter x, Parameter y)
		{
			if (x.DriverParameter.ParameterType != y.DriverParameter.ParameterType)
				return false;
			switch (x.DriverParameter.ParameterType)
			{
				case ParameterType.Enum:
				case ParameterType.Int:
					return x.IntValue == y.IntValue;
				case ParameterType.String:
					return x.StringValue == y.StringValue;
				case ParameterType.Double:
					return x.DoubleValue == y.DoubleValue;
				case ParameterType.Bool:
					return x.BoolValue == y.BoolValue;
				case ParameterType.DateTime:
					return  x.DateTimeValue == y.DateTimeValue;
				default:
					return false;
			}
		}
		 
	}
}
