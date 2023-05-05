using Newtonsoft.Json;
using System;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// VariableDetails
    /// </summary>
    public class VariableDetails
    {
        /// <summary>
        /// ProjectId
        /// </summary>
        [JsonProperty("Projects.ProjectId")]
        public string ProjectId { get; set; }
        /// <summary>
        /// ProjectName
        /// </summary>
        [JsonProperty("Projects.ProjectName")]
        public string ProjectName { get; set; }
        [JsonProperty("Projects.Branch")]
        public string Branch { get; set; }
        [JsonProperty("Projects.SalesStage")]
        public string SalesStage { get; set; }
        [JsonProperty("LayoutDetails.Language")]
        public string Language { get; set; }
        [JsonProperty("LayoutDetails.MeasuringUnit")]
        public string MeasuringUnit { get; set; }
        [JsonProperty("AccountDetails.AccountName")]
        public string AccountName { get; set; }
        [JsonProperty("AccountDetails.Contact")]
        public string Contact { get; set; }
        [JsonProperty("AccountDetails.SiteAddress.AddressLine1")]
        public string AddressLine1 { get; set; }
        [JsonProperty("AccountDetails.SiteAddress.AddressLine2")]
        public string AddressLine2 { get; set; }
        [JsonProperty("AccountDetails.SiteAddress.City")]
        public string City { get; set; }
        [JsonProperty("AccountDetails.SiteAddress.State")]
        public string State { get; set; }
        [JsonProperty("AccountDetails.SiteAddress.Country")]
        public string Country { get; set; }
        [JsonProperty("AccountDetails.SiteAddress.ZipCode")]
        public string ZipCode { get; set; }
        [JsonProperty("Projects.VersionId")]
        public int VersionId { get; set; }
        [JsonProperty("Projects.AwardCloseDate")]
        public Nullable<DateTime> AwardCloseDate { get; set; }
        [JsonProperty("Projects.Description")]
        public string Description { get; set; }
        [JsonProperty("Projects.SalesEmail")]
        public string SalesRepEmail { get; set; }
        [JsonProperty("Projects.OperationContactEmail")]
        public string OperationContactEmail { get; set; }
    }
}
