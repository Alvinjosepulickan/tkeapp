/************************************************************************************************************
************************************************************************************************************
    File Name     :   OpeningLocationStubDL
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using Configit.Configurator.Server.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class OpeningLocationStubDL : IOpeningLocationDL
    {
        public List<string> GetPermissionByRole(int id, string roleName)
        {
            throw new NotImplementedException();
        }

        public Result UpdateGroupConflictStatus(int configurationId, bool conflictStatusFlag)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// this method is for setting stub data for the method GetOpeningLocationBygroupId
        ///// </summary>
        ///// <param name="GroupConfigurationId"></param>
        ///// <returns></returns>
        //public Object GetOpeningLocationBygroupId(int GroupConfigurationId)
        //{
        //    if (GroupConfigurationId != 0)
        //    {
        //        //List<OpeningLocationdata> lstOpeningLocationdata = new List<OpeningLocationdata>();
        //        Object lstOpeningLocationdata = new Object();
        //        OpeningLocationdata lstOpenLocationdata = new OpeningLocationdata();
        //        lstOpenLocationdata.groupConfigurationId = 1;
        //        lstOpeningLocationdata = lstOpenLocationdata;
        //        return lstOpeningLocationdata;
        //    }
        //    else
        //    {
        //        return null;

        //    }
        //}

        /// <summary>
        /// this method is for setting stub data for the method UpdateOpeningLocation
        /// </summary>
        /// <param name="groupConfigurationId"></param>
        /// <param name="openingLocation"></param>
        /// <returns></returns>
        public int UpdateOpeningLocation(OpeningLocations openingLocation)
        {
            if (openingLocation == null)
            {
                return 0;
            }
            else if (openingLocation.GroupConfigurationId != 0)
            {
                List<ResultOpeningLocation> lstResultOpeningLocationData = new List<ResultOpeningLocation>();
                return 1;
            }
            return 0;
        }

        public int UpdateOpeningLocation(OpeningLocations openingLocation, List<LogHistoryTable> changeLogForOpenings)
        {
            throw new NotImplementedException();
        }

        OpeningLocations IOpeningLocationDL.GetOpeningLocationBygroupId(int GroupConfigurationId, List<VariableAssignment> val, List<ConfigVariable> mapperVariables, string sessionId)
        {
            if (GroupConfigurationId > 0)
            {
                return new OpeningLocations { Permissions = new List<string> { "" }, NoOfFloors = 2, GroupConfigurationId = 5, Units
                =new List<UnitData> { new UnitData { SideOpening=1,OcuppiedSpace=false,FrontOpening=1,NoOfFloors=1,RearOpening=1,UnitId=1,UnitName="",
                OpeningDoors = new OpeningDoors{Side=true,Front=true,Rear=true } ,OpeningsAssigned= new List<OpeningsAssigned>{ new OpeningsAssigned
                {Side=true,AlternateEgress=true,FloorDesignation=""
                } } } }
                };
            }
            else

            {
                return new OpeningLocations { Permissions = new List<string> { "" }, NoOfFloors = 2, GroupConfigurationId = 0 };
            }
        }

        List<string> IOpeningLocationDL.GetPermissionByRole(int id, string roleName)
        {
            return new List<string>
            {
                "value"
            };
        }

        int IOpeningLocationDL.UpdateOpeningLocation(OpeningLocations openingLocation, List<LogHistoryTable> changeLogForOpenings)
        {
            if (openingLocation.BuildingRise == 0)
            {
                return 0;
            }
            else
            {

                return 1;
            }
        }

        //OpeningLocations IOpeningLocationDL.GetOpeningLocationBygroupId(int GroupConfigurationId, List<VariableAssignment> val)
        //{
        //    if (GroupConfigurationId == 001)
        //    {
        //        OpeningLocations lstOpeningLocationdata = new OpeningLocations();
        //        OpeningLocations lstOpenLocationdata = new OpeningLocations();
        //        lstOpeningLocationdata.GroupConfigurationId = 0;
        //        lstOpeningLocationdata = lstOpenLocationdata;
        //        return lstOpeningLocationdata;
        //    }
        //    else if (GroupConfigurationId != 0)
        //    {

        //        //List<OpeningLocationdata> lstOpeningLocationdata = new List<OpeningLocationdata>();
        //        OpeningLocations lstOpeningLocationdata = new OpeningLocations();
        //        OpeningLocations lstOpenLocationdata = new OpeningLocations();
        //        lstOpeningLocationdata.GroupConfigurationId = 2;
        //        return lstOpeningLocationdata;
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();

        //    }
        //}

        //public int UpdateOpeningLocation(OpeningLocations openingLocation)
        //{
        //    throw new NotImplementedException();
        //}

        //OpeningLocations IOpeningLocationDL.GetOpeningLocationBygroupId(int GroupConfigurationId)
        //{
        //    if (GroupConfigurationId != 0)
        //    {
        //        OpeningLocations OpeningLocatonData = new OpeningLocations();
        //        return OpeningLocatonData;
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();

        //    }
        //}
    }
}
