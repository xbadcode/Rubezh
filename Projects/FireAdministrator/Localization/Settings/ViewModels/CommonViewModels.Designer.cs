﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Localization.Settings.ViewModels {
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
    public class CommonViewModels {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CommonViewModels() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Localization.Settings.ViewModels.CommonViewModels", typeof(CommonViewModels).Assembly);
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
        ///   Looks up a localized string similar to Connection to server {0} {1}.
        /// </summary>
        public static string AppServer_Connection {
            get {
                return ResourceManager.GetString("AppServer_Connection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to could not established due to error: 
        ///
        ///{0}.
        /// </summary>
        public static string AppServer_ConnectionError {
            get {
                return ResourceManager.GetString("AppServer_ConnectionError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to has been successfully installed.
        /// </summary>
        public static string AppServer_ConnectionSuccess {
            get {
                return ResourceManager.GetString("AppServer_ConnectionSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Сервер приложений.
        /// </summary>
        public static string GlobalSettings_ApplicationServer {
            get {
                return ResourceManager.GetString("GlobalSettings_ApplicationServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameters.
        /// </summary>
        public static string GlobalSettings_Parameters {
            get {
                return ResourceManager.GetString("GlobalSettings_Parameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to reset the configuration?.
        /// </summary>
        public static string GlobalSettings_ResetConfig {
            get {
                return ResourceManager.GetString("GlobalSettings_ResetConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to reset the database?.
        /// </summary>
        public static string GlobalSettings_ResetDB {
            get {
                return ResourceManager.GetString("GlobalSettings_ResetDB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to reset the default settings?.
        /// </summary>
        public static string GlobalSettings_ResetSettings {
            get {
                return ResourceManager.GetString("GlobalSettings_ResetSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to reset the default device settings SKD library?.
        /// </summary>
        public static string GlobalSettings_ResetSKD {
            get {
                return ResourceManager.GetString("GlobalSettings_ResetSKD", resourceCulture);
            }
        }
    }
}
