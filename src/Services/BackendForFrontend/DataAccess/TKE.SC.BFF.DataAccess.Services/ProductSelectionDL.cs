/************************************************************************************************************
************************************************************************************************************
    File Name     :   ProductSelectionDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model.UIModel;
using System.IO;
using TKE.SC.Common;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class ProductSelectionDL : IProductSelectionDL
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param Name="logger"></param>
        public ProductSelectionDL(ILogger<ProductSelectionDL> logger)
        {
            Utility.SetLogger(logger);
        }
        /// <summary>
        /// this method used to get the product details
        /// </summary>
        /// <param Name = "GroupConfigurationId" ></ param >
        /// <param Name="productSelection"></param>
        /// <param Name="businessLine"></param>
        /// <param Name="country"></param>
        /// <param Name="controlLanding"></param>
        /// <returns></returns>
        public int SaveProductSelection(int groupConfigurationId, ProductSelection productSelection, string businessLine, string country, string controlLanding, string fixtureStrategy,string supplyingFactory)
        {
            var methodBegin = Utility.LogBegin();
            var uHFDefaults = String.Empty;
            var entranceDefaults = String.Empty;
            if (productSelection.productSelected.Equals(Constant.EVO200))
            {
                uHFDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, Constant.EVOLUTION200))).ToString();
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constant.EVOLUTION200))).ToString();
            }
            else if (productSelection.productSelected.Equals(Constant.ENDURA100))
            {
                uHFDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, Constants.END100))).ToString();
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constants.END100))).ToString();
            }
            else if (productSelection.productSelected.Equals(Constant.EVO100))
            {
                uHFDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, Constants.EVOLUTION100))).ToString();
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constants.EVOLUTION100))).ToString();
            }
            var defaultUHFVariables = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(uHFDefaults);
            var defaultEntranceConfigurationValues = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(entranceDefaults);
            if (productSelection.productSelected == null)
            {
                productSelection.productSelected = string.Empty;
            }
            int controlLdg = controlLanding == null ? 0 : Convert.ToInt32(controlLanding);
            DataTable productSelectionDataTable = Utility.GenerateDataTableForProductSelection(productSelection);
            DataTable defaultVariablesUHF = Utility.GenerateDataTableForDefaultConsoleVariables(defaultUHFVariables);
            DataTable defaultVariablesEntranceConfiguration = Utility.GenerateDataTableForDefaultConsoleVariables(defaultEntranceConfigurationValues);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSavingProductSelection(groupConfigurationId, productSelection, productSelectionDataTable, businessLine, country, controlLdg, defaultVariablesUHF, defaultVariablesEntranceConfiguration, fixtureStrategy,supplyingFactory);
            int resultForSaveUnitConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEPRODUCTSELECTION, lstSqlParameter);
            Utility.LogEnd(methodBegin);
            return resultForSaveUnitConfiguration;

        }

        /// <summary>
        /// this method used to get the product details
        /// </summary>
        /// <param Name="UnitId"></param>
        /// <returns></returns>
        public int UnitSetValidation(List<int> unitId)
        {
            var methodBegin = Utility.LogBegin();
            DataTable unitDataTable = Utility.GenerateDataTableForProductSelection(unitId);
            SqlParameter param = new SqlParameter("@unitList", unitDataTable);
            List<SqlParameter> sqlParameterForUnitSet = Utility.SqlParameterForUnitSet(unitDataTable);
            Utility.LogEnd(methodBegin);
            return (CpqDatabaseManager.ExecuteNonquery(Constant.SPCHECKUNITSET, sqlParameterForUnitSet));
        }

        /// <summary>
        /// this method used to get the product details
        /// </summary>
        /// <param Name="UnitId"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetUnitVariableAssignments(List<int> unitId, string identifier)
        {
            var methodBegin = Utility.LogBegin();
            var unitMapperVariables = Utility.VariableMapper(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITMAPPER);
            var productSelectionMapperVariables = Utility.VariableMapper(Constant.PRODUCTSELECTIONCONSTANTPATH, Constant.VARIABLES);
            var productTypeMapperVariables = Utility.VariableMapper(Constant.PRODUCTSELECTIONCONSTANTPATH, Constants.PRODUCTTYPE);
            DataTable unitDataTable = Utility.GenerateDataTableForProductSelection(unitId);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@unitList", Value = unitDataTable}
            };
            DataSet dataSet = new DataSet();
            var unitDetailsList = new List<ConfigVariable>();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETUNITDETAILSFORVARIABLEASSIGNMENTS, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables.Count > 1)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var elevationData = (from DataRow row in dataSet.Tables[0].Rows
                                         select new { Floor = Convert.ToInt32(row[Constant.FLOORNUMBER]), Front = Convert.ToBoolean(row[Constant.FRONT]), Rear = Convert.ToBoolean(row[Constant.REAR]) }).Distinct().ToList();
                    unitDetailsList.AddRange((from DataRow row in dataSet.Tables[9].Rows
                                         select new ConfigVariable(){ VariableId = Convert.ToString(row[Constants.GROUPCONFIGURATIONTYPE]), Value = Convert.ToString(row[Constant.GROUPCONFIGURATIONVALUE]) }).Distinct().ToList());
                    var buildingElevation= (from DataRow row in dataSet.Tables[6].Rows
                                            select new { Floor = Convert.ToInt32(row[Constant.FLOORNUMBER]), FloorToFloorHeighFeet = Convert.ToDecimal(row[Constants.FLOORTOFLOORHEIGHTFEET]),
                                                FloorToFloorHeighInch = Convert.ToDecimal(row[Constant.FLOORTOFLOORHEIGHTINCH])
                                            , ElevationFeet = Convert.ToDecimal(row[Constants.ELEVAIONFEET]), ElevationInch = Convert.ToDecimal(row[Constants.ELEVATIONINCH]) }).Distinct().ToList();
                    if (elevationData != null && buildingElevation != null)
                    {
                        if(elevationData.Where(x=>x.Rear).Any())
                        {
                            unitDetailsList.Add(new ConfigVariable()
                            {
                                VariableId = productSelectionMapperVariables[Constants.REAROPEN],
                                Value = Constants.TRUEVALUES
                            });

                            unitDetailsList.Add(new ConfigVariable()
                            {
                                VariableId = productSelectionMapperVariables[Constants.ELEVATORTOPR],
                                Value = elevationData.Where(x => x.Rear).ToList().Max(x => x.Floor)
                            });
                        }
                        unitDetailsList.Add(new ConfigVariable()
                        {
                            VariableId = productSelectionMapperVariables[Constants.BUILDINGBLANDINGS],
                            Value = buildingElevation.Count()
                        });
                        unitDetailsList.Add(new ConfigVariable()
                        {
                            VariableId = productSelectionMapperVariables[Constants.ELEVATORTOPF],
                            Value = elevationData.Where(x => x.Front).ToList().Max(x => x.Floor)
                        }) ;
                        foreach (var floor in elevationData)
                        {
                            var elevationDataForFloor = buildingElevation.Where(x => x.Floor == floor.Floor).Distinct().ToList();
                            if (elevationDataForFloor.Any())
                            {
                                var floorNumber = Convert.ToString(floor.Floor).PadLeft(3, Convert.ToChar(Constants.ZERO));
                                unitDetailsList.Add(new ConfigVariable()
                                {
                                    VariableId = string.Format(productSelectionMapperVariables[Constants.ELEVATION], floorNumber),
                                    Value = elevationDataForFloor[0].ElevationFeet * 12 + elevationDataForFloor[0].ElevationInch
                                });
                                unitDetailsList.Add(new ConfigVariable()
                                {
                                    VariableId = string.Format(productSelectionMapperVariables[Constants.ENTF], floorNumber),
                                    Value = floor.Front? Constants.TRUEVALUES: Constants.FALSEVALUES
                                });
                                unitDetailsList.Add(new ConfigVariable()
                                {
                                    VariableId = string.Format(productSelectionMapperVariables[Constants.ENTR], floorNumber),
                                    Value = floor.Rear ? Constants.TRUEVALUES : Constants.FALSEVALUES
                                });
                            }
                        }
                    }
                    var frontOpening = elevationData.Where(x => x.Front).ToList().Count();
                    var rearOpenings = elevationData.Where(x => x.Rear).ToList().Count();
                    var noOfOpenings = new ConfigVariable()
                    {
                        VariableId = productSelectionMapperVariables[Constant.TotalNoOpenings],
                        Value = elevationData.Where(x => x.Front || x.Rear).ToList().Count()
                    };
                    unitDetailsList.Add(noOfOpenings);
                    var noOfFrontOpenings = new ConfigVariable()
                    {
                        VariableId = productSelectionMapperVariables[Constant.NUMBEROFFRONTOPENINGS],
                        Value = frontOpening
                    };
                    unitDetailsList.Add(noOfFrontOpenings);
                    var noOfRearOpenings = new ConfigVariable()
                    {
                        VariableId = productSelectionMapperVariables[Constant.NUMBEROFREAROPENINGS],
                        Value = rearOpenings
                    };
                    unitDetailsList.Add(noOfRearOpenings);
                    var noOfTotalOpening = new ConfigVariable()
                    {
                        VariableId = productSelectionMapperVariables[Constant.TOTALOPENING],
                        Value = frontOpening + rearOpenings
                    };
                    unitDetailsList.Add(noOfTotalOpening);
                    var unitList = (from DataRow row in dataSet.Tables[0].Rows
                                    select new { CounterWeightSafety = Convert.ToBoolean(row[Constant.OCCUPIEDSPACEBELOW]), unitId = Convert.ToInt32(row[Constant.UNITID]), travelFeet = row[Constant.TRAVELFEET] == DBNull.Value ? 0 : Convert.ToInt32(row[Constant.TRAVELFEET]), travelInch = row[Constant.TRAVELINCH] == DBNull.Value ? 0 : Convert.ToDecimal(row[Constant.TRAVELINCH]) }).ToList();
                    int elevatornumber = 1;
                    if (unitList != null && unitList.Any())
                    {
                        unitDetailsList.Add(new ConfigVariable()
                        {
                            VariableId = productSelectionMapperVariables[Constant.TRAVEL],
                            Value = Convert.ToDecimal(unitList[0].travelFeet * 12) + (unitList[0].travelInch)
                        });
                        unitDetailsList.Add(new ConfigVariable()
                        {
                            VariableId = productSelectionMapperVariables[Constant.COUNTERWEIGHTSAFETY],
                            Value = Convert.ToString(unitList[0].CounterWeightSafety).ToUpper()
                        });
                    }
                }
                if (dataSet.Tables[1].Rows.Count > 0)
                {
                    var variableList = (from DataRow row in dataSet.Tables[1].Rows
                                        select new ConfigVariable
                                        {
                                            VariableId = Convert.ToString(row[Constant.VARIABLEASSIGNMENT]),
                                            Value = Convert.ToString(row[Constant.VALUE])
                                        }).ToList();
                    unitDetailsList.AddRange(variableList);
                }
                if(dataSet.Tables[2].Rows.Count>0)
                {
                    var variableList = (from DataRow row in dataSet.Tables[2].Rows
                                        select new 
                                        {
                                            UnitId = Convert.ToInt32(row[Constants.UNITIDCOLUMN]),
                                            Value = Convert.ToString(row[Constants.SELECTEDDOOR])
                                        }).ToList();
                    foreach (var doorSelected in variableList)
                    {
                        if (!string.IsNullOrEmpty(doorSelected?.Value))
                        {
                            var doorValue = Utilities.DeserializeObjectValue<ConfigVariable>(doorSelected.Value);
                            doorValue.VariableId = doorValue.VariableId.Replace(doorValue.VariableId.Split(Constants.DOT)[0], Constants.ELEVATOR1);
                            unitDetailsList.Add(doorValue);
                        }
                    }
                }
                if (dataSet.Tables[4].Rows.Count > 0)
                {
                    var variableList = (from DataRow row in dataSet.Tables[4].Rows
                                        select new ConfigVariable
                                        {
                                            VariableId = Convert.ToString(row[Constant.BUINDINGTYPE]),
                                            Value = Convert.ToString(row[Constant.BUINDINGVALUE])
                                        }).ToList();
                    unitDetailsList.AddRange(variableList);
                }
                var variableAssignment = new ConfigVariable()
                {
                    VariableId = Constants.PRODUCTTYPE,
                    Value = string.Empty
                };
                if (dataSet.Tables[7].Rows.Count > 0)
                {
                    var variableList = (from DataRow row in dataSet.Tables[7].Rows
                                        select new
                                        {
                                            productName = Convert.ToString(row[Constants.PRODUCTNAMECOLUMN])
                                        }).ToList();
                    foreach (var product in variableList)
                    {
                        if (productTypeMapperVariables.ContainsKey(product.productName))
                        {
                            variableAssignment.Value = productTypeMapperVariables[product.productName];
                        }
                    }
                }
                unitDetailsList.Add(variableAssignment);
                variableAssignment = new ConfigVariable()
                {
                    VariableId = Constants.TRAVEL,
                    Value = false
                };
                variableAssignment = new ConfigVariable()
                {
                    VariableId = Constants.TRAVEL,
                    Value = false
                };
                var hydraulicFloorToFloorHeight = new ConfigVariable()
                {
                    VariableId = Constants.TRAVELHYDRAULIC,
                    Value = false
                };
                if (dataSet.Tables[8].Rows.Count > 0)
                {
                    var floorList = (from DataRow row in dataSet.Tables[8].Rows
                                        select new
                                        {
                                            FloorNumber = Convert.ToString(row[Constants.FLOORNUMBER]),
                                            Front = Convert.ToBoolean(row[Constants.FRONT]),
                                            Rear = Convert.ToBoolean(row[Constants.REAR]),
                                            TravelFeet = Convert.ToDecimal(row[Constants.FLOORTOFLOORHEIGHTFEET]),
                                            TravelInch = Convert.ToDecimal(row[Constants.FLOORTOFLOORHEIGHTINCH])
                                        }).ToList();
                    floorList = floorList.Where(x => x.Front || x.Rear).ToList();
                    foreach (var floor in floorList)
                    {
                        if (((floor.TravelFeet * 12) + floor.TravelInch) != 0 && ((floor.TravelFeet * 12) + floor.TravelInch) < 106)
                        {
                            variableAssignment.Value = true;
                        }
                        if (((floor.TravelFeet * 12) + floor.TravelInch) != 0 && ((floor.TravelFeet * 12) + floor.TravelInch) < 101)
                        {
                            hydraulicFloorToFloorHeight.Value = true;
                        }
                    }
                }
                unitDetailsList.Add(hydraulicFloorToFloorHeight);
                unitDetailsList.Add(variableAssignment);
                if (identifier.Equals(Constants.PRODUCTSELECTION))
                {
                    var carposVariable = dataSet.Tables[16];
                    if (carposVariable != null)
                    {
                        if (carposVariable.Rows.Count > 0)
                        {

                            var getunitGroupConsoleList = (from DataRow row in dataSet.Tables[16].Rows
                                                           select new
                                                           {
                                                               unitId = row["UnitId"].ToString(),
                                                               name = row["name"].ToString(),
                                                               MappedLocation = row["location"].ToString()
                                                           }).ToList();
                            var lowestUnitId = 0;

                            lowestUnitId = Convert.ToInt32((from openingData in getunitGroupConsoleList select openingData?.unitId)?.ToList()?.Min());

                            List<ConfigVariable> valD = new List<ConfigVariable>();
                            foreach (var opening in getunitGroupConsoleList)
                            {
                                if (opening.unitId.Equals(lowestUnitId.ToString()))
                                {
                                    ConfigVariable newVar = new ConfigVariable
                                    {
                                        VariableId = unitMapperVariables[Constants.CARPOSITIONVALFORVFD],
                                        Value = opening.MappedLocation
                                    };
                                    valD.Add(newVar);
                                }
                            }
                            unitDetailsList.AddRange(valD);
                        }
                    }
                    var controlLanding = dataSet.Tables[11];
                    if (controlLanding.Rows.Count > 0)
                    {
                        ConfigVariable newVar = new ConfigVariable()
                        {
                            VariableId = unitMapperVariables[Constants.CONTROLLERLANDINGVAL]
                        };
                        var variableAssignmentJsonString = string.Empty;
                        foreach (DataRow groupVariableAssignment in controlLanding.Rows)
                        {
                            if (!(Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON]) + Constant.COMA;
                            }
                        }
                        var jsonData =string.Concat(Constant.OPENINGSQUAREBRACKET, variableAssignmentJsonString,  Constant.CLOSINGSQUAREBRACKET);
                        var groupVariables = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(Constant.COMA + Constant.CLOSINGSQUAREBRACKET, Constant.CLOSINGSQUAREBRACKET).ToString());
                        groupVariables = groupVariables.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                        newVar.VariableId = unitMapperVariables[Constants.CONTROLLERLANDINGVALVFD];
                        newVar.Value = 0;
                        foreach (var controls in groupVariables)
                        {
                            if (controls.VariableId.Equals(unitMapperVariables[Constants.CONTROLROOMFLOOR]))
                            {
                                newVar.Value = controls.Value;
                            }
                        }
                        unitDetailsList.Add(newVar);
                    }
                    else
                    {
                        ConfigVariable newVar = new ConfigVariable
                        {
                            VariableId = unitMapperVariables[Constants.CONTROLLERLANDINGVALVFD],
                            Value = 0
                        };
                        unitDetailsList.Add(newVar);
                    }
                    var groupConfiguration = dataSet.Tables[11];
                    if (groupConfiguration.Rows.Count > 0)
                    {
                        if (dataSet.Tables[1].Rows.Count > 0)
                        {
                            var variableAssignmentJsonString = string.Empty;
                            foreach (DataRow groupVariableAssignment in groupConfiguration.Rows)
                            {
                                if (!(Convert.ToString(groupVariableAssignment[Constant.UNITJSON])).Equals(Constant.EMPTYSTRING))
                                {
                                    variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.UNITJSON]) + Constant.COMA;
                                }
                                if (!(Convert.ToString(groupVariableAssignment[Constant.HALLRISERJSON])).Equals(Constant.EMPTYSTRING))
                                {
                                    variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.HALLRISERJSON]) + Constant.COMA;
                                }
                                if (!(Convert.ToString(groupVariableAssignment[Constant.DOORJSON])).Equals(Constant.EMPTYSTRING))
                                {
                                    variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.DOORJSON]) + Constant.COMA;
                                }
                                if (!(Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON])).Equals(Constant.EMPTYSTRING))
                                {
                                    variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON]) + Constant.COMA;
                                }
                            }
                            var jsonData =string.Concat(Constant.OPENINGSQUAREBRACKET, variableAssignmentJsonString, Constant.CLOSINGSQUAREBRACKET);
                            var groupVariables = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(Constant.COMA + Constant.CLOSINGSQUAREBRACKET, Constant.CLOSINGSQUAREBRACKET).ToString());
                            groupVariables = groupVariables.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                            foreach (var variables in groupVariables)
                            {
                                if (variables.VariableId.Contains(productSelectionMapperVariables[Constants.SPPARAMETERS]) && !variables.VariableId.Contains(unitMapperVariables[Constants.CONTROLROOMVALUE]))
                                {
                                    variables.VariableId = Constants.ELEVATOR001 + variables.VariableId;
                                }
                                if (variables.VariableId.Contains(unitMapperVariables[Constants.CONTROLXVALUE]) || variables.VariableId.Contains(unitMapperVariables[Constants.CONTROLYVALUE]))
                                {
                                    variables.Value = Convert.ToDouble(variables.Value) * 12;
                                }
                            }
                            var controlLocation = groupVariables.Where(g => g.Value.Equals(Constants.CONTROLLOCATIONREMOTE) || g.Value.Equals(Constants.CONTROLLOCATIONADJACENT)).FirstOrDefault();
                            if (controlLocation != null && groupVariables.Where(g => g.VariableId.Contains(unitMapperVariables[Constants.CONTROLROOMVALUE])).Count() == 0)
                            {
                                if (controlLocation.Value.Equals(Constants.CONTROLLOCATIONREMOTE))
                                {
                                    groupVariables.Add(new ConfigVariable { VariableId = unitMapperVariables[Constants.CONTROLROOMQUADSPVALUES], Value = Constants.REMOTEDEFAULTVALUE });
                                }
                                if (controlLocation.Value.Equals(Constants.CONTROLLOCATIONADJACENT))
                                {
                                    groupVariables.Add(new ConfigVariable { VariableId = unitMapperVariables[Constants.CONTROLROOMQUADSPVALUES], Value = Constants.ADJACENTDEFAULTVALUE });
                                }
                            }
                            unitDetailsList.AddRange(groupVariables);
                        }
                    }
                }

            }
            else
            {
                return unitDetailsList;
            }
            Utility.LogEnd(methodBegin);
            return unitDetailsList;
        }



    }
}
