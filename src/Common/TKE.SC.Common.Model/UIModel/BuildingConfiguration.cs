using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model.UIModel
{
	/// <summary>
	/// 
	/// </summary>
    public class BuildingConfiguration
	{
		/// <summary>
		/// Operation
		/// </summary>
		public Operation Operation { get; set; }
		/// <summary>
		/// BuildingIDs
		/// </summary>
		public List<int> BuildingIDs { get; set; }
		/// <summary>
		/// VariableAssignments
		/// </summary>
		public JArray VariableAssignments { get; set; }
        public string QuoteId { get; set; }
    }
	public enum Operation
	{
		Save ,
		Update,
		OverWrite,
		Duplicate
	}

}
