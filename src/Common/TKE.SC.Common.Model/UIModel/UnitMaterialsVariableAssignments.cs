using System.Collections.Generic;
using Configit.Configurator.Server.Common;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// BuildingVariableAssignment
    /// </summary>
    public class BuildingVariableAssignment
	{
		public int BuildingId { get; set; }
		public List<VariableAssignment> BuildingVariableAssignments { get; set; }
		public List<GroupVariableAssignment> GroupVariableAssignment { get; set; }
	}

	/// <summary>
	/// GroupVariableAssignment
	/// </summary>
	public class GroupVariableAssignment
	{
		public int GroupId { get; set; }
		public bool isNCP { get; set; }
		public List<VariableAssignment> GroupVariableAssignments { get; set; }
		public List<UnitVariableAssignment> UnitVariableAssignments { get; set; }
		public List<SetVariableAssignment> SetVariableAssignment { get; set; }
	}

	/// <summary>
	/// UnitVariableAssignment
	/// </summary>
	public class UnitVariableAssignment
	{
		public int UnitId { get; set; }
		//public VariableAssignment UnitVariableAssignments { get; set; }
		public int SetId { get; set; }
	}

	/// <summary>
	/// SetVariableAssignment
	/// </summary>
	public class SetVariableAssignment
	{
		public int SetId { get; set; }
		public bool RearDoorSelected { get; set; }
		public List<VariableAssignment> UnitVariableAssignments { get; set; }
		public List<VariableAssignment> ProductVariableAssignments { get; set; }
		public List<VariableAssignment> SytemValidationVariableAssignments { get; set; }
		public string ProductName { get; set; }
		public Dictionary<string, object> ProductTreeVariables { get; set; }
	}
}