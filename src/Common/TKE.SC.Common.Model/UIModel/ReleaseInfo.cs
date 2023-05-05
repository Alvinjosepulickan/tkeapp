using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// GroupDetailsForReleaseInfo
    /// </summary>
    public class GroupDetailsForRelease
    {
        public List<ReleaseInfoBuildingDetails> GroupDetailsForReleaseInfo { get; set; }
    }

    /// <summary>
    /// ReleaseInfoBuildingDetails
    /// </summary>
    public class ReleaseInfoBuildingDetails
    {
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public List<ReleaseInfoGroupDetails> GroupDetails { get; set; }
    }

    /// <summary>
    /// ReleaseInfoGroupDetails
    /// </summary>
    public class ReleaseInfoGroupDetails
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public Nullable<bool> ReleaseToManufacturing { get; set; }
        public Status GroupStatus { get; set; }
        public int UnitLength { get; set; }
        public String ProductCategory { get; set; }
        public List<string> Permissions { get; set; }
    }

    /// <summary>
    /// DetailsForReleaseToManufacture
    /// </summary>
    public class DetailsForReleaseToManufacture
    {
        public GroupDetailsReleaseToManufacture GroupReleaseToManufacture { get; set; }
        /// <summary>
        /// EnrichedData
        /// </summary>
        public JObject EnrichedData { get; set; }
        public List<string> Permissions { get; set; }
    }

    /// <summary>
    /// GroupDetailsReleaseToManufacture
    /// </summary>
    public class GroupDetailsReleaseToManufacture
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<ReleaseInfoQuestions> ReleaseQueries { get; set; }
        public List<ReleaseInfoSetUnitDetails> UnitDetails { get; set; }
        public string Action { get; set; }
        public bool ReadyToReleaseCheck { get; set; }
    }

    /// <summary>
    /// ReleaseInfoQuestions
    /// </summary>
    public class ReleaseInfoQuestions
    {
        public string ReleaseQueId { get; set; }
        public string ReleaseQueDesc { get; set; }
        public bool ReleaseQueCheck { get; set; }
    }

    /// <summary>
    /// ReleaseInfoSetUnitDetails
    /// </summary>
    public class ReleaseInfoSetUnitDetails
    {
        public int Id { get; set; }
        public string UnitName { get; set; }
        public int SetId { get; set; }
        public string ReleaseComments { get; set; }
        public string FactoryJobId { get; set; }
        public List<ReleaseInfoDataPoints> DataPointDetails { get; set; }
    }

    /// <summary>
    /// ReleaseInfoDataPoints
    /// </summary>
    public class ReleaseInfoDataPoints
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public Nullable<bool> IsAcknowledged { get; set; }
    }

    /// <summary>
    /// ResultGroupReleaseResponse
    /// </summary>
    public class ResultGroupReleaseResponse
    {
        public int Result { get; set; }
        public int GroupId { get; set; }
        public string Message { get; set; }
    }
}