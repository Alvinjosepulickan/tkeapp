using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ViewExportDetails
    /// </summary>
    public class ViewExportDetails
    {
        /// <summary>
        /// code
        /// </summary>
        //public string Code { get; set; }
        /// <summary>
        /// data
        /// </summary>
        public QuotationDetails Quotation { get; set; }
        public List<Identifications> Units { get; set; }
    }

    /// <summary>
    /// QuotationDetails
    /// </summary>
    public class QuotationDetails
    {
        public OpportunityValues OpportunityInfo { get; set; }
        public Quote Quote { get; set; }
        public List<JObject> UnitMaterials { get; set; }
    }

    /// <summary>
    /// OpportunityValues
    /// </summary>
    public class OpportunityValues
    {
        public DateFormatClass ValidityDate { get; set; }
        public Boolean QuickQuote { get; set; }
        public string FactoryQuoteCurrency { get; set; }
        public string BaseBidCreator { get; set; }
        public string OpportunityURL { get; set; }
        public string OpportunityId { get; set; }
    }

    /// <summary>
    /// Quote
    /// </summary>
    public class Quote
    {
        public string QuoteNumber { get; set; }
        public Boolean BaseBid { get; set; }
        //public Boolean TP3Update { get; set; }
        public DateFormatClass QuoteCreatedDate { get; set; }
        public DateFormatClass QuoteLastModifiedDate { get; set; }
        public string VIEW_Version { get; set; }
        public string QuoteStatus { get; set; }

    }

    /// <summary>
    /// UnitMaterials
    /// </summary>
    public class UnitMaterials
    {
        public int BuildingID { get; set; }
        public string BuildingLabel { get; set; }
        public string Mainlinevoltage { get; set; }
        public string Bacnet { get; set; }
        public string RoboticControllerInterface { get; set; }
        public string IntergroupEmergencyPower { get; set; }
        public List<GroupConfigrtn> Groups { get; set; }
    }

    /// <summary>
    /// GroupConfigrtn
    /// </summary>
    public class GroupConfigrtn
    {
    }

    /// <summary>
    /// DateFormatClass
    /// </summary>
    public class DateFormatClass
    {
        [DefaultValue("MM/dd/yyyy HH:mm:ss")]
        public string DateFormat { get; set; }
        public string DateValue { get; set; }
    }

    /// <summary>
    /// ViewVariableAndDataType
    /// </summary>
    public class ViewVariableAndDataType
    {
        public string value { get; set; }
        public string type { get; set; }
    }

    /// <summary>
    /// Identifications
    /// </summary>
    public class Identifications
    {
        public Unitsection Identification { get; set; }
    }

    /// <summary>
    /// Unitsection
    /// </summary>
    public class Unitsection
    {
        public string UEID { get; set; }
    }
}
