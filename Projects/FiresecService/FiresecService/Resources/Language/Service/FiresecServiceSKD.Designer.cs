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
    internal class FiresecServiceSKD {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal FiresecServiceSKD() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FiresecService.Resources.Language.Service.FiresecServiceSKD", typeof(FiresecServiceSKD).Assembly);
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
        ///   Looks up a localized string similar to Точка доступа не найдена в конфигурации.
        /// </summary>
        internal static string AccessPointNotFoundInConfiguration {
            get {
                return ResourceManager.GetString("AccessPointNotFoundInConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to У точки доступа не указано устройство входа.
        /// </summary>
        internal static string AccessPointWithoutEntrance {
            get {
                return ResourceManager.GetString("AccessPointWithoutEntrance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для точки доступа не найден замок.
        /// </summary>
        internal static string AccessPointWithoutLock {
            get {
                return ResourceManager.GetString("AccessPointWithoutLock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Перевод в режим замка &quot;{0}&quot; не подтвержден контроллером.
        /// </summary>
        internal static string ChangeLockModeNotConfirmed {
            get {
                return ResourceManager.GetString("ChangeLockModeNotConfirmed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В данный момент контроллер выполняет другую операцию. Текущая операция не может быть выполнена. Повторите попытку позднее..
        /// </summary>
        internal static string ControllerIsBusy {
            get {
                return ResourceManager.GetString("ControllerIsBusy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Нельзя перевести замок &quot;{0}&quot; в данный режим.
        /// </summary>
        internal static string CouldNotChangeLockMode {
            get {
                return ResourceManager.GetString("CouldNotChangeLockMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка БД: .
        /// </summary>
        internal static string DBError {
            get {
                return ResourceManager.GetString("DBError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Сотрудник удален.
        /// </summary>
        internal static string DeleteEmployee {
            get {
                return ResourceManager.GetString("DeleteEmployee", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка удаления файла.
        /// </summary>
        internal static string DeleteFile {
            get {
                return ResourceManager.GetString("DeleteFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка удаления файла &apos;{0}&apos;.
        /// </summary>
        internal static string DeleteFileError {
            get {
                return ResourceManager.GetString("DeleteFileError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В процессе удаления файла возника ошибка:
        ///{0}.
        /// </summary>
        internal static string DeleteFileException {
            get {
                return ResourceManager.GetString("DeleteFileException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Устройство не найдено в конфигурации.
        /// </summary>
        internal static string DeviceNotFoundInConfiguration {
            get {
                return ResourceManager.GetString("DeviceNotFoundInConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка при получении карт или шаблонов карт.
        /// </summary>
        internal static string GetCardsError {
            get {
                return ResourceManager.GetString("GetCardsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Количество активных пропусков в соответствии с лицензией не может превышать {0}. Для создания нового пропуска приобретите дополнительные лицензии..
        /// </summary>
        internal static string LicenseAddCardMessage {
            get {
                return ResourceManager.GetString("LicenseAddCardMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не найдена предыдущая карта.
        /// </summary>
        internal static string LostLastCard {
            get {
                return ResourceManager.GetString("LostLastCard", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Нет&gt;.
        /// </summary>
        internal static string NoUserName {
            get {
                return ResourceManager.GetString("NoUserName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка получения файла.
        /// </summary>
        internal static string RecieveFile {
            get {
                return ResourceManager.GetString("RecieveFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка передачи файла &apos;{0}&apos;.
        /// </summary>
        internal static string SendFileError {
            get {
                return ResourceManager.GetString("SendFileError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В процессе передачи файла &apos;{0}&apos; произошла ошибка:
        ///{1}.
        /// </summary>
        internal static string SendFileException {
            get {
                return ResourceManager.GetString("SendFileException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Функция обновления ПО не доступна.
        /// </summary>
        internal static string UpdateSoftwareNotAvailable {
            get {
                return ResourceManager.GetString("UpdateSoftwareNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Запись пропусков на все контроллеры.
        /// </summary>
        internal static string WriteCardsOnAllController {
            get {
                return ResourceManager.GetString("WriteCardsOnAllController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Запись пропусков на все контроллеры отменена.
        /// </summary>
        internal static string WriteCardsOnAllController_Cancel {
            get {
                return ResourceManager.GetString("WriteCardsOnAllController_Cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Запись пропусков на контроллер &quot;{0}&quot;.
        /// </summary>
        internal static string WriteCardsOnController {
            get {
                return ResourceManager.GetString("WriteCardsOnController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Запись паролей замков на все контроллеры.
        /// </summary>
        internal static string WriteLockPasswordOnAllController {
            get {
                return ResourceManager.GetString("WriteLockPasswordOnAllController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Запись паролей замков на все контроллеры отменена.
        /// </summary>
        internal static string WriteLockPasswordOnAllController_Cancel {
            get {
                return ResourceManager.GetString("WriteLockPasswordOnAllController_Cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Запись паролей замков на контроллер &quot;{0}&quot;.
        /// </summary>
        internal static string WriteLockPasswordOnController {
            get {
                return ResourceManager.GetString("WriteLockPasswordOnController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Запись графиков доступа на все контроллеры.
        /// </summary>
        internal static string WriteScheduleOnAllController {
            get {
                return ResourceManager.GetString("WriteScheduleOnAllController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Запись графиков доступа на все контроллеры отменена.
        /// </summary>
        internal static string WriteScheduleOnAllController_Cancel {
            get {
                return ResourceManager.GetString("WriteScheduleOnAllController_Cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Запись графиков доступа на контроллер &quot;{0}&quot;.
        /// </summary>
        internal static string WriteScheduleOnController {
            get {
                return ResourceManager.GetString("WriteScheduleOnController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Зона не найдена в конфигурации.
        /// </summary>
        internal static string ZoneNotFoundInConfiguration {
            get {
                return ResourceManager.GetString("ZoneNotFoundInConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для зоны не найден замок.
        /// </summary>
        internal static string ZoneWithoutLock {
            get {
                return ResourceManager.GetString("ZoneWithoutLock", resourceCulture);
            }
        }
    }
}