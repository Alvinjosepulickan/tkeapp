/************************************************************************************************************
************************************************************************************************************
    File Name     :   Client.cs
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// Client
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// contactPerson
        /// </summary>
        public User contactPerson { get; set; }
    }
}
