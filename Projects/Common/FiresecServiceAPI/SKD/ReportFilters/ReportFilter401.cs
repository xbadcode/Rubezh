﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Journal;

namespace FiresecAPI.SKD.ReportFilters
{
    [DataContract]
    public class ReportFilter401 : SKDReportFilter, IReportFilterPeriod, IReportFilterEmployee
    {
        public ReportFilter401()
        {
            JournalEventSubsystemTypes = new List<JournalSubsystemType>();
            JournalOjbectSubsystemTypes = new List<JournalSubsystemType>();
            JournalEventNameTypes = new List<JournalEventNameType>();
            JournalObjectTypes = new List<JournalObjectType>();
            ObjectUIDs = new List<Guid>();
        }

        #region IReportFilterPeriod Members

        [DataMember]
        public ReportPeriodType PeriodType { get; set; }

        [DataMember]
        public DateTime DateTimeFrom { get; set; }

        [DataMember]
        public DateTime DateTimeTo { get; set; }

        #endregion
 
        #region IReportFilterEmployee Members

        [DataMember]
        public List<Guid> Employees { get; set; }

        #endregion

        [DataMember]
        public List<JournalSubsystemType> JournalEventSubsystemTypes { get; set; }
        [DataMember]
        public List<JournalEventNameType> JournalEventNameTypes { get; set; }
        [DataMember]
        public List<JournalSubsystemType> JournalOjbectSubsystemTypes { get; set; }
        [DataMember]
        public List<JournalObjectType> JournalObjectTypes { get; set; }
        [DataMember]
        public List<Guid> ObjectUIDs { get; set; }
    }
}
