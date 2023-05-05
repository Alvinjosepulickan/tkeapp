using Microsoft.AspNetCore.Http;

namespace TKE.SC.Common
{
    public static class Constants
    {
        public static readonly string BRANCH_GUID = "BranchGuid";

        #region Logging related constants
        public static readonly string FORBIDDENERROR = "Invalid User";
        public static readonly string CACHEEMPTY = "Cache is Empty";
        public static readonly string GROUPSEMPTY = "No Groups Available For Selection";
        public static readonly string SESSIONEXPIRYMESSAGE = "Session got expired";
        public static readonly string SOMETHINGWENTWRONG = "Something went wrong:(";
        public static readonly string INETRNALSERVERERRORMSG = "Internal Server Error. Unable to fetch data";
        public static readonly string BADREQUESTMSG = "Bad Request";



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


        public static readonly string FDANOTNULLERRORMESSAGE = "Please give the correct groupid and quoteid";
        public static readonly string BUILDINGNAMEALREADYEXISTS = "Building Name already exists.";
        public static readonly string FLOORDESIGNATIONREPEATING = "Floor Designation is repeating";
        public static readonly string FLOORDESIGNATIONREPEATINGDESCRIPTION = "Floor Designation is repeating,Please have it unique";
        public static readonly string RELEASEINFOERROR = "Error Occurred while Releasing the Project";
        public static readonly string SOMEFIELDSAREMISSING = "Some fields are missing";
        public static readonly string SOMEFIELDSAREMISSINGDESCRIPTION = "Some fields are missing,Fill them and check again";
        public static readonly string SAVEBUILDINGEQUIPMENTCONSOLEERRORMSG = "Error occured while saving building equipment console";
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

        
        #endregion

        #region Settings related constants

        public static readonly string APPSETTINGS = "appsettings.json";
        public static readonly string PARAMSETTINGS = "ParamSettings";

        public static readonly string IDENTITYSERVERSETTINGS = "Identity";
        public static readonly string ISSUER = "Issuer";
        public static readonly string USERINFO = "UserInfo";
        public static readonly string PRODUCTIONCHECKSETTINGS = "ProductionCheckSettings";
        public static readonly string UISETTINGS = "UISettings";

        public static readonly string TTL = "TTL";

        #endregion

        #region Special characters

        public static readonly string EQUALTO = "=";
        public static readonly string ASTERISK = "*";
        public static readonly string HYPHEN = "-";
        public static readonly char HYPHENCHAR = '-';
        public static readonly string UNDERSCORE = "_";
        public static readonly char UNDERSCORECHAR = '_';
        public static readonly string DOT = ".";
        public static readonly string SPACE = "";
        public static readonly char SPACECHAR = ' ';
        public static readonly string EMPTYSPACE = " ";
        public static readonly string COMMA = ",";
        public static readonly string COLON = ":";
        public static readonly char COMMACHAR = ',';
        public static readonly char EQUALTOCHAR = '=';
        public static readonly char DOTCHAR = '.';
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
        public static readonly string APOSTROPHE = "'";
        public static readonly string OPENINGSQUAREBRACKET = "[";
        public static readonly string CLOSINGSQUAREBRACKET = "]";
        #endregion
        #region StatusCode related constants

        public static readonly int INTERNALSERVERERROR = StatusCodes.Status500InternalServerError;
        public static readonly int SERVICEUNAVAILABLE = StatusCodes.Status503ServiceUnavailable;
        public static readonly int SUCCESS = StatusCodes.Status200OK;
        public static readonly int BADREQUEST = StatusCodes.Status400BadRequest;
        public static readonly int UNAUTHORIZED = StatusCodes.Status401Unauthorized;
        public static readonly int FORBIDDEN = StatusCodes.Status403Forbidden;
        public static readonly int NOTFOUND = StatusCodes.Status404NotFound;
        public static readonly int INVALIDCONFIG = StatusCodes.Status409Conflict;

        #endregion

        /////////////////////
        #region Caching related constants
        public static readonly string CPQ = "CPQ";

        public static readonly string ENVIRONMENT = "Environment";
        public static readonly string DEV = "DEV";

        public static readonly string USERINFOCPQ = "USERINFO";
        public static readonly string USERDETAILSCPQ = "USERDETAILS";
        public static readonly string UNITDETAILSCPQ = "UNITDETAILS";
        public static readonly string FIXTUREASSIGNAMENT = "FIXTUREASSIGNAMENT";
        public static readonly string MAINFIXTUREASSIGNAMENT = "MAINFIXTUREASSIGNAMENT";
        public static readonly string USERADDRESS = "USERADDRESS";
        public static readonly string VIEWDATA = "VIEWDATA";
        public static readonly string CURRENTMACHINECONFIGURATION = "CURRENTMACHINECONFIGURATION";

        public static readonly string MACHINEREQUESTCPQ = "MACHINEREQUEST";

        public static readonly string SUBLINESRESPONSE = "SUBLINESRESPONSE";

        public static readonly string KEYBUNCH = "KEYBUNCH";

        public static readonly string PERSONA = "Persona";
        public static readonly string OBOM = "OBOM";
        public static readonly string PRODUCTTYPE = "PRODUCTTYPE";
        public static readonly string CUSTOMENGINEEREDPRODUCTS = "CUSTOMENGINEEREDPRODUCTS";
        public static readonly string HYDRAULICCUSTOMENGINEEREDPRODUCTS = "HYDARULICCUSTOMENGINEEREDPRODUCTS";
        public static readonly string DWGPENDING = "DWG_PEN";
        public static readonly string DWGSUBMITTED = "DWG_SUBD";
        public static readonly string MIXEDGROUPERROR = "MIXEDGROUPERROR";
        public const string LIFTDESIGNERBRANCH = "LIFTDESIGNERBRANCH";
        public const string LIFTDESIGNERARCH = "LIFTDESIGNERARCH";
        public const string LIFTDESIGNERGENCOUNT = "LIFTDESIGNERGENCOUNT";
        public const string LIFTDESIGNERCITY = "LIFTDESIGNERCITY";
        public static readonly string PRODUCTSELECTED = "PRODUCTSELECTED";
        public const string EVO100_USERS = "Evolution100Users";
        public static readonly string SUPPLYINGFACTORY="SUPPLYINGFACTORY";
        public static readonly string LAYTOUCH = "layTouch";
        public static readonly string GENERATEOBOM="GenerateOBOM";
        public static readonly string LDENRICHMENT = "LDEnrichment"; 
        public static readonly string OBOMVARIABLEASSIGNMENTS = "ObomVariableAssignments";
        public static readonly string DEFAULTVALUES_UPPERCASE = "xmlMainDefaultValues";

        public static readonly string CROSSPACKAGEVARIABLESUNITTOLD = "CrossPackageVariablesUnittoLD";

        public static readonly string CROSSPACKAGEVARIABLESSYSTEMBRACKETANDHEATTOLD = "CrossPackageVariablesSystemBracketandHeattoLD";

        public static readonly string SYSTEMVALIDATIONVALUES = "SystemValidationValues";

        public static readonly string MAINSECTIONENRICHEDNAMES = "xmlMainEnrichedNames";



        public static readonly string ELEVATORID = "ELEVATOR";

        public static readonly string SUMPQTYID = "SUMPQTY";

        public static readonly string COUNTERWTLOCATION = "COUNTERWTLOCATION";

        public static readonly string SUMPQTY_SP = "SUMPQTY_SP";

        public static readonly string GOVACCESS = "GOVACCESS";











        // PUBLIC_CUSTOMER FOR persona
        public static readonly string PUBLICCUSTOMER = "PUBLIC_CUSTOMER";
        public static readonly string ISPRODUCTIONCHECK = "ISPRODUCTIONCHECK";
        public static readonly string LOCALEWORD = "locale";
        public static readonly string CONFIGURATION = "Configuration";
        public static readonly string BUILDINGMAPPERCONFIGURATION = "BuildingConstantMapper";


        public static readonly string NCPCONFIGURATION = "NCPVariables";



        public static readonly string GROUPMAPPERCONFIGURATION = "GroupConstantMapper";
        public static readonly string DEFAULTVARIABLEASSIGNMENTS = "defaultVariableAssignments";
        public static readonly string HALLSTATIONMAPPEROBJECT = "HallStation";
        public const string HALLSTATIONS = "HallStationVariables";
        public static readonly string FLOORPLANRULESMAPPEROBJECT = "floorPlanRules";
        public static readonly string DOORSMATERIALVARIANT = "doorsMaterialVariant";

        public static readonly string BUILDINGEQUIPMENTVARIABLES = "BuildingEquipmentVariables";

        public static readonly string FDAVARIABLES = "fieldDrawingAutomation";        //mixed case sharing the same value with variable "FIELDDRAWINGAUTOMATION" in line 1261

        public static readonly string CUSTOMENGINEEREDVARIABLES = "CustomEngineeredConstantMapper";



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

        public const string CUSTOMENGINEEREDGEARLESSPATH = "CustomEngineeredGearlessPath";

        public const string CUSTOMENGINEEREDGEAREDPATH = "CustomEngineeredGearedPath";

        public const string CUSTOMENGINEEREDHYDRAULICPATH = "CustomEngineeredHydraulicPath";

        public const string SYNERGYPATH = "SynergyPath";


        public const string XMLEXTENSION = ".xml";


        public const string XMLFILEPATH = "\\XMLFiles";

        public const string XMLPREFIXNAME = "WrapperXML_";

        public const string DDMMYY = "ddMMyyyy";

        public const string FORWARDSLASH = "\\";
















        public static readonly string PRODUCTSELECTIONPATH = "ProductSelectionPath";


        public static readonly string LIFTDESIGNERPATH = "LiftDesignerPath";

        public static readonly string LDVALIDATIONPATH = "LDValidationPath";


        public static readonly string ESCLATORPATH = "EsclatorPath";

        public static readonly string TWINELEVATORPATH = "TwinElevatorPath";

        public static readonly string OTHERSCREENPATH = "OtherScreenPath";


        public static readonly string HEAT = "HEAT";

        public static readonly string BRACKET = "BRACKET";





        public static readonly string ENUS = "EN-US";


        public static readonly string ERROR = "DWG_ERR";


        public static readonly int DOCGEN = 2;
        public static readonly string ZEROZERO = "00";
        public static readonly string GETREQUESTLAYOUTINTERVAL = "GetRequestLayoutInterval";
        public static readonly string CONTROLLOCATIONTYPE = "ControlLocationType";
        public static readonly string CONTROLXVALUE = "CONTROLXVALUE";
        public static readonly string CONTROLYVALUE = "CONTROLYVALUE";
        public static readonly string VALIDATIONPATH = "ValidationPath";
        public static readonly string CABVALIDATIONPATH = "CabValidationPath";
        public static readonly string SLINGVALIDATIONPATH = "SlingValidationPath";
        public static readonly string EMPTYVALIDATIONPATH = "EmptyValidationPath";
        public static readonly string DUTYVALIDATIONPATH = "DutyValidationPath";
        public static readonly string MATERIALNAMEVALUES = "MATERIALNAMEVALUES";
        public static readonly string MATERIALEXTERNALID = "MATERIALEXTERNALID";

        public static readonly string CABVLAIDATIONPATHEVO100 = "CabValidationPathEvo100";
        public static readonly string CABVALIDATIONSUBPATHEVO100 = "CabValidationPathEvo100SubPath";
        public static readonly string EMPTYVALIDATIONPATHEVO100 = "EmptyValidationPathEvo100";
        public static readonly string EMPTYVALIDATIONPATHEVO100SUBPATH = "EmptyValidationPathEvo100SubPath";
        public static readonly string DUTYVALIDATIONPATHEVO100 = "DutyValidationPathEvo100";
        public static readonly string DUTYVALIDATIONEVO100SUBPATH = "DutyValidationEvo100SubPath";
        public static readonly string SLINGVALIDATIONSUBPATHEVO100 = "SlingValidationPathEvo100SubPath";
        public static readonly string SLINGVALIDATIONPATHEVO100 = "SlingValidationPathEvo100";

        public static readonly string CABVLAIDATIONPATHEND100 = "CabValidationPathEnd100";
        public static readonly string CABVALIDATIONSUBPATHEND100 = "CabValidationPathEnd100SubPath";
        public static readonly string SLINGVALIDATIONSUBPATHEND100 = "SlingValidationPathEnd100SubPath";
        public static readonly string SLINGVALIDATIONPATHEND100 = "SlingValidationPathEnd100";
        public static readonly string DUTYVALIDATIONPATHEND100 = "DutyValidationPathEnd100";
        public static readonly string DUTYVALIDATIONEND100SUBPATH = "DutyValidationEnd100SubPath";
        public static readonly string JACKDUTYVALIDATIONPATHEND100 = "JackDutyValidationPathEnd100";
        public static readonly string JACKDUTYVALIDATIONEND100SUBPATH = "JackDutyValidationEnd100SubPath";

