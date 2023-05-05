using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
	/// <summary>
	/// UnitDetail
	/// </summary>
	public class UnitDetail
	{
		public int UnitId { get; set; }
		public string UnitName { get; set; }
		public int NoOfFloors { get; set; }
		public bool OccupiedSpaceBelow { get; set; }
		public OpeningDoors openingDoors { get; set; }
		public List<Opening> openingsAssigned { get; set; }
	}

	/// <summary>
	/// Opening
	/// </summary>
	public class Opening
	{
		public int FloorNumber { get; set; }
		public string FloorDesignation { get; set; }
		public Compatible Front { get; set; }
		public Compatible Rear { get; set; }
	}

	/// <summary>
	/// Compatible
	/// </summary>
	public class Compatible
	{
		public bool InCompatible { get; set; }
		public bool Value { get; set; }
		public bool NotAvailable { get; set; }
	}
}
