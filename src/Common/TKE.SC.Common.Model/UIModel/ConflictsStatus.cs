using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ConflictsStatus
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
	public enum ConflictsStatus
	{
		Valid,
		InValid,
		NeedValidation
	}
}