        public static readonly string HALLRISERTYPE = "HallRiserType";
        public static readonly string HALLRISERVALUE = "HallRiserValue";
        public static readonly string DOORTYPE = "DoorType";
        public static readonly string DOORVALUE = "DoorValue";
        public static readonly string CONFIGUREVARIABLE = "ConfigureVariables";
        public static readonly string CONFIGUREVARIABLEVALUE = "ConfigureValue";
        public static readonly string FOREACH = "foreach";
        public static readonly string FOREACHVALUE = "foreachvalue";
        public static readonly string CONFIGUREVARIABLEVALUES = "ConfigureValues";
        public static readonly string SYSTEMVALIDATIONKEY="SystemVariableKeys";
        public static readonly string SYSTEMVALIDATIONVALUE="SystemVariableValues";

        public static readonly string FIXTURESTRATEGYOFCURRENTSET = "FIXTURESTRATEGYOFCURRENTSET";
        #endregion

        #region Gigya related constants


        public static readonly string ENTITLEMENT = "Entitlement";
        public static readonly string BEARER = "Bearer";

        public static readonly string SESSIONID = "SessionId";

        public static readonly string UID = "GigyaUid";



        #endregion

        #region Configuration Service Bl
        public const string GEN_S_MODEL = "GEN_S_MODEL";
        public const string GEN_S_MODEL_RANGE = "GEN_S_MODEL_RANGE";
        public const string ROOT = "ROOT";
        #endregion

        #region categories related constants


        public static readonly string F = "F";
        public static readonly string R = "R";
        public static readonly string G = "G";
        public static readonly string PRICEVARIABLES = "PriceVariables";

        #endregion

        public static readonly string FLOORMATRIXGENERALFLOORPROPERTIES = "Floor Matrix: General Floor Properties";
        public static readonly string FLOORMATRIXENTRANCEFRAME = "Floor Matrix: Entrance Frames";
        public static readonly string FLOORMATRIXHOISTWAYSILLS = "Floor Matrix: Hoistway Sills";
        public static readonly string FLOORMATRIXHALLLANTERN = "Floor Matrix: Hall Lanterns";
        public static readonly string FLOORMATRIXCOMBO = "Floor Matrix: Combo Hall Lantern/PI";
        public static readonly string COPANDLOCKEDCOMPARTMENTSTABLENAME = "COP and Locked Compartment";
        public static readonly string VARIANTS = "Variants";
        public static readonly string TRADITIONALHALLSTATIONS = "Traditional Hall Stations";
        public static readonly string DRAWINGTYPESBASEPACKAGE = "drawingTypesBasePackage";
        public static readonly string MANUFACTURINGCOMMENTSTABLENAME = "Manufacturing Comments";
        public static readonly string MANUFACTURINGCOMMENTSTABLEID = "manufacturingCommentsTable";
        public static readonly string FLOORMATRIX = "FloorMatrix";
        public static readonly string LANDINGPARAMETER="LandingParameter";
        public static readonly string LANDING = "LANDING";
        public static readonly string FLOORMATRIXPI = "Floor Matrix: Position Indicators";
        public static readonly string DRAWINGTYPESEXTERIORPACKAGE = "drawingTypesExteriorArchitecturalPackage";
        public const string CONTROLLERLOCATION = "ControllerLocation";
        public const string JAMB = "JAMB";
        public static readonly string SHAFTOBOMPATH = "ShaftOBOMPath";
        public static readonly string DRAWINGTYPESINTERIORPACKAGE = "drawingTypesInteriorArchitecturalPackage";
        public static readonly string TRACTIONREQESTBODYSTUBPATH = @"Templates\OBOM\TractionRequestBody.json";
        public static readonly string TRACTIONOBOMPATH = "TractionOBOMPath";
        public static readonly string ELECTRICALSYSTEMREQESTBODYSTUBPATH = @"Templates\OBOM\ElectricalSystemRequestBody.json";
        public static readonly string DOORSREQESTBODYSTUBPATH = @"Templates\OBOM\DoorsRequestBody.json";
        public static readonly string SIGNALIZATIONPACKAGESTUBPATH = "Signalization";
        public static readonly string CARANDSLINGPACKAGESTUBPATH = "CARSLING";
        public static readonly string ADDITIONALWIRINGVALUE = "additionalWiring";
        public static readonly string SETEXCEPTIONWIRINGDATA = "SETEXCEPTIONWIRINGDATA";
        public static readonly string SETEXCEPTIONCARPOSITION = "SETEXCEPTIONCARPOSITION";
        public static readonly string CARPOS = "CARPOS";
        public static readonly string UNITSINSET = "unitsInSet";
        public static readonly string STRINGTYPE = "String";
        public static readonly string OTHEREQUIPVALUE = "otherequipment.hoistwayWiring";
        public static readonly string UNITIDVALUE = "unitid";
        public static readonly string CONFIGUREVARIABLES = "configurevariables";
        public static readonly string CONFIGUREVALUES = "configurevalues";
        public const string WEIGHTRANGE = "The value should be between {0} lbs and {1} lbs";
        public const string OTHERWEIGHT = "OTHERWEIGHT";
        public static readonly string RELEASE = "Release";
        public const string HOISTWAYVALIDATIONMESSAGE = "Total Wiring should be less than {0} ft";
        public const string ADDITIONALWIRINGMESSAGE = "Total Wiring cannot be greater that 150 ft, so Additional Wiring cannot exceed {0} ft.";
        public static readonly string AGILEHALLSTATIONS = "AGILE Hall Stations";
        public static readonly string PROJECTBIDAWARDED = "PRJ_BDAWD";
        public static readonly string ELECTRICALSYSTEMOBOMPATH = "ElecticalSystem";
        public static readonly string DOORSOBOMPATH = "DoorsPackagePath";
        public static readonly string YES = "YES";
        public static readonly string NO = "NO";
        public static readonly string None = "None";
        public static readonly string PITDEPTHVALUE = "PITDEPTH";
        public static readonly string HANDRAILTYPE = "HANDRAILTYPE";
        public static readonly string INFOMESSAGE = "infoMessage";
        public static readonly string FACEPLT = "FACEPLT";
        public static readonly string ARRAYTYPE = "Array";
        public static readonly string SVCCBT = "SVCCBT";
        public const string SPGGETCARPOSITIONBYGROUPID = "usp_GetCarPositionByGroupId";
        public const string UPDATEGROUPCONFLICTSTATUS = "usp_UpdateGroupConflictStatus";
        public static readonly string FIRECBT = "FIRECBT";
        public const string SPUPSERTPRICELINE= "usp_UpsertCustomPriceLine";
        public const string SPDELETEPRICELINE = "usp_DeleteCustomPriceLine";
        public const string SPCREATEUPDATEFACTORYJOBID = "usp_CreateUpdateFactoryJobId";
        public const string USPGETJOBIDFORDA = "usp_GetJobIdForDA";
        public const string USPSAVEUPDATEJOBDETAILSFORDA = "usp_SaveUpdateJobDetailsForDA";
        public const string USPSAVEUPDATEHANGFIREJOBDETAILSFORDA = "usp_SaveUpdateHangFireJobDetailsForDA";
        public const string WORKFLOWSTAGE = "WorkFlowStage";
        public const string WORKFLOWSTAGEPASCAL = "workFlowStage";
        public const string FACTORYJOBID = "FactoryJobID"; 
        public const string ORDER = "PRJ_ORD"; 
        public const string ESCLATORMOVINGWALK = "Escalator/Moving-Walk";
        public const string ESCALATORTYPE = "Escalator";
        public const string SPGETDETAILSFOROBOMXMLGENERATION = "usp_GetdetailsForOBOMXMLGeneration";
        public const string SPGETCONFIGURATIONDETAILSFORSTATUSREPORT = "usp_GetConfigurationDetailsForStatusReport";
        public const string SETID = "@setId";
        public const string SELECTEDTAB = "@selectedTab";
        public const string SETIDCOLUMNNAME = "SetId";
        public const string QUOTEIDSPVARIABLE= "@quoteId";
        public const string TRACTIONHOISTWAYEQUIPMENT = "tractionhoistwayequipment";
        public const string PRODUCTELEVATOR = "Elevator";
        public static readonly string CARSLINGREQESTBODYSTUBPATH = @"Templates\OBOM\OBOMCarSlingRequestBody.json";
        public const string OTHER = "Other";
        public static readonly string TWODPARAMTERLIST = @"Templates\OBOM\2DParameters.json";
        public static readonly string THREEDPARAMTERLIST = @"Templates\OBOM\3DParameters.json";
        public static readonly string PARTNUM = ".PARTNUM";
        public static readonly string QTY = ".QTY";
        public static readonly string DESC = ".DESC";
        public static readonly string GHFMASTERTOLANDINGMAPPING = @"Templates\OBOM\GHFMastertoLandingMapping.json";
        public static readonly string UNITMASTERTOLANDINGMAPPING = @"Templates\OBOM\UnitMastertoLandingMapping.json";
        public static readonly string OBOMCONSTANTMAPPERPATH = @"Templates\OBOM\ConstantMapper.json";
        public static readonly string OBOMVARIABLEMAPPERPATH = @"Templates\OBOM\VariablesMapper.json";
        public const string ELEVBASE = "ELEVBASE";
        public const string VARIABLETYPE= "VariableType";
        public const string TWINELEVATOR = "TWIN Elevator"; 
        public const string VARIABLEVALUE="VariableValue";
        public const string INCLINEDCAMALCASE = "Inclined"; 
        public const string HORIZONTAL = "Horizontal";
        public static readonly string FLOORMATRIXVALUEFORVFD = "FLOORMATRIXVALUEFORVFD";
        public static readonly string CARPOSITIONVALFORVFD = "CARPOSITIONVALFORVFD";
        public static readonly string CONTROLROOMVALUE = "CONTROLROOMVALUE";
        public static readonly string CONTROLLERLANDINGVALVFD = "CONTROLLERLANDINGVALVFD";
        public static readonly string FDAVALUE = "FDA";
        public static readonly string VFDVARIABLE = "VFDVARIABLE";
        public const string PROPERTIES = "properties";
        public static readonly string REAR = "Rear";
        public static readonly string FRONT = "Front";
        public const string CONTROLROOMFLOOR = "CONTROLROOMFLOOR";
        public const string ELECTRICALCALCULATION="ElectricalCalculation";
        public const string ENDURACARSPEEDVARIABLE = "CARSPDHUT";
        public const string SETIDCAMELCASE = "setId";

        public const string TOKENSECTIONS = "sections";
        public const string FLOORPLANLAYOUT = "floorPlanLayout";
        public const string SAVEGROUPLAYOUT = "SaveGroupLayout";
        public const string TOKENID = "id";
        public const string BEAMWALLSECTIONKEY = "fda.beamWall";
        public const string LAYOUTSECTIONKEY = "fda.unitLayoutDetails";
        public const string LAYOUTGENERATIONSETTINGSSECTIONKEY = "fda.layoutGenerationSettings";
        public const string DRAWINGSECTIONKEY = "fda.layoutGenerationSettings.drawingTypes";
        public const string OUTPUTSECTIONKEY = "fda.layoutGenerationSettings.outputTypes";
        public const string HASH = "#";
        public const string BYRULEUPPERCASE = "ByRule";
        public const string BYDEFAULTUPPERCASE = "ByDefault";
        public static readonly string UNITDESIGNATIONCOLUMN = "Designation";
        public static readonly string SOURCEENV = "SOURCEENV";
        public static readonly string TARGETENV = "TARGETENV";
        public static readonly string SENDINGTYPE = "SendingType";
        public static readonly string PLANT = "Plant";
        public static readonly string GROUPCONFIGURATIONTYPE = "GroupConfigurationType";
        public static readonly string SPECMEMOVERSION = "SpecMemoVersion";
        public static readonly string UNITJSON = "UnitJson";
        public static readonly string GROUPCONFIGURATIONVALUE = "GroupConfigurationValue";
        public const string GEARLESSPACKAGEID = "GearlessPackageId";
        public const string GEAREDPACKAGEID = "GearedPackageId";
        public const string HYDRAULICPACKAGEID = "HydraulicPackageId";
        public const string SYNERGYPACKAGEID = "SynergyPackageId";
        public static readonly string UNITIDCOLUMN = "UnitId";
        public const string LDPACKAGEID = "LDPackageId";
        public const string LDVALIDATIONPACKAGEID = "LDValidationPackageId";
        public const string LDHEATPACKAGEID = "LDHeatPackageId";
        public const string LDBRACKETPACKAGEID = "LDBracketPackageId";
        public const string MANUFACTORINGCOMMENTS = "ManufacturingComments";
        public const string ESCALATORPACKAGEID = "EscalatorPackageId";
        public const string TWINELEVATORPACKAGEID = "TwinElevatorPackageId";
        public const string OTHERSCREENPACKAGEID = "OtherScreenPackageId";
        public const string SECTIONTOBATCH = "SECTIONTOBATCH";
        public const string CUSTOMPRICEKEY= "TK_PriceKey";
        public static readonly string DoorsParameters = @"Templates\OBOM\DoorsParameters.json";
        public static readonly string SELECTEDDOOR="DoorJson";
        public const string BASEPACKAGESECTION1 = "Cover Sheets (One drawing for Group)";
        public const string BASEPACKAGESECTION2 = "Customer Verification Sheet (One drawing for each Set. This document has 'editable' fields that the customer fills in or chooses from an LOV.)";
        public const string SC = "SC";
        public const string ELECTRICALCALCULATIONHARDCODEDDVARIABLES = "ElectricalCalculationHardCodedVariable";
        public const string EXTERIORSECTION1 = "Entrances (One entrance drawing package per set, each unique entrance configuration will be displayed with floor markings). These entrance drawings will show location/size of hall fixtures as well.";
        public const string EXTERIORSECTION2 = "Hall Fixtures (One drawing for each unit and landing)";
        public const string ISDELETED="IsDeleted";
        public static readonly string DISPLAYNAMESTATUS = "DisplayName";

