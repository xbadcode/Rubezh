﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class ShortDevice
    {
        [DataMember]
        public List<ShortDevice> Children { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string DriverId { get; set; }

        [DataMember]
        public string ValidationError { get; set; }

        [DataMember]
        public string Zone { get; set; }

        //[DataMember]
        //public List<DeviceProperty> DeviceProperties { get; set; }

        [DataMember]
        public List<Parameter> Parameters { get; set; }

        [DataMember]
        public List<string> AvailableFunctions { get; set; }

        //[DataMember]
        //public string State { get; set; }

        //[DataMember]
        //public List<string> States { get; set; }

        [DataMember]
        public string PlaceInTree { get; set; }

        [DataMember]
        public string Address { get; set; }

        //[DataMember]
        //public bool StateChanged { get; set; }

        //[DataMember]
        //public bool StatesChanged { get; set; }

        //[DataMember]
        //public bool ParameterChanged { get; set; }

        //[DataMember]
        //public bool VisibleParameterChanged { get; set; }

        [DataMember]
        public string Path { get; set; }
    }
}
