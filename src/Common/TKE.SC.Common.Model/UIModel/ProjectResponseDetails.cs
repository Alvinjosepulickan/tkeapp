using System;
using System.Collections.Generic;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ProjectResponseDetails
    /// </summary>
    public class ProjectResponseDetails
    {
        public string OpportunityId { get; set; }
        public string Name { get; set; }
        public string ViewUrl { get; set; }
        public string Branch { get; set; }
        public string SalesMan { get; set; }
        public string CscCoordinator { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Status SalesStage { get; set; }
        public int QuoteCount { get; set; }
        public List<string> Permissions { get; set; }
        public ProjectSource ProjectSource { get; set; }
        public List<QuoteDisplayDetails> Quotes { get; set; }
    }

    /// <summary>
    /// GetProject
    /// </summary>
    public class GetProject
    {
        public bool createProject { get; set; }
        public bool IsViewUser { get; set; }
        public List<ProjectResponseDetails> Projects { get; set; }
    }
}

/// <summary>
/// QuotePricingValues
/// </summary>
public class QuotePricingValues
{
    public int Total { get; set; }
    public string Unit { get; set; }
}

/// <summary>
/// QuoteDisplayDetails
/// </summary>
public class QuoteDisplayDetails
{
    /// <summary>
    /// QuoteId
    /// </summary>
    public string QuoteId { get; set; }
    /// <summary>
    /// VersionId
    /// </summary>
    public string VersionId { get; set; }
    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// UnitCount
    /// </summary>
    public int UnitCount { get; set; }
    /// <summary>
    /// QuoteStatus
    /// </summary>
    public Status QuoteStatus { get; set; }
    /// <summary>
    /// Pricing
    /// </summary>
    public QuotePricingValues Pricing { get; set; }
    /// <summary>
    /// CreatedDate
    /// </summary>
    public DateTime CreatedDate { get; set; }
    /// <summary>
    /// ModifiedDate
    /// </summary>
    public DateTime ModifiedDate { get; set; }
    /// <summary>
    /// Permissions
    /// </summary>
    public List<string> Permissions { get; set; }
    /// <summary>
    /// IsPrimaryQuote
    /// </summary>
    public bool IsPrimary { get; set; }
}

public class ProjectSource
{
    public int Id { get; set; }
    public string Source { get; set; }
}