using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.Common.Model;

namespace TKE.SC.Common.Model.UIModel
{
	public class CustomPriceLine
	{
		public Dictionary<string, UnitPriceValues> PriceValue { get; set; }
		public PriceValuesDetails priceKeyInfo { get; set; }
		public string UserId { get; set; }
	}
}
