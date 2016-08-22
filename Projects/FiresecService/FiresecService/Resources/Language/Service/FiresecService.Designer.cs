﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FiresecService.Resources.Language.Service {
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
    internal class FiresecService {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal FiresecService() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FiresecService.Resources.Language.Service.FiresecService", typeof(FiresecService).Assembly);
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
        ///   Looks up a localized string similar to Сервер не активирован. Подключение к серверу возможно только после его активации.
        /// </summary>
        internal static string CanConnect {
            get {
                return ResourceManager.GetString("CanConnect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Количество активных пропусков в базе данных системы превышает лицензированное значение. Загрузка приложения невозможна.
        /// </summary>
        internal static string CheckActiveCardsCountAgainstLicenseData {
            get {
                return ResourceManager.GetString("CheckActiveCardsCountAgainstLicenseData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Другой администратор осуществил вход с компьютера &apos;{0}&apos;. Одновременная работа двух администраторов в системе не допускается. Для входа в систему завершите работу на другом компьютере.
        /// </summary>
        internal static string CheckAdministratorConnectionRightsUsingLicenseData {
            get {
                return ResourceManager.GetString("CheckAdministratorConnectionRightsUsingLicenseData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Конфигурация не соответствует ограничениям лицензии. Для продолжения работы загрузите другую лицензию или измените конфигурацию.
        /// </summary>
        internal static string CheckConfigurationValidation {
            get {
                return ResourceManager.GetString("CheckConfigurationValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Удаленные подключения к серверу не разрешены лицензией.
        /// </summary>
        internal static string CheckConnectionRightsUsingLicenseData {
            get {
                return ResourceManager.GetString("CheckConnectionRightsUsingLicenseData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Достигнуто максимальное количество подключений к серверу, допускаемое лицензией.
        /// </summary>
        internal static string CheckMonitorConnectionRightsUsingLicenseData {
            get {
                return ResourceManager.GetString("CheckMonitorConnectionRightsUsingLicenseData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Другой экземпляр ОЗ осуществил вход с компьютера &apos;{0}&apos;. Одновременная работа на одном ПК двух и более экземпляров ОЗ не допускается..
        /// </summary>
        internal static string CheckMonitorConnectionRightsUsingLicenseData_OneClient {
            get {
                return ResourceManager.GetString("CheckMonitorConnectionRightsUsingLicenseData_OneClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не найден пользователь.
        /// </summary>
        internal static string UserNotFound {
            get {
                return ResourceManager.GetString("UserNotFound", resourceCulture);
            }
        }
    }
}
