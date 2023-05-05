using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// GroupConflictsStatus
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupConflictsStatus
    {
        GRP_VAL,
        GRP_INV,
        GRP_NV,
        Valid,
        InValid,
        NeedValidation
    }
}