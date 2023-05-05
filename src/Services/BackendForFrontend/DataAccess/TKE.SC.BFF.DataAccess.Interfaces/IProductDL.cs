/************************************************************************************************************
    File Name     :   IProductDL
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model;
using System.Threading.Tasks;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IProductDL
    {
        /// <summary>
        /// method to get list of product line
        /// </summary>
        /// <returns></returns>
        Task<ResponseMessage> GetListOfProductLine();
    }
}
