/************************************************************************************************************
/************************************************************************************************************
    File Name     :   ConfigurationVariableView
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using System.Collections.Generic;

namespace TKE.SC.Common.Model.ViewModel
{
    /// <summary>
    /// ConfigurationVariableView
    /// </summary>
    public class ConfigurationVariableView
    {   /// <summary>
        /// Values
        /// </summary>
        public List<SingletonValueView> Values { get; set; }
    }
}