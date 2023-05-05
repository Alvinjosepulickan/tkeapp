using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// SectionsValues
    /// </summary>
    public class SectionsValues
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
        /// Property for checking all the openings for entrance consoles are selected
        /// </summary>
        public Nullable<bool> AllOpeningsSelected { get; set; }
        /// <summary>
        /// OpeningRange For Consoles
        /// </summary>
        public string OpeningRange { get; set; }
        /// <summary>
        /// AssignedGroups
        /// </summary>
        public Nullable<int> AssignedGroups { get; set; }
        /// <summary>
        /// AssignedUnits
        /// </summary>
        public Nullable<int> AssignedUnits { get; set; }
        /// <summary>
        /// assignOpenings
        /// </summary>
        public Nullable<bool> assignOpenings { get; set; }

        /// <summary>
        /// Beam Wall layout
        /// </summary>
        public IList<UnitLayOutDetails> Layout { get; set; }

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
        public IList<SectionsGroupValues> sections { get; set; }
        /// <summary>
        /// Variables
        /// </summary>
        public IList<Variables> Variables { get; set; }
        /// <summary>
        /// Properties
        /// </summary>
        public IList<PropertyDetailsValues> Properties { get; set; }
        /// <summary>
        /// isDelete
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public Nullable<int> Quantity { get; set; }
        /// <summary>
        /// ReadOnly property
        /// </summary>
        public bool IsHallStation { get; set; }
        /// <summary>
        /// Compartments
        /// </summary>
        public IList<Compartment> Compartments { get; set; }
        /// <summary>
        /// DisplaySection
        /// </summary>
        public Nullable<bool> DisplaySection { get; set; }
        /// <summary>
        /// ConfiguredGroups
        /// </summary>
        public IList<ConfiguredGroups> configuredGroups { get; set; }
        public bool LobbyRecallSwitchConfigured { get; set; }
        public List<LobbyRecallSwitchConfigured> LobbyRecallSwitchPerGroup { get; set; }
        /// <summary>
        /// CounterWeightLocation
        /// </summary>
        public Variables CounterWeightLocation { get; set; }

    }

}
