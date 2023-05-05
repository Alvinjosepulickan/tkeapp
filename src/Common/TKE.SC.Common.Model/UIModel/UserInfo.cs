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
using TKE.SC.Common.Model;


namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// UserInfo
    /// </summary>
    public class UserInfo : CustomerProfile
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }
    }
}