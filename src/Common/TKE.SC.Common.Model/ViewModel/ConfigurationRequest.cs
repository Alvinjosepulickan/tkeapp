/************************************************************************************************************
************************************************************************************************************
    File Name     :   ConfigurationRequest
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Configit.Configurator.Server.Common;

namespace TKE.SC.Common.Model.ViewModel
{
    /// <summary>
    /// ConfigurationRequest
    /// </summary>
    public class ConfigurationRequest : ConfigureRequest
    {
        /// <summary>
        /// PackagePath
        /// </summary>
        public string PackagePath { get; set; }
    }
}
