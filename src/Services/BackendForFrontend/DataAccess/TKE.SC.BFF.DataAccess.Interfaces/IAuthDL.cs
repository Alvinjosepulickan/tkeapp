/************************************************************************************************************
************************************************************************************************************
    File Name     :   IAuthDL class 
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
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
  public  interface IAuthDL
    {
        /// <summary>
        /// Interface to get user information using user
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetUserDetails(User userDetailsModel);
    }
}
