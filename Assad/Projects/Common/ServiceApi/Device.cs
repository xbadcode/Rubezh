﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Firesec;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract(IsReference=true)]
    public class Device
    {
        [DataMember]
        public string Description { get; set; }

        public string DriverName
        {
            get
            {
                return FiresecMetadata.DriversHelper.GetDriverNameById(DriverId);
            }
        }

        [DataMember]
        public string DriverId { get; set; }

        [DataMember]
        public string ValidationError { get; set; }

        [DataMember]
        public List<string> Zones { get; set; }

        [DataMember]
        public List<State> States { get; set; }

        [DataMember]
        public List<DeviceProperty> DeviceProperties { get; set; }

        [IgnoreDataMember]
        public List<Firesec.Metadata.paramInfoType> Parameters { get; set; }
        [IgnoreDataMember]
        public List<Firesec.Metadata.propInfoType> Properties { get; set; }

        [IgnoreDataMember]
        public Firesec.CoreConfig.devType InnerDevice { get; set; }

        public List<string> AvailableFunctions { get; set; }
        public List<string> AvailableEvents { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public int MinPriority { get; set; }

        [DataMember]
        public string SourceState { get; set; }

        [DataMember]
        public bool AffectChildren { get; set; }

        [DataMember]
        public List<string> LastEvents { get; set; }

        // свойство, по которому можно идентифицировать устройство в текущей конфигурации

        public string PlaceInTree { get; set; }

        [DataMember]
        public string Address { get; set; }

        // главное всойство, по которому можно идентифицировать устройство в системе

        [DataMember]
        public string Path { get; set; }

        Device parent;
        [DataMember]
        public Device Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                if (parent != null)
                {
                    parent.Children.Add(this);
                }
            }
        }

        List<Device> children;
        [DataMember]
        public List<Device> Children
        {
            get
            {
                if (children == null)
                    children = new List<Device>();
                return children;
            }
            set
            {
                children = value;
            }
        }
    }
}
