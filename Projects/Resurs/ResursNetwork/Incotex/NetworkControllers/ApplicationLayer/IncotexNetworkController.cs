﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Timers;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transactions;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.Management;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.NetworkControllers.Messages;
using ResursNetwork.Incotex.NetworkControllers.DataLinkLayer;
using ResursAPI;
using ResursAPI.Models;
using ResursAPI.ParameterNames;
using Common;

namespace ResursNetwork.Incotex.NetworkControllers.ApplicationLayer
{
    /// <summary>
    /// Сетевой контроллер для работы со устройствами производства Incotex
    /// </summary>
    public class IncotexNetworkController: NetworkControllerBase
    {
        /// <summary>
        /// Класс буфера исходящих запросов к удалённым устройтсвам
        /// </summary>
        internal class OutputBuffer
        {
            #region Fields And Properties

            /// <summary>
            /// Буфер исходящих запросов, для внешних вызовов
            /// (со стороны UI интерфейса и т.п.) Данные запросы
            /// имеют приоритет над внутренними вызовами 
            /// </summary>
            private Queue<NetworkRequest> _OutputBufferExternalCalls = 
                new Queue<NetworkRequest>();

            /// <summary>
            /// Буфер исходящих запросов, для внутренних вызовов
            /// (переодический автоматизированный опрос удалённых устройств)
            /// </summary>
            private Queue<NetworkRequest> _OutputBufferInternalCalls = 
                new Queue<NetworkRequest>();
            
            /// <summary>
            /// Возвращает состояние исходящего буфера (наличие в нём запросов)
            /// </summary>
            internal bool IsEmpty
            {
                get { return ((_OutputBufferExternalCalls.Count == 0) && 
                    (_OutputBufferInternalCalls.Count == 0)) ? true : false; }
            }

            internal int Count
            {
                get { return _OutputBufferExternalCalls.Count + _OutputBufferInternalCalls.Count; }
            }

            #endregion

            /// <summary>
            /// Записывает запрос в выходной буфер 
            /// </summary>
            /// <param name="request">Сетевой запрос</param>
            /// <param name="isExternalCall">Признак внешнего вызова</param>
            internal void Enqueue(NetworkRequest request, bool isExternalCall) 
            { 
                if (isExternalCall)
                {
                    _OutputBufferExternalCalls.Enqueue(request);
                }
                else
                {
                    _OutputBufferInternalCalls.Enqueue(request);
                }
            }

            /// <summary>
            /// Читает запрос из выходного буфера
            /// </summary>
            /// <returns>null- если буфер пуст</returns>
            internal NetworkRequest Dequeue()
            {
                // Вынешние вызовы имеют приоритет перед внутренними. Поэтому,
                // выбираем сообщение сначала из буфера внешних вызовов и только,
                // затем, если этот буфер пуст, выбирает из буфера внутренних вызовов 
                if (_OutputBufferExternalCalls.Count != 0)
                {
                    return _OutputBufferExternalCalls.Dequeue();
                }
                
                if ( _OutputBufferInternalCalls.Count != 0)
                {
                    return _OutputBufferInternalCalls.Dequeue();
                }

                return null;
            }
        }

        #region Fields And Properties

		/// <summary>
		/// Минимальное значение, которое можно установить для ствойства PollingPeriod
		/// </summary>
		const int MIN_POLLING_PERIOD = 1000;
		
		static DeviceModel[] _supportedDevices = 
			new DeviceModel[] { DeviceModel.Mercury203 };
		static Type[] _supportedInterfaces = 
			new Type[] { typeof(Incotex.NetworkControllers.DataLinkLayer.ComPort) };
		static object _syncRoot = new object(); 
		NetworkRequest _currentNetworkRequest;
		int _requestTimeout = 2000; // Значение по умолчанию
		int _broadcastRequestDelay = 2000; // Значение по умолчанию
		AutoResetEvent _autoResetEventRequest = new AutoResetEvent(false);
		AutoResetEvent _autoResetEventWorker = new AutoResetEvent(false);
		OutputBuffer _outputBuffer = new OutputBuffer();

        /// <summary>
        /// Хранит входящее сообщение от удалённого устройтсва во 
        /// время действия сетевой транзакции
        /// </summary>
        private DataMessage _CurrentIncomingMessage;

        /// <summary>
        /// Возвращает состояние текущей сетевой транзакции
        /// </summary>
        public NetworkRequest CurrentNetworkRequest
        {
            get { return _currentNetworkRequest; }
        }

