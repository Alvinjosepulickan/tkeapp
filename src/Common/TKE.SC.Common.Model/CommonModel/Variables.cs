using System.Collections.Generic;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// Variables
    /// </summary>
    public class Variables
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Values
        /// </summary>
        public IList<Values> Values { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Sequence
        /// </summary>
        public string Sequence { get; set; }
        /// <summary>
        /// Properties
        /// </summary>
        public IList<Properties> Properties { get; set; }
        /// <summary>
        /// LeadTime
        /// </summary>
        public string LeadTime { get; set; }
        /// <summary>
        /// itemNumber
        /// </summary>
        public string ItemNumber { get; set; }
        /// <summary>
        /// ValueType
        /// </summary>
        public string ValueType { get; set; }
        /// <summary>
        /// AllowMultipleAssignments
        /// </summary>
        public bool AllowMultipleAssignments { get; set; }
        /// <summary>
        /// Reason
        /// </summary>
        public string Reason { get; set; }
    }
}
