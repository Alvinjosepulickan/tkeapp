/************************************************************************************************************
************************************************************************************************************
    File Name     :   ProductBL.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TKE.SC.BFF.BusinessProcess.Helpers;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class ProductBL : IProduct
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables 
        /// <summary>
        /// object IProductDL
        /// </summary>
        private readonly IProductDL _productdl;
        #endregion

        /// <summary>
        /// ProductBL
        /// </summary>
        /// <param Name="productdl"></param>
        /// <param Name="logger"></param>
        public ProductBL(IProductDL productdl, ILogger<ProductBL> logger)
        {
            _productdl = productdl;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// Get list of Product Line method
        /// </summary>
        /// <returns></returns>
        public Task<ResponseMessage> GetListOfProductLine()
        {
            //Call to DB
            return _productdl.GetListOfProductLine();
        }
    }
}
