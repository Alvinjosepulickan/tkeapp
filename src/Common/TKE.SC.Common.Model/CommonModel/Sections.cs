using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// Sections
    /// </summary>
    public class Sections
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// OpeningRange For Consoles
        /// </summary>
        public string OpeningRange { get; set; }
        /// <summary>
        /// assignOpenings
        /// </summary>
        public Nullable<bool> assignOpenings { get; set; }
        /// <summary>
        /// isController
        /// </summary>
        public Nullable<bool> isController { get; set; }
        /// <summary>
        /// isLobby
        /// </summary>
        public Nullable<bool> isLobby { get; set; }
        /// <summary>
        /// fixtureType
        /// </summary>
        public string fixtureType { get; set; }

        /// <summary>
        /// Sections
        /// </summary>
        public IList<SectionsValues> sections { get; set; }
        /// <summary>
        /// Entrance Locations
        /// </summary>
        public EntranceAssignment FixtureLocations { get; set; }
        /// <summary>
        /// UnitDetails
        /// </summary>
        public List<UnitDetailsValues> UnitDetails { get; set; }
        /// <summary>
        /// Compartments
        /// </summary>
        public IList<Compartment> Compartments { get; set; }
        /// <summary>
        /// Variables
        /// </summary>
        public IList<Variables> Variables { get; set; }
        /// <summary>
        /// Properties
        /// </summary>
        public IList<PropertyDetailsValues> Properties { get; set; }
        /// <summary>
        /// FixtureAssignments
        /// </summary>
        public JArray FixtureAssignments { get; set; }
        /// <summary>
        /// Units
        /// </summary>
        public List<UnitNames> Units { get; set; }
        /// <summary>
        /// ConfiguredGroups
        /// </summary>
        public IList<ConfiguredGroups> configuredGroups { get; set; }
        /// <summary>
        /// ExistingGroups
        /// </summary>
        public IList<BuildingEquipmentGroupDetails> existingGroups { get; set; }
        /// <summary>
        /// FutureGroups
        /// </summary>
        public IList<BuildingEquipmentGroupDetails> futureGroups { get; set; }
        /// <summary>
        /// IsAllOpeningsSelected
        /// </summary>
        public Nullable<bool> AllOpeningsSelected { get; set; }
        /// <summary>
        /// for device slots
        /// </summary>
        public deviceLimit deviceLimit { get; set; }
        /// <summary>
        /// IsHallStation
        /// </summary>
        public bool IsHallStation { get; set; }
        /// <summary>
        /// IsDelete
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// AssignedGroups
        /// </summary>
        public Nullable<int> AssignedGroups { get; set; }
        /// <summary>
        /// AssignedUnits
        /// </summary>
        public Nullable<int> AssignedUnits { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public Nullable<int> Quantity { get; set; }
        /// <summary>
        /// for opening location
        /// </summary>
        public OpeningLocations OpeningLocationResponse { get; set; }
        /// <summary>
        /// DisplaySection
        /// </summary>
        public Nullable<bool> DisplaySection { get; set; }
        public JObject EnrichedData { get; set; }
        public bool LobbyRecallSwitchConfigured { get; set; }
        public List<LobbyRecallSwitchConfigured> LobbyRecallSwitchPerGroup { get; set; }
        public List<UnitLayOutDetails> UnitLayoutDetails { get; set; }
        public List<HallStationDetails> HallStationDetails { get; set; }

        public bool CarRidingQuantityFlag { get; set; }
    }
}
