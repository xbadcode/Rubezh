﻿//------------------------------------------------------------------------------
// <auto-generated>
//	 Этот код создан программой.
//	 Исполняемая версия:4.0.30319.18449
//
//	 Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//	 повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GKImitator.Properties {
	
	
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
		
		private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
		
		public static Settings Default {
			get {
				return defaultInstance;
			}
		}
		
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
		[global::System.Configuration.DefaultSettingValueAttribute("Data Source=.\\SQLEXPRESS;Initial Catalog=SKUD;Integrated Security=True")]
		public string SKUDConnectionString {
			get {
				return ((string)(this["SKUDConnectionString"]));
			}
		}
		
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
		[global::System.Configuration.DefaultSettingValueAttribute("Data Source=.\\sqlexpress;Initial Catalog=SKDImirator;Integrated Security=True")]
		public string SKDImiratorConnectionString {
			get {
				return ((string)(this["SKDImiratorConnectionString"]));
			}
		}
	}
}
