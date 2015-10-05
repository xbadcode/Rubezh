﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.ApplicationLayer;

namespace ResursNetwork.Devices
{
    /// <summary>
    /// Реализует базовые компонеты счётчика электроэнергии
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Возвращает тип устройства (счётчика электро)
        /// </summary>
        DeviceType DeviceType { get; }
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
        UInt32 Address { get; }
        /// <summary>
        /// Сетевой контроллер, владелец данного устройства
        /// </summary>
        INetwrokController Network { get; }
        /// <summary>
        /// Возвращает коллекцию описания параметров устройства
        /// </summary>
        ParatemersCollection Parameters { get; }
        /// <summary>
        /// Возникает при ошибках в устройтве
        /// </summary>
        event EventHandler<ErrorOccuredEventArgs> ErrorOccurred;
    }
}