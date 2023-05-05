/************************************************************************************************************
************************************************************************************************************
    File Name     :   IProductSelectionDL class 
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
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IProductSelection
    {
        Task<ResponseMessage> SaveProductSelection(int groupConfigurationId, ProductSelection productSelection, string sessionId);

        Task<ResponseMessage> UnitSetValidation(List<int> unitId);
        Task<ResponseMessage> GetUnitVariableAssignments(List<int> unitId,string sessionId,string identifier);
    }
}