        public const string INTERIORSECTION1 = "Car Fixtures (one interior package for each set)";
        public const string INTERIORSECTION2 = "Cabs (One drawing for each Set)";

        public const string LAYOUTDRAWINGSSECTION1 = "Layout Drawing Package (per group)includes:";

        public const string LAYOUTDRAWINGSSECTION2 = "Layout Section View (per set)";

        public const string LAYOUTDRAWINGSSECTION3 = "Layout (single layout per group):";

        public const string LAYOUTDRAWINGSSUBSECTION1 = "Pit(per group)";

        public const string LAYOUTDRAWINGSSUBSECTION2 = "Hoistway (per group)";

        public const string LAYOUTDRAWINGSSUBSECTION3 = "Overhead view (per group)";

        public const string LAYOUTDRAWINGSSUBSECTION4 = "Rail Stack Page (per set)";

        
        public static readonly string DRAWINGSAPISETTINGS = "DrawingsApi";
        public static readonly string GENERALINFORMATIONVALUE = "generalInformation";


        public static readonly string CRMOD = "CRMOD";

        public static readonly string XMLITEM = "item";
        public static readonly string XMLNAME = "NAME";

        public static readonly string XMLVALUE = "VALUE";

        public static readonly string XMLHEAD = "<?xml version='1.0' encoding='utf-8'?>";

        public static readonly string XMLBODY = "BODY";

        public static readonly string XMLCOMMON = "COMMON";

        public static readonly string XMLVALUES = "VALUES";
        public static readonly string XMLUNITS = "UNITS";
        public static readonly string MANUALCSC = "ManualCSC";
        public static readonly string AUTOMATED = "Automated";
        public static readonly string ISCOMPLETED = "Completed";
        public static readonly string INCOMPLETESTATUSCODE = "UNIT_INC";
        public static readonly string INCOMPLETESTATUS = "Incomplete";
        public static readonly string STATUSKEYUNIT_INC = "UNIT_INC";
        public static readonly string STATUSKEYUNIT_SVINP = "UNIT_SVINP";
        public static readonly string INCOMPLETEDESCRIPTIONSTATUS = "Unit configuration is not complete";
        public static readonly string INPROGRESSSTATUSCODE = "SysValInProgress";
        public static readonly string INPROGRESSSTATUS = "Validating..";
        public static readonly string INPROGRESSDESCRIPTIONSTATUS = "System validation of the Unit is in progress";
        public static readonly string ECARWT = "ECARWT";
        public const string SUMMARYQUOTEID = "quoteid";
        public const string SUMMARYBUILDING = "building";
        public const string DISPLAYVARIABLEASSIGNMENTS = "displayVariableAssignments";
        public const string BANKTWOPOSITION = "B2P";
        public static readonly string OUTPUTTYPES = "outputTypes";
        public static readonly string DRAWINGTYPES = "drawingTypes";
        public static readonly string DBTYP = "DBTYP";
        public static readonly string WALL = "WALL";
        public static readonly string NODATACACHE = "No Data in Cache.";
        public const string CAPACITY = "CAPACITY";
        public const string SPEED = "SPEED";
        public const string WIDTH = "WIDTH";
        public const string DEPTH = "DEPTH";
        public const string PITDEPTH = "PIT DEPTH";
        public const string OVERHEAD = "OVERHEAD";
        public const string MACHINETYPE = "MACHINETYPE";
        public const string MOTORTYPESIZE = "MOTORTYPESIZE";
        public static readonly string CAPACITYPARAMETER = "ELEVATOR.Parameters.CAPACITY";
        public static readonly string CARSPEEDPARAMETER = "ELEVATOR.Parameters.CARSPEED";
        public static readonly string HYDWIDPARAMETER = "ELEVATOR.Parameters.HWYWID";
        public static readonly string HYDDEPPARAMETER = "ELEVATOR.Parameters.HWYDEP";
        public static readonly string PITDEPTHPARAMETER = "ELEVATOR.Parameters.PITDEPTH";
        public static readonly string OVHEADPARAMETER = "ELEVATOR.Parameters.OVHEAD";
        public static readonly string VARIABLEVALUES = "variables";
        public const string CARSPEEDVARIABLE = "CARSPEED";
        public const string TYPSVC = "TYPSVC";
        public const string FRONTDOORHEIGHTMAP = "FRONTDOORHEIGHT";
        public const string REARDOORHEIGHTMAP = "REARDOORHEIGHT";
        public const string FRONTDOORWIDTHMAP = "FRONTDOORWIDTH";
        public const string REARDOORWIDTHMAP = "REARDOORWIDTH";
        public const string FRONTDOORTYPEMAP = "FRONTDOORTYPE";
        public const string REARDOORTYPEMAP = "REARDOORTYPE";
        public const string LAYOUT = "Layout";
        public const string STRINGVAL = "String";
        public const string LEFTLAYOUT = "col-md-6 mr-md-1 col-sm-12";
        public const string FULLLAYOUT = "col-md-6 col-sm-12";
        public const string IDTWO = "2";
        public const string FRONTDOORTYPEANDHAND = "FRONTDOORTYPEANDHAND";

        public const string BUINDINGTYPE = "BuindingType";
        public const string BUINDINGVALUE = "BuindingValue";
        public static readonly string PARAMETERFRONTOPENINGS = "Parameters.Num_FrontOpenings";
        public static readonly string PARAMETERREAROPENINGS = "Parameters.Num_RearOpenings";

        public const string ENDURA100_PRODUCT = "Endura 100";
        public const string EVO100_PRODUCT = "Evolution 100";


        public const string QUOTEID = "quoteId";
        public const string BUILDINGID = "buildingId";
        public const string GROUPID = "groupId";
        public const string UNITID = "unitId";
        public const string PRODUCTNAME = "productName";

        #region Logic related constants

        public static readonly string BYUSER = "BYUSER";
        public static readonly string BYDEFAULT = "BYDEFAULT";
        public static readonly string BYRULE = "BYRULE";
        public static readonly string RESULT = "Result";
        public static readonly string BYUSER_LOWERCASE = "byUser";
        public static readonly string FRONTVALUE = "FRONTVALUE";
        public static readonly string REARVALUE = "REARVALUE";
        public static readonly string FLOORMATRIXVALUE = "FLOORMATRIXVALUE";
        public static readonly string ELEVATIONPARAMETER = "ELEVATIONPARAMETER";
        public static readonly string CARPOSITIONVAL = "CARPOSITIONVAL";
        public static readonly string CONTROLLERLANDINGVAL = "CONTROLLERLANDINGVAL";
        public static readonly string BNKDISTANCE = "BNKDISTANCE";
        public static readonly string BNKSEMIDISTANCE = "BNKSEMIDISTANCE";
        public static readonly string GENERICVARIABLE = "GENERICVARIABLE";
        public static readonly string FRONTVAL = "front";
        public static readonly string REARVAL = "rear";
        public static readonly string BuildingType = "BuindingType";
        public static readonly string BuildingValue = "BuindingValue";
        public static readonly string FLOORNUMBERS = "FloorNumber";
        public static readonly string ELEVATIONFEET = "ElevationFeet";
        public static readonly string CONTROLLERLANDING = "ControllerLanding";
        public static readonly string MAPPEDLOCATION = "MappedLocation";
        public static readonly string VARIABLEIDS = "VariableId";
        public static readonly string CURRENTVALUE = "CurrentValue";
        public static readonly string NUMBEROFFLOORSUNITPACKAGEVARIABLES = "NUMBEROFFLOORSUNITPACKAGEVARIABLE";
        public static readonly string NUMBEROFFLOORSSEMIUNITPACKAGEVARIABLES = "NUMBEROFFLOORSSEMIUNITPACKAGEVARIABLES";
        public static readonly string NUMBEROFFLOORS = "NUMBEROFFLOORS";
        public static readonly string BLANDINGVARIABLE = "BLANDINGVARIABLE";
        public static readonly string OPPMACH = "OPPMACH";
        public static readonly string BELOW = "BELOW";
        public static readonly string VFDWIRING = "VFDWIRING";
        public static readonly string TOTALWIRINGS = "TOTALWIRING";
        public static readonly string ADDITIONALWIRING = "ADDITIONALWIRING";
        public static readonly string PARAMETERCONTROLLOCATION = "PARAMETERCONTROLLOCATION";
        public static readonly string CONTROLROOMQUAD = "CONTROLROOMQUAD";
        public const string CONTROLCLOSET = "CONTROLCLOSET";
        public const string HOISTWAYENTRY = "HOISTWAYENTRY";
        public const string REMOTEDEFAULTVALUE = "Q1";
        public const string ADJACENTDEFAULTVALUE = "Q1P1";

        public static readonly string REQUIRED = "REQUIRED";
        public static readonly string PRIMARYCOORDINATOR = "PrimaryCoordinator";
        public static readonly string ORACLEPROJECTID = "OracleProjectId";
        public static readonly string BUILDINGIDCOLUMNNAME = "BuildingId";
        public static readonly string GROUPIDCOLUMNNAME = "GroupId";
        public static readonly string UNITIDCOLUMNID = "UnitId";
        public static readonly string BUILDINGNAMECOLUMNNAME = "BuildingName";
        public static readonly string GROUPNAMECOLUMNNAME = "GroupName";
        public static readonly string UNITNAMECOLUMNNAME = "UnitName";
        public const string BUILDING = "Building ";
        public const string BUILDINGCAPS = "BUILDING";
        public const string GROUPCAPS = "GROUP";
        public const string GROUP = "Group";
        public const string SETLOWERCASE = "set";
        public const string QUOTEVERSION = "QuoteVersion";
        public static readonly string UEID = "UEID";
        public static readonly string PRODUCTNAMECOLUMN = "ProductName";
        public static readonly string FRONTOPENING = "FrontOpening";
        public static readonly string REAROPENING = "RearOpening";
        public const string UNIT = "Unit ";
        public static readonly string PROJECTNAME = "ProjectName";
        public static readonly string PROJECT = "Project";
        public const string UNITCAPS = "UNIT";
        public const string UNITS = "Units ";
        public const string CREATEDSUCCESSFULLY = " created successfully";
        public const string OVERWRITTENSUCCESSFULLY = " overwritten successfully";
        public static readonly string PROJECTIDCOLUMNNAME = "ProjectId";

        public const string FDASAVEPROCESSINGMESSAGE = "Drawings is Processing...Please check after sometime.";

        
        public const string OVERWRITE = "OVERWRITE";
        public const string SHAFTMECHANICS = "SHAFTMECHANICS";
        public const string DOORS = "DOORS";
        public const string TRACTION = "TRACTION";
        public const string SIGNALIZATION = "SIGNALIZATION";
        public const string CARANDSLING = "CARANDSLING";
        public const string BUILDINGPACKAGEPATH = "buildingconfiguration";
        public const string GROUPPACKAGEPATH = "groupconfiguration";
        public const string UNITPACKAGEPATH = "unitvalidation";
        public const string PRODUCTSELECTIONPACKAGEPATH = "productselection";
        public static readonly string VALUE = "value";
        public const string ELECTRICALSYSTEM = "ELECTRICALSYSTEM";
        public const string LIFTDESIGNERPACKAGEPATH = "liftdesigner";
        public const string CARSLING = "CARSLING";
        public const string GLOBALARGUMENTS = "globalArguments";
        public const string MATERIALVARIANT = "MaterialVariant";
        public const string MATERIAL = "Material";
        public const string CONFIGURABLEMATERIALNAME = "ConfigurableMaterialName";
        public const string EXTERNALID = "ExternalId";
        public const string LINE = "line";
        public const string REQPRODUCTID = "ProductId";
        public const string PRODUCT = "product";
        public const string ID = "Id";
        public const string VARIABLEID = "variableId";
        public static readonly string SHAFTMECHANICSREQESTBODYSTUBPATH = @"Templates\OBOM\ShaftMechanicsRequestBody.json";
        public static readonly string AVGROOFHEIGHT = "avgRoofHeight";
        public static readonly string SIGNALIZATIONREQESTBODYSTUBPATH = @"Templates\OBOM\SignalizationRequestBody.json";
        public static readonly string CARANDSLINGREQESTBODYSTUBPATH = @"Templates\OBOM\CarAndSlingRequestBody.json";
        public static readonly string OBOMREQESTBODYSTUBPATH = @"Templates\OBOM\OBOMRequestBody.json";
        public static readonly string OBOMINCLUDESECTIONPATH = @"Templates\OBOM\IncludeSections.json";

