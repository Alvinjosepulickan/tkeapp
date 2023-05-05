using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
	/// <summary>
	/// GroupLayout
	/// </summary>
	public class GroupLayout
	{
		/// <summary>
		/// ConfigVariable
		/// </summary>
		public List<ConfigVariable> ConfigVariable { get; set; }
		/// <summary>
		/// DisplayVariableAssignmentsValues
		/// </summary>
		public List<DisplayVariableAssignmentsValues> DisplayVariableAssignmentsValues { get; set; }
		/// <summary>
		/// UpdatedTotalNumberOfFloors
		/// </summary>
		public int UpdatedTotalNumberOfFloors { get; set; }
		/// <summary>
		/// VariableIds
		/// </summary>
		public List<string> VariableIds { get; set; }
	}
}