using System.Collections.Generic;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// SystemValidationConflictsValues
    /// </summary>
    public class SystemValidationConflictsValues
    {
        /// <summary>
        /// FlagId
        /// </summary>
        public string FlagId { get; set; }
        /// <summary>
        /// Description 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// SectionId
        /// </summary>
        public List<string> SectionId { get; set; }
        /// <summary>
        /// SystemConflictVariables
        /// </summary>
        public List<string> SystemConflictVariables { get; set; }
        /// <summary>
        /// UnitId
        /// </summary>
        public int UnitId { get; set; }
    }
}
