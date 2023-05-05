/************************************************************************************************************
************************************************************************************************************
    File Name     :   UnitConfigurationStubDL.class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
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
using TKE.SC.Common.Model.CommonModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    class UnitConfigurationStubDL : IUnitConfigurationDL
    {
        public List<ResultUnitConfiguration> SaveCabInteriorDetails(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            if (groupid == 0 || listOfDetails.Count == 0 || productName == null || userId == null)
            {
                return null;
            }
            ResultUnitConfiguration resCabInterior = new ResultUnitConfiguration();
            resCabInterior.result = 1;
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            resCabInterior.message = "Cab Interior Saved Successfully";
            lstResult.Add(resCabInterior);
            return lstResult;
        }

        public List<ResultUnitConfiguration> UpdateCabInteriorDetails(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            if (groupid == 0 || listOfDetails.Count == 0 || productName == null || userId == null)
            {
                return null;
            }
            ResultUnitConfiguration resCabInterior = new ResultUnitConfiguration();
            resCabInterior.result = 1;
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            resCabInterior.message = "Cab Interior Updated Successfully";
            lstResult.Add(resCabInterior);
            return lstResult;
        }

        List<UnitNames> IUnitConfigurationDL.GetUnitsByGroupId(int groupConfigurationId, int setId)
        {
            if (groupConfigurationId == null || setId == 0)
            {
                return null;
            }
            else
            {
                return new List<UnitNames> { new UnitNames { SetId = 1, Price = 12, ProductName = "s", Ueid = "", Unitid = 2, Unitname = "" } } ;
            }
        }

        public List<ResultUnitConfiguration> SaveHoistwayTractionEquipment(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            if (groupid == 0 || listOfDetails.Count == 0 || productName == null || userId == null)
            {
                return null;
            }
            ResultUnitConfiguration resHoistwayTractionEquipment = new ResultUnitConfiguration();
            resHoistwayTractionEquipment.result = 1;
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            resHoistwayTractionEquipment.message = "Hoistway Traction Equipment Saved Successfully";
            lstResult.Add(resHoistwayTractionEquipment);
            return lstResult;
        }

        public List<ResultUnitConfiguration> UpdateHoistwayTractionEquipment(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            if (groupid == 0 || listOfDetails.Count == 0 || productName == null || userId == null)
            {
                return null;
            }
            ResultUnitConfiguration resHoistwayTractionEquipment = new ResultUnitConfiguration();
            resHoistwayTractionEquipment.result = 1;
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            resHoistwayTractionEquipment.message = "Hoistway Traction Equipment Updated Successfully";
            lstResult.Add(resHoistwayTractionEquipment);
            return lstResult;
        }

        public List<ConfigVariable> GetUnitConfigurationByGroupId(int groupConfigurationId, int setId, string selectTab)
        {
            if (groupConfigurationId == 0 || setId == 0 || selectTab == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return new List<ConfigVariable>();
            }
        }



        //public List<ConfigVariable> GetGeneralInformationByGroupId(int groupConfigurationId, string selectTab)
        //{
        //    throw new NotImplementedException();
        //}





        //List<UnitNames> IUnitConfigurationDL.GetUnitsByGroupId(int groupConfigurationId, string productName, string selectTab)
        //{
        //    if(groupConfigurationId == null || productName == null || selectTab == null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return new List<UnitNames>();
        //    }
        //    lstResult.Add(result);
        //    return lstResult;

        //}

        //List<ConfigVariable> IUnitConfigurationDL.GetGeneralInformationByGroupId(int groupConfigurationId, string productName, string selectTab)
        //{
        //    throw new NotImplementedException();
        //}

        public List<ResultUnitConfiguration> UpdateGeneralInformation(int groupid, string productName, List<ConfigVariable> listOfGeneralInformationVariables, string userId)
        {
            if (groupid == 0 || productName == null || listOfGeneralInformationVariables == null || userId == null)
            {
                return null;
            }
            else
            {
                ResultUnitConfiguration resSaveEntrance = new ResultUnitConfiguration();
                resSaveEntrance.result = 1;
                List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
                resSaveEntrance.message = "General Information updated Successfully";
                lstResult.Add(resSaveEntrance);
                return lstResult;
            }

        }
        public List<ConfigVariable> GetCabInteriorByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            if (groupConfigurationId == 0 || productName == null || selectTab == null)
            {
                return null;
            }
            else
            {
                List<ConfigVariable> res = new List<ConfigVariable>();
                return res;
            }
        }

        public List<ResultUnitConfiguration> SaveGeneralInformation(int groupid, string productName, List<ConfigVariable> listOfGeneralInformationVariables, string userId)
        {
            if (groupid == 0 || productName == null || listOfGeneralInformationVariables == null || userId == null)
            {
                return null;
            }
            else
            {
                ResultUnitConfiguration resSaveEntrance = new ResultUnitConfiguration();
                resSaveEntrance.result = 1;
                List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
                resSaveEntrance.message = "General Information saved Successfully";
                lstResult.Add(resSaveEntrance);
                return lstResult;
            }
        }

        public List<ConfigVariable> GetHoistwayTractionByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            if (groupConfigurationId == 0 || productName == null || selectTab == null)
            {
                return null;
            }
            else
            {
                List<ConfigVariable> res = new List<ConfigVariable>();
                return res;
            }
        }

        public List<UnitNames> GetHoistwayTractionUnitsByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            if (groupConfigurationId == 0 || productName == null || selectTab == null)
            {
                return null;
            }
            else
            {
                List<UnitNames> res = new List<UnitNames>();
                return res;
            }
        }

        public List<ConfigVariable> GetEntranceByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            if (groupConfigurationId == 0 || productName == null || selectTab == null)
            {
                return null;
            }
            else
            {
                List<ConfigVariable> res = new List<ConfigVariable>();
                return res;
            }
        }

        public List<ResultUnitConfiguration> SaveEntrances(int groupid, string productName, List<ConfigVariable> listOfEntranceVariables, string userId)
        {
            if (groupid == 0 || productName == null || listOfEntranceVariables == null || userId == null)
            {
                throw new NotImplementedException();
            }
            ResultUnitConfiguration resSaveEntrance = new ResultUnitConfiguration();
            resSaveEntrance.result = 1;
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            resSaveEntrance.message = "Entrances Saved Successfully";
            lstResult.Add(resSaveEntrance);
            return lstResult;
        }

        public List<ResultUnitConfiguration> UpdateEntrances(int groupid, string productName, List<ConfigVariable> listOfEntranceVariables, string userId)
        {
            if (groupid == 0 || productName == null || listOfEntranceVariables == null || userId == null)
            {
                throw new NotImplementedException();
            }
            ResultUnitConfiguration resUpdateEntrance = new ResultUnitConfiguration();
            resUpdateEntrance.result = 1;
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            resUpdateEntrance.message = "Entrances Updated Successfully";
            lstResult.Add(resUpdateEntrance);
            return lstResult;
        }

        public List<ResultSetConfiguration> UpdateUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId)
        {
            if (setId == 0 || listOfDetails == null || userId == null)
            {
                throw new NotImplementedException();
            }
            ResultSetConfiguration resUpdateUnitConfiguration = new ResultSetConfiguration();
            resUpdateUnitConfiguration.result = 1;
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            resUpdateUnitConfiguration.message = "Unit details updated Successfully";
            lstResult.Add(resUpdateUnitConfiguration);
            return lstResult;
        }

        public List<ResultSetConfiguration> SaveUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, List<LogHistoryTable> historyTable)
        {
            if (setId == 0 || listOfDetails == null || userId == null)
            {
                throw new NotImplementedException();
            }
            ResultSetConfiguration resSaveUnitConfiguration = new ResultSetConfiguration();
            resSaveUnitConfiguration.result = 1;
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            resSaveUnitConfiguration.message = "Unit details saved Successfully";
            lstResult.Add(resSaveUnitConfiguration);
            return lstResult;
        }

        public List<ConfigVariable> GetGeneralInformationByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            if (groupConfigurationId == 0 || productName == null || selectTab == null)
            {
                throw new NotImplementedException();
            }
            return new List<ConfigVariable>();
        }

        public int EditUnitDesignation(int groupId, int unitId, string userid, UnitDesignation unit)
        {
            if (groupId == 1)
            {
                return 1;
            }
            return 0;
        }



        public List<ResultSetConfiguration> SaveEntranceConfiguration(int setId, EntranceConfigurationData entranceConfigurationData, string userId, int is_Save, bool isReset, List<LogHistoryTable> logHistoryTable)
        {
            throw new NotImplementedException();
        }

        public List<EntranceConfigurations> GetEntranceConfiguration(int setId, int groupId)
        {
            if (setId != 0)
            {
                List<EntranceConfigurations> lstEntranceconsole = new List<EntranceConfigurations>();
                foreach (var index in Enumerable.Range(0, 2))
                {
                    Openings Openings = new Openings()
                    {
                        Front = true,
                        Rear = true
                    };
                    List<ConfigVariable> VariableAssignments = new List<ConfigVariable>();
                    foreach (var assignment in Enumerable.Range(1, 3))
                    {
                        ConfigVariable variable = new ConfigVariable()
                        {
                            VariableId = "ELEVATOR.Sales_UI_Screens.Entrance_Config_SP.Parameters.Doors.Landing_Doors_Assembly.Landing_Door_Jamb.COLFACEF",
                            Value = assignment
                        };
                        VariableAssignments.Add(variable);
                    }
                    List<EntranceLocations> EntranceLocations = new List<EntranceLocations>();
                    foreach (var location in Enumerable.Range(1, 5))
                    {
                        EntranceLocations entranceLocation = new EntranceLocations()
                        {
                            FloorNumber = location,
                            FloorDesignation = location.ToString(),
                            Front = new LandingOpening()
                            {
                                InCompatible = false,
                                Value = false
                            },
                            Rear = new LandingOpening()
                            {
                                InCompatible = false,
                                Value = false
                            }
                        };
                        EntranceLocations.Add(entranceLocation);
                    }
                    EntranceConfigurations entranceConsole = new EntranceConfigurations()
                    {
                        EntranceConsoleId = index,
                        ConsoleName = Constant.ENTRANCECONSOLENAME + index,
                        AssignOpenings = true,
                        IsController = false,
                        Openings = Openings,
                        VariableAssignments = VariableAssignments,
                        FixtureLocations = EntranceLocations


                    };
                    lstEntranceconsole.Add(entranceConsole);
                }
                return lstEntranceconsole;
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        public List<ResultSetConfiguration> SaveEntranceConfiguration(int setId, EntranceConfigurationData entranceConfigurationData, string userId, int is_Saved)
        {
            throw new NotImplementedException();
        }

        public TP2Summary GetDetailsForTP2SummaryScreen(int setId)
        {
            if (setId == -1)
            {
                throw new NotImplementedException();
            }
            TP2Summary result = new TP2Summary();
            result.GroupId = 1;
            List<UnitDetailsForTP2> resultUnit = new List<UnitDetailsForTP2>();
            UnitDetailsForTP2 unit = new UnitDetailsForTP2();
            unit.UnitId = 1;
            unit.Ueid = "Ueid";
            unit.UnitName = "U1";
            unit.ProductName = "ProductName";
            resultUnit.Add(unit);
            result.UnitDetails = resultUnit;
            List<VariablesList> resultVar = new List<VariablesList>();
            VariablesList var = new VariablesList();
            var.Id = "unitVariablesList";
            var.VariableAssignments = new List<ConfigVariable>();
            ConfigVariable c1 = new ConfigVariable();
            c1.Value = "Value";
            c1.VariableId = "VariableId";
            var.VariableAssignments.Add(c1);
            VariablesList var1 = new VariablesList();
            var1.Id = "buildingVariablesList";
            var1.VariableAssignments = new List<ConfigVariable>();
            ConfigVariable variable = new ConfigVariable();
            variable.VariableId = "VariableId";
            variable.Value = "VariableValue";
            var1.VariableAssignments.Add(variable);
            resultVar.Add(var);
            resultVar.Add(var1);
            result.VariableAssignments = resultVar;
            return result;

        }

        public string GetFixtureStrategy(int groupConfigurationId)
        {
            if (groupConfigurationId != 0 && groupConfigurationId != 111)
            {
                return Constant.ETA;
            }
            else if (groupConfigurationId == 111)
            {
                return Constant.ETD;
            }
            return null;
        }

        public List<UnitHallFixtures> GetUnitHallFixturesData(int setId, int groupId, string userName, string fixtureStrategy)
        {
            if (setId != 0)
            {
                List<UnitHallFixtures> lstConsole = new List<UnitHallFixtures>();
                foreach (var index in Enumerable.Range(0, 2))
                {
                    Openings Openings = new Openings()
                    {
                        Front = true,
                        Rear = true
                    };
                    List<ConfigVariable> VariableAssignments = new List<ConfigVariable>();
                    if (setId != 111)
                    {
                        foreach (var assignment in Enumerable.Range(1, 3))
                        {
                            ConfigVariable variable = new ConfigVariable()
                            {
                                VariableId = "ELEVATOR.Sales_UI_Screens.Entrance_Config_SP.Parameters.Doors.Landing_Doors_Assembly.Landing_Door_Jamb.COLFACEF",
                                Value = assignment
                            };
                            VariableAssignments.Add(variable);
                        }
                    }
                    List<EntranceLocations> EntranceLocations = new List<EntranceLocations>();
                    foreach (var location in Enumerable.Range(1, 5))
                    {
                        EntranceLocations entranceLocation = new EntranceLocations()
                        {
                            FloorNumber = location,
                            FloorDesignation = location.ToString(),
                            Front = new LandingOpening()
                            {
                                InCompatible = false,
                                Value = false
                            },
                            Rear = new LandingOpening()
                            {
                                InCompatible = false,
                                Value = false
                            }
                        };
                        EntranceLocations.Add(entranceLocation);
                    }
                    UnitHallFixtures console = new UnitHallFixtures()
                    {
                        ConsoleId = index,
                        ConsoleName = Constant.ENTRANCECONSOLENAME + index,
                        AssignOpenings = true,
                        IsController = false,
                        UnitHallFixtureType = Constant.HALLLANTERN,
                        Openings = Openings,
                        VariableAssignments = VariableAssignments,
                        UnitHallFixtureLocations = EntranceLocations




                    };
                    lstConsole.Add(console);
                }
                return lstConsole;
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        public List<string> GenerateUnitHallFixturesList(string fixtureStrategy)
        {
            List<string> newList = new List<string>();
            newList.Add(Constant.HALLLANTERN);
            return newList;
        }

        public List<ResultSetConfiguration> SaveUnitHallFixtureConfiguration(int setId, UnitHallFixtureData unitHallFixtureConfigurationData, string userId, int is_Save, List<LogHistoryTable> logHistoryTabled)
        {
            if (setId == 0 || userId == null || unitHallFixtureConfigurationData == null || userId == null)
            {
                throw new NotImplementedException();
            }
            ResultSetConfiguration resSaveEntrance = new ResultSetConfiguration();
            resSaveEntrance.result = 1;
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            resSaveEntrance.message = "Unit Configuration Saved Successfully";
            lstResult.Add(resSaveEntrance);
            return lstResult;
        }
        public List<string> GetGroupHallFixturesTypesList(string fixtureStrategy)
        {
            throw new NotImplementedException();

        }

        public List<ResultSetConfiguration> DeleteEntranceConsole(int consoleId, int setId, List<LogHistoryTable> logHistoryTable, string userId)
        {
            if (consoleId == 0 || setId == 0 || userId.Equals(null))
            {
                throw new NotImplementedException();
            }
            ResultSetConfiguration resSaveEntrance = new ResultSetConfiguration();
            resSaveEntrance.result = 1;
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            resSaveEntrance.message = "Unit Configuration Saved Successfully";
            lstResult.Add(resSaveEntrance);
            return lstResult;
        }

        public List<ResultSetConfiguration> DeleteUnitHallFixtureConsole(int setId, int consoleId, string fixtureType, List<LogHistoryTable> logHistoryTable, string userId)
        {
            if (setId == 0 || consoleId == 0 || fixtureType == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                ResultSetConfiguration resDelete = new ResultSetConfiguration();
                resDelete.result = 1;
                List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
                resDelete.message = "Unit Hall Fixture Deleted Successfully";
                lstResult.Add(resDelete);
                return lstResult;
            }
        }

        public List<UnitHallFixtures> ResetUnitHallFixtureConsole(int setId, int consoleId, string fixtureType)
        {
            throw new NotImplementedException();
        }

        public List<UnitHallFixtures> ResetUnitHallFixtureConsole(int setId, int consoleId, string fixtureType, string userName)
        {
            throw new NotImplementedException();
        }

        public List<ResultSetConfiguration> SaveCarCallCutoutKeyswitchOpenings(int setId, CarcallCutoutData carcallCutoutData, string userId, List<LogHistoryTable> historyTable)
        {
            if (setId == 0 || userId == null || carcallCutoutData == null)
            {
                throw new NotImplementedException();
            }
            ResultSetConfiguration resSaveEntrance = new ResultSetConfiguration();
            resSaveEntrance.result = 1;
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            resSaveEntrance.message = "Unit Configuration Saved Successfully";
            lstResult.Add(resSaveEntrance);
            return lstResult;
        }

        public EntranceAssignment GetCarCallCutoutOpenings(int setId)
        {
            EntranceAssignment objOpeningsData = new EntranceAssignment();
            if (setId != 0)
            {
                var mainEgress = 1;
                List<EntranceConfigurations> entranceConsoles = new List<EntranceConfigurations>();
                foreach (var index in Enumerable.Range(0, 2))
                {
                    var opening = new Openings()
                    {
                        Front = true,
                        Rear = true
                    };
                    List<EntranceLocations> entranceLocations = new List<EntranceLocations>();
                    foreach (var location in Enumerable.Range(0, 5))
                    {
                        EntranceLocations entranceLocation = new EntranceLocations()
                        {
                            FloorNumber = location,
                            FloorDesignation = location.ToString()
                        };
                        LandingOpening landingOpening = new LandingOpening()
                        {
                            InCompatible = false,
                            NotAvailable = false,
                            Value = false
                        };
                        entranceLocation.Front = landingOpening;
                        landingOpening = new LandingOpening()
                        {
                            InCompatible = false,
                            NotAvailable = false,
                            Value = false
                        };
                        entranceLocation.Rear = landingOpening;
                        entranceLocations.Add(entranceLocation);


                    }
                    foreach (var location in entranceLocations)
                    {
                        if (location.FloorNumber.Equals(mainEgress))
                        {
                            location.Front.NotAvailable = true;
                            location.Rear.NotAvailable = true;
                        }
                    }
                    objOpeningsData.Openings = opening;
                    objOpeningsData.FixtureAssignments = entranceLocations;

                }
                objOpeningsData.IsSaved = true;
                return objOpeningsData;
            }


            throw new NotImplementedException();
        }

        public int GetCarCallcutoutSavedOpenings(int setId)
        {
            if (setId != 0)
            {
                return 1;
            }
            throw new NotImplementedException();
        }

        public List<ResultSetConfiguration> UpdateUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, ConflictsStatus isEditFlow, List<LogHistoryTable> historyTable)
        {
            if (setId > 0 && listOfDetails.Count > 0 && userId != string.Empty)
            {
                ResultSetConfiguration res = new ResultSetConfiguration();
                res.setId = 1;
                res.result = 1;
                res.message = "success";
                List<ResultSetConfiguration> results = new List<ResultSetConfiguration>();
                results.Add(res);
                return results;
            }
            throw new NotImplementedException();
        }


        public LogHistoryResponse GetLogHistoryUnit(int SetId, int UnitId, string lastDate)
        {
            if (SetId != 0)
            {
                var loghistoryresponse = new LogHistoryResponse();
                loghistoryresponse.Data = new List<Data>();
                Data data = new Data();
                string date;
                date = "26 / 02 / 2021";
                List<LogParameters> logparamters = new List<LogParameters>();
                {

                    foreach (var num in Enumerable.Range(1, 5))
                    {

                        LogParameters log = new LogParameters();
                        log.VariableId = "ELEVATOR.Parameters.Basic_Info.CAPACITY";
                        log.Name = "Dimension";
                        log.UpdatedValue = "Minimum";
                        log.PreviousValue = " ";
                        log.User = "c2duser";
                        log.Role = " ";
                        logparamters.Add(log);

                    }
                    data = new Data { Date = date, LogParameters = logparamters };
                }

                loghistoryresponse.Section = "unit";
                loghistoryresponse.Description = "U3";
                loghistoryresponse.Data.Add(data);
                loghistoryresponse.ShowLoadMore = false;
                return loghistoryresponse;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public TP2Summary GetTravelValue(int setId)
        {
            return null;
        }

        public string GetSystemsValValues(int setId, string userId)
        {
            throw new NotImplementedException();
        }

        public ConfigurationResponse GetLatestSystemsValValues(int setId, string statusKey, string userId)
        {
            throw new NotImplementedException();
        }

        public ConfigurationResponse GetLatestSystemsValValues(int setId, string statusKey, string userId, List<SystemValidationKeyValues> systemKeyValues, string type, List<UnitDetailsForTP2> UnitDetails)
        {
            throw new NotImplementedException();
        }

        public TP2Summary GetDetailsForUnits(int setId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetPermissionByRole(int id, string roleName)
        {
            throw new NotImplementedException();
        }

        public ConfigurationResponse GetLatestSystemsValValues(int setId, string statusKey, string userId, List<SystemValidationKeyValues> systemKeyValues, string type)
        {
            throw new NotImplementedException();
        }

        public List<ResultSetConfiguration> SavePriceValuesDL(int setId, List<ConfigVariable> listOfDetails, List<ConfigVariable> listOfLeadTimes, string userId, List<LogHistoryTable> historyTable)
        {
            if (setId == 0 || listOfDetails.Count == 0 || listOfLeadTimes.Count == 0 || userId == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                var res = new ResultSetConfiguration() { setId = 1, result = 1, description = Constant.PRICEDETAILSSAVEMESSAGE, message = Constant.PRICEDETAILSSAVEMESSAGE };
                List<ResultSetConfiguration> listResult = new List<ResultSetConfiguration>() { res };
                return listResult;
            }
        }

        Status IUnitConfigurationDL.GetSystemsValValues(int setId, string userId)
        {
            throw new NotImplementedException();
        }

        public ConfigurationResponse GetLatestSystemsValValues(int setId, string statusKey, string userId, List<SystemValidationKeyValues> systemKeyValues, string type, List<UnitDetailsForTP2> UnitDetails, List<VariableAssignment> Response = null)
        {
            throw new NotImplementedException();
        }

        public string GetProductCategoryByGroupId(int id, string type)
        {
            throw new NotImplementedException();
        }

        public List<ConfigVariable> GetVariableAssignmentsBySetId(int setId)
        {
            throw new NotImplementedException();
        }

        public List<ResultSetConfiguration> SaveNonConfigurableUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId)
        {
            throw new NotImplementedException();
        }


        List<ResultUnitConfiguration> IUnitConfigurationDL.SaveCabInteriorDetails(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            if(groupid>0)
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 1 } };
            }
            else
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 0 } };
            }
        }

        List<ResultUnitConfiguration> IUnitConfigurationDL.UpdateCabInteriorDetails(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            if (groupid > 0)
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 1 } };
            }
            else
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 0 } };
            }
        }

        List<ResultUnitConfiguration> IUnitConfigurationDL.SaveHoistwayTractionEquipment(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            if (groupid > 0)
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 1 } };
            }
            else
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 0 } };
            }
        }



        List<ResultUnitConfiguration> IUnitConfigurationDL.UpdateHoistwayTractionEquipment(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            if (groupid > 0)
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 1 } };
            }
            else
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 0 } };
            };
        }

        List<ConfigVariable> IUnitConfigurationDL.GetGeneralInformationByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            throw new NotImplementedException();
        }

        List<ConfigVariable> IUnitConfigurationDL.GetCabInteriorByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            throw new NotImplementedException();
        }

        List<ConfigVariable> IUnitConfigurationDL.GetHoistwayTractionByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            throw new NotImplementedException();
        }

        List<UnitNames> IUnitConfigurationDL.GetHoistwayTractionUnitsByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            throw new NotImplementedException();
        }

        List<ResultUnitConfiguration> IUnitConfigurationDL.UpdateGeneralInformation(int groupid, string productName, List<ConfigVariable> listOfGeneralInformationVariables, string userId)
        {
            throw new NotImplementedException();
        }

        List<ResultUnitConfiguration> IUnitConfigurationDL.SaveGeneralInformation(int groupid, string productName, List<ConfigVariable> listOfGeneralInformationVariables, string userId)
        {
            if (groupid > 0)
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 1 } };
            }
            else
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 0 } };
            }
        }

        List<ResultUnitConfiguration> IUnitConfigurationDL.SaveEntrances(int groupid, string productName, List<ConfigVariable> listOfEntranceVariables, string userId)
        {
            if (groupid > 0)
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 1 } };
            }
            else
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 0 } };
            }
        }

        List<ConfigVariable> IUnitConfigurationDL.GetEntranceByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            throw new NotImplementedException();
        }

        List<ResultUnitConfiguration> IUnitConfigurationDL.UpdateEntrances(int groupid, string productName, List<ConfigVariable> listOfEntranceVariables, string userId)
        {
            if (groupid > 0)
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 1 } };
            }
            else
            {
                return new List<ResultUnitConfiguration> { new ResultUnitConfiguration { result = 0 } };
            }
        }

        //public int EditUnitDesignation(int groupId, int unitId, string userid, UnitDesignation unit)
        // {
        //     var methodBegin = Utility.LogBegin();
        //     if (unit.Designation == null)
        //     {
        //         unit.Designation = "";
        //     }
        //     if (unit.Description == null)
        //     {
        //         unit.Description = "";
        //     }
        //     IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForEditUnitdesignation(groupId, unitId, userid, unit);
        //     Utility.LogEnd(methodBegin);
        //     return CpqDatabaseManager.ExecuteNonquery(Constant.SPEDITUNITDESIGNATION, lstSqlParameter); ;
        // }

        List<ResultSetConfiguration> IUnitConfigurationDL.SaveEntranceConfiguration(int setId, EntranceConfigurationData entranceConfigurationData, string userId, int is_Saved, bool isReset, List<LogHistoryTable> historyTable)
        {
            if (entranceConfigurationData.ConsoleName == "1")
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 1,message="Entrance Configuration Saved Successfully" } };
            }
            else
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 0 } };
            }
        }

        List<EntranceConfigurations> IUnitConfigurationDL.GetEntranceConfiguration(int setId, int groupId,string controlLanding, string username, bool isJambMounted)
        {
            return new List<EntranceConfigurations> { new EntranceConfigurations {
            AssignOpenings=true,ConsoleName="1",EntranceConsoleId=0,IsController=true,NoOfFloor=5,RearOpenings=0,FrontOpenings=1,
            ProductName="EVO_200",VariableAssignments= new List<ConfigVariable>{ new ConfigVariable { Value="ELEVATOR.Sales_UI_Screens.Entrance_Config_SP.Parameters.COLFACEF"
            ,VariableId="val"} },FixtureLocations=new List<EntranceLocations>{new EntranceLocations
            {FloorDesignation="val",FloorNumber=5,Front=new LandingOpening{ InCompatible=false,NotAvailable=true,Value=false} ,Rear
            =new LandingOpening{InCompatible=false,NotAvailable=true,Value=""}
            } },Openings= new Openings{Front=true,Rear=false }
            } };
        }

        TP2Summary IUnitConfigurationDL.GetDetailsForTP2SummaryScreen(int setId)
        {
            return new TP2Summary
            {
                VariableAssignments = new List<VariablesList> { new VariablesList { VariableAssignments=
                new List<ConfigVariable>{ new ConfigVariable { Value= "buildingVariablesList", VariableId= "buildingVariablesList" } },Id="buildingVariablesList" },new VariablesList{ Id="unitVariablesList",VariableAssignments=new List<ConfigVariable>{ new ConfigVariable {
                Value="val",VariableId="val"} } }, new VariablesList{ Id="groupVariablesList",VariableAssignments=new List<ConfigVariable>{ new ConfigVariable {
                Value="val",VariableId="val"} } }},
                OpeningVariableAssginments
            = new List<OpeningVariables> { new OpeningVariables { VariableAssigned = new ConfigVariable { VariableId = "buildingVariablesList", Value = "buildingVariablesList" } } }
            ,
                FloorMatrixTable = new List<PriceSectionDetails> { new PriceSectionDetails { Id= "manufacturingCommentsTable",Section=""
            ,Name="",PriceKeyInfo=new List<PriceValuesDetails>{ new PriceValuesDetails { } } } }
                ,UnitDetails= new List<UnitDetailsForTP2> { new UnitDetailsForTP2 { ProductName="val",Ueid="1",UnitId=1,UnitName="val"} }
            ,GroupUnitInfo= new List<UnitNames> { new UnitNames { SetId=1,Price=12,ProductName="val",Ueid="1",Unitid=1,Unitname="val"} }
            , CustomPriceLine= new List<CustomPriceLine> { }, projectInfo=new ProjectInfo { Source="",FrontOpenings="" +
            "s",Status="",PrimarySalesRep="",ProjectStatus="",Branch="",BuildingName="",GroupName="",OracleProjectId=""
            ,PrimaryCoordinator="",ProjectId="",ProjectName="",QuoteId="",QuoteVersion="",RearOpenings="",Travel="",
            UnitMFGJobNo="",UnitName=new List<string> { ""}
            },QuoteSummary= new List<QuoteSummary> { new QuoteSummary { SetId="",BuildingId="",BuildingName="",GroupId=""
            ,GroupName="",NumberOfUnits="",OrderType="",ProductLine="",UnitName=""} }
            ,ProjectData= new ProjectData { QuoteStatus="",QuoteDate="",OpportunityId="",SalesStage="",ProjectStatus=""
            ,Address="",BookeDate="",Contact="",CustomerAccount="",VersionId=""}
            };
        }

        string IUnitConfigurationDL.GetFixtureStrategy(int groupConfigurationId)
        {
            if (groupConfigurationId > 0)
            {
                return "ETA";
            }
            else
            {
                return "ETD";
            }


        }

        List<UnitHallFixtures> IUnitConfigurationDL.GetUnitHallFixturesData(int setId, int groupId, string userName, string fixtureStrategy, string sessionId)
        {
            List<UnitHallFixtures> lstConsole = new List<UnitHallFixtures>();
            foreach (var index in Enumerable.Range(0, 2))
            {
                Openings Openings = new Openings()
                {
                    Front = true,
                    Rear = true
                };
                List<ConfigVariable> VariableAssignments = new List<ConfigVariable>();
                if (setId != 111)
                {
                    foreach (var assignment in Enumerable.Range(1, 3))
                    {
                        ConfigVariable variable = new ConfigVariable()
                        {
                            VariableId = "ELEVATOR.Sales_UI_Screens.Entrance_Config_SP.Parameters.Doors.Landing_Doors_Assembly.Landing_Door_Jamb.COLFACEF",
                            Value = assignment
                        };
                        VariableAssignments.Add(variable);
                    }
                }
                List<EntranceLocations> EntranceLocations = new List<EntranceLocations>();
                foreach (var location in Enumerable.Range(1, 5))
                {
                    EntranceLocations entranceLocation = new EntranceLocations()
                    {
                        FloorNumber = location,
                        FloorDesignation = location.ToString(),
                        Front = new LandingOpening()
                        {
                            InCompatible = false,
                            Value = false
                        },
                        Rear = new LandingOpening()
                        {
                            InCompatible = false,
                            Value = false
                        }
                    };
                    EntranceLocations.Add(entranceLocation);
                }
                UnitHallFixtures console = new UnitHallFixtures()
                {
                    ConsoleId = index,
                    ConsoleName = Constant.ENTRANCECONSOLENAME + index,
                    AssignOpenings = true,
                    IsController = false,
                    UnitHallFixtureType = Constant.HALLLANTERN,
                    Openings = Openings,
                    VariableAssignments = VariableAssignments,
                    UnitHallFixtureLocations = EntranceLocations




                };
                lstConsole.Add(console);
            }
            return lstConsole;

        }

        List<string> IUnitConfigurationDL.GenerateUnitHallFixturesList(string fixtureStrategy)
        {
            return new List<string> { "Val" };
        }

        List<ResultSetConfiguration> IUnitConfigurationDL.SaveUnitHallFixtureConfiguration(int setId, UnitHallFixtureData unitHallFixtureConfigurationData, string userId, int is_Saved, List<LogHistoryTable> logHistorutable)
        {
            if (Convert.ToInt32(unitHallFixtureConfigurationData.ConsoleId)==1)
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 1, message="Unit Hall Fixture Saved Successfully" } };
            }
            else
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 0 } };
            }
        }

        List<string> IUnitConfigurationDL.GetGroupHallFixturesTypesList(string fixtureStrategy)
        {
            throw new NotImplementedException();
        }

        List<ResultSetConfiguration> IUnitConfigurationDL.DeleteEntranceConsole(int consoleId, int setId, List<LogHistoryTable> historyTable, string userId)
        {
            if (consoleId == 1)
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 1 ,message="Entrance Configuration Deleted"} };
            }
            else
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 0 } };
            }
        }


        List<ResultSetConfiguration> IUnitConfigurationDL.DeleteUnitHallFixtureConsole(int setId, int consoleId, string fixtureType, List<LogHistoryTable> historyTable, string userId)
        {
            if (setId == 1)
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 1,message="Unit Hall Fixture Deleted Successfully" } };
            }
            else
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 0 } };
            }
        }

        List<UnitHallFixtures> IUnitConfigurationDL.ResetUnitHallFixtureConsole(int setId, int consoleId, string fixtureType, string userName)
        {
            throw new NotImplementedException();
        }

        List<ResultSetConfiguration> IUnitConfigurationDL.SaveCarCallCutoutKeyswitchOpenings(int setId, CarcallCutoutData carcallCutoutData, string userId, List<LogHistoryTable> loghistoryTable)
        {
            if (setId == 1)
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 1 ,message="Car Call Saved"} };
            }
            else
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 0 } };
            }
        }

        EntranceAssignment IUnitConfigurationDL.GetCarCallCutoutOpenings(int setId)
        {
            if (setId > 0)
            {
                return new EntranceAssignment
                {
                    IsSaved = true,
                    Openings = new Openings { Front = true, Rear = true }
                ,
                    FixtureAssignments = new List<EntranceLocations> { new EntranceLocations { FloorDesignation = "", FloorNumber = 2, Front = new LandingOpening { InCompatible = true, NotAvailable = true, Value = 2 }, Rear = new LandingOpening { InCompatible = true, NotAvailable = true, Value = 2 } } }
                };
            }
            else
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
        }

        int IUnitConfigurationDL.GetCarCallcutoutSavedOpenings(int setId)
        {
            throw new NotImplementedException();
        }

        LogHistoryResponse IUnitConfigurationDL.GetLogHistoryUnit(int SetId, int UnitId, string lastDate)
        {
            throw new NotImplementedException();
        }

        TP2Summary IUnitConfigurationDL.GetTravelValue(int setId)
        {
            if (setId == -1)
            {
                throw new NotImplementedException();
            }
            TP2Summary result = new TP2Summary();
            result.GroupId = 1;
            List<UnitDetailsForTP2> resultUnit = new List<UnitDetailsForTP2>();
            UnitDetailsForTP2 unit = new UnitDetailsForTP2();
            unit.UnitId = 1;
            unit.Ueid = "Ueid";
            unit.UnitName = "U1";
            unit.ProductName = "ProductName";
            resultUnit.Add(unit);
            result.UnitDetails = resultUnit;
            List<VariablesList> resultVar = new List<VariablesList>();
            VariablesList var = new VariablesList();
            var.Id = "unitVariablesList";
            var.VariableAssignments = new List<ConfigVariable>();
            ConfigVariable c1 = new ConfigVariable();
            c1.Value = "Value";
            c1.VariableId = "VariableId";
            var.VariableAssignments.Add(c1);
            VariablesList var1 = new VariablesList();
            var1.Id = "buildingVariablesList";
            var1.VariableAssignments = new List<ConfigVariable>();
            ConfigVariable variable = new ConfigVariable();
            variable.VariableId = "VariableId";
            variable.Value = "VariableValue";
            var1.VariableAssignments.Add(variable);
            resultVar.Add(var);
            resultVar.Add(var1);
            result.VariableAssignments = resultVar;
            return result;
        }

        List<string> IUnitConfigurationDL.GetPermissionByRole(int id, string roleName, string entity = "Unit")
        {
            return new List<string> { "val" };
        }


        TP2Summary IUnitConfigurationDL.GetDetailsForUnits(int setId)
        {
            throw new NotImplementedException();
        }



        public string GetProductCategoryByGroupId(int id, string type, DataTable dtVariables)
        {
            if (id == 0)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
            else
            {
                return "Elevator";
            }
        }

        List<ConfigVariable> IUnitConfigurationDL.GetVariableAssignmentsBySetId(int setId, DataTable dtVariables)
        {
            throw new NotImplementedException();
        }

        List<ResultSetConfiguration> IUnitConfigurationDL.SaveNonConfigurableUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, DataTable configVariables)
        {
            throw new NotImplementedException();
        }

        string IUnitConfigurationDL.GetProductType(int setId)
        {
            if (setId > 0)
            {
                return "EVO_200";

            }
            else
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
        }

        public ConfigurationResponse GetLatestSystemsValValues(int setId, string statusKey, string userId, List<SystemValidationKeyValues> systemKeyValues, string type, List<UnitDetailsForTP2> UnitDetails, string sessionId, List<VariableAssignment> Response = null)
        {
            return new ConfigurationResponse { AllBeamsSelected = false, ReadOnly = true, Permissions = new List<string> { "" } };
        }

        public List<ResultSetConfiguration> SavePriceValuesDL(int setId, List<ConfigVariable> listOfDetails, List<ConfigVariable> listOfLeadTimes, string userId, List<LogHistoryTable> historyTable, List<UnitNames> unitPrices)
        {
            if (setId == 1)
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 1 } };
            }
            else
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 0 } };
            }
        }

        public List<CustomPriceLine> SaveCustomPriceLine(int setId, string sessionId, List<CustomPriceLine> customPriceLine)
        {
            throw new NotImplementedException();
        }

        public CustomPriceLine EditCustomPriceLine(int setId, string sessionId, CustomPriceLine customPriceLine)
        {
            throw new NotImplementedException();
        }

        public int DeleteCustomPriceLine(int setId, int priceLineId)
        {
            throw new NotImplementedException();
        }

        public List<ConfigVariable> GetDetailsForHoistwayWiring(string val, int setId)
        {
            throw new NotImplementedException();
        }

        public int CreateUpdateFactoryJobId(int unitId, string userid, string factoryJobId)
        {
            throw new NotImplementedException();
        }

        public List<ResultSetConfiguration> UpdateUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, ConflictsStatus isEditFlow, List<LogHistoryTable> historyTable, int unitId)
        {
            if (setId == 1)
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 1 } };
            }
            else
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 0 } };
            }
        }

        public List<ResultSetConfiguration> SaveUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, List<LogHistoryTable> historyTable, int unitId)
        {
            if (setId == 1)
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 1,message="Units Saved Successfully"} };
            }
            else
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result = 0 } };
            }
        }

        public List<ConfigVariable> GetDetailsForHoistwayWiring(string val, int setId, string sessionId, int unitId)
        {
            return new List<ConfigVariable> { new ConfigVariable {Value= "CARPOS", VariableId= "CARPOS" } };
        }

        UnitVariableDetails IUnitConfigurationDL.GetUnitConfigurationByGroupId(int groupConfigurationId, int setId, string selectTab)
        {
            return new UnitVariableDetails
            {
                conflictListOfStrings = new List<string> { "Value" },
                listOfConfigVariables
            = new List<ConfigVariable> { new ConfigVariable { Value = "CARPOS", VariableId = "CARPOS" } },
                Value = "",
                VariableId = ""
            };
        }
        public List<string> GetConflictsData(int setId)
        {
            throw new NotImplementedException();
        }
    }
}
