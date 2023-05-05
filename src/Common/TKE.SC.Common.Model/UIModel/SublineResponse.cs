/************************************************************************************************************
************************************************************************************************************
    File Name     :   model class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Configit.Configurator.Server.Common;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// SublineResponse
    /// </summary>
    public class SublineResponse : SublinesResponse
    {
        /// <summary>
        /// Parent Code
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        /// Configuration Time Stamp
        /// </summary>
        public ConfigurationTimeStamp ConfigurationTimeStamp { get; set; }
    }
}