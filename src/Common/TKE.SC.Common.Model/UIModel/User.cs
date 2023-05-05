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

using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Profile Pic
        /// </summary>
        public string ProfilePic { get; set; }
        /// <summary>
        /// Location
        /// </summary>
        public Location Location { get; set; }
        /// <summary>
        /// Role
        /// </summary>
        public Role Role { get;  set; }
        /// <summary>
        /// IsViewUser
        /// </summary>
        public bool IsViewUser { get; set; }
        /// <summary>
        /// Groups
        /// </summary>
        public IList<string> Groups { get; set; }
    }
}
