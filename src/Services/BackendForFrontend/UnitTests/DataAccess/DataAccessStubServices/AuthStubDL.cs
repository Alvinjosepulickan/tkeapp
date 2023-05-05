/************************************************************************************************************
************************************************************************************************************
    File Name     :   AuthStubDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class AuthStubDL : IAuthDL
    {
        /// <summary>
        /// To Validate User by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ValidateUser(int userId)
        {
            var response = JObject.Parse(File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.VALIDATEUSERVALUES)));
            var resultObject = response["Users"].Values<JObject>()
            .Where(n => n["id"].Value<int>() == userId);

            if (resultObject != null && resultObject.Count() > 0)
            {
                foreach (JObject j in resultObject)
                {
                    response = (j);
                }
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
            }
            else
            {
                response = null;
                return new ResponseMessage { StatusCode = Constant.UNAUTHORIZED, Response = response };
            }


        }

        /// <summary>
        /// To Get User Information by userId 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetUserInfo(int userId)
        {
            var getResponse = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH,Constant.USERDETAILS));
            if (userId == 0)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Response = JObject.Parse(getResponse)
                };
            }
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = JObject.Parse(getResponse)
            };

        }

        async Task<ResponseMessage> IAuthDL.GetUserDetails(User userDetailsModel)
        {
            var getResponse = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.USERDETAILS));
            if (userDetailsModel == null)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Response = JObject.Parse(getResponse)
                };
            }
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = JObject.Parse(getResponse),
                Message = "UserDataAvailable"
            };
        }
    }
}