        public static readonly string CONFIGITOBOMREQUESTBODYPATH1 = @"Templates\DesignAutomation\{0}.json";
        public static readonly string CONFIGITOBOMREQUESTBODYPATH = @"Templates\DesignAutomation\ConfigitOBOMRequestBody.json";
        public static readonly string DAPACKAGEPATH = @"Templates\DesignAutomation\DAPackagePath.json";
        public static readonly string DESIGNAUTOMATION = "DesignAutomation"; 
        public static readonly string GETOBOM = "GetOBOM"; 
        public static readonly string DEFAULTEXPORTTYPES = "DefaultExportTypes";
        public static readonly string AVAILABLEEXPORTTYPES = "AvailableExportTypes";
        public static readonly string SUBMITBOM = "SubmitBOM"; 
        public static readonly string AUTOMATIONSTATUS = "AutomationStatus";
        public static readonly string PACKAGEPATH = "PackagePath"; 
        public static readonly string OBOMSERVICEURL = "OBOMServiceUrl";
        public static readonly string DASERVICEURL = "DaServiceUrl";
        public static readonly string DAOUTPUTPATH = "DAOutputPath";
        public static readonly string JOBID = "jobId"; 
        public static readonly string JOBID_PASCALCASE = "JobId"; 
        public static readonly string HANGFIREJOBID_PASCALCASE = "HangFireJobId";
        public static readonly string PACKAGENAME_PASCALCASE = "PackageName";
        public static readonly string DAJOBSTATUS_PASCALCASE = "DAJobStatus";
        public static readonly string JOBSTATUS_PASCALCASE = "JobStatus";
        public static readonly string FULLYQUALIFIEDNAME = "fullyqualifiedname";
        public static readonly string BOMGROUP = "bomgroup";
        public const string DESIGNAUTOMATIONSTATUSID ="daStatusId";
        public const string DESIGNAUTOMATIONSTATUSKEY = "daStatusKey";
        public const string DESIGNAUTOMATIONDISPLAYNAME ="daDisplayName";
        public const string DESIGNAUTOMATIONSTATUSNAME ="daStatusName";
        public const string DESIGNAUTOMATIONDESCRIPTION ="daDescription"; 
        public const string EXTERIORPACKAGE = "drawingTypesExteriorArchitecturalPackage"; 
        public const string INTERIORPACKAGE = "drawingTypesInteriorArchitecturalPackage"; 
        public const string OUTPUTTYPE = "OutputTypes"; 
        public const string PDFOUTPUTTYPE = "outputTypesPdf";
        public const string DWGOUTPUTTYPE = "outputTypesDWG";
        public const string ARCHITECTURALPACKAGE = "ArchitecturalPackage";

        public static readonly string JOBID_CAMELCASE = "@jobId";
        public static readonly string HANGFIREJOBID = "@hangfireJobId";
        public static readonly string HANGFIREJOBSTATUS = "@jobStatus";
        public static readonly string OUTPUTLOCATION = "@outputLocation";
        public static readonly string DADETAILS = "@daDetails";
        public static readonly string BACKSLASH = "\\";
        public static readonly string QUESTIONMARK = "?";
        public static readonly char DOUBLE_QUOTES = '"';
        public static readonly char EMPTY = ' ';
        public static readonly string SLDASM = "sldasm";
        public static readonly string SAVEJOBIDERRORMSG = "unable to Save Job Ids...";
        public static readonly string DASTATREDMESSAGE = "Design Automation is in progress. Please Check the status after some time";




        public static readonly string BUILDINGENTITY = "building";
        public static readonly string GROUPENTITY = "Group";
        public static readonly string UNITENTITY = "Unit";
        public static readonly string PRODUCTENTITY = "ProductSelection";

        #endregion


        #region Basket related constants

        public static readonly string BASKETASSIGNEDPROP = "AssignedByBasket";

       

        #endregion

        #region Building and Car related constants

        public static readonly string REGEN = "ELEVATOR.Parameters.Electrical_System.REGEN";
        public static readonly string COPANDLOCKEDCOMPARTMENT = "carFixture.copAndLockedCompartment";
        public static readonly string CAROPERATINGPANEL = "carFixture.carOperatingPanel";
        public static readonly string CARCALLCUTOUTKEYSWITCH = "Car Call Cutout Keyswitches";
        public static readonly string CARRIDINGLANTERNQUANTITY = "carFixture.carRidingLanternQuantity";
        public static readonly string CARRIDINGLANTERNQUANTITYSP = "CARRIDINGLANTERNQUANTITYSP";
        public static readonly string TOTALDEVICESLOTSSP = "ELEVATOR.Parameters_SP.totalDeviceSlots_SP";

        public static readonly int TOTALDEVICELIMIT = 9;
        public static readonly string PARAMETERELECTRICALSYSTEM = "Parameters.Electrical_System";
        public static readonly string ELEVSYSPARAMETER = "ELEVSYSPARAMETER";
        public static readonly string EVO = "EVO";
        public static readonly string CARPOSITIONLOCATION = "CarPositionLocation";
        public static readonly string BUILDINGCONFIGURATIONVARIABLELIST = "BuildingConfiguration";
        public static readonly string BUILDINGCONSOLECONFIGURATIONVARIABLELIST = "BuildingEquipmentConsoleConfiguration";
        public static readonly string BUILDINGEQUIPMENTCONFIGURATIONVARIABLELIST = "BuildingEquipmentConfiguration";
        public static readonly string CONTROLLOCATIONVARIABLELIST = "ControlLocation";
        public static readonly string UNITCNFIGURATIONVARIABLELIST = "UnitConfiguration";
        public static readonly string PARAMETERlAYOUT = "Parameters.Layout.";
        public const string UNITNAMEREPEATING = "Unit Name/s = already used. Please enter unique names";

        #endregion

        #region UnitBL related constants

        public static readonly string CURRENCYCODE = "USD";
        public static readonly string MANUFACTURINGCOMMENTS = "ManufacturingComments";
        public static readonly string CORPORATEASSISTANCE = "CorporateAssistance";
        public static readonly string PRODUCTSUBSIDIES = "ProductSubsidy";
        public static readonly string CORPORATESUBSIDIES = "CorporateSubsidies";
        public static readonly string STRATEGICDISCOUNT = "StrategicDiscount";
        public static readonly string MANUFACTURINGDISCOUNTS = "ManufacturingDiscount";
        public static readonly string TOTALPRICEVALUES = "totalPrice";
        public static readonly string HANDRAILHEIGHT = "HANDRAILHEIGHT";
        public static readonly string CORPORATEASSISTANCECAMALCASE = "corporateAssistance";
        #endregion



        #region Response Message Related Constants
        public const string OPENINGLOCATIONUPDATE = "{\"message\":\"Opening Location updated successfully\"}";
        public const string UNITDESIGNATIONMESSAGE = "{\"message\":\"UnitDetails updated successfully\"}";
        public const string FACTORYJOBIDMESSAGE = "{\"message\":\"Factory Job ID updated successfully\"}";
        public const string UNITERRORMESSAGE = "Unit cannot be configured";
        public const string UNITSETERRORMESSAGE = "Units cannot be configured together";
        public const string UNITSETERRORMESSAGEOPENING1 = "Please assign at least two openings in Group Opening Locations to proceed further";
        public const string UNITSETERRORMESSAGE1 = "Selected Units cannot be configured together";
        public const string UNITSETERRORFORCONFLICTINGOPENINGS = "Cannot Configure a set with Conflicting opening locations selection";

        public const string UNITCONFIGURATIONUPDATEERRORMESSAGE = "Error while Updating Unit Configuration";
        public const string UNITCONFIGURATIONSAVEERRORMESSAGE = "Error while Saving Unit Configuration";
        public const string UNITDESIGNATIONPARAMETERERRORMESSAGE1 = "Unit Designation and Description cannot be empty";
        public const string UNITDESIGNATIONPARAMETERERRORDESCRIPTION = "Please add designation and description";
        public const string FACTORYJOBERRORMESSAGE = "Factory Job ID  cannot be empty";
        public const string FACTORYJOBERRORDESCRIPTION = "Please add Factory Job ID";
        public const string FACTORYJOBERRORMESSAGE1 = "Factory Job ID should be unique";
        public const string UNITDESCRIPTIONERRORMESSAGE1 = "Unit Details should be unique";
        public const string DELETEENTRANCECONSOLEERRORMSG = "Error occurred while deleting entrance console";
        public const string DELETEUNITHALLFIXTURECONSOLEERRORMSG = "Cannot Delete with null values";
        public const string NOTFOUNDERRORMSG = "Value not found";
        public const string PRICEDETAILSSAVEERROR = "Error while saving price details";
        public const string COUNTRYNAMEERRORMESSAGE = "Country's name cannot have special characters";

        public const string ADDQUOTEERRORMESSAGE = "Add Quote not be created";



