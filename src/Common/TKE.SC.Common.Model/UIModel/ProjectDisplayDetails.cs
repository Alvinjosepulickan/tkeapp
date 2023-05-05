using System;
using Newtonsoft.Json;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ProjectDisplayDetails
    /// </summary>
    public class ProjectDisplayDetails
    {
        [JsonProperty("Projects.ProjectId")]
        public string ProjectId { get; set; }
        public DateTime ProposedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime ContractBookedDate { get; set; }
        public string QuoteId { get; set; }
        public string QuoteStatus { get; set; }
    }
}