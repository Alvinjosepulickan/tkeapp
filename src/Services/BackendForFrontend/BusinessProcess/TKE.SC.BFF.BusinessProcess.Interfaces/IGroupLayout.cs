/************************************************************************************************************
************************************************************************************************************
    File Name     :   IGroupLayout.cs 
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
using TKE.SC.Common.Model.ViewModel;
using Newtonsoft.Json.Linq;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IGroupLayout
    {
        /// <summary>
        /// Interface for Save Group Layout method in the Business Layer
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveGroupLayout(int groupId, string sessionId, JObject variableAssignments);

        // <summary>
        /// DuplicateGroupLayout
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        Task<ResponseMessage> DuplicateGroupLayoutById(List<int> unitID, int groupID, List<CarPosition> CarPosition,Operation operation);

        /// <summary>
        /// Interface for Update Group Layout method in the Business Layer
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> UpdateGroupLayout(int groupId, string sectionTab, string sessionId, JObject variableAssignments);

    }
}
