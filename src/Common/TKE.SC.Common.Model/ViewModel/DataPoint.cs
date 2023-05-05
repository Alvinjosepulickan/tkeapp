using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.ViewModel
{
    public class DataPoint
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public bool IsAcknowledged { get; set; }
        public string SetId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GroupVariableAssignmentsForFlags
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