        public const string SOMETHINGWENTWRONGMSG = "Something went wrong";
        public const string INVALIDGROUPID = "Invalid Group Id";
        public const string INVALIDINPUT = "Invalid input/s";
        public const string SAVEPRODUCTSELECTIONMESSAGE = "Product selection saved successfully";
        public const string EDITINDEPENDENTMESSAGE = "New set created for selected Unit";
        public const string INVALIDOPPORTUNITYID = "Invalid opportunity Id";
        public const string SAVEDISCOUNTERRORMESSAGE = "Sum of discounts cannot be greater than the price of the unit";
        #endregion
        public static readonly string PROJECTSALESTAGE = "ProjectSalesStage";
        public static readonly string UNITVARIABLESFORQUICKSUMMARY = @"Templates\Unit\QuickSummaryVariables.json";
        public static readonly string INPUTJSONPATH = "../TKE.CPQ.AppGateway.Test/InputJson/";
        public static readonly string BUILDINGNAMEVARIABLEID = "BUILDINGNAMEVARIABLEID";
        public static readonly string BUILDINGYEAR = "BUILDINGYEAR";
        public static readonly string YEAR = "2016";
        public static readonly string CONFLICTMANAGEMENT = "conflictManagement";
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
        public static readonly string LANDINGVARIABLEREAR = "LandingVariableRear";
        public static readonly string LANDINGVARIABLEFRONT = "LandingVariableFront";
        public static readonly string LANDINGVARIABLE = "LandingVariable";
        public static readonly string NR = "NR";
        public static readonly string FIRE_KEY_BOX = "Fire_Key_Box";
        public static readonly string FIREBOXCONSTANT = "FIREBOX";
        public static readonly string FIREPHONEJACK = "FIREPHONEJACK";
        public static readonly string FEATURESPERPANELL = "FEATURESPERPANELL";
        public static readonly string SENDTOCOORDINATIONDATA = "sendToCoordinationData";
        public static readonly string PARAMETERSVARIABLE = "Parameters.";
        public const string SEQUENCE = "sequence";
        public const string BYDEFAULTVALUE = "byDefault";
        public const string SECTIONNAME = "sectionname";
        public const string SECTIONNAMEPASCAL = "sectionName";
        public static readonly string NUMBERLOWER = "number";
        public static readonly string BOOLEANLOWER = "boolean";
        public static readonly string ENTRANCECONSOLE = "EntranceConsole";
        public static readonly string LOBBYPANEL = "Lobby Panel ";
        public static readonly string LOBBYPANELSECTION = "lobbypanelsection";
        public static readonly string BACNETSECTION = "bacnetsection";
        public static readonly string ROBOTICCONTROLLERSECTION = "roboticcontrollerinterfacesection";
        public static readonly string SMARTRESCUEPHONE5SECTION = "smartrescuephone5section";
        public static readonly string SMARTRESCUEPHONE10SECTION = "smartrescuephone10section";
        public static readonly string SMARTRESCUEPHONESTANDALONENAME = "Smart Rescue Phone(Standalone)";
        public static readonly string SMARTRESCUEPHONESTANDALONE = "buildingEquipment.smartRescuePhone(Standalone)";
        public static readonly string BACNETSECTIONS = "BACNet Section";
        public static readonly string ROBOTICCONTROLLERSECTIONS = "Robotic Controller Interface Section";
        public static readonly string THIRDPARTYINTERFACES = "buildingEquipment.thirdPartyInterfaces";
        public static readonly string SMARTRESCUEPHONE5SECTIONS = "Smart Rescue Phone 5 Section";
        public static readonly string SMARTRESCUEPHONE10SECTIONS = "Smart Rescue Phone 10 Section";
        public static readonly string BUILDINGEQUIPMENTCONSOLE = "BuildingEquipmentConsole";
        public static readonly string BUILDINGEQUIPMENTCONSOLECHANGE = "ChangeBuildingEquipmentConsole";
        public static readonly string BACNETNAME = "BACNet Interface";
        public static readonly string ZERO = "0";
        public static readonly string UNITHALLFIXTURECONSOLE = "UnitHallFixtureConsole";
        public static readonly string ENTRANCECONSOLEID="EntranceConsoleId";
        public static readonly string GROUPHALLFIXTURECONSOLE = "GroupHallFixtureConsole";
        public static readonly string HOISTWAYACCESS = "Hoistway_Access"; 
        public static readonly string SPAREQUANTITY = "SPAREQUANTITY"; 
        public static readonly string BUILDING_CONFIGURATION = "Building_Configuration";
        public static readonly string LISTOFUNITS = "ListOfUnits";
        public static readonly string REAROPEN = "REAROPEN";
        public const string DISPLAYNAME = "displayname";
        public const string FIXTURESTRATEGY = "FixtureStrategy";
        public const string FIXTURESTRATEGYVARIABLE = "FIXTURESTRATEGYVARIABLE";
        public const string CARCALLCUTOUTKEYSWITCHESCONSOLE = "CarCallCutoutKeyswitchesConsole";
        public const string GETVARIABLESCACHE = "GETVARIABLESCACHE";
        public static readonly string ELEVATOR_PARAMETERS_SP = "ELEVATOR.Parameters_SP";
        public static readonly string DEVICESLOTERRORMESSAGE = "Total number of device slots has exceeded the maximum limit of 9. Remove or move some devices to another location";
        public const string FIXTURESTRATEGY_SP = "fixtureStrategy_SP";
        public const string HALLSTATIONPARAM = "Parameters_SP.HS";
        public const string PRICEKEYDETAILS = "PriceKeyDetails";
        public static readonly string OPENFRONT = "OPENFRONT";
        public static readonly string HOISTWAYDIMENSIONVARIABLE = "HOISTWAYDIMENSIONVARIABLE";
        public static readonly string OPENREAR = "OPENREAR";
        public static readonly string BUMPERHEIGHT = "BUMPERHEIGHT";
        public static readonly string TOTALWIRING = "TOTALWIRING";
        public static readonly string COUNTERWEIGHTSAFETY = "COUNTERWEIGHTSAFETY";
        public static readonly string TOTALOPENINGS_SP = "ELEVATOR.Parameters_SP.TOTALOPENINGS_SP";
        public const string GROUPCONFIGURATIONNAME = "group";
        public const string ELEVATORFIXTURESTRATEGY = "ELEVATOR.Parameters_SP.fixtureStrategy_SP";
        public const string FIXTURESTRATEGYVRAIBLEID = "FIXTURESTRATEGYVRAIBLEID";
        public const string PRICINGCROSSVARIABLES = "PricingCrossVariables";
        public const string TOTALPRICE = "TotalPrice";
        public const string PRODUCTTREE = "ProductTree";
        public const string BATCH1LEADTIME = "batch1LeadTime";
        public const string BATCH2LEADTIME = "batch2LeadTime";
        public const string BATCH3LEADTIME = "batch3LeadTime";
        public const string BATCH4LEADTIME = "batch4LeadTime";
        public const string BATCH5LEADTIME = "batch5LeadTime";
        public const string MANUFACTURINGLEADTIME = "manufacturingLeadTime";
        public static readonly string FEETINCHVARIABLESLIST = @"Templates/History/FeetInchConversionVariableList.json";
        public static readonly string VARIABLENOTUSINGVALUEENRICHMENTLIST = @"Templates/History/VariablesNotUsingValueEnrichment.json";
        public static readonly string GROUPUNITPOSITIONDISPLAYNAME = @"Templates/History/UnitPositionDisplayName.json";
        public static readonly string BUILDINGCONFIGURATIONINCLUDESECTIONVALUES = "Templates/Building/IncludeSections.json";
        public static readonly string BUILDINGEQUIPMENTINCLUDESECTIONVALUES = "Templates/Building/BuildingEquipment/ConsoleIncludeSections.json";
        public static readonly string INCLUDESECTIONSTEMPLATE = "Templates/Unit/{0}/IncludeSections.json";
        public static readonly string CABINTERIORINCLUDESECTIONVALUES = "Template/UnitConfiguration/CABINTERIORINCLUDESECTIONVALUES.json";
        public static readonly string CABINTERIORINCLUDESECTIONVALUESENDURA100 = "Template/UnitConfiguration/Endura100/CABINTERIORINCLUDESECTIONVALUES.json";
        public static readonly string TRACTIONHOISTWAYEQUIPMENTINCLUDESECTIONVALUESENDURA100 = "Template/UnitConfiguration/Endura100/TRACTIONHOISTWAYEQUIPMENTINCLUDESECTIONVALUES.json";
        public static readonly string CARFIXTUREINCLUDESECTIONVALUES = "Template/UnitConfiguration/CARFIXTUREINCLUDESECTIONVALUES.json";
        public static readonly string CARFIXTUREINCLUDESECTIONVALUESENDURA100 = "Template/UnitConfiguration/Endura100/CARFIXTUREINCLUDESECTIONVALUES.json";
        public static readonly string UNITHALLFIXTURECONSOLEINLUDESECTIONVALUESENDURA100 = "Template/UnitConfiguration/Endura100/UnitHallFixtureConsoleIncludeSectionValues.json";
        public static readonly string GROUPINLUDESECTIONTEMPATE = "Templates/Group/IncludeSections.json";
        public static readonly string INTEGRATIONCONSTANTMAPPER = @"Templates\Integration\Constantmapper.json";
        public static readonly string OZ = "OZ";
        public static readonly string FIELDDRAWINGAUTOMATIONINLUDESECTIONVALUES = "Templates/FieldDrawingAutomation/IncludeSections.json";
        public static readonly string LIFTDESIGNERINLUDESECTIONVALUES = "Templates/FieldDrawingAutomation/LiftDesigner/IncludeSections.json";
        public static readonly string UNITCONSTANTMAPPERTEMPLATE = @"Templates/Unit/ConstantMapper.json";

        public static readonly string COPVALUES = "COPVALUES";
        public static readonly string VALUETOCOMPARTMENT = "ValueToCompartment";

        public static readonly string NCPINLUDESECTIONVALUES = @"Templates\Group\NCP\IncludeSections.json";
        public static readonly string CUSTOMENGINEERDINLUDESECTIONVALUES = @"Templates\Unit\CustomEngineered\IncludeSections.json";
        public static readonly string BUILDINGEQUIPMENTLOBBYPANELSUBSECTIONS = @"Templates\Building\BuildingEquipment\LobbyPanelUIResponse.json";
        public static readonly string BUILDINGEQUIPMENTPROPERTIESSTUB = @"Templates\Building\BuildingEquipment\Properties.json";
        public static readonly string CARPOSITION = "Template/GroupConfiguration/CarPosition.json";
        public static readonly string GROUPCONFIGURATIONVARIABLEMAPPER = "GroupConfigurationMapper";
        public static readonly string GROUPHALLFIXTURECONSOLEDEFAULTVALUES = @"Templates\Group\Elevator\HallFixtures\ConsoleDefaults.json";
        public static readonly string VARIABLEDICTIONARY = @"Templates\CrossPackage\VariableDictionary.json";
        public static readonly string PRODUCTSELECTIONCONSTANTTEMPLATEPATH = @"Templates\ProductSelection\ConstantMapper.json";


        public static readonly string CROSSPACKAGEVARIABLEMAPPING = "Templates/CrossPackage/VariableDictionary.json";


        public static readonly string LOCATIONTOELEVQATORMAPPING = "Template/CrossPackageVariables/CarPositionToElevator.json";
        public static readonly string XMLGENERATIONVARIABLEMAPPING = "Templates/FieldDrawingAutomation/LiftDesigner/XMLGenerationVariables.json";

        public const string BUILDINGCONFIGURATION = "BUILDINGCONFIGURATION";
        public static readonly string PRODUCT_SELECTION = "ProductSelection";
        public const string PRODUCTSELECTIONINVALIDEVO200 = "ProductSelectionInvalidEVO200";
        public const string READONLY = "readOnly";
        public const string READONLY_LOWER = "readonly";
        public static readonly string GROUPDESIGNATIONNAME = "GRPDESG";
        public static readonly string TRAVEL = "TRAVEL";
        public static readonly string TRAVELHYDRAULIC = "TRAVELHYDRAULIC";
        public static readonly string NEWINSTALLATION = "New Installation";
        public static readonly string NI = "NI";
        public static readonly string MD = "MD";
        public static readonly string US = "US";
        public static readonly string CA = "CA";
        public static readonly string CANADA = "CANADA";
        public static readonly string COUNTRYVALUE = "AccountDetails.SiteAddress.Country";
        public static readonly string UNITMFGJOBNO = "UnitMFGJobNo";
        public static readonly string FDAU1 = "B1P1";
        public static readonly string FDAU2 = "B1P2";
        public static readonly string FDAU3 = "B1P3";
        public static readonly string FDAU4 = "B1P4";
        public static readonly string FDAU5 = "B2P1";
        public static readonly string FDAU6 = "B2P2";
        public static readonly string FDAU7 = "B2P3";
        public static readonly string FDAU8 = "B2P4";
        public static readonly string UNITSTATUS = "UnitStatus";
        public const string ELEV = "ELEV";
        public static readonly string FDAELEVATOR007 = "ELEVATOR007";
        public static readonly string PROJECTSTATUS = "ProjectStatus";

        //GroupVariable mapper
        public static readonly string HALLSTATION = "hallStation";
        public static readonly string ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE = "Elevator_Communication_Failure_Jewel";
        public static readonly string FIREPHONEJACKCONSOLE = "Fire_Phone_Jack";
        public static readonly string REFRESHTYPE = "REFRESHTYPE";
        public static readonly string CUSTOMERACCOUNT = "CustomerAccount";
        public static readonly string GROUPTOBUILDINGCROSSPACKAGEVARIABLE = "GroupToBuilding";
        public static readonly string BUILDINGTOUNITCROSSPACKAGEVARIABLE = "BuildingToUnit";
        public static readonly string GROUPTOUNITCROSSPACKAGEVARIABLE = "GroupToUnit";
        public static readonly string FLOORNUMBER = "FloorNumber";
        #region ConfigurationBL related Constants
        public static readonly string STARTGROUPCONFIGURATIONSTUBVALIDATEDATAPATH = @"Template\GroupConfiguration\GroupConfigurationValidateData.json";
        public static readonly string CANADACAMELCASE = "Canada";
        public static readonly string NOTEQUALTOCANADA = "!=Canada";





        public static readonly string STARTGROUPLAYOUTCONFIGURATIONSTUBPATH = @"Template\GroupConfiguration\GroupLayoutConfigurationStub.json";
        
        public static readonly string GROUPLANDINGSLISTDATA = "Template/GroupConfiguration/GroupLandingsList.json";

        public static readonly string PRODUCTSELECTIONINVALIDEVO200STUBPATH = @"Template\GroupConfiguration\ProductSelectionInvalidEVO200Stub.json";
        public static readonly string PRODUCTSELECTIONUNITLEVELVALIDATIONREQUESTBODYPATHPATH = @"Template\ProductSelection\ProductSelectionUnitLevelvalidationrequestBody.json";
        public static readonly string PRODUCTSELECTIONGROUPLEVELVALIDATIONEVO200REQUESTBODYPATHPATH = @"Template\ProductSelection\ProductSelectionGroupLevelValidationEVO_200RequestBody.json";
        public static readonly string GENERALINFORMATIONTEMPLATE = @"Templates\Unit\{0}\GeneralInformationUIResponse.json";
        public static readonly string CABINTERIORTEMPLATENEW = @"Templates\Unit\{0}\CabInteriorNewUIResponse.json";


        public static readonly string ESCLATORSTUBPATH = @"Templates\Group\NCP\EsclatorUIResponse.json";

        public static readonly string TWINELEVATORSTUBPATH = @"Templates\Group\NCP\TwinElevatorUIResponse.json";

        public static readonly string OTHERSCREENSTUBPATH = @"Templates\Group\NCP\OtherScreenUIResponse.json";



        public static readonly string CUSTOMENGINEEREDGEARLESSSTUBPATH = @"Templates\Unit\CustomEngineered\Gearless\UIResponse.json";

        public static readonly string CUSTOMENGINEEREDGEAREDSTUBPATH = @"Templates\Unit\CustomEngineered\Geared\UIResponse.json";

        public static readonly string CUSTOMENGINEEREDHYDRAULICSTUBPATH = @"Templates\Unit\CustomEngineered\Hydraulic\UIResponse.json";

        public static readonly string SYNERGYSTUBPATH = @"Templates\Unit\CustomEngineered\Synergy\UIResponse.json";





        public static readonly string UNITDEFAULTSCLMCALLFILEPATH = @"Template\UnitConfiguration\unitdefaultsclm.json";
        
        public static readonly string GROUPHALLFIXTUREUITEMPLATE = @"Templates\Group\Elevator\HallFixtures\UIResponse.json";

        public static readonly string GROUPHALLFIXTURECONSOLEPATH = @"Templates\Group\Elevator\HallFixtures\Consoles.json";








        

        public static readonly string STARTFIELDDRAWINGAUTOMATIONMAINTEMPLATEPATH = @"Templates\FieldDrawingAutomation\UIResponse.json";


        public static readonly string UNITENRICHMENTSTEMPLATE = @"Templates\Unit\{0}\Enrichments.json";

        public static readonly string PROPERTIESTEMPLATE = @"Templates\Building\Properties.json";
        public static readonly string ENTRANCECONSOLESTUBPATH = @"Template\UnitConfiguration\EntranceConsoleStub.json";

        public static readonly string CARFIXTURETEMPLATE = @"Templates\Unit\{0}\CarFixturesUIResponse.json";

        public static readonly string PROPERTYTEMPLATE = @"Templates\Unit\{0}\HoistwayMinMaxProperties.json";

        public static readonly string CUSTOMPROPERTYTEMPLATE = @"Templates\Unit\Endura100\PitDepthCustomSelection.json";

