using Configit.Configurator.Server.Common;
using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
	public class ConfigurationDetails
	{
		public string QuoteId { get; set; }
		public int BuildingId { get; set; }
		public int GroupId { get; set; }
		public int SetId { get; set; }
        public string UEID { get; set; }
    }
	public class ObomVariableAssignment
	{
		public string QuoteId { get; set; }
		public string OpportunityId { get; set; }
		public string VersionId { get; set; }
		public string Name { get; set; }
		public string AccountName { get; set; }
		public string Address { get; set; }
		public string CustomerNumber { get; set; }
		public string QuoteStatus { get; set; }
		public List<BuildingData> BuildingData { get; set; }
		public bool IsQuoteReleased { get; set; }
		public string ProjectStatus { get; set; }
		public string Branch { get; set; }

		public string Country { get; set; }
	}
	public class BuildingData
	{
		public int BuildingId { get; set; }
		public List<VariableAssignment> ConfigurationVariables { get; set; }
		public List<GroupData> GroupData { get; set; }
		public Dictionary<string, object> BuildingConfigurationVariables { get; set; }
		public int NumberOfLanding { get; set; }
	}
	public class GroupData
	{
		public int GroupId { get; set; }
		public bool IsNcp { get; set; }
		public List<VariableAssignment> VariableAssignment { get; set; }
		public List<UnitDataForObom> UnitDataForObom { get; set; }
		public List<SetData> SetData { get; set; }
		public List<VariableAssignment> FDAVariableAssignments { get; set; }
		public Dictionary<string,object> GroupConfigurationVariables { get; set; }
	}
	public class UnitDataForObom
	{
		public int UnitId { get; set; }
		public string UnitName { get; set; } 
		public string VariableType { get; set; }
		public string UEID { get; set; }
		public int SetId { get; set; }
		public string ElevatorName { get; set; }
		public string Location { get; set; }
		public int SpecMemoVersion { get; set; }
		public List<Characteristics> Characteristics { get; set; }
		public string MjobNum { get; set; }
		public string Name { get; set; }
		public List<OpeningsAssigned> OpeningLocation { get; set; }
	}
	public class SetData
	{
		public int SetId { get; set; }
		public List<VariableAssignment> VariableAssignment { get; set; }
		public string ProductSelected{ get; set; }
		public List<VariableAssignment> SystemValidationVariables { get; set; }
		public List<string> SelectedUnits { get; set; }
		public Dictionary<string,object> SetConfigurationVariables { get; set; }
	}

}
