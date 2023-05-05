/************************************************************************************************************
************************************************************************************************************
    File Name     :   BaseConfigureResponse
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
    /// BaseConfigureResponse
    /// </summary>
    public class BaseConfigureResponse : ConfigureResponse
    {
        /// <summary>
        /// TotPrice
        /// </summary>
        public double TotPrice { get; set; }
    }
}