using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.IO;

namespace TKE.SC.BFF.BusinessProcess.Helpers
{
    public class Constant
    {


        #region Logging related constants


        public static readonly string FORBIDDENERROR = "Invalid User";
        public static readonly string CACHEEMPTY = "Cache is Empty";
        public static readonly string GROUPSEMPTY = "No Groups Available For Selection";

        public static readonly string SESSIONEXPIRYMESSAGE = "Session got expired";

        public static readonly string SOMETHINGWENTWRONG = "Something went wrong:(";
        public static readonly string INETRNALSERVERERRORMSG = "Internal Server Error. Unable to fetch data";
        public static readonly string BADREQUESTMSG = "Bad Request";
        public const string BADREQUESTTEXT = "BADREQUEST";



        public static readonly string NOAUTHENTICATIONSCHEME = "No authenticationScheme was specified, and there was no DefaultChallengeScheme found.";
        public static readonly string LOGOFFSUCESSMESSAGE = "Logout Sucessfully.";
        public static readonly string VIEWERROEMESSAGE = "Error encountered while getting project details from VIEW.";
        public static readonly string INAVLIDGROUPID = "No Configuration Found for given GroupId.";
        public static readonly string DEFAULTVALUES = "DefaultValues";
        public static readonly string OCCUPIEDSPACEBELOW = "Occupied Space Below";
        public static readonly string RELEASEINFOLIST = "ReleaseInfoList";
        public static readonly string FDALIST = "FDALIST";
        public static readonly string RELEASEINFO = "ReleaseInfo";
        public static readonly string FDA = "FDA";
        public static readonly string DUPLICATEOVERWRITE = "Cannot Duplicate Existing Version Under This Project ";

        public static readonly string VIEWTOCANADAUSERERRORMESSAGE = "VIEWTOCANADAUSERERRORMESSAGE";
        public static readonly string CANADATOVIEWUSERERRORMESSAGE = "CANADATOVIEWUSERERRORMESSAGE";
        public static readonly string USERLOGINERRORTYPE= "USERLOGINERRORTYPE";



        public static readonly string FDANOTNULLERRORMESSAGE = "Please give the correct groupid and quoteid";
        public static readonly string BUILDINGNAMEALREADYEXISTS = "Building Name already exists.";
        public static readonly string BUILDINGSAVEISSUES = "Building Has EVO_200 Models. Please Change the Building Power Phase to 3";
        public static readonly string FLOORDESIGNATIONREPEATING = "Floor Designation is repeating";
        public static readonly string FLOORDESIGNATIONREPEATINGDESCRIPTION = "Floor Designation is repeating,Please have it unique";
        public static readonly string RELEASEINFOERROR = "Error Occurred while Releasing the Project";
        public static readonly string SOMEFIELDSAREMISSING = "Some fields are missing";
        public static readonly string SOMEFIELDSAREMISSINGDESCRIPTION = "Some fields are missing,Fill them and check again";
        public static readonly string SAVEBUILDINGEQUIPMENTCONSOLEERRORMSG = "Error occured while saving building equipment console";
        public static readonly string SAVEBUILDINGEQUIPMENTCONSOLEDESCRIPTION = "Error occured while saving building equipment console";
        public static readonly string DUPLICATEBUILDINGEQUIPMENTCONSOLEERRORMSG = "Error Occurred while duplicating the console";
        public static readonly string UPDATEGROUPLAYOUTERROR = "Error Occurred while updating the groupLayout";
        public static readonly string SOMEERROROCCURED = "Error Occurred while saving/updating the Project";
        public static readonly string DELETEBUILDINGEQUIPMENTSUCCESSMSG = "BuildingEquipmentConsole deleted sucessfully";
        public static readonly string CONSOLEDOESNOTEXIST = "Console Does not exist";
        public static readonly string SAVEBUILDINGEQUIPMENTERRORMSG = "Error occurred while saving building equipment selections";

        public static readonly string SESSIONIDPARENTCODEISNOMOREVALID = "SessionId or ParentCode is no more valid";
        public static readonly string REQUESTCANNOTBENULL = "Request can not be null";

        public static readonly string INVALIDEXCEPTIONLOGGING =
            "User is not having required permission to do this action";

        public static readonly string DELETEUNITHALLFIXTURECONFIGURATIONBYIDINITIATE = " DeleteUnitHallFixtureConfigurationById BL call Initiated";
        public static readonly string DELETEUNITHALLFIXTURECONFIGURATIONBYIDCOMPLETE = " DeleteUnitHallFixtureConfigurationById BL call Completed";

        public static readonly string DELETEGROUPHALLFIXTURECONFIGURATIONBYIDINITIATE = " DeleteGroupHallFixtureConfigurationById BL call Initiated";
        public static readonly string DELETEGROUPHALLFIXTURECONFIGURATIONBYIDCOMPLETE = " DeleteGroupHallFixtureConfigurationById BL call Completed";

        #endregion

        #region Settings related constants

        public static readonly string APPSETTINGS = "appsettings.json";
        public static readonly string PARAMSETTINGS = "ParamSettings";
        public static readonly string WRAPPERAPI = "WrapperAPIAuthentication";
        public static readonly string SERILOG = "Serilog";
        public static readonly string INFYJSONLOGGERPATH = "InfyJsonLoggerPath";
        public static readonly string PRODUCTNAMEPARAM = "ProductName";
        public static readonly string PRODUCTIONCHECKSETTINGS = "ProductionCheckSettings";
        public static readonly string UISETTINGS = "UISettings";
        public static readonly string DOCUMENTGENERATOR = "DocumentGenerator";
        public static readonly string TTL = "TTL";
        public static readonly string DDMMYYYY = "dd.MM.yyyy";
        public static readonly string HHMMSS = "HH:mm:ss";
        public static readonly string YYYYMMDD = "yyyyMMdd";
        public static readonly string DATE = "Date:-";
        public static readonly string BASEURL="BaseUrlForConfiguration";
        public static readonly string CONFIGURATIONPATH="ConfigurationPath";
        public static readonly string BUILDINGCONFIGURATIONURL = "BuildingConfigurationUrl";
        public static readonly string GROUPCONFIGURATIONURL = "GroupConfigurationUrl";
        public static readonly string UNITCONFIGURATIONURL = "UnitConfigurationUrl";
        #endregion

        #region Special characters

        public static readonly string EQUALTO = "=";
        public static readonly string DOUBLEEQUALTO = "==";
        public static readonly string SUBSTRACTION = "-";
        public static readonly string HYPHEN = "-";
        public static readonly char HYPHENCHAR = '-';
        public static readonly string ADDITION = "+";
        public static readonly string UNDERSCORE = "_";
        public static readonly char UNDERSCORECHAR = '_';
        public static readonly string SLASH = "/";
        public static readonly string DOT = ".";
        public static readonly string PERCENT = "%";
        public static readonly string SPACE = "";
        public static readonly char SPACECHAR = ' ';
        public static readonly string EMPTYSPACE = " ";
        public static readonly string COMMA = ",";
        public static readonly string COLON = ":";
        public static readonly string OPENSQUAREBRACKET = "[";
        public static readonly string CLOSESQUAREBRACKET = "]";
        public static readonly char COMMACHAR = ',';
        public static readonly char EQUALTOCHAR = '=';
        public static readonly char SEMICOLON = ';';
        public static readonly char SEMICOLAN = ';';
        public static readonly char DOTCHAR = '.';
        public static readonly char QUESTIONMARK = '?';
        public static readonly string TXT = ".txt";
        public static readonly char TILDE = '~';
        public static readonly char SLASHCHAR = '/';
        public static readonly string OPENROUNDBRACKET = "(";
        public static readonly string CLOSEDROUNDBRACKET = ")";
        public static readonly string STRING_F = "F - ";
        public static readonly string STRING_R = "R - ";
        public static readonly string SPACE_COMMA = ", ";
        public static readonly string OR = " | ";
        public static readonly string HYPHENWITHSPACE = " - ";
        public static readonly string GREATERTHAN = ">";
        #endregion
        #region StatusCode related constants

        public static readonly int INTERNALSERVERERROR = StatusCodes.Status500InternalServerError;
        public static readonly int SERVICEUNAVAILABLE = StatusCodes.Status503ServiceUnavailable;
        public static readonly int CREATED = StatusCodes.Status201Created;
        public static readonly int SUCCESS = StatusCodes.Status200OK;
        public static readonly int BADREQUEST = StatusCodes.Status400BadRequest;
        public static readonly int UNAUTHORIZED = StatusCodes.Status401Unauthorized;
        public static readonly int FORBIDDEN = StatusCodes.Status403Forbidden;
        public static readonly int NOTFOUND = StatusCodes.Status404NotFound;
        public static readonly int INVALIDCONFIG = StatusCodes.Status409Conflict;
        public static readonly int CONFLICT = StatusCodes.Status409Conflict;

        #endregion

        /////////////////////
        #region Caching related constants
        public static readonly string CPQ = "CPQ";

        public static readonly string ENVIRONMENT = "Environment";
        public static readonly string DEV = "DEV";
        public static readonly string PRICELISTREFNO = "PriceListRefNo";

        public static readonly string USERINFOCPQ = "USERINFO";
        public static readonly string USERDETAILSCPQ = "USERDETAILS";
        public static readonly string UNITDETAILSCPQ = "UNITDETAILS";
        public static readonly string CROSSPACKAGEVARIABLEASSIGNMENTS = "CROSSPACKAGEVARIABLEASSIGNMENTS";
        public static readonly string FIXTUREASSIGNAMENT = "FIXTUREASSIGNAMENT";
        public static readonly string MAINFIXTUREASSIGNAMENT = "MAINFIXTUREASSIGNAMENT";
        public static readonly string USERADDRESS = "USERADDRESS";
        public static readonly string POST = "POST";
        public static readonly string VIEWDATA = "VIEWDATA";
        public static readonly string CURRENTMACHINECONFIGURATION = "CURRENTMACHINECONFIGURATION";
        public static readonly string PRICESECTION = "PRICESECTION";
        public static readonly string MACHINEREQUESTCPQ = "MACHINEREQUEST";   

        public static readonly string SUBLINESRESPONSE = "SUBLINESRESPONSE";
        public static readonly string PRICERESPONSECPQ = "PRICERESPONSE";

        public static readonly string PREVIOUSSERVICECPQ = "PREVIOUSSERVICE";

        public static readonly string KEYBUNCH = "KEYBUNCH";

        public static readonly string PERSONA = "Persona";

        public static readonly string PRODUCTTYPE = "PRODUCTTYPE";

        public static readonly string DWGPENDING = "DWG_PEN";
        public static readonly string DWGSUBMITTED = "DWG_SUBD";

        public const string LIFTDESIGNERBRANCH = "LIFTDESIGNERBRANCH";
        public const string LIFTDESIGNERARCH = "LIFTDESIGNERARCH";
        public const string LIFTDESIGNERGENCOUNT = "LIFTDESIGNERGENCOUNT";
        public const string LIFTDESIGNERCITY = "LIFTDESIGNERCITY";



        public static readonly string LAYTOUCH = "layTouch";

        public static readonly string LDENRICHMENT = "LDEnrichment";

        public static readonly string ENRICHEDNAMES = "EnrichedNames";

        public static readonly string DEFAULTVALUES_UPPERCASE = "xmlMainDefaultValues";

        public static readonly string CROSSPACKAGEVARIABLESUNITTOLD = "CrossPackageVariablesUnittoLD";

        public static readonly string CROSSPACKAGEVARIABLESSYSTEMBRACKETANDHEATTOLD = "CrossPackageVariablesSystemBracketandHeattoLD";

        public static readonly string SYSTEMVALIDATIONVALUES = "SystemValidationValues";

        public static readonly string MAINSECTIONENRICHEDNAMES = "xmlMainEnrichedNames";



        public static readonly string ELEVATORID = "ELEVATOR";

        public static readonly string SUMPQTYID = "SUMPQTY";











        // PUBLIC_CUSTOMER FOR persona
        public static readonly string PUBLICCUSTOMER = "PUBLIC_CUSTOMER";
        public static readonly string ISPRODUCTIONCHECK = "ISPRODUCTIONCHECK";
        public static readonly string LOCALEWORD = "locale";
        public static readonly string CONFIGURATION = "Configuration";
        public static readonly string BUILDINGMAPPERCONFIGURATION = "BuildingConstantMapper";


        public static readonly string NCPCONFIGURATION = "NCPVariables";

        public static readonly string GROUPMAPPERCONFIGURATION = "GroupConstantMapper";
        public static readonly string DEFAULTVARIABLEASSIGNMENTS = "defaultVariableAssignments";
        public static readonly string UNITTABLE = "UnitTable";
        public static readonly string HALLSTATIONMAPPEROBJECT = "HallStation";
        public static readonly string FLOORPLANRULESMAPPEROBJECT = "floorPlanRules";
        public static readonly string DOORSMATERIALVARIANT = "doorsMaterialVariant";

        public static readonly string BUILDINGEQUIPMENTVARIABLES = "BuildingEquipmentVariables";

        public static readonly string FDAVARIABLES = "fieldDrawingAutomation";

        public static readonly string CUSTOMENGINEEREDVARIABLES = "CustomEngineeredConstantMapper";


        public static readonly string PRODUCTCATEGORYTYPE = "PRODUCTCATEGORY";

        public static readonly string XMLGENERATIONMAPPER = "xmlGenerationMapper";

        public static readonly string CACHEVARIABLESMAPPER = "cacheVariables";

        

        public static readonly string LDVARIABLEMAPPER = "ldVariables";

        public static readonly string BUILDINGANDUNITVARIABLESCONSTANTS = "buildingAndUnitVariablesConstants";

        

        public static readonly string HEATINPUTVARIABLEMAPPER = "heatInput";

        public static readonly string BRACKETINPUTVARIABLEMAPPER = "bracketInput";









        public static readonly string BUILDINGCONFIGURATIONPATH = "BuildingConfigurationPath";

        public static readonly string GROUPCONFIGURATIONPATH = "GroupConfigurationPath";

        public static readonly string UNITVALIDATIONPATH = "UnitValidationPath";

        public static readonly string UNITVALIDATIONENDURA100PATH = "UnitValidationEndura100Path";

        public static readonly string UNITVALIDATIONPATHEVO100PATH = "UnitValidationPathEvo100Path";

        public const string CUSTOMENGINEEREDGEARLESSPATH = "CustomEngineeredGearlessPath";

        public const string CUSTOMENGINEEREDGEAREDPATH = "CustomEngineeredGearedPath";

        public const string CUSTOMENGINEEREDHYDRAULICPATH = "CustomEngineeredHydraulicPath";

        public const string SYNERGYPATH = "SynergyPath";

        public const string CrossPackageVariablesUnittoLD = "CrossPackageVariablesUnittoLD";

        public const string XMLEXTENSION = ".xml";

        public static readonly string BUILDINGGROUPCROSSPACKAGEVARIABLES = "CrossPackageVariables";

        public const string XMLFILEPATH = "\\XMLFiles";

        public const string XMLPREFIXNAME = "WrapperXML_";

        public const string DDMMYY = "ddMMyyyy";

        public const string FORWARDSLASH = "\\";

        public const string LDLAYOUTXML = "LDLayoutXML"; 















        public static readonly string PRODUCTSELECTIONPATH = "ProductSelectionPath";

        public static readonly string PRODUCTTREEPATH = "ProductTree";

        public static readonly string LIFTDESIGNERPATH = "LiftDesignerPath";

        public static readonly string LDVALIDATIONPATH = "LDValidationPath";

        public static readonly string LIFTDESIGNERPATHEVO100 = "LiftDesignerPathEvo100";

        public static readonly string LDPACKAGEIDEVO100 = "LDPackageIdEvo100";


        public static readonly string ESCLATORPATH = "EsclatorPath";

        public static readonly string TWINELEVATORPATH = "TwinElevatorPath";

        public static readonly string OTHERSCREENPATH = "OtherScreenPath";


        public static readonly string HEAT = "HEAT";

        public static readonly string BRACKET = "BRACKET";




        public static readonly string SALESCONFIGURATORQUOTE = "Sales Configurator Quote or ID";

        public static readonly string ENUS = "EN-US";

        public static readonly int DEFAULTPLOTSTYLE = 2;

        public static readonly string REFRERENCEIDGENERATED = "DWG_SUBD";

        public static readonly string REFRERENCEIDERROR = "DWG_ERR";

        public static readonly string ERROR = "DWG_ERR";
        public static readonly string ERRORVALUE = "error";


        public static readonly int DOCGEN = 2;

        public static readonly string GETREQUESTLAYOUTINTERVAL = "GetRequestLayoutInterval";

        public static readonly string VALIDATIONPATH = "ValidationPath";

        public static readonly string CABVALIDATIONPATH = "CabValidationPath";
        public static readonly string SLINGVALIDATIONPATH = "SlingValidationPath";
        public static readonly string EMPTYVALIDATIONPATH = "EmptyValidationPath";
        public static readonly string DUTYVALIDATIONPATH = "DutyValidationPath";
        public static readonly string MATERIALNAMEVALUES = "MATERIALNAMEVALUES";
        public static readonly string MATERIALEXTERNALID = "MATERIALEXTERNALID";




        #endregion

        #region Gigya related constants


        public static readonly string ENTITLEMENT = "Entitlement";
        public static readonly string BEARER = "Bearer";
        public static readonly string ILLEGALSTRINGEXCEPTION = "Illegal base64url string!";

        public static readonly string SESSIONID = "SessionId";

        public static readonly string TKEUUID = "TkeUuid";
        public static readonly string UID = "GigyaUid";



        #endregion

        #region Configuration Service Bl
        public const string GEN_S_COUNTRY = "GEN_S_COUNTRY";
        public const string GEN_S_MODEL = "GEN_S_MODEL";
        public const string GEN_S_MODEL_RANGE = "GEN_S_MODEL_RANGE";
        public const string AUTOGUIDE = "AUTOGUIDE";
        public const string DOCUMENTATION_PACK = "DOCUMENTATION_PACK";
        public const string REVERSE = "REVERSE";
        public const string ROOT = "ROOT";
        #endregion

        #region categories related constants


        public static readonly string F = "F";
        public static readonly string R = "R";
        public static readonly string G = "G";
        public static readonly string PRICEVARIABLES = "PriceVariables";

        #endregion

        public static readonly string ACCESSTOKEN = "access_token";
        public static readonly string VIEWUSERNAME = "ViewUserName";
        public static readonly string VIEWPASSWORD = "ViewPassword";

