/************************************************************************************************************
************************************************************************************************************
    File Name     :   GroupConfiguration.cs
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using System;
using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
	/// <summary>
	/// GroupConfiguration
	/// </summary>
	public class GroupConfiguration
	{
		/// <summary>
		/// Id
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// BuildingId
		/// </summary>
		public string BuildingId { get; set; }
		/// <summary>
		/// GroupName
		/// </summary>
		public string GroupName { get; set; }
		/// <summary>
		/// ProductCategory
		/// </summary>
		public string ProductCategory { get; set; }
		/// <summary>
		/// ControlLocation
		/// </summary>
		public string controlLocation { get; set; }
		/// <summary>
		/// FixtureStrategy
		/// </summary>
		public string FixtureStrategy { get; set; }
		/// <summary>
		/// CreatedBy
		/// </summary>
		public User CreatedBy { get; set; }
		/// <summary>
		/// CreatedOn
		/// </summary>
		public DateTime CreatedOn { get; set; }
		/// <summary>
		/// ModifiedBy
		/// </summary>
		public User ModifiedBy { get; set; }
		/// <summary>
		/// ModifiedOn
		/// </summary>
		public DateTime ModifiedOn { get; set; }
		/// <summary>
		/// FloorPlan
		/// </summary>
		public List<FloorPlan> FloorPlan { get; set; }
	}

	public class ResultGroupConfiguration
	{
		/// <summary>
		/// Result
		/// </summary>
		public int Result { get; set; }
		/// <summary>
		/// GroupConfigurationId
		/// </summary>
		public int GroupConfigurationId { get; set; }
		/// <summary>
		/// GroupConfigurationId
		/// </summary>
		public int FieldDrawingId { get; set; }
		/// <summary>
		/// Message
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// IsDuplicateNameError
		/// </summary>
		public Boolean IsDuplicateNameError { get; set; }
		/// <summary>
		/// CarPositionsWithDuplicateNames
		/// </summary>
		public List<string> CarPositionsWithDuplicateNames { get; set; }
		/// <summary>
		/// Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// SetId
		/// </summary>
		public int SetId { get; set; }

	}

	/// <summary>
	/// GroupResult
	/// </summary>
	public class GroupResult
	{
		/// <summary>
		/// Message
		/// </summary>
		public string Message { get; set; }
	}

	/// <summary>
	/// unitdetails
	/// </summary>
	public class unitdetails
	{
		/// <summary>
		/// UnitId
		/// </summary>
		public int UnitId { get; set; }
		/// <summary>
		/// UnitName
		/// </summary>
		public string UnitName { get; set; }
		/// <summary>
		/// SideDoor
		/// </summary>
		public bool SideDoor { get; set; }
		/// <summary>
		/// FutureTravel
		/// </summary>
		public bool FutureTravel { get; set; }
		/// <summary>
		/// OccupiedSpaceBelow
		/// </summary>
		public bool OccupiedSpaceBelow { get; set; }
		/// <summary>
		/// OpeningsAssigned
		/// </summary>
		public List<OpeningDetails> OpeningsAssigned { get; set; }
	}

	/// <summary>
	/// OpeningDetails
	/// </summary>
	public class OpeningDetails
	{
		/// <summary>
		/// FloorNumber
		/// </summary>
		public string FloorNumber { get; set; }
		/// <summary>
		/// FloorDesignation
		/// </summary>
		public string FloorDesignation { get; set; }
		/// <summary>
		/// Front
		/// </summary>
		public bool Front { get; set; }
		/// <summary>
		/// Rear
		/// </summary>
		public bool Rear { get; set; }
		/// <summary>
		/// Side
		/// </summary>
		public bool Side { get; set; }
	}

	/// <summary>
	/// ResultProjectSave
	/// </summary>
	public class ResultProjectSave
	{
		/// <summary>
		/// Result
		/// </summary>
		public int Result { get; set; }
		/// <summary>
		/// Message
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// OpportunityId
		/// </summary>
		public string OpportunityId { get; set; }
		/// <summary>
		/// VersionId
		/// </summary>
		public int VersionId { get; set; }
		/// <summary>
		/// QuoteId
		/// </summary>
		public string QuoteId { get; set; }
		/// <summary>
		/// QuoteStatus 
		/// </summary>
		public string QuoteStatus { get; set; }
	}

	/// <summary>
	/// ResultProjectDelete
	/// </summary>
	public class ResultProjectDelete
	{
		/// <summary>
		/// Result
		/// </summary>
		public int Result { get; set; }
		/// <summary>
		/// Message
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// OpportunityId
		/// </summary>
		public string OpportunityId { get; set; }
		/// <summary>
		/// VersionId
		/// </summary>
		public List<int> VersionId { get; set; }
		/// <summary>
		/// QuoteId
		/// </summary>
		public List<string> QuoteId { get; set; }
	}
}
