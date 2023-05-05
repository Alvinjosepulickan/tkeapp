/************************************************************************************************************
************************************************************************************************************
    File Name     :   IAuth.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IAuth
    {

        /// <summary>
        /// Interface for GetUserInfo
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetUserDetails(User userDetailsModel, string sessionId);

    }
}