        public static readonly string VARIANTS = "Variants";
        public static readonly string ROLENAME = "RoleName";
        public static readonly string TRADITIONALHALLSTATIONS = "Traditional Hall Stations";
        public static readonly string DRAWINGTYPESBASEPACKAGE = "drawingTypesBasePackage";



        public static readonly string DRAWINGTYPESEXTERIORPACKAGE = "drawingTypesExteriorArchitecturalPackage";

        public static readonly string DRAWINGTYPESINTERIORPACKAGE = "drawingTypesInteriorArchitecturalPackage";



        public static readonly string AGILEHALLSTATIONS = "AGILE Hall Stations";
        public static readonly string PROJECTBIDAWARDED = "PRJ_BDAWD";

        public static readonly string FDADISPLAYNAME = "displayname";

        public static readonly string LAYOUTDRAWINGS = "Layout Drawings";

        public static readonly string INFOMESSAGE = "infoMessage";

        public static readonly string ARRAYTYPE = "Array";

        public const string SPGGETCARPOSITIONBYGROUPID = "usp_GetCarPositionByGroupId";


        public const string ESCLATORMOVINGWALK = "Escalator/Moving-Walk";

        public const string NONCONFIGURABLEPRODUCTS_PASCALCASE = "NonConfigurableProducts";

        public const string PRODUCTELEVATOR = "Elevator";

        public const string OTHER = "Other";

        public const string TWINELEVATOR = "TWIN Elevator";

        public const string THIRDPARTY = "3RD_PARTY";

        public const string TWIN = "TWIN";


        public const string UNITNAME_LOWERCASE = "unitName";
        public const string PROPERTIES = "properties";

        public const string NULLVALUE = "null";

        public const string TOKENSECTIONS = "sections";
        public const string TOKENID = "id";
        public const string BEAMWALLSECTIONKEY = "fda.beamWall";
        public const string LAYOUTSECTIONKEY = "fda.unitLayoutDetails";
        public const string LAYOUTGENERATIONSETTINGSSECTIONKEY = "fda.layoutGenerationSettings";
        public const string DRAWINGSECTIONKEY = "fda.layoutGenerationSettings.drawingTypes";
        public const string OUTPUTSECTIONKEY = "fda.layoutGenerationSettings.outputTypes";
        public const string HASH = "#";
        public const string BYRULEUPPERCASE = "ByRule";
        public const string BYDEFAULTUPPERCASE = "ByDefault";


        public const string GEARLESSPACKAGEID = "GearlessPackageId";
        public const string GEAREDPACKAGEID = "GearedPackageId";
        public const string HYDRAULICPACKAGEID = "HydraulicPackageId";
        public const string SYNERGYPACKAGEID = "SynergyPackageId";

        public const string LDPACKAGEID = "LDPackageId";
        public const string LDVALIDATIONPACKAGEID = "LDValidationPackageId";
        public const string LDHEATPACKAGEID = "LDHeatPackageId";
        public const string LDBRACKETPACKAGEID = "LDBracketPackageId";

        public const string ESCALATORPACKAGEID = "EscalatorPackageId";
        public const string TWINELEVATORPACKAGEID = "TwinElevatorPackageId";
        public const string OTHERSCREENPACKAGEID = "OtherScreenPackageId";


        public const string BASEPACKAGESECTION1 = "Cover Sheets (One drawing for Group)";
        public const string BASEPACKAGESECTION2 = "Customer Verification Sheet (One drawing for each Set. This document has 'editable' fields that the customer fills in or chooses from an LOV.)";


        public const string EXTERIORSECTION1 = "Entrances (One entrance drawing package per set, each unique entrance configuration will be displayed with floor markings). These entrance drawings will show location/size of hall fixtures as well.";
        public const string EXTERIORSECTION2 = "Hall Fixtures (One drawing for each unit and landing)";


        public const string INTERIORSECTION1 = "Car Fixtures (one interior package for each set)";
        public const string INTERIORSECTION2 = "Cabs (One drawing for each Set)";





        public const string LAYOUTDRAWINGSSECTION1 = "Layout Drawing Package (per group)includes:";

        public const string LAYOUTDRAWINGSSECTION2 = "Layout Section View (per set)";

        public const string LAYOUTDRAWINGSSECTION3 = "Layout (single layout per group):";

        public const string LAYOUTDRAWINGSSUBSECTION1 = "Pit(per group)";

        public const string LAYOUTDRAWINGSSUBSECTION2 = "Hoistway (per group)";

        public const string LAYOUTDRAWINGSSUBSECTION3 = "Overhead view (per group)";

        public const string LAYOUTDRAWINGSSUBSECTION4 = "Rail Stack Page (per set)";

        public static readonly string DRAWINGTYPESECTION = "Parameters.Field_Drawing_Automation_Layout_Generation_Settings.DrawingTypes";

        public static readonly string DRAWINGSAPISETTINGS = "DrawingsApi";

        public static readonly string STARTCONFIGUREBLBLOCKSTARTED = "[Information] StartConfigureBl block started";

        public static readonly string EXCEPTION = "[Information] Exception: ";

        public static readonly string SETCACHEBLOCKSTARTED = "[Information] SetCache block started";

        public static readonly string SUCCESSBLOCKSTARTED = "[Information] success block started";

        public static readonly string GETBASECONFIGUREREQUESTBLOCKSTARTED = "[Information] GetBaseConfigureRequest block started";

        public static readonly string CONFIGURATIONBLBLOCKSTARTED = "[Information] ConfigurationBL block started";

        public static readonly string CRMOD = "CRMOD";
        public static readonly string VIEW = "VIEW";

        public static readonly string XMLITEM = "item";
        public static readonly string XMLNAME = "NAME";
        public static readonly string PROJECTNAMECOLUMNAME = "Name";
        public static readonly string XMLVALUE = "VALUE";
        public static readonly string XMLUNIT = "UNIT";

        public static readonly string XMLHEAD = "<?xml version='1.0' encoding='utf-8'?>";

        public static readonly string XMLBODY = "BODY";

        public static readonly string XMLCOMMON = "COMMON";

        public static readonly string XMLVALUES = "VALUES";
        public static readonly string XMLUNITS = "UNITS";
        public static readonly string MANUALCSC = "ManualCSC";
        public static readonly string AUTOMATED = "Automated";
        public static readonly string PROJECTID = "projectId";
        public static readonly string MODEL = "model";
        public static readonly string TEST = "test";
        public static readonly string ISCOMPLETED = "Completed";
        public static readonly string INCOMPLETESTATUSCODE = "UNIT_INC";
        public static readonly string INCOMPLETESTATUS = "Incomplete";
        public static readonly string STATUSKEYUNIT_INC = "UNIT_INC";
        public static readonly string STATUSKEYUNIT_SVINP = "UNIT_SVINP";
        public static readonly string INCOMPLETEDESCRIPTIONSTATUS = "Unit configuration is not complete";
        public static readonly string INPROGRESSSTATUSCODE = "SysValInProgress";
        public static readonly string INPROGRESSSTATUS = "Validating..";
        public static readonly string INPROGRESSDESCRIPTIONSTATUS = "System validation of the Unit is in progress";
        public static readonly string SYSVALNOTVALIDATE = "SysValNotValidate";
        public static readonly int IS_SAVE = 0;
        public static readonly int IS_UPDATE = 1;
        public const string SUMMARYOPPORTUNITY = "opportunity";
        public const string SUMMARYQUOTEID = "quoteid";
        public const string SUMMARYBUILDING = "building";
        public const string SUMMARYGROUP = "group";
        public const string SUMMARYSET = "set";
        public const string DISPLAYVARIABLEASSIGNMENTS = "displayVariableAssignments";
        public const string BANKTWOPOSITION = "B2P";
        public static readonly string VARIABLEASSIGNMENTS = "variableAssignments";
        public static readonly string FIXTURESELECTED = "fixtureSelected";
        public static readonly string OUTPUTTYPES = "outputTypes";
        public static readonly string DRAWINGTYPES = "drawingTypes";
        public static readonly string DBTYP = "DBTYP";
        public static readonly string WALL = "WALL";
        public static readonly string BEAMWALLWARNINGMESSAGE = "The layout defined has more than 3 cars in a common hoistway. If all cars serve the same portion of the building, a partition is required per IBC code 3002.2";
        public static readonly string NODATACACHE = "No Data in Cache.";
        public const string CAPACITY = "CAPACITY";
        public const string SPEED = "SPEED";
        public const string WIDTH = "WIDTH";
        public const string DEPTH = "DEPTH";
        public const string PITDEPTH = "PIT DEPTH";
        public const string OVERHEAD = "OVERHEAD";
        public const string MACHINETYPE = "MACHINE TYPE";
        public const string MOTORTYPESIZE = "MOTOR TYPE/SIZE";
        public const string AVGFINWEIGHT = "AVAILABLE FINISH WEIGHT";
        public const string GROSSLOADJACK = "GROSS LOAD ON JACKS";
        public const string GROSSLOADPOWERUNIT = "GROSS LOAD ON POWER UNIT";
        public static readonly string CAPACITYPARAMETER = "ELEVATOR.Parameters.CAPACITY";
        public static readonly string CARSPEEDPARAMETER = "ELEVATOR.Parameters.CARSPEED";
        public static readonly string HYDWIDPARAMETER = "ELEVATOR.Parameters.HWYWID";
        public static readonly string HYDDEPPARAMETER = "ELEVATOR.Parameters.HWYDEP";
        public static readonly string PITDEPTHPARAMETER = "ELEVATOR.Parameters.PITDEPTH";
        public static readonly string OVHEADPARAMETER = "ELEVATOR.Parameters.OVHEAD";
        public static readonly string VARIABLEVALUES = "variables";
        public const string CARSPEEDVARIABLE = "CARSPEED";
        public const string CARSPEEDVARIABLEENDURA = "CARSPEEDENDURA";
        public const string TYPSVC = "TYPSVC";
        public const string HYDRAULIC = "CE_Hydraulic";
        public const string ORINOCO = "Orinoco";
        public const string GEARLESS = "CE_Gearless";
        public const string MACHINEAPPLICATION = "Machine_Application";
        public const string JACKTYPE = "Jack_Type";   
        public const string PRODUCTTECHNOLOGY = "Inclined";
        public static readonly string IDPARAM = "id";
        #region Logic related constants

        public static readonly string ISBASKET = "isbasket";
        public static readonly string BYSYSTEM = "BYSYSTEM";
        public static readonly string BYUSER = "BYUSER";
        public static readonly string BYDEFAULT = "BYDEFAULT";
        public static readonly string BYRULE = "BYRULE";
        public static readonly string RESULT = "Result";
        public static readonly string BYUSER_LOWERCASE = "byUser";
        public static readonly string BYDEF = "byDefault";



        public static readonly string REQUIRED = "REQUIRED";
        public static readonly string PRIMARYCOORDINATOR = "PrimaryCoordinator";
        public static readonly string MODELGENSVARIANT = "MODEL.GEN_S_VARIANT";
        public static readonly string ORACLEPROJECTID = "OracleProjectId";
        public static readonly string BUILDINGIDCOLUMNNAME = "BuildingId";
        public static readonly string OPPORTUNITYIDCOLUMNNAME = "OpportunityId";
        public static readonly string VERSIONIDCOLUMNNAME = "VersionId";
        public static readonly string QUOTEIDIDCOLUMNNAME = "QuoteId";
        public static readonly string GROUPIDCOLUMNNAME = "GroupId";
        public static readonly string UNITIDCOLUMNID = "UnitId";
        public static readonly string BUILDINGNAMECOLUMNNAME = "BuildingName";
        public static readonly string GROUPNAMECOLUMNNAME = "GroupName";
        public static readonly string UNITNAMECOLUMNNAME = "UnitName";
        public const string BUILDING = "Building ";
        public const string BUILDINGCAPS = "BUILDING";
        public const string GROUPCAPS = "GROUP";
        public const string GROUP = "Group";
        public const string GROUPLOWERCASE = "group";
        public const string SETLOWERCASE = "set";
        public const string QUOTEVERSION = "QuoteVersion";
        public const string CONTROLOCATIONTYPE = "ControlLocationType";
        public const string CONTROLOCATIONVALUE = "ControlLocationValue";
        public static readonly string UEID = "UEID";
        public static readonly string SETCONFIGURATIONID = "SetId";
        public static readonly string PRODUCTNAMECOLUMN = "ProductName";
        public static readonly string DESIGNATION = "Designation";
        public static readonly string TRAVELFEET = "Travelfeet";
        public static readonly string TRAVELINCH = "TravelInch";
        public static readonly string FRONTOPENING = "FrontOpening";
        public static readonly string REAROPENING = "RearOpening";
        public static readonly string GROUPCONFIGID = "GroupConfigurationId";
        public const string UNIT = "Unit ";
        public static readonly string CREATEDON = "CreatedOn";
        public static readonly string CONFIGVARIABLES = "ConfigureVariables";
        public static readonly string CONFIGUREVALUES = "ConfigureValues";
        public static readonly string PROJECTNAME = "ProjectName";
        public static readonly string PROJECT = "Project";
        public const string UNITCAPS = "UNIT";
        public const string UNITS = "Units ";
        public const string CREATEDSUCCESSFULLY = " created successfully";
        public const string OVERWRITTENSUCCESSFULLY = " overwritten successfully";
        public static readonly string PROJECTIDCOLUMNNAME = "ProjectId";
        public const string FDASAVEMESSAGE = "Field Drawings Details Saved Successfully";

        public const string FDASAVEPROCESSINGMESSAGE = "Drawings is Processing...Please check after sometime.";

        public const string SETID = "setId";
        public const string DUPLICATEOPERATION = "DUPLICATE";
        public const string OVERWRITE = "OVERWRITE";

        public const string BUILDINGPACKAGEPATH = "buildingconfiguration";
        public const string GROUPPACKAGEPATH = "groupconfiguration";
        public const string UNITPACKAGEPATH = "unitvalidation";
        public const string PRODUCTSELECTIONPACKAGEPATH = "productselection";
        public const string INT = "INT";
        public const string BOOLEANUPPERCASE = "BOOLEAN";
        public const string DECIMAL = "DECIMAL";
        public const string BUINDINGTYPE = "BuindingType";
        public const string BUINDINGVALUE = "BuindingValue";
        public const string VARIABLETYPE = "VariableType";
        public static readonly string VALUE = "value";

        public const string VARIABLEVALUE = "VariableValue";

        public const string LIFTDESIGNERPACKAGEPATH = "liftdesigner";



        public const string GLOBALARGUMENTS = "globalArguments";
        public const string MATERIALVARIANT = "MaterialVariant";
        public const string MATERIAL = "Material";
        public const string CONFIGURABLEMATERIALNAME = "ConfigurableMaterialName";
        public const string EXTERNALID = "ExternalId";
        public const string LINE = "line";
        public const string REQPRODUCTID = "ProductId";
        public const string PRODUCT = "product";
        public const string REQID = "id";
        public const string VARIABLEID = "variableId";
        public const string RANGEVALIDATIONMESSAGE = "rangeValidationMessage";





        #endregion


        #region Basket related constants

        public static readonly string ISBASKETPROP = "isbasket";
        public static readonly string BASKETCOMPONENTPROP = "basketcomponent";
        public static readonly string BASKETASSIGNEDPROP = "AssignedByBasket";

        public static readonly string NA = "N.A.";

        #endregion

        public static readonly string REGEN = "ELEVATOR.Parameters.Electrical_System.REGEN";
        public static readonly string COPANDLOCKEDCOMPARTMENT = "carFixture.copAndLockedCompartment";
        public static readonly string CAROPERATINGPANEL = "carFixture.carOperatingPanel";
        public static readonly string CARCALLCUTOUTKEYSWITCH = "Car Call Cutout Keyswitches";
        public static readonly string CARRIDINGLANTERNQUANTITY = "carFixture.carRidingLanternQuantity";
        public static readonly string CARRIDINGLANTERNQUANTITYSP = "CARRIDINGLANTERNQUANTITYSP";
        public static readonly string TOTALDEVICESLOTSSP = "ELEVATOR.Parameters_SP.totalDeviceSlots_SP";
        public static readonly string FACEPLT = "FACEPLT";
        public static readonly int TOTALDEVICELIMIT = 9;
        public static readonly string PARAMETERELECTRICALSYSTEM = "Parameters.Electrical_System";
        public static readonly string ELEVSYSPARAMETER = "ELEVSYSPARAMETER";
        public static readonly string REGIONPARAMETER = "REGIONPARAMETER";
        public static readonly string EVO = "EVO";
        public static readonly string LOCATION = "Location";
        public static readonly string CARPOSITIONLOCATION = "CarPositionLocation";
        public static readonly string BUILDINGCONFIGURATIONVARIABLELIST = "BuildingConfiguration";
        public static readonly string BUILDINGCONSOLECONFIGURATIONVARIABLELIST = "BuildingEquipmentConsoleConfiguration";
        public static readonly string BUILDINGEQUIPMENTCONFIGURATIONVARIABLELIST = "BuildingEquipmentConfiguration";
        public static readonly string CONTROLLOCATIONVARIABLELIST = "ControlLocation";
        public static readonly string UNITCNFIGURATIONVARIABLELIST = "UnitConfiguration";
        public static readonly string PARAMETERlAYOUT = "Parameters.Layout.";
        public const string UNITNAMEREPEATING = "Unit Name/s = already used. Please enter unique names";
        #region UnitBL related constants

        public static readonly string ID = "1";
        public static readonly string NAME = "Project1";
        public static readonly string CLIENTID = "C1";
        public static readonly string CLIENTNAME = "One World Trade Center";
        public static readonly string UNITID = "U1";
        public static readonly string UnitNAME = "Unit1";
        public static readonly int NOOFLANDINGS = 5;
        public static readonly string CAPACITYUNIT = "pounds";
        public static readonly int CAPACITYVALUE = 3000;
        public static readonly string CARSPEEDUNIT = "v/s";
        public static readonly int CARSPEEDVALUE = 30;
        public static readonly string CURRENCYCODE = "USD";
        public static readonly double AMOUNT = 1051034.99;
        public static readonly int STATUSID = 1;
        public static readonly string STATUSVALUE = "Estimate Price";
        public static readonly string CONTROLGROUP = "Group1";
        public static readonly string CONFIGGROUP = "G1";
        public static readonly string PRODUCTID = "Product1";
        public static readonly string PRODUCTNAME = "Evolution 100";

        #endregion