        public override IEnumerable<DeviceModel> SuppotedDevices
        {
            get { return _supportedDevices; }
        }

        public override IDataLinkPort Connection
        {
            get
            {
                return base.Connection;
            }
            set
            {
                if (value != null)
                {
                    // Закоментировал из unit-тестов
                    //if (!_SupportedInterfaces.Contains(value.GetType()))
                    //{
                    //    throw new NotSupportedException("Данный интерфейс не поддреживается контроллером");
                    //}
                }
                base.Connection = value;
            }
        }

        /// <summary>
        /// Время (мсек), в течении которого удалённое 
        /// должно ответить на запрос
        /// </summary>
        public int RequestTimeout
        {
            get { return _requestTimeout; }
            set 
            {
                if (value > 0)
                {
                    _requestTimeout = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Выдержка времени (мсек) после выполнения широковешательного запроса
        /// </summary>
        public int BroadcastRequestDelay
        {
            get { return _broadcastRequestDelay; }
            set 
            {
                if (value > 0)
                {
                    _broadcastRequestDelay = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("BroadcastRequestDelay", 
                        "Недопустимое значение параметра");
                }
            }
        }

        /// <summary>
        /// Период (мсек) получения данных от удалённых устройтв
        /// </summary>
		public override int PollingPeriod
		{
			get
			{
				return base.PollingPeriod;
			}
			set
			{
				if (value < MIN_POLLING_PERIOD)
				{
					throw new ArgumentOutOfRangeException("PollingPeriod",
						String.Format("Значение не должно быть меньше {0}", MIN_POLLING_PERIOD));
				}
				base.PollingPeriod = value;
			}
		}

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public IncotexNetworkController()
        {
			// По умолчнию период синхронизации 1 день;
			_pollingPeriod = Convert.ToInt32(TimeSpan.FromDays(1).TotalMilliseconds); 
        }

        #endregion

        #region Methods

        /// <summary>
        /// Принимает сетевое устройтсво ищет в нём методы с атрибутом 
        /// PeriodicPollingEnabledAttribute и выполняет их.
        /// </summary>
        /// <param name="device"></param>
        private void ReadDeviceParameters(DeviceBase device)
        {
            foreach (var methodInfo in device.GetType().GetMethods())
            {
                // Проверяем атрибут PeriodicPollingEnabledAttribute у метода
                // если присутствует, проверяем аргументы метода и тип возвращаемого значения
                if ((methodInfo.GetCustomAttributes(typeof(PeriodicReadEnabledAttribute), false).Length > 0) ||
                    (methodInfo.GetParameters().Length == 0) ||
                    (methodInfo.ReturnType == typeof(Transaction)))
                {
                    // Записываем транзакцию в выходной буфер
                    methodInfo.Invoke(device, new object[] { false });
                }
            }
        }

        /// <summary>
        /// Обработчик приёма сообщения из сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void EventHandler_Connection_MessageReceived(
            object sender, EventArgs e)
        {
            IDataLinkPort port = (IDataLinkPort)sender;
            var messages = new List<MessageBase>();
            
            // Читаем входящие сообщения из входного буфера порта 
            while(port.MessagesToRead > 0)
            {
                var msg = port.Read();
                messages.Add((MessageBase)msg);
            }

            // Обрабатываем сервисные сообщения
            var serviceErrorMessages = messages
                .Where(y => y.MessageType == MessageType.ServiceErrorMessage)
                .Select(z => (ServiceErrorMessage)z);

            foreach (var msg in serviceErrorMessages)
            {
                //запись в лог ошибок
                Logger.Error(String.Format("Controller Id={0} | Ошибка Code={1} | Description={2}",
                    _Id, msg.Code, msg.Description));

                //TODO: Сделать обработчик ошибок, если потребуется
                //switch (msg.SpecificErrorCode)
                //{
                //    case ErrorCode.
                //}
            }

            // TODO: сделать сервистные сообщения, если понадобятся 
            //var serviceInfoMessages = messages
            //    .Where(y => y.MessageType == MessageType.ServiceInfoMessage)
            //    .Select(z => (....));

            var dataMessages = messages
                .Where(y => y.MessageType == MessageType.IncomingMessage)
                .Select(z => (DataMessage)z).ToArray();

            if (dataMessages.Length > 1)
            {
                throw new Exception(
                    "Сетевой контроллер принял одновременно более одного сообщения из сети");
            }

            if ((_currentNetworkRequest == null) || 
                (_currentNetworkRequest.Status != NetworkRequestStatus.Running))
            {
                throw new Exception("Принято сообщение в отсутствии запроса");
            }

            // Обрабатывает сообщение
            _CurrentIncomingMessage = dataMessages[0];
			_autoResetEventRequest.Set();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        protected override void NetworkPollingAction(object cancellationToken)
        {
            //DateTime lastUpdate;
            List<IDevice> faultyDevices = new List<IDevice>();
            NetworkRequest networkRequest;

            var cancel = (CancellationToken)cancellationToken;
            cancel.ThrowIfCancellationRequested();

            while(!cancel.IsCancellationRequested)
            {
                if (!_autoResetEventWorker.WaitOne(Convert.ToInt32(PollingPeriod)))
                {
                    // При срабатывании по таймауту обновляем данные из удалённых устройтств
                    // При условии что контроллер в активном состоянии
                    foreach (DeviceBase device in _devices)
                    {
                        ReadDeviceParameters(device);
                    }
                }

                // Выполняем все запросы в буфере
                while (_outputBuffer.Count > 0)
                {
                    lock (_syncRoot)
                    {
                        networkRequest = _outputBuffer.Dequeue();

                        if (Status != Status.Running)
                        {
                            networkRequest.CurrentTransaction.Start();
                            networkRequest.CurrentTransaction.Abort(
                                new TransactionError
                                {
                                    ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
                                    Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
                                });
							networkRequest.AsyncRequestResult.SetCompleted();
                            continue;
                        }
                    }
                    
                    if (cancel.IsCancellationRequested)
                    {
                        networkRequest.CurrentTransaction.Start();
                        networkRequest.CurrentTransaction.Abort(new TransactionError
                        {
                            ErrorCode = TransactionErrorCodes.RequestWasCancelled,
                            Description = "Выполнение запроса прервано по требованию"
                        });
						networkRequest.AsyncRequestResult.SetCompleted();
                        continue;
                    }

                    // Проверяем устройство. Если оно в списке
                    if (faultyDevices.Contains(networkRequest.CurrentTransaction.Sender))
                    {
                        // Если устройство содежится в списке неисправных, то пропускаем его
                        networkRequest.CurrentTransaction.Start();
                        networkRequest.CurrentTransaction.Abort(new TransactionError
                        {
                            ErrorCode = TransactionErrorCodes.RequestTimeout,
                            Description =
                            "Исключено из обработки по причине неудачного предыдущего запроса к этому устройству"
                        });
						networkRequest.AsyncRequestResult.SetCompleted();
                        continue;
                    }

                    // Выполняем сетевой запрос
                    ExecuteTransaction(networkRequest);

                    var result = networkRequest.AsyncRequestResult;

                    if (!result.IsCompleted)
                    {
                        throw new Exception("Сетевой запрос не выполнен. Это невозможно и никогда не должно появиться!!!");
                    }

                    if (result.HasError)
                    {
                        if (result.LastTransaction.Error.ErrorCode == TransactionErrorCodes.RequestTimeout)
                        {
                            // Запоминаем данное устройство, для того, что бы игнорировать 
                            // все последующие запросы от данного устройства
                            if (!faultyDevices.Contains(result.Sender))
                            {
                                faultyDevices.Add(result.Sender);
                                //Установить ошибку в данном устройстве
                                result.Sender.CommunicationError = true;
                            }
                        }
                    }
                    else
                    {
                        // Проверяем: если запрос выполен успешно, но устройтсво содежит ошибку ComunicationError,
                        // то считаем, что связь с устройcтвом восстановилась и убираем данную ошибку
                        if (result.Sender.Errors.CommunicationError)
                        {
                            // удаляем данное устройтсво из списка неисправных
                            if (faultyDevices.Contains(result.Sender))
                            {
                                faultyDevices.Remove(result.Sender);
                            }
                            // сбросить ошибку в данном устройстве
                            result.Sender.CommunicationError = false;
                        }
                    }
                }

            }
        }

        public override IAsyncRequestResult Write(
            NetworkRequest networkRequest, bool isExternalCall = true)
        {
            lock (_syncRoot)
            {
                if (Status == Status.Running)
                {
                    networkRequest.TotalAttempts = TotalAttempts;
                    _outputBuffer.Enqueue(networkRequest, isExternalCall);
                    _autoResetEventWorker.Set();
                    return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Невозможно выполнить операцию. Контроллер остановлен");
                }
            }
        }

        /// <summary>
        /// Выполняет сетевую транзакцию 
        /// </summary>
        /// <param name="networkRequest"></param>
        public void ExecuteTransaction(NetworkRequest networkRequest)
        {
            if ((CurrentNetworkRequest != null) &&
                (CurrentNetworkRequest.Status == NetworkRequestStatus.Running))
            {
                throw new InvalidOperationException(
                    "Попытка выполнить транзакцию во время действия другой");
            }

            if ((networkRequest.CurrentTransaction.TransactionType != TransactionType.BroadcastMode) &&
                (networkRequest.CurrentTransaction.TransactionType != TransactionType.UnicastMode))
            {
                networkRequest.CurrentTransaction.Start();
                networkRequest.CurrentTransaction.Abort(new TransactionError
                {
                    ErrorCode = TransactionErrorCodes.TransactionTypeIsWrong,
                    Description = "Попытка запустить сетевую транзакцию с недопустимым типом"
                });
                throw new InvalidOperationException(
                    String.Format("Попытка запустить сетевую транзакцию с недопустимым типом: {0}",
                    networkRequest.CurrentTransaction.TransactionType));
            }

            // Устанавливаем транзакцию в качестве текущей
            _currentNetworkRequest = networkRequest;
            var result = _currentNetworkRequest.AsyncRequestResult;

            // Если запрос адресованный, то ждём ответа
            // Если запрос широковещательный выдерживаем установленную паузу
            switch (_currentNetworkRequest.CurrentTransaction.TransactionType)
            {
                case TransactionType.UnicastMode:
                    {
                        var disconnected = false;

                        for (int i = 0; i < TotalAttempts; i++)
                        {
                            // Отправляем запрос к удалённому устройтву
                            _currentNetworkRequest.CurrentTransaction.Start();
                            _Connection.Write(_currentNetworkRequest.CurrentTransaction.Request);

                            // Ждём ответа от удалённого устройтва или тайм аут
                            if (!_autoResetEventRequest.WaitOne(_requestTimeout))
                            {
                                // TimeOut!!! Прекращает текущую транзакцию
                                _currentNetworkRequest.CurrentTransaction.Abort(new TransactionError
                                {
                                    ErrorCode = TransactionErrorCodes.RequestTimeout,
                                    Description = "Request timeout"
                                });
                                
                                Transaction trn;
                                // Повторяем запрос
                                _currentNetworkRequest.NextAttempt(out trn);
                                disconnected = true;
                                continue;                               
                            }
                            else
                            {
                                // Ответ получен
                                _currentNetworkRequest.CurrentTransaction.Stop(_CurrentIncomingMessage);
                                _CurrentIncomingMessage = null;
                                disconnected = false;
                                break;
                            }
                        }
                        
                        // Кол-во попыток доступа к устройтсву исчерпано
                        if (disconnected)
                        {

                            //var errors = ((DeviceBase)_CurrentNetworkRequest.CurrentTransaction.Sender).Errors;
                            //errors.CommunicationError = true;
                            //((DeviceBase)_CurrentNetworkRequest.CurrentTransaction.Sender).SetError(errors);
                        }
                        else
                        {
                            // Если ранее была установлена ошибка связи, то сбрасываем её
                            //var errors = ((DeviceBase)_CurrentNetworkRequest.CurrentTransaction.Sender).Errors;
                            //if (errors.CommunicationError == true)
                            //{
                            //    errors.CommunicationError = false;
                            //    ((DeviceBase)_CurrentNetworkRequest.CurrentTransaction.Sender).SetError(errors);
                            //}
                        }

						result.SetCompleted();

                        OnNetwrokRequestCompleted(
                            new NetworkRequestCompletedArgs { NetworkRequest = _currentNetworkRequest });

                        break;
                    }
                case TransactionType.BroadcastMode:
                    {
                        // Отправляем запрос к удалённому устройтву
                        _currentNetworkRequest.CurrentTransaction.Start();
                        _Connection.Write(_currentNetworkRequest.CurrentTransaction.Request);

                        if (!_autoResetEventRequest.WaitOne(_broadcastRequestDelay))
                        {
                            _currentNetworkRequest.CurrentTransaction.Stop(null);
                        }
                        else
                        {
                            _CurrentIncomingMessage = null;
                            throw new Exception(
                                "Принят ответ от удалённого устройтства во время широковещательного запроса");
                        }

						result.SetCompleted();
						
						OnNetwrokRequestCompleted(
							new NetworkRequestCompletedArgs { NetworkRequest = _currentNetworkRequest });

                        break;
                    }
                default:
                    {
                        result.SetCompleted();
                        throw new NotSupportedException();
                    }
            }
        }

		/// <summary>
		/// Синхронизирует время в группе устройтсв с указанным групповым
		/// адресом
		/// </summary>
		/// <param name="groupAddress"></param>
		public override void SyncDateTime(ValueType groupAddress)
        {
			var result = Mercury203.WriteDateTimeInGroupDevices(DateTime.Now, (uint)groupAddress, 
				(INetwrokController)this, isExternalCall: true);

			// Ждём завершения операции
			for (int i = 0; i < 2; i++)
			{
				Thread.Sleep(BroadcastRequestDelay);
				
				if (result.IsCompleted)
				{
					return;
				}
			}

			throw new Exception(
				"Широковешательный запрос не завершился за заданное время");
        }

		public override void SyncDateTime()
		{
			Boolean flag = false;
			var groups = _devices.GroupBy(x => ((Mercury203)x).GroupAddress);

			foreach (var group in groups)
			{
				var result = Mercury203.WriteDateTimeInGroupDevices(DateTime.Now, group.Key,
					(INetwrokController)this, isExternalCall: true);

				// Ждём завершения операции
				for (int i = 0; i < 2; i++)
				{
					Thread.Sleep(BroadcastRequestDelay);

					if (result.IsCompleted)
					{
						flag = true;
						break;
					}
				}

				if (!flag)
				{
					throw new Exception(
						"Широковешательный запрос не завершился за заданное время"); 
				}
			}
		}

		public override OperationResult ReadParameter(string parameterName)
		{
			switch (parameterName)
			{
				case ParameterNamesIncotexNetwork.BautRate:
					{
						return new OperationResult
						{
							Result =
								new TransactionError
								{
									ErrorCode = TransactionErrorCodes.NoError,
									Description = String.Empty
								},
							Value = ((ComPort)base._Connection).BaudRate
						};
					}
				case ParameterNamesIncotexNetworkVirtual.BroadcastDelay:
					{
						return new OperationResult
						{
							Result =
								new TransactionError
								{
									ErrorCode = TransactionErrorCodes.NoError,
									Description = String.Empty
								},
							Value = _broadcastRequestDelay
						};
					}
				case ParameterNamesIncotexNetworkVirtual.PollInterval:
					{
						return new OperationResult
						{
							Result =
								new TransactionError
								{
									ErrorCode = TransactionErrorCodes.NoError,
									Description = String.Empty
								},
							Value = _pollingPeriod
						};
					}
				case ParameterNamesIncotexNetworkVirtual.PortName:
					{
						return new OperationResult
						{
							Result =
								new TransactionError
								{
									ErrorCode = TransactionErrorCodes.NoError,
									Description = String.Empty
								},
							Value = new ParameterStringContainer 
							{
								Value = ((ComPort)base._Connection).PortName
							}
						};
					}
				case ParameterNamesIncotexNetworkVirtual.Timeout:
					{
						return new OperationResult
						{
							Result =
								new TransactionError
								{
									ErrorCode = TransactionErrorCodes.NoError,
									Description = String.Empty
								},
							Value = _requestTimeout
						};
					}
				default:
					{
						throw new InvalidOperationException(String.Format(
							"Ошибка чтения параметра. Параметр {0} не найден", parameterName));
					}
			}
		}

		public override void WriteParameter(string parameterName, ValueType value)
		{
			switch (parameterName)
			{
				case ParameterNamesIncotexNetwork.BautRate:
					{
						((ComPort)base._Connection).BaudRate = (int)value; break;
					}
				case ParameterNamesIncotexNetworkVirtual.BroadcastDelay:
					{
						BroadcastRequestDelay = (int)value; break;
					}
				case ParameterNamesIncotexNetworkVirtual.PollInterval:
					{
							PollingPeriod = (int)value; break;
					}
				case ParameterNamesIncotexNetworkVirtual.PortName:
					{
						((ComPort)base._Connection).PortName = 
							((ParameterStringContainer)value).Value;
						break;
					}
				case ParameterNamesIncotexNetworkVirtual.Timeout:
					{
						RequestTimeout = (int)value; break;
					}
				default:
					{
						throw new InvalidOperationException(String.Format(
							"Ошибка чтения параметра. Параметр {0} не найден", parameterName));
					}
			}
		}

        #endregion

        #region Events
        #endregion
    }
}
