using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// latestDrawingStatus
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum latestDrawingStatus
    {
        NotSubmitted
           , Submitted
           , Pending
           , Completed
           , Error
    }
}