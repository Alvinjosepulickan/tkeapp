using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ProjectInformation
    /// </summary>
    public class ProjectInformation
    {
        public string JobName { get; set; }
        public Branch Branch { get; set; }
        public JobSite JobSite { get; set; }
        public GeneralContractor GeneralContractor { get; set; }
        public List<SalesRepresentative> SalesRepresentative { get; set; }
        public List<OperationsContact> OperationsContact { get; set; }
        public ProjectIdentifier ProjectIdentifier { get; set; }
        public string LineOfBusiness { get; set; }
        public string BookingType { get; set; }
    }

    /// <summary>
    /// Branch
    /// </summary>
    public class Branch
    {
        public string Name { get; set; }
        public Identifier identifier { get; set; }

    }

    /// <summary>
    /// Identifier
    /// </summary>
    public class Identifier
    {
        public int OracleId { get; set; }
    }

    /// <summary>
    /// JobSite
    /// </summary>
    public class JobSite
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; } = string.Empty;
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
    }

    /// <summary>
    /// GeneralContractor
    /// </summary>
    public class GeneralContractor
    {
        public string Name { get; set; }
        public Address Address { get; set; }
    }

    /// <summary>
    /// Address
    /// </summary>
    public class Address
    {
        public ContactOZ Contact { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StateProvince { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string CountryRegion { get; set; } = string.Empty;
    }

    /// <summary>
    /// ContactOZ
    /// </summary>
    public class ContactOZ
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

    }

    /// <summary>
    /// SalesRepresentative
    /// </summary>
    public class SalesRepresentative
    {
        public Person Person { get; set; }
    }

    /// <summary>
    /// Person
    /// </summary>
    public class Person
    {
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// OperationsContact
    /// </summary>
    public class OperationsContact
    {
        public Person Person { get; set; }
    }

    /// <summary>
    /// ProjectIdentifier
    /// </summary>
    public class ProjectIdentifier
    {
        public string QuoteId { get; set; }
        public string TransactionId { get; set; }
        public string OpportunityId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectVersionId { get; set; }
    }
}