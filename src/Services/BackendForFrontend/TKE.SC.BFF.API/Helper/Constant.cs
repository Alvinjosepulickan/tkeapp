using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.Helper
{
    public class Constant
    {
        /// <summary>
        /// Constants Related to settings
        /// Setting Constants are using for Couchbase Credentials and appsetiing extensions
        /// </summary>
        #region Settings related constants
        public static readonly string LOGOUTPUTTEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss:fff zzz} [{Level}] {Message}{NewLine}{Exception}";
        public static readonly string APPSETTINGS = "appsettings.json";
        public static readonly string PARAMSETTINGS = "ParamSettings";
        public static readonly string ENVIRONMENT = "Environment";
        public static readonly string LOGFILE = "Log\\Log.txt";
        public static readonly string SESSIONID = "SessionId";
        public static readonly string ENCODED = "Content-Type:application/x-www-form-urlencoded;charset=utf-8;";
        public static readonly string LOGLEVEL = "LogLevel";
        public static readonly string NOOFLOGFILESTORETAIN = "NoOfLogFilesToRetain";
        public static readonly string TKEDBConnection = "TKEDBConnection";


        //public static readonly string COUCHUSERNAME = "CouchUserName";
        //public static readonly string COUCHKEYVALUE = "CouchKeyValue";
        //public static readonly string COUCHBASEBUCKET = "CouchBaseBucket";
        #endregion

        /// <summary>
        /// Constants Related to Status Code
        /// Status Code Constants are using for Get http Response Values
        /// </summary>
        #region Status codes
        public static readonly int INTERNALSERVERERROR = StatusCodes.Status500InternalServerError;
        public static readonly int CREATED = StatusCodes.Status201Created;
        public static readonly int SUCCESS = StatusCodes.Status200OK;
        public static readonly int NOTFOUND = StatusCodes.Status404NotFound;
        public static readonly int UNAUTHORIZED = StatusCodes.Status401Unauthorized;
        public static readonly int BADREQUEST = StatusCodes.Status400BadRequest;

        #endregion


        /// <summary>
        /// Constants Related to Session
        ///Session related constants are using to get TKe UID, error log etc values from appsettings.
        /// </summary>
        #region Session related constants
        public static readonly string TKEUID = "Tkeuid";
        public static readonly string ERRORLOG = "errorLog";
        #endregion

        #region Caching related constants
        public static readonly string REDISURI = "RedisUri";
        public static readonly string INSTANCENAME = "InstanceName";

        public static readonly string HANGFIREDBCONNECTION = "HangFireDBConnection";



        //public static readonly string TKEDBConnection = "TKEDBConnection";


        public static readonly string STATUS = "status";

        public static readonly string UID = "GigyaUid";
        public static readonly string LOCALEWORD = "locale";

        public static readonly string DEFAULTCONNECTION = "DefaultConnection";

        #endregion




        #region Logging related constants
        public static readonly string REQUESTCONFIGURATIONLOGS = "Calling Request Configuration API : ";
        public static readonly string SUBLINESLOGC = "Completed Request Configuration API : ";
        public static readonly string VALIDATETOKEN = "Calling Validate token API : ";
        public static readonly string GENERATETOKEN = "Calling Generate token API : ";
        public static readonly string VALIDATED = " Gigya Validation completed.";
        public static readonly string TOKENGENERATED = " Token generation completed.";

        public static readonly string SESSIONEXPIRYMESSAGE = "Session expired";
        public static readonly string SESSIONIDMISSINGMESSAGE = "Unauthorized";

        public static readonly string SOMETHINGWENTWRONG = "Something went wrong";
        public static readonly string INVALIDEXCEPTIONLOGGING = "Operation is not valid due to the current state of the object.";
        public static readonly string LOCALEERROR = "Logout from existing Session, Please login again";
        public static readonly string SERVICEUNAVAILABLEEXCEPTION =
            "The server is currently unable to handle the request due to a temporary overloading or maintenance of the server.";
        public static readonly string REFERENCEID= "ReferenceId-";
        public static readonly string ERRORWHILECONNECTINGEXTERNALAPI = "Error occured while connecting to external API";
        public static readonly string OPENINGBRACKET = "(";
        public static readonly string CLOSINGBRACKET = ")";
        public static readonly string AUTOSAVECONFIGURATIONINITIATE = " AutoSave Configuration BL Call Initiated";
        public static readonly string AUTOSAVECONFIGURATIONCOMPLETE = " AutoSave Configuration BL Call Completed";
        public static readonly string DELETEAUTOSAVECONFIGURATIONBYUSERINITIATE = " Delete AutoSave Configuration By User BL call Initiated";
        public static readonly string DELETEAUTOSAVECONFIGURATIONBYUSERCOMPLETE = " Delete AutoSave Configuration By User BL call Completed";
        public static readonly string GETAUTOSAVECONFIGURATIONBYUSERINITIATE = " Get AutoSave Configuration By User BL call Initiated";
        public static readonly string GETAUTOSAVECONFIGURATIONBYUSERCOMPLETE = " Get AutoSave Configuration By User BL call Completed";
        public static readonly string GETLISTOFCONFIGURATIONFORPROJECTINITIATE = " Get List Of Configuration ForProject BL call Initiated";
        public static readonly string GETLISTOFCONFIGURATIONFORPROJECTCOMPLETE = " Get List Of Configuration ForProject BL call Completed";
        public static readonly string SAVEBUILDINGCONFIGURATIONINITIATE = " SaveBuildingConfigurationForProject BL call Initiated";
        public static readonly string SAVEBUILDINGCONFIGURATIONCOMPLETE = " SaveBuildingConfigurationForProject BL call Completed";
        public static readonly string UPDATEBUILDINGCONFIGURATIONINITIATE = " UpdateBuildingConfigurationForProject BL call Initiated";
        public static readonly string UPDATEBUILDINGCONFIGURATIONCOMPLETE = " UpdateBuildingConfigurationForProject BL call Completed";
        public static readonly string SAVEBUILDINGELEVATIONINITIATE = "SaveBuildingElevation BL call Initiated";
        public static readonly string SAVEBUILDINGELEVATIONCOMPLETE = " SaveBuildingElevation BL call Completed";
        public static readonly string UPDATEBUILDINGELEVATIONINITIATE = " UpdateBuildingElevation BL call Initiated";
        public static readonly string UPDATEBUILDINGELEVATIONCOMPLETE = " UpdateBuildingElevation BL call Completed";
        public static readonly string AUTOSAVEBUILDINGELEVATIONINITIATE = " AutoSaveBuildingElevation BL call Initiated";
        public static readonly string AUTOSAVEBUILDINGELEVATIONCOMPLETE = " AutoSaveBuildingElevation BL call Completed";
        public static readonly string GETBUILDINGELEVATIONBYIDINITIATE = " GetBuildingElevationById BL call Initiated";
        public static readonly string GETBUILDINGELEVATIONBYIDCOMPLETE = " GetBuildingElevationById BL call Completed";
        public static readonly string DELETEBUILDINGCONFIGURATIONBYIDINITIATE = " DeleteBuildingConfigurationById BL call Initiated";
        public static readonly string DELETEBUILDINGCONFIGURATIONBYIDCOMPLETE = " DeleteBuildingConfigurationById BL call Completed";
        public static readonly string DELETEBUILDINGELEVATIONBYIDINITIATE = " DeleteBuildingElevationById BL call Initiated";
        public static readonly string DELETEBUILDINGELEVATIONBYIDCOMPLETE = " DeleteBuildingElevationById BL call Completed";
        public static readonly string STARTBUILDINGCONFIGUREINITIATE = " StartBuildingConfigure BL call Initiated";
        public static readonly string STARTBUILDINGCONFIGUREURL = " StartBuildingConfigure URL";
        public static readonly string STARTBUILDINGCONFIGURECOMPLETE = " StartBuildingConfigure BL call Completed";
        public static readonly string CHANGEBUILDINGCONFIGUREINITIATE = " ChangeBuildingConfigure BL call Initiated";
        public static readonly string CHANGEBUILDINGCONFIGURECOMPLETE = " ChangeBuildingConfigure BL call Completed";
        public static readonly string GETLISTOFPROJECTSFORUSERINITIATE = " getListOfProjectsForUser BL call Initiated";
        public static readonly string GETLISTOFPROJECTSFORUSERCOMPLETE = " getListOfProjectsForUser BL call Completed";
        public static readonly string SEARCHUSERINITIATE = " SearchUser BL call Initiated";
        public static readonly string SEARCHUSERCOMPLETE = " SearchUser BL call Completed";
        public static readonly string GETPROJECTDETAILSINITIATE = " GetProjectDetails BL call Initiated";
        public static readonly string GETPROJECTDETAILSCOMPLETE = " GetProjectDetails BL call Completed";
        public static readonly string GETBUILDINGEQUIPMENTTABSINITIATE = " GetBuildingEquipmentTabs BL call Initiated";
        public static readonly string GETBUILDINGEQUIPMENTTABSCOMPLETE = " GetBuildingEquipmentTabs BL call Completed";
        public static readonly string DELETEPROJECTBYQUOTEID = " DELETEPROJECTBYQUOTEID BL call Initiated";

        #endregion

        public static readonly int IS_SAVE = 0;
        public static readonly int IS_UPDATE = 1;
        public static readonly string SECTIONTABS = "sectionTab";
        public static readonly string FIXTURESELECTED = "fixtureSelected";
        public const string OPENINGLOCATIONSTAB = "OPENINGLOCATIONS";
        public const string DISPLAYVARIABLEASSIGNMENTS = "displayVariableAssignments";
        public const string OVERWRITE = "OVERWRITE";
        /// <summary>
        /// Constants Related to Session
        public static readonly string INPUTMESSAGE = "Provided input parameter is wrong.";
        public static readonly string DATETIMESTRING = "yyyy/MM/dd HH:mm:ss.ff ";
        public static readonly string EXERRORMESSAGE = "Something went wrong";

        /// </summary>
        #region Session related constants
        public static readonly string TKEUUID = "TkeUuid";

        public static readonly string CPQ = "CPQ";
        #endregion

        #region Variable Assignment related constants

        public static readonly string VARIABLEASSIGNMENTS = "variableAssignments";
        #endregion
        #region Configuration related constants

        public static readonly string BUILDINGCONFIGURATIONREQESTBODYSTUBPATH = @"Templates\Building\BaseRequestbody.json";
        public static readonly string GROUPCONFIGURATIONREQESTBODYSTUBPATH = @"SampleStubData\GroupConfiguration\GroupConfigurationRequestbody.json";

        #endregion


        #region switch case related variables
        public const string DUPLICATEOPERATION = "DUPLICATE";
        #endregion

        public static readonly string LOGDIRECTORY = "Logs/";
        public static readonly string JSONFILEFILTER = "*.json";
        public static readonly string JSONEXTENTION = ".json";
        public static readonly string CURRENTLOGFILENAME = "log";
        public static readonly string GROUPS = "groups";

        public static readonly Dictionary<string, string> SERILOGPROPERTIES = new Dictionary<string, string>() {
                                              {"@t","TimeStamp" }, {"@m","Message" }, {"@l","Level" }, {"@i","EventId" }, {"@x","Exception"} };
    }
}