        public static readonly string UNITCARFIXTUREMAPPERCONFIGURATION = "CarFixtureConstantMapper";


        public static readonly string CARFIXTURECOMPARTMENTPATH = @"Templates\Unit\{0}\CarfixtureCompartment.json";

       
        public static readonly string CARFIXTURESUBSECTIONSTUBPATHENDURA100 = @"Template\UnitConfiguration\Endura100\CarFixtureWithSectionsStubResponse.json";

        
        public static readonly string UNITHALLFIXTUREPATH = @"Templates\Unit\{0}\UnitHallFixtures\UIResponse.json";
        public static readonly string UNITHALLFIXTURECONSOLESPATH = @"Templates\Unit\{0}\UnitHallFixtures\Consoles.json";

        public static readonly string BUILDINGCONFIGURATIONREQESTBODYSTUBPATH = @"Templates\Building\CLMRequestBody.json";

        public static readonly string NCPCLMREQESTBODYSTUBPATH = @"Templates\Group\NCP\CLMRequest.json";


        public static readonly string CLMREQESTBODYSTUBPATH = @"Templates\FieldDrawingAutomation\LiftDesigner\CLMRequest.json";
        public static readonly string SYSTEMVALIDATIONBASEREQUEST = @"Templates\Unit\{0}\CLMRequest.json";

        public static readonly string LIFTDESIGNERXMLGENERATIONMAPPERSTUBPATH = @"Templates\FieldDrawingAutomation\LiftDesigner\ConstantMapper.json";
        public static readonly string UNITCONFIGURATIONREQESTBODYSTUBPATH = @"Template\UnitConfiguration\UnitConfigurationRequestBody.json";
        public static readonly string UNITCONFIGURATIONREQESTBODYSTUBPATHENDURA100 = @"Templates\Unit\Endura100\ClmRequest.json";
        public static readonly string UNITCONFIGURATIONREQESTBODYSTUBPATHFOREVO100 = @"Templates\Unit\Evolution100\ClmRequest.json";
        public static readonly string CUSTOMENGINEEREDREQESTBODYSTUBPATH = @"Templates\Unit\CustomEngineered\CLMRequest.json";
        public static readonly string UNITTUIRESPONSETEMPLATE = @"Templates\Unit\UIResponse.json";
        public static readonly string GROUPINFOUIRESPONSEPATH = @"Templates\Group\GroupInfo\GroupInfoUIResponse.json";
        public static readonly string SUMMARYRESPONSEOBJECT = @"Template\UnitConfiguration\SummaryResponseObject.json";
        public static readonly string SUMMARYINCLUDESECTIONVALUES = @"Template\UnitConfiguration\SummaryIncludeSection.json";
        public static readonly string BUILDINGEQUIPMENTUITEMPLATE = @"Templates\Building\BuildingEquipment\UIResponse.json";
        public static readonly string BUILDINGCONFIGURATIONTABSTUBPATH = @"Templates\Building\SectionTab.json";
        public static readonly string GROUPCONFIGURATIONTABSTUBPATH = @"Template\GroupConfiguration\GroupMainSectionTab.json";
        public static readonly string UNITENRICHEDDATA = @"Templates\Unit\Evolution200\Enrichments.json";
        public static readonly string UNITENRICHEDDATAENDURA100 = @"Template\UnitConfiguration\Endura100\UnitConfigurationEnrichedData.json";

        public static readonly string NCPENRICHEDDATA = @"Templates\Group\NCP\{0}\Enrichments.json";

        public static readonly string GROUPINFOENRICHEDDATA = @"Templates\Group\GroupInfo\Enrichments.json";

        public static readonly string CUSTOMENGINEEREDENRICHEDDATA = @"Templates\Unit\CustomEngineered\{0}\Enrichments.json";




        public static readonly string BUILDINGENRICHEDDATA = @"Templates\Building\Enrichments.json";

        public static readonly string FDAENRICHEDDATA = @"Templates\FieldDrawingAutomation\Enrichments.json";
        public const string ENRICHEDDATA = "enrichedData";







        public static readonly string RANGEJOBJECT = @"Template\UnitConfiguration\RangeIdValuesForUnitConfig.json";

        public static readonly string VARIABLESDATAVALUES = @"Templates\Integration\VariablesData.json";

        public const string SUMMARYTAB = "SUMMARYTAB";

        public static readonly string OZREQUESTBODY = @"Templates\Integration\OzRequestPayload.json";
        public const string INTERGROUPEMEGENCYPOWER = "Building_Configuration.Parameters_SP.interGroupEmergencyPower_SP";
        public const string BUILDINGEQUIPMENT = "BUILDINGEQUIPMENT";
        public const string FIELDDRAWINGAUTOMATION = "FIELDDRAWINGAUTOMATION";
        public const string LIFTDESIGNER = "LIFTDESIGNER";
        public const string GROUPCONFIGURATION = "GROUPCONFIGURATION";
        public const string GROUPLAYOUTCONFIGURATION = "GROUPLAYOUTCONFIGURATION";
        public const string UNITCONFIGURATION = "UNITCONFIGURATION";
        public const string OPENINGLOCATION = "OPENINGLOCATION";
        public const string PRODUCTSELECTION = "PRODUCTSELECTION";
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





        public const string UNITNAME = "unitConfiguration";
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

        public const string ERRORMSGFORRISERANDOPENING = "Please Select the required Riser Location and Opening Locations";
        public const string ERRORMSFFORRISERLOCATION = "Please Select the required Riser Location";
        public const string ERRORMSGFORGROUPOPENING = "Please Select the required Opening Locations";

        public const string SYSTEMVALIDATIONSLINGCALL = "SYSTEMVALIDATIONSLING";
        public const string SYSTEMVALIDATIONCABCALL = "SYSTEMVALIDATIONCAB";
        public const string SYSTEMVALIDATIONEMPTYCALL = "SYSTEMVALIDATIONEMPTY";
        public const string SYSTEMVALIDATIONDUTYCALL = "SYSTEMVALIDATIONDUTY";
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
        public const string SECTIONID = "SectionId";
        public const string SALESUIPARAMETER = "Sales_UI";
        public const string UNITHALLFIXTURELOWER = "unithallfixture";
        public const string UNITHALLFIXTURECONSOLES = "unithallfixtureconsoles";
        public const string ENTRANCESLOWERCASE = "entrances";
        public const string ITEMNUMBER= "ItemNumber";
        public const string ETA = "ETA";
        public const string ETD = "ETD";
        public const string ETA_AND_ETD = "ETA/ETD";
        public const string ZEROOPENINGS = "F - 0, R - 0";
        public const string ZEROFRONTOPENINGS = "F - 0";
        public const string TRADITIONALHALLSTATION = "Traditional_Hall_Stations";
        public const string FRONTHALLSTATION = "F_SP"; 
        public const string REARHALLSTATION = "R_SP";
        public const string COMPONENTNAME= "ComponentName";
        public const string AGILEHALLSTATION = "AGILE_Hall_Stations";
        public const string HALLCALLLOCKOUT = "Hall_Call_Lockout";
        public const string HALLCALLREGISTRATION = "Hall_Call_Registration";  
        public const string LANDINGS = "Landings"; 
        public const string DESCRIPTION="Description";
        public const string FIRESERVICE = "Fire_Service";
        public const string EMERGENCYPOWER = "Emergency_Power";
        public const string QUANTITY="Quantity";
        public const string UNITPRICE = "UnitPrice";
        public const string PRODUCTMODELSP = "PRODUCTMODELSP";
        public const string TKEFACTORYMODEL = "TKEFACTORYMODEL";
        public const string GENERALINFORMATIONMESSAGE = "The value should be between {0} ft {1} in and {2} ft {3} in";
        public const string WALLTHICKNESSRANGEMESSAGE = "The value should be between {0} in and {1} in";
        public const string WALLTHICKNESS = "WALLTHICKNESS";
        public const string ENTRANCE = "Entrances";
        public const string MINVALUE = "minValue";
        public const string MAXVALUE = "maxValue";
        public const string RANGEVALIDATION = "rangeValidationMessage";
        public static readonly string SINGLETONVALUE = "SingletonValue";
        public static readonly string INTERVALVALUE = "IntervalValue";
        public static readonly string MINVALUESMALLCASE = "minvalue";
        public static readonly string MAXVALUESMALLCASE = "maxvalue";
        public const string BUILDINGVARIABLESLIST = "buildingVariablesList";
        public const string GROUPVARIABLESLIST = "groupVariablesList";
        public const string UNITVARIABLESLIST = "unitVariablesList";
        public const string AVAILABLE = "Available";
        public const string SELECTED = "Selected";
        public const string TOPFLOORELEVATION = "TopFloorElevation";
        public const string BOTTOMFLOORELEVATION = "BottomFloorElevation";
        public const string MAINEGRESSELEVATIONS = "MainEgressElevation";
        public const string TOPLANDINGELEVATION = "TOPLANDINGELEVATION";
        public const string MAINEGRESSELEVATION = "MAINEGRESSELEVATION";
        public const string BUTTOMLANDINGELEVATION = "BUTTOMLANDINGELEVATION";
        public const string EGRESSFLAG = "EGRESSFLAG";
        public static readonly string LEADTIME = "LeadTime";

        public static readonly string PARAMETERS = "PARAMETERS";
        public static readonly string SPPARAMETERS = "SPPARAMETERS";
        public static readonly string GROUPVALIDATION = "Group_Validation";
        public static readonly string PARAMETERSVALUES = "Parameters";
        public static readonly string UNITDESIGNATION = "unitDesignation";
        public static readonly string PARAMETERS_SP = "Parameters_SP";
        public static readonly string PARAMETERSSPCONTROLROOM = "Parameters_SP.control";
        public const string ELEVATOR = "ELEVATOR";
        public static readonly string ELEVATOR001 = "ELEVATOR001.";

        public static readonly string SYSTEMVARIABLEKEYS = "SystemVariableKeys";
        public static readonly string SYSTEMVARIABLEVALUES = "SystemVariableValues";

        public static readonly string ELEVATOR_DOT = "ELEVATOR.";


        public static readonly string ELEVATOR1 = "ELEVATOR001";
        public static readonly string ELEVATOR2 = "ELEVATOR002";
        public static readonly string ELEVATOR3 = "ELEVATOR003";
        public static readonly string ELEVATOR4 = "ELEVATOR004";
        public static readonly string ELEVATOR5 = "ELEVATOR005";
        public static readonly string ELEVATOR6 = "ELEVATOR006";
        public static readonly string ELEVATOR7 = "ELEVATOR007";
        public static readonly string ELEVATOR8 = "ELEVATOR008";

        public static readonly string MINIMUM = "Minimum";
        public static readonly string MAXIMUM = "Maximum";
        public static readonly string CUSTOM = "Custom";
        public static readonly string HOISTWAYDIMENSIONS = "hoistwayDimensionSelection";
        public static readonly string HOISTWAYDIMENSIONVALUE = "HOISTWAYDIMENSIONS";
        public static readonly string RANGEVALUES = "RangeValues";
        public const string ISRANGEINPUTTYPE = "isRangeInputType";
        public const string EVO_200 = "EVO_200";
        public const string EVO_100 = "EVO_100";
        public static readonly string CONTROLLOCATIONVALUE = "CONTROLOC";
        public static readonly string CONTROLLOCATIONVALUECOLUMNNAME = "ControlLocationValue";
        public static readonly string DISPLAYVARIABLEASSIGNMENTSFORGROUP = "DISPLAYVARIABLEASSIGNMENTSFORGROUP";



        public const string CEGEARLESS = "CE_Gearless";
        public const string CEGEARED = "CE_Geared";
        public const string CEHYDRAULIC = "CE_Hydraulic";
        public const string SYNERGY = "Synergy";


        public static readonly string EVOLUTION_200 = "EVOLUTION 200";

        public static readonly string CONTROLFLOOR = "CONTROLFLOOR";

        public static readonly string PARAMETERSSPCONTROLFLOOR = "PARAMETERSSPCONTROLFLOOR";
        public static readonly string CONTROLLOCATIONID = "controlLocation";
        public static readonly string CONTROLLOCATION = "CONTROLLOCATION";
        public static readonly string TOPF = "TOPF";
        public static readonly string TOPR = "TOPR";

        public static readonly string CONTROLLERLOCATION_SP = "CONTROLLERLOCATION_SP";
        public static readonly string CONTROLLOCATIONREMOTE = "Remote";
        public static readonly string CONTROLLOCATIONOVERHEAD = "Overhead";
        public static readonly string CONTROLLOCATIONJAMBMOUNTED = "Jamb-Mounted";
        public static readonly string PARMETERSXDIMENSIONVALUES = "PARMETERSXDIMENSIONVALUES";
        public static readonly string PARAMETERSYDIMENSIONVALUES = "PARMETERSYDIMENSIONVALUES";
        public static readonly string PARAMETERSNXVMDISTANCEFLOOR = "PARAMETERSNXVMDISTANCEFLOOR";
        public static readonly string CONTROLLOCATIONADJACENT = "Adjacent";
        public static readonly string CONTROLROOMQUADSPVALUES = "CONTROLROOMQUADSPVALUES";

        public static readonly string PARAMETERSCARPOS = "CARPOS";

