﻿using System.ComponentModel;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Тип пользователя ГК
	/// </summary>
	public enum GKCardType
	{
		[Description("Сотрудник")]
		Employee = 0,

		[Description("Оператор")]
		Operator = 1,

		[Description("Администратор")]
		Administrator = 2,

		[Description("Инсталлятор")]
		Installator = 3,

		[Description("Изготовитель")]
		Manufactor = 4,
	}
}