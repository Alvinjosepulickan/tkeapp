using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model.UIModel
{
	/// <summary>
	/// GroupConfigurationSave
	/// </summary>
	public class GroupConfigurationSave
	{
		public Operation Operation { get; set; }
		public List<int> GroupIDs { get; set; }
		public int buildingID { get; set; }
		public JArray VariableAssignments { get; set; }
	}
}