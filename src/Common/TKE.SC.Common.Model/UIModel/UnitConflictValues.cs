using System.Collections.Generic;
using TKE.SC.Common.Model;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// UnitConflictValues
    /// </summary>
    public class UnitConflictValues
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public List<SystemValidationConflictsValues> ConflictVaribales { get; set; }
    }
}
