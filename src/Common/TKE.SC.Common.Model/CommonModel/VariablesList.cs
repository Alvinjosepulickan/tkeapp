using System.Collections.Generic;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// VariablesList
    /// </summary>
    public class VariablesList
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// ListVariableAssignment
        /// </summary>
        public List<ConfigVariable> VariableAssignments { get; set; }
    }
}