        public static readonly string PARAMETERSREARDOOR = ".Parameters_SP.rearDoorTypeAndHand_SP";
        public static readonly string FRONTREARDOORTYPEANDHAND_SP = "DoorTypeAndHand_SP";
        public static readonly string FLOORPLANDISTANCEPARAMETERS = "floorPlanDistanceParameters";
        public static readonly string PARAMETERSHALLRISER = ".Layout.HS2QUAD";
        public static readonly string PARAMETERS_LAYOUT_BANKTYPE = "Parameters.B[1-9]P[1-9]";
        public static readonly string UNITTABLEVALUES = "UNITTABLEVALUES";
        public static readonly string POSITIONVALUES = "positionvalues";
        public static readonly string TRUEVALUES = "TRUE";
        public static readonly string ELEVATORTOPR = "ELEVATORTOPR";
        public static readonly string ELEVATORTOPF = "ELEVATORTOPF";
        public static readonly string ELEVATION = "ELEVATION";
        public static readonly string ENTF = "ENTF";
        public static readonly string ENTR = "ENTR";
        public static readonly string True = "True";
        public static readonly string False = "False";
        public static readonly string FALSEVALUES = "FALSE";
        public static readonly string PARAMETERS_LAYOUT_B = "Parameters.B";
        public static readonly string ELEVATORSVALUE = "ELEVATOR00";
        public static readonly string TOTALBUILDINGFLOORTOFLOORHEIGHT = "TOTALBUILDINGFLOORTOFLOORHEIGHT";
        public static readonly string FLOORTOFLOORHEIGHT = "Floor to Floor Height";
        public static readonly string ELEVATION_1 = "Elevation - 1";
        public static readonly string BUILDINGRISE = "buildingRise";
        public static readonly string BYRULE_CAMELCASE = "byRule";
        public static readonly string BYDEFAULT_CAMELCASE = "byDefault";
        public static readonly string AGILEBUILDINGFEATURES = "Building_Configuration.Parameters.AGILE_Building_Features";

        public static readonly string ELEVATOR_PARAMETERS_HALL_PI = "ELEVATOR.Parameters.Hall_PI";
        public static readonly string ELEVATOR_Parameters_Combo_Hall_Lantern_PI = "ELEVATOR.Parameters.Combo_Hall_Lantern/PI";
        public static readonly string ELEVATOR_Parameters_Hall_Lantern = "ELEVATOR.Parameters.Hall_Lantern";
        public static readonly string ELEVATOR_Parameters_Hall_Elevator_Designation_Plate = "ELEVATOR.Parameters.Hall_Elevator_Designation_Plate";
        public static readonly string ELEVATOR_Parameters_Hall_Target_Indicator = "ELEVATOR.Parameters.Hall_Target_Indicator";
        public static readonly string ELEVATOR_Braille = "ELEVATOR.Parameters.Braille";
        public static readonly string ELEVATOR_Elevator_and_Floor_Designation_Braille = "ELEVATOR.Parameters.Elevator_&_Floor_Designation_Braille";

        public static readonly string USERINTERFACEPICOLOR = "ELEVATOR.Sales_UI_Screens.Unit_HallFixtures_SP.Parameters.User_Interface_Devices.Landing_Indicator_Panel_LIP.PICOLOR";
        public static readonly string USERINTERFACEHALLFIN = "ELEVATOR.Sales_UI_Screens.Unit_HallFixtures_SP.Parameters_SP.HALLFINMAT_SP";
        public static readonly string BRAILLEPARAMETER = "ELEVATOR.Sales_UI_Screens.Unit_HallFixtures_SP.Parameters_SP.dDBraille_SP";
        public static readonly string PRICEKEYPARAMETER = "ELEVATOR.PriceKey";
        public static readonly string PRICEKEY = "PriceKey";
        public static readonly string SMARTRESCUEPHONE5SPPARAM = "Building_Configuration.Parameters_SP.qtyOfSmartRescuePhone5_StndAlone_SP";
        public static readonly string SMARTRESCUEPHONE10SPPARAM = "Building_Configuration.Parameters_SP.qtyOfSmartRescuePhone10_StndAlone_SP";
        public static readonly string DOOR = "Door";
        public static readonly string QUAD = "QUAD";
        public static readonly string CIB = "CIB";
        public static readonly string FLOORTOFLOORHEIGHTFEET = "FloorToFloorHeightFeet";
        public static readonly string FLOORTOFLOORHEIGHTINCH = "FloorToFloorHeightInch";
        public static readonly string CURRENTGROUPCONFIGURATION = "CURRENTGROUPCONFIGURATION";
        public static readonly string BASEGROUPCONFIGURATION = "BASEGROUPCONFIGURATION";
        public static readonly string PREVIOUSGROUPCONFIGURATION = "PREVIOUSGROUPCONFIGURATION";
        public static readonly string  ELEVATIONINCH="ElevationInch";
        public static readonly string ELEVAIONFEET = "ElevationFeet";
        public static readonly string CURRENTBUILDINGCONFIGURATION = "CURRENTBUILDINGCONFIGURATION";
        public static readonly string BASEBUILDINGCONFIGURATION = "BASEBUILDINGCONFIGURATION";
        public static readonly string PREVIOUSBUILDINGCONFIGURATION = "PREVIOUSBUILDINGCONFIGURATION";
        public static readonly string EDITCONFLITFLOWCACHEKEY = "EDITCONFLITFLOWCACHEKEY";
        public static readonly string PREVIOUSCONFLICTSVALUES = "PERVIOUSCONFLICTSVALUES";
        public static readonly string PREVIOUSGROUPCONFLICTSVALUES = "PERVIOUSGROUPCONFLICTSVALUES";
        public static readonly string PREVIOUSUNITCONFLICTSVALUES = "PERVIOUSUNITCONFLICTSVALUES";

        public static readonly string ENTRANCE_CONFIGURATION = "Entrance Configuration";

        public static readonly string TRAVELVARIABLEIDVALUE = "TRAVELVARIABLEIDVALUE";
        public static readonly string GROUPCONFIGURATIONID = "GroupConfigurationId";

        public static readonly string GETENRICHMENTVALUESDATA = "GETENRICHMENTVALUESDATA";
        public static readonly string NEWPROJECTIDFLAG = "0";
        public static readonly string VARIABLEDETAILS = "variableDetails";
        public static readonly string PROJECTDISPLAYDETAILS = "projectDisplayDetails";

        public static readonly string ISEDITFLOWFLAGCHECK = "ISEDITFLOWFLAGCHECK";
        public static readonly string SYSTEMVALIDATIONPARAMETERS = @"Templates\Unit\{0}\SystemValidation\Parameters.json";

        public static readonly string PREVIOUSUNITCONFLICTSVALUESFORVALIDATION = "PREVIOUSUNITCONFLICTSVALUESFORVALIDATION";

        public static readonly string SYSTEMVALIDATIONCABREQUESTBODYVARIABLES = @"Templates\Unit\{0}\SystemValidation\CabCallRequiredVariables.json";
        public static readonly string SYSTEMVALIDTAIONEMPTYREQUESTBODYVARIABLES = @"Templates\Unit\{0}\SystemValidation\EmptyCallRequiredVariables.json";
        public static readonly string SYSTEMVALIDATIONDUTYREQUESTBODYVARIABLES = @"Templates\Unit\{0}\SystemValidation\DutyCallRequiredVariables.json";

        public static readonly string SYSVALVARIABLEASSIGNMENTS = "variableAssignments";

        public static readonly string ACROSSTHEHALLDISTANCEPARAMETER = "ACROSSTHEHALLDISTANCEPARAMETER";
        public static readonly string BANKOFFSETPARAMETER = "BANKOFFSETPARAMETER";
        public static readonly string SYSVALUNIT_VAL = "UNIT_VAL";
        public static readonly string SYSVALUNIT_INV = "UNIT_INV";
        public static readonly string SYSVALDESCRIPTION = "Description";
        public static readonly string EMPTYCALLMATERIALNAME = "R100174108";
        public static readonly string DUTYCALLMATERIALNAME = "R100205358";
        public static readonly string PERMISSIONTYPEBUILDING = "Building";
        public static readonly string PERMISSIONTYPEUNIT = "Unit";
        public static readonly string PARAMETERS_SPCONTROLLERLOCATION_SP = "ELEVATOR.Parameters_SP.controllerLocation_SP";
        public static readonly string PARAMETERSCONTROLLERLOCATION_SP = "Parameters_SP.controllerLocation_SP";
        public static readonly string HALLFINPARAM = "ELEVATOR.HALLFINMAT_SP";
        public static readonly string HALLFINVARIABLEID = "ELEVATOR.Parameters_SP.HALLFINMAT_SP";
        public static readonly string GROUPLAYOUTSIZE = "GROUPLAYOUTSIZE";
        public static readonly string CONTROLOCVARIABLE = "ELEVATOR.Parameters.CONTROLOC";
        public static readonly string CONTROLOC = "CONTROLOC";
        public static readonly string CONTROLHERE = "CONTROLHERE";

        public static readonly string STARTBUILDINGCONFIGURATIONSTUBPATH = @"Templates\Building\UIResponse.json";
        public static readonly string BUILDINGMAPPERVARIABLESMAPPERPATH = @"Templates\Building\ConstantMapper.json";
        public static readonly string GROUPMAPPERVARIABLES = @"Templates\Group\Elevator\ConstantMapper.json";
        public static readonly string UNITSVARIABLESMAPPERPATH = @"Templates\Unit\ConstantMapper.json";
        public static readonly string BUILDINGEQUIPMENTMAPPERVARIABLESMAPPERPATH = @"Templates\Building\BuildingEquipment\BuildingEquipmentConstantMapper.json";
        public static readonly string FDAMAPPERVARIABLESMAPPERPATH = @"Templates\FieldDrawingAutomation\ConstantMapper.json";
        public static readonly string UNITHALLFIXTURECONSTANTMAPPER = "UnitHallFixtureConstantMapper";
        public static readonly string BUILDINGMAPPERVARIABLES = @"Templates\Building\BuildingConstantMapper.json";
        public static readonly string SUMMARYLANDINGINCLUDESECTIONVALUES = "Tp2SummaryLandingIncludeSections";

        public static readonly string CUSTOMENGINEEREDCONSTANTMAPPERPATH = @"Templates\Unit\CustomEngineered\ConstantMapper.json";



        public static readonly string BUILDINGCODEIBCCBCVALUES1 = @"Template\StartCnfgn\BuildingCodeIbcCbc.json";
        public static readonly string BUILDINGCODENBCCVALUES1 = @"Template\StartCnfgn\BuildingCodeNbcc.json";
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
        
        //public static readonly string UNITHALLFIXTUREMOCKRESPONSEFORTESTING = @"Templates\Unit\Evolution200\UnitHallFixtures\UnitHallFixtureMockResponseForTesting.json";
        public static readonly string DEFAULTBUILDINGCONFIGVALUES = "DEFAULTBUILDINGCONFIGVALUES";
        public static readonly string DEFAULTBUILDINGEQUIPMENTCONFIGVALUES = "DEFAULTBUILDINGEQUIPMENTCONFIGVALUES";
        public static readonly string DEFAULTGROUPCONFIGVALUES = "DEFAULTGROUPCONFIGVALUES";
        public static readonly string DEFAULTUNITCONFIGVALUES = "DEFAULTUNITCONFIGVALUES";
        public static readonly string DEFAULTPRODUCTCONFIGVALUES = "DEFAULTPRODUCTCONFIGVALUES";
        public static readonly string BUILDINGCONFIGURATIONID = "Building_Configuration";
        public static readonly string CONFLICTVARIABLEIDDATA = "VariableId";


        public static readonly string CONFLICTVARIABLEID = "conflictVariableId";
        public static readonly string CONFLICTVARIABLEVALUES = "conflictVariableValues";
        public static readonly string LISTOFVARIABLEASSIGNMENT = "@listofVariableAssignment";
        public static readonly string LISTOFCONFIGURATION = "LISTOFCONFIGURATION";
        #endregion

        #region Log related constants

        public static readonly string STARTBUILDINGCONFIGVTPACKAGERESPONSE = "Start Building vt package response";
        public static readonly string GETBUILDINGCONFIGDLINITIATE = " ChangeConfigureBl BL call Initiated";
        public static readonly string STARTBUILDINGFILTEREDCONFIGRESPONSE = " StartBuildingConfigure Filtered response";
        public static readonly string NUMBEROFFLOORSUNITPACKAGEVARIABLE = "ELEVATOR.Parameters.Basic_Info.BLANDINGS";
        public static readonly string AUTOSAVECONFIGURATIONDL = " AutoSaveConfiguration DL call Initiated";
        public static readonly string DELETEAUTOSAVECONFIGURATIONBYUSERDL = " DeleteAutoSaveConfigurationByUser DL call Initiated";
        public static readonly string GETAUTOSAVECONFIGURATIONBYUSERDL = " GetAutoSaveConfigurationByUser DL call Initiated";
        public static readonly string SAVEFIELDDRAWINGAUTOMATIONBYGROUPIDINITIATEDDL = "SaveFieldDrawingAutomationByGroupId DL call Initiated";
        public static readonly string SAVEFIELDDRAWINGAUTOMATIONBYGROUPIDCOMPLETEDL = "SaveFieldDrawingAutomationByGroupId DL call Completed";
        public static readonly string SAVEUPDATERELEASEINFOINITIATEDL = " SaveUpdateReleaseInfo DL call Initiated";

