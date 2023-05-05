using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKE.SC.DocModel.ApiContracts
{
    public class OrderFormModel:BaseModel
    {
            public IDictionary<string, object> Variables { get; set; }
            public IDictionary<string, string> Enrichment { get; set; }
            public IList<PriceSectionDetails> PriceSections { get; set; }
            public IDictionary<string, UnitPriceValues> PriceValue { get; set; }
    }

    public class PriceSectionDetails
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Section
        /// </summary>
        public string Section { get; set; }
        /// <summary>
        /// PriceKeyInfo
        /// </summary>
        public List<PriceValuesDetails> PriceKeyInfo { get; set; }
    }

    public class UnitPriceValues
    {
        /// <summary>
        /// Unit
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// quantity
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// unitPrice
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// totalPrice
        /// </summary>
        public decimal TotalPrice { get; set; }
    }

    public class PriceValuesDetails
    {
        /// <summary>
        /// Section
        /// </summary>
        public string Section { get; set; }
        /// <summary>
        /// ItemNumber
        /// </summary>
        public string ItemNumber { get; set; }
        /// <summary>
        /// SectionName
        /// </summary>
        [JsonProperty("Section Name")]
        public string SectionName { get; set; }
        /// <summary>
        /// Component
        /// </summary>
        [JsonProperty("Component Name")]
        public string Component { get; set; }
        /// <summary>
        /// PartDescription
        /// </summary>
        public string PartDescription { get; set; }
        /// <summary>
        /// ComponentName
        /// </summary>
        public string ComponentName { get; set; }
        /// <summary>
        /// LeadTime
        /// </summary>
        public string LeadTime { get; set; }
        /// <summary>
        /// BatchNo
        /// </summary>
        public string BatchNo { get; set; }
        public UnitPriceValues UnitPriceValues { get; set; }
        public string Parameter3 { get; set; }
        public string Parameter3Value { get; set; }
        public string Parameter4 { get; set; }
        public string Parameter4Value { get; set; }
    }
}