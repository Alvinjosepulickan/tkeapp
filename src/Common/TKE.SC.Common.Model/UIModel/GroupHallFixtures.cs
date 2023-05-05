using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// GroupHallFixturesData
    /// </summary>
    public class GroupHallFixturesData
    {
        public object GroupHallFixtureConsoleId { get; set; }
        public string ConsoleName { get; set; }
        public bool IsController { get; set; }
        public string FixtureType { get; set; }
        public List<ConfigVariable> VariableAssignments { get; set; }
        public List<GroupHallFixtureLocation> GroupHallFixtureLocations { get; set; }
    }

    /// <summary>
    /// GroupHallFixtureLocations
    /// </summary>
    public class GroupHallFixtureLocations
    {
        public int FloorNumber { get; set; }
        public int UnitId { get; set; }
        public string FloorDesignation { get; set; }
        public LandingOpening Front { get; set; }
        public LandingOpening Rear { get; set; }
        public string UnitDesignation { get; set; }
        public string HallStationName { get; set; }

    }

    /// <summary>
    /// UnitDetailsValues
    /// </summary>
    public class UnitDetailsValues
    {
        public int UnitId { get; set; }
        public string UniDesgination { get; set; }
        public string MainEgress { get; set; }
        public string AlternateEgress { get; set; }
        public string unitName { get; set; }
        public bool OccupiedSpaceBelow { get; set; }
        public OpeningDoors openingDoors { get; set; }
        public int NoOfFloors { get; set; }
        public List<Opening> openingsAssigned { get; set; }
        public List<GroupHallFixtureLocations> UnitGroupValues { get; set; }
        public string HallStationName { get; set; }
    }

    /// <summary>
    /// GroupHallFixtureLocation
    /// </summary>
    public class GroupHallFixtureLocation
    {
        public int UnitId { get; set; }
        public string HallStationId { get; set; }
        public string HallStationName { get; set; }
        public List<OpeningsDetails> Assignments { get; set; }

    }

    /// <summary>
    /// OpeningsDetails
    /// </summary>
    public class OpeningsDetails
    {
        public int FloorNumber { get; set; }
        public string FloorDesignation { get; set; }
        public bool Front { get; set; }
        public bool Rear { get; set; }
    }

    /// <summary>
    /// GroupHallFixtures
    /// </summary>
    public class GroupHallFixtures
    {
        public int ConsoleId { get; set; }
        public string ConsoleName { get; set; }
        public bool IsController { get; set; }
        public bool AssignOpenings { get; set; }
        public int NoOfFloor { get; set; }
        public Openings Openings { get; set; }
        public List<ConfigVariable> VariableAssignments { get; set; }
        public List<GroupHallFixtureLocations> GroupHallFixtureLocations { get; set; }
        public JArray FixtureAssignments { get; set; }
        public List<UnitDetailsValues> UnitDetails { get; set; }
        public string GroupHallFixtureType { get; set; }
        public List<HallStations> HallStations { get; set; }
        public List<string> VariableIds { get; set; }
    }

        /// <summary>
        /// HallStations
        /// </summary>
        public class HallStations
    {
        public string HallStationId { get; set; }
        public string HallStationName { get; set; }
        public OpeningDoors openingDoors { get; set; }
        public int NoOfFloors { get; set; }
        public List<GroupHallFixtureLocations> openingsAssigned { get; set; }
    }
}
   