        #region VariableAssignments Related Constants
        //Variable Assignments
        public const string BLDVALIDATIONBLDGNAME = "BldValidation.BLDGNAME";
        public const string BLDVALIDATIONFLOOR = "BldValidation.FLOOR";
        public const string BLDVALIDATIONBLDGRISE = "BldValidation.BLDGRISE";
        public const string BLDVALIDATIONBLDGCODE = "BldValidation.BLDGCODE";
        public const string BLDVALIDATIONASYEAR = "BldValidation.ASYEAR";
        public const string BLDVALIDATIONBPV = "BldValidation.BPV";
        public const string BLDVALIDATIONFREQUENCY = "BldValidation.Frequency";
        public const string BLDVALIDATIONFIRESERVICE = "BldValidation.FireService";
        public const string BLDVALIDATIONFRESERVICECODES = "BldValidation.FreServiceCodes";


        #endregion

        #region Response Message Related Constants
        public static readonly string SAVEMSG = "Saved Successfully";
        public const string OPENINGLOCATIONUPDATE = "{\"message\":\"Opening Location updated successfully\"}";
        public const string UNITDESIGNATIONMESSAGE = "{\"message\":\"UnitDetails updated successfully\"}";
        public const string UNITDESIGNATIONPARAMETERERRORMESSAGE = "{\"message\":\"Unit Designation and Description cannot be empty\"}";
        public const string ERRORUNITSAVEMESSAGE = "{\"message\":\"Unit names should be unique\"}";
        public const string UNITDESCRIPTIONERRORMESSAGE = "{\"message\":\"Unit Details should be unique\"}";
        public const string UNITERRORMESSAGE = "Unit cannot be configured";
        public const string UNITSETERRORMESSAGE = "Units cannot be configured together";
        public const string UNITSETERRORMESSAGEOPENING1 = "Please assign at least two openings in Group Opening Locations to proceed further";
        public const string UNITSETERRORMESSAGEDIFFERENTGROUP = "Selected Unit belong to different Groups";
        public const string UNITSETERRORMESSAGEDIFFERENTSETS = "Selected Unit belong to different Sets";
        public const string UNITSETERRORMESSAGEOPENING2 = "Please assign at least two openings in Group Opening Locations to proceed further";
        public const string UNITSETERRORMESSAGE1 = "Selected Units cannot be configured together";
        public const string UNITSETERRORFORCONFLICTINGOPENINGS = "Cannot Configure a set with Conflicting opening locations selection";
        public const string UNITSETERRORMESSAGEFORDOORS = "Cannot Configure a set with Conflicting doors selection";
        //public const string UNITSETERRORMESSAGEFORDOORS = "Cannot Configure a set with Left/Right Opening doors and Center Opening";
        public const string UNITCONFIGURATIONUPDATEERRORMESSAGE = "Error while Updating Unit Configuration";
        public const string UNITCONFIGURATIONSAVEERRORMESSAGE = "Error while Saving Unit Configuration";
        public const string UNITDESIGNATIONPARAMETERERRORMESSAGE1 = "Unit Designation and Description cannot be empty";
        public const string UNITDESIGNATIONPARAMETERERRORDESCRIPTION = "Please add designation and description";
        public const string UNITDESCRIPTIONERRORMESSAGE1 = "Unit Details should be unique";
        public const string DELETEENTRANCECONSOLEERRORMSG = "Error occurred while deleting entrance console";
        public const string DELETEUNITHALLFIXTURECONSOLEERRORMSG = "Cannot Delete with null values";
        public const string NOTFOUNDERRORMSG = "Value not found";
        public const string PRICEDETAILSSAVEERROR = "Error while saving price details";
        public const string OPENINGLOCATIONUPDATEERRORMESSAGE = "Error while Updating Opening Locations";
        public const string GETOPENINGLOCATIONERRORMESSAGE = "Group Configuration Id cannot be 0";
        public const string MINIPROJECTSAVEERRORMESSAGE = "Error while saving Project Details";
        public const string COUNTRYNAMEERRORMESSAGE = "Country's name cannot have special characters";

        public const string ADDQUOTEERRORMESSAGE = "Add Quote not be created";




        public const string SAVEPRODUCTSELECTION = "Product selection saved successfully";
        public const string PRODUCTSELECTIONERRORMESSAGE = "Selected Units cannot be configured together";
        public const string EDITUNITSINDEPENDENTLY = "New set created for the selected Unit";
        public const string SOMETHINGWENTWRONGMSG = "Something went wrong";
        public const string INVALIDGROUPID = "Invalid Group Id";
        public const string INVALIDINPUT = "Invalid input/s";

        public const string OPENINGLOCATIONUPDATEMESSAGE = "Opening Location updated successfully";
        public const string SAVEPRODUCTSELECTIONMESSAGE = "Product selection saved successfully";
        public const string EDITINDEPENDENTMESSAGE = "New set created for selected Unit";
        public const string UPDATEENTRANCECONFIGURATIONMESSAGE = "Entrance Configuration Updated Successfully";
        public const string SAVEENTRANCECONFIGURATIONMESSAGE = "Entrance Configuration Saved Successfully";
        public const string INVALIDOPPORTUNITYID = "Invalid opportunity Id";
        #endregion
        public static readonly string PROJECTSALESTAGE = "ProjectSalesStage";        
        public static readonly string UNITVARIABLESFORQUICKSUMMARY = @"Templates\Unit\QuickSummaryVariables.json";
        public static readonly string UNITVARIABLESFORQUICKSUMMARYENDURA = @"Templates\Unit\QuickSummaryVariablesEndura100.json";
        public static readonly string APPGATEWAY_INPUTJSON_PATH = "../../../../TKE.CPQ.AppGateway.Test";
        public static readonly string INPUTJSONPATH = "../TKE.CPQ.AppGateway.Test/InputJson/";
        public static readonly string VIEWEXPORTVARIABLELISTJSON = INPUTJSONPATH + "ViewExportVariables.json";
        public static readonly string BUILDINGNAMEVARIABLEID = "BUILDINGNAMEVARIABLEID";
        public static readonly string BUILDINGCONSTANTMAPPER = "BuildingConstantMapper";
        public static readonly string BUILDINGPARAMETERS = "Building_Configuration.Parameters";
        public static readonly string BUILDINGPARAMETERS_SP = "BUILDINGPARAMETERS_SP";
        public static readonly string ELEVATORCODE = "ELEVATORCODE";
        public static readonly string CONFLICTMANAGEMENT = "conflictManagement";
        public static readonly string CONFLICTASSIGNMENTS = "conflictAssignments";
        public const string HANDRAIL = "ELEVATOR.Parameters.Handrail";
        public const string BUMPERRAIL = "ELEVATOR.Parameters.Bumper_Rail";
        public static readonly string SAVECABINTERIORCARIABLEID = "Elevator.Parameters";
        public static readonly string ENTRANCECONSOLESECTION = "entrances.entranceConfiguration";
        public static readonly string BUILDINGEQUIPMENTCONSOLESECTION = "Building_Configuration.Parameters.";
        public static readonly string PARAMETERLOBBYPANEL = "buildingEquipment.lobbyPanel";
        public static readonly string PARAMETERSMARTRESCUEPHONE5 = "buildingEquipment.smartRescuePhone_5Unit";
        public static readonly string PARAMETERSMARTRESCUEPHONE10 = "buildingEquipment.smartRescuePhone_10Unit";
        public static readonly string PARAMETERROBOTICCONTROLLERINTERFACE = "buildingEquipment.roboticControllerInterface";
        public static readonly string PARAMETERBACNET = "buildingEquipment.bacNet";
        public static readonly string PARAMSMARTRESCUEPHONE5 = "Building_Configuration.Parameters_SP.IsSmartRescue5_Bool_SP";
        public static readonly string ERRORMESSAGE1SMARTRESCUEPHONE5 = "The Smart Rescue Phone 5 fixture cannot be assigned to the group[s] ";
        public static readonly string ERRORMESSAGE2SMARTRESCUEPHONE5 = " as it has more than 5 units";
        public static readonly string ERRORMESSAGEFORNOTCONFIGUREDGROUPS = "There are groups in this building that have not been assigned to a lobby panel";
        public static readonly string KEYWORDLOBBYPANEL = "Lobby Panel";
        public static readonly string KEYWORDSMARTRESCUEPHONE5 = "Smart Rescue Phone, 5-unit";
        public static readonly string KEYWORDSMARTRESCUEPHONE10 = "Smart Rescue Phone, 10-unit";
        public static readonly string KEYWORDROBOTICCONTROLLERINTERFACE = "Robotic Controller Interface";
        public static readonly string KEYWORDBACNET = "BACNet";
        public static readonly string LOCKREG = "ELEVATOR.Parameters.LOCKREG";
        public static readonly string CARCALLOUT = "Car_Operating_Panel_COP.LOCKREG";
        public static readonly string CCREG = "CCREG";
        public static readonly string LOCKOUT = "LOCKOUT";
        public static readonly string DISPLAYNAMEVAL = "displayname";
        public static readonly string LANDINGVARIABLEREAR = "LandingVariableRear";
        public static readonly string LANDINGVARIABLEFRONT = "LandingVariableFront";
        public static readonly string LANDINGVARIABLE = "LandingVariable"; 
        public static readonly string SEQUENCEVAL = "sequence";
        public static readonly string NR = "NR";
        public static readonly string FIRE_KEY_BOX = "Fire_Key_Box";
        public static readonly string FIREBOXCONSTANT = "FIREBOX";
        public static readonly string FIREPHONEJACK = "FIREPHONEJACK";
        public static readonly string FIREBOX = "FIREBOX";
        public static readonly string FIREKEYBOX = "Sales_UI_Screens.Group_HallFixtures_SP.Parameters.User_Interface_Devices.Landing_Operating_Panel_LOP.FIREBOX";
        public static readonly string FEATURESPERPANELL = "FEATURESPERPANELL";
        public static readonly string LayoutGenerationSettings = "Parameters.Field_Drawing_Automation_Layout_Generation_Settings";
        public static readonly string SENDTOCOORDINATIONDATA = "sendToCoordinationData";
        public const string HEADING = "heading";
        public const string SEQUENCE = "sequence";
        public const string BYDEFAULTVALUE = "byDefault";
        public const string SECTIONNAME = "sectionname";
        public const string SECTIONNAMEPASCAL = "sectionName";
        public const string SECTIONS = "sections";
        public static readonly string ISSESCTIONCARD = "isSectionACard";
        public static readonly string NUMBER = "Number";
        public static readonly string BOOLEAN = "Boolean";
        public static readonly string NUMBERLOWER = "number";
        public static readonly string BOOLEANLOWER = "boolean";
        public static readonly string ENTRANCECONSOLE = "EntranceConsole";
        public static readonly string LOBBYPANEL = "Lobby Panel ";
        public static readonly string LOBBYPANELSECTION = "lobbypanelsection";
        public static readonly string BACNETSECTION = "bacnetsection";
        public static readonly string ROBOTICCONTROLLERSECTION = "roboticcontrollerinterfacesection";
        public static readonly string SMARTRESCUEPHONE5SECTION = "smartrescuephone5section";
        public static readonly string SMARTRESCUEPHONE10SECTION = "smartrescuephone10section";
        public static readonly string BACNETSECTIONS = "BACNet Section";
        public static readonly string ROBOTICCONTROLLERSECTIONS = "Robotic Controller Interface Section";
        public static readonly string SMARTRESCUEPHONE5SECTIONS = "Smart Rescue Phone 5 Section";
        public static readonly string SMARTRESCUEPHONE10SECTIONS = "Smart Rescue Phone 10 Section";
        public static readonly string BUILDINGEQUIPMENTCONSOLE = "BuildingEquipmentConsole";
        public static readonly string BUILDINGEQUIPMENTCONSOLECHANGE = "ChangeBuildingEquipmentConsole";
        public static readonly string ENTRANCECONSOLEDELETESUCCESSMESSAGE = "Entrance console deleted successfully";
        public static readonly string ZERO = "0";
        public static readonly string UNITHALLFIXTURECONSOLE = "UnitHallFixtureConsole";
        public static readonly string GROUPHALLFIXTURECONSOLE = "GroupHallFixtureConsole";
        public static readonly string UNITHALLFIXTURECONSOLEVARIABLESPLIT = "Landing_Operating_Panel_LOP.";
        public static readonly string UNITHALLFIXTURECONSOLEVARIABLESPLITFORLIP = "Landing_Indicator_Panel_LIP.";
        public static readonly string HALLLANTERN = "HALL_LANTERN";
        public static readonly string HOISTWAYACCESS = "Hoistway_Access";
        public static readonly string BUILDING_CONFIGURATION = "Building_Configuration";
        public static readonly string ELEVATOR_CONFIGURATION = "ELEVATOR";
        public static readonly string UNITHALLFIXTURECONSOLESECTION = "Elevator.Parameters.";
        public static readonly string LISTOFUNITS = "ListOfUnits";
        public static readonly string REAROPEN = "REAROPEN";
        public const string DISPLAYNAME = "displayname";
        public const string FIXTURETYPE = "fixturetype";
        public const string FIXTURESTRATEGY = "FixtureStrategy";
        public const string CONTROLLERFLOOR ="ControllerFloor";
        public const string CARCALLCUTOUTKEYSWITCHESCONSOLE = "CarCallCutoutKeyswitchesConsole";
        public const string GETVARIABLESCACHE = "GETVARIABLESCACHE";
        public static readonly string DEVICESLOTS = "DEVICESLOTS";
        public static readonly string ELEVATOR_PARAMETERS_SP = "ELEVATOR.Parameters_SP";
        public static readonly string DEVICESLOTERRORMESSAGE = "Total number of device slots has exceeded the maximum limit of 9. Remove or move some devices to another location";
        public const string FIXTURESTRATEGY_SP = "fixtureStrategy_SP";
        public const string HALLSTATIONPARAM = "Parameters_SP.HS";
        public const string PRICEKEYDETAILS = "PriceKeyDetails";
        public static readonly string OPENFRONT = "OPENFRONT";
        public static readonly string HOISTWAYDIMENSIONVARIABLE = "HOISTWAYDIMENSIONVARIABLE";
        public static readonly string PREFIXELEVATOR = "PREFIXELEVATOR";
        public static readonly string DEFAULTFRONTHANDVARIABLE = "DEFAULTFRONTHANDVARIABLE";
        public static readonly string DEFAULTREARHANDVARIABLE = "DEFAULTREARHANDVARIABLE";
        public static readonly string FRONTHANDVARIABLE = "FRONTHANDVARIABLE";
        public static readonly string REARHANDVARIABLE = "REARHANDVARIABLE";
        public static readonly string OPENREAR = "OPENREAR";
        public static readonly string BUMPERHEIGHT = "BUMPERHEIGHT";
        public static readonly string TOTALOPENINGS_SP = "ELEVATOR.Parameters_SP.TOTALOPENINGS_SP";
        public static readonly string PARAMETERDOORS = ".Parameters.Doors.";
        public static readonly string ENTRANCECONFIGURATIONVARIABLEIDSPLIT = "Landing_Doors_Assembly.";
        public static readonly string ENTRANCECONFIGURATIONCONSOLE = "ELEVATOR.Parameters.Entrance_Configurations.EntranceConsole";
        public static readonly string ENTRANCECONFIGURATIONCONSOLEMASTER = "ELEVATOR.Parameters.Entrance_Configurations_Master";
        public static readonly string BUILDINGNAME = "building";
        public static readonly string OPPORTUNITY = "Opportunity";
        public static readonly List<string> BUILIDINGCONFIGURATION = new List<string> { "Building_Configuration", "Building_Configuration.Building_Configuration.Parameters" };
        public const string GROUPCONFIGURATIONNAME = "group";
        public const string LIFTDESIGNERNAME = "liftDesigner";
        public const string UNITCONFIG = "unitConfiguration";
        public const string ELEVATORFIXTURESTRATEGY = "ELEVATOR.Parameters_SP.fixtureStrategy_SP";
        public const string FIXTURESTRATEGYVRAIBLEID = "FIXTURESTRATEGYVRAIBLEID";
        public const string FIXTURESTRATEGYID = "FIXTURESTRATEGYVRAIBLEID";
        public const string PRICINGCROSSVARIABLES = "PricingCrossVariables";
        public const string TOTALPRICE = "TotalPrice";
        public const string PRODUCTTREE = "ProductTree";
        public const string BATCH1LEADTIME = "batch1LeadTime";
        public const string BATCH2LEADTIME = "batch2LeadTime";
        public const string BATCH3LEADTIME = "batch3LeadTime";
        public const string BATCH4LEADTIME = "batch4LeadTime";
        public const string BATCH5LEADTIME = "batch5LeadTime";
        public const string MANUFACTURINGLEADTIME = "manufacturingLeadTime";
        public static readonly string FEETINCHVARIABLESLIST = "Templates/Building/FeetInchConversionVariableList.json";
        public static readonly string BUILDINGCONFIGURATIONINCLUDESECTIONVALUES = "Templates/Building/IncludeSections.json";
        public static readonly string BUILDINGEQUIPMENTINCLUDESECTIONVALUES = "Templates/Building/BuildingEquipment/ConsoleIncludeSections.json";        
        public static readonly string INCLUDESECTIONSTEMPLATE = "Templates/Unit/{0}/IncludeSections.json";
        public static readonly string GENERALINFORMATIONINCLUDESECTIONVALUESENDURA100 = "Templates/Unit/Endura100/GeneralInformationIncludeSections.json";        
        public static readonly string CABINTERIORINCLUDESECTIONVALUESENDURA100 = "SampleStubData/UnitConfiguration/Endura100/CABINTERIORINCLUDESECTIONVALUES.json";        
        public static readonly string TRACTIONHOISTWAYEQUIPMENTINCLUDESECTIONVALUESENDURA100 = "SampleStubData/UnitConfiguration/Endura100/TRACTIONHOISTWAYEQUIPMENTINCLUDESECTIONVALUES.json";   
        public static readonly string CARFIXTUREINCLUDESECTIONVALUESENDURA100 = "SampleStubData/UnitConfiguration/Endura100/CARFIXTUREINCLUDESECTIONVALUES.json";
        public static readonly string UNITHALLFIXTUREINCLUDESECTIONVALUESENDURA100 = "SampleStubData/UnitConfiguration/Endura100/UNITHALLFIXTUREINCLUDESECTIONVALUES.json";        
        public static readonly string ENTRANCECONSOLESINCLUDESECTIONVALUESENDURA100 = "SampleStubData/UnitConfiguration/Endura100/EntranceConsoleIncludeSectionStub.json";
        public static readonly string UNITHALLFIXTURECONSOLEINLUDESECTIONVALUESENDURA100 = "SampleStubData/UnitConfiguration/Endura100/UnitHallFixtureConsoleIncludeSectionValues.json";        
        public static readonly string GROUPINLUDESECTIONTEMPATE = "Templates/Group/IncludeSections.json";        
        public static readonly string INTEGRATIONCONSTANTMAPPER = @"Templates\Integration\Constantmapper.json";
        public static readonly string OZ = "OZ";
        public static readonly string FIELDDRAWINGAUTOMATIONINLUDESECTIONVALUES = "Templates/FieldDrawingAutomation/IncludeSections.json";
        public static readonly string LIFTDESIGNERINLUDESECTIONVALUES = "Templates/FieldDrawingAutomation/LiftDesigner/IncludeSections.json";
        public static readonly string HALLLANTERNCAMELCASE = "Hall_Lantern";
        public static readonly string HALLPICAMELCASE = "Hall_PI";
        public static readonly string COMBOHALLLANTERNPICAMELCASE = "Combo_Hall_Lantern/PI";


