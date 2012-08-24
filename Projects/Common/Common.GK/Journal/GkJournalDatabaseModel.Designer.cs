﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]

namespace Common.GK.Journal
{
    #region Контексты
    
    /// <summary>
    /// Нет доступной документации по метаданным.
    /// </summary>
    public partial class GkJournalDatabaseEntities : ObjectContext
    {
        #region Конструкторы
    
        /// <summary>
        /// Инициализирует новый объект GkJournalDatabaseEntities, используя строку соединения из раздела "GkJournalDatabaseEntities" файла конфигурации приложения.
        /// </summary>
        public GkJournalDatabaseEntities() : base("name=GkJournalDatabaseEntities", "GkJournalDatabaseEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Инициализация нового объекта GkJournalDatabaseEntities.
        /// </summary>
        public GkJournalDatabaseEntities(string connectionString) : base(connectionString, "GkJournalDatabaseEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Инициализация нового объекта GkJournalDatabaseEntities.
        /// </summary>
        public GkJournalDatabaseEntities(EntityConnection connection) : base(connection, "GkJournalDatabaseEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Разделяемые методы
    
        partial void OnContextCreated();
    
        #endregion
    
        #region Свойства ObjectSet
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        public ObjectSet<gkEvent> gkEvents
        {
            get
            {
                if ((_gkEvents == null))
                {
                    _gkEvents = base.CreateObjectSet<gkEvent>("gkEvents");
                }
                return _gkEvents;
            }
        }
        private ObjectSet<gkEvent> _gkEvents;

        #endregion
        #region Методы AddTo
    
        /// <summary>
        /// Устаревший метод для добавления новых объектов в набор EntitySet gkEvents. Взамен можно использовать метод .Add связанного свойства ObjectSet&lt;T&gt;.
        /// </summary>
        public void AddTogkEvents(gkEvent gkEvent)
        {
            base.AddObject("gkEvents", gkEvent);
        }

        #endregion
    }
    

    #endregion
    
    #region Сущности
    
    /// <summary>
    /// Нет доступной документации по метаданным.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="GkJournalDatabaseModel", Name="gkEvent")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class gkEvent : EntityObject
    {
        #region Фабричный метод
    
        /// <summary>
        /// Создание нового объекта gkEvent.
        /// </summary>
        /// <param name="gKNo">Исходное значение свойства GKNo.</param>
        public static gkEvent CreategkEvent(global::System.Int32 gKNo)
        {
            gkEvent gkEvent = new gkEvent();
            gkEvent.GKNo = gKNo;
            return gkEvent;
        }

        #endregion
        #region Свойства-примитивы
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 GKNo
        {
            get
            {
                return _GKNo;
            }
            set
            {
                if (_GKNo != value)
                {
                    OnGKNoChanging(value);
                    ReportPropertyChanging("GKNo");
                    _GKNo = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("GKNo");
                    OnGKNoChanged();
                }
            }
        }
        private global::System.Int32 _GKNo;
        partial void OnGKNoChanging(global::System.Int32 value);
        partial void OnGKNoChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> Date
        {
            get
            {
                return _Date;
            }
            set
            {
                OnDateChanging(value);
                ReportPropertyChanging("Date");
                _Date = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Date");
                OnDateChanged();
            }
        }
        private Nullable<global::System.DateTime> _Date;
        partial void OnDateChanging(Nullable<global::System.DateTime> value);
        partial void OnDateChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String EventName
        {
            get
            {
                return _EventName;
            }
            set
            {
                OnEventNameChanging(value);
                ReportPropertyChanging("EventName");
                _EventName = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("EventName");
                OnEventNameChanged();
            }
        }
        private global::System.String _EventName;
        partial void OnEventNameChanging(global::System.String value);
        partial void OnEventNameChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String EventDescription
        {
            get
            {
                return _EventDescription;
            }
            set
            {
                OnEventDescriptionChanging(value);
                ReportPropertyChanging("EventDescription");
                _EventDescription = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("EventDescription");
                OnEventDescriptionChanged();
            }
        }
        private global::System.String _EventDescription;
        partial void OnEventDescriptionChanging(global::System.String value);
        partial void OnEventDescriptionChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> KAUNo
        {
            get
            {
                return _KAUNo;
            }
            set
            {
                OnKAUNoChanging(value);
                ReportPropertyChanging("KAUNo");
                _KAUNo = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("KAUNo");
                OnKAUNoChanged();
            }
        }
        private Nullable<global::System.Int32> _KAUNo;
        partial void OnKAUNoChanging(Nullable<global::System.Int32> value);
        partial void OnKAUNoChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> GKObjectNo
        {
            get
            {
                return _GKObjectNo;
            }
            set
            {
                OnGKObjectNoChanging(value);
                ReportPropertyChanging("GKObjectNo");
                _GKObjectNo = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("GKObjectNo");
                OnGKObjectNoChanged();
            }
        }
        private Nullable<global::System.Int32> _GKObjectNo;
        partial void OnGKObjectNoChanging(Nullable<global::System.Int32> value);
        partial void OnGKObjectNoChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> KAUAddress
        {
            get
            {
                return _KAUAddress;
            }
            set
            {
                OnKAUAddressChanging(value);
                ReportPropertyChanging("KAUAddress");
                _KAUAddress = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("KAUAddress");
                OnKAUAddressChanged();
            }
        }
        private Nullable<global::System.Int32> _KAUAddress;
        partial void OnKAUAddressChanging(Nullable<global::System.Int32> value);
        partial void OnKAUAddressChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> Code
        {
            get
            {
                return _Code;
            }
            set
            {
                OnCodeChanging(value);
                ReportPropertyChanging("Code");
                _Code = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Code");
                OnCodeChanged();
            }
        }
        private Nullable<global::System.Int32> _Code;
        partial void OnCodeChanging(Nullable<global::System.Int32> value);
        partial void OnCodeChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ObjectNo
        {
            get
            {
                return _ObjectNo;
            }
            set
            {
                OnObjectNoChanging(value);
                ReportPropertyChanging("ObjectNo");
                _ObjectNo = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ObjectNo");
                OnObjectNoChanged();
            }
        }
        private Nullable<global::System.Int32> _ObjectNo;
        partial void OnObjectNoChanging(Nullable<global::System.Int32> value);
        partial void OnObjectNoChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ObjectDeviceType
        {
            get
            {
                return _ObjectDeviceType;
            }
            set
            {
                OnObjectDeviceTypeChanging(value);
                ReportPropertyChanging("ObjectDeviceType");
                _ObjectDeviceType = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ObjectDeviceType");
                OnObjectDeviceTypeChanged();
            }
        }
        private Nullable<global::System.Int32> _ObjectDeviceType;
        partial void OnObjectDeviceTypeChanging(Nullable<global::System.Int32> value);
        partial void OnObjectDeviceTypeChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ObjectDeviceAddress
        {
            get
            {
                return _ObjectDeviceAddress;
            }
            set
            {
                OnObjectDeviceAddressChanging(value);
                ReportPropertyChanging("ObjectDeviceAddress");
                _ObjectDeviceAddress = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ObjectDeviceAddress");
                OnObjectDeviceAddressChanged();
            }
        }
        private Nullable<global::System.Int32> _ObjectDeviceAddress;
        partial void OnObjectDeviceAddressChanging(Nullable<global::System.Int32> value);
        partial void OnObjectDeviceAddressChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ObjectFactoryNo
        {
            get
            {
                return _ObjectFactoryNo;
            }
            set
            {
                OnObjectFactoryNoChanging(value);
                ReportPropertyChanging("ObjectFactoryNo");
                _ObjectFactoryNo = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ObjectFactoryNo");
                OnObjectFactoryNoChanged();
            }
        }
        private Nullable<global::System.Int32> _ObjectFactoryNo;
        partial void OnObjectFactoryNoChanging(Nullable<global::System.Int32> value);
        partial void OnObjectFactoryNoChanged();
    
        /// <summary>
        /// Нет доступной документации по метаданным.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ObjectState
        {
            get
            {
                return _ObjectState;
            }
            set
            {
                OnObjectStateChanging(value);
                ReportPropertyChanging("ObjectState");
                _ObjectState = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ObjectState");
                OnObjectStateChanged();
            }
        }
        private Nullable<global::System.Int32> _ObjectState;
        partial void OnObjectStateChanging(Nullable<global::System.Int32> value);
        partial void OnObjectStateChanged();

        #endregion
    
    }

    #endregion
    
}
