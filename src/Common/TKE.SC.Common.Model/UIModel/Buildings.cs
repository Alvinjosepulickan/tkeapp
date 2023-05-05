/************************************************************************************************************
************************************************************************************************************
    File Name     :   Building.cs
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// Result
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Property-Result
        /// </summary>
        public int result { get; set; }
        /// <summary>
        /// Property- Message
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Property-BuildingId
        /// </summary>
        public int buildingId { get; set; }
    }

    /// <summary>
    /// ResultElevation
    /// </summary>
    public class ResultElevation
    {
        /// <summary>
        /// Property- Result
        /// </summary>
        public int result { get; set; }
    }

    /// <summary>
    /// BuildingElevationData
    /// </summary>
    public class BuildingElevationData
    {
        /// <summary>
        /// Property- FloorDesignation
        /// </summary>
        public string floorDesignation { get; set; }
        /// <summary>
        /// Property-FloorNumber
        /// </summary>
        public int FloorNumber { get; set; }
        /// <summary>
        /// Property- mainEgress
        /// </summary>
        public Boolean mainEgress { get; set; }
        /// <summary>
        /// Property- alternateEgress
        /// </summary>
        public Boolean alternateEgress { get; set; }
        /// <summary>
        /// Property- elevation
        /// </summary>
        public TypicalFloor elevation { get; set; }
        /// <summary>
        /// Property- floorToFloorHeight
        /// </summary>
        public TypicalFloor floorToFloorHeight { get; set; }
    }

    /// <summary>
    /// BuildingElevation
    /// </summary>
    public class BuildingElevation
    {
        /// <summary>
        /// Property- buildingConfigurationId
        /// </summary>
        public int buildingConfigurationId { get; set; }
        /// <summary>
        /// Property- buildingRise
        /// </summary>
        public decimal buildingRiseValue { get; set; }
        /// <summary>
        /// Property- numberOfFloor
        /// </summary>
        public int noOFFloor { get; set; }
        /// <summary>
        /// Property- CreatedBy
        /// </summary>
        public User createdBy { get; set; }
        /// <summary>
        /// Property- ModifiedBy
        /// </summary>
        public User modifiedBy { get; set; }
        /// <summary>
        /// Property- buildingElevation
        /// </summary>
        public List<BuildingElevationData> buildingElevation { get; set; }
        /// <summary>
        /// List of permission
        /// </summary>
        public List<string> Permissions { get; set; }
        /// <summary>
        /// ConflictAssignments
        /// </summary>
        public ConflictManagement ConflictAssignments { get; set; }
        /// <summary>
        /// EnrichedData
        /// </summary> 
        public JObject EnrichedData { get; set; }
        /// <summary>
        /// AvgRoofHeight
        /// </summary> 
        public decimal AvgRoofHeight { get; set; }
        /// <summary>
        /// IsEditFlag
        /// </summary>
        public bool IsEditFlag { get; set; }
        
    }
}