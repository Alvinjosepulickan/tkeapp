using System.Collections.Generic;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// PriceSectionDetails
    /// </summary>
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
}
