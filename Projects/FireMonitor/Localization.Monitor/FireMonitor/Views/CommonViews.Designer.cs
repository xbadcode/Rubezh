﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Localization.FireMonitor.Views {
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
    public class CommonViews {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CommonViews() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Localization.FireMonitor.Views.CommonViews", typeof(CommonViews).Assembly);
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
        ///   Looks up a localized string similar to Server configuration changed..
        /// </summary>
        public static string ConfigChanged {
            get {
                return ResourceManager.GetString("ConfigChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to On/Off sound.
        /// </summary>
        public static string OnOffSound {
            get {
                return ResourceManager.GetString("OnOffSound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  - Press &quot;Postepone restart&quot; to save changes. After 50 seconds application will restart automaticaly!.
        /// </summary>
        public static string PressPostponeRestart {
            get {
                return ResourceManager.GetString("PressPostponeRestart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  - Press &quot;Restart&quot; to restart application immediately.
        /// </summary>
        public static string PressRestart {
            get {
                return ResourceManager.GetString("PressRestart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stop playing sound.
        /// </summary>
        public static string StopSound {
            get {
                return ResourceManager.GetString("StopSound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User:.
        /// </summary>
        public static string User {
            get {
                return ResourceManager.GetString("User", resourceCulture);
            }
        }
    }
}