using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// QuickSummary
    /// </summary>
    public class QuickSummary
    {
        public OpportunityDetails project { get; set; }
        public BuildingDetails building { get; set; }
        public GroupDetails group { get; set; }
        public UnitsTable units { get; set; }
    }

    /// <summary>
    /// OpportunityDetails
    /// </summary>
    public class OpportunityDetails
    {
        public string Id { get; set; }
        public string OpportunityId { get; set; }
        public string VersionId { get; set; }
        public string QuoteId { get; set; }
        public string OpportunityName { get; set; }
        public AccountEntity AccountAddress { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public ProjectDetails projectDetails { get; set; }
    }

    /// <summary>
    /// ProjectDetails
    /// </summary>
    public class ProjectDetails
    {
        public int numberOfBuildings { get; set; }
    }

    /// <summary>
    /// BuildingDetails
    /// </summary>
    public class BuildingDetails
    {
        public int id { get; set; }
        public string name { get; set; }
        public int numberOfGroups { get; set; }
    }

    /// <summary>
    /// GroupDetails
    /// </summary>
    public class GroupDetails
    {
        public int id { get; set; }
        public string name { get; set; }
        public int numberOfUnits { get; set; }
        public List<UnitLayOutDetails> unitLayoutDetails { get; set; }
        public List<HallStationDetails> hallStationDetails { get; set; }
    }

    /// <summary>
    /// UnitsTable
    /// </summary>
    public class UnitsTable
    {
        public string model { get; set; }
        public List<SelectedUnits> selectedUnits { get; set; }
        public UnitConfigurationDetails1 unitDetails { get; set; }
        public OpeningDetail openingDetails { get; set; }
        public List<UnitLayOutDetails> unitLayoutDetails { get; set; }
        public UnitConfigurationDetails unitConfigurationDetails { get; set; }
    }

    /// <summary>
    /// UnitsDetails
    /// </summary>
    public class UnitsDetails
    {
        public List<UnitLayOutDetails> unitLayoutDetails { get; set; }

    }

    /// <summary>
    /// SelectedUnits
    /// </summary>
    public class SelectedUnits
    {
        public string ueid { get; set; }
        public int unitid { get; set; }
        public string unitname { get; set; }
    }

    /// <summary>
    /// UnitConfigurationDetails
    /// </summary>
    public class UnitConfigurationDetails
    {
        public List<VariablesDetails> variables { get; set; }
    }

    /// <summary>
    /// VariablesDetails
    /// </summary>
    public class VariablesDetails
    {
        public string id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string unitType { get; set; }

    }

    /// <summary>
    /// OpeningDetail
    /// </summary>
    public class OpeningDetail
    {
        public TypicalFloor travel { get; set; }
        public int frontOpenings { get; set; }
        public int rearOpenings { get; set; }
        public int floorsServed { get; set; }
    }

    /// <summary>
    /// UnitLayOutDetails
    /// </summary>
    public class UnitLayOutDetails
    {
        public string unitDesignation { get; set; }
        public string displayCarPosition { get; set; }
        public bool unitCurrentlyConfigured { get; set; }
        public DoorOpenings doorOpenings { get; set; }
        public IList<Variables> layoutVariables { get; set; }
    }

    /// <summary>
    /// DoorOpenings
    /// </summary>
    public class DoorOpenings
    {
        public Front frontDoor { get; set; }
        public Front rearDoor { get; set; }
        public Front leftSideDoor { get; set; }
        public Front rightSideDoor { get; set; }
    }

    /// <summary>
    /// SumPitQty
    /// </summary>
    public class SumPitQty
    {
        public string id { get; set; }
        public int value { get; set; }
    }

    /// <summary>
    /// Front
    /// </summary>
    public class Front
    {
        public bool isSelected { get; set; }
        public string doorTypeHand { get; set; }
    }

    /// <summary>
    /// UnitConfigurationDetails1
    /// </summary>
    public class UnitConfigurationDetails1
    {
        public string capacity { get; set; }
        public string speed { get; set; }
        public string status { get; set; }
        public string width { get; set; }
        public string depth { get; set; }
        public string pitDepth { get; set; }
        public string overHead { get; set; }
        public string dimensionSelection { get; set; }
        public string machineType { get; set; }
        public string motorTypeSize {get;set;}
        public string availableFinishWeight { get; set; }
        public string grossLoadOnJacks { get; set; }
        public string grossLoadOnPowerUnit { get; set; }

    }
    public class HallStationDetails
    {
        public ConfigVariable hallStationVariables { get; set; }
        public string key { get; set; }
    }
}