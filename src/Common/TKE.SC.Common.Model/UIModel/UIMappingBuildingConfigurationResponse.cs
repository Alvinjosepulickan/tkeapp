using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TKE.SC.Common.Model;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// UIMappingBuildingConfigurationResponse
    /// </summary>
    public class UIMappingBuildingConfigurationResponse : ConfigurationResponse
    {
        /// <summary>
        /// FloorPlanRules
        /// </summary>
        public JObject FloorPlanRules { get; set; }
        /// <summary>
        /// DisplayVariableAssignments
        /// </summary>
        public List<DisplayVariableAssignmentsValues> DisplayVariableAssignments { get; set; }
        /// <summary>
        /// IsEditFlow
        /// </summary>
        public bool IsEditFlow { get; set; }
        /// <summary>
        /// EntranceConfigurationExists
        /// </summary>
        public bool EntranceConfigurationExists { get; set; }
    }
}