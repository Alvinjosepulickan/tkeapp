using System.Collections.Generic;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// RemovedAssignments
    /// </summary>
    public class RemovedAssignments
    {
        /// <summary>
        /// VariableAssignments
        /// </summary>
        public IList<RemovedAssignment> variableAssignments { get; set; }
    }
}
