﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Настройки кодонаборника в охранной зоне
	/// </summary>
	[DataContract]
	public class GKCodeReaderSettings
	{
		public GKCodeReaderSettings()
		{
			SetGuardSettings = new GKCodeReaderSettingsPart();
			ResetGuardSettings = new GKCodeReaderSettingsPart();
			ChangeGuardSettings = new GKCodeReaderSettingsPart();
			AlarmSettings = new GKCodeReaderSettingsPart();

			AutomaticOnSettings = new GKCodeReaderSettingsPart();
			AutomaticOffSettings = new GKCodeReaderSettingsPart();
			StartSettings = new GKCodeReaderSettingsPart();
			StopSettings = new GKCodeReaderSettingsPart();

			SetGuardSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne;
			ResetGuardSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndTwo;
			AutomaticOnSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne;
			AutomaticOffSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndTwo;
		}

		/// <summary>
		/// Настройка на постановку на охрану
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart SetGuardSettings { get; set; }

		/// <summary>
		/// Настройка на снятие с охраны
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart ResetGuardSettings { get; set; }

		/// <summary>
		/// Настройка на изменение состояния
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart ChangeGuardSettings { get; set; }

		/// <summary>
		/// Настройка на вызов тревоги
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart AlarmSettings { get; set; }

		/// <summary>
		/// Настройка на постановку в автоматику
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart AutomaticOnSettings { get; set; }

		/// <summary>
		/// Настройка на снятие с автоматики
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart AutomaticOffSettings { get; set; }

		/// <summary>
		/// Настройка на пуск
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart StartSettings { get; set; }

		/// <summary>
		/// Настройка на останов
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart StopSettings { get; set; }
	}

	/// <summary>
	/// Настройка кодонаборника
	/// </summary>
	[DataContract]
	public class GKCodeReaderSettingsPart
	{
		public GKCodeReaderSettingsPart()
		{
			CodeUIDs = new List<Guid>();
			CodeReaderEnterType = GKCodeReaderEnterType.None;
		}

		/// <summary>
		/// Метод ввода
		/// </summary>
		[DataMember]
		public GKCodeReaderEnterType CodeReaderEnterType { get; set; }

		/// <summary>
		/// Идентификаторы кодов
		/// </summary>
		[DataMember]
		public List<Guid> CodeUIDs { get; set; }

		/// <summary>
		/// Минимальный уровень на проход
		/// </summary>
		[DataMember]
		public int AccessLevel { get; set; }
	}
}