using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// GroupLayoutSave
    /// </summary>
    public class GroupLayoutSave
    {
        /// <summary>
        /// Operation
        /// </summary>
        public Operation Operation { get; set; }
        /// <summary>
        /// UnitID
        /// </summary>
        public List<int> UnitID { get; set; }
        /// <summary>
        /// GroupID
        /// </summary>
        public int GroupID { get; set; }
        /// <summary>
        /// CarPosition
        /// </summary>
        public List<CarPosition> CarPosition { get; set; }
        /// <summary>
        /// VariableAssignments
        /// </summary>
        public JArray VariableAssignments { get; set; }
        /// <summary>
        /// DisplayVariableAssignments
        /// </summary>
        public List<DisplayVariableAssignmentsValues> DisplayVariableAssignments { get; set; }
        /// <summary>
        /// SectionTab
        /// </summary>
        public string SectionTab { get; set; }
    }

    /// <summary>
    /// CarPosition
    /// </summary>
    public class CarPosition
    {
        /// <summary>
        /// CarPosition Variable
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// UnitDesignationVariable
        /// </summary>
        public string UnitDesignation { get; set; }
    }
}