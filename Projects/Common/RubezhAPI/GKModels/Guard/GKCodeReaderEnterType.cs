﻿using System.ComponentModel;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Метод ввода
	/// </summary>
	public enum GKCodeReaderEnterType
	{
		[Description("<Нет>")]
		None,

		[Description("* Код #")]
		CodeOnly,

		[Description("* 1 * Код #")]
		CodeAndOne,

		[Description("* 2 * Код #")]
		CodeAndTwo
	}
}