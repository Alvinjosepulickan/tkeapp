using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    public class ConflictManagement
    {
        /// <summary>
        /// Removed Configurataions
        /// </summary>
        public List<ConflictMgmtList> ResolvedAssignments { get; set; }
        /// <summary>
        /// PendingConfiguration
        /// </summary>
        public List<ConflictMgmtList> PendingAssignments { get; set; }
        /// <summary>
        /// ValidationAssignments
        /// </summary>
        public List<UnitConflictValues> ValidationAssignments { get; set; }
    }
}