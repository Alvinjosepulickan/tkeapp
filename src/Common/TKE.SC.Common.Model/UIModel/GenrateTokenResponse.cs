/************************************************************************************************************
************************************************************************************************************
    File Name     :   GenrateTokenResponse.cs
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// GenrateTokenResponse
    /// </summary>
    public class GenrateTokenResponse
    {
        /// <summary>
        /// Token
        /// </summary>
        public JObject Token { get; set; }
        /// <summary>
        /// User Info
        /// </summary>
        public UserInfo UserInfo { get; set; }
        /// <summary>
        /// SessionId
        /// </summary>
        public string SessionId { get; set; }
        /// <summary>
        /// Persona
        /// </summary>
        public string Persona { get; set; }
        /// <summary>
        /// IsProChk
        /// </summary>
        public bool IsProdChk { get; set; }
    }
}