﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ResursNetwork.ComponentModel;

namespace ResursNetwork.Management
{
    /// <summary>
    /// Состояния которые может принимать управляемый объект 
    /// реализующий интерфейс IManageable
    /// </summary>
    [TypeConverter(typeof(EnumTypeConverter))]
    public enum Status: int
    {
        /// <summary>
        /// Объект запустил свою работу
        /// </summary>
        [Description("Работает")]
        Running,
        /// <summary>
        /// Объект остановил свою работу
        /// </summary>
        [Description("Остановлено")]
        Stopped,
    }
}
