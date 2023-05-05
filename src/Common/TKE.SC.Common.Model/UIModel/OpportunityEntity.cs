/************************************************************************************************************
************************************************************************************************************
   File Name     :   Opportunity.cs 
   Created By    :   Infosys LTD
   Created On    :   01-JAN-2020
   Modified By   :
   Modified On   :
   Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// OpportunityEntity
    /// </summary>
    public class OpportunityEntity
    {
        public string Id { get; set; }
        public string OpportunityName { get; set; }
        public string OpportunityType { get; set; }
        public string SalesStage { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public AccountEntity AccountAddress { get; set; }
        public DateTime? ProposedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string LineOfBusiness { get; set; }
        public string Region { get; set; }
        public string Branch { get; set; }
        public string MarketSegment { get; set; }
        public string EagleSalesStage { get; set; }
        public double ExpectedEagleRevenue { get; set; }
        public double Probability { get; set; }
        public string LeadSource { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFirstName { get; set; }
        public string Owner { get; set; }
        public string OwnerFullName { get; set; }
        public string OwnerId { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByFullName { get; set; }
        public string CreatedByAlias { get; set; }
        public string QuoteId { get; set; }
        public string QuoteStatus { get; set; }
        public string MFileFolderPath { get; set; }
        public string SalesRepEmail { get; set; }
        public string OperationContactEmail { get; set; }
    }
}