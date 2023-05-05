/************************************************************************************************************
    File Name     :   ProductDL
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using System.Collections.Generic;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using System.Data;
using System.Data.SqlClient;
using TKE.SC.Common.Model;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class ProductDL : IProductDL
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param Name="logger"></param>
        public ProductDL(ILogger<ProductDL> logger)
        {
            Utility.SetLogger(logger);
        }
        /// <summary>
        /// this method used to get the product details
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseMessage> GetListOfProductLine()
        {
            var methodBeginTime = Utility.LogBegin();
            ProductLineList lstPL = new ProductLineList();
            lstPL.productLineList = new List<ProductLine>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPRODUCTLINEDETAILS);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                DataTable dataTable = dataSet.Tables[0];
                lstPL.productLineList = CpqDatabaseManager.ConvertDataTable<ProductLine>(dataTable);
            }             
            var response = JObject.FromObject(lstPL);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }
    }
}
