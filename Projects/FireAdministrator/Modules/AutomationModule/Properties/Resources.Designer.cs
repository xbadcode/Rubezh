﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutomationModule.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AutomationModule.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Аргумент с таким именем уже существует..
        /// </summary>
        internal static string ErrorArgumentExistContent {
            get {
                return ResourceManager.GetString("ErrorArgumentExistContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Возникла ошибка при получении списка сценариев FireSec. Проверьте, запущен ли сервер интеграции ОПС, а также настройки серевого подключения..
        /// </summary>
        internal static string ErrorOPCScriptConnectionContent {
            get {
                return ResourceManager.GetString("ErrorOPCScriptConnectionContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Переменная с таким именем уже существует..
        /// </summary>
        internal static string ErrorVariableExist {
            get {
                return ResourceManager.GetString("ErrorVariableExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Нажмите для выбора сценария.
        /// </summary>
        internal static string PressToSelectOPCScenarioLabel {
            get {
                return ResourceManager.GetString("PressToSelectOPCScenarioLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to № Сценария: {0} Название: {1}.
        /// </summary>
        internal static string ScenarioInfoLabel {
            get {
                return ResourceManager.GetString("ScenarioInfoLabel", resourceCulture);
            }
        }
    }
}
