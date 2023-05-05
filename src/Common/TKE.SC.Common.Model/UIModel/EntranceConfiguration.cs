using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// EntranceConfigurationData
    /// </summary>
    public class EntranceConfigurationData
    {
        public object EntranceConsoleId { get; set; }
        public string ConsoleName { get; set; }
        public bool IsController { get; set; }
        public List<ConfigVariable> VariableAssignments { get; set; }
        public List<EntranceLocation> EntranceLocations { get; set; }
    }

    /// <summary>
    /// EntranceConfigurations
    /// </summary>
    public class EntranceConfigurations
    {
        public int EntranceConsoleId { get; set; }
        public string ConsoleName { get; set; }
        public bool IsController { get; set; }
        public bool AssignOpenings { get; set; }
        public string ProductName { get; set; }
        public int NoOfFloor { get; set; }
        public int FrontOpenings { get; set; }
        public int RearOpenings { get; set; }
        public Openings Openings { get; set; }
        public List<ConfigVariable> VariableAssignments { get; set; }
        public List<EntranceLocations> FixtureLocations { get; set; }
    }

    /// <summary>
    ///EntranceLocations 
    /// </summary>
    public class EntranceLocations
    {
        public int FloorNumber { get; set; }
        public string FloorDesignation { get; set; }
        public LandingOpening Front { get; set; }
        public LandingOpening Rear { get; set; }
    }

    /// <summary>
    /// EntranceLocation
    /// </summary>
    public class EntranceLocation
    {
        public int FloorNumber { get; set; }
        public bool Front { get; set; }
        public bool Rear { get; set; }
    }

    /// <summary>
    /// Openings
    /// </summary>
    public class Openings
    {
        public bool Front { get; set; }
        public bool Rear { get; set; }
    }

    /// <summary>
    /// LandingOpening
    /// </summary>
    public class LandingOpening
    {
        public bool InCompatible { get; set; }
        public object Value { get; set; }
        public bool NotAvailable { get; set; }
    }

    /// <summary>
    /// IncludeSection
    /// </summary>
    public class IncludeSection
    {
        public string ConsoleNumber { get; set; }
        public List<string> IncludeSections { get; set; }
    }

    /// <summary>
    /// EntranceAssignment
    /// </summary>
    public class EntranceAssignment
    {
        public Openings Openings { get; set; }
        public List<EntranceLocations> FixtureAssignments { get; set; }
        public bool IsSaved { get; set; }
    }
}