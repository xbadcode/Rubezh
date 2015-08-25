﻿using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class FiresecLicenseInfo
    {
        [DataMember]
        public int NumberOfUsers { get; set; }
        [DataMember]
        public bool FireAlarm { get; set; }
        [DataMember]
        public bool SecurityAlarm { get; set; }
        [DataMember]
        public bool Skd { get; set; }
        [DataMember]
        public bool ControlScripts { get; set; }
        [DataMember]
        public bool OrsServer { get; set; }
    }
}