using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
	public class Configuration
	{
		public List<Buildings> Buildings { get; set; }
		public List<String> Permissions { get; set; }
	}
	public class ProjectDetail
	{
		public string Id { get; set; }
		public string VersionId { get; set; }
		public string OpportunityName { get; set; }
		public string SalesStage { get; set; }
		public AccountEntity AccountAddress { get; set; }
		public DateTime? ProposedDate { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? BookingDate { get; set; }
	}
	public class Buildings
	{
		public int Id { get; set; }
		public string BuildingName { get; set; }
		public string BuildingEquipmentStatus { get; set; }
		public Status BuildingStatus { get; set; }
		public List<Groups> Groups { get; set; }
		public List<string> Permissions { get; set; }
	}
	public class Groups
	{
		public int GroupId { get; set; }
		public string GroupName { get; set; }
		public Status GroupStatus { get; set; }
		public List<UnitsForConfiguration> Units { get; set; }
		public List<string> Permission { get; set; }
	}
	public class UnitsForConfiguration
	{
		public int UnitId { get; set; }
		public string UnitName { get; set; }
		public string Product { get; set; }
		public string Description { get; set; }
		public string Capacity { get; set; }
		public string Speed { get; set; }
		public int Landings { get; set; }
		public int FrontOpenings { get; set; }
		public int RearOpening { get; set; }
		public decimal Price { get; set; }
		public Status Status { get; set; }
		public int SetId { get; set; }
		public string SetName { get; set; }
		public string Ueid { get; set; }
		public Factory Factory { get; set; }
		public DateTime CreatedOn { get; set; }
		public List<string> Permissions { get; set; }
	}

}
