using Configit.Configurator.Server.Common;
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
using TKE.SC.BFF.Test.Common;
using Constant = TKE.SC.BFF.DataAccess.Helpers.Constant;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    class BuildingEquipmentStubDL : IBuildingEquipmentDL
    {
        public List<Result> DeleteBuildingEquipmentConsole(int buildingId, int consoleId, string userId,List<LogHistoryTable> logHistory)
        {
            if(buildingId == 0 || consoleId == 0 || userId.Equals(null))
            {
                throw new NotImplementedException();
            }
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            result.result = 1;
            result.buildingId = buildingId;
            result.message = Constant.DELETEBUILDINGEQUIPMENTSUCCESSMSG;
            lstResult.Add(result);
            return lstResult;
        }

        public List<Result> DuplicateBuildingEquipmentConsole(int buildingId, int consoleId, string userId)
        {
            if(buildingId ==0 || consoleId ==0 || userId.Equals(null))
            {
                throw new NotImplementedException();
            }
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            result.result = 1;
            result.buildingId = buildingId;
            result.message = Constant.DUPLICATEBUILDINGEQUIPMENTSUCCESSMSG;
            lstResult.Add(result);
            return lstResult;
        }

        public List<ConfigVariable> GetBuildingEquipmentConfigurationByBuildingId(int buildingId)
        {
            if(buildingId==-1)
            {
                throw new NotImplementedException();
            }
            ConfigVariable us = new ConfigVariable();
            us.VariableId = "VariableId";
            us.Value = "Value";
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            listGroup.Add(us);
            return listGroup;
        }

        public List<BuildingEquipmentData> GetBuildingEquipmentConsoles(int buildingId, string userName)
        {
            if(buildingId == 0 || userName.Equals(null))
            {
                throw new NotImplementedException();
            }
            List<BuildingEquipmentData> consoles = new List<BuildingEquipmentData>();
            BuildingEquipmentData console = new BuildingEquipmentData();
            console.ConsoleId = 1;
            console.ConsoleName = "Lobby Panel 1";
            console.ConsoleNumber = 1;
            consoles.Add(console);
            return consoles;
        }

        public List<BuildingEquipmentData> GetBuildingEquipmentConsoles(int buildingId, string userName, string sessionId)
        {
            List<BuildingEquipmentData> consoles = new List<BuildingEquipmentData>();
            BuildingEquipmentData console = new BuildingEquipmentData();
            if (buildingId == 0 || userName.Equals(null))
            {
                console.ConsoleId = 1;
            }
            console.ConsoleId = 1;
            console.ConsoleName = "Lobbey Panel 1";
            console.ConsoleNumber = 1;
            consoles.Add(console);
            return consoles;
        }

        public List<Result> SaveAssignGroups(int buildingId, int consoleId, BuildingEquipmentData buildingEquipmentConfigurationData, string userId, int is_Saved)
        {
            if(buildingId == 0 || consoleId ==0 || buildingEquipmentConfigurationData == null )
            {
                throw new NotImplementedException();
            }
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            result.result = 1;
            result.buildingId = buildingId;
            result.message = Constant.SAVEBUILDINGEQUIPMENTCONSOLESUCCESSMSG;
            lstResult.Add(result);
            return lstResult;
        }

        public List<Result> SaveAssignGroups(int buildingId, int consoleId, BuildingEquipmentData buildingEquipmentConfigurationData, string userId, int is_Saved, List<LogHistoryTable> historyTable)
        {
            throw new NotImplementedException();
        }

        public List<Result> SaveBuildingEquipmentConfiguration(int buildingId, List<ConfigVariable> buildingEquipmentConfigurationData, string userId, int is_Saved)
        {
            if (buildingId == 0 || userId.Equals(null) || buildingEquipmentConfigurationData == null)
            {
                throw new NotImplementedException();
            }
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            result.result = 1;
            result.buildingId = buildingId;
            result.message = Constant.SAVEBUILDINGEQUIPMENTSUCCESSMSG;
            lstResult.Add(result);
            return lstResult;
        }

        List<Result> IBuildingEquipmentDL.DeleteBuildingEquipmentConsole(int buildingId, int consoleId, string userId, List<LogHistoryTable> historyTable)
        {
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            result.result = 1;
            if (buildingId == 0 || userId.Equals(null)|| consoleId==0)
            {
                result.result = 2;
            }
            result.buildingId = buildingId;
            result.message = Constant.SAVEBUILDINGEQUIPMENTSUCCESSMSG;
            lstResult.Add(result);
            return lstResult;
        }

        List<Result> IBuildingEquipmentDL.DuplicateBuildingEquipmentConsole(int buildingId, int consoleId, string userId)
        {
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            result.result = 1;
            if (buildingId == 0 || userId.Equals(null))
            {
                result.result = 2;
            }
            result.buildingId = buildingId;
            result.message = Constant.SAVEBUILDINGEQUIPMENTSUCCESSMSG;
            lstResult.Add(result);
            return lstResult;
        }

        List<ConfigVariable> IBuildingEquipmentDL.GetBuildingEquipmentConfigurationByBuildingId(int buildingId, DataTable configVariables)
        {
            if (buildingId > 0)
            {
                var configVar = new List<ConfigVariable> { new ConfigVariable { Value = 1, VariableId = "" } };
                return configVar;
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = BFF.BusinessProcess.Helpers.Constant.BADREQUEST
                });
            }
        }

      

        List<Result> IBuildingEquipmentDL.SaveAssignGroups(int buildingId, int consoleId, BuildingEquipmentData buildingEquipmentConfigurationData, string userId, int is_Saved, List<LogHistoryTable> historyTable)
        {
            if (buildingId > 0)
            {
                if (consoleId == 5|| buildingEquipmentConfigurationData.ConsoleId==0)
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = BFF.BusinessProcess.Helpers.Constant.BADREQUEST
                    });
                }
                Result result = new Result();
                List<Result> lstResult = new List<Result>();
                result.result = 1;
                result.message = "Saved Assigned Groups";
                lstResult.Add(result);
                return lstResult;
            }

            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = BFF.BusinessProcess.Helpers.Constant.BADREQUEST
                });
            }
        }

        List<Result> IBuildingEquipmentDL.SaveBuildingEquipmentConfiguration(int buildingId, List<ConfigVariable> buildingEquipmentConfigurationData, string userId, int is_Saved)
        {
            if (buildingId > 0)
            {
                Result result = new Result();
                List<Result> lstResult = new List<Result>();
                result.result = 1;
                result.message = "Building Configuration selections saved successfully";
                lstResult.Add(result);
                return lstResult;
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = BFF.BusinessProcess.Helpers.Constant.BADREQUEST
                });
            }
        }
    }
}
