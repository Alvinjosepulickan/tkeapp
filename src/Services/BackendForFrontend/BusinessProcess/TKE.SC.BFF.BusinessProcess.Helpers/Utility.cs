using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.Common;

namespace TKE.SC.BFF.BusinessProcess.Helpers
{
    public class Utility : Utilities
    {
        /// <summary>
        /// ElevatorProducts
        /// </summary>
        public static List<string> ElevatorProducts = new List<string>(4)
            {
                 Constant.EVO_100
                ,Constant.EVO_200
                ,Constant.ENDURA_100
                ,Constant.ENDURA200
            };


        /// <summary>
        /// CustomEngineeredProducts
        /// </summary>
        public static List<string> CustomEngineeredProducts = new List<string>(4)
            {
                 Constant.CEGEARLESS
                ,Constant.CEGEARED
                ,Constant.CEHYDRAULIC
                ,Constant.SYNERGY
            };

        /// <summary>
        /// NonConfigurableProducts
        /// </summary>
        public static List<string> NonConfigurableProducts = new List<string>(3)
            {
                 Constant.ESCLATORMOVINGWALK
                ,Constant.TWINELEVATOR
                ,Constant.OTHER
            };

        /// <summary>
        /// HallStationConsoles
        /// </summary>
        public static List<string> HallStationConsoles = new List<string>(2)
        {
            Constant.TRADITIONALHALLSTATION,
            Constant.AGILEHALLSTATION
        };

        /// <summary>
        /// bank1CarPosition
        /// </summary>
        public static List<string> bank1CarPosition = new List<string>(4)
        {
            Constant.FDAU1,
            Constant.FDAU2,
            Constant.FDAU3,
            Constant.FDAU4
        };

        /// <summary>
        /// bank2CarPosition
        /// </summary>
        public static List<string> bank2CarPosition = new List<string>(4)
        {
            Constant.FDAU5,
            Constant.FDAU6,
            Constant.FDAU7,
            Constant.FDAU8
        };

        /// <summary>
        /// beamWallBank1
        /// </summary>
        public static List<string> beamWallBank1 = new List<string>(3)
        {
            Constant.FDAELEVATOR001,
            Constant.FDAELEVATOR002,
            Constant.FDAELEVATOR003
        };

        /// <summary>
        /// beamWallBank2
        /// </summary>
        public static List<string> beamWallBank2 = new List<string>(3)
        {
            Constant.FDAELEVATOR005,
            Constant.FDAELEVATOR006,
            Constant.FDAELEVATOR007
        };

        /// <summary>
        /// Serializes object to a string
        /// </summary>
        /// <param Name="value"></param>
        /// <returns></returns>
        public static string SerializeObjectValue(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }


