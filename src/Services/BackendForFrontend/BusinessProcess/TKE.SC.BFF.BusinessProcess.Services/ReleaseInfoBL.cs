using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common;
using System;
using TKE.SC.PIPO;
using Constants = TKE.SC.Common.Constants;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class ReleaseInfoBL : IReleaseInfo
    {

        private readonly IReleaseInfoDL _rleaseInfo;
        private readonly IPipoConnector _pipoConnector;
        private readonly IConfigure _configure;
        private readonly IObom _obomBL;
        private IConfiguration _configuration;
        public ReleaseInfoBL(IReleaseInfoDL releaseInfoDL, IConfigure configure, ICacheManager cpqCacheManager, IConfiguration configuration,
            ILogger<ReleaseInfoBL> logger, IObom obomBL,IPipoConnector pipoConnector)
        {
            _rleaseInfo = releaseInfoDL;
            _configure = configure;
            _obomBL = obomBL;
            _configuration = configuration;
            _pipoConnector = pipoConnector;
            Utility.SetLogger(logger);
        }


        /// <summary>
        /// Get list of builgings and groups for the project ID
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetProjectReleaseInfo(string quoteId,string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            //Get FieldDrawingAutomation Variables
            var lstReleaseDetails = _rleaseInfo.GetProjectReleaseInfo(quoteId);
            var roleName = _configure.GetRoleName(sessionId);
            var permissions = _rleaseInfo.GetPermissionForReleaseInfo(quoteId, roleName,Constant.RELEASEINFOLIST);
            if (lstReleaseDetails.GroupDetailsForReleaseInfo != null)
            {
                foreach (var building in lstReleaseDetails.GroupDetailsForReleaseInfo)
                {
                    foreach (var groups in building.GroupDetails)
                    {
                        var permissionGroup = (from permission in permissions
                                               where permission.GroupStatus.Equals(groups.GroupStatus.StatusName)
                                               select permission.PermissionKey).Distinct().ToList();
                        groups.Permissions = permissionGroup;
                    }
                }
                    
            }
            var response = JObject.FromObject(lstReleaseDetails);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };

        }


        /// <summary>
        /// Get Unit configuration datapoints for release to manufacture
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetGroupReleaseInfo(int groupId,string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var releaseInfoDetails = _rleaseInfo.GetGroupReleaseInfo(groupId, sessionId);
            // added enriched data in the main response 
            var enrichedData = JObject.Parse(File.ReadAllText(Constant.UNITENRICHEDDATA));
            var roleName = _configure.GetRoleName(sessionId);
            var permissions= _rleaseInfo.GetPermissionForReleaseInfo(groupId.ToString(), roleName, Constant.RELEASEINFO);
            releaseInfoDetails.Permissions = permissions.Count > 0 ? permissions.Select(x => x.PermissionKey).ToList() : new List<string>();
            releaseInfoDetails.EnrichedData = enrichedData;
            var response = JObject.FromObject(releaseInfoDetails);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }


        /// <summary>
        /// Save and release method to save the release to manufacture approved details.
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveUpdatReleaseInfoDetails(int groupId, JObject variableAssignments, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            var groupData = Utility.DeserializeObjectValue<GroupDetailsReleaseToManufacture>(Utility.SerializeObjectValue(variableAssignments));
            var actionFlag = groupData.Action;
            var setIdList = (from set in groupData.UnitDetails
                             select set.SetId).ToList();
            var unitIdList = (from unit in groupData.UnitDetails
                              select unit.Id).ToList();
            var checkQuestionsList = (from ques in groupData.ReleaseQueries select ques.ReleaseQueId).ToList();
            List<ReleaseInfoSetUnitDetails> listOfGroupVariables = new List<ReleaseInfoSetUnitDetails>();
            List<ReleaseInfoQuestions> listOfQuestions = new List<ReleaseInfoQuestions>();
            foreach (var unitIds in unitIdList)
            {
                foreach (var sets in setIdList)
                {
                    var dataPointsList = (from dataPts in groupData.UnitDetails
                                          where dataPts.SetId.Equals(sets) && dataPts.Id.Equals(unitIds)
                                          select dataPts);
                    if (dataPointsList.FirstOrDefault() != null)
                    {
                        listOfGroupVariables.Add(dataPointsList.FirstOrDefault());
                    }
                }
            }
            foreach (var que in checkQuestionsList)
            {
                var questions = (from check in groupData.ReleaseQueries where check.ReleaseQueId.Equals(que) select check);
                listOfQuestions.Add(questions.FirstOrDefault());
            }
            Utility.LogTrace(Constant.SAVEUPDATERELEASEINFOINITIATEDL);
            var result = _rleaseInfo.SaveUpdatReleaseInfoDetailsDL(groupId, listOfGroupVariables, listOfQuestions, userId, actionFlag);
            var response = JArray.FromObject(result);
            var viewSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constants.OBOM);
            var message = string.Empty;
            if (Convert.ToBoolean(Utility.GetPropertyValue(viewSettings, Constants.GENERATEOBOM)) )
            {
                ConfigurationDetails configurationDetails = new ConfigurationDetails()
                {
                    GroupId = groupId
                };
                var obomResponse = await _obomBL.GETOBOMResponse(configurationDetails, sessionId).ConfigureAwait(false);
                if (obomResponse.XmlDocument.Any())
                {
                    foreach (var obomXml in obomResponse.XmlDocument)
                    {
                        var output=_pipoConnector.CreateOrUpdateSpecMemoAsync(Convert.ToString(obomXml), TKE.SC.PIPO.Constants.ContentType.XmlString, TKE.SC.PIPO.Constants.PiPoSettings).ConfigureAwait(false);
                        if(output.GetAwaiter().GetResult())
                            Utility.LogInfo("Xmls succesfully send to MFiles");
                    }
                    message = "OBOM xml generated successfully";

                    configurationDetails.QuoteId = obomResponse.QuoteId;
                    //await _obomBL.GenerateExcelForStatusReport(configurationDetails).ConfigureAwait(false);
                }
            }

            if (result[0].Result >= 1)
            {
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response,Message= message };
            }
            else
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].Message,
                    Description=Constant.RELEASEINFOERROR,
                    ResponseArray = response
                });
            }
        }
    }
}
