/************************************************************************************************************
************************************************************************************************************
    File Name     :   ValueStateView  
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TKE.SC.Common.Model.ViewModel
{
    /// <summary>
    /// ValueStateView
    /// </summary>
    public class ValueStateView
    {
        /// <summary>
        /// Assigned
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueStateAssignedView? Assigned { get; set; }
    }
}