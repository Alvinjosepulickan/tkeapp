using Newtonsoft.Json;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// PriceValuesDetails
    /// </summary>
    public class PriceValuesDetails
    {
        /// <summary>
        /// PriceKeyId
        /// </summary>
        public int PriceKeyId { get; set; }
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
        /// <summary>
        /// GroupMaterial
        /// </summary>
        public string GroupMaterial { get; set; }
        /// <summary>
        /// Parameter2
        /// </summary>
        public string Parameter2 { get; set; }
        /// <summary>
        /// Parameter2Value
        /// </summary>
        public string Parameter2Value { get; set; }
        /// <summary>
        /// Parameter3
        /// </summary>
        public string Parameter3 { get; set; }
        /// <summary>
        /// Parameter3Value
        /// </summary>
        public string Parameter3Value { get; set; }
        /// <summary>
        /// Parameter4
        /// </summary>
        public string Parameter4 { get; set; }
        /// <summary>
        /// Parameter4Value
        /// </summary>
        public string Parameter4Value { get; set; }
        /// <summary>
        /// qty
        /// </summary>
        public int qty { get; set; }
        /// <summary>
        /// CustomPriceKey
        /// </summary>
        public bool IsCustomPriceLine { get; set; }
        /// <summary>
        /// Numsusp
        /// </summary>
        public string Numsusp { get; set; }
    }

    /// <summary>
    /// UnitPriceValues
    /// </summary>
    public class UnitPriceValues
    {
        /// <summary>
        /// Unit
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// quantity
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// unitPrice
        /// </summary>
        public decimal unitPrice { get; set; }
        /// <summary>
        /// totalPrice
        /// </summary>
        public decimal totalPrice { get; set; }
    }
}
