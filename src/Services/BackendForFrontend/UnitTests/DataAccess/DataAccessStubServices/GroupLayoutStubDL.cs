/************************************************************************************************************
************************************************************************************************************
    File Name     :   GroupLayoutStubDL.class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using Constant = TKE.SC.BFF.DataAccess.Helpers.Constant;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class GroupLayoutStubDL : IGroupLayoutDL
    {
        //public DataSet DuplicateGroupLayoutById(DataTable unitIDDataTable, int groupID)
        //{
        //    throw new NotImplementedException();
        //}

        public DataSet DuplicateGroupLayoutById(DataTable unitIDDataTable, int groupID, DataTable carPositionDataTable)
        {
            throw new NotImplementedException();
        }

        public List<ResultGroupConfiguration> UpdateGroupLayout(int groupId, string sectionTab, List<ConfigVariable> unitVariableAssignment, List<ConfigVariable> unitVariableAssignmentForHallRiser, List<ConfigVariable> unitVariableAssignmentForDoor, string userName, List<UnitMappingValues> unitMappingValues, List<ConfigVariable> unitVariableAssignmentForControlLocation, List<DisplayVariableAssignmentsValues> displayVariableAssignments, ConflictsStatus conflictsStatus)
        {


            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            ResultGroupConfiguration resGroupLayout = new ResultGroupConfiguration();

            var lstNames = (from assignment in displayVariableAssignments
                            where assignment.Value.Equals("TRUE")
                            select assignment).ToList();
            var duplicatedNames = lstNames.GroupBy(x => x.UnitDesignation).All(g => g.Count() > 1); 
            if (duplicatedNames)
            {
                resGroupLayout.Result = 0;
                resGroupLayout.Message = "Unit names should be unique";
                lstResult.Add(resGroupLayout);
                return lstResult;
            }
            
            resGroupLayout.Result = 1;
            if (groupId == 0 )
            {
                resGroupLayout.Result = 0;
            }
            resGroupLayout.Message = "Building Configuration Updated Successfully";
            lstResult.Add(resGroupLayout);
            return lstResult;
        }

        DataSet IGroupLayoutDL.DuplicateGroupLayoutById(DataTable unitIDDataTable, int groupID, DataTable carPositionDataTable)
        {
            if(groupID==0)
            {
                return new DataSet {DataSetName= "Duplicated Group Layout" };
            }
            throw new NotImplementedException();
        }

        List<ResultGroupConfiguration> IGroupLayoutDL.SaveGroupLayout(int groupId, List<ConfigVariable> unitVariableAssignment, List<ConfigVariable> unitVariableAssignmentForHallRiser, List<ConfigVariable> unitVariableAssignmentForDoor, string userName, List<UnitMappingValues> unitMappingValues, List<ConfigVariable> unitVariableAssignmentForControlLocation, List<DisplayVariableAssignmentsValues> displayVariableAssignments)
        {


            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            ResultGroupConfiguration resGroupLayout = new ResultGroupConfiguration();
            var lstNames = (from assignment in displayVariableAssignments
                            where assignment.Value.Equals("TRUE")
                            select assignment).ToList();
            var duplicatedNames = lstNames.GroupBy(x => x.UnitDesignation).All(g => g.Count() > 1);
            if (duplicatedNames)
            {
                resGroupLayout.Result = 0;
                resGroupLayout.Message = "Unit names should be unique";
                lstResult.Add(resGroupLayout);
                return lstResult;
            }

            resGroupLayout.Result = 1;
            resGroupLayout.Message = "Building Configuration Saved Successfully";
            if (groupId == 0)
            {
                resGroupLayout.Result = 0;
            }
            lstResult.Add(resGroupLayout);
            return lstResult;
        }
    }
}
