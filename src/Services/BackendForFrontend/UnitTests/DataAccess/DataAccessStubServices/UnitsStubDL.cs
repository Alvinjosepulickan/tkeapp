/************************************************************************************************************
************************************************************************************************************
    File Name     :   UnitsStubDL
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class UnitsStubDL : IUnitsDL
    {
        /// <summary>
        /// this method is for setting stub data for the method GetProjectBasedUnits1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Units> GetProjectBasedUnits1(string id)
        {
            return null;
        }
        /// <summary>
        /// this method is for setting stub data for the method GetListOfUnitsForProject
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetListOfUnitsForProject(string id)
        {
            return null;
        }
        /// <summary>
        /// this method is for setting stub data for the method GetUnitConfigurationDetailsDL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetUnitConfigurationDetailsDL(string id)
        {
            return null;

        }
    }

    public interface IUnitsDL
    {
    }
}
