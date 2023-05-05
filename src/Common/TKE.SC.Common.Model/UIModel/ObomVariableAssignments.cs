using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
	public class ObomVariableAssignmentsTemp
	{
		public Dictionary<string, TempValues> GlobalParameters { get; set; }
		public List<List<Dictionary<string,object>>> FloorMatrixParameters { get; set; }
	}
	public class TempValues
	{
		public string Value { get; set; }
		public bool Included { get; set; }
	}
}
