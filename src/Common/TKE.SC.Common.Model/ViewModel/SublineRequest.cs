/************************************************************************************************************
************************************************************************************************************
    File Name     :   SublineRequest
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
    /// SublineRequest
    /// PackagePath
    /// </summary>
    public class SublineRequest : SublinesRequest
    {
        public string PackagePath { get; set; }
    }
}