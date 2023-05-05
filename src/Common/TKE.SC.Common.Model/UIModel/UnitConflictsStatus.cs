using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// UnitConflictsStatus
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UnitConflictsStatus
    {
        UNIT_VAL,
        UNIT_INV,
        UNIT_NV,
        Valid,
        InValid,
        NeedValidation,
        UNIT_CNV
    }
}