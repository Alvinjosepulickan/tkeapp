/************************************************************************************************************
    File Name     :   IOpeningLocationDL
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
using Newtonsoft.Json.Linq;
using Configit.Configurator.Server.Common;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IOpeningLocationDL
    {
        /// <summary>
        /// Method to update opening location
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="openingLocation"></param>
        /// <returns></returns>
        int UpdateOpeningLocation(OpeningLocations openingLocation, List<LogHistoryTable> changeLogForOpenings);

        /// <summary>
        /// Method to get opening location details
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        OpeningLocations GetOpeningLocationBygroupId(int GroupConfigurationId, List<VariableAssignment> val, List<ConfigVariable> mapperVariables, string sessionId);
        /// <summary>
        /// Get Permission by role
        /// </summary>
        /// <param Name="Id"></param>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        List<string> GetPermissionByRole(int id, string roleName);

        /// <summary>
        /// UpdateGroupConflictStatus
        /// </summary>
        /// <param name="configurationId"></param>
        /// <param name="conflictStatusFlag"></param>
        /// <returns></returns>
        Result UpdateGroupConflictStatus(int configurationId, bool conflictStatusFlag);
    }
}