        public static string SerializeObjectValuePascalCase(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        /// <summary>
        /// Deserialize string Value to the specified object
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="value"></param>
        /// <returns></returns>
        public static T DeserializeObjectValue<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// String comparision
        /// </summary>
        /// <param Name="valueOne"></param>
        /// <param Name="valueTwo"></param>
        /// <returns></returns>
        public static bool CheckEquals(string valueOne, string valueTwo)
        {
            return !string.IsNullOrEmpty(valueOne) && !string.IsNullOrEmpty(valueTwo) &&
                   valueOne.Trim().ToUpperInvariant() == valueTwo.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Removes null values from the object
        /// </summary>
        /// <param Name="obj"></param>
        /// <returns></returns>
        public static JObject FilterNullValues(object obj)
        {
            var filteredJsonString = JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            return JObject.Parse(filteredJsonString);
        }



        /// <summary>
        /// utility method for generating data table for saving and updating building elevation
        /// </summary>
        /// <param Name="buildingElevation"></param>
        /// <returns></returns>
        public static DataTable GetBuildingElevationDataTable(BuildingElevation buildingElevation)
        {
            DataTable elevationDataTable = new DataTable();
            elevationDataTable.Clear();

            DataColumn floorDesignation = new DataColumn("FloorDesignation")
            {
                DataType = typeof(string)
            };
            elevationDataTable.Columns.Add(floorDesignation);

            DataColumn elevationFeet = new DataColumn("elevationFeet")
            {
                DataType = typeof(int)
            };
            elevationDataTable.Columns.Add(elevationFeet);

            DataColumn elevationInch = new DataColumn("elevationInch")
            {
                DataType = typeof(decimal)
            };
            elevationDataTable.Columns.Add(elevationInch);

            DataColumn floorToFloorHeightFeet = new DataColumn("floorToFloorHeightFeet")
            {
                DataType = typeof(int)
            };
            elevationDataTable.Columns.Add(floorToFloorHeightFeet);

            DataColumn floorToFloorHeightInch = new DataColumn("floorToFloorHeightInch")
            {
                DataType = typeof(decimal)
            };
            elevationDataTable.Columns.Add(floorToFloorHeightInch);

            DataColumn buildingId = new DataColumn("BuildingId")
            {
                DataType = typeof(int),
                DefaultValue = buildingElevation.buildingConfigurationId
            };
            elevationDataTable.Columns.Add(buildingId);

            DataColumn mainEgress = new DataColumn("mainEgress")
            {
                DataType = typeof(Boolean)
            };
            elevationDataTable.Columns.Add(mainEgress);
            DataColumn createdBy = new DataColumn("userId")
            {
                DataType = typeof(string)
            };




            if (string.IsNullOrEmpty(buildingElevation.modifiedBy.UserId))
            {
                createdBy.DefaultValue = buildingElevation.createdBy.UserId;
            }
            else
            {
                createdBy.DefaultValue = buildingElevation.modifiedBy.UserId;
            }

            elevationDataTable.Columns.Add(createdBy);
            DataColumn createdOn = new DataColumn("CreatedOn")
            {
                DataType = typeof(DateTime),
                DefaultValue = DateTime.Now
            };
            elevationDataTable.Columns.Add(createdOn);

            DataColumn alternateEgress = new DataColumn("alternateEgress")
            {
                DataType = typeof(Boolean)
            };
            elevationDataTable.Columns.Add(alternateEgress);

            DataColumn noOfFloor = new DataColumn("noOfFloor")
            {
                DataType = typeof(int),
                DefaultValue = buildingElevation.noOFFloor
            };
            elevationDataTable.Columns.Add(noOfFloor);

            DataColumn buildingRise = new DataColumn("buildingRise")
            {
                DataType = typeof(decimal),
                DefaultValue = buildingElevation.buildingRiseValue
            };
            elevationDataTable.Columns.Add(buildingRise);

            DataColumn floorNumber = new DataColumn("FloorNumber");
            int floor = 0;
            floorNumber.DataType = typeof(int);
            elevationDataTable.Columns.Add(floorNumber);

            foreach (var buildingElev in buildingElevation.buildingElevation)
            {
                floor += 1;
                DataRow buildingElevationFloor = elevationDataTable.NewRow();
                buildingElevationFloor[0] = buildingElev.floorDesignation;
                buildingElevationFloor[12] = floor;
                buildingElevationFloor[6] = buildingElev.mainEgress ? 1 : 0;
                buildingElevationFloor[9] = buildingElev.alternateEgress ? 1 : 0;
                buildingElevationFloor[1] = buildingElev.elevation.feet;
                buildingElevationFloor[2] = buildingElev.elevation.inch;
                buildingElevationFloor[3] = buildingElev.floorToFloorHeight.feet;
                buildingElevationFloor[4] = buildingElev.floorToFloorHeight.inch;
                elevationDataTable.Rows.Add(buildingElevationFloor);

            }
            return elevationDataTable;
        }

        /// <summary>
        /// GetBuildingIdDataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetBuildingIDDataTable(List<int> buildingIDs)
        {
            DataTable buildingIDDataTable = new DataTable();
            buildingIDDataTable.Clear();

            DataColumn buildingId = new DataColumn("buildingID")
            {
                DataType = typeof(int)
            };
            buildingIDDataTable.Columns.Add(buildingId);
            foreach (var buildingID in buildingIDs)
            {
                DataRow buildingIdRow = buildingIDDataTable.NewRow();
                buildingIdRow[0] = buildingID;
                buildingIDDataTable.Rows.Add(buildingIdRow);

            }

            return buildingIDDataTable;
        }

        /// <summary>
        /// GetBuildingIdDataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetGroupIDDataTable(List<int> groupIDs)
        {
            DataTable groupIDDataTable = new DataTable();
            groupIDDataTable.Clear();

            DataColumn groupId = new DataColumn("UnitID")
            {
                DataType = typeof(int)
            };
            groupIDDataTable.Columns.Add(groupId);
            foreach (var groupID in groupIDs)
            {
                DataRow groupIdRow = groupIDDataTable.NewRow();
                groupIdRow[0] = groupID;
                groupIDDataTable.Rows.Add(groupIdRow);

            }

            return groupIDDataTable;
        }


        public static DataTable GetCarPositionDataTable(List<CarPosition> carPositionList)
        {
            DataTable CarPositionDataTable = new DataTable();
            CarPositionDataTable.Clear();

            DataColumn CarPositionID = new DataColumn("CarPositionID")
            {
                DataType = typeof(int)
            };
            CarPositionDataTable.Columns.Add(CarPositionID);

            DataColumn CarPosition = new DataColumn("CarPositionLocation")
            {
                DataType = typeof(string)
            };
            CarPositionDataTable.Columns.Add(CarPosition);

            DataColumn UnitDesignation = new DataColumn("UnitDesignation")
            {
                DataType = typeof(string)
            };
            CarPositionDataTable.Columns.Add(UnitDesignation);

            DataColumn UnitName = new DataColumn("UnitName")
            {
                DataType = typeof(string)
            };
            CarPositionDataTable.Columns.Add(UnitName);
            int id = 1;           
            var unitNames = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.UNITTABLE];
            var unitNameDictionary = unitNames.ToObject<Dictionary<string, string>>();
            foreach (var carPosn in carPositionList)
            {
                DataRow CarPositions = CarPositionDataTable.NewRow();
                CarPositions[0] = id;
                var positionVariable = carPosn.Position.Split(Constant.DOT);
                CarPositions[1] = positionVariable[positionVariable.Count() - 1];
                if (string.IsNullOrEmpty(carPosn.UnitDesignation))
                {
                    carPosn.UnitDesignation = string.Empty;
                }
                CarPositions[2] = carPosn.UnitDesignation;
                CarPositions[3] = unitNameDictionary[positionVariable[positionVariable.Count() - 1]];
                CarPositionDataTable.Rows.Add(CarPositions);
                id += 1;
            }

            return CarPositionDataTable;
        }       

        /// <summary>
        /// GetLandingOpeningAssignmentSelected
        /// </summary>
        /// <param Name="varEntrance"></param>
        /// <returns></returns>
        public static string GetLandingOpeningAssignmentSelected(List<EntranceLocations> varEntrance)
        {
            var frontvariable = new LandingOpening()
            {
                Value = false
            };
            varEntrance.Where(x => x.Front == null).ToList().ForEach(y => y.Front = frontvariable);
            varEntrance.Where(x => x.Rear == null).ToList().ForEach(y => y.Rear = frontvariable);
            var FrontRearAssignment = "";
            //logic to fetch assigned opening for Entrance Console Card
            var lstFrontLanding = (from location in varEntrance
                                   where location.Front.Value.Equals(true)
                                   orderby location.FloorNumber ascending
                                   select location.FloorNumber).ToList();
            var lstRearLanding = (from location in varEntrance
                                  where location.Rear.Value.Equals(true)
                                  orderby location.FloorNumber ascending
                                  select location.FloorNumber).ToList();

            var FloorNumber = lstFrontLanding.Count > 0 ? lstFrontLanding[0] : 1;
            var FrontAssignement = "";
            var RearAssignment = "";

            if (lstFrontLanding.Count == 1)
            {
                FrontAssignement = "F - " + lstFrontLanding[0];
            }
            else
            {
                for (var index = 1; index < lstFrontLanding.Count; index++)
                {
                    if (lstFrontLanding[index] - lstFrontLanding[index - 1] != 1)
                    {
                        if (lstFrontLanding[index - 1] != FloorNumber)
                        {
                            FrontAssignement += FrontAssignement.Equals("") ? FloorNumber + "-" + lstFrontLanding[index - 1] : ", " + FloorNumber + "-" + lstFrontLanding[index - 1];

                        }
                        else
                        {
                            FrontAssignement += FrontAssignement.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                        }
                        FloorNumber = lstFrontLanding[index];

                    }
                    if (index == lstFrontLanding.Count - 1)
                    {
                        if (lstFrontLanding[index] != FloorNumber)
                        {
                            FrontAssignement += FrontAssignement.Equals("") ? FloorNumber + "-" + lstFrontLanding[index] : ", " + FloorNumber + "-" + lstFrontLanding[index];

                        }
                        else
                        {
                            FrontAssignement += FrontAssignement.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                        }
                        FrontAssignement = "F - " + FrontAssignement;

                    }
                }
            }
            FloorNumber = lstRearLanding.Count > 0 ? lstRearLanding[0] : 1;
            if (lstRearLanding.Count == 1)
            {
                RearAssignment = "R - " + lstRearLanding[0];
            }
            else
            {
                for (var index = 1; index < lstRearLanding.Count; index++)
                {

                    if (lstRearLanding[index] - lstRearLanding[index - 1] != 1)
                    {
                        if (lstRearLanding[index - 1] != FloorNumber)
                        {
                            RearAssignment += RearAssignment.Equals("") ? FloorNumber + "-" + lstRearLanding[index - 1] : ", " + FloorNumber + "-" + lstRearLanding[index - 1];

                        }
                        else
                        {
                            RearAssignment += RearAssignment.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                        }

                        FloorNumber = lstRearLanding[index];

                    }
                    if (index == lstRearLanding.Count - 1)
                    {
                        if (lstRearLanding[index] != FloorNumber)
                        {
                            RearAssignment += RearAssignment.Equals("") ? FloorNumber + "-" + lstRearLanding[index] : ", " + FloorNumber + "-" + lstRearLanding[index];

                        }
                        else
                        {
                            RearAssignment += RearAssignment.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                        }
                        RearAssignment = "R - " + RearAssignment;

                    }

                }
            }


            FrontRearAssignment = lstFrontLanding.Count == 0 ? lstRearLanding.Count == 0 ? "" : RearAssignment : lstRearLanding.Count == 0 ? FrontAssignement :
                                 FrontAssignement + " | " + RearAssignment;




            return FrontRearAssignment;

        }

        /// <summary>
        /// GetFilteredSection
        /// </summary>
        /// <param Name="modelResponse"></param>
        /// <param Name="stubResponse"></param>
        /// <returns></returns>
        public static IList<Sections> GetFilteredSection(IList<Sections> modelResponse, IList<Sections> stubResponse)
        {
            foreach (var modelSection in modelResponse)
            {
                foreach (var stubSection in stubResponse)
                {
                    if (Utility.CheckEquals(modelSection.Id, stubSection.Id))
                    {
                        //filtering variables in each section
                        var filteredVariables = (from mainSubVariableValues in modelSection.Variables
                                                 from localSubVariableValues in stubSection.Variables
                                                 where mainSubVariableValues.Id.ToUpper() == localSubVariableValues.Id.ToUpper()
                                                 select mainSubVariableValues).ToList();
                        modelSection.Variables = filteredVariables;
                        if (filteredVariables != null && filteredVariables.Any())
                        {

                            foreach (var modelVariableItems in modelSection.Variables)
                            {
                                //Assigning local properties to the model response

                                var localPropertyValues = stubSection.Variables.Where(x => x.Id.ToUpper().Equals(modelVariableItems.Id.ToUpper())).ToList();

                                if (localPropertyValues.Count > 0)
                                {
                                    modelVariableItems.Properties = localPropertyValues[0].Properties;
                                }
                                foreach (var variableItemsValues in modelVariableItems.Values)
                                {
                                    if (variableItemsValues.value != null && (variableItemsValues.value.Equals(Constant.TRUEVALUES) || variableItemsValues.value.Equals(Constant.True)))
                                    {
                                        variableItemsValues.value = true;
                                    }
                                    else if (variableItemsValues.value != null && (variableItemsValues.value.Equals(Constant.FALSEVALUES) || variableItemsValues.value.Equals(Constant.False)))
                                    {
                                        variableItemsValues.value = false;
                                    }
                                }
                            }
                        }

                        //Filtering sections inside the sections
                        var filteredSection = (from mainSubVariableValues in modelSection.sections
                                               from localSubVariableValues in stubSection.sections
                                               where mainSubVariableValues.Id.ToUpper() == localSubVariableValues.Id.ToUpper()
                                               select mainSubVariableValues).ToList();
                        if (filteredSection != null && filteredSection.Any())
                        {
                            modelSection.sections = GetFilteredSectionValues(filteredSection, stubSection.sections);
                        }
                    }

                }
            }
            return modelResponse;
        }
        /// <summary>
        /// GetFilteredSectionValues
        /// </summary>
        /// <param Name="modelResponse"></param>
        /// <param Name="stubResponse"></param>
        /// <returns></returns>
        public static IList<SectionsValues> GetFilteredSectionValues(IList<SectionsValues> modelResponse, IList<SectionsValues> stubResponse)
        {
            foreach (var modelSection in modelResponse)
            {
                foreach (var stubSection in stubResponse)
                {
                    if (Utility.CheckEquals(modelSection.Id, stubSection.Id))
                    {
                        //filtering variables in each section
                        var filteredVariables = (from mainSubVariableValues in modelSection.Variables
                                                 from localSubVariableValues in stubSection.Variables
                                                 where mainSubVariableValues.Id.ToUpper() == localSubVariableValues.Id.ToUpper()
                                                 select mainSubVariableValues).ToList();

                        modelSection.Variables = filteredVariables;
                        if (filteredVariables != null && filteredVariables.Any())
                        {
                            foreach (var modelVariableItems in modelSection.Variables)
                            {
                                //Assigning local properties to the model response
                                var localPropertyValues = stubSection.Variables.Where(x => x.Id.ToUpper().Equals(modelVariableItems.Id.ToUpper())).ToList();
                                if (localPropertyValues.Count > 0)
                                {
                                    modelVariableItems.Properties = localPropertyValues[0].Properties;
                                }
                                foreach (var variableItemsValues in modelVariableItems.Values)
                                {
                                    if (variableItemsValues.value != null && (variableItemsValues.value.Equals(Constant.TRUEVALUES) || variableItemsValues.value.Equals(Constant.True)))
                                    {
                                        variableItemsValues.value = true;
                                    }
                                    else if (variableItemsValues.value != null && (variableItemsValues.value.Equals(Constant.FALSEVALUES) || variableItemsValues.value.Equals(Constant.False)))
                                    {
                                        variableItemsValues.value = false;
                                    }
                                }
                            }
                        }

                        //Filtering sections inside the sections
                        var filteredSection = (from mainSubVariableValues in modelSection.sections
                                               from localSubVariableValues in stubSection.sections
                                               where mainSubVariableValues.Id.ToUpper() == localSubVariableValues.Id.ToUpper()
                                               select mainSubVariableValues).ToList();
                        if (filteredSection != null && filteredSection.Any())
                        {
                            modelSection.sections = GetFilteredSectionGroupValues(filteredSection, stubSection.sections);
                        }
                    }

                }
            }
            return modelResponse;
        }
        /// <summary>
        /// GetFilteredSectionGroupValues
        /// </summary>
        /// <param Name="modelResponse"></param>
        /// <param Name="stubResponse"></param>
        /// <returns></returns>
        public static IList<SectionsGroupValues> GetFilteredSectionGroupValues(IList<SectionsGroupValues> modelResponse, IList<SectionsGroupValues> stubResponse)
        {
            foreach (var modelSection in modelResponse)
            {
                foreach (var stubSection in stubResponse)
                {
                    if (Utility.CheckEquals(modelSection.Id, stubSection.Id))
                    {
                        //filtering variables in each section
                        var filteredVariables = (from mainSubVariableValues in modelSection.Variables
                                                 from localSubVariableValues in stubSection.Variables
                                                 where mainSubVariableValues.Id.ToUpper() == localSubVariableValues.Id.ToUpper()
                                                 select mainSubVariableValues).ToList();

                        modelSection.Variables = filteredVariables;
                        if (filteredVariables != null && filteredVariables.Any())
                        {
                            foreach (var modelVariableItems in modelSection.Variables)
                            {
                                //Assigning local properties to the model response
                                var localPropertyValues = stubSection.Variables.Where(x => x.Id.ToUpper().Equals(modelVariableItems.Id.ToUpper())).ToList();
                                if (localPropertyValues.Count > 0)
                                {
                                    modelVariableItems.Properties = localPropertyValues[0].Properties;
                                }
                                foreach (var variableItemsValues in modelVariableItems.Values)
                                {
                                    if (variableItemsValues.value != null && (variableItemsValues.value.Equals(Constant.TRUEVALUES) || variableItemsValues.value.Equals(Constant.True)))
                                    {
                                        variableItemsValues.value = true;
                                    }
                                    else if (variableItemsValues.value != null && (variableItemsValues.value.Equals(Constant.FALSEVALUES) || variableItemsValues.value.Equals(Constant.False)))
                                    {
                                        variableItemsValues.value = false;
                                    }
                                }
                            }
                        }

                        //Filtering sections inside the sections
                        var filteredSection = (from mainSubVariableValues in modelSection.sections
                                               from localSubVariableValues in stubSection.sections
                                               where mainSubVariableValues.Id.ToUpper() == localSubVariableValues.Id.ToUpper()
                                               select mainSubVariableValues).ToList();
                        if (filteredSection != null && filteredSection.Any())
                        {
                            modelSection.sections = GetFilteredSectionValues(filteredSection, stubSection.sections);
                        }
                    }

                }
            }
            return modelResponse;
        }

        public static List<Variables> GetAllSectionVariables(IList<Sections> modelSections)
        {
            List<Variables> lstVariables = new List<Variables>();
            foreach (var section in modelSections)
            {
                if (section.Variables != null)
                {
                    lstVariables.AddRange(section.Variables);
                }

                if (section.sections != null)
                {
                    if (section.sections.Count > 0)
                    {
                        var sectionValues = (List<SectionsValues>)section.sections;
                        lstVariables.AddRange(GetAllSectionValuesVariables(sectionValues));
                    }
                }
            }
            return lstVariables;
        }



        public static List<Variables> GetAllSectionVariables1(IList<Sections> modelSections)
        {
            List<Variables> lstVariables = new List<Variables>();
            foreach (var section in modelSections)
            {
                lstVariables.AddRange(section.Variables);
                if (section.sections != null)
                {
                    if (section.sections.Count > 0)
                    {
                        var sectionValues = (List<SectionsValues>)section.sections;
                        lstVariables.AddRange(GetAllSectionValuesVariables(sectionValues));
                    }
                }
            }
            return lstVariables;
        }




        public static List<Variables> GetAllSectionValuesVariables(List<SectionsValues> modelSections)
        {
            List<Variables> lstVariables = new List<Variables>();
            foreach (var section in modelSections)
            {
                lstVariables.AddRange(section.Variables);
                if (section.sections != null)
                {
                    if (section.sections.Count > 0)
                    {
                        var sectionValues = (List<SectionsGroupValues>)section.sections;
                        lstVariables.AddRange(GetAllSectionGroupValuesVariables(sectionValues));
                    }
                }
            }
            return lstVariables;
        }
        public static List<Variables> GetAllSectionGroupValuesVariables(List<SectionsGroupValues> modelSections)
        {
            List<Variables> lstVariables = new List<Variables>();
            foreach (var section in modelSections)
            {
                lstVariables.AddRange(section.Variables);
                if (section.sections.Count > 0)
                {
                    var sectionValues = (List<SectionsValues>)section.sections;
                    lstVariables.AddRange(GetAllSectionValuesVariables(sectionValues));
                }
            }
            return lstVariables;
        }

        public static List<Variables> GetUnitHallFixtureVariables()
        {
            var stubResponse = JObject.Parse(File.ReadAllText(Constant.UNITHALLFIXTUREPATH));
            var lstVariables = new List<Variables>();
            var stubUnitConfigurationResponseObj = stubResponse.ToObject<ConfigurationResponse>();

            foreach (var subSection in stubUnitConfigurationResponseObj.Sections)
            {
                if (subSection.Variables.Count > 0)
                {
                    lstVariables.AddRange(subSection.Variables);
                }
                if (subSection.sections != null && subSection.sections.Any())
                {
                    foreach (var subSection1 in subSection.sections)
                    {
                        if (subSection1.Variables.Count > 0)
                        {
                            lstVariables.AddRange(subSection1.Variables);
                        }
                        if (subSection1.sections != null && subSection1.sections.Any())
                        {
                            foreach (var subSection2 in subSection1.sections)
                            {
                                if (subSection2.Variables.Count > 0)
                                {
                                    lstVariables.AddRange(subSection2.Variables);
                                }
                                if (subSection2.sections != null && subSection2.sections.Any())
                                {
                                    foreach (var subSection3 in subSection2.sections)
                                    {
                                        if (subSection3.Variables.Count > 0)
                                        {
                                            lstVariables.AddRange(subSection3.Variables);
                                        }
                                        if (subSection3.sections != null && subSection3.sections.Any())
                                        {
                                            foreach (var subSection4 in subSection3.sections)
                                            {
                                                if (subSection4.Variables.Count > 0)
                                                {
                                                    lstVariables.AddRange(subSection4.Variables);
                                                }
                                                if (subSection4.sections != null && subSection4.sections.Any())
                                                {
                                                    foreach (var subSection5 in subSection4.sections)
                                                    {
                                                        if (subSection5.Variables.Count > 0)
                                                        {
                                                            lstVariables.AddRange(subSection5.Variables);
                                                        }
                                                        if (subSection5.sections != null && subSection5.sections.Any())
                                                        {
                                                            foreach (var subSection6 in subSection5.sections)
                                                            {
                                                                if (subSection6.Variables.Count > 0)
                                                                {
                                                                    lstVariables.AddRange(subSection6.Variables);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lstVariables;
        }

        public static string GetLandingOpeningAssignmentSelectedForUnitHallFixture(List<EntranceLocations> varUnitHallFixtures)
        {
            var frontvariable = new LandingOpening()
            {
                Value = false
            };
            varUnitHallFixtures.Where(x => x.Front == null).ToList().ForEach(y => y.Front = frontvariable);
            varUnitHallFixtures.Where(x => x.Rear == null).ToList().ForEach(y => y.Rear = frontvariable);
            var FrontRearAssignment = "";
            //logic to fetch assigned opening for Entrance Console Card
            var lstFrontLanding = (from location in varUnitHallFixtures
                                   where location.Front.Value.Equals(true)
                                   orderby location.FloorNumber ascending
                                   select location.FloorNumber).ToList();
            var lstRearLanding = (from location in varUnitHallFixtures
                                  where location.Rear.Value.Equals(true)
                                  orderby location.FloorNumber ascending
                                  select location.FloorNumber).ToList();
            var FloorNumber = lstFrontLanding.Count > 0 ? lstFrontLanding[0] : 1;
            var FrontAssignement = "";
            var RearAssignment = "";

            if (lstFrontLanding.Count == 1)
            {
                FrontAssignement = "F - " + lstFrontLanding[0];
            }
            else
            {
                for (var index = 1; index < lstFrontLanding.Count; index++)
                {
                    if (lstFrontLanding[index] - lstFrontLanding[index - 1] != 1)
                    {
                        if (lstFrontLanding[index - 1] != FloorNumber)
                        {
                            FrontAssignement += FrontAssignement.Equals("") ? FloorNumber + "-" + lstFrontLanding[index - 1] : ", " + FloorNumber + "-" + lstFrontLanding[index - 1];

                        }
                        else
                        {
                            FrontAssignement += FrontAssignement.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                        }
                        FloorNumber = lstFrontLanding[index];

                    }
                    if (index == lstFrontLanding.Count - 1)
                    {
                        if (lstFrontLanding[index] != FloorNumber)
                        {
                            FrontAssignement += FrontAssignement.Equals("") ? FloorNumber + "-" + lstFrontLanding[index] : ", " + FloorNumber + "-" + lstFrontLanding[index];

                        }
                        else
                        {
                            FrontAssignement += FrontAssignement.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                        }
                        FrontAssignement = "F - " + FrontAssignement;

                    }
                }
            }
            FloorNumber = lstRearLanding.Count > 0 ? lstRearLanding[0] : 1;
            if (lstRearLanding.Count == 1)
            {
                RearAssignment = "R - " + lstRearLanding[0];
            }
            else
            {
                for (var index = 1; index < lstRearLanding.Count; index++)
                {

                    if (lstRearLanding[index] - lstRearLanding[index - 1] != 1)
                    {
                        if (lstRearLanding[index - 1] != FloorNumber)
                        {
                            RearAssignment += RearAssignment.Equals("") ? FloorNumber + "-" + lstRearLanding[index - 1] : ", " + FloorNumber + "-" + lstRearLanding[index - 1];

                        }
                        else
                        {
                            RearAssignment += RearAssignment.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                        }

                        FloorNumber = lstRearLanding[index];

                    }
                    if (index == lstRearLanding.Count - 1)
                    {
                        if (lstRearLanding[index] != FloorNumber)
                        {
                            RearAssignment += RearAssignment.Equals("") ? FloorNumber + "-" + lstRearLanding[index] : ", " + FloorNumber + "-" + lstRearLanding[index];

                        }
                        else
                        {
                            RearAssignment += RearAssignment.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                        }
                        RearAssignment = "R - " + RearAssignment;
                    }

                }
            }


            FrontRearAssignment = lstFrontLanding.Count == 0 ? lstRearLanding.Count == 0 ? "" : RearAssignment : lstRearLanding.Count == 0 ? FrontAssignement :
                                 FrontAssignement + " | " + RearAssignment;

            return FrontRearAssignment;
        }


        public static string GetLandingOpeningAssignmentSelectedForGroupHallFixture(List<UnitDetailsValues> groupUnitHallFixturesOpeningValues)
        {

            var FrontRearUnitAssignment = "";
            foreach (var groupHallFixturesOpeningValues in groupUnitHallFixturesOpeningValues)
            {
                var frontvariable = new LandingOpening()
                {
                    Value = false
                };
                if (groupHallFixturesOpeningValues.UnitGroupValues == null)
                {
                    groupHallFixturesOpeningValues.UnitGroupValues = new List<GroupHallFixtureLocations>();
                }
                groupHallFixturesOpeningValues.UnitGroupValues.Where(x => x.Front == null).ToList().ForEach(y => y.Front = frontvariable);
                groupHallFixturesOpeningValues.UnitGroupValues.Where(x => x.Rear == null).ToList().ForEach(y => y.Rear = frontvariable);
                var FrontRearAssignment = "";
                //logic to fetch assigned opening for Entrance Console Card
                var lstFrontLanding = (from location in groupHallFixturesOpeningValues.UnitGroupValues
                                       where location.Front.Value.Equals(true)
                                       orderby location.FloorNumber ascending
                                       select location.FloorNumber).ToList();
                var lstRearLanding = (from location in groupHallFixturesOpeningValues.UnitGroupValues
                                      where location.Rear.Value.Equals(true)
                                      orderby location.FloorNumber ascending
                                      select location.FloorNumber).ToList();
                var FloorNumber = lstFrontLanding.Count > 0 ? lstFrontLanding[0] : 1;
                var FrontAssignement = "";
                var RearAssignment = "";

                if (lstFrontLanding.Count == 1)
                {
                    FrontAssignement = groupHallFixturesOpeningValues.UniDesgination + Constant.COLON + Constant.EMPTYSPACE + "F - " + lstFrontLanding[0];
                }
                else
                {
                    for (var index = 1; index < lstFrontLanding.Count; index++)
                    {
                        if (lstFrontLanding[index] - lstFrontLanding[index - 1] != 1)
                        {
                            if (lstFrontLanding[index - 1] != FloorNumber)
                            {
                                FrontAssignement += FrontAssignement.Equals("") ? FloorNumber + "-" + lstFrontLanding[index - 1] : ", " + FloorNumber + "-" + lstFrontLanding[index - 1];

                            }
                            else
                            {
                                FrontAssignement += FrontAssignement.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                            }
                            FloorNumber = lstFrontLanding[index];

                        }
                        if (index == lstFrontLanding.Count - 1)
                        {
                            if (lstFrontLanding[index] != FloorNumber)
                            {
                                FrontAssignement += FrontAssignement.Equals("") ? FloorNumber + "-" + lstFrontLanding[index] : ", " + FloorNumber + "-" + lstFrontLanding[index];

                            }
                            else
                            {
                                FrontAssignement += FrontAssignement.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                            }
                            FrontAssignement = groupHallFixturesOpeningValues.UniDesgination + Constant.COLON + Constant.EMPTYSPACE + "F - " + FrontAssignement;

                        }
                    }
                }
                FloorNumber = lstRearLanding.Count > 0 ? lstRearLanding[0] : 1;
                if (lstRearLanding.Count == 1)
                {
                    RearAssignment = groupHallFixturesOpeningValues.UniDesgination + Constant.COLON + Constant.EMPTYSPACE + "R - " + lstRearLanding[0];
                }
                else
                {
                    for (var index = 1; index < lstRearLanding.Count; index++)
                    {

                        if (lstRearLanding[index] - lstRearLanding[index - 1] != 1)
                        {
                            if (lstRearLanding[index - 1] != FloorNumber)
                            {
                                RearAssignment += RearAssignment.Equals("") ? FloorNumber + "-" + lstRearLanding[index - 1] : ", " + FloorNumber + "-" + lstRearLanding[index - 1];

                            }
                            else
                            {
                                RearAssignment += RearAssignment.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                            }

                            FloorNumber = lstRearLanding[index];

                        }
                        if (index == lstRearLanding.Count - 1)
                        {
                            if (lstRearLanding[index] != FloorNumber)
                            {
                                RearAssignment += RearAssignment.Equals("") ? FloorNumber + "-" + lstRearLanding[index] : ", " + FloorNumber + "-" + lstRearLanding[index];

                            }
                            else
                            {
                                RearAssignment += RearAssignment.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                            }
                            RearAssignment = groupHallFixturesOpeningValues.UniDesgination + Constant.COLON + Constant.EMPTYSPACE + "R - " + RearAssignment;
                        }

                    }
                }


                FrontRearAssignment = lstFrontLanding.Count == 0 ? lstRearLanding.Count == 0 ? "" : RearAssignment : lstRearLanding.Count == 0 ? FrontAssignement :
                                     FrontAssignement + " | " + RearAssignment;
                if (!string.IsNullOrEmpty(FrontRearAssignment))
                {
                    FrontRearUnitAssignment = string.IsNullOrEmpty(FrontRearUnitAssignment) ? FrontRearAssignment : FrontRearUnitAssignment + ", " + FrontRearAssignment;

                }
            }


            return FrontRearUnitAssignment;
        }

        public static ConfigurationResponse AssignUnitDesignation(ConfigurationResponse stubGroupConfigurationResponseObj, List<DisplayVariableAssignmentsValues> displayVariablesValuesResponse)
        {
            foreach (var variable in displayVariablesValuesResponse)
            {
                foreach (var sectionss in stubGroupConfigurationResponseObj.Sections)
                {
                    if (CheckEquals(sectionss.Id, Constant.GROUPVALIDATION))
                    {
                        foreach (var sections in sectionss.sections)
                        {
                            if (CheckEquals(sections.Id, Constant.PARAMETERSVALUES))
                            {
                                foreach (var sec in sections.sections)
                                {
                                    if (CheckEquals(sec.Id, Constant.PARAMETERSVALUES))
                                    {
                                        foreach (var variables in sec.Variables)
                                        {
                                            if (CheckEquals(variables.Id, variable.VariableId))
                                            {
                                                foreach (var property in variables.Properties)
                                                {
                                                    if (CheckEquals(property.Id, Constant.UNITDESIGNATION))
                                                    {
                                                        property.Value = variable.UnitDesignation;
                                                    }
                                                    else if (CheckEquals(property.Id, Constant.ISFUTUREELEVATOR))
                                                    {
                                                        property.Value = variable.IsFutureElevator;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return stubGroupConfigurationResponseObj;
        }

        public static List<UnitDetailsValues> CheckFixtureAssigned(List<List<UnitDetailsValues>> lstSelectedFrontFloorNumber, List<UnitDetailsValues> newGroupLocation)
        {
            if (lstSelectedFrontFloorNumber.Count > 0)
            {
                foreach (var floorList in lstSelectedFrontFloorNumber)
                {
                    if (floorList != null)
                    {
                        if (floorList.Count > 0)
                        {
                            foreach (var unit in floorList)
                            {
                                if (unit != null)
                                {
                                    foreach (var groupUnit in unit.UnitGroupValues)
                                    {
                                        if (groupUnit != null)
                                        {
                                            foreach (var units in newGroupLocation)
                                            {
                                                if (units != null)
                                                {
                                                    if (unit.UniDesgination != null && units.unitName != null)
                                                    {
                                                        if (unit.UniDesgination == units.unitName)
                                                        {
                                                            foreach (var unitInUnits in units.openingsAssigned)
                                                            {
                                                                if (unitInUnits != null)
                                                                {
                                                                    if (groupUnit.FloorDesignation != null && unitInUnits.FloorDesignation != null)
                                                                    {
                                                                        unitInUnits.Front.InCompatible = groupUnit.FloorDesignation == unitInUnits.FloorDesignation && Convert.ToBoolean(groupUnit.Front.Value);
                                                                        unitInUnits.Rear.InCompatible = groupUnit.FloorDesignation == unitInUnits.FloorDesignation && Convert.ToBoolean(groupUnit.Rear.Value);

                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return newGroupLocation;
        }

        /// <summary>
        /// Method to convert Decimal to Fraction
        /// </summary>
        /// <param Name="dblDecimal"></param>
        /// <param Name="type"></param>
        /// <returns></returns>
        public static string ConvertDecimalToFraction(double dblDecimal, string type = "")
        {
            string txtFraction = string.Empty;
            int accuracy = 64;
            int whole = 0;
            int numerator = 0;
            int denominator = 1;
            double dblAccuracy = accuracy;
            whole = (int)(Math.Truncate(dblDecimal));
            var fraction = Math.Abs(dblDecimal - whole);
            if (fraction == 0)
            {
                if (type.Equals(Constant.BUILDINGRISE))
                {
                    int feet = whole / 12;
                    whole %= 12;
                    txtFraction = feet + " ft " + whole + " in";
                }
                else
                {
                    txtFraction = whole.ToString();
                }
                return txtFraction;
            }
            var n = Enumerable.Range(0, accuracy + 1).SkipWhile(e => (e / dblAccuracy) < fraction).First();
            var hi = n / dblAccuracy;
            var lo = (n - 1) / dblAccuracy;
            if ((fraction - lo) < (hi - fraction))
            {
                n--;
            }

            if (n == accuracy)
            {
                return "";
            }
            var gcd = GCD(n, accuracy);
            numerator = n / gcd;
            denominator = accuracy / gcd;
            if (type.Equals(Constant.BUILDINGRISE))
            {
                int feet = whole / 12;
                whole %= 12;
                txtFraction = feet + " ft ";
                txtFraction += whole > 0 ? whole + " " + numerator + "/" + denominator + " in" : numerator + "/" + denominator + " in";
            }
            else
            {
                txtFraction = whole > 0 ? whole + " " + numerator + "/" + denominator : numerator + "/" + denominator;
            }
            return txtFraction;

        }
        /// <summary>
        /// method to get Greates Common Divisor
        /// </summary>
        /// <param Name="a"></param>
        /// <param Name="b"></param>
        /// <returns></returns>
        public static int GCD(int value1, int value2)
        {
            if (value2 == 0)
            {
                return value1;
            }
            else
            {
                return GCD(value2, value1 % value2);
            }
        }

        public static string GetPropertyValue(IConfiguration section, string propertyName)
        {
            if (GetSection(section, propertyName) is null)
            {
                return string.Empty;
            }
            return section.GetSection(propertyName).Value;
        }


        public static IConfiguration GetSection(IConfiguration section, string propertyName)
        {
            var subSection = section.GetSection(propertyName);
            if (subSection is null)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = 500,
                    Message = $"No settings found for key:{propertyName}",
                    Description = $"No settings found for key:{propertyName}"
                });
            }
            return subSection;
        }

        public static string GetOpeningForHallStation(List<HallStations> HallStationOpenings)
        {

            var FrontRearUnitAssignment = string.Empty;
            foreach (var groupHallFixturesOpeningValues in HallStationOpenings)
            {
                var frontvariable = new LandingOpening()
                {
                    Value = false
                };
                if (groupHallFixturesOpeningValues.openingsAssigned == null)
                {
                    groupHallFixturesOpeningValues.openingsAssigned = new List<GroupHallFixtureLocations>();
                }
                groupHallFixturesOpeningValues.openingsAssigned.Where(x => x.Front == null).ToList().ForEach(y => y.Front = frontvariable);
                groupHallFixturesOpeningValues.openingsAssigned.Where(x => x.Rear == null).ToList().ForEach(y => y.Rear = frontvariable);
                var FrontRearAssignment = string.Empty;
                //logic to fetch assigned opening for Entrance Console Card
                var lstFrontLanding = (from location in groupHallFixturesOpeningValues.openingsAssigned
                                       where location.Front.Value.Equals(true)
                                       orderby location.FloorNumber ascending
                                       select location.FloorNumber).ToList();
                var lstRearLanding = (from location in groupHallFixturesOpeningValues.openingsAssigned
                                      where location.Rear.Value.Equals(true)
                                      orderby location.FloorNumber ascending
                                      select location.FloorNumber).ToList();
                var FloorNumber = lstFrontLanding.Count > 0 ? lstFrontLanding[0] : 1;
                var FrontAssignement = string.Empty;
                var RearAssignment = string.Empty;

                if (lstFrontLanding.Count == 1)
                {
                    FrontAssignement = groupHallFixturesOpeningValues.HallStationName + Constant.COLON + Constant.EMPTYSPACE + Constant.STRING_F + lstFrontLanding[0];
                }
                else
                {
                    for (var index = 1; index < lstFrontLanding.Count; index++)
                    {
                        if (lstFrontLanding[index] - lstFrontLanding[index - 1] != 1)
                        {
                            if (lstFrontLanding[index - 1] != FloorNumber)
                            {
                                FrontAssignement += FrontAssignement.Equals(string.Empty) ? FloorNumber + Constant.HYPHEN + lstFrontLanding[index - 1] : Constant.SPACE_COMMA + FloorNumber + Constant.HYPHEN + lstFrontLanding[index - 1];

                            }
                            else
                            {
                                FrontAssignement += FrontAssignement.Equals(string.Empty) ? FloorNumber.ToString() : Constant.SPACE_COMMA + FloorNumber;
                            }
                            FloorNumber = lstFrontLanding[index];

                        }
                        if (index == lstFrontLanding.Count - 1)
                        {
                            if (lstFrontLanding[index] != FloorNumber)
                            {
                                FrontAssignement += FrontAssignement.Equals(string.Empty) ? FloorNumber + Constant.HYPHEN + lstFrontLanding[index] : Constant.SPACE_COMMA + FloorNumber + Constant.HYPHEN + lstFrontLanding[index];

                            }
                            else
                            {
                                FrontAssignement += FrontAssignement.Equals(string.Empty) ? FloorNumber.ToString() : Constant.SPACE_COMMA + FloorNumber;
                            }
                            FrontAssignement = groupHallFixturesOpeningValues.HallStationName + Constant.COLON + Constant.EMPTYSPACE + Constant.STRING_F + FrontAssignement;

                        }
                    }
                }
                FloorNumber = lstRearLanding.Count > 0 ? lstRearLanding[0] : 1;
                if (lstRearLanding.Count == 1)
                {
                    RearAssignment = groupHallFixturesOpeningValues.HallStationName + Constant.COLON + Constant.EMPTYSPACE + Constant.STRING_R + lstRearLanding[0];
                }
                else
                {
                    for (var index = 1; index < lstRearLanding.Count; index++)
                    {

                        if (lstRearLanding[index] - lstRearLanding[index - 1] != 1)
                        {
                            if (lstRearLanding[index - 1] != FloorNumber)
                            {
                                RearAssignment += RearAssignment.Equals(string.Empty) ? FloorNumber + Constant.HYPHEN + lstRearLanding[index - 1] : Constant.SPACE_COMMA + FloorNumber + Constant.HYPHEN + lstRearLanding[index - 1];

                            }
                            else
                            {
                                RearAssignment += RearAssignment.Equals(string.Empty) ? FloorNumber.ToString() : Constant.SPACE_COMMA + FloorNumber;
                            }

                            FloorNumber = lstRearLanding[index];

                        }
                        if (index == lstRearLanding.Count - 1)
                        {
                            if (lstRearLanding[index] != FloorNumber)
                            {
                                RearAssignment += RearAssignment.Equals(string.Empty) ? FloorNumber + Constant.HYPHEN + lstRearLanding[index] : Constant.SPACE_COMMA + FloorNumber + Constant.HYPHEN + lstRearLanding[index];

                            }
                            else
                            {
                                RearAssignment += RearAssignment.Equals(string.Empty) ? FloorNumber.ToString() : Constant.SPACE_COMMA + FloorNumber;
                            }
                            RearAssignment = groupHallFixturesOpeningValues.HallStationName + Constant.COLON + Constant.EMPTYSPACE + Constant.STRING_R + RearAssignment;
                        }

                    }
                }


                FrontRearAssignment = lstFrontLanding.Count == 0 ? lstRearLanding.Count == 0 ? string.Empty : RearAssignment : lstRearLanding.Count == 0 ? FrontAssignement :
                                     FrontAssignement + Constant.OR + RearAssignment;
                if (!string.IsNullOrEmpty(FrontRearAssignment))
                {
                    FrontRearUnitAssignment = string.IsNullOrEmpty(FrontRearUnitAssignment) ? FrontRearAssignment : FrontRearUnitAssignment + Constant.SPACE_COMMA + FrontRearAssignment;

                }
            }


            return FrontRearUnitAssignment;
        }

        public static string GetGroupLayoutTab(string selectedTab)
        {
            string selectedNewTab = string.Empty;
            if (Utility.CheckEquals(selectedTab, Constant.FLOORPLANTAB) || Utility.CheckEquals(selectedTab, Constant.DOORTAB) || Utility.CheckEquals(selectedTab, Constant.CONTROLROOMTAB) || Utility.CheckEquals(selectedTab, Constant.RISERLOCATIONSTAB))
            {
                selectedNewTab = Constant.GROUPLAYOUTCONFIGURATION;
            }
            return selectedNewTab;
        }

        /// <summary>
        /// VariableMapper
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="variableType"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetVariableMapping(string filePath, string variableType)
        {
            // get the constant Values
            var mapperData = JObject.Parse(File.ReadAllText(filePath));
            var variables = mapperData[variableType].ToObject<Dictionary<string, string>>();
            return variables;
        }


        public static string MapVariables(string clmResponseString, string appResponseString)
        {
            //transform to JObject
            var clmResponse = JObject.Parse(clmResponseString);
            var appResponse = JObject.Parse(appResponseString);

            return MapVariables(clmResponse, appResponse).ToString();
        }

        public static JObject MapVariables(JObject clmResponse, JObject appResponse)
        {
            //extract the variables
            var collection = GetVariables(clmResponse);
            var appSections = GetTokens("sections", appResponse);

            foreach (var item in appSections)
            {
                var currentVariables = new JArray(GetTokens("variables", item));
                var replacable = (collection.Where(x => currentVariables.Any(y => CheckEquals(x,y)))).ToArray<JToken>();
                if (replacable.Count() > 0)
                {
                    currentVariables.Merge(replacable, new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Replace });
                    currentVariables = TranslatePropertyId(currentVariables, "minimumvalue", "minvalue");
                    currentVariables = TranslatePropertyId(currentVariables, "maximumvalue", "maxvalue");
                    item["variables"] = currentVariables;
                }
            }
            bool CheckEquals(JToken first,JToken second)
            {
                var firstId = (string)first["id"];
                var secondId= (string)second["id"];
                if (firstId == null )
                {
                    firstId = (string)first["Id"];
                }
                if(secondId==null)
                {
                    secondId = (string)second["Id"];
                }
                return secondId == firstId;
            }

            return appResponse;
        }
        public static IEnumerable<JToken> GetVariables(JToken parent)
        {
            var tokenCollection = new List<JToken>();
            var children = GetTokens("sections", parent);

            foreach (var item in children)
            {
                tokenCollection.AddRange(GetTokens("variables", item));
            }

            return tokenCollection;
        }


        public static IEnumerable<JToken> GetTokens(string propertyName, JToken parent, bool includeEmptyTokens = false)
        {
            var collection = new List<JToken>();

            if (parent is JArray jArrayParent)
            {
                collection.AddRange(GetTokens(propertyName, jArrayParent));
            }

            if (parent is JObject jObjectParent)
            {
                foreach (var item in jObjectParent.Children<JProperty>())
                {
                    if (item.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (includeEmptyTokens || item.Value.Count() > 0)
                        {
                            collection.AddRange(GetTokens(propertyName, item.Value));
                            collection.AddRange(item.Value.Children());

                        }
                    }
                }
            }
            return collection;
        }

        public static IEnumerable<JToken> GetTokens(string propertyName, JArray parent)
        {
            return parent.Children().SelectMany(x => GetTokens(propertyName, x));
        }


        /// <summary>
        /// Method for Getting the RequestBody Assignments
        /// </summary>
        /// <param name="stubReqbody"></param>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public static JObject CLMRequestPayloadAssignments(string stubReqbody, string packageId)
        {
            var methodBeginTime = Utility.LogBegin();
            var assignments = Utility.DeserializeObjectValue<JObject>(stubReqbody);
            assignments[Constant.GLOBALARGUMENTS][Constant.MATERIALVARIANT][Constant.MATERIAL][Constant.CONFIGURABLEMATERIALNAME] = packageId;
            assignments[Constant.GLOBALARGUMENTS][Constant.MATERIALVARIANT][Constant.MATERIAL][Constant.EXTERNALID] = packageId;
            assignments[Constant.LINE][Constant.REQPRODUCTID] = packageId;
            assignments[Constant.LINE][Constant.PRODUCT][Constant.REQID] = packageId;
            Utility.LogEnd(methodBeginTime);
            return assignments;
        }

        /// <summary>
        /// Method For Getting the Request Body With Variable Assignments
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        public static ConfigurationRequest GetRequestBodyWithAssignments(string reqBodyPath, string packageId, string packagePath)
        {
            var methodBeginTime = Utility.LogBegin();
            var configurationRequest = new ConfigurationRequest();
            var stubReqbody = JObject.Parse(System.IO.File.ReadAllText(reqBodyPath)).ToString();
            var requestBodyAssignmentsObj = Utility.CLMRequestPayloadAssignments(stubReqbody, packageId);
            configurationRequest = requestBodyAssignmentsObj.ToObject<ConfigurationRequest>();
            configurationRequest.PackagePath = packagePath;
            Utility.LogEnd(methodBeginTime);
            return configurationRequest;
        }

        public static JArray TranslatePropertyId(JArray variables, string oldId, string newId)
        {
            var variableList = variables.ToObject<List<Variables>>();
            foreach (var variable in variableList)
            {
                var selectedProperty = variable.Properties.FirstOrDefault(_ => CheckEquals(_.Id, oldId));
                if (selectedProperty != null)
                {
                    selectedProperty.Id = newId;
                }
            }

            return JArray.FromObject(variableList);
        }

        public static string MapElevatorNameFromModelToUI(string modelElevatorId)
        {
            var splitName = modelElevatorId.Split(Constant.DOTCHAR);
            var elevatorName = Constant.ELEVATOR1;
            switch (splitName[0])
            {
                case "B1P1":
                    elevatorName = Constant.ELEVATOR1;
                    break;
                case "B1P2":
                    elevatorName = Constant.ELEVATOR2;
                    break;
                case "B1P3":
                    elevatorName = Constant.ELEVATOR3;
                    break;
                case "B1P4":
                    elevatorName = Constant.ELEVATOR4;
                    break;
                case "B2P1":
                    elevatorName = Constant.ELEVATOR5;
                    break;
                case "B2P2":
                    elevatorName = Constant.ELEVATOR6;
                    break;
                case "B2P3":
                    elevatorName = Constant.ELEVATOR7;
                    break;
                case "B2P4":
                    elevatorName = Constant.ELEVATOR8;
                    break;
            }
            splitName[0] = elevatorName;
            return string.Join(Constant.DOTCHAR, splitName);
        }


        public static void CreateAndWriteFile(string directory, string fileName, string content)
        {
            // Determine whether the directory exists.
            if (!Directory.Exists(directory))
            {
                _ = Directory.CreateDirectory(directory);
            }
            //Saving the XML File
            using System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(directory, fileName));
            file.WriteLine(content);
        }


        public static string ConvertInchtoFeet(string feet)
        {
            return Convert.ToString(Convert.ToInt32(Convert.ToDecimal(feet) / 12)) + "'-" + (Convert.ToString(Convert.ToDouble((Convert.ToDecimal(feet) % 12)))) + "'";
        }
    }


}
