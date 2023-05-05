/************************************************************************************************************
************************************************************************************************************
    File Name     :   ProductStubDL
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.BFF.Test.Common;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class ProductStubDL : IProductDL
    {
        /// <summary>
        /// this method is for setting stub data for the method GetListOfProductLine
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseMessage> GetListOfProductLine()
        {
            try
            {

                var request1 = AppGatewayJsonFilePath.PRODUCTOUTPUT;
                var getResponse = JObject.Parse(File.ReadAllText(request1));
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = getResponse };

            }
            catch (Exception ex)
            {
                return new ResponseMessage { StatusCode = Constant.BADREQUEST, Message = ex.Message };
            }
        }
    }
}
