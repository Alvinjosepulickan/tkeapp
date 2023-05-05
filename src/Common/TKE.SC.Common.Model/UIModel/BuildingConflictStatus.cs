using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace TKE.SC.Common.Model.UIModel
{
	/// <summary>
	/// BuildingConflictStatus
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum BuildingConflictStatus
	{
		BLDG_VAL,
		BLDG_INV,
		BLDG_NV,
		Valid,
		InValid,
		NeedValidation
	}
}