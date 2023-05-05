/************************************************************************************************************
************************************************************************************************************
    File Name     :   ConfigureResponseView
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System.Collections.Generic;

namespace TKE.SC.Common.Model.ViewModel
{
    /// <summary>
    /// ConfigureResponseView
    /// </summary>
    public class ConfigureResponseView
    {
        public List<SectionView> Sections { get; set; }
        public string PackagePath { get; set; }
    }
}