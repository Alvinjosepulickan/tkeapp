using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
	/// <summary>
	/// UnitSummaryUIModel
	/// </summary>
	public class UnitSummaryUIModel : ConfigurationResponse
	{
		/// <summary>
		/// Sectiopns
		/// </summary>
		public List<PriceSectionDetails> PriceSections { get; set; }
		/// <summary>
		/// PriceValues
		/// </summary>
		public Dictionary<string, UnitPriceValues> PriceValue { get; set; }
		/// <summary>
		/// UserVariables
		/// </summary>
		public Dictionary<string, string> UserVariables { get; set; }
		/// <summary>
		/// variables
		/// </summary>
		public List<Variables> Variables { get; set; }
		/// <summary>
		/// ManufacturingCommentsHistory
		/// </summary>
		public List<LogParameters> ManufacturingCommentsHistory { get; set; }
	}
}