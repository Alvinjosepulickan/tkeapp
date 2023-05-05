using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
	public class QuoteStatusReport
	{
		public string QuoteId { get; set; }
		public string QuoteStatus { get; set; }
		public bool QuoteIsDeleted { get; set; }
		public List<BuildingStatusReport> BuildingStatusReport { get; set; }
	}
	public class BuildingStatusReport
	{
		public string BuildingName { get; set; }
		public int BuildingId { get; set; }
		public string BuildingStatus { get; set; }
		public bool BuildingIsDeleted { get; set; }
		public List<GroupStatusReport> GroupStatusReport { get; set; }
	}
	public class GroupStatusReport
	{
		public int GroupId { get; set; }
		public string GroupName { get; set; }
		public bool GroupIsDeleted { get; set; }
		public string GroupStatus { get; set; }
		public List<UnitStatusReport> UnitStatusReport { get; set; }
	}
	public class UnitStatusReport
	{
		public int UnitId { get; set; }
		public string UnitName { get; set; }
		public string FactoryJobId { get; set; }
		public string UnitStatus { get; set; }
		public bool UnitIsDeleted { get; set; }
		public string SetId { get; set; }
		public string ProductName { get; set; }
		public string UEID { get; set; }
		public string UnitLocation { get; set; }
	}
}
