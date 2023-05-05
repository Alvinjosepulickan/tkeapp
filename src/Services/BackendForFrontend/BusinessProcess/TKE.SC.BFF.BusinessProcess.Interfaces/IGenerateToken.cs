/************************************************************************************************************
************************************************************************************************************
    File Name     :   IGenerateToken.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using UserInfoUI = TKE.SC.Common.Model.UIModel.UserInfo;
using TKE.SC.Common.Model;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IGenerateToken
    {
        /// <summary>
        /// GetAuthToken
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="distributedCache"></param>
        /// <param Name="userDetails"></param>

        /// <param Name="entitlements"></param>
        /// <returns></returns>
        string GetAuthToken(string sessionId, UserInfoUI userDetails,
            string tkeUuid, List<string> entitlements = null);

        /// <summary>
        /// ValidityToken
        /// </summary>
        /// <param Name="locale"></param>
        /// <param Name="distributedCache"></param>
        /// <param Name="uid"></param>
        /// <param Name="sw"></param>
        /// <param Name="sessionID"></param>
        /// <returns></returns>
        Task<ResponseMessage> ValidityToken(string locale,
            string uid = null, string sessionId = null, string persona = null, bool isProdchk = false);



        /// <summary>
        /// Extends session for logged in user
        /// </summary>
        /// <param Name="sessionId"></param>
        JObject ExtendSession(string sessionId);

        /// <summary>
        /// UI App Settings
        /// </summary>
        /// <returns></returns>
        List<IConfigurationSection> UIAppsettings();
    }
}
