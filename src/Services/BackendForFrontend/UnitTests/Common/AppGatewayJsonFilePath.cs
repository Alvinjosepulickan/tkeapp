using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKE.SC.BFF.DataAccess.Helpers;
namespace TKE.SC.BFF.Test.Common
{
    public class AppGatewayJsonFilePath
    {
        public static readonly string INPUTJSONPATH = @"InputJson\";

        public static readonly string CommonFile = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "UserInfoValidate.json");

        public static readonly string GETBUILDINGCONFIGREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "GetBuildingRequestBody.json");

        public static readonly string SAVEBUILDINGREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveBuildingRequestBody.json");

        public static readonly string UPDATEBUILDINGREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "UpdateBuildingConfigRequestBody.json");

        public static readonly string SAVEBUILDINGELEVATIONREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveBuildingElevationRequestBody.json");

        public static readonly string SAVEBUILDINGELEVATIONREQUESTBODYVALUE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "VariableAssignmentValue.json");

        public static readonly string SAVENEWBUILDINGELEVATIONREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveNewRequestBody.json");

        public static readonly string UPDATEBUILDINGELEVATIONREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "UpdateBuildingElevationRequestBody.json");

        public static readonly string STARTGROUPCONFIGREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "StartGroupConfigRequestBody.json");

        public static readonly string STARTGROUPCONSOLEREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "StartGroupConsoleReqBody.json");

        
        public static readonly string CHANGEGROUPCONFIGREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ChangeGroupconfigRequestBody.json");

        public static readonly string UPDATEOPENINGLOCATIONREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "UpdateOpeningLocationRequestBody.json");

        public static readonly string PRODUCTOUTPUT = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ProductOutput.json");

        public static readonly string PROJECTDETAILS = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "projectdetails.json");

        public static readonly string GETGROUPCONFIGREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "GetGroupRequestBody.json");

        public static readonly string SAVEGROUPREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveGroupRequestBody.json");

        public static readonly string UPDATEGROUPREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "UpdateGroupConfigRequestBody.json");

        public static readonly string CHANGECONFIGUREOUTPUT = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "changeconfigureoutput.json");


        public static readonly string LOGOFFREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "logOff.json");

        public static readonly string SAVEBUILDINGELEVATIONREQUESTBODY1 = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveBuildingElevationRequestBody1.json");

        public static readonly string SAVEBUILDINGELEVATIONREQUESTBODY2 = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveBuildingElevationRequestBody2.json");

        public static readonly string UPDATEBUILDINGELEVATIONREQUESTBODY1 = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "UpdateBuildingElevationRequestBody1.json");

        public static readonly string UPDATEBUILDINGELEVATIONREQUESTBODY2 = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "UpdateBuildingElevationRequestBody2.json");

        public static readonly string GETBUILDINGCONFIGBYIDSTUBRESPONSEBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "GetBuildingConfigByIdStubResponseBody.json");

        public static readonly string STARTGROUPCONFIGURATIONRESPONSEBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "StartGroupConfigurationResponseBody.json");

        public static readonly string GETBUILDINGCONFIGBYIDSTUBRESPONSEBODY1 = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "GetBuildingConfigByIdStubResponseBody1.json");

        public static readonly string VALIDATETOKENREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ValidateTokenRequestBody.json");

        public static readonly string BASESTARTCONFIGURATION_ = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "BaseStartConfiguration.json");

        public static readonly string CURRENTMACHINECONFIGURATION = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CURRENTMACHINECONFIGURATION_.json");

        public static readonly string AUTOSAVEBUILDINGCONFIGREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "AutoSaveBuildingConfigRequestBody.json");

        public static readonly string STARTCONFIGBLREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "StartConfigBlRequestBody.json");

        public static readonly string CHANGEGROUPLAYOUTBLREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ChangeGroupLayoutBlRequestBody.json");

        public static readonly string CHANGEGROUPCONFIGBLREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ChangeGroupConfigBlRequestBody.json");

        public static readonly string SAVEGROUPLAYOUTFLOORPLANREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveGroupLayoutFloorPlanRequestBody.json");
        public static readonly string SAVEGROUPLAYOUTFLOORPLANERRORREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveGroupLayoutFloorPlanRequestBodyError.json");

        public static readonly string VARIABLEASSIGNMENTREQUESTPAYLOAD = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "VariableAssignmentRequestPayload.json");

        public static readonly string SAVECABINTERIORREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveCabInteriorRequestBody.json");

        public static readonly string SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveHoistwayTractionEquipmentRequestBody.json");

        public static readonly string CHANGEGROUPCONFIGLAYOUTREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ChangeGroupConfigLayoutRequestBody.json");

        public static readonly string STARTUNITCONFIGREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "StartUnitConfigureRequestBody.json");

        public static readonly string STARTUNITCONFIGCARFIXTUREREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "StartUnitConfigureRequestBody.json");

        public static readonly string CHANGEUNITCONFIGREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ChangeUnitConfigRequestBody.json");

        public static readonly string SAVEGENERALINFORMATIONREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveGeneralInformationRequestBody.json");

        public static readonly string PRODUCTSELECTIONCONTROLLERREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ProductSelectionControllerRequestBody.json");

        public static readonly string SAVEPRODUCTSELECTIONREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveProductSelectionRequestBody.json");

        public static readonly string SAVEENTRANCEREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveEntranceRequestBody.json");
        public static readonly string SAVEENTRANCECONFIGUREREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveEntranceConfigureRequestBody.json");

        public static readonly string SAVEUNITHALLFIXTUREREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveUnitHallFixturesRequestBody.json");

        public static readonly string STARTUNITHALLFIXTURECONSOLEREQUESTBOSY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "StartUnitHallFixtureConsoleRequestBody.json");

        public static readonly string STARTCONSOLECACHEDATA = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "StartConsoleCacheData.json");

        public static readonly string SAVECARCALLCUTOUTKEYSWITCHOPENINGS = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveCarCallCutoutKeyswitchOpenings.json");


        public static readonly string EDITUNITHLLFIXTURECONSOLEDATA = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "EditUnitHallFixtureConsoleRequestBody.json");

        public static readonly string CHANGECONSOLECACHEDDATA = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ChangeConsoleCachedData.json");

        

        public static readonly string SAVEGROUPHALLFIXTURESREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveGroupHallFixtureReqBody.json");

        public static readonly string DELETEGROUPHALLFIXTUREREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "deleteGroupHallFixtureReqbody.json");

        public static readonly string CHANGEBUILDINGEQUIPMENTREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ChangeBuildingEquipmentRequestBody.json");

        public static readonly string SAVEBUILDINGEQUIPMENTCONSOLEREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SaveBuildingEquipmentConsoleRequestBody.json");

        public static readonly string CHANGEBULDINGEQUIPMENTCONSOLEREQUESTBODY = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ChangeBuildingEquipmentConsoleRequestBody.json");

        public static readonly string FIELDDRAWINGAUTOMATIONSTUB = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "FieldDrawingAutomationStub.json");

        public static readonly string DESIGNAUTOMATIONSTUB = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "DaInputDetails.json");

        public static readonly string CSC1003AAEXTF = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CSC1003AAEXTF.json");

        public static readonly string CSC1003AAEXTE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CSC1003AAEXTE.json");

        public static readonly string CSC1003AAINT = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CSC1003AAINT.json");

        public static readonly string CSC1003AAINTF = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CSC1003AAINTF.json");

        public static readonly string CSC1003AAEXTFRESPONSE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CSC1003AAEXTFResponse.json");

        public static readonly string CSC1003AAEXTERESPONSE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CSC1003AAEXTEResponse.json");

        public static readonly string CSC1003AAINTRESPONSE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CSC1003AAINTResponse.json");

        public static readonly string CSC1003AAINTFRESPONSE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CSC1003AAINTFResponse.json");

        public static readonly string SUBMITBOMREQUESTCSC1003AAEXTF = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SubmitBomRequestCSC1003AAEXTF.json");

        public static readonly string SUBMITBOMREQUESTCSC1003AAEXTE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SubmitBomRequestCSC1003AAEXTE.json");

        public static readonly string SUBMITBOMREQUESTCSC1003AAINT = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SubmitBomRequestCSC1003AAINT.json");

        public static readonly string SUBMITBOMREQUESTCSC1003AAINTF = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SubmitBomRequestCSC1003AAINTF.json");

        public static readonly string SUBMITBOMRESPONSECSC1003AAEXTF = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SubmitBomResponseCSC1003AAEXTF.json");

        public static readonly string SUBMITBOMRESPONSECSC1003AAEXTE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SubmitBomResponseCSC1003AAEXTE.json");

        public static readonly string SUBMITBOMRESPONSECSC1003AAINT = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SubmitBomResponseCSC1003AAINT.json");

        public static readonly string SUBMITBOMRESPONSECSC1003AAINTF = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "SubmitBomResponseCSC1003AAINTF.json");

        public static readonly string AVAILABLEEXPORTTYPESRESPONSE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "AvailableExportTypesResponse.json");

        public static readonly string GETJOBSTATUSESRESPONSE = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "GetJobStatusResponse.json");

        public static readonly string REQUESTLAYOUTJSON = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "FieldAutomationWrapperRequest.json");

        public static readonly string VIEWEXPORTVARIABLELIST = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ViewExportParam.json");


        public static readonly string SAVESENTTOCOORDINATIONREQUESTLAYOUTJSON = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "CoordinationRequestBody.json");

        public static readonly string SAVEINPUTVARIABLEVALUES = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ProjectVariableInputInfo.json");

        public static readonly string SAVEINPUTVARIABLEVALUEDSERROR = Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, INPUTJSONPATH + "ProjectVariableInputInfoError.json");
    }
}
