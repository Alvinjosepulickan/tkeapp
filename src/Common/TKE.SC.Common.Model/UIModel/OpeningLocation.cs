using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model.UIModel
{
	public class OpeningLocations
	{
		/// <summary>
		/// DisplayVariableAssignments
		/// </summary>
		public List<DisplayVariableAssignmentsValues> DisplayVariableAssignments { get; set; }
		/// <summary>
		/// GroupConfigurationId
		/// </summary>
		public int GroupConfigurationId { get; set; }
		/// <summary>
		/// username
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// building rise
		/// </summary>
		public double BuildingRise { get; set; }
		/// <summary>
		/// no of floors
		/// </summary>
		public int NoOfFloors { get; set; }
		/// <summary>
		/// Units
		/// </summary>
		public List<UnitData> Units { get; set; }
		/// <summary>
		/// ReadOnly property
		/// </summary>
		public bool ReadOnly { get; set; }
		/// <summary>
		/// ConflictAssignments
		/// </summary>
		public ConflictManagement ConflictAssignments { get; set; }
		/// <summary>
		/// EnrichedData
		/// </summary>
		public JObject EnrichedData { get; set; }
		/// <summary>
		/// Permission
		/// </summary>
		public List<string> Permissions { get; set; }
		/// <summary>
		/// VariableIds
		/// </summary>
		public List<string> VariableIds { get; set; }
		/// <summary>
		/// UHFExists
		/// </summary>
		public Boolean UHFExists { get; set; }
		/// <summary>
		/// SaveOpeningLocations
		/// </summary>
		public Boolean SaveOpeningLocations { get; set; }
	}

	/// <summary>
	/// UnitData
	/// </summary>
	public class UnitData
	{
		public int UnitId { get; set; }
		public string UnitName { get; set; }
		public TypicalFloor Travel { get; set; }
		public Boolean OcuppiedSpace { get; set; }
		public int NoOfFloors { get; set; }
		public int FrontOpening { get; set; }
		public int RearOpening { get; set; }
		public int SideOpening { get; set; }
		public OpeningDoors OpeningDoors { get; set; }
		public List<OpeningsAssigned> OpeningsAssigned { get; set; }

	}

	/// <summary>
	/// OpeningDoors
	/// </summary>
	public class OpeningDoors
	{
		public Boolean Front { get; set; }

		public Boolean Rear { get; set; }

		public Boolean Side { get; set; }
	}

	/// <summary>
	/// OpeningsAssigned
	/// </summary>
	public class OpeningsAssigned
	{
		/// <summary>
		/// FloorNumber
		/// </summary>
		public int FloorNumber { get; set; }
		/// <summary>
		/// FloorDesignation
		/// </summary>
		public string FloorDesignation { get; set; }
		/// <summary>
		/// Elevation
		/// </summary>
		public TypicalFloor Elevation { get; set; }
		/// <summary>
		/// Front
		/// </summary>
		public Boolean Front { get; set; }
		/// <summary>
		/// Rear
		/// </summary>
		public Boolean Rear { get; set; }
		/// <summary>
		/// Side
		/// </summary>
		public Boolean Side { get; set; }
		/// <summary>
		/// MainEgress
		/// </summary>
		public bool MainEgress { get; set; }
		/// <summary>
		/// AlternateEgress
		/// </summary>
		public bool AlternateEgress { get; set; }
	}
}