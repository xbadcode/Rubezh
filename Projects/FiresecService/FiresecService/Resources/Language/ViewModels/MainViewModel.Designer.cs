﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FiresecService.Resources.Language.ViewModels {
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
    public class MainViewModel {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MainViewModel() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FiresecService.Resources.Language.ViewModels.MainViewModel", typeof(MainViewModel).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Лицензия загружена.
        /// </summary>
        public static string LicLoadAccept {
            get {
                return ResourceManager.GetString("LicLoadAccept", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Лицензия отсутствует.
        /// </summary>
        public static string LicLoadFailed {
            get {
                return ResourceManager.GetString("LicLoadFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Сервер приложений.
        /// </summary>
        public static string MainWindow_Title {
            get {
                return ResourceManager.GetString("MainWindow_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка чтения файла лицензии.
        /// </summary>
        public static string OnLoadLicense_ErrorHeader {
            get {
                return ResourceManager.GetString("OnLoadLicense_ErrorHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Выбранный файл не является файлом лицензии и не может быть использован для активации сервера. Выберите другой файл..
        /// </summary>
        public static string OnLoadLicense_ErrorText {
            get {
                return ResourceManager.GetString("OnLoadLicense_ErrorText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Отсутствует.
        /// </summary>
        public static string PropertyValue_IsOff {
            get {
                return ResourceManager.GetString("PropertyValue_IsOff", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Включено.
        /// </summary>
        public static string PropertyValue_IsOn {
            get {
                return ResourceManager.GetString("PropertyValue_IsOn", resourceCulture);
            }
        }
    }
}