        public static readonly string NCPINLUDESECTIONVALUES = @"Templates\Group\NCP\IncludeSections.json";
        public static readonly string CUSTOMENGINEERDINLUDESECTIONVALUES = @"Templates\Unit\CustomEngineered\IncludeSections.json";        
        public static readonly string BUILDINGEQUIPMENTLOBBYPANELSUBSECTIONS = @"Templates\Building\BuildingEquipment\LobbyPanelUIResponse.json";
        public static readonly string BUILDINGEQUIPMENTPROPERTIESSTUB = @"Templates\Building\BuildingEquipment\Properties.json";        
        public static readonly string GROUPCONFIGURATIONVARIABLEMAPPER = "GroupConfigurationMapper";
         public static readonly string VARIABLEDICTIONARY = @"Templates\CrossPackage\VariableDictionary.json";
        public static readonly string CROSSPACKAGEVARIABLEMAPPING = "Templates/CrossPackage/VariableDictionary.json";        
        public static readonly string LOCATIONTOELEVQATORMAPPING = "Templates/CrossPackage/CarPositionToElevator.json";
        public static readonly string XMLGENERATIONVARIABLEMAPPING = "Templates/FieldDrawingAutomation/LiftDesigner/XMLGenerationVariables.json";        
        public const string BUILDINGCONFIGURATION = "BUILDINGCONFIGURATION";
        public static readonly string APPENDIXHOSIGNAGE = "Appendix_H/O_Signage";
        public static readonly string APPENDIXHOSIGNAGEFILENAME = "Appendix_HO_Signage";
        public static readonly string PRODUCT_SELECTION = "ProductSelection";
        public const string PRODUCTSELECTIONINVALIDEVO200 = "ProductSelectionInvalidEVO200";
        public const string READONLY = "readOnly";
        public const string READONLY_LOWER = "readonly";
        public static readonly string GROUPDESIGNATIONNAME = "GRPDESG";
        public static readonly string TRAVEL = "TRAVEL";
        public static readonly string JSON = ".json";
        public static readonly string NEWINSTALLATION = "New Installation";
        public static readonly string MODERNIZATION = "Modernization";
        public static readonly string NI = "NI";
        public static readonly string MD = "MD";
        public static readonly string USA = "USA";
        public static readonly string US = "US";
        public static readonly string CA = "CA";
        public static readonly string CANADA = "CANADA";
        public static readonly string COUNTRYVALUE = "AccountDetails.SiteAddress.Country";
        public static readonly string UNITMFGJOBNO="UnitMFGJobNo";
        public static readonly string FDAU1 = "B1P1";
        public static readonly string FDAU2 = "B1P2";
        public static readonly string FDAU3 = "B1P3";
        public static readonly string FDAU4 = "B1P4";
        public static readonly string FDAU5 = "B2P1";
        public static readonly string FDAU6 = "B2P2";
        public static readonly string FDAU7 = "B2P3";
        public static readonly string FDAU8 = "B2P4";
        public static readonly string UNITSTATUS="UnitStatus";
        public static readonly string FDAELEVATOR001 = "ELEVATOR001";
        public static readonly string FDAELEVATOR002 = "ELEVATOR002";
        public static readonly string FDAELEVATOR003 = "ELEVATOR003";
        public static readonly string FDAELEVATOR005 = "ELEVATOR005";
        public static readonly string FDAELEVATOR006 = "ELEVATOR006";
        public static readonly string FDAELEVATOR007 = "ELEVATOR007";
        public static readonly string PROJECTSTATUS= "ProjectStatus";
        public static readonly string LEADTIME = "projectedLeadTime";

        //GroupVariable mapper
        public static readonly string HALLSTATION = "hallStation";
        public static readonly string FRONTDOOR = "frontDoor";
        public static readonly string REARDOOR = "rearDoor";

        public static readonly string REFRESHTYPE = "REFRESHTYPE";
        public static readonly string CUSTOMERACCOUNT = "CustomerAccount";
        public static readonly string GROUPTOBUILDINGCROSSPACKAGEVARIABLE = "GroupToBuilding";
        public static readonly string BUILDINGTOUNITCROSSPACKAGEVARIABLE = "BuildingToUnit";
        public static readonly string GROUPTOUNITCROSSPACKAGEVARIABLE = "GroupToUnit";
        public static readonly string LOBBYRECALLSWITCHVARAIBLES = "LOBBYRECALLSWITCHVARAIBLES";

        #region ConfigurationBL related Constants       
        public static readonly string STARTGROUPCONFIGURATIONSTUBVALIDATEDATAPATH = @"SampleStubData\GroupConfiguration\GroupConfigurationValidateData.json";
        public static readonly string STARTGROUPLAYOUTCONFIGURATIONSTUBPATH = @"SampleStubData\GroupConfiguration\GroupLayoutConfigurationStub.json"; 
        public static readonly string GROUPLANDINGSLISTDATA = "SampleStubData/GroupConfiguration/GroupLandingsList.json";

        public static readonly string PRODUCTSELECTIONINVALIDEVO200STUBPATH = @"SampleStubData\GroupConfiguration\ProductSelectionInvalidEVO200Stub.json";
        public static readonly string PRODUCTSELECTIONUNITLEVELVALIDATIONREQUESTBODYPATHPATH = @"SampleStubData\ProductSelection\ProductSelectionUnitLevelvalidationrequestBody.json";
        public static readonly string PRODUCTSELECTIONGROUPLEVELVALIDATIONEVO200REQUESTBODYPATHPATH = @"SampleStubData\ProductSelection\ProductSelectionGroupLevelValidationEVO_200RequestBody.json";
        
        public static readonly string GENERALINFORMATIONTEMPLATE = @"Templates\Unit\{0}\GeneralInformationUIResponse.json";


        public static readonly string ESCLATORSTUBPATH = @"Templates\Group\NCP\Escalator\UIResponse.json";

        public static readonly string TWINELEVATORSTUBPATH = @"Templates\Group\NCP\TWIN Elevator\UIResponse.json";

        public static readonly string OTHERSCREENSTUBPATH = @"Templates\Group\NCP\Other\UIResponse.json";



        public static readonly string CUSTOMENGINEEREDGEARLESSSTUBPATH = @"Templates\Unit\CustomEngineered\Gearless\UIResponse.json";

        public static readonly string CUSTOMENGINEEREDGEAREDSTUBPATH = @"Templates\Unit\CustomEngineered\Geared\UIResponse.json";

        public static readonly string CUSTOMENGINEEREDHYDRAULICSTUBPATH = @"Templates\Unit\CustomEngineered\Hydraulic\UIResponse.json";

        public static readonly string SYNERGYSTUBPATH = @"Templates\Unit\CustomEngineered\Synergy\UIResponse.json";





        public static readonly string STARTBUILDINGCONFIGURATIONPARAMETERSSP_STUBPATH = @"Templates\Building\BuildingConfigurationParametersSP_Response.json";
        
        public static readonly string BUILDINGDEFAULTSCLMCALLFILEPATH = @"Templates\Building\CLMRequestBody.json";
        public static readonly string UNITDEFAULTSCLMCALLFILEPATH = @"Templates\Unit\Evolution200\ClmRequest.json";
        public static readonly string UNITDEFAULTSCLMCALLFILEPATHEVO100 = @"Templates\Unit\Evolution100\ClmRequest.json";
        public static readonly string GROUPCLMREQUESTTEMPLATE = @"Templates\Group\Elevator\CLMRequest.json";
        public static readonly string GROUPCONFIGURATIONSMAINUIRESPONSE = @"Templates\Group\Elevator\MainUIResponse.json";

        public static readonly string GROUPHALLFIXTUREUITEMPLATE = @"Templates\Group\Elevator\HallFixtures\UIResponse.json";

        public static readonly string GROUPHALLFIXTURECONSOLEPATH = @"Templates\Group\Elevator\HallFixtures\Consoles.json";

        


        public static readonly string STARTFIELDDRAWINGAUTOMATIONMAINTEMPLATEPATH = @"Templates\FieldDrawingAutomation\UIResponse.json";  
        

        public static readonly string UNITENRICHMENTSTEMPLATE = @"Templates\Unit\{0}\Enrichments.json";
        public static readonly string USERINPUTVARAIBLESTEMPLATE = @"Templates\Unit\{0}\TP2Summary\UserVariables.json";     
     
        public static readonly string PROPERTIESTEMPLATE = @"Templates\Building\Properties.json";

        public static readonly string BUILDINGEQUIPMENTCONSOLETEMPLATEPATH = @"Templates\Building\BuildingEquipment\UIResponse.json";
        public static readonly string BUILDINGEQUIPMENTLOBBYPANELRESPONSE = @"Templates\Building\BuildingEquipment\LobbyPanelUIResponse.json"; 

        public static readonly string CARFIXTURETEMPLATE_ETA = @"Templates\Unit\{0}\CarFixturesETAUIResponse.json";
        public static readonly string CARFIXTURETEMPLATE = @"Templates\Unit\{0}\CarFixturesUIResponse.json";        

        public static readonly string CARFIXTUREENDURA_TEMPLATE = @"Templates\Unit\{0}\CarFixturesUIResponse.json";
        public static readonly string PROPERTYTEMPLATE = @"Templates\Unit\{0}\HoistwayMinMaxProperties.json";
        public static readonly string UNITCARFIXTUREMAPPERCONFIGURATION = "CarFixtureConstantMapper";

        public static readonly string CARFIXTURECOMPARTMENTPATH = @"Templates\Unit\{0}\CarfixtureCompartment.json";        

        public static readonly string CARFIXTURESUBSECTIONSTUBPATHENDURA100 = @"SampleStubData\UnitConfiguration\Endura100\CarFixtureWithSectionsStubResponse.json";
        public static readonly string CARFIXTURESUBSECTIONSTUBPATHEVOLUTION100 = @"SampleStubData\UnitConfiguration\\CarFixtureWithSectionsStubResponse.json";
        public static readonly string UNITHALLFIXTUREPATH = @"Templates\Unit\{0}\UnitHallFixtures\UIResponse.json";
        public static readonly string UNITHALLFIXTURECONSOLESPATH = @"Templates\Unit\{0}\UnitHallFixtures\Consoles.json";
        public static readonly string UNITHALLFIXTURECONSOLESFORQUANTITYPATH = @"Templates\Unit\{0}\UnitHallFixtures\ConsolesForCarQuantity.json";
        public static readonly string GROUPHALLFIXTURESRESPONSEPATH = @"SampleStubData\GroupConfiguration\GroupHallFixturesResponse.json";
        public static readonly string UNITHALLFIXTUREPROPERTIESSTUBPATHENDURA100 = @"SampleStubData\UnitConfiguration\Endura100\UnitHallFixtureProperty.json";

        public static readonly string UNITHALLFIXTUREADDFIXTURESTUBENDURA100 = @"SampleStubData\UnitConfiguration\Endura100\UnitHallFixturesAddFixtureList.json";
        
        public static readonly string BUILDINGCONFIGURATIONREQESTBODYSTUBPATH = @"Templates\Building\CLMRequestBody.json";
        


        public static readonly string NCPCLMREQESTBODYSTUBPATH = @"Templates\Group\NCP\CLMRequest.json";


        public static readonly string CLMREQESTBODYSTUBPATH = @"Templates\FieldDrawingAutomation\LiftDesigner\CLMRequest.json";
        public static readonly string SYSTEMVALIDATIONBASEREQUEST = @"Templates\Unit\{0}\CLMRequest.json";
       
        public static readonly string LIFTDESIGNERXMLGENERATIONMAPPERSTUBPATH = @"Templates\FieldDrawingAutomation\LiftDesigner\ConstantMapper.json";
        public static readonly string PRODUCTSELECTIONGROUPLEVELSTUBRESPONSEPATH = @"SampleStubData\ProductSelection\ProductSelectionGroupLevelValidationEVO_200StubResponse.json";
        public static readonly string PRODUCTSELECTIONUNITLEVELSTUBRESPONSEPATH = @"SampleStubData\ProductSelection\ProductselectionUnitLevelvalidationResponseStub.json";
        public static readonly string GROUPCONFIGURATIONPRODUCTCATEGORY = @"Templates\Group\Elevator\GroupConfigurationProductCategory.json";
        public static readonly string UNITCONFIGURATIONREQESTBODYSTUBPATH = @"Templates\Unit\Evolution200\ClmRequest.json";       
        public static readonly string CUSTOMENGINEEREDREQESTBODYSTUBPATH = @"Templates\Unit\CustomEngineered\CLMRequest.json";
        public static readonly string UNITTUIRESPONSETEMPLATE = @"Templates\Unit\UIResponse.json";        
        public static readonly string GROUPINFOUIRESPONSEPATH = @"Templates\Group\GroupInfo\GroupInfoUIResponse.json";
        public static readonly string GROUPINFOENRICHMENTPATH = @"Templates\Group\GroupInfo\Enrichments.json";        
        public static readonly string UNITTABLEVALUESSTUBDATA = @"Templates\Group\UnitTableValues.json";        
        public static readonly string HALLSTATIONVALUEUPDATED = @"Templates\Group\HallStationsData.json";    
       
        public static readonly string SUMMARYINCLUDESECTIONVALUES = @"Templates\Unit\Evolution200\Tp2Summary\SummaryIncludeSection.json";

        public static readonly string FLOORPLANRULESGROUPLAYOUT = @"Templates\Group\FloorPlanRules.json";

        public static readonly string BUILDINGEQUIPMENTUITEMPLATE = @"Templates\Building\BuildingEquipment\UIResponse.json";



        public static readonly string BUILDINGCONFIGURATIONTABSTUBPATH = @"Templates\Building\SectionTab.json";

        public static readonly string GROUPCONFIGURATIONTABSTUBPATH = @"SampleStubData\GroupConfiguration\GroupMainSectionTab.json";

        public static readonly string UNITENRICHEDDATA = @"Templates\Unit\Evolution200\Enrichments.json";

        public static readonly string UNITENRICHEDDATAENDURA100 = @"Templates\Unit\Endura100\Enrichments.json";

        public static readonly string UNITENRICHEDDATAEVOLUTION100 = @"Templates\Unit\Evolution100\Enrichments.json";

        public static readonly string NCPENRICHEDDATA = @"Templates\Group\NCP\{0}\Enrichments.json";

        public static readonly string GROUPINFOENRICHEDDATA = @"Templates\Group\GroupInfo\Enrichments.json";

        public static readonly string CUSTOMENGINEEREDENRICHEDDATA = @"Templates\Unit\CustomEngineered\{0}\Enrichments.json";


        

        public static readonly string BUILDINGENRICHEDDATA = @"Templates\Building\Enrichments.json";

        public static readonly string FDAENRICHEDDATA = @"Templates\FieldDrawingAutomation\Enrichments.json";

        

        

        public static readonly string GROUPUNITPOSITIONDISPLAYNAME = @"SampleStubData\GroupConfiguration\UnitPositionDisplayName.json";

        public static readonly string RANGEJOBJECT = @"Templates\Unit\RangeIdValuesForUnitConfig.json";
        public static readonly string UNITSCONSTANTMAPPER = @"Templates\Unit\ConstantMapper.json";

        public static readonly string VARIABLESDATAVALUES = @"Templates\Integration\VariablesData.json";

        public const string SUMMARYTAB = "SUMMARYTAB";

        public static readonly string OZREQUESTBODY = @"Templates\Integration\OzRequestPayload.json";
        public const string INTERGROUPEMEGENCYPOWER= "Building_Configuration.Parameters_SP.interGroupEmergencyPower_SP";
        public const string BUILDINGEQUIPMENT = "BUILDINGEQUIPMENT";
        public const string FIELDDRAWINGAUTOMATION = "FIELDDRAWINGAUTOMATION";
        public const string NONCONFIGURABLEPRODUCTS = "NONCONFIGURABLEPRODUCTS";
        public const string LIFTDESIGNER = "LIFTDESIGNER";
        public const string GROUPCONFIGURATION = "GROUPCONFIGURATION";
        public const string GROUPLAYOUTCONFIGURATION = "GROUPLAYOUTCONFIGURATION";
        public const string GROUPVARIABLESFORFLAGS_UPPER = "GROUPVARIABLESFORFLAGS";

        public const string UNITCONFIGURATION = "UNITCONFIGURATION";
        public const string OPENINGLOCATION = "OPENINGLOCATION";
        public const string PRODUCTSELECTION = "PRODUCTSELECTION";
        public const string PRODUCTSELECTION_LOWER = "productslection";
        public const string ESCLATORCONFIGURATION = "ESCLATORCONFIGURATION";
        public const string TWINELEVATORCONFIGURATION = "TWINELEVATORCONFIGURATION";
        public const string OTHERSCREENCONFIGURATION = "OTHERSCREENCONFIGURATION";

        public const string CUSTOMENGINEEREDGEARLESSCONFIGURATION = "CUSTOMENGINEEREDGEARLESSCONFIGURATION";
        public const string CUSTOMENGINEEREDGEAREDCONFIGURATION = "CUSTOMENGINEEREDGEAREDCONFIGURATION";
        public const string CUSTOMENGINEEREDHYDRAULICCONFIGURATION = "CUSTOMENGINEEREDHYDRAULICCONFIGURATION";
        public const string SYNERGYCONFIGURATION = "SYNERGYCONFIGURATION";


