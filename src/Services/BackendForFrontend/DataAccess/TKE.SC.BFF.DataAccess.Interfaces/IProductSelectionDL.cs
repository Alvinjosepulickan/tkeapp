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

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IProductSelectionDL
    {
        int SaveProductSelection(int groupConfigurationId, ProductSelection productSelection,string businessLine,string country,string controlLanding, string fixtureStrategy, string supplyingFactory);

        int UnitSetValidation(List<int> unitId);
        List<ConfigVariable> GetUnitVariableAssignments(List<int> unitId, string identifier);
    }
}
