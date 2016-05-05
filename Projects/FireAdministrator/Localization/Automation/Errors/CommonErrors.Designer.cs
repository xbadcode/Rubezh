﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Localization.Automation.Errors {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Localization.Automation.Errors.CommonErrors", typeof(CommonErrors).Assembly);
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
        ///   Looks up a localized string similar to Аргумент с таким именем уже существует..
        /// </summary>
        public static string ArgumentExist_Error {
            get {
                return ResourceManager.GetString("ArgumentExist_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Несвязанная процедура.
        /// </summary>
        public static string UnboundProcedure_Error {
            get {
                return ResourceManager.GetString("UnboundProcedure_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не выбран макет.
        /// </summary>
        public static string ValidatorProcedureLayout_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedureLayout_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не выбран элемент макета.
        /// </summary>
        public static string ValidatorProcedureLayoutElement_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedureLayoutElement_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Функция не может быть загружена по причине лицензионных ограничений.
        /// </summary>
        public static string ValidatorProcedureLicense_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedureLicense_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не выбран план.
        /// </summary>
        public static string ValidatorProcedurePlan_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedurePlan_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не выбран элемент плана.
        /// </summary>
        public static string ValidatorProcedurePlanElement_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedurePlanElement_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не выбрано свойство.
        /// </summary>
        public static string ValidatorProcedureProperty_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedureProperty_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Аргумент с таким именем уже существует {0}.
        /// </summary>
        public static string ValidatorProcedureSameNameArgument_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedureSameNameArgument_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Процедура с таким именем уже существует {0}.
        /// </summary>
        public static string ValidatorProcedureSameNameProcedure_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedureSameNameProcedure_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Переменная с таким именем уже существует {0}.
        /// </summary>
        public static string ValidatorProcedureSameNameVariable_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedureSameNameVariable_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Все переменные должны быть инициализированы.
        /// </summary>
        public static string ValidatorProcedureVariables_Error {
            get {
                return ResourceManager.GetString("ValidatorProcedureVariables_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Должна быть задана конкретная дата начала периода {0}.
        /// </summary>
        public static string ValidatorScheduleBeginDate_Error {
            get {
                return ResourceManager.GetString("ValidatorScheduleBeginDate_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Должен быть задан не нулевой период {0}.
        /// </summary>
        public static string ValidatorScheduleNullPeriod_Error {
            get {
                return ResourceManager.GetString("ValidatorScheduleNullPeriod_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Расписание с таким именем уже существует {0}.
        /// </summary>
        public static string ValidatorScheduleSameName_Error {
            get {
                return ResourceManager.GetString("ValidatorScheduleSameName_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Звуковой элемент с таким именем уже существует {0}.
        /// </summary>
        public static string ValidatorSound_Error {
            get {
                return ResourceManager.GetString("ValidatorSound_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Переменная с таким именем уже существует..
        /// </summary>
        public static string VariableExist_Error {
            get {
                return ResourceManager.GetString("VariableExist_Error", resourceCulture);
            }
        }
    }
}