        public const string PRODUCTSELECTIONUNITLEVELVALIDATION = "PRODUCTSELECTIONUNITLEVELVALIDATION";
        public const string PRODUCTSELECTIONGROUPLEVELVALIDATIONEVO200 = "PRODUCTSELECTIONGROUPLEVELVALIDATIONEVO200";
        public const string GROUPVALIDATIONPARAMETER = "Parameters.Basic_Info";
        public const string GROUPDESIGNATION = "GROUPDESIGNATION";
        public const string PRODUCTCATEGORY = "PRODUCTCATEGORY";
        public const string NOOFUNITS = "noOfUnits";

        public const string ESCALATORMOVINGWALK = "Escalator/Moving-Walk";
        public const string ESCALATORTYPE = "Escalator";



        public const string UNITCONFIGURATIONLANDINGDOORSASSEMBLY = "ELEVATOR.PARAMETERS.DOORS.LANDING_DOORS_ASSEMBLY";
        public const string UNITNAME = "unitConfiguration";
        public const string ADDFIXTUREPARAMETER = "Elevator.Parameters.Add_Fixtures";
        public const string GROUPADDFIXTUREPARAMETER = "Parameters.Add_Fixtures";
        public const string BUILDINGEQUIPMENTLOWERCASE = "buildingequipment";
        public const string GROUPHALLFIXTURELOWERCASE = "group-hall-fixtures";
        public const string OPENINGLOCATIONLOWERCASE = "opening-locations";
        public const string ISFUTUREELEVATOR = "isFutureElevator";
        public const string ELEVATORCARCALLCUTOUTKEYSWITCHESCONSOLE = "ELEVATOR.Parameters.Car_Operating_Panel.CarCallCutoutKeyswitchesConsole";
        public const string ONE = "1";
        public const string GROUPDEFAULTSCLMCALL = "GROUPDEFAULTSCLMCALL";
        public const string UNITDEFAULTSCLMCALL = "UNITDEFAULTSCLMCALL";
        public const string LIFTDESIGNERDEFAULTSCLMCALL = "LIFTDESIGNERDEFAULTSCLMCALL";
        public const string LDVALIDATIONDEFAULTSCLMCALL = "LDVALIDATIONDEFAULTSCLMCALL";
        public const string BUILDINGDEFAULTSCLMCALL = "BUILDINGDEFAULTSCLMCALL";
        public const string OPENINGLOCATIONPASCALCASE = "Opening Location";
        public const string LIFTDESIGNERHEATDEFAULTSCLMCALL = "LIFTDESIGNERHEATDEFAULTSCLMCALL";
        public const string LIFTDESIGNERBRACKETDEFAULTSCLMCALL = "LIFTDESIGNERBRACKETDEFAULTSCLMCALL";



        public const string GROUPHALLFIXTURES = "GroupHallFixtures";
        public const string OPENINGLOCATIONS = "OpeningLocations";
        public const string FLOORPLANTAB = "FLOORPLANLAYOUT";
        public const string DOORTAB = "DOORS";
        public const string CONTROLROOMTAB = "CONTROLROOM";
        public const string RISERLOCATIONSTAB = "HALLRISER";
        public const string GROUPHALLFIXTURETAB = "GROUPHALLFIXTURES";
        public const string OPENINGLOCATIONSTAB = "OPENINGLOCATIONS";
        public const string HALLSTATIONS = "HallStationVariables";
        public const string HALLSTATIONSFORQUANTITYPARAM = "HallStationsForHallStationQuantityParameter";
        public const string ELEVATORPOSITIONS = "ElevatorPositions";

        public const string ERRORMSGFORRISERANDOPENING = "Please Select the required Riser Location and Opening Locations";
        public const string ERRORMSFFORRISERLOCATION = "Please Select the required Riser Location";
        public const string ERRORMSGFORGROUPOPENING = "Please Select the required Opening Locations";
        public const string TP2SUMMARYERROR = "Please Generate TP2 Price again";
        public const string SYSTEMVALIDATIONSLINGCALL = "SYSTEMVALIDATIONSLING";
        public const string SYSTEMVALIDATIONCABCALL = "SYSTEMVALIDATIONCAB";
        public const string SYSTEMVALIDATIONEMPTYCALL = "SYSTEMVALIDATIONEMPTY";
        public const string SYSTEMVALIDATIONDUTYCALL = "SYSTEMVALIDATIONDUTY";
        public const string SYSTEMVALIDATIONJACKDUTYCALL = "SYSTEMVALIDATIONJACKDUTY";
        public const string SLINGWEIGHTAPI = "SLINGWEIGHTAPI";
        public const string EMPTYCARWEIGHTAPI = "EMPTYCARWEIGHTAPI";
        public const string EMPTYCALL = "EmptyCall";
        public const string CABCALL = "CabCall";
        public const string DUTYCALL = "DutyCall";

        public const string GENERALINFORMATION = "GENERALINFORMATION";
        public const string CABINTERIOR = "CABINTERIOR";
        public const string HOISTWAYTRACTIONEQUIPMENT = "TRACTIONHOISTWAYEQUIPMENT";
        public const string ENTRANCES = "ENTRANCES";
        public const string CARFIXTURE = "CARFIXTURE";
        public const string UNITHALLFIXTURE = "UNITHALLFIXTURE";
        public const string GROUPHALLFIXTURE = "GROUPHALLFIXTURE";
        public const string SECTIONID = "id";
        public const string TP2SUMMARYREPLACEUNIT = "ELEVATOR.Sales_UI_Screens.Unit_HallFixtures_SP";
        public const string SALESUIPARAMETER = "Sales_UI";
        public const string UNITHALLFIXTURELOWER = "unithallfixture";
        public const string UNITHALLFIXTURECONSOLES = "unithallfixtureconsoles";
        public const string GROUPCONSTANTMAPPER = "GroupConstantMapper";
        public const string ENTRANCESLOWERCASE = "entrances";
        public const string GENERALINFORMATIONLOWER = "generalinformation";
        public const string CABINTERIORLOWER = "cabinterior";
        public const string TRACTIONHOISTWAYEQUIPMENTLOWER = "tractionhoistwayequipment";
        public const string CARFIXTURELOWER = "carfixture";

        public const string TP2SUMMARYREPLACEENTRANCE = "ELEVATOR.Sales_UI_Screens.Entrance_Config_SP";
        public const string TP2SUMMARYREPLACENEWVARIABLEID = "ELEVATOR.FloorMatrix.LANDING0";
        public const string ETA = "ETA";
        public const string ETD = "ETD";
        public const string ETA_AND_ETD = "ETA/ETD";
        public const string ZEROOPENINGS = "F - 0, R - 0";
        public const string ZEROFRONTOPENINGS = "F - 0";
        public const string TRADITIONALHALLSTATION = "Traditional_Hall_Stations";

        public const string AGILEHALLSTATION = "AGILE_Hall_Stations";

        public const string FIRESERVICE = "Fire_Service";
        public const string EMERGENCYPOWER = "Emergency_Power";

        public const string DOORS = "ELEVATOR.PARAMETERS.DOORS";
        public const string ELEVATORPARAMUSERINTERFACEDEVICES = "ELEVATOR.PARAMETERS.USER_INTERFACE_DEVICES";
        public const string ELEVATORFLOORMATRIXLANDING = "ELEVATOR.FLOORMATRIX.LANDING001.PARAMETERS.USER_INTERFACE_DEVICES";
        public const string ELEVATORLANDINGDOORSASSEMBLY = "ELEVATOR.FLOORMATRIX.LANDING001.PARAMETERS.DOORS.LANDING_DOORS_ASSEMBLY";
        public const string ELEVATORFLOORMATRIX = "ELEVATOR.FLOORMATRIX";
        public const string ELEVATORPARAMETERSSP = "ELEVATOR.PARAMETERS_SP";
        public const string ELEVATORFLOORMATRIXLANDINGS = "ELEVATOR.FloorMatrix.LANDING";
        public const string PRODUCTMODELSP = "PRODUCTMODELSP";
        public const string TKEFACTORYMODEL = "TKEFACTORYMODEL";

        public const string ELEVATORENTRANCECONFIGURATION = "ELEVATOR.Parameters.Entrance_Configurations";
        public const string ELEVATORPARAMETERDOOR = "ELEVATOR.Parameters.Doors.";

        public const string HALLLANTERNUNITHALLFIXTURES = "ELEVATOR.Parameters.Hall_Lantern";
        public const string GENERALINFORMATIONMESSAGE = "The value should be between {0} ft {1} in and {2} ft {3} in";
        public const string TWINELEVATORMESSAGE = "The value should be between ";
        public const string AND = " and ";
        public const string MINVALUE = "minValue";
        public const string MAXVALUE = "maxValue";
        public const string RANGEVALIDATION = "rangeValidationMessage";
        public const string ENTRANCECONFIGURATION = "entranceConfiguration";
        public static readonly string SINGLETONVALUE = "SingletonValue";
        public static readonly string INTERVALVALUE = "IntervalValue";
        public static readonly string MINVALUESMALLCASE = "minvalue";
        public static readonly string MAXVALUESMALLCASE = "maxvalue";
        public const string SECTIONSWITHPRICEVALUE = "sectionswithpriceValue";
        public const string BUILDINGVARIABLESLIST = "buildingVariablesList";
        public const string GROUPVARIABLESLIST = "groupVariablesList";
        public const string UNITVARIABLESLIST = "unitVariablesList";
        public const string AVAILABLE = "Available";
        public const string UNAVAILABLE = "Unavailable";
        public const string SELECTED = "Selected";

        public static readonly string PARAMETERS = "PARAMETERS";
        public static readonly string GROUPVALIDATION = "Group_Validation";
        public static readonly string PARAMETERSVALUES = "Parameters";
        public static readonly string UNITDESIGNATION = "unitDesignation";
        public static readonly string PARAMETERS_SP = "Parameters_SP";
        public static readonly string PARAMETERSSPCONTROLROOM = "Parameters_SP.control";
        public const string ELEVATOR = "ELEVATOR";
        public static readonly string ELEVATOR001 = "ELEVATOR001.";
        public static readonly string MANUFACTURINGCOMMENTS = "ManufacturingComments";
        public static readonly string CORPORATEASSISTANCE = "CorporateAssistance";
        public static readonly string STRATEGICDISCOUNT = "StrategicDiscount";

        public static readonly string ELEVATOR_DOT = "ELEVATOR.";


        public static readonly string ELEVATOR1 = "ELEVATOR001";
        public static readonly string ELEVATOR2 = "ELEVATOR002";
        public static readonly string ELEVATOR3 = "ELEVATOR003";
        public static readonly string ELEVATOR4 = "ELEVATOR004";
        public static readonly string ELEVATOR5 = "ELEVATOR005";
        public static readonly string ELEVATOR6 = "ELEVATOR006";
        public static readonly string ELEVATOR7 = "ELEVATOR007";
        public static readonly string ELEVATOR8 = "ELEVATOR008";

        public static readonly string SECTIONTAB = "SECTIONTAB";
        public static readonly string SECTIONTABS = "sectionTab";
        public static readonly string MINIMUM = "Minimum";
        public static readonly string MAXIMUM = "Maximum";
        public static readonly string CUSTOM = "Custom";
        public static readonly string HOISTWAYDIMENSIONS = "hoistwayDimensionSelection";
        public static readonly string NOHOISTWAYDIMENSIONS = "NoHoistwayDimensions";
        public static readonly string HOISTWAYDIMENSIONVALUE = "HOISTWAYDIMENSIONS";
        public static readonly string RANGEVALUES = "RangeValues";
        public const string MINIMUMVALUE = "minimumValue";
        public const string MAXIMUMVALUE = "maximumValue";
        public const string ISRANGEINPUTTYPE = "isRangeInputType";
        public const string EVO_200 = "EVO_200";
        public const string EVO_100 = "EVO_100";


        public const string CEGEARLESS = "CE_Gearless";
        public const string CEGEARED = "CE_Geared";
        public const string CEHYDRAULIC = "CE_Hydraulic";
        public const string SYNERGY = "Synergy";


        public static readonly string EVOLUTION_200 = "EVOLUTION 200";

        public static readonly string PARAMETERSSP = "PARAMETERS_SP";
        public static readonly string CONTROLFLOOR = "CONTROLFLOOR";

        public static readonly string PARAMETERSSPCONTROLFLOOR = "PARAMETERSSPCONTROLFLOOR";
        public static readonly string CONTROLLOCATIONID = "controlLocation";

        public static readonly string CONTROLLERLOCATION_SP = "CONTROLLERLOCATION_SP";
        public static readonly string CONTROLLOCATIONREMOTE = "Remote";
        public static readonly string CONTROLLOCATIONOVERHEAD = "Overhead";
        public static readonly string CONTROLLOCATIONJAMBMOUNTED = "Jamb-Mounted";
        public static readonly string PARMETERSXDIMENSIONVALUES = "PARMETERSXDIMENSIONVALUES";
        public static readonly string PARAMETERSYDIMENSIONVALUES = "PARMETERSYDIMENSIONVALUES";
        public static readonly string PARAMETERSF2F2TOPFLOOR = "PARAMETERSF2F2TOPFLOOR";
        public static readonly string PARAMETERSF2FBASEMENT = "PARAMETERSF2FBASEMENT";
        public static readonly string PARAMETERSNXVMDISTANCEFLOOR = "PARAMETERSNXVMDISTANCEFLOOR";
        public static readonly string CONTROLLOCATIONADJACENT = "Adjacent";
        public static readonly string CONTROLROOMQUADSPVALUES = "CONTROLROOMQUADSPVALUES";
        public static readonly string JAMB="Jamb";
        public static readonly string PARAMETERSBASICINFO = "Parameters.Basic_Info";
        public static readonly string PARAMETERSCARPOS = "CARPOS";
        public static readonly string REMOTE = "Remote";

        public static readonly string PARAMETERSREAROPEN = "REAROPEN";
        public static readonly string PARAMETERSREARDOOR = ".Parameters_SP.rearDoorTypeAndHand_SP";

        public static readonly string FLOORPLANDISTANCEPARAMETERS = "floorPlanDistanceParameters";
        public static readonly string PARAMETERSHALLRISER = ".Layout.HS2QUAD";
        public static readonly string PARAMETERS_LAYOUT_BANKTYPE = "Parameters.B[1-9]P[1-9]";
        public static readonly string UNITTABLEVALUES = "UNITTABLEVALUES";
        public static readonly string POSITIONVALUES = "positionvalues";
        public static readonly string TRUEVALUES = "TRUE";
        public static readonly string True = "True";
        public static readonly string False = "False";
        public static readonly string FALSEVALUES = "FALSE";
        public static readonly string PARAMETERS_LAYOUT_B = "Parameters.B";
        public static readonly string ELEVATORSVALUE = "ELEVATOR00";
        public static readonly string BLANDINGS = "BLANDINGS";
        public static readonly string NUMSUSP = "NUMSUSP";
        public static readonly string TOTALBUILDINGFLOORTOFLOORHEIGHT = "TOTALBUILDINGFLOORTOFLOORHEIGHT";
        public static readonly string TOTALBUILDINGFLOORTOFLOORHEIGHTSP = "totalBuildingFloorToFloorHeight_SP";
        public static readonly string FLOORTOFLOORHEIGHT = "Floor to Floor Height";
        public static readonly string ELEVATION_1 = "Elevation - 1";
        public static readonly string BUILDINGRISE = "buildingRise";
        public static readonly string PARAMETERSBASICINFOGRPDESG = "GROUPDESIGNATION";
        public static readonly string BYUSER_CAMELCASE = "byUser";
        public static readonly string BYRULE_CAMELCASE = "byRule";
        public static readonly string BYDEFAULT_CAMELCASE = "byDefault";
        public static readonly string SUBSECTION = "SUBSECTION";
        public static readonly string LOBBYSUBSECTION = "LOBBYSUBSECTION";
        public static readonly string AGILEBUILDINGFEATURES = "Building_Configuration.Parameters.AGILE_Building_Features";
        public static readonly string AVGHEIGHT = "AVGHEIGHT";

        public static readonly string ELEVATOR_PARAMETERS_HALL_PI = "ELEVATOR.Parameters.Hall_PI";
        public static readonly string ELEVATOR_Parameters_Combo_Hall_Lantern_PI = "ELEVATOR.Parameters.Combo_Hall_Lantern/PI";
        public static readonly string ELEVATOR_Parameters_Hall_Lantern = "ELEVATOR.Parameters.Hall_Lantern";
        public static readonly string ELEVATOR_Parameters_Hall_Elevator_Designation_Plate = "ELEVATOR.Parameters.Hall_Elevator_Designation_Plate";
        public static readonly string ELEVATOR_Parameters_Hall_Target_Indicator = "ELEVATOR.Parameters.Hall_Target_Indicator";
        public static readonly string ELEVATOR_Braille = "ELEVATOR.Parameters.Braille";
        public static readonly string ELEVATOR_Elevator_and_Floor_Designation_Braille = "ELEVATOR.Parameters.Elevator_&_Floor_Designation_Braille";
        public static readonly string HALL_PI = "HALL_PI";
        public static readonly string COMBO_HALL_LANTERN_PI = "COMBO_HALL_LANTERN/PI";

        public static readonly string USERINTERFACEPICOLOR = "ELEVATOR.Sales_UI_Screens.Unit_HallFixtures_SP.Parameters.User_Interface_Devices.Landing_Indicator_Panel_LIP.PICOLOR";
        public static readonly string USERINTERFACEHALLFIN = "ELEVATOR.Sales_UI_Screens.Unit_HallFixtures_SP.Parameters_SP.HALLFINMAT_SP";
        public static readonly string BRAILLEPARAMETER = "ELEVATOR.Sales_UI_Screens.Unit_HallFixtures_SP.Parameters_SP.dDBraille_SP";
        public static readonly string PRICEKEYPARAMETER = "ELEVATOR.PriceKey";
        public static readonly string PRICEKEY = "PriceKey";

        public static readonly string SELECTED_CAMELCASE = "Selected";
        public static readonly string DOOR = "Door";
        public static readonly string QUAD = "QUAD";
        public static readonly string CIB = "CIB";

        public static readonly string CURRENTGROUPCONFIGURATION = "CURRENTGROUPCONFIGURATION";
        public static readonly string BASEGROUPCONFIGURATION = "BASEGROUPCONFIGURATION";
        public static readonly string PREVIOUSGROUPCONFIGURATION = "PREVIOUSGROUPCONFIGURATION";

