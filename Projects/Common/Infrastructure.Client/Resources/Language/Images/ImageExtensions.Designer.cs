﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infrastructure.Client.Resources.Language.Images {
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
    internal class ImageExtensions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ImageExtensions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Infrastructure.Client.Resources.Language.Images.ImageExtensions", typeof(ImageExtensions).Assembly);
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
        ///   Looks up a localized string similar to Все файлы изображений|*.bmp; *.png; *.jpeg; *.jpg; *.svg; *.svgx; *.wmf; *.emf|BMP Файлы|*.bmp|PNG Файлы|*.png|JPEG Файлы|*.jpeg|JPG Файлы|*.jpg|SVG Файлы|*.svg;*.svgx|WMF Файлы|*.wmf|EMF Файлы|*.emf.
        /// </summary>
        internal static string GraphicFilter {
            get {
                return ResourceManager.GetString("GraphicFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Все файлы изображений|*.bmp; *.png; *.jpeg; *.jpg|BMP Файлы|*.bmp|PNG Файлы|*.png|JPEG Файлы|*.jpeg|JPG Файлы|*.jpg.
        /// </summary>
        internal static string RaterGraphicFilter {
            get {
                return ResourceManager.GetString("RaterGraphicFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Все файлы изображений|*.svg; *.svgx; *.wmf; *.emf|SVG Файлы|*.svg; *.svgx|WMF Файлы|*.wmf|EMF Файлы|*.emf.
        /// </summary>
        internal static string VectorGraphicFilter {
            get {
                return ResourceManager.GetString("VectorGraphicFilter", resourceCulture);
            }
        }
    }
}