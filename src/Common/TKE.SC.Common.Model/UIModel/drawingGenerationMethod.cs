using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// drawingGenerationMethod
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum drawingGenerationMethod
    {
        Automated
      , ManualCSC
    }
}