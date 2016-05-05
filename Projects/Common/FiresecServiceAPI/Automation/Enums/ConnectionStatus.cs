﻿using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.Automation.Enums
{
	/// <summary>
	/// Описывает статус соединения в автоматизации
	/// </summary>
	public enum ConnectionStatus
	{
		/// <summary>
		/// Норма
		/// </summary>
		//[Description("Норма")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.ConnectionStatus), "Connected")]
		Connected,
		
		/// <summary>
		/// Потеря связи
		/// </summary>
        //[Description("Потеря связи")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.ConnectionStatus), "Disconnected")]
		Disconnected
	}
}