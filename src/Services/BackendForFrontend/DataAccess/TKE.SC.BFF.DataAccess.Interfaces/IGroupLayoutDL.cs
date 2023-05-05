/************************************************************************************************************
************************************************************************************************************
    File Name     :   IGroupLayoutDL class 
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
using System.Data;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IGroupLayoutDL
    {
        /// <summary>
        /// Interface to save group layout floor plan
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>

        List<ResultGroupConfiguration> SaveGroupLayout(int groupId, List<ConfigVariable> unitVariableAssignment, List<ConfigVariable> unitVariableAssignmentForHallRiser, List<ConfigVariable> unitVariableAssignmentForDoor, string userName, List<UnitMappingValues> unitMappingValues, List<ConfigVariable> unitVariableAssignmentForControlLocation, List<DisplayVariableAssignmentsValues> displayVariableAssignments);

        /// <summary>
        /// Interface to save group layout floor plan
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ResultGroupConfiguration> UpdateGroupLayout(int groupId , string sectionTab, List<ConfigVariable> unitVariableAssignment, List<ConfigVariable> unitVariableAssignmentForHallRiser, List<ConfigVariable> unitVariableAssignmentForDoor, string userName, List<UnitMappingValues> unitMappingValues, List<ConfigVariable> unitVariableAssignmentForControlLocation, List<DisplayVariableAssignmentsValues> displayVariableAssignments, ConflictsStatus conflictsStatus);

        /// <summary>
        /// Interface to save group layout floor plan
        /// </summary>
        /// <param Name="groupID"></param>
        /// <param Name="unitIDDataTable"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        DataSet DuplicateGroupLayoutById(DataTable unitIDDataTable, int groupID, DataTable carPositionDataTable);
    }
}
