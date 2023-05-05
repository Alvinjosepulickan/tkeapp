using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// UnitHallFixtureData
    /// </summary>
    public class UnitHallFixtureData
    {
        public object ConsoleId { get; set; }
        public string ConsoleName { get; set; }
        public bool IsController { get; set; }
        public List<ConfigVariable> VariableAssignments { get; set; }
        public List<UnitHallFixtureLocation> FixtureLocations { get; set; }
        public string FixtureType { get; set; }
    }

    /// <summary>
    /// UnitHallFixtures
    /// </summary>
    public class UnitHallFixtures
    {
        public int ConsoleId { get; set; }
        public string ConsoleName { get; set; }
        public bool IsController { get; set; }
        public bool AssignOpenings { get; set; }
        public string ProductName { get; set; }
        public int NoOfFloor { get; set; }
        public Openings Openings { get; set; }
        public List<ConfigVariable> VariableAssignments { get; set; }
        public List<EntranceLocations> UnitHallFixtureLocations { get; set; }
        public string UnitHallFixtureType { get; set; }
    }

    /// <summary>
    /// UnitHallFixtureLocation
    /// </summary>
    public class UnitHallFixtureLocation
    {
        public int FloorNumber { get; set; }
        public bool Front { get; set; }
        public bool Rear { get; set; }
    }
}
