using Configit.Configurator.Server.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
	public class OBOMCrossPackageVariables
	{
		public List<VariableAssignment> BuildingVariables { get; set; }

		public Object BuildingElevation { get; set; }

		public List<VariableAssignment> GroupConfiguration { get; set; }

		public List<VariableAssignment> GroupLayout { get; set; }

		public DataTable OpeningLocation { get; set; }

		public List<UnitVariableAssignments> UnitConfiguration { get; set; }

		public List<UnitDetailsForOBOM> UnitDetails { get; set; }

		public Dictionary<string, ElevatorCarPositionandSetId> UnitDictionary { get; set; }
		public bool QuoteFactoryReleased { get; set; }
	}
	public class UnitDetailsForOBOM
	{
		public int Unitid { get; set; }

		public string UnitName { get; set; }

		public string VariableType { get; set; }

		public string UEID { get; set; }

		public int SetId { get; set; }

		public string ElevatorName { get; set; }
		public string Location { get; set; }
		public int SpecMemoVersion { get; set; }
		public string QuoteId { get; set; }
		public int GroupId { get; set; }
		public string  OpportunityId { get; set; }
		public string VersionId { get; set; }
		public List<Characteristics> Characteristics { get; set; }
		public string ElevatorLocation { get; set; }
	}

	public class UnitVariableAssignments
	{
		public int SetId { get; set; }

		public List<VariableAssignment> VariableAssignments { get; set; }

		public Dictionary<string, object> ConfigVariables { get; set; }
	}
	public class ElevatorCarPositionandSetId
	{
		public string ElevatorName { get; set; }
		public int SetId { get; set; }
	}
	public class ObomVariables
	{
		/// <summary>
		/// partnum varaiable
		/// </summary>
		public Object PartNumber { get; set; }
		/// <summary>
		/// Quantity
		/// </summary>
		public int Quantity { get; set; }
		/// <summary>
		/// Characteristics
		/// </summary>
		public Dictionary<string, Object> Characteristics { get; set; }

		public List<ObomVariables> Child { get; set; }

		public string VariableId { get; set; }

		public Dictionary<string, Object> XMLVariables { get; set; }

		public string Description { get; set; }
	}
	public class OBOMResponse
	{
		/// <summary>
		/// Section Values
		/// </summary>
		public List<SectionsValues> Sections { get; set; }
		//public List<SectionsValues> sections { get; set; }
	}
	public class SpecMemo
	{
		public string ClassName { get; set; }
		public string CharacName { get; set; }
		public Object Value { get; set; }
	}

	public class Characteristics
	{
		public string ClassName { get; set; }
		public string CharacName { get; set; }
		public string Value { get; set; }
	}

	public class OrderCharacteristics
	{
		public string ClassName { get; set; }
		public List<string> CharacName { get; set; }
	}
}
