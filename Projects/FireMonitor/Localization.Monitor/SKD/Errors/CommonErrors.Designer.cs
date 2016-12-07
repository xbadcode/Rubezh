﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Localization.SKD.Errors {
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
    public class CommonErrors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CommonErrors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Localization.SKD.Errors.CommonErrors", typeof(CommonErrors).Assembly);
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
        ///   Looks up a localized string similar to Error adding missing template.
        /// </summary>
        public static string AccessTemplate_Error {
            get {
                return ResourceManager.GetString("AccessTemplate_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid card number.
        /// </summary>
        public static string CardNumber_Error {
            get {
                return ResourceManager.GetString("CardNumber_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The card number must be given within 1 ... 2147483647.
        /// </summary>
        public static string CardNumberLimits_Error {
            get {
                return ResourceManager.GetString("CardNumberLimits_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File not found..
        /// </summary>
        public static string FileNotFound_Error {
            get {
                return ResourceManager.GetString("FileNotFound_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The original file can not be found. It is necessary to add an image file substrate..
        /// </summary>
        public static string ImageFileNotFound_Error {
            get {
                return ResourceManager.GetString("ImageFileNotFound_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The image file is not found..
        /// </summary>
        public static string ImageNotFound_Error {
            get {
                return ResourceManager.GetString("ImageNotFound_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The image size is too big..
        /// </summary>
        public static string ImageOutOfMemory_Error {
            get {
                return ResourceManager.GetString("ImageOutOfMemory_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was an error loading the image.
        /// </summary>
        public static string LoadImage_Error {
            get {
                return ResourceManager.GetString("LoadImage_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File transfer error {0}: 
        ///
        ///{1}.
        /// </summary>
        public static string Transfer_Error {
            get {
                return ResourceManager.GetString("Transfer_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to load the image to the server..
        /// </summary>
        public static string UploadImage_Error {
            get {
                return ResourceManager.GetString("UploadImage_Error", resourceCulture);
            }
        }
    }
}