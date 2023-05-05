using System;
using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ViewProjectDetails
    /// </summary>
    public class ViewProjectDetails
    {
        /// <summary>
        /// code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// data
        /// </summary>
        public DataDetails Data { get; set; }
        /// <summary>
        /// retMsg
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// ProjectInfoDetails
        /// </summary>
        public OpportunityEntity ProjectInfoDetails { get; set; }
        /// <summary>
        /// Unit Details
        /// </summary>
        public List<FactoryDetails> FactoryDetails { get; set; }
    }

    /// <summary>
    /// DataDetails
    /// </summary>
    public class DataDetails
    {
        public Quotation Quotation { get; set; }
        public string VersionId { get; set; }
        public List<UnitsData> UnitsData { get; set; }
    }

    /// <summary>
    /// Quotation
    /// </summary>
    public class Quotation
    {
        public OpportunityInfo OpportunityInfo { get; set; }
        public UserAddressDetailsDataValues Architect { get; set; }
        public Contact Contact { get; set; }
        public UserAddressDetailsDataValues GC { get; set; }
        public UserAddressDetailsDataValues Owner { get; set; }
        public UserAddressDetailsDataValues Billing { get; set; }
        public UserAddressDetailsDataValues Building { get; set; }
        public QuoteInfo Quote { get; set; }
    }

    /// <summary>
    /// OpportunityInfo
    /// </summary>
    public class OpportunityInfo
    {
        public string OpportunityId { get; set; }
        public string OpportunityURL { get; set; }
        public string BusinessLine { get; set; }
        public string JobName { get; set; }
        public string Branch { get; set; }
        public string SalesmanActiveDirectoryID { get; set; }
        public string Salesman { get; set; }
        public string Category { get; set; }
        public AwardClosedDate AwardClosedDate { get; set; }
        public ProposedDate ProposedDate { get; set; }
        public ContractBookedDate ContractBookedDate { get; set; }
        public string SalesStage { get; set; }
        public string OraclePSNumber { get; set; }
        public string Superintendent { get; set; }
        public string SalesEmail { get; set; }
    }

    /// <summary>
    /// AwardClosedDate
    /// </summary>
    public class AwardClosedDate
    {
        public string DateFormat { get; set; }
        public DateTime? DateValue { get; set; }
    }

    /// <summary>
    /// /ProposedDate
    /// </summary>
    public class ProposedDate
    {
        public string DateFormat { get; set; }
        public DateTime? DateValue { get; set; }
    }

    /// <summary>
    /// ContractBookedDate
    /// </summary>
    public class ContractBookedDate
    {
        public string DateFormat { get; set; }
        public DateTime? DateValue { get; set; }
    }

    /// <summary>
    /// Contact
    /// </summary>
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
    }

    /// <summary>
    /// UnitsData
    /// </summary>
    public class UnitsData
    {
        public BuildingInfo BuildingInfo { get; set; }
    }
    public class FactoryDetails
    {
        public string FacotryJobId { get; set; }
        public string UEID { get; set; }
    }

    /// <summary>
    /// BuildingInfo
    /// </summary>
    public class BuildingInfo
    {
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string ZipCode_bldg { get; set; }
        public string AddressLine1_bldg { get; set; }
        public string AddressLine2_bldg { get; set; }
    }

    /// <summary>
    /// QuoteInfo
    /// </summary>
    public class QuoteInfo
    {
        public string QuoteDescription { get; set; }
        public string BaseBid { get; set; }
        public string UIVersionId { get; set; }
        public bool LatestViewVersion { get; set; }
        public bool IsPrimary { get; set; }
    }

    /// <summary>
    /// Added for the accountDetails of the project.
    /// </summary>
    public class UserAddressDetailsDataValues
    {
        public string AccountName { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string CustomerNumber { get; set; }
        public string AddressLine2 { get; set; }
        public Nullable<DateTime> AwardCloseDate { get; set; }

    }
}