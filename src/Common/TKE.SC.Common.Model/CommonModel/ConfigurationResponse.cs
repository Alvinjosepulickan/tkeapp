using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.Common.Model
{
    public class ConfigurationResponse
    {
        /// <summary>
        /// Sections
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Section Values
        /// </summary>
        public IList<Sections> Sections { get; set; }
        /// <summary>
        /// RemovedAssignments
        /// </summary>
        public RemovedAssignments RemovedAssignments { get; set; }
        /// <summary>
        /// ConflictAssignments
        /// </summary>
        public ConflictManagement ConflictAssignments { get; set; }
        /// <summary>
        /// UnitLayoutDetails
        /// </summary>
        public List<UnitLayOutDetails> UnitLayoutDetails { get; set; }
    
        /// <summary>
        /// Beam Wall layout
        /// </summary>
        public bool AllBeamsSelected { get; set; }
        /// <summary>
        /// Group Status
        /// </summary>
        public string GroupStatus { get; set; }

        /// <summary>
        /// Drawing Status
        /// </summary>
        public string DrawingStatus { get; set; }

        /// <summary>
        /// Units
        /// </summary>
        public List<UnitNames> Units { get; set; }
        /// <summary>
        /// readonly property
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// EnrichedData
        /// </summary>
        public JObject EnrichedData { get; set; }
        /// <summary>
        /// ConfigurationStatus
        /// </summary>
        public string ConfigurationStatus { get; set; }
        /// <summary>
        /// SystemValidateStatus
        /// </summary>
        public Status SystemValidateStatus { get; set; }
        /// <summary>
        /// List of permission keys
        /// </summary>
        public List<string> Permissions { get; set; }
        /// <summary>
        /// ConfiguratorStatus
        /// </summary>
        public Status ConfiguratorStatus { get; set; }
        /// <summary>
        /// readonly property
        /// </summary>
        public bool EgressDistanceExceeded { get; set; }
        /// <summary>
        /// LobbyRecallSwitchConfigured
        /// </summary>
        public bool LobbyRecallSwitchConfigured { get; set; }
        public List<LobbyRecallSwitchConfigured> LobbyRecallSwitchPerGroup { get; set; }
        public bool BuildingEquipmentConfigured { get; set; }
        /// <summary>
        /// AllGroupsNotAssigned
        /// </summary>
        public bool AllGroupsNotAssigned { get; set; }
        /// <summary>
        /// IsAllOpeningsSelected
        /// </summary>
        public bool AllOpeningsSelected { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ConfigurationConflictExists { get; set; }
    }
}
