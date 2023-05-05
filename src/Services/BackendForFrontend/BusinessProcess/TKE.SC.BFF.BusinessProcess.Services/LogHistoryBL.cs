using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using Microsoft.Extensions.Configuration;
using TKE.SC.Common.Model.HttpClientModel;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class LogHistoryBL : ILogHistory
    {
        #region Variables
        private readonly IUnitConfigurationDL _unitconfigurationdl;
        private readonly IConfigure _configure;
        string parentCode = null, locale = null, modelNumber = null;
        private readonly IBuildingConfigurationDL _buildingConfigurationdl;
        private readonly IGroupConfigurationDL _groupdl;
        private readonly IConfiguration _configuration;
        #endregion

        /// <summary>
        /// Constructor for LogHistoryBL
        /// </summary>
        /// <param Name="utility"></param>
        /// <param Name="configure"></param>
        /// <param Name="unitdl"></param>
        /// <param Name="buildingdl"></param>
        /// <param Name="groupdl"></param>
        public LogHistoryBL(IConfiguration configuration, ILogger<LogHistoryBL> logger, IConfigure configure, IUnitConfigurationDL unitdl, IBuildingConfigurationDL buildingdl, IGroupConfigurationDL groupdl)
        {
            _configure = configure;
            _unitconfigurationdl = unitdl;
            _buildingConfigurationdl = buildingdl;
            _groupdl = groupdl;
            _configuration = configuration;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// GetLogHistory
        /// </summary>
        /// <param Name="requestBody"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetLogHistory(LogHistoryRequestBody requestBody)
        {
            var methodBeginTime = Utility.LogBegin();
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGCONSTANTMAPPER);
            var feetInchVaribles = JArray.Parse(File.ReadAllText(Constants.FEETINCHVARIABLESLIST));
            var lstfeetInchVaribles = Utility.DeserializeObjectValue<List<string>>(feetInchVaribles.ToString());
            var variableNotUsingValueEnrichment = JArray.Parse(File.ReadAllText(Constants.VARIABLENOTUSINGVALUEENRICHMENTLIST));
            var lstVariableNotUsingValueEnrichment = Utility.DeserializeObjectValue<List<string>>(variableNotUsingValueEnrichment.ToString());
            var variables = new Dictionary<string, Section>();
            var values = new Dictionary<string, Section>();
            var responseTuple = GetHistoryAndEnrichmentForEntity(requestBody);
            var logHistoryResponseObj = responseTuple.Item1;
            var enrichedData = responseTuple.Item2;
            variables = Utility.DeserializeObjectValue<Dictionary<string, Section>>(enrichedData[Constants.VARIABLES].ToString());
            values = Utility.DeserializeObjectValue<Dictionary<string, Section>>(enrichedData[Constants.OPTIONS].ToString());
            var GroupLayoutPositionJObject = JObject.Parse(File.ReadAllText(Constants.GROUPUNITPOSITIONDISPLAYNAME));
            var GroupLayoutPosition = Utility.DeserializeObjectValue<Dictionary<string, string>>(GroupLayoutPositionJObject.ToString());
            var lstEmailId = (from lstDeatils in logHistoryResponseObj.Data
                              from logparameter in lstDeatils.LogParameters
                              select logparameter.User
                            ).Distinct().ToList();
            var userData = await Utilities.GetUserDetailsAsync(lstEmailId,_configuration);
            if (logHistoryResponseObj.Data != null)
            {
                foreach (var data in logHistoryResponseObj.Data)
                {
                    if (data.LogParameters != null)
                    {
                        foreach (var parameter in data.LogParameters)
                        {
                            var feetinchinput = lstfeetInchVaribles.Where(x => parameter.VariableId.Contains(x)).ToList();
                            if (feetinchinput.Count > 0)
                            {
                                double decimalValue;
                                if (parameter.PreviousValue != "" && parameter.PreviousValue != null)
                                {
                                    decimalValue = Convert.ToDouble(parameter.PreviousValue);
                                    parameter.PreviousValue = Utility.ConvertDecimalToFraction(decimalValue, Constants.BUILDINGRISE);
                                }
                                if (parameter.UpdatedValue != "" && parameter.UpdatedValue != null)
                                {
                                    decimalValue = Convert.ToDouble(parameter.UpdatedValue);
                                    parameter.UpdatedValue = Utility.ConvertDecimalToFraction(decimalValue, Constants.BUILDINGRISE);
                                }

                            }
                            if (parameter.VariableId.Contains(Constants.FLOORTOFLOORHEIGHT) || parameter.VariableId.Contains(Constants.ELEVATION_1))
                            {
                                if (parameter.PreviousValue != "" && parameter.PreviousValue != null)
                                {
                                    var arrfeetInch = parameter.PreviousValue.Split(Constants.COMMA);
                                    if (arrfeetInch.Length > 1)
                                    {
                                        var feet = arrfeetInch[0];
                                        var inch = Utility.ConvertDecimalToFraction(Convert.ToDouble(arrfeetInch[1]));
                                        parameter.PreviousValue = feet + " ft " + inch + " in";
                                    }
                                }
                                if (parameter.UpdatedValue != "" && parameter.UpdatedValue != null)
                                {
                                    var arrfeetInch = parameter.UpdatedValue.Split(Constants.COMMA);
                                    if (arrfeetInch.Length > 1)
                                    {
                                        var feet = arrfeetInch[0];
                                        var inch = Utility.ConvertDecimalToFraction(Convert.ToDouble(arrfeetInch[1]));
                                        parameter.UpdatedValue = feet + " ft " + inch + " in";
                                    }
                                }
                            }
                            if (variables.ContainsKey(parameter.VariableId))
                            {
                                var lstProperties = variables[parameter.VariableId].Properties;
                                var displayname = lstProperties.Where(x => Utility.CheckEquals(x.Id, Constants.DISPLAYNAME)).ToList();
                                parameter.Name = (displayname.Count > 0 && (displayname[0].Value.ToString()!=String.Empty)) ? displayname[0].Value.ToString() : parameter.Name;
                            }
                            if (GroupLayoutPosition.ContainsKey(parameter.VariableId))
                            {
                                parameter.Name = GroupLayoutPosition[parameter.VariableId];
                            }
                            if (!lstVariableNotUsingValueEnrichment.Contains(parameter.VariableId) && values.ContainsKey(parameter.UpdatedValue))
                            {
                                if (!parameter.VariableId.Contains(buildingContantsDictionary[Constants.FLOORDESIGNATION]))
                                {
                                    var lstProperties = values[parameter.UpdatedValue]?.Properties;
                                    var displayname = lstProperties.Where(x => Utility.CheckEquals(x.Id, Constants.DISPLAYNAME)).ToList();
                                    parameter.UpdatedValue = displayname.Count > 0 ? displayname[0].Value.ToString() : parameter.UpdatedValue;
                                }
                            }
                            if (!lstVariableNotUsingValueEnrichment.Contains(parameter.VariableId) && values.ContainsKey(parameter.PreviousValue))
                            {
                                var lstProperties = values[parameter.PreviousValue].Properties;
                                var displayname = lstProperties.Where(x => Utility.CheckEquals(x.Id, Constants.DISPLAYNAME)).ToList();
                                parameter.PreviousValue = displayname.Count > 0 ? displayname[0].Value.ToString() : parameter.PreviousValue;
                            }
                            var user = userData?.Where(x => Utility.CheckEquals(x.Email, parameter?.User)).ToList().FirstOrDefault();
                            if (user != null)
                            {
                                parameter.User = user.FirstName + " " + user.LastName;
                            }
                        }
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage
            {
                StatusCode = Constants.SUCCESS,
                Response = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(logHistoryResponseObj))
            };
        }

        public (LogHistoryResponse, JObject) GetHistoryAndEnrichmentForEntity(LogHistoryRequestBody requestBody)
        {
            JObject enrichedData = null;
            string productType = string.Empty;
            LogHistoryResponse logHistoryResponseObj = new LogHistoryResponse();
            var unitContantsDictionary = new Dictionary<string, string>();
            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            switch (requestBody.Section.ToUpper())
            {
                case Constants.UNITCAPS:
                    logHistoryResponseObj = _unitconfigurationdl.GetLogHistoryUnit(requestBody.SetID, requestBody.UnitID, requestBody.LastDate);
                    unitContantsDictionary = Utility.GetVariableMapping(Constants.CUSTOMENGINEEREDCONSTANTMAPPERPATH, Constants.CUSTOMENGINEEREDVARIABLES);
                    foreach (var variable in unitContantsDictionary)
                    {
                        ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                        lstConfigVariable.Add(configVariable);
                    }
                    var dtVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));
                    productType = _groupdl.GetProductCategoryByGroupId(requestBody.SetID, Constants.SETLOWERCASE, dtVariables);
                    if (Utility.CheckEquals(productType, Constants.PRODUCTELEVATOR))
                    {
                        productType = _unitconfigurationdl.GetProductType(requestBody.SetID);
                    }
                    enrichedData = productType switch
                    {
                        Constants.EVO_100 => JObject.Parse(File.ReadAllText(string.Format(Constants.UNITENRICHMENTSTEMPLATE, Constants.EVOLUTION100))),
                        Constants.EVO_200 => JObject.Parse(File.ReadAllText(string.Format(Constants.UNITENRICHMENTSTEMPLATE, Constants.EVOLUTION200))),
                        Constants.ENDURA_100 => JObject.Parse(File.ReadAllText(string.Format(Constants.UNITENRICHMENTSTEMPLATE, Constants.END100))),
                        Constants.ESCLATORMOVINGWALK => JObject.Parse(File.ReadAllText(string.Format(Constants.NCPENRICHEDDATA, Constants.ESCALATORTYPE))),
                        Constants.TWINELEVATOR => JObject.Parse(File.ReadAllText(string.Format(Constants.NCPENRICHEDDATA, Constants.TWINELEVATOR))),
                        Constants.OTHER => JObject.Parse(File.ReadAllText(string.Format(Constants.NCPENRICHEDDATA, Constants.OTHER))),
                        _ => JObject.Parse(File.ReadAllText(string.Format(Constants.CUSTOMENGINEEREDENRICHEDDATA, productType.Replace("CE_", "")))),
                    };
                    break;
                case Constants.BUILDINGCAPS:
                    logHistoryResponseObj = _buildingConfigurationdl.GetLogHistoryBuilding(requestBody.BuildingId, requestBody.LastDate);
                    enrichedData = JObject.Parse(File.ReadAllText(Constants.BUILDINGENRICHEDDATA));
                    break;
                case Constants.GROUPCAPS:
                    logHistoryResponseObj = _groupdl.GetLogHistoryGroup(requestBody.GroupId, requestBody.LastDate);
                    unitContantsDictionary = Utility.GetVariableMapping(Constants.CUSTOMENGINEEREDCONSTANTMAPPERPATH, Constants.CUSTOMENGINEEREDVARIABLES);
                    foreach (var variable in unitContantsDictionary)
                    {
                        ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                        lstConfigVariable.Add(configVariable);
                    }
                    var dtVariablesForProduct = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));
                    productType = _groupdl.GetProductCategoryByGroupId(requestBody.GroupId, Constant.GROUPLOWERCASE, dtVariablesForProduct);
                    enrichedData = productType switch
                    {
                        Constants.PRODUCTELEVATOR => JObject.Parse(File.ReadAllText(Constants.ELEVATORENRICHMENTTEMPLATE)),
                        Constants.ESCLATORMOVINGWALK => JObject.Parse(File.ReadAllText(Constants.GROUPPOPUPENRICHMENTTEMPLATE)),
                        Constants.TWINELEVATOR => JObject.Parse(File.ReadAllText(Constants.GROUPPOPUPENRICHMENTTEMPLATE)),
                        Constants.OTHER => JObject.Parse(File.ReadAllText(Constants.GROUPPOPUPENRICHMENTTEMPLATE)),
                        _ => JObject.Parse(File.ReadAllText(Constants.ELEVATORENRICHMENTTEMPLATE))
                    };
                    break;
                default:
                    logHistoryResponseObj = _unitconfigurationdl.GetLogHistoryUnit(requestBody.SetID, requestBody.UnitID, requestBody.LastDate);
                    break;
            }
            return (logHistoryResponseObj, enrichedData);
        }

    }
}
