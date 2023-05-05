/************************************************************************************************************
************************************************************************************************************
    File Name     :   ConfigurationTimeStamp.cs
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ConfigurationTimeStamp
    /// </summary>
    public class ConfigurationTimeStamp
    {
        /// <summary>
        /// Create On
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// Modification
        /// </summary>
        public DateTime? ModifiedOn { get; set; }
    }
}