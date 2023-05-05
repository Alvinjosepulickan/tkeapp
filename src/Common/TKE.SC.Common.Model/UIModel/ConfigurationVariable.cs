/************************************************************************************************************
************************************************************************************************************
    File Name     :   ConfigVariable.cs
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ConfigVariable
    /// </summary>
    public class ConfigVariable
    {
        /// <summary>
        /// Property- VariableId
        /// </summary>
        public string VariableId { get; set; }
        /// <summary>
        /// Property- Value
        /// </summary>
        public object Value { get; set; }
    }
}
