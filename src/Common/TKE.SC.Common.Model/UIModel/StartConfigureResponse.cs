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
    /// StartConfigureResponse
    /// </summary>
    public class StartConfigureResponse : ConfigureResponse
    {
        /// <summary>
        /// Audits
        /// </summary>
        public ConfigurationTimeStamp Audits { get; set; }
        /// <summary>
        /// ReadOnly
        /// </summary>
        public bool ReadOnly { get; set; }
    }
}