        public static readonly string CURRENTBUILDINGCONFIGURATION = "CURRENTBUILDINGCONFIGURATION";
        public static readonly string BASEBUILDINGCONFIGURATION = "BASEBUILDINGCONFIGURATION";
        public static readonly string PREVIOUSBUILDINGCONFIGURATION = "PREVIOUSBUILDINGCONFIGURATION";
        public static readonly string EDITCONFLITFLOWCACHEKEY = "EDITCONFLITFLOWCACHEKEY";
        public static readonly string PREVIOUSCONFLICTSVALUES = "PERVIOUSCONFLICTSVALUES";
        public static readonly string PREVIOUSGROUPCONFLICTSVALUES = "PERVIOUSGROUPCONFLICTSVALUES";
        public static readonly string PREVIOUSUNITCONFLICTSVALUES = "PERVIOUSUNITCONFLICTSVALUES";

        public static readonly string ENTRANCE_CONFIGURATION = "Entrance Configuration";

        public static readonly string TRAVELVARIABLEIDVALUE = "TRAVELVARIABLEIDVALUE";

        public static readonly string GETENRICHMENTVALUESDATA = "GETENRICHMENTVALUESDATA";
        public static readonly string NEWPROJECTIDFLAG = "0"; 

        public static readonly string VARIABLEDETAILS = "variableDetails";
        public static readonly string SECTIONSVALUES = "sections";
        public static readonly string PROJECTDISPLAYDETAILS = "projectDisplayDetails";

        public static readonly string ISEDITFLOWFLAGCHECK = "ISEDITFLOWFLAGCHECK";
        public static readonly string SYSTEMVALIDATIONPARAMETERS = @"Templates\Unit\{0}\SystemValidation\Parameters.json";

        public static readonly string PREVIOUSUNITCONFLICTSVALUESFORVALIDATION = "PREVIOUSUNITCONFLICTSVALUESFORVALIDATION";

        public static readonly string SYSTEMVALIDATIONCABREQUESTBODYVARIABLES = @"Templates\Unit\{0}\SystemValidation\CabCallRequiredVariables.json";
        public static readonly string SYSTEMVALIDATIONEMPTYREQUESTBODYVARIABLES = @"Templates\Unit\{0}\SystemValidation\EmptyCallRequiredVariables.json";
        public static readonly string SYSTEMVALIDATIONDUTYREQUESTBODYVARIABLES = @"Templates\Unit\{0}\SystemValidation\DutyCallRequiredVariables.json";
        public static readonly string SYSTEMVALIDATIONSLINGREQUESTBODYVARIABLES = @"Templates\Unit\Evolution100\SystemValidation\SlingCallRequiredVariables.json";
        public static readonly string SYSTEMVALIDATIONSLINGREQUESTBODYVARIABLE = @"Templates\Unit\{0}\SystemValidation\SlingCallRequiredVariables.json";
        public static readonly string SYSTEMVALIDATIONJACKDUTYREQUESTBODYVARIABLES = @"Templates\Unit\{0}\SystemValidation\JackDutyCallRequiredVariables.json";

        public static readonly string SYSVALVARIABLEASSIGNMENTS = "variableAssignments";

        public static readonly string ACROSSTHEHALLDISTANCEPARAMETER = "ACROSSTHEHALLDISTANCEPARAMETER";
        public static readonly string BANKOFFSETPARAMETER = "BANKOFFSETPARAMETER";
        public static readonly string SYSVALUNIT_VAL = "UNIT_VAL";
        public static readonly string SYSVALUNIT_INV = "UNIT_INV";
        public static readonly string SYSVALDESCRIPTION = "Description";
        public static readonly string EMPTYCALLMATERIALNAME = "R100174108";
        public static readonly string DUTYCALLMATERIALNAME = "R100205358";
        public static readonly string PERMISSIONTYPEBUILDING = "Building";
        public static readonly string PERMISSIONTYPEGROUP = "Group";
        public static readonly string PERMISSIONTYPEUNIT = "Unit";
        public static readonly string JAMB_MOUNTEDVALUE = "Jamb-Mounted";
        public static readonly string PARAMETERS_SPCONTROLLERLOCATION_SP = "ELEVATOR.Parameters_SP.controllerLocation_SP";
        public static readonly string HALLFINPARAM = "ELEVATOR.HALLFINMAT_SP";
        public static readonly string HALLFINVARIABLEID ="ELEVATOR.Parameters_SP.HALLFINMAT_SP";
        public static readonly string GROUPLAYOUTSIZE = "GROUPLAYOUTSIZE";
        public static readonly string NOOFFRONTHALLSTATIONS = "NOOFFRONTHALLSTATIONS";
        public static readonly string GROUPVARIABLESFORFLAGS = "GroupVariablesForFlags";

        public static readonly string BUILDINGCODE = "BUILDINGCODE";
        public static readonly string BUILDINGVARIABLESFORBUILDINGCODED = "BuildingVariablesForBuildingCode";
        public static readonly string STARTBUILDINGCONFIGURATIONSTUBPATH = @"Templates\Building\UIResponse.json";
        public static readonly string BUILDINGMAPPERVARIABLESMAPPERPATH = @"Templates\Building\ConstantMapper.json";
        public static readonly string GROUPMAPPERVARIABLES = @"Templates\Group\Elevator\ConstantMapper.json";
        public static readonly string UNITSVARIABLESMAPPERPATH = @"Templates\Unit\ConstantMapper.json";
        public static readonly string PRODUCTSELECTIONCONSTANTTEMPLATEPATH = @"Templates\ProductSelection\ConstantMapper.json";
        public static readonly string BUILDINGEQUIPMENTMAPPERVARIABLESMAPPERPATH = @"Templates\Building\BuildingEquipment\BuildingEquipmentConstantMapper.json";
        public static readonly string FDAMAPPERVARIABLESMAPPERPATH = @"Templates\FieldDrawingAutomation\ConstantMapper.json";
        public static readonly string UNITHALLFIXTURECONSTANTMAPPER = "UnitHallFixtureConstantMapper";
        public static readonly string BUILDINGMAPPERVARIABLES = @"Templates\Building\BuildingConstantMapper.json";
        public static readonly string SUMMARYLANDINGINCLUDESECTIONVALUES = "Tp2SummaryLandingIncludeSections";
        public static readonly string DEFAULTVARIABLEVALUE = "DefaultVariableValue";
        public static readonly string CUSTOMENGINEEREDCONSTANTMAPPERPATH = @"Templates\Unit\CustomEngineered\ConstantMapper.json";



        public static readonly string BUILDINGCODEIBCCBCVALUES1 = @"SampleStubData\StartCnfgn\BuildingCodeIbcCbc.json";
        public static readonly string BUILDINGCODENBCCVALUES1 = @"SampleStubData\StartCnfgn\BuildingCodeNbcc.json";
        public static readonly string SYSTEMVALIDATIONCONSTANTMAPPER = "SystemValidationConstantMapper";
        public static readonly string BUILDINGBLANDINGS = "BLANDINGS";
        public static readonly string UNITMAINRESPONSE = @"Templates\Unit\UnitMainResponse.json";
        
        public static readonly string ADDFIXTURES = "addFixtures";
        public static readonly string CONSOLES = "consoles";
        public static readonly string CONSOLE = "console";
        public static readonly string ETACONSOLES = "ETAConsoles";
        public static readonly string ETDCONSOLES = "ETDConsoles";
        public static readonly string ETDETDCONSOLES = "ETAPLUSETDConsoles";
        public static readonly string ETDORETAETDCONSOLES = "ETDOrETA/ETDConsoles";
        public static readonly string CONSOLEWITHSWITCHVARIABLES = "ConsolesWithSwitchVariables";
        public static readonly string SWITCHVARIABLESTOBEREMOVED = "SwitchVariablesToBeRemoved";
        public static readonly string UNITHALLFIXTUREMOCKRESPONSEFORTESTING = @"Templates\Unit\Evolution200\UnitHallFixtures\UnitHallFixtureMockResponseForTesting.json";
        public static readonly string HALLLANTERNCONSOLE = "Hall_Lantern";
        public static readonly string BRAILLECONSOLE = "Braille";
        public static readonly string ELEVATORFLOORBRAILLECONSOLE = "Elevator_and_Floor_Designation_Braille";
        public static readonly string HALLLANTERNVARIABLE = "HALLLANTERNVARIABLE";
        public static readonly string UNITHALLFIXTUREVALIDATIONERRORMESSAGE = "Please select all openings for Braille and Hall Lantern Consoles";
        public static readonly string UNITCONFIGURATIONSAVEMESSAGE = "Unit Configuration Saved Successfully";
        public static readonly string RANGEMESSAGE = "Value should be between 0 and ";
        public static readonly string LOBBYRECALLSWITCHFLAG = "LOBBYRECALLSWITCHFLAG";
        public static readonly string CARRIDINGLANTERNQUANT = "CARRIDINGLANTERNQUANT";
        public static readonly string INTERNALPREVIOUSGROUPCONFLICTSVALUES = "INTERNALPREVIOUSGROUPCONFLICTSVALUES";
        public static readonly string INTERNALPREVIOUSUNITCONFLICTSVALUES = "INTERNALPERVIOUSUNITCONFLICTSVALUES";
        public static readonly string INTERNALPREVIOUSCONFLICTSVALUES = "INTERNALPERVIOUSCONFLICTSVALUES";


        #endregion

        #region Log related constants
        public static readonly string LOGPATH = "LogErrorPath";

        public static readonly string GETGROUPCONFIGURATIONDETAILSINITIATE = " GetGroupConfigurationDetailsByGroupId BL call Initiated";
        public static readonly string STARTBUILDINGCONFIGVTPACKAGERESPONSE = "Start Building vt package response";
        public static readonly string GETGROUPCONFIGURATIONDETAILSCOMPLETE = " GetGroupConfigurationDetailsByGroupId BL call Completed";
        public static readonly string SAVEGROUPCONFIGURATIONDETAILSINITIATE = " SaveGroupConfigurationDetails BL call Initiated";
        public static readonly string SAVEGROUPCONFIGURATIONDETAILSCOMPLETE = " SaveGroupConfigurationDetails BL call Completed";
        public static readonly string UPDATEGROUPCONFIGURATIONDETAILSINITIATE = " UpdateGroupConfigurationDetails BL call Initiated";
        public static readonly string UPDATEGROUPCONFIGURATIONDETAILSCOMPLETE = " UpdateGroupConfigurationDetails BL call Completed";
        public static readonly string STARTGROUPCONFIGUREINITIATE = " StartGroupConfigure BL call Initiated";
        public static readonly string STARTGROUPCONFIGURECOMPLETE = " StartGroupConfigure BL call Completed";
        public static readonly string CHANGEGROUPCONFIGUREINITIATE = " ChangeGroupConfigure BL call Initiated";
        public static readonly string CHANGEGROUPCONFIGURECOMPLETE = " ChangeGroupConfigure BL call Completed";
        public static readonly string DELETEGROUPCONFIGURATIONINITIATE = " DeleteGroupConfiguration BL call Initiated";
        public static readonly string DELETEGROUPCONFIGURATIONCOMPLETE = " DeleteGroupConfiguration BL call Completed";
        public static readonly string SAVEGROUPLAYOUTDETAILSINITIATE = " SaveGroupLayoutDetails BL call Initiated";
        public static readonly string SAVEGROUPLAYOUTDETAILSCOMPLETE = " SaveGroupLayoutDetails BL call Completed";
        public static readonly string UPDATEGROUPLAYOUTDETAILSINITIATE = " UpdateGroupLayoutDetails BL call Initiated";
        public static readonly string UPDATEGROUPLAYOUTDETAILSCOMPLETE = " UpdateGroupLayoutDetails BL call Completed";
        public static readonly string UPDATEOPENINGLOCATIONINITIATE = " UpdateOpeningLocation BL call Initiated";
        public static readonly string UPDATEOPENINGLOCATIONCOMPLETE = " UpdateOpeningLocation BL call Completed";
        public static readonly string GETOPENINGLOCATIONBYGOUPIDINITIATE = " GetOpeningLocationByGoupId BL call Initiated";
        public static readonly string GETOPENINGLOCATIONBYGOUPIDCOMPLETE = " GetOpeningLocationByGoupId BL call Completed";
        public static readonly string GETLISTOFPRODUCTLINEINITIATE = " GetListOfProductLine BL call Initiated";
        public static readonly string GETLISTOFPRODUCTLINECOMPLETE = " GetListOfProductLine BL call Completed";
        public static readonly string PRODUCTSELECTIONINITIATE = " ProductSelection BL call Initiated";
        public static readonly string PRODUCTSELECTIONCOMPLETE = " ProductSelection BL call Completed";
        public static readonly string SAVEPRODUCTSELECTIONINITIATE = " SaveProductSelection BL call Initiated";
        public static readonly string SAVEPRODUCTSELECTIONCOMPLETE = " SaveProductSelection BL call Completed";
        public static readonly string GETUNITDETAILSINITIATE = " GetUnitDetails BL call Initiated";
        public static readonly string GETUNITDETAILSCOMPLETE = " GetUnitDetails BL call Completed";
        public static readonly string STARTUNITCONFIGUREINITIATE = " StartUnitConfigure BL call Initiated";
        public static readonly string STARTUNITCONFIGURECOMPLETE = " StartUnitConfigure BL call Completed";
        public static readonly string STARTENTRANCECONFIGUREINITIATE = " StartEntranceConfigure BL call Initiated";
        public static readonly string STARTENTRANCECONFIGURECOMPLETE = " StartEntranceConfigure BL call Completed";
        public static readonly string CHANGEUNITCONFIGUREINITIATE = " ChangeUnitConfigure BL call Initiated";
        public static readonly string CHANGEUNITCONFIGURECOMPLETE = " ChangeUnitConfigure BL call Completed";
        public static readonly string SAVEUNITCONFIGURATIONINITIATE = " SaveUnitConfiguration BL call Initiated";
        public static readonly string SAVEUNITCONFIGURATIONCOMPLETE = " SaveUnitConfiguration BL call Completed";
        public static readonly string UPDATEUNITCONFIGURATIONINITIATE = " UpdateUnitConfiguration BL call Initiated";
        public static readonly string UPDATEUNITCONFIGURATIONCOMPLETE = " UpdateUnitConfiguration BL call Completed";
        public static readonly string SAVECABINTERIORDETAILSINITIATE = " SaveCabInteriorDetails BL call Initiated";
        public static readonly string SAVECABINTERIORDETAILSCOMPLETE = " SaveCabInteriorDetails BL call Completed";
        public static readonly string UPDATECABINTERIORDETAILSINITIATE = " UpdateCabInteriorDetails BL call Initiated";
        public static readonly string UPDATECABINTERIORDETAILSCOMPLETE = " UpdateCabInteriorDetails BL call Completed";
        public static readonly string SAVEHOISTWAYTRACTIONEQUIPMENTINITIATE = " SaveHoistwayTractionEquipment BL call Initiated";
        public static readonly string SAVEHOISTWAYTRACTIONEQUIPMENTCOMPLETE = " SaveHoistwayTractionEquipment BL call Completed";
        public static readonly string UPDATEHOISTWAYTRACTIONEQUIPMENTINITIATE = " UpdateHoistwayTractionEquipment BL call Initiated";
        public static readonly string UPDATEHOISTWAYTRACTIONEQUIPMENTCOMPLETE = " UpdateHoistwayTractionEquipment BL call Completed";
        public static readonly string SAVEGENERALINFORMATIONINITIATE = " SaveGeneralInformation BL call Initiated";
        public static readonly string SAVEGENERALINFORMATIONCOMPLETE = " SaveGeneralInformation BL call Completed";
        public static readonly string UPDATEGENERALINFORMATIONINITIATE = " UpdateGeneralInformation BL call Initiated";
        public static readonly string UPDATEGENERALINFORMATIONCOMPLETE = " UpdateGeneralInformation BL call Completed";
        public static readonly string SAVEENTRANCESINITIATE = " SaveEntrances BL call Initiated";
        public static readonly string SAVEENTRANCESCOMPLETE = " SaveEntrances BL call Completed";
        public static readonly string UPDATEENTRANCESINITIATE = " UpdateEntrances BL call Initiated";
        public static readonly string UPDATEENTRANCESCOMPLETE = " UpdateEntrances BL call Completed";
        public static readonly string GETLISTOFCONFIGURATIONFORPROJECTINITIATEDL = " GetListOfConfigurationForProject DL call Initiated";
        public static readonly string GETBUILDINGCONFIGURATIONBYIDDL = " GetBuildingConfigurationById DL call Initiated";
        public static readonly string GETBUILDINGCONFIGDLINITIATE = " ChangeConfigureBl BL call Initiated";
        public static readonly string GETBUILDINGCONFIGDLCOMPLETE = " ChangeConfigureBl BL call Completed";
        public static readonly string SAVEBUILDINGCONFIGURATIONFORPROJECTDL = " SaveBuildingConfigurationForProject DL call Initiated";
        public static readonly string SAVEBUILDINGELEVATIONDL = " SaveBuildingElevation DL call Initiated";
        public static readonly string UPDATEBUILDINGELEVATIONDL = " UpdateBuildingElevation DL call Initiated";
        public static readonly string AUTOSAVEBUILDINGELEVATIONDL = " AutoSaveBuildingElevation DL call Initiated";
        public static readonly string GETBUILDINGELEVATIONBYIDDL = " GetBuildingElevationById DL call Initiated";
        public static readonly string DELETEBUILDINGCONFIGURATIONBYIDDL = " DeleteBuildingConfigurationById DL call Initiated";
        public static readonly string DELETEBUILDINGELEVATIONBYIDDL = " DeleteBuildingElevationById DL call Completed";
        public static readonly string STARTBUILDINGCONFIGDLINITIATE = " StartBuildingConfigure BL call Initiated";
        public static readonly string STARTBUILDINGFILTEREDCONFIGRESPONSE = " StartBuildingConfigure Filtered response";
        public static readonly string STARTBUILDINGCONFIGDLCOMPLETE = " StartBuildingConfigure BL call Completed";
        public static readonly string STARTBUILDINGCONFIGUREDL = " GetBuildingConfigurationById DL call Initiated";
        public static readonly string STARTBUILDINGCONFIGUREDLINITIATE = " ChangeConfigureBl BL call Initiated";
        public static readonly string STARTBUILDINGCONFIGUREDLCOMPLETE = " ChangeConfigureBl BL call Completed";
        public static readonly string UPDATEOPENINGLOCATIONINITIATEDL = " UpdateOpeningLocation DL call Initiated";
        public static readonly string GETOPENINGLOCATIONBYGROUPIDINITIATEDL = " GetOpeningLocationByGroupId DL call Initiated";
        public static readonly string SAVEPRODUCTSELECTIONINITIATEDL = " SaveProductSelection DL call Initiated";
        public static readonly string GETUNITDETAILSFORPRODUCTSELECTIONINITIATEDL = " GetUnitDetailsForProductSelection DL call Initiated";
        public static readonly string GETUNITVARIABLEASSIGNMENTSINITIATEDL = " GetUnitVariableAssignments DL call Initiated";
        public static readonly string GETLISTOFPROJECTSFORUSERINITIATEDL = " GetListOfProjectsForUser DL call Initiated";
        public static readonly string SEARCHUSERINITIATEDL = " SearchUser DL call Initiated";
        public static readonly string GETUNITCONFIGURATIONBYGROUPIDINITIATEDL = " GetUnitConfigurationByGroupId DL call Initiated";
        public static readonly string NUMBEROFFLOORSUNITPACKAGEVARIABLE = "ELEVATOR.Parameters.Basic_Info.BLANDINGS";
        public static readonly string TOTALNUMBEROFFLOORSUNITPACKAGEVARIABLE = "TOTALNUMBEROFFLOORSUNITPACKAGEVARIABLE";
        public static readonly string DUPLICATEUNITCONFIGURATIONBYIDDL = " DuplicateUnitConfigurationById DL call Initiated";
        public static readonly string DELETEPROJECTSBYIDCOMPLETEDBL = " DELETEPROJECTSBYIDCOMPLETE BL call Completed";
        public static readonly string DELETEPROJECTSBYIDSTARTBL = " DELETEPROJECTSBYIDSTART BL call Completed";

        public static readonly string SAVEUPDATRELEASEINFODETAILSINITIATE = " SaveUpdatReleaseInfoDetails BL call Initiated";
        public static readonly string SAVEUPDATRELEASEINFODETAILSCOMPLETE = " SaveUpdatReleaseInfoDetails BL call complete";
        public static readonly string GETRELEASEINFOBYPROJECTIDINITIATED = " GetReleaseInfoByProject BL call initiated";
        public static readonly string GETRELEASEINFOBYPROJECTIDCOMPLETE = " GetReleaseInfoByProject BL call complete";
        public static readonly string GETRELEASETOMANUFACTUREINITIATED = " GetReleaseToManufacture BL call initiated";
        public static readonly string GETRELEASETOMANUFACTURECOMPLETE = " GetReleaseToManufacture BL call complete";

        public static readonly string GETENTRANCECONFIGURATIONBYGROUPIDINITIATEDL = " GetEntranceConfigurationBySetId DL call Initiated";
        public static readonly string GETUNITCONFIGDLINITIATE = " StartConfigureBl BL call Initiated";


        public static readonly string GETUNITCONFIGDLCOMPLETE = " StartConfigureBl BL call Completed";
        public static readonly string SAVEUNITCONFIGURATIONINITIATEDL = " SaveUnitConfiguration DL call Initiated";
        public static readonly string UPDATEUNITCONFIGURATIONINITIATEDL = " UpdateUnitConfiguration DL call Initiated";
        public static readonly string SAVECABINTERIORDETAILSINITIATEDL = " SaveCabInteriorDetails DL call Initiated";
        public static readonly string UPDATECABINTERIORDETAILSINITIATEDL = " UpdateCabInteriorDetails DL call Initiated";
        public static readonly string SAVEHOISTWAYTRACTIONEQUIPMENTINITIATEDL = " SaveHoistWayTractioEquipment DL call Initiated";
        public static readonly string UPDATEHOISTWAYTRACTIONEQUIPMENTINITIATEDL = " UpdateHoistWayTractioEquipment DL call Initiated";
        public static readonly string SAVEGENERALINFORMATIOINITIATEDL = " SaveGeneralInformation DL call Initiated";
        public static readonly string UPDATEGENERALINFORMATIOINITIATEDL = " UpdateGeneralInformation DL call Initiated";
        public static readonly string SAVEENTRANCESINITIATEDL = " SaveEntrances DL call Initiated";
        public static readonly string UPDATEENTRANCESINITIATEDL = " UpdateEntrances DL call Initiated";
        public static readonly string GETPROJECTBASEDUNITS1INITIATEDL = " GetProjectBasedUnits1 DL call Initiated";
        public static readonly string AUTOSAVECONFIGURATIONDL = " AutoSaveConfiguration DL call Initiated";
        public static readonly string DELETEAUTOSAVECONFIGURATIONBYUSERDL = " DeleteAutoSaveConfigurationByUser DL call Initiated";
        public static readonly string GETAUTOSAVECONFIGURATIONBYUSERDL = " GetAutoSaveConfigurationByUser DL call Initiated";
        public static readonly string GETGROUPCONFIGURATIONDETAILSBYGROUPIDDL = " GetGroupConfigurationDetailsByGroupId DL call Completed";
        public static readonly string SAVEGROUPCONFIGURATIONDL = " SaveGroupConfiguration DL call Initiated";
        public static readonly string UPDATEGROUPCONFIGURATIONDL = " UpdateGroupConfiguration DL call Initiated";
        public static readonly string DELETEGROUPCONFIGURATIONDL = " DeleteGroupConfiguration DL call Initiated";
        public static readonly string CHANGEGROUPCONFIGUREBLINITIATE = " ChangeGroupConfigureBl BL call Initiated";
        public static readonly string CHANGEGROUPCONFIGUREBLCOMPLETE = " ChangeGroupConfigureBl BL call Completed";
        public static readonly string SAVEGROUPLAYOUTDL = " SaveGroupLayout DL call Initiated";
        public static readonly string UPDATEGROUPLAYOUTDL = " UpdateGroupLayout DL call Initiated";
        public static readonly string EDITUNITDESIGNATIONCATCHBLOCK = " Edit Unit Designation Catch Block";
        public static readonly string STARTUNITHALLFIXTUREINITIATE = "StartUnitHallFixture BL call Initiated";
        public static readonly string STARTUNITHALLFIXTURECOMPLETE = "StartUnitHallFixture BL call Completed";
        public static readonly string SAVEUNITHALLFIXTUREDLINITIATED = "SaveUnitHallFixture DL call Initiated";
        public static readonly string SAVEUNITHALLFIXTUREDLCOMPLETED = "SaveUnitHallFixture DL call Completed";
        public static readonly string SAVEGROUPHALLFIXTUREINITIATE = " SaveGroupHallFixture BL call Initiated";
        public static readonly string SAVEGROUPHALLFIXTURECOMPLETE = " SaveGroupHallFixture BL call Completed";
        public static readonly string GETSTARTGROUPHALLFIXTUREINITIATE = " GetStartGroupHallFixture BL call Initiated";
        public static readonly string GETSTARTGROUPHALLFIXTURECOMPLETE = " GetStartGroupHallFixture BL call Completed";
        public static readonly string DELETEUNITHALLFIXTURECONFIGURATIONBYIDDL = " DeleteUnitHallFixtureConfigurationById DL call Initiated";
        public static readonly string STARTGROUPHALLFIXTUREINITIATE = "StartUnitHallFixture BL call Initiated";
        public static readonly string STARTGROUPHALLFIXTURECOMPLETE = "StartUnitHallFixture BL call Completed";
        public static readonly string GETFIELDDRAWINGAUTOMATIONBYGROUPIDINITIATEDDL = "GetFieldAutomationByGroupId DL call Initiated";
        public static readonly string GETFIELDDRAWINGAUTOMATIONLAYOUTBYGROUPIDINITIATEDDL = "GetFieldAutomationLayoutByGroupId DL call Initiated";
        public static readonly string SAVEFIELDDRAWINGAUTOMATIONBYGROUPIDINITIATEDDL = "SaveFieldDrawingAutomationByGroupId DL call Initiated";
        public static readonly string GETBUILDINGGROUPVARIABLESBYGROUPIDINITIATEDDL = "GetBuildingGroupVariablesByGroupId DL call Initiated";
        public static readonly string GETUNITVARIABLESBYGROUPIDINITIATEDDL = "GetUnitVariablesByGroupId DL call Initiated";
        public static readonly string GETUNITVARIABLESWITHUNITBYGROUPIDINITIATEDDL = "GetUnitVariableWithUnitByGroupId DL call Initiated";
        public static readonly string REQUESTLAYOUTINITIATEDDL = "RequestLayout DL call Initiated";
        public static readonly string SAVEREFERENCEIDINITIATEDDL = "SaveReferenceId DL Call Initiated";
        public static readonly string LAYOUTSTATUSINITIATEDDL = "LayoutStatus DL Call Initiated";
        public static readonly string GENERATEWRAPPERTOKENINITIATEDDL = "GenerateWrapperToken DL Call Initiated";
        public static readonly string GENERATEWRAPPERTOKENCOMPLETEDL = "GenerateWrapperToken DL Call Completed";
        public static readonly string GETFIELDDRAWINGAUTOMATIONBYGROUPIDCOMPLETEDL = "GetFieldAutomationByGroupId DL call Completed";
        public static readonly string GETFIELDDRAWINGAUTOMATIONLAYOUTBYGROUPIDCOMPLETEDL = "GetFieldAutomationLayoutByGroupId DL call Completed";
        public static readonly string SAVEFIELDDRAWINGAUTOMATIONBYGROUPIDCOMPLETEDL = "SaveFieldDrawingAutomationByGroupId DL call Completed";
        public static readonly string GETBUILDINGGROUPVARIABLESBYGROUPIDCOMPLETEDL = "GetBuildingGroupVariablesByGroupId DL call Completed";
        public static readonly string GETUNITVARIABLESBYGROUPIDCOMPLETEDL = "GetUnitVariablesByGroupId DL Completed";
        public static readonly string GETUNITVARIABLESWITHUNITBYGROUPIDCOMPLETEDL = "GetUnitVariableWithUnitByGroupId DL Completed";
        public static readonly string REQUESTLAYOUTCOMPLETEDL = "RequestLayout DL call Completed";
        public static readonly string SAVEREFERENCEIDCOMPLETEDL = "SaveReferenceId DL Call Completed";
        public static readonly string LAYOUTSTATUSCOMPLETEDL = "LayoutStatus DL Call Completed";
        public static readonly string GETBUILDINGEQUIPMENTCONFIGURATIONBYBUILDINGIDINITIATEDL = "GetBuildingEquipmentConfigurationByBuildingId DL Call Initiated";
        public static readonly string GETBUILDINGEQUIPMENTCONFIGURATIONBYBUILDINGIDCOMPLETEDL = "GetBuildingEquipmentConfigurationByBuildingId DL Call Completed";
        public static readonly string GETBUILDINGEQUIPMENTCONSOLESINITIATEDL = "GetBuildingEquipmentConsole DL call Initiated";
        public static readonly string GETBUILDINGEQUIPMENTCONSOLESCOMPLETEDL = "GetBuildingEquipmentConsole DL call Completed";
        public static readonly string SETCACHEBUILDINGEQUIPMENTCOMPLETEBL = "SetCacheBuildingEquipment DL call Completed";
        public static readonly string SETCACHEFIXTURESTRATERGYCOMPLETEBL = "SetCacheFixtureStratergy DL call Completed";
        public static readonly string SETBUILDINGDINGEQUIPMENTCONFIGURECOMPLETEBL = "SetBuildingEquipmentConfigure DL call Completed";
        public static readonly string BUILDINGEQUIPMENTCONSOLECONFIGURECOMPLETEBL = "BuildingEquipmentConsoleConfigure DL call Completed";
        public static readonly string STARTFIELDDRAWINGCONFIGUREINITIATEDL = "StartFieldDrawingConfiguration DL call Initiated";
        public static readonly string GETFIELDDRAWINGBYPROJECTIDINITIATEDL = "GetFieldDrawingByProjectId DL call Initiated";
        public static readonly string GETREQUESTQUEUEBYGROUPIDINITIATEDL = "GetRequestQueueByGroupId DL call Initiated";
        public static readonly string SAVEFIELDDRAWINGCONFIGUREINITIATEDL = "SaveFieldDrawingConfigure DL call Initiated";
        public static readonly string DISPLAYVARIABLEVALUERESPONSEINITIATEDL = "DisplayVariableValueResponse DL call Initiated";
        public static readonly string GETLOCKEDGROUPSBYPROJECTIDINITIATEDL = "GetLockedGroupsByProjectId DL call Initiated";
        public static readonly string GETLOCKEDGROUPSBYPROJECTIDCOMPLETEBL = "GetLockedGroupsByProjectId BL call Completed";
        public static readonly string GETLOCKEDGROUPSBYPROJECTIDINITIATEBL = "GetLockedGroupsByProjectId BL call Initiated";
        public static readonly string GENERATEREQUESTBODYFORVIEWEXPORTSTARTED = "Generate Request body for view export BL call started";
        public static readonly string GENERATEREQUESTBODYFORVIEWEXPORTCOMPLETED = "Generate Request body for view export BL call completed";
        public static readonly string POSTCONFIGURATIONVIEWEXPORTSTARTED = "post configuration to view  BL call started";
        public static readonly string POSTCONFIGURATIONVIEWEXPORTCOMPLETED = "post configuration to view  BL call completed";
        public static readonly string GETGROUPHALLFIXTURETABSINITIATE = " GetGroupHallFixtureTabs BL call Initiated";
        public static readonly string GETGROUPHALLFIXTURETABSCOMPLETE = " GetGroupHallFixtureTabs BL call Completed";
        public static readonly string SAVEPRICEFORTP2SUMMARYINITIATED = " SavePriceValues DL call Initiated";
        public static readonly string SAVEPRICEFORTP2SUMMARYCOMPLETE = " SavePriceValues DL call Completed";
        public static readonly string GETRELEASEINFOBYPROJECTIDINITIATEDL = "GetReleaseInfoByProjectId DL call Initiated";
        public static readonly string GETRELEASEINFOBYGROUPIDINITIATEDL = "GetReleaseInfoByGroupId DL call Initiated";
        public static readonly string SAVEUPDATERELEASEINFOINITIATEDL = " SaveUpdateReleaseInfo DL call Initiated";

        public const string DATETIMESTRING = "yyyy/MM/dd HH:mm:ss.ff ";
        public const string VARIABLES = "variables";
        public const string OPTIONS = "options";
        public static readonly string SECTIONTYPE = "sectionType";
        public static readonly string STRING = "String";

        public static readonly string STARTFIELDDRAWINGAUTOMATIONINITIATE = " Start Field Automation BL call Initiated";
        public static readonly string STARTFIELDDRAWINGAUTOMATIONCOMPLETE = " Start Field Automation BL call Completed";


        public static readonly string CHANGEFIELDDRAWINGAUTOMATIONINITIATE = " Start Field Automation BL call Initiated";
        public static readonly string CHANGEFIELDDRAWINGAUTOMATIONCOMPLETE = " Change Field Automation BL call Completed";

        public static readonly string GENERATEOZTOKENINITIATE = " Generate OZ token BL call Initiated";
        public static readonly string GENERATEOZTOKENCOMPLETE = " Generate OZ token BL call Completed";
        public static readonly string OZBOOKINGREQUESTINITIATE = " OZ Booking Request BL call Initiated";
        public static readonly string OZBOOKINGREQUESTCOMPLETE = " OZ Booking Request BL call Completed";
        public static readonly string OZBOOKINGGENERATEREQUESTBODYINITIATE = " OZ Booking Request BL call Initiated";
        public static readonly string OZBOOKINGGENERATEREQUESTBODYCOMPLETE = " OZ Booking Request BL call Completed";
        public static readonly string SAVESENDTOCOORDINATIONINITIATE = "SaveSendToCoordination BL call Initiated";
        public static readonly string SAVESENDTOCOORDINATIONCOMPLETE = "SaveSendToCoordination BL call Completed";
        public static readonly string SAVESENDTOCOORDINATIONINITIATEDL = "SaveSendToCoordination DL call Initiated";
        public static readonly string SAVESENDTOCOORDINATIONCOMPLETEDL = "SaveSendToCoordination DL call Completed";



        public static readonly string GETSUMMARYFORPROJECTINITIATEBL = " GetSUmmaryConfiguration BL call Initiated";

        public static readonly string SAVEASSIGNGROUPSINITIATED = "SaveAssignGroups BL call Initiated";
        public static readonly string SAVEASSIGNGROUPSCOMPLETE = "SaveAssignGroups BL call Completed";
        public static readonly string SAVEASSIGNGROUPSINITIATEDL = "SaveAssignGroups DL call Initiated";
        public static readonly string SAVEASSIGNGROUPSCOMPLETEDL = "SaveAssignGroups DL call Completed";
        public static readonly string DUPLICATEBUILDINGEQUIPMENTCONSOLEINITIATEDL = "DuplicateBuildingEquipmentConsole DL call Initiated";
        public static readonly string DUPLICATEBUILDINGEQUIPMENTCONSOLECOMPLETEDDL = "DuplicateBuildingEquipmentConsole DL call Completed";
        public static readonly string DELETEBUILDINGEQUIPMENTCONSOLEINITIATEDL = "DeleteBuildingEquipmentConsole DL call Initiated";
        public static readonly string DELETEBUILDINGEQUIPMENTCONSOLECOMPLETEDDL = "DeleteBuildingEquipmentConsole DL call Completed";
        public static readonly string SAVEBUILDINGEQUIPMENTCONFIGURATIONINITIATE = "SaveBuildingEquipment BL call Initiated";
        public static readonly string SAVEBUILDINGEQUIPMENTCONFIGURATIONCOMPLETE = "SaveBuildingEquipment BL call Completed";
        public static readonly string SAVEBUILDINGEQUIPMENTINITIATEDL = "SaveBuildingEquipment DL call Initiated";
        public static readonly string SAVEBUILDINGEQUIPMENTCOMPLETEDL = "SaveBuildingEquipment DL call Completed";
        public static readonly string GETBUILDINGEQUIPMENTFORPROJECTINITIATEDL = " GetBuildingEquipmentTab DL call Initiated";
        public static readonly string SAVECARCUTOUTOPENININGSINITIATE = "SaveCarCallCutoutKeyswitchOpening BL call Initiated";
        public static readonly string SAVECARCUTOUTOPENININGSCOMPLETE = "SaveCarCallCutoutKeyswitchOpening BL call Completed";
        public static readonly string SAVECARCUTOUTOPENININGSINITIATEDL = "SaveCarCutoutOpenings DL call Initiated";
        public static readonly string STARTCARCALLCUTOUTASSIGNOPENINGS = "GetCarCallCutoutOpenings DL call Initiated";
        public static readonly string GETGROUPSECTIONINITIATEDL = " GetGroupSectionTab DL call Initiated";