        public const string VARIABLES = "variables";
        public const string VARIABLESCAPS = "Variables";
        public const string OPTIONS = "options";
        public static readonly string SECTIONTYPE = "sectionType";
        public static readonly string STRING = "String";
        public static readonly string GETBUILDINGEQUIPMENTFORPROJECTINITIATEDL = " GetBuildingEquipmentTab DL call Initiated";
        public static readonly string FATALERROR = "Fatal error occured";
        public const string FLOORDESIGNATION = "FLOORDESIGNATION";


        #endregion

        #region WrapperAPI

        #endregion

        #region OzAPI

        public static readonly string PROJECTINFORMATION = "ProjectInformation";
        public static readonly string CSC = "CSC";
        


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
        public static readonly string BRANCH = "Branch";
        public static readonly string SLSMNACTVDRCTRYID = "SalesmanActiveDirectoryID";
        public static readonly string SALESMAN = "Salesman";
        public static readonly string CATGRY = "Category";
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
        public static readonly string PROPOSEDDATEFORMAT = "ProposedDateFormat";
        public static readonly string PROPOSEDDATE = "ProposedDate";
        public static readonly string CONTRACTBOOKEDDATEFORMAT = "ContractBookedDateFormat";
        public static readonly string CONTRACTBOOKEDDATE = "ContractBookedDate";
        public static readonly string RETMSG = "retMsg";
        public static readonly string COMMONNAME_SP = "CommonName_SP";
        public const string ENDURA_100 = "ENDURA_100";
        public static readonly string ENDURA100 = "endura_100";
        public static readonly string ENDURA200 = "endura_200";
        public static readonly string SCUSER = "SC-";
        public static readonly string NOCONFIGURATIONDATAAVAILABLE = "No configuration data available";
        public static readonly string NOCONFIGURATIONDATAAVAILABLEDESCRIPTION = "please check configuration details are available before proceeding further";
        public static readonly string RETMSGVALUES = "retMsg";
        public static readonly string CODEVALUES = "code";
        public static readonly string COMMONNAME = "Product_Selection.Common_Name";
        public static readonly string PRODUCTTREEREAROPENVARIABLE = "Product_Selection.Parameters.Basic_Info.REAROPEN"; 
        public static readonly string INCLINED = "INCLINED";
        public static readonly string PRODUCTTECHNOLOGY = "PRODUCTTECHNOLOGY"; 
        public static readonly string PROJECTS_PROJECTID = "projects.ProjectId";
        public static readonly string PROJECTS_PROJECTNAME = "projects.ProjectName";
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
        public static readonly string PRODUCTTECHNOLOGYA = "PRODUCTTECHNOLOGYA";
        #endregion

        #region ProductSelection

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
        public static readonly string GROUPDESGINATIONSTUBRESPONSEPATH = @"Templates\Group\Elevator\GroupDesignationStub.json";
        public static readonly string ELEVATORCONFIGURATIONS = @"Templates\Group\Elevator\ElevatorConfigurations.json";
        public static readonly string ELEVATORLISTDATA = @"Templates\Group\Elevator\IncludeSections.json";
        public static readonly string DOORSUITEMPLATE = @"Templates\Group\Elevator\DoorsUIResponse.json";
        public static readonly string RISERLOCATIONSUITEMPLATE = @"Templates\Group\Elevator\HallRisersUIResponse.json";
        public static readonly string HALLSTATIONTODOORSMAPPING = @"Templates\Group\Elevator\HallStationToDoorsMapping.json";
        public static readonly string GROUPPOPUPENRICHMENTTEMPLATE = @"Templates\Group\Enrichments.json";
        public static readonly string GROUPHALLFIXTUREDEFAULTVALUES = @"Templates\Group\Elevator\HallFixtures\ConsoleDefaults.json";
        #endregion
        #region Products
        public static readonly string EVOLUTION200 = "Evolution200";
        public static readonly string END100 = "Endura100";
        public static readonly string EVOLUTION100 = "Evolution100";
        #endregion

        public static readonly string ETAMAPPER = "ETACARFIXTURE";

        #region CabInterior
        public static readonly string CABINTERIORTEMPLATE = @"Templates\Unit\{0}\CabInteriorUIResponse.json";
        #endregion

        #region UnitTemplates

        public static readonly string OTHEREQUIPMENTUIRESPONSETEMPLATE = @"Templates\Unit\{0}\OtherEquipmentUIResponse.json";
        public static readonly string ENTRANCESUIRESPONSETEMPLATE = @"Templates\Unit\{0}\Entrances\UIResponse.json";
        public static readonly string ENTRANCESCONSOLEUIRESPONSETEMPLATE = @"Templates\Unit\{0}\Entrances\ConsoleUIResponse.json";
        public static readonly string UNITHALLFIXTURECONSOLEDEFAULTVALUES = @"Templates\Unit\{0}\UnitHallFixtures\ConsoleDefaults.json";
        public static readonly string ENTRANCESCONSOLEDEFAULTVALUES = @"Templates\Unit\{0}\Entrances\ConsoleDefaults.json";
        public static readonly string PRICEDETAILS = @"Templates\Unit\Evolution200\TP2Summary\priceDetails.json";
        public static readonly string VARIABLENAMESFORPRICING = @"Templates\Unit\Evolution200\TP2Summary\PricingCrossVariables.json";
        public static readonly string UNITCOMMONMAPPER = "UnitCommonMapper";
        public static readonly string CABINTERIORMAPPER = "CabInterior";
        public static readonly string OTHEREQUIPMENT = "OtherEquipment";
        public static readonly string GENERALINFOMAPPER = "GeneralInformation";

        #endregion

        #region PROJECTS

        public static readonly string PROJECTCONSTANTMAPPER = @"Templates\Integration\ConstantMapper.json";
        public static readonly string PRODUCTTREESTUBPATH = @"Templates\Integration\ProductTreeCLMRequest.json";
        public static readonly string PROJECTSENRICHEDDATA = @"Templates\Integration\Enrichments.json";
        public static readonly string VIEWEXPORTVARIABLELIST = @"Templates\Integration\View\ExportVariables.json";
        public static readonly string VIEWVARIABLEMAPPING = @"Templates\Integration\View\DictionaryVariables.json";
        public static readonly string PROJECTCOMMONNAME = "commonName";
        public static readonly string PRODUCTUNAVAILABLE = "PRODUCTUNAVAILABLE";
        public static readonly string PRODUCTTREEVARIABLEMAPPER = "productTreemapper";


        #endregion

        #region DOCCUMENTGENERATION
        public static readonly string DOCUMENTGENERATIONMAPPERPATH = @"Templates\DocumentGeneration\ConstantMapper.json";
        public static readonly string ORDERFORM = "OrderForm";
        public static readonly string ELEVATIONATBUILDINGBASE = "Elevation At BuildingBase";
        public static readonly string AVEREAGEROOFHEIGHT = "Average Roof Height";
        public static readonly string FRONTDOORWIDTH = "Front Door Width";
        public static readonly string REARDOORWIDTH = "Rear Door Width";
        public static readonly string FRONTDOORHEIGHT = "Front Door Height";
        public static readonly string REARDOORHEIGHT = "Rear Door Height";

        #endregion

        /// <summary>
        /// Exception Class Related
        /// All Exception Constants which are using in Model Project We are using this Constants
        /// </summary>
        public static readonly string CONFIGSERVICEEXCEPTION = "Something went wrong with the Configurator services :(";
        public static readonly string LOGBEGINSTRINGFORMAT = "Method '{0}' started:";
        public static readonly string LOGENDSTRINGFORMAT = "Method '{0}' execution ended: Time elapsed {1} milliseconds";

        public static readonly string DEFAULTSTRING = "";
        #region HTTPCLIENTMODEL


        public enum TOKENTYPE
        {
            Basic,
            Bearer
        }

        public static readonly string CONTENTTYPE = "application/json";
        public static readonly string CONTENTTYPEFORMDATA = "multipart/form-data";
        public const string POST = "POST";
        public const string GET = "GET";
        public const string PUT = "PUT";
        public const string PATCH = "PATCH";
        public static readonly string CONTENTTYPEFORMURI = "application/x-www-form-urlencoded";
        #endregion

        public static readonly string RISEPARAMETER = "Parameters.Rise";
        public const string NOOFFLOORSSERVED = "NO.OF FLOORS SERVED";
        public static readonly string FLOORSSERVED = "FloorServed";
        public static readonly string NONCONTROLLERCONSOLE = "NonControllerConsole";

        public static readonly string CESCREENLIST = "CEScreenList";
        public static readonly string BUILDINGEQUIPMENTSTATUS = "BuildingEquipmentStatus";
        public static readonly string BUILDINGEQUIPMENTSTATUSKEY = "BUILDINGEQUIPMENTSTATUS";
        public static readonly string CONFIGURATIONCONFLICTS = "configurationConflicts";
        public static readonly string CONFIGURATIONCONFLICTEXIST = "configurationConflictExist";
        public static readonly string BUILDINGSTATUS = "buildingStatus";
        public static readonly string BUILDINGEQUIPMENTCOMPLETESTATUSKEY = "BLDGEQP_COM";
        public static readonly string EMERGENCYPOWERSWITCHLOBBY = "EMERGENCYPOWERSWITCHLOBBY";


        #region Product Selection
        public static readonly string PRODUCTSELECTIONCONSTANTPATH = @"Templates\ProductSelection\ConstantMapper.json";
        public const string TotalNoOpenings = "TotalOpenings";
        public const string NUMBEROFFRONTOPENINGS = "NumberFrontOpenings";
        public const string NUMBEROFREAROPENINGS = "NumberRearOpenings";
        public const string TOTALOPENING = "TotalOpening";
        public static readonly string OCCUPIEDSPACEBELOWCOLUMN = "OcuppiedSpaceBelow";
        public const string VARIABLESKEY = "Variables";
        public const string TRAVELKEY = "Travel";
        public const string TOTTRAVELKEY = "TOTTravel";
        public static readonly string COUNTERWEIGHTSAFETYKEY = "CounterWeightSaftey";
        #endregion

        public static readonly string SETUNITSDATA = "SETUNITSDATA";
        public static readonly string TOTALCUSTOMPRICE = "TOTALCUSTOMPRICE";

        #region OBOM

        public static readonly string OPPORTUNITY = "OpportunityId";
        public static readonly string VERSIONIDFORCRM = "VersionId";
        public static readonly string CREATEDBYCRM = "CreatedBy";
        public static readonly string CREATEDONCRM = "CreatedOn";
        public static readonly string MODIFIEDONCRM = "ModifiedOn";
        public const string CREATEPROJECTCOLUMNCUSTOMERNUMBER = "CustomerNumber";
        public const string CREATEPROJECTCOLUMNACCOUNTNAME = "AccountName";
        public const string CREATEPROJECTCOLUMNAWARDCLOSEDATE = "AwardCloseDate";
        public const string PROJECTSTATUSCOLUMNNAME = "ProjectStatus";
        public const string CREATEPROJECTCOLUMNADDRESSLINE1 = "AddressLine1";
        public const string CREATEPROJECTCOLUMNADDRESSLINE2 = "AddressLine2";
        public const string CREATEPROJECTCOLUMNCITY = "City";
        public const string CREATEPROJECTCOLUMNSTATE = "State";
        public const string CREATEPROJECTCOLUMNCOUNTRY = "Country";
        public const string CREATEPROJECTCOLUMNZIPCODE = "ZipCode";
        public static readonly string STATUSNAME = "StatusName";
        public const string PROJECTQUOTESTATUSKEY = "QuoteStatusKey";
        public const string  UNITHALLFIXTURECONSOLEID= "UnitHallFixtureConsoleId";
        #endregion

        public static readonly string EDITFLAGFORGROUP = "EDITFLAGFORGROUP";
        public static readonly string BUILDINCITY="BUILDINCITY";
        public const string BUILDINGELEVATIONISEDITFLAG = "BUILDINGELEVATIONISEDITFLAG";

        public const string UHFEXISTSFLAG = "UHFEXISTSFLAG";
        public const string SAVEOPENINGLOCATIONSFLAG = "saveOpeningLocation";

        public const string CARRIDINGLANTERNQUANT = "CARRIDINGLANTERNQUANT";
        public const string  CARRIDINGQUANTITYVALUE = "CarRidingLanternQuantity";
        public static readonly string GROUPINGID_LOWERCASE = "@groupId";
        public static readonly string CONFLICTSTATUSFLAG = "@conflictStatusFlag";
        public static readonly string RESULT_LOWCASE = "@result";
        public static readonly string STATUS = "status";
        public static readonly string STATUSCOLUMNNAME = "Status";
        public static readonly string GUID = "guid";
        public const string TOTALUNITSINPANEL = "TOTALUNITSINPANEL";
        public static readonly string UNITMAPPER = "UnitCommonMapper";

        #region Entities
        public const string UNIT_ENTITY = "Unit";
        public const string DISCOUNT_ENTITY = "DISCOUNT";
        #endregion
    }
}
