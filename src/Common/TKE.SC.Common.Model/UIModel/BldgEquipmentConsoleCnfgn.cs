using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// BuildingEquipmentData
    /// </summary>
    public class BuildingEquipmentData
    {
        public int ConsoleId { get; set; }
        public int ConsoleNumber { get; set; }
        public string ConsoleName { get; set; }
        public bool IsController { get; set; }
        public bool IsLobbyPanel { get; set; }
        public List<ConfiguredGroups> lstConfiguredGroups { get; set; }
        public bool IsLobby { get; set; }
        public bool IsChecked { get; set; }
        public List<ConfigVariable> VariableAssignments { get; set; }
        public List<BuildingEquipmentGroupDetails> lstExistingGroups { get; set; }
        public List<BuildingEquipmentGroupDetails> lstFutureGroup { get; set; }
        public int AssignedGroups { get; set; }
        public int AssignedUnits { get; set; }
        public List<ConfiguredGroups> ConfiguredGroups { get; set; }
        public List<BuildingEquipmentGroupDetails> FutureGroups { get; set; }
        public List<BuildingEquipmentGroupDetails> ExistingGroups { get; set; }
}

    /// <summary>
    /// ConfiguredGroups
    /// </summary>
    public class ConfiguredGroups
    {
        public int consoleId { get; set; }
        public int groupId { get; set; }
        public string groupName { get; set; }
        public int totalGroups { get; set; }
        public int noOfUnits { get; set; }
        public bool isChecked { get; set; }
        public bool InCompatible { get; set; }
    }

    /// <summary>
    /// BuildingEquipmentGroupDetails
    /// </summary>
    public class BuildingEquipmentGroupDetails
    {
        public int consoleId { get; set; }
        public string groupCategoryId { get; set; }
        public string groupCategoryName { get; set; }
        public string groupName { get; set; }
        public int noOfUnits { get; set; }
        public string groupFactoryId { get; set; }
        public List<BEUnitDetails> units { get; set; }
    }

    /// <summary>
    /// BEUnitDetails
    /// </summary>
    public class BEUnitDetails
    {
        public string unitName { get; set; }
        public string factoryId { get; set; }
    }

    public class LobbyRecallSwitchConfigured
    {
        public int groupId { get; set; }
        public bool lobbyRecallSwitch { get; set; }
    }
}