        public static readonly string GETSENDTOCOORDINATIONBYPROJECTIDINITIATED = "GetSendToCoordinationByProjectId BL call Initiated";
        public static readonly string GETSENDTOCOORDINATIONBYPROJECTIDCOMPLETE = "GetSendToCoordinationByProjectId BL call Completed";
        public static readonly string GETSENDTOCOORDINATIONBYPROJECTIDINITIATEDDL = "GetSendToCoordinationByProjectId DL call Initiated";

        public static readonly string APPLICATIONBOOTING = "Application Booting Up";
        public static readonly string FATALERROR = "Fatal error occured";


        #endregion

        #region WrapperAPI
        public const string ACCESS_TOKEN = "access_token";
        public const string TOKEN_TYPE = "token_type";
        public const string EXPIRES_IN = "expires_in";
        public const string MESSAGE = "Message";
        public static readonly string APIUSERNAME = "UserName";
        public static readonly string APIPASSWORD = "Password";
        public const string WRAPPERAPISTATUSID = "StatusId";
        public const string INTEGRATEDSYTEMID = "IntegratedSystemId";
        public const string GRANTTYPE = "grant_type";
        public static readonly string GENERATEWRAPPERTOKEN = "Calling Generate token API : ";
        public static readonly string WRAPPERTOKENGENERATED = "Wrapper API token generated successfully :";
        #endregion

        #region OzAPI

        public static readonly string PROJECTINFORMATION = "ProjectInformation";
        public static readonly string EQUIPMENT = "Equipment";
        public static readonly string REQUESTEDDRAWING = "RequestedDrawing\r\n";
        public static readonly string ACCOUNTADDRESS = "AccountAddress";
        public static readonly string JOBNAME = "Id";
        public static readonly string JOBSITEADDRESSLINE1 = "AccountAddressStreetAddress";
        public static readonly string JOBSITEADDRESSLINE2 = "AccountAddressStreetAddress2";
        public static readonly string JOBSIECITY = "AccountAddressCity";
        public static readonly string JOBSITESTATEPROVINCE = "AccountAddressState";
        public static readonly string JOBSITEPOSTALCODE = "AccountAddressAddressZipCode";
        public static readonly string LINEOFBUSINESS = "LineOfBusiness";
        public static readonly string OZCONNECTIONSTRING = "OzConnectionString";
        public static readonly string OZUSERNAME = "user_name";
        public static readonly string OZPASSWORD = "password";
        public static readonly string OZGRANTYPE = "grant_type";
        public static readonly string OZUSER = "username";
        public static readonly string OZTEXT = "text/plain";
        public static readonly string ENCODING = "utf-8";
        public static readonly char EQUAL = '=';
        public static readonly char AMPERSAND = '&';
        public static readonly string TOKEN = "servicestest/token";
        public static readonly string OPPORTUNITYID = "OpportunityId";
        public static readonly string CSC = "CSC";
        public static readonly string URLPORTION = "servicestest/api/Booking";
        public static readonly string AUTHORIZATION = "Authorization";
        public static readonly string CONTENTTYPE = "Content-Type";
        public static readonly string APPLICATIONJSON = "application/json";
        public static readonly string BEARER1 = "Bearer ";
        public static readonly string OZTOKEN = "OzToken";
 


        #endregion

        #region VIEW Api related constants

        public static readonly string VIEWVARIABLES = "ViewVariables";
        public static readonly string CODE = "Code";
        public static readonly string VERSIONID = "VersionId";
        public static readonly string OPPORTNTYID = "OpportunityId";
        public static readonly string OPPURL = "OpportunityURL";
        public static readonly string BUSINESSLINE = "BusinessLine";
        public static readonly string JOBNAMEVIEW = "JobName";
        public static readonly string QUOTEBASEBID = "QuoteBaseBid";
        public static readonly string LATESTVIEWVERSION = "LatestViewVersion";
        public static readonly string BRANCH = "Branch";
        public static readonly string SLSMNACTVDRCTRYID = "SalesmanActiveDirectoryID";
        public static readonly string SALESMAN = "Salesman";
        public static readonly string CATGRY = "Category";
        public static readonly string AWRDCLSDDATE = "AwardClosedDate";
        public static readonly string DATEFORMAT = "DateFormat";
        public static readonly string DATEVALUE = "DateValue";
        public static readonly string ORACLEPSNMBR = "OraclePSNumber";
        public static readonly string SALESSTG = "SalesStage";
        public static readonly string SUPERINTENDENT = "SUPERINTENDENT";
        public static readonly string ACCOUNTNME = "AccountName";
        public static readonly string ADDRESSLINE1 = "AddressLine1";
        public static readonly string ADDRESSLINE2 = "AddressLine2";
        public static readonly string CITY = "City";
        public static readonly string COUNTRY = "Country";
        public static readonly string COUNTY = "County";
        public static readonly string STATE = "State";
        public static readonly string ZIPCODE = "ZipCode";
        public static readonly string CUSTOMERNMBR = "CustomerNumber";
        public static readonly string FIRSTNAME = "FirstName";
        public static readonly string LASTNAME = "LastName";
        public static readonly string EMAIL = "Email";
        public static readonly string MOBILEPHONE = "MobilePhone";
        public static readonly string UIVERSIONID = "UI_VersionID";
        public static readonly string QUOTEDESCRIPTION = "QuoteDescription";
        public static readonly string SALESEMAIL = "SalesEmail";
        public static readonly string GCVARIABLES = "GCVariables";
        public static readonly string OWNERVARIABLES = "OwnerVariables";
        public static readonly string BILLINGVRBLES = "BillingVariables";
        public static readonly string ARCHITECHVRBLES = "ArchitectVariables";
        public static readonly string BUILDINGINFOVRBLES = "BuildingInfoVariables";
        public static readonly string BUILDINGVRBLES = "BuildingVariables";
        public static readonly string CONTACTVRBLES = "ContactVariables";
        public static readonly string QUOTEVRBLES = "QuoteVariables";
        public static readonly string ZIPCODE_BLDG = "ZipCode_bldg";
        public static readonly string ADDRESSLINE1_BLDG = "AddressLine1_bldg";
        public static readonly string ADDRESSLINE2_BLDG = "AddressLine2_bldg";
        public static readonly string QUOTE = "Quote";
        public static readonly string VIEWUNITS = "data.Units"; 
        public static readonly string FACTORYJOBNUMBER = "Identification.FactoryJobNumber"; 
        public static readonly string VIEWUEID = "Identification.UEID";
        public static readonly string PROPOSEDDATEFORMAT = "ProposedDateFormat";
        public static readonly string PROPOSEDDATE = "ProposedDate";
        public static readonly string CONTRACTBOOKEDDATEFORMAT = "ContractBookedDateFormat";
        public static readonly string CONTRACTBOOKEDDATE = "ContractBookedDate";
        public static readonly string RETMSG = "retMsg";
        public static readonly string PRODUCTTYPE_SP = "ProductType_SP";
        public static readonly string COMMONNAME_SP = "CommonName_SP";
        public static readonly string SUBMODELTYPE = "SUBMODELTYPE";
        public const string ENDURA_100 = "ENDURA_100";
        public const string ENDURA_200 = "ENDURA_200";
        public const string EVOLUTION_100 = "EVOLUTION_100";
        public static readonly string EVOLUTION100 = "Evolution_100";
        public static readonly string ENDURA100 = "endura_100";
        public static readonly string ENDURA200 = "endura_200";
        public static readonly string NEWINSTALLATIONVALUES = "New Installation";
        public static readonly string SCUSER = "SC-";
        public static readonly string NOCONFIGURATIONDATAAVAILABLE = "No configuration data available";
        public static readonly string NOCONFIGURATIONDATAAVAILABLEDESCRIPTION = "please check configuration details are available before proceeding further";
        public static readonly string RETMSGVALUES = "retMsg";
        public static readonly string CODEVALUES = "code";
        public static readonly string COMMONNAME = "Product_Selection.Common_Name";
        public static readonly string PRODUCTTREEREAROPENVARIABLE = "Product_Selection.Parameters.Basic_Info.REAROPEN";
        public static readonly string PRODUCTTREECAPACITYVARIABLE="Product_Selection.Parameters.Basic_Info.CAPACITY";
        public static readonly string PRODUCTTREESPEEDVARIABLE = "Product_Selection.Parameters.Basic_Info.CARSPEED";
        public static readonly string PRODUCTTREETYPSVCVARIABLE = "Product_Selection.Parameters.Basic_Info.TYPSVC";

        public static readonly string PROJECTS_PROJECTID = "projects.ProjectId";
        public static readonly string PROJECTS_PROJECTNAME = "projects.ProjectName";
        public static readonly string PROJECTS_VERSIONID = "projects.VersionId";
        public static readonly string PROJECTS_BRANCH = "projects.Branch";
        public static readonly string PROJECTS_SALESSATGE = "projects.SalesStage";
        public static readonly string LAYOUT_LANGUAGE = "layoutDetails.Language";
        public static readonly string LAYOUT_MEASURINGUNIT = "layoutDetails.MeasuringUnit";
        public static readonly string ACCOUNTDETAIL_ACCOUNTNAME = "accountDetails.AccountName";
        public static readonly string ACCOUNTDETAIL_ADDRESS1 = "accountDetails.SiteAddress.AddressLine1";
        public static readonly string ACCOUNTDETAIL_ADDRESS2 = "accountDetails.SiteAddress.AddressLine2";
        public static readonly string ACCOUNTDETAIL_COUNTRY = "accountDetails.SiteAddress.Country";
        public static readonly string ACCOUNTDETAIL_CITY = "accountDetails.SiteAddress.City";
        public static readonly string ACCOUNTDETAIL_STATE = "accountDetails.SiteAddress.State";
        public static readonly string ACCOUNTDETAIL_ZIPCODE = "accountDetails.SiteAddress.ZipCode";
        public static readonly string ACCOUNTDETAIL_CONTACT = "accountDetails.Contact";
        public static readonly string ACCOUNTDETAIL_AWARDCLOSEDATE = "projects.AwardCloseDate";
        public static readonly string MAINEGRESS = "mainEgress";
        public static readonly string ALTERNATEEGRESS = "alternateEgress";
        public static readonly string NOOFFRONTRISERS = "FrontRisers";
        public static readonly string NOOFREARRISERS = "RearRisers";
        public static readonly string NOOFFRONTINCONFRISERS = "FrontInconRisers";
        public static readonly string NOOFREARINCONFRISERS = "RearInconRisers";
        #endregion

        #region ProductSelection

        public static readonly string PRODUCTLISTDATA = @"Templates\ProductSelection\ProductList.json";
        public static readonly string PRODUCTSELECTIONCLMREQUESTTEMPLATE = @"Templates\ProductSelection\CLMRequest.json";
        public static readonly string PRODUCTSELECTIONUIRESPONSETEMPLATE = @"Templates\ProductSelection\UIResponse.json";
        public static readonly string PRODUCTSELECTIONJAMBMOUNTEDLANDINGVALUE = @"Templates\ProductSelection\JampMountedLandingValue.json";

        #endregion

        #region ElevatorTemplates

        public static readonly string ELEVATORPOPUPUITEMPLATE = @"Templates\Group\PopUpUIResponse.json";
        public static readonly string FLOORPLANUIRESPONSETEMPLATE = @"Templates\Group\Elevator\FloorPlanUIResponse.json";
        public static readonly string CONTROLROOMSUIESPONSE = @"Templates\Group\Elevator\ControlRoomUIResponse.json";
        public static readonly string DISPLAYLOCATIONSGROUPLAYOUT = @"Templates\Group\Elevator\DisplayLocationsGroupLayout.json";
        public static readonly string MAINUIRESPONSETEMPLATE = @"Templates\Group\Elevator\MainUIResponse.json";
        public static readonly string ELEVATORENRICHMENTTEMPLATE = @"Templates\Group\Elevator\Enrichments.json";
        public static readonly string GROUPCONFIGURATIONREQESTBODYSTUBPATH = @"Templates\Group\Elevator\CLMRequest.json";
        public static readonly string GROUPLAYOUTSTUBFORPARAMETERSSP = @"Templates\Group\Elevator\GroupConfigurationParameterSP.json";
        public static readonly string GROUPDESGINATIONSTUBRESPONSEPATH = @"Templates\Group\Elevator\GroupDesignationStub.json";
        public static readonly string ELEVATORCONFIGURATIONS = @"Templates\Group\Elevator\ElevatorConfigurations.json";
        public static readonly string ELEVATORLISTDATA = @"Templates\Group\Elevator\IncludeSections.json";
        public static readonly string DOORSUITEMPLATE = @"Templates\Group\Elevator\DoorsUIResponse.json";
        public static readonly string RISERLOCATIONSUITEMPLATE = @"Templates\Group\Elevator\HallRisersUIResponse.json";
        public static readonly string HALLSTATIONTODOORSMAPPING = @"Templates\Group\Elevator\HallStationToDoorsMapping.json";
        public static readonly string CONTROLROOMID = "controlRoom";
        public static readonly string GROUPPOPUPENRICHMENTTEMPLATE = @"Templates\Group\Enrichments.json";
        public static readonly string doorsRequestbody = @"Templates\OBOM\VariableAssignments.json";
        #endregion
        #region Products
        public static readonly string EVOLUTION200 = "Evolution200";
        public const string END100 = "Endura100";
        public const string MODEL_EVO200 = "EVO_200";
        public static readonly string EVOLUTION__100 = "Evolution100";
        public const string MODEL_EVO100 = "EVO_100";
        #endregion

        public static readonly string ETAMAPPER = "ETACARFIXTURE";
        public static readonly string BASESECTION = "Base";

        #region CabInterior
        public static readonly string CABINTERIORTEMPLATE = @"Templates\Unit\{0}\CabInteriorUIResponse.json";
        #endregion

        #region UnitTemplates

        public static readonly string OTHEREQUIPMENTUIRESPONSETEMPLATE = @"Templates\Unit\{0}\OtherEquipmentUIResponse.json";
        public static readonly string ENTRANCESUIRESPONSETEMPLATE = @"Templates\Unit\{0}\Entrances\UIResponse.json";
        public static readonly string ENTRANCESCONSOLEUIRESPONSETEMPLATE = @"Templates\Unit\{0}\Entrances\ConsoleUIResponse.json";
        

        public static readonly string PRICEDETAILS = @"Templates\Unit\Evolution200\TP2Summary\priceDetails.json";
        public static readonly string PRICEDETAILSEVO100 = @"Templates\Unit\Evolution100\TP2Summary\priceDetails.json";
        public static readonly string PRICEDETAILSEND100 = @"Templates\Unit\Endura100\TP2Summary\priceDetails.json";
        public static readonly string VARIABLENAMESFORPRICING = @"Templates\Unit\Evolution200\TP2Summary\PricingCrossVariables.json";
        public static readonly string UNITCOMMONMAPPER = "UnitCommonMapper";
        public static readonly string CABINTERIORMAPPER = "CabInterior";
        public static readonly string GENERALINFOMAPPER = "GeneralInformation";

        #endregion

        #region PROJECTS

        public static readonly string PROJECTCONSTANTMAPPER = @"Templates\Integration\ConstantMapper.json";
        public static readonly string PRODUCTTREESTUBPATH = @"Templates\Integration\ProductTreeCLMRequest.json";
        public static readonly string PROJECTSENRICHEDDATA = @"Templates\Integration\Enrichments.json";
        public static readonly string VIEWEXPORTVARIABLELIST = @"Templates\Integration\View\ExportVariables.json";
        public static readonly string VIEWVARIABLEMAPPING = @"Templates\Integration\View\DictionaryVariables.json";
        public static readonly string PROJECTCOMMONNAME = "commonName";
        public static readonly string PRODUCTTREESUBMODEL = "productTreeSubModel";
        public static readonly string PROJECTVARIABLEDETAILS = "variableDetails";
        public static readonly string PRODUCTTREEVARIABLEMAPPER="productTreemapper";
        public static readonly string PRODUCTUNAVAILABLE="PRODUCTUNAVAILABLE";

        #endregion

        #region DOCCUMENTGENERATION
        public static readonly string DOCUMENTGENERATIONMAPPERPATH = @"Templates\DocumentGeneration\ConstantMapper.json";
        public static readonly string ORDERFORM = "OrderForm";
        public static readonly string ELEVATIONATBUILDINGBASE="Elevation At BuildingBase";
        public static readonly string AVEREAGEROOFHEIGHT="Average Roof Height";
        public static readonly string FRONTDOORWIDTH="Front Door Width";
        public static readonly string REARDOORWIDTH="Rear Door Width";
        public static readonly string FRONTDOORHEIGHT="Front Door Height";
        public static readonly string REARDOORHEIGHT="Rear Door Height";
        public static readonly string REEVING = "REEVING";
        public static readonly string HOISTWAYDEPTH = "HOISTWAYDEPTH";
        public static readonly string HOISTWAYWIDTH = "HOISTWAYWIDTH";
        public static readonly string REARDOORWIDTHVTPACKAGEVARIABLE = "REARDOORWIDTH";
        public static readonly string REARDOORHEIGHTVTPACKAGEVARIABLE = "REARDOORHEIGHT";
        public static readonly string  FRONTRISERS="FrontRisers";
        public static readonly string REARRISERS = "RearRisers";
        #endregion

        #region LD-Payload
        public const string COMMON_TAG_TEMPLATE= "Templates/FieldDrawingAutomation/LiftDesigner/Common.XML";
        public const string UNIT_TAG_TEMPLATE = "Templates/FieldDrawingAutomation/LiftDesigner/Unit.XML";
        public const string REQUEST_TEMPLATE = "Templates/FieldDrawingAutomation/LiftDesigner/LDRequestPayload.XML";
        public const string SHAFT_IX = "SHAFT_IX";
        public const string TRANSFACE = "TRANSFACE";
        public const string TRANSFACEF = TRANSFACE + "F";
        public const string TRANSFACER = TRANSFACE + "R";
        public const string BLDGCITY = "BLDGCITY";
        public const string COLFACE = "COLFACE";
        public const string COLFACEF = COLFACE+"F";
        public const string COLFACER = COLFACE+"R";
        public const string CARID = "CARID";
        public const string FLL_DISTANCES = "FLL_DISTANCES";
        public const string ROOFHEIGHT = "ROOFHEIGHT";
        public const string AVGRFHT = "AVGRFHT";
        #endregion
    }
}
