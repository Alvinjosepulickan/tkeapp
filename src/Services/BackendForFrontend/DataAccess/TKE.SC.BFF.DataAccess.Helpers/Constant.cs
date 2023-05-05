using Microsoft.AspNetCore.Http;

namespace TKE.SC.BFF.DataAccess.Helpers
{
    public class Constant
    {
        public static readonly string PARAMSETTINGS = "ParamSettings";
        public static readonly string ENVIRONMENT = "Environment";
        public static readonly string SESSIONID = "SessionId";
        public static readonly string ISPRODUCTIONCHECK = "ISPRODUCTIONCHECK";
        public static readonly string PRODUCTIONCHECKSETTINGS = "ProductionCheckSettings";
        public static readonly string CONFIGURATORURL = "ConfiguratorServiceUrl";
        public static readonly string CONFIGURATORSERVICESYSTEMVALIDATIONURL = "ConfiguratorServiceSystemValidationUrl";
        public static readonly string PROCESSSTARTED = "Process Started";
        public static readonly string PROCESSCOMPLETED = "process completed";
        public static readonly string SESSIONSTARTED = "Session Started";
        public static readonly string SESSIONCOMPLETED = "Session Completed";
        public static readonly string VIEWURL = "ViewUrl";
        public static readonly string VIEW = "VIEW";
        public static readonly string SC = "SC";
        public static readonly string DEV = "DEV";

        #region QueryString
        public static readonly string GRANTTYPE = "grant_type";
        public static readonly string GRANTTYPEVALUE = "client_credentials";
        public static readonly string CLIENTID = "client_id";
        public static readonly string CLIENTSECRET = "client_secret";
        public static readonly string SETCOOKIE = "Set-Cookie";
        public static readonly string JSESSIONID = "JSESSIONID";
        public const string FDACURRENT = "Current";
        public const string FDACOMPLETED = "Completed";
        public const string DWGPENDING = "DWG_PEN";
        public const string DWGCOMPLETED = "DWG_CMP";

        #endregion

        #region Configurator Service related Constants

        public static readonly string PACKAGEPATH = "packagePath";

        public const string CONFIGURE = "configurator/v1/configure";
        public const string PRICE = "configurator/v1/price";
        #endregion

        #region Special characters related Constants

        public static readonly string COLON = ":";
        public static readonly string COMA = ",";
        public static readonly string OPENINGSQUAREBRACKET = "[";
        public static readonly string CLOSINGSQUAREBRACKET = "]";
        public static readonly string SLASH = "/";
        public static readonly string EQUALTO = "=";
        public static readonly char EQUAL = '=';
        public static readonly char AMPERSAND = '&';
        public static readonly string EMPTYSTRING = "";
        public static readonly string UNDERSCORE = "_";
        public static readonly char UNDERSCORECHAR = '_';
        public static readonly char PERCENTAGESYMBOL = '%';
        public static readonly char HYPHEN = '-';
        public static readonly char SLASHR = '\r';
        public static readonly string ZERO = "0";
        public static readonly string ZEROZERO = "00";
        public static readonly string DOT = ".";
        public static readonly string CLOSINGSQUAREBRACKETWITHCOMA = ",]";
        public static readonly string EMPTYSPACE = " ";
        public static readonly string G1 = "G1";
        public static readonly string RANDOMTIME = "7:06pm";
        public static readonly string RANDOMDATE = "09 / 03 / 2021";
        public static readonly string GROUPIDNOTFOUND = "group id not found";
        public static readonly string GROUPIDDELETED = "group id deleted";
        public static readonly string OPENINGBRACKET = "(";
        public static readonly string CLOSINGINGBRACKET = ")";
        public static readonly string APOSTROPHE = "'";
        #endregion

        #region HttpRequest related constants

        public static readonly string CONTENTTYPE = "application/json";
        public static readonly string CONTENTTYPEFORMDATA = "multipart/form-data";
        public const string POST = "POST";
        public const string GET = "GET";
        public const string PUT = "PUT";
        public const string PATCH = "PATCH";

        public static readonly string ACCESSTOKENTYPE = "Basic";
        public static readonly string TYPEMULTIFORMCONTENT = "System.Net.Http.MultipartFormDataContent";


        #endregion

        #region ConfiguratorDL stubdata related Constants    
             
        public static readonly string JSONFIELDS = @"Templates\Integration\View\QuoteRequestPayload.json";        

        #endregion

        #region Parameters I/O related Constants

        public static readonly string USERS = "Users";
        public static readonly string ID = "Id";
        public static readonly string PROJECTSOURCE = "ProjectSource";
        public static readonly string SOURCE = "Source";
        public static readonly string OUTPUTTYPES = "outputTypes";
        public static readonly string DRAWINGTYPES = "drawingTypes";
        public static readonly string STATUS = "Status";
        public static readonly string OPPID = "opportunityid";
        public static readonly string USD = "USD";
        public static readonly string @FDAPARAMSTATUSID = "@StatusId";

        public static readonly string STATUSKEY = "StatusKey";

        public static readonly string QUOTEID_CAMELCASE = "quoteId";

       public const string TOKENSECTIONS = "sections";
        public const string TOKENID = "id";
        public const string LAYOUTGENERATIONSETTINGSSECTIONKEY = "fda.layoutGenerationSettings";
        public const string FLOORTOFLOORHEIGHTINCH="FloorToFloorHeightInch";
        public const string FLOORTOFLOORHEIGHTFEET = "FloorToFloorHeightFeet";








        public static readonly string GETTOKENURL = "api/get_token";

        public const string ESCLATORMOVINGWALK = "Escalator/Moving-Walk";

        public const string PRODUCTELEVATOR = "Elevator";

        public const string OTHER = "Other";

        public const string TWINELEVATOR = "TWIN Elevator";


        public static readonly string NUMBEROFBUILDINGS = "NumberOfBuildings";
        public static readonly string BLDGID = "Id";
        public static readonly string BLDGNAME = "BuildingName";
        public static readonly string NUMBEROFGROUPS = "NumberOfGroups";
        public static readonly string PERMISSIONKEY = "PermissionKey";
        public static readonly string ENTITYNAME = "EntityName";
        public static readonly string BUILDINGSTATUS = "BuildingStatus";
        public static readonly string GROUPSTATUS_CAMEL = "GroupStatus";
        public static readonly string UNITSTATUS = "UnitStatus";
        public static readonly string PROJECTSTAGE = "ProjectStage";
        public static readonly string SALESMAN = "SalesMan";

        public static readonly string BuildingRise = "buildingRise";
        public static readonly string GRPID = "GroupConfigurationId";
        public static readonly string GRPNAME = "GroupName";
        public static readonly string NUMBEROFUNITS = "NumberOfUnits";
        public static readonly string NAME = "Name";
        public static readonly string BUILDINGNAME = "BldName";
        public static readonly string ISBUILDINGEQUIPMENT = "isBuildingEquipment";
        //public static readonly string BUILDINGEQUIPMENTSTATUS = "BuildingEquipmentStatus";
        public static readonly string MAINLINEVOLTAGE = "Mainlinevoltage";
        public static readonly string CONTROLROBOTIC = "ContolRobotic";
        public static readonly string BACNET = "Bacnet";
        public static readonly string INTERGRPEMRGNCYPOWER = "IntergroupEmergencyPower";

        public static readonly string NOTAPPLICABLE = "Not Applicable";
        public static readonly string COMPLETED = "Completed";
        public static readonly string PENDING = "Pending";
        public static readonly string QUEUEPOSITION = "QueuePosition";
        public static readonly string GROUPIDUPPER = "GroupID";
        public static readonly string GROUPVARIABLENAME = "Group";
        public static readonly string FORMSLAYOUTREFERENCEID = "FormsLayoutReferenceId";

        
        public static readonly string QUERYUSERNAME = "username=";
        public static readonly string QUERYPASSWORD = "&password=";
        public static readonly string ACCESSTOKENOBJ = "access_token";

        public static readonly string GRANTTYPEPASSWORD = "&grant_type=password";
        public static readonly string UTF8 = "utf-8";


        public static readonly string LINEID = "LineID";
        public static readonly string UNITNICKNAME = "UnitNickname";
        public static readonly string GROUPID = "GroupId";
        public static readonly string GROUPNAME = "GroupName";
        public static readonly string NEEDSVALIDATION = "NeedsValidation";
        public static readonly string UNITID = "UnitId"; 
        public static readonly string FIRESERVICEHALLSTATION = "FireServiceHallstation"; 
        public static readonly string DESIGNATION = "Designation";
        public static readonly string PRODUCTNAM = "ProductName";
        public static readonly string UNITID_LOWERCASE = "unitid";
        public static readonly string UNITID_CAMELCASE = "UnitId";
        public static readonly string UNITNAME_LOWERCASE = "unitname";
        public static readonly string UEID_LOWERCASE = "ueid";
        public static readonly string UNITNAME = "UnitName";
        public static readonly string UNITNAMEFDA_CAMECASE = "UnitName";
        public static readonly string CONTROLLOCATON_SP = "ControllerLocation_SP";
        public static readonly string CONTROLROOM_X = "ControlRoomX";
        public static readonly string CONTROLROOM_Y = "ControlRoomY";
        public static readonly string PRODUCTLINEIDNAME = "ProductLineIdName";
        public static readonly string GROUPSTATUS = "GroupStatus";
        public static readonly string UNIT_DESIGNATION = "UnitDesignation";
        public static readonly string PIT_DEPTH = "PitDepth";
        public static readonly string OVER_HEAD = "OverHead";
        public static readonly string FLOORING_THICKNESS = "FlooringThickness";
        public static readonly string NUMAUXILLIARYCOPS = "NumAuxilliaryCOPs";
        public static readonly string CARD_READER = "CardReader";
        public static readonly string FRONTDOORCOMPLEXITY = "FrontDoorComplexity";
        public static readonly string REARDOORCOMPLEXITY = "RearDoorComplexity";
        public static readonly string TRAVEL_FEET = "TravelFeet";
        public static readonly string TRAVEL_INCH = "TravelInch";
        public static readonly string GETDATASUCCESSFULLY = "Get data successfully";
        public static readonly string MAINEGRESS = "MainEgress";
        public static readonly string ENABLEDRAWINGGENERATION = "enableDrawingGeneration";
        public static readonly string DRAWINGGENERATIONMETHOD = "drawingGenerationMethod";
        public static readonly string ISMANUALDRAWINGGENERATION = "isManualDrawingGeneration";
        public static readonly string DRAWINGGENERATIONINFOMESSAGE = "drawingGenerationInfoMessage";
        public static readonly string ALTERNATEEGRESS = "AlternateEgress";
        public static readonly string LATESTDRAWINGSTATUS = "latestDrawingStatus";
        public static readonly string ISGROUPEDITABLE = "isGroupEditable";
        public static readonly string FDAVARIABLES = "@FDAVariables";
        public static readonly string EDITLAYOUT = "EditLayout";
        public static readonly string GROUPLOCKED = "GroupLocked";
        public static readonly string OPPORTUNITYIDVARIABLE = "OpportunityId";
        public static readonly string QUOTEID = "QuoteId";
        public static readonly string VERSIONID = "VersionId";
        public static readonly string VERSIONIDVALUE = "@VersionId";
        public static readonly string HISTORYTABLE = "@historyTable";
        public static readonly string ERRORMESSAGE = "Authorization has been denied for this request";
        public static readonly string REPORTERRORMESSAGE = "Error Encountered";
        public static readonly string PROXYERROR = "Proxy Authentication Required";
        public static readonly string PROXYISSUE = "Wrapper Token having proxy issue";
        public static readonly string UNITSCOUNTINQUOTE = "UnitsCountInQuotes";
        public static readonly string RESULTVALUES = "Result";
        public static readonly string ISNEWQUOTE = "IsNewQuote";
        public static readonly string VARIABLEMAPPERDATATABLE = "@VariableMapperDataTable";
        public static readonly string FLOORMATRIXHALLLANTERN= "Floor Matrix: Hall Lanterns";
        public static readonly string FLOORMATRIXHOISTWAYSILLS= "Floor Matrix: Hoistway Sills";
        public static readonly string FLOORMATRIXENTRANCEFRAME="Floor Matrix: Entrance Frames";
        public static readonly string FLOORMATRIXGENERALFLOORPROPERTIES="Floor Matrix: General Floor Properties";
        public static readonly string FLOORMATRIXPI= "Floor Matrix: Position Indicators";
        public static readonly string FLOORMATRIXCOMBO= "Floor Matrix: Combo Hall Lantern/PI";
        public static readonly string FLOORMATRIXHALLSTATIONS = "Floor Matrix: Hall Stations";
        public static readonly string FLOORMATRIXBRAILLE= "Floor Matrix: Elevator & Floor Designation Braille";
        public static readonly string FLOORMATRIXHALLTARGETINDICATOR = "Floor Matrix: Hall Target Indicator";
        public static readonly string FLOORMATRIXDESIGNATIONPLATE = "Floor Matrix: Hall Elevator Designation Plate";
        public static readonly string FLOORMATRIXBRAILLEETAETD = "Floor Matrix: Braille";
        public static readonly string BY = "BY";
        public static readonly string STATUSNAME = "StatusName";
        public static readonly string NUMBEROFFLOOR = "numberoffloor";
        public static readonly string MODEL = "Model";
        public static readonly string FLOORMATRIX="floorMatrix";
        public static readonly string STATUS_LOWERCASE = "StatusKey";
        public static readonly string STATUSNAME_LOWERCASE = "StatusName";
        public static readonly string LASTMODIFIED = "lastModified";
        public static readonly string VERSION = "version";
        public static readonly string MODIFIEDBY_CAMELCASE = "ModifiedBy";
        public static readonly string MODIFIEDBYFN = "modifiedByFN";
        public static readonly string OCCUPIEDSPACEBELOW = "OccupiedSpaceBelow";
        public static readonly string NOOFFLOORS = "NumberOfFloors";
        public static readonly string MINIMUM = "Minimum";
        public static readonly string CENTRE = "Center";
        public static readonly string RIGHT = "Right";
        public static readonly string WIDTH = "Width";
        public static readonly string DEPTH = "Depth";
        public static readonly string PITDEPTH = "PitDepth";
        public static readonly string OVERHEAD = "OverHead";
        public static readonly string DIMENSIONSELECTION = "DimensionSelection";
        public static readonly string MACHINETYPE = "MachineType";
        public static readonly string MOTORTYPESIZE = "MotorTypeSize";
        public static readonly string AVGFINISHWEIGHT = "AvailableFinishWeight";
        public static readonly string GROSSLOADONJACK = "GrossLoadOnJack";
        public static readonly string GROSSLOADONPOWER = "GrossLoadOnPower";
        public static readonly string NUMBEROFFLOORS = "NoOfFloors";
        public static readonly string FRONTOPENINGS = "FrontOpenings";
        public static readonly string REAROPENINGS = "RearOpenings";
        public static readonly string FLOORSSERVED = "FloorServed";
        public static readonly string UNITSDESIGNATION = "UnitDesignation";
        public static readonly string DISPLAYCARPOSITION = "DisplayCarPosition";
        public static readonly string CONTROLLOCATIONTYPE = "ControlLocationType";
        public static readonly string CONTROLLOCATIONVALUE = "ControlLocationValue"; 
        public static readonly string UNITCURRENTLYCONFIGURED = "UnitCurrentlyConfigured";
        public static readonly string FRONTDOOR = "Front";
        public static readonly string NR = "NR";
        public static readonly string REARDOOR = "Rear";
        public static readonly string LEFTSIDEDOOR = "LeftSideDoor";
        public static readonly string RIGHTSIDEDOOR = "RightSideDoor";
        public static readonly string DOOROPENINGS = "DoorOpenings";
        public static readonly string PRODUCT = "Product";
        public static readonly string PRICE_CAMELCASE = "Price";
        public static readonly string DESCRIPTION = "Description";
        public static readonly string SIDEDOOR = "Side";
        public static readonly string TRAVELFEET = "Travelfeet";
        public static readonly string TRAVELINCH = "TravelInch";
        public static readonly string TRAVEL = "Travel";
        public static readonly string MACHINEMODEL = "MachineModel";
        public static readonly string PRODUCTMODEL = "ProductModel";
        public static readonly string TRAVELVARIABLEID = "TRAVELVARIABLEID";
        public static readonly string TRAVELVARIABLEIDVALUE = "TRAVELVARIABLEIDVALUE";
        public static readonly string VARIABLEASSIGNMENT = "variableAssignment";
        public static readonly string VALUE = "value";
        public static readonly string SIDEOPENING = "SideOpening";
        public static readonly string UEID = "UEID";
        public static readonly string CAPACITY = "Capacity";
        public static readonly string SPEED = "Speed";
        public static readonly string LANDINGS = "Landings";
        public static readonly string FRONTOPENING = "FrontOpening";
        public static readonly string REAROPENING = "RearOpening";
        public static readonly string SETCONFIGURATIONID = "SetId";
        public static readonly string SETNAME = "SetName";
        public static readonly string BUILDINGJSON = "BldJson";
        public static readonly string @USERID = "@userId";
        public static readonly string @USERNAME = "@userName";
        public static readonly string COUNTRYFORPROJECTS = "@Country";
        public static readonly string ROLE = "@userRole";
        public static readonly string SETDESCRIPTION = "SetDescription";
        public static readonly string BRANCHDETAILS = "BranchDetails";
        public static readonly string MEASURINGUNITDETAILS = "MeasuringUnitDetails";
        public static readonly string SALESDETAILS = "SalesDetails";
        public static readonly string BUILDINGLABEL = "BuildingLabel";
        public static readonly string BIDAWARDED = "Bid Awarded";
        public static readonly string AWARDCLOSEDATEJSON = "Projects.AwardCloseDate"; 
        public static readonly string MODIFIEDBYLN = "modifiedByLN";
        public static readonly string B2P = "B2P";
        public static readonly string FRONTOPEN = "FrontOpenings";
        public static readonly string REAROPEN = "RearOpenings";
        public static readonly string @QUOTEIDSPPARAMETER = "@quoteId";
        public static readonly string @PROJECTID = "@ProjectId";
        public static readonly string @OPPORTUNITYID = "@OpportunityId";
        public static readonly string @GRPCONFIGURATIONID = "@GroupConfigurationId";
        public static readonly string @UNITSETID = "@setId";
        public static readonly string QUOTEID_UPPERCASE = "@QuoteId";
        public static readonly string IsRequest = "@IsRequest";
        public static readonly string ISDOD = "IsDoD";
        public static readonly string OUTFORAPPROVAL = "OutForApproval";
        public static readonly string FORFINAL = "ForFinal";
        public static readonly string FORREVISERESUBMIT = "ForReviseResubmit";
        public static readonly string RECEIVEDDATE = "ReceivedDate";
        public static readonly string SENTDATE = "SentDate";
        public static readonly string LAYOUTSUBMITTAL = "LayoutSubmittal";
        public static readonly string ENTRANCESUBMITTAL = "EntranceSubmittal";
        public static readonly string CABSUBMITTAL = "CabSubmittal";
        public static readonly string CARFIXTURESUBMITTAL = "CarFixtureSubmittal";
        public static readonly string HALLFIXTURESUBMITTAL = "HallFixtureSubmittal";
        public static readonly string LOBBYPANELSUBMITTAL = "LobbyPanelSubmittal";
        public static readonly string FREIGHTSUBMITTAL = "FreightSubmittal";
        public static readonly string NUMBEROFSTOPS = "NumberOfStops";
        public static readonly string NUMBEROFGROUPSINBUILDING = "NumberOfGroupsInBuilding";
        public static readonly string NUMUNITSINGROUP = "Numunitsingroup";
        public static readonly string ESTIMATESATUS = "EstimateStatus";
        public static readonly string @DRAWINGMETHODSTATUS = "@drawingMethodStatus";
        public static readonly string @USERROLE = "@userRole";
        public static readonly string UNITPOSITION = "UnitPosition";
        public static readonly string TWIN = "TWIN";
        public static readonly string @USERBRANCH = "@userBranch";
        public static readonly string MANUFACTURINGCOMMENTSTABLEID = "manufacturingCommentsTable";
        public static readonly string @FIELDDRAWINGID = "@FieldDrawingId";
        public static readonly string MANUFACTURINGCOMMENTSTABLENAME="Manufacturing Comments";
        public static readonly string @QUOTEIDSPPARAMETERLATEST = "@quoteId";
        public static readonly string @_ID = "@id";
        public static readonly string @TYPE = "@type";
        public const string MANUFACTORINGCOMMENTS = "ManufacturingComments";
        public static readonly string @ROLENAME = "@roleName";
        public static readonly string @ENTITY = "@entity";
        public static readonly string BUILDINGENTITY = "building";
        public static readonly string GROUPENTITY = "Group";
        public static readonly string UNITENTITY = "Unit";
        public static readonly string PRODUCTENTITY = "ProductSelection";
        public static readonly string @SECTION = "@section";
        public static readonly string @OPPORTUNITYID1 = "@OpportunityId";
        public static readonly string @BUILDINGCONFIGID = "@buildingConfigurationId";
        public static readonly string @MODIFIEDBY = "@ModifiedBy";
        public static readonly string @RESULT = "@Result";
        public static readonly string REQUESTMESSAGE = "requestMessage";
        public static readonly string CREATEDBY = "CreatedBy";
        public static readonly string CREATEDON = "CreatedOn";
        public static readonly string @LAYOUTSTATUS = "@layoutStatus";
        public static readonly string @FIXTURESTRATEGY = "@FixtureStrategy";
        public static readonly string @UHFDefaults = "@defaultUHFConfiguration";
        public static readonly string MMDDYYYYFORMAT = "MM/dd/yyyy";
        public static readonly string CREATEDBYFN = "createdByFN";
        public static readonly string CREATEDBYLN = "createdByLN";
        public static readonly string HALOCTOPF="HALOCTOPF";
        //Log History
        public static readonly string CURRENTVALUE = "CurrentValue";
        public static readonly string PREVIOUSVALUE = "Previousvalue";
        public static readonly string USER = "User";
        public static readonly string UNIT = "Unit";
        public static readonly string SHOWLOADMORE = "ShowLoadMore";
        public static readonly string VARIABLEID = "VariableId";
        public static readonly string COUNTERWEIGHTSAFETY = "CounterWeightSaftey";
        public static readonly string FDAXMLVARIABLEID = "VariableId";
        public static readonly string FDAXMLVALUE = "Value";
        public static readonly string LOCATION = "Location";
        public static readonly string FLOORSSERVICED = "FloorsServiced";
        public static readonly string ELELANDPPARAMETER = "ELELANDPPARAMETER";
        public static readonly string TOTALOPENINGSPARAMETER = "TOTALOPENINGSPARAMETER";

        public static readonly string PARAMETERS = "Parameters.";
        public static readonly string FDAGROUPCONFIGURATIONID = "GroupConfigurationId";

        public static readonly string FDAINTEGRATEDPROCESSID = "IntegratedProcessId";

        public static readonly string FDAID = "Id";

        public static readonly string FDASTATUSID = "StatusId";

        public static readonly string FDASTATUSIDLOWERCASE = "StatusId";
        public static readonly string FDAREFERENCEID = "referenceId";

        public static readonly string FDAFIELDDRAWINGINTEGRATIONID = "fieldDrawingIntegrationMasterId";

        public static readonly string CESCREENCONSTANTMAPPER = @"Templates\Unit\ConstantMapper.json";

        public static readonly string FLOORMATRIXLANDING = ".FloorMatrix.LANDING";

        public static readonly string FDAINTEGRATEDSYSTEMREF = "IntegratedSystemRef";

        public static readonly string FDASETID = "setId";
        public static readonly string FDAUNITNAME = "Name";

        public static readonly string FDAUNITID = "UnitId";

        public static readonly string MAPPEDLOCATION = "MappedLocation";
        public static readonly string ELEVATORLEVEL = "ElevatorLevel";
        public static readonly string LANDINGLEVEL = "LandingLevel";





        public static readonly string GROUPCONFIGURATION = "GroupConfiguration";
        public static readonly string GROUPJSON = "GroupJson";
        public static readonly string DISPLAYJSON = "DisplayUnitJson";
        public static readonly string UNITJSON = "UnitJson";
        public static readonly string UNITDesignation = "Designation";
        public static readonly string UNITDESIGNATION = "UnitDesignation";
        public static readonly string HALLRISERJSON = "HallRiserJson";
        public static readonly string DOORJSON = "DoorJson";
        public static readonly string CONTROLLOCATIONJSON = "ControlLocationJson";
        public static readonly string GROUPCONFIGID = "GroupConfigurationId";
        public static readonly string ISDELETED = "isDeleted";
        public static readonly string ISEDITABLE = "isEditable";
        public static readonly string BUILDINGID = "@BuildingId";
        public static readonly string BUILDINGEQUIPMENTVARIABLESLIST = "@BuildingEquipmentVariablesList";

        public static readonly string CONSTANTMAPPERLIST = "@ConstantMapperList";


        public static readonly string GROUPINGID = "@GroupId";
        public static readonly string GROUPINGID_LOWERCASE = "@groupId";

        public static readonly string NUMBEROFUNITS_LOWERCASE = "@numberOfUnits";

        public static readonly string CREATEDBY_LOWERCASE = "@createdBy";


        public static readonly string QUOTEID_LOWERCASE = "@quoteId";


        public static readonly string FIELDDRAWINGINTEGRATIONMASTERID = "@fieldDrawingIntegrationMasterId";

        public static readonly string SYSTEMSTATUS = "SystemStatus";
        public static readonly string DISPLAYNAMESTATUS = "DisplayName";
        public static readonly string DESCRIPTIONSTATUS = "Description";

        public static readonly string ACTION = "@Action";
        public static readonly string CARPOSITIONLIST = "@CarPositionArray";
        public static readonly string BUILDINGIDLIST = "@BuildingList";
        //public static readonly string GROUPINGID = "@GroupId";
        //public static readonly string CARPOSITIONLIST = "@CarPositionList";
        public static readonly string GROUPIDLIST = "@GroupList";
        public static readonly string UNITIDLIST = "@UnitList";
        public static readonly string BUILDINGIDCOLUMNNAME = "BuildingId";
        public static readonly string BUILDINGIDUPPER = "BuildingID";
        public static readonly string GROUPIDCOLUMNNAME = "GroupId";

        public static readonly string GROUPNAMECOLUMNNAME = "GroupName";
        public static readonly string TOTALNUMBEROFFLOORS = "TotalNumberOfFloors";
        public static readonly string NUMBEROFFLOORSUNITPACKAGEVARIABLE = "NUMBEROFFLOORSUNITPACKAGEVARIABLE";
        public static readonly string TOTALNUMBEROFFLOORSUNITPACKAGEVARIABLE = "TOTALNUMBEROFFLOORSUNITPACKAGEVARIABLE";
        //public static readonly string UNITIDLIST = "@UnitList";
        public static readonly string BUILDINGNAMECOLUMNNAME = "BuildingName";
        public static readonly string SELECTEDELEVATORS = "@selectedElevators";
        public static readonly string @PRODUCTCATEGORYID = "@ProductCategoryId";
        public static readonly string @GROUPCONFIGRATIONID = "@GroupConfigurationId";
        public static readonly string @GROUPCONID = "@GroupId";
        //public static readonly string GROUPIDCOLUMNNAME = "GroupId";
        //public static readonly string GROUPNAMECOLUMNNAME = "GroupName";
        public static readonly string ISFUTUREELEVATOR = "IsFutureElevator";
        public static readonly string @FLAG = "@Flag";
        //public static readonly string GROUPNAMELOWERCASE="GroupName";
        public static readonly string BUILDINGID_CAMELCASE = "@BuildingId";
        public static readonly string GROUPID_CAMELCASE = "@groupId";
        public static readonly string OPENFRONT = "OPENFRONT";
        public static readonly string OPENREAR = "OPENREAR";
        public static readonly string LOBBYPANEL = "Lobby Panel";
        public static readonly string ISDISABLEDFORGHF = "IsDisabledForGroupHallFixtures";
        public static readonly string ISDISABLEDFOROPENINGS = "IsDisabledForOpenings";
        public static readonly string PRODUCTCATEGORY = "ProductCategory";
        public static readonly string GROUPHALLFIXTURE = "GroupHallFixtures";
        public const string INT = "INT";
        public const string BOOLEAN = "BOOLEAN";
        public const string DECIMAL = "DECIMAL";
        public const string BUINDINGTYPE = "BuindingType";
        public const string BUINDINGVALUE = "BuindingValue";
        public const string CONTROLOCATIONTYPE = "ControlLocationType";
        public const string CONTROLOCATIONVALUE = "ControlLocationValue";
        public static readonly string PRODUCTNAMECOLUMN = "ProductName";
        public const string DATEDATATYPE = "DATE";
        public const string CONTROLLOCATION = "controlLocation";
        public const string OPENINGLOCATIONS = "OpeningLocations";
        public const string ISEDITFLOW = "isEditFlow";

        public const string TOTALNUMLANDINGS_SP = "TOTALNUMLANDINGS_SP";
        public const string OPNFRTP = "OPNFRTP";
        public const string CWTSFTY = "CWTSFTY";
        public const string OPNREARP = "OPNREARP";
        public const string TOPR = "TOPR";
        public const string TOPF = "TOPF";
        public const string TOTOPN = "TOTOPN";
        public const string TRAVELCAPS = "TRAVEL";
        public const string ENTF = "ENTF";
        public const string ENTR = "ENTR";
        public const string ELEVATION = "ELEVATION";
        public const string FLRHTR = "FLRHTR";
        public const string FIELDDRAWINGAUTOMATION = "fieldDrawingAutomation";


        public const string ISEDITFLAG = "isEditFlag";
        public static readonly string @GROUPIDSPPARAM = "@GroupId";
        public static readonly string @GRPCONFIGID = "@GroupConfigurationId";
        public static readonly string @UNITLIST = "@unitList";


        //Wrapper API
        public static readonly string @WRAPPERUSERNAME = "c2d.svcaccount@tkelevator.com";
        //public static readonly string @WRAPPEPASSWORD = "C2Dpa$$word";
        public static readonly string @WRAPPERGRANTTYPE = "password";
        public const string GETFIELDDRAWINGAUTOMATIONWRAPPERID = "usp_GetFieldDrawingAutomationWrapperId";


        public static readonly string CAPACITY_UPPERCASE = "CAPACITY";
        public static readonly string CARSPEED_UPPERCASE = "CARSPEED";
        public static readonly string @USERNAMEUSER = @"TK-AMS/C2DUSER";
        public static readonly string PASSWORD = "CRM@12345";
        public static readonly string PROJECTS = "Projects";
        public static readonly string @PRODUCTNAME = "@productName";
        public static readonly string HOSITWAYTRACTIONEQUIPMENTJSON = "HositwayTractionEquipmentJson";
        public static readonly string CABINTERIORJSON = "CabInteriorJson";
        public static readonly string GENERALINFORMATIONJSON = "GeneralInformationJson";
        public static readonly string ENTRANCEJSON = "EntranceJson";
        public static readonly string CONFIGVARIABLES = "ConfigureVariables";
        public static readonly string CONFIGUREVALUES = "ConfigureValues";
        public static readonly string ENTRANCECONFIGURATIONVARIABLEIDSPLIT = "Landing_Doors_Assembly.";
        public static readonly string HALLLANTERNVARIABLEIDSPLIT = "User_Interface_Devices.Landing_Indicator_Panel_LIP.";
        public static readonly string HALLLANTERNVARIABLESIDSPLITLOP = "User_Interface_Devices.Landing_Operating_Panel_LOP.";
        public static readonly string DISPLAYPROPERTY = "displayColor";
        public static readonly string COLORYELLOW = "yellow";
        public static readonly string COLORRED = "red";
        public static readonly string QUOTATION = "Quotation";
        public static readonly string UNITMATERIALS = "UnitMaterials";
        public static readonly string DOORTYPE = "DoorType";
        public static readonly string ACTIONFIELDDRAWINGAUTOMATION = "FieldDrawingAutomation";
        public static readonly string DOORVALUE = "DoorValue";
        public static readonly string ACTIONFIELDDRAWINGLAYOUTDETAILS = "FieldDrawingLayoutDetails";
        public static readonly string MAPPEDLOCATIONJSON = "MappedLocationJson";
        public static readonly string FIELDDRAWINGGENERATIONSETTINGS = "FieldDrawingGenerationSettings";
        public static readonly string GROUPCONFIGURATIONTYPE = "GroupConfigurationType";
        public static readonly string BATCH1LEADTIME = "Batch1LeadTime";
        public static readonly string BATCH2LEADTIME = "Batch2LeadTime";
        public static readonly string BATCH3LEADTIME = "Batch3LeadTime";
        public static readonly string BATCH4LEADTIME = "Batch4LeadTime";
        public static readonly string BATCH5LEADTIME = "Batch5LeadTime";
        public static readonly string BATCH6LEADTIME = "Batch6LeadTime";
        public static readonly string MANUFACTURINGLEADTIME = "ManufacturingLeadTime";

        public const string BATCH1LEADTIMECAPS = "BATCH1LEADTIME";
        public const string BATCH2LEADTIMECAPS = "BATCH2LEADTIME";
        public const string BATCH3LEADTIMECAPS = "BATCH3LEADTIME";
        public const string BATCH4LEADTIMECAPS = "BATCH4LEADTIME";
        public const string BATCH5LEADTIMECAPS = "BATCH5LEADTIME";
        public const string BATCH6LEADTIMECAPS = "BATCH6LEADTIME";
        public const string MANUFACTURINGLEADTIMECAPS = "MANUFACTURINGLEADTIME";

        public const string GROUPCONFIGURATIONVALUE = "GroupConfigurationValue";
        public static readonly string @SOURCEQUOTEID = "@SourceQuoteId";
        public static readonly string SOURCEVERSION = "@VersionId";
        public static readonly string PARENTVERSION = "@ParentVersionId";

        public static readonly string DESTINATIONPROJECTID = "@DestinationProjectId";

        public static readonly string CONFLICTVARIABLEID = "conflictVariableId";
        public static readonly string CONFLICTVARIABLEVALUES ="conflictVariableValues";
        public static readonly string LISTOFVARIABLEASSIGNMENT = "@listofVariableAssignment";
        public static readonly string CONFLICTVARIABLEIDDATA = "VariableId";


        #endregion


        #region template paths
        public static readonly string GROUPMAPPERVARIABLESMAPPERPATH = @"Templates\Group\Elevator\ConstantMapper.json";
        public static readonly string GROUPMAPPERCONFIGURATION = "GroupConstantMapper"; 
        public static readonly string FLOORPLANDISTANCEPARAMETERS = "floorPlanDistanceParameters";
        public static readonly string DUPLICATECONSTANTMAPPERPATH = @"Templates\Duplicate\ConstantMapper.json";
        public static readonly string CONSTANTMAPPER="ConstantMapper";
        public static readonly string FDAMAPPERVARIABLESMAPPERPATH = @"Templates\FieldDrawingAutomation\ConstantMapper.json";
        #endregion


        #region Parameter related Constants
        public static readonly string FRONTDOORTYPEANDHAND = "FRONTDOORTYPEANDHAND";
        public static readonly string REARDOORTYPEANDHAND = "REARDOORTYPEANDHAND";
        public static readonly string SIDEDOORTYPEANDHAND = "SIDEDOORTYPEANDHAND";        
        public static readonly string PARAMETERBASICREAROPEN = "PARAMETERBASICREAROPEN";
        public static readonly string ELEVATOR00 = "Elevator00";
        public static readonly string ELEVATOR00CAPS =  "ELEVATOR00";
        public static readonly string TOTALUNITSINPANEL = "TOTALUNITSINPANEL";

        #endregion


        public static readonly string GROUPMAPPERVARIABLES = @"Templates\Group\Elevator\ConstantMapper.json";
        public static readonly string CONFIGUREORPRICEBLOCKSTARTED = "[Information] : ConfigureOrPrice block started";
        public static readonly string EXCEPTION = "[Information] Exception: ";
        public const string GROUPHALLFIXTUREUPDATEMESSAGE = "GroupHallFixture updated Successfully";
        public const string ACROSSTHEHALLDISTANCE = "ACROSSTHEHALLDISTANCEPARAMETER";
        public const string GROUPDESGN = "GROUPDESGN";
        public const string BANKOFFSET = "BANKOFFSETPARAMETER";
        public const string SAVEBUILDINGEQUIPMENTCONSOLESUCCESSMSG = "Lobby Panel selections saved successfully";
        public const string SAVEBUILDINGEQUIPMENTCONSOLEERRORMSG = "Error occurred while saving building equipment console";
        public const string UPDATEBUILDINGEQUIPMENTCONSOLESUCCESSMSG = "Building Equipment console updated successfully";
        public const string DUPLICATEBUILDINGEQUIPMENTSUCCESSMSG = "Console Duplicated sucessfully";
        public const string DUPLICATEBUILDINGEQUIPMENTCONSOLEERRORMSG = "Error Occurred while duplicating the console";
        public const string DELETEBUILDINGEQUIPMENTSUCCESSMSG = "BuildingEquipmentConsole deleted sucessfully";
        public const string DELETEPROJECTSUCCESSMSG = "project,version deleted sucessfully";
        public const string DELETEBUILDINGEQUIPMENTCONSOLEERRORMSG = "Error Occurred while deleting the console";
        public const string SAVEBUILDINGEQUIPMENTSUCCESSMSG = "Building Equipment selections saved successfully";
        public const string SAVEBUILDINGEQUIPMENTERRORMSG = "Error occurred while saving building equipment selections";
        public const string UPDATEBUILDINGEQUIPMENTSUCCESSMSG = "Building Equipment selections updated successfully";
        public const string WRAPPERAPISTARTED = "Wrapper API Started";
        public const string PROXY = "Proxy: {0}";
        public const string PROXYNULL = "Proxy is null; no proxy will be used";
        public const string POSTARRAYCALLED = "Post array Called :";
        public const string CALLINGDATASTREAM = "Calling Data Stream :";
        public const string EXECTINGTHERESSTREAM = "execting the resstream";
        public const string GETGROUOCONFIGURATIONSTARTED = "GetGroupConfigurationDetailsByGroupId started";

        public const string STREAMREADEREXECUTEDSUCCESSFULLY = "stream reader executed successfully";

        public const string ACCESSTOKEN = "Access Token: ";

        public const string WRAPPERAPIRESULT = "Wrapper API Result : ";

        public const string WRAPPERAPICOMPLETED = "Wrapper API Completed";

        public const string WRAPPERAPIERROR = "Wrapper API Error :";

        public static readonly string THEREISNODATA = "There is no data avaiable in DB";

        public static readonly string STRINGVARIABLENAME = "string";

        public const string PRODUCTCATEGORY_LOWERCASE = "productCategory";

        public static readonly string BUILDING_CONFIGURATION = "Building_Configuration";

        public static readonly string ELEVATOR_CONFIGURATION = "ELEVATOR";


        public const string ADDED = "Added";
        public const string REMOVED = "Removed";
        public const string MODIFIED = "Modified";








        public static readonly string ENVIRONMENTPATH = "EnvironmentPath";
        public static readonly string CRMODUSERNAME = "CRMODUserName";
        public static readonly string CRMODPASSWORD = "CRMODPassword";
        public static readonly string CRMODPROXYSERVER = "CRMODProxyServer";
        public static readonly string ISDISABLED = "isDisabled";
        public static readonly string BUILDINGEQUIPMENTSTATUS = "BuildingEquipmentStatus";
        public static readonly string FDABUILDINGSTATUS = "BuildingStatus";
        public static readonly string FDAGROUPSTATUS = "GroupStatus";
        public static readonly string ADINFOSYS = "ad.infosys.com";
        public static readonly string COORDINATION = "Coordination";
        public static readonly string OPPORTUNITY = "OpportunityId";
        public static readonly string VERSIONIDFORCRM = "VersionId";
        public static readonly string CREATEDBYCRM = "CreatedBy";
        public static readonly string CREATEDONCRM = "CreatedOn";
        public static readonly string MODIFIEDONCRM = "ModifiedOn";

        public static readonly string BUILDINGCONFLICTCHECK = "BuildingConflictstatus";
        public static readonly string GROUPCONFLICTCHECK = "GroupConflictstatus";
        public static readonly string UNITSCONFLICTCHECK = "UnitsConflictstatus";
        public static readonly string HASCONFLICTFLAG = "HasConflictsFlag";
        public static readonly string UNITCONFLICTCHECK = "UnitsConflictstatus";
        public static readonly string DOOR = "Door";
        public static readonly string USERNAMEVARIABLE = "userName";
        public const string FIXTURETYPEVARIABLE = "FixtureType";
        public const string HALLSTATIONNAME = "HallStationName";
        public static readonly string DOORVARIABLES = "doorVariables";
        public static readonly string HALLSTATIONVARIABLES = "hallStationVariables"; 
        public static readonly string DEFAULTVARIABLES = "defaultVariables";
        /// <summary>
        /// / Wrapper API
        /// </summary>
        public static readonly string DRAWINGSAPISETTINGS = "DrawingsApi";
        public static readonly string BASEURL = "BaseUrl";
        public static readonly string TOKENAPI = "TokenApi";
        public static readonly string REQUESTLAYOUTAPI = "RequestLayoutApi";
        public static readonly string LAYOUTSTATUSAPI = "LayoutStatusApi";
        public static readonly string APIUSERNAME = "UserName";
        public static readonly string APIPASSWORD = "Password";
        public static readonly string GRANTTYPESETTING = "GrantType";
        public static readonly string PROXYURI = "Proxy";

        /// <summary>
        /// View API
        /// </summary>
        public static readonly string PROJECTMMANAGEMENTSETTINGS = "ProjectManagementApi";
        public static readonly string PROJECTINFOAPI = "ProjectInfoApi";
        public static readonly string SAVEQUOTEAPI = "SaveQuoteApi";
        public static readonly string ERRORCNST = "Error";
        public static readonly string ERRORSTACKTRACECNST = "Error Stack Trace";
        public static readonly string ISOCODE = "ISO-8859-1";
        public static readonly string AUTHORIZATION = "Authorization";

        /// <summary>
        /// Oz API
        /// </summary>
        public static readonly string COORDINATIONSETTINGS = "CoOrdinationApi";
        public static readonly string BOOKINGAPI = "BookingApi";
        public const string BADREQUESTTEXT = "BADREQUEST";
        public const string SUCCESSTEXT = "SUCCESS";
        public const string OK = "OK";
        public const string ERRORINBOOKINGREQUEST = "Error while sending to Coordination";
        public const string ERRORWHILEUPDATINGWORKFLOWSTATUS = "An error occured while updating workflow status";
        public static readonly string SAVEMSG = "Saved Successfully";
        public static readonly string SENDTOCOORDINATIONSUCCESSMSG = "Sent to Coordination successful";
        public static readonly string OZQUOTETABLE = "dbo.tblQuote";
        public static readonly string DUPLICATE = "duplicate";
        public static readonly string OZSUBMITTALTABLE = "dbo.tblSubmittal";
        public static readonly string QUOTEIDERROR1 = "Quote Id : ";
        public static readonly string QUOTEIDERROR2 = " already present Database.Please try using different QuoteId";
        public static readonly string OZUEIDERROR = "One or more UEIDs already present in Database. Please try with different UEIDs";
        public static readonly string OZ="OZ";
        /// <summary>
        /// SYSTEM VALIDATION API
        /// </summary>
        public static readonly string SYSTEMVALIDATIONSETTINGS = "SystemValidationApi";
        /// MFile API
        public const string VAULTSETTINGS = "Vault";
        public static readonly string VAULTGUID = "VaultGuid";
        public static readonly string FOLDERAPI = "FolderApi";
        public static readonly string XAUTHENTICATION = "X-Authentication";
        public static readonly string VAULTEXCEPTION = "Error while fetching Vault Location";
        public static readonly string UPLOADAPI = "UploadApi";

        public static readonly string SERVICETESTPATH = "ServiceTestPath";
        public static readonly string OZUSERNAME = "user_name";
        public static readonly string OZPASSWORD = "password";
        public static readonly string OZGRANTYPE = "grant_type";

        public static readonly string ACCEPT = "Accept";
        public static readonly string BEARER = "Bearer ";
        public static readonly string APPLICATIONJSON = "application/json";
        public static readonly string DODID = "id";

        public static readonly string BASIC = "Basic ";
        public static readonly string PROCESSTARTED = "Process Started";
        public static readonly string GETPROJECTINFOMETHOD = "sharp/configitapi/project/getprojectinfo?";
        public static readonly string CANADA = "Canada";
        public static readonly string COUNTRYVALUE = "AccountDetails.SiteAddress.Country";
        public static readonly string LEADVALUES = "Lead";
        public static readonly string NEWINSTALLATION = "NewInstallation";
        public static readonly string INCOMPLETEVALUES = "Incomplete";
        /// <summary>
        /// DocumentGeneration API
        /// </summary>
        public static readonly string DOCUMENTGENERATIONSETTINGS = "DocumentGenerator";
        public static readonly string DOCUMENTGENERATIONAPIROUTE = "apiRoute";
        //public static readonly string VIEWPROXYSERVER = "CRMODProxyServer";

        public static readonly string INTEGRATEDSYSTEMREF = "@IntegratedSystemRef";
        public static readonly string INTEGRATEDPROCESSID = "@IntegratedProcessId";
        public static readonly string INTEGRATEDSYSTEMID = "@IntegratedSystemId";

        public static readonly string FIELDDRAWINGINTEGRATIONID = "@FieldDrawingIntegrationId";
        public static readonly string STATUSID = "@StatusId";
        public static readonly string STATUSIDCOMOLETED = "@statusIdCompleted";
        public static readonly string FDAPROCESSJSON = "@FDAProcessJson";

        public const string GETVARIABLEVALUESFOROZ = "usp_GetVariableValuesForOZ";
        public static readonly string Q1 = "Q1";
        public static readonly string Q2 = "Q2";
        public static readonly string Q3 = "Q3";
        public static readonly string Q4 = "Q4";
        public static readonly string YES = "Yes";
        public static readonly string NO = "No";
        public static readonly string SUBMITTALSOUTFORAPPROVAL = "Submittals_out_for_approval";
        public static readonly string SUBMITTALSBEINGRETURNEDFORFINALS = "Submittals_being_returned_for_finals";
        public static readonly string SUBMITTALSBEINGRETURNEDFORREVISIONS = "Submittals_being_returned_for_revisions";
        public static readonly string BRANCHNAME = "@branchName";
        public static readonly string BRANCHNUMBER = "BranchNumber";
        public static readonly string BRANCHNAMEVARIABLENAME = "BranchName";
        public static readonly string NEWINSTALLATIONVALUES = "New Installation";
        public static readonly string RETURNVALUE = "returnValue";
        public static readonly string BRANCH = "Branch";

        #region StatusCode related constants

        public static readonly int INTERNALSERVERERROR = StatusCodes.Status500InternalServerError;
        public static readonly int SUCCESS = StatusCodes.Status200OK;
        public static readonly int CREATED = StatusCodes.Status201Created;
        public static readonly int CONFLICT = StatusCodes.Status409Conflict;
        public static readonly int UNAUTHORIZED = StatusCodes.Status401Unauthorized;
        public static readonly int FORBIDDEN = StatusCodes.Status403Forbidden;
        public static readonly int NOTFOUND = StatusCodes.Status404NotFound;
        public static readonly int BADREQUEST = StatusCodes.Status400BadRequest;

        #endregion

        /// <summary>
        /// Constants Related to Store procedure Name
        /// This constants are using to get Store procedure Name
        /// </summary>
        #region Store Procedure names constants


        //BuildingConfigurationDL
        public const string SPGETBUILDINGCONFIGFORPROJECT = "usp_GetListOfConfigurationForProject";
        public const string SPGETBUILDINGCONFIGBYID = "usp_GetBuildingConfigurationById";
        public const string SPGETPERMISSIONBYROLENAME = "usp_GetPermissionByRoleName";
        public const string SPUPDATEWORKFLOWSTATUS = "usp_UpdateWorkflowStatus";
        public const string SPGETBUILDINGCONFIGBYGROUPID = "usp_GetBuildingConfigurationByGroupId";
        public const string SPGETQUICKSUMMARYCONFIGBYID = "usp_GetQuickConfigurationSummary";
        public const string SPGETDETAILSFORTP2SUMMARY = "usp_GetDetailsForTP2Summary";
        public const string SPGETAUTOSAVECONFIGBYID = "usp_GetAutoSaveConfigurationByUser";
        public const string SPINSERTBUILDINGELEVATION = "usp_InsertBuildingElevation";
        public const string SPAUTOSAVEBUILDINGELEVATION = "usp_AutoSaveBuildingElevation";
        public const string SPUPDATEBUILDINGELEVATION = "usp_UpdateBuildingElevation";
        public const string SPGETBUILDINGELEVATIONBYID = "usp_GetBuildingElevationById";
        public const string SPDELETEBUILDINGCONFIGBYID = "usp_DeleteBuildingConfigurationById";
        public const string SPDELETEBUILDINGELEVATIONBYID = "usp_DeleteBuildingElevationById";
        public const string SPDELETEGROUPBYID = "usp_DeleteGroupConfiguration";
        public const string SPADDBUILDINGSFORPROJECT = "usp_SaveBuildingConfiguration";
        public const string SPAUTOSAVECONFIGURATION = "usp_AutoSaveConfiguration";
        public const string SPDELETEAUTOSAVECONFIGURATION = "usp_DeleteAutoSaveConfiguration";
        public const string SPUPDATEBUILDINGSFORPROJECT = "usp_UpdateBuildingConfiguration";
        public const string SPDELETEUNITHALLFIXTURECONFIGBYID = "usp_DeleteUnitHallFixtureConsole";
        public const string SPDELETEGROUPHALLFIXTURECONFIGBYID = "usp_DeleteGroupHallFixtureConsole";
        public const string SPCHECKIFGROUPEXISTFORABUILDING = "usp_CheckGroupExist";
        public const string SPCHECKIFUNITCONFIGURED = "usp_CheckUnitConfigured";
        public const string SPCHECKPRODUCTSELECTED = "usp_CheckProductSelected";
        public const string SPDUPLICATEBUILDINGCONFIGURATIONBYBUILDINGID = "usp_DuplicateBuildingByBuildingid";
        public const string SPDUPLICATEGROUP = "usp_DuplicateGroupToDifferentBuilding";
        public const string SPDUPLICATEUNIT = "usp_DuplicateUnitToGroup";
        public const string SPGETGROUPVARIABLEDETAILS = "GetGroupVariableDetails";
        public const string SPGETTRAVELVALUEFORUNITCONFIGURATION = "usp_GetTravelValueForUnitConfiguration";

        public const string USP_GETINPROGRESSSYSTEMSVALUES = "usp_GetInProgressSystemsValues";
        public const string USP_SAVEANDUPDATESYSTEMVALIDATION = "usp_SaveAndUpdateSystemValidation";

        public const string USP_GETVALIDATIONSTATUS = "usp_GetValidationStatus";

        //public const string SPDUPLICATEUNIT = "usp_DuplicateUnitToGroup";



        public const string SPSAVEASSIGNEDGROUPS = "usp_SaveAssignedGroups";
        public const string SPSAVEBUILDINGEQUIPMENTCONFIGURATION = "usp_SaveBuildingEquipmentConfiguration";
        //public const string SPCHECKIFGROUPEXISTFORABUILDING = "usp_CheckIfGroupExistForBuilding";
        //public const string SPCHECKIFUNITCONFIGURED = "usp_CheckIfUnitConfiguredForGroup";
        //public const string SPDUPLICATEGROUP = "usp_DuplicateGroupToDifferentBuilding";
        public const string SPRESETUNITHALLFIXTURE = "usp_ResetUnitHallFixtureConsoleNew";

        public const string SPSAVEGROUPCONFIGURATION = "usp_SaveGroupConfiguration";
        public const string SPSAVEUNITSFORNONCONFIGURABLEPRODUCTS = "usp_SaveUnitsForNonConfigurableProducts";

        public const string SPUPDATEGROUPCONFIGURATION = "usp_UpdateGroupConfiguration";
        public const string SPUPDATEOPENINGLOCATION = "usp_UpdateOpeningLocation";


        public const string SPGETOPENINGLOCATIONBYID = "usp_GetOpeningLocationBygroupId";
        public const string SPSAVEGROUPLAYOUT = "usp_SaveGroupLayoutFloorPlan";

        public const string SPSAVEPRODUCTSELECTION = "usp_SaveUpdateProductSelection";
        public const string SPGETVARIABLEVALUESFORVIEW = "usp_GetViewExportParmDetails";
        public const string OPPORTUNITY_ID = "OpportunityId";
        public const string QUOTECOUNT = "QuoteCounts";
        public const string DUPLICATEBUILDINGEQUIPMENTCONSOLE = "usp_DuplicateBuildingEquipmentConsole";
        public const string DELETEBUILDINGEQUIPMENTCONSOLE = "usp_DeleteBuildingEquipmentConsole";
        public const string FNGETBUILDINGEQUIPMENTBYBUILDINGID = "usp_GetBuildingTab";
        public const string SPGSAVEFIELDDRAWINGAUTOMATIONBYGROUPID = "usp_SaveFieldDrawingAutomation";
        public const string SPGSAVEFDABYGROUPID = "usp_SaveFDA";
        public const string SPSAVESENDTOCOORDINATION = "usp_SaveSendToCoordination";
        public const string GETGROUPCONFIGURATIONBYGROUPID = "usp_GetGroupTab";

        public const string USPSAVEWRAPPERFIELDDRAWINGAUTOMATION = "usp_SaveWrapperFieldDrawingAutomation";
        public const string USPSAVEWRAPPERFIELDLAYOUT = "usp_SaveWrapperFieldLayout";
        public const string USPUPDATELOCKGROUPPROPERTYFORFDA = "usp_UpdateLockGroupPropertyForFDA";
        public const string USPUPDATEFDADRAWINGMETHODBYGROUPID = "usp_UpdateFDADrawingMethodByGroupId";
        public const string USPUPDATESTATUSFORFDA = "usp_UpdateFDAStatus";

        public const string USPGETQUOTEDETAILS = "usp_getQuoteDetails";
        public const string USPGETBRANCHID = "usp_GetBranchId";
        public const string USPGETPROJECTIDVERSIONID = "usp_GetProjectIdVersionId";
        public const string USPGETREQUESTEDDRAWING = "usp_GetRequestedDrawing";
        public const string USPUPDATEFDAREQUESTSTATUS = "usp_UpdateFDARequestStatus";




        public const string SPGETUNITDETAILSFORVARIABLEASSIGNMENTS = "usp_GetUnitVariableAssignments";
        public const string SPCHECKUNITSET = "usp_CheckUnitSet";
        public const string SPSAVEUNITCONFIGURATION = "usp_SaveUnitConfiguration";
        public const string SPSAVEENTRANCECONFIGURATION = "usp_SaveEntanceConfiguration";
        public const string SPDELETEFLOORPLAN = "sp_DeleteFloorPlan";
        public const string SPSAVEFLOORPLAN = "sp_SaveFloorPlanForGroupConfiguration";
        public const string SPUPDATEFLOORPLAN = "sp_UpdateFloorPlanForGroupConfiguration";
        public const string SPSAVEGROUPHALLFIXTURE = "usp_SaveUpdateGroupHallFixtures";
        public const string SPGETBUILDINGEQUIPMENTCONSOLECONFIGURATION = "usp_GetBuildingEquipmentConsoleConfiguration";
        public const string SPSAVECARCALLCUTOUTKEYSWITCHOPENING = "usp_SaveCarCallOutKeyswitchLocations";
        public const string SPSAVEPRICEDETAILS = "SavePriceDetails";

        public const string SAVEUNITCONFIGURATIONFORNCP = "usp_SaveUnitConfigurationForNCP";

        public const string SPSAVERELEASETOMANUFACTURE = "usp_SaveUpdateReleaseInfo";

        public const string SPGETMINIPROJECTVALUES = "usp_GetMiniProjectValues";

        public const string GENERALINFORMATION = "GENERALINFORMATION";
        public const string CABINTERIOR = "CABINTERIOR";
        public const string HOISTWAYTRACTIONEQUIPMENT = "HOISTWAYTRACTIONEQUIPMENT";
        public const string ENTRANCES = "ENTRANCES";

        public static readonly string SETID = "@setId";
        public static readonly string DATE = "@date";
        public static readonly string SETID1 = "@SetId";
        public static readonly string FDAGROUPID = "@groupId";
        public static readonly string FDACREATEDDBY = "@CreatedBy";
        public static readonly string FDAGROUPVARIABLES = "@groupVariables";
        public static readonly string FDARESULT = "@FdaResult";
        public static readonly string ISLOCK = "@islock";
        public static readonly string FDADRAWINGMETHOD = "@drawingMethod";
        public static readonly string COORDINATIONDATA = "@coordinationData";
        public static readonly string ISJAMBMOUNTED = "@isJambMounted";
        public static readonly string @CONTROLLANDING = "@controlLanding";
        public static readonly string @DEFAULTUHFCONFIGURATION = "@defaultUHFConfiguration";
        public static readonly string FDAGUID = "@guid";


        public const string TRADITIONALHALLSTATION = "Traditional_Hall_Stations";

        public const string AGILEHALLSTATION = "AGILE_Hall_Stations";
        public const string F = "F";
        public const string R = "R";

        public static readonly string CREATEDBYVALUE = "@CreatedBy";

        public static readonly string PROJECTIDVALUES = "@projectId";

        public static readonly string GROUPCONFIGIDVALUE = "GroupConfigurationId";
        public static readonly string CONSOLEID = "@consoleId";
        public static readonly string ATCONSOLENUMBER = "@ConsoleNumber";
        public static readonly string CONFIGUREJSON = "ConfigureJson";
        public const string SPGETUNITCONFIGURATIONBYGROUPID = "usp_GetUnitConfigurationById";
        public static readonly string CONSOLESID = "@ConsoleId";
        public const string SPGETCARCUTOUTSAVEDOPENINGS = "[usp_GetCarCutoutSavedOpenings]";

        public const string SPGETBUILDINGEQUIPMENTCONFIGURATIONBYID = "usp_GetBuildingEquipmentConfigurationById";

        public const string SPGGETFIELDDRAWINGAUTOMATIONBYGROUPID = "usp_GetFieldDrawingAutomationByGroupId";

        public const string SPGGETSUMPPITQUANTITYBYGROUPID = "usp_GetSumPitQuantity";

        public const string SPGGETGROUPINFORMATION = "usp_GetGroupInformation";

        public const string SPGGETCARPOSITIONBYGROUPID = "usp_GetCarPositionByGroupId";

        public const string SPWRAPPERAPIXMLGENERATIONBYGROUPID = "usp_WrapperAPIXmlGeneration";

        public const string SPGETOPPORTUNITYANDVERSIONIDBYQUOTEID = "usp_GetOpportunityAndVersionIdByQuoteId";

        public const string SPWRAPPERGETUNITVARIABLEASSIGNMENTSBYGROUPID = "usp_GetUnitVariableAssignmentsForWrapper";

        public const string SPWRAPPERGETBUILDINGANDGROUPVARIABLEASSIGNMENTSBYGROUPID = "usp_GetBuildingGroupVariableAssignmentsForWrapper";

        public const string SPGETBUILDINGGROUPUNITVARIABLEASSIGNMENTS = "usp_GetBuildingGroupandUnitVariableAssignments";

        public const string SPCHECKREQUESTIDBYFDAINTEGRATIONID = "usp_CheckRequestIdByFDAIntegrationId";

        public const string SPCHECKHANGFIRERECURRINGJOB = "usp_CheckHangFireRecurringJob";

        public const string SPGETVARIABLESFORLIFTDESIGNER = "usp_GetVariablesForLiftDesigner";

        public const string SPGETOUTPUTTYPESFORXMLGENERATION = "usp_GetOutputTypesForXMLGeneration";

        public const string SPGETFDASTATUSFORREFRESH = "usp_GetFDAStatusForRefresh";

        public const string SPGETGROUPSTATUSFORFDA = "usp_GetGroupStatusForFDA";

        public const string SPGETPROJECTSTATUSFORFDA = "usp_GetProjectStatusForFDA";

        public const string SPGETDRAWINGSTATUSFORFDA = "usp_GetDrawingStatusForFDA";

        public const string SPGETFDAQUOTEIDBYGROUPID = "usp_GetFDAQuoteIdByGroupId";



        public const string SPGETPRODUCTCATEGORYBYGROUPID = "usp_GetProductCategoryByGroupId";

        public const string SPGETGROUPINFODETAILSBYGROUPID = "usp_GetGroupInfoDetailsByGroupId";

        public const string SPGETUNITVARIABLEASSIGNMENTSBYSETID = "usp_GetUnitVariableAssignmentsBySetId";

        public const string SPGETDRAWINGS = "usp_GetDrawings";
        public const string SPGETSENDTOCORDINATIONSTATUS = "usp_GetSendToCoordinationStatus";

        public const string SPGETRELEASEINFO = "usp_GetReleaseInfo";

        public const string SPGETRELEASETOMANUFACTURE = "usp_GetReleaseInfoByGroupId";

        public const string SPGETDRAWINGSBYGROUPIDFORREFRESH = "usp_GetDrawingsByGroupIdForRefresh";

        public const string GETREQUESTQUEUEDETAILS = "usp_GetRequestQueueDetails";

        public const string SPGETSENDTOCOORDINATION = "usp_GetSendToCoordination";

        public static readonly string FIXTURETYPE = "@fixtureType";

        public static readonly string ATFIXTURETYPE = "@FixtureType";

        public const string REGEN = "ELEVATOR.Parameters.Electrical_System.REGEN";

        public const string TRUE_LOWERCASE = "True";
        public const string FALSE_LOWERCASE = "False";
        public const string TRUE_UPPERCASE = "TRUE";
        public const string FALSE_UPPERCASE = "FALSE";
        public const string TRUE_FULL_LOWERCASE = "true";
        public const string FALSE_FULL_LOWERCASE = "false";


        public const string SPGETLOGHISTORYUNIT = "[usp_GetUnitLogHistory]";
        public const string SPGETLOGHISTORYBUILDING = "[dbo].[usp_GetBuildingLogHistory]";
        public const string SPGETLOGHISTORYGROUP = "[dbo].[usp_GetGroupLogHistory]";

        public static readonly string SPGETLISTOFPROJECTS = "usp_GetListOfProjects";

        public const string SPGETPRODUCTTYPE = "usp_GetProductType";



        //LoginDL
        public const string SPGETUSERINFO = "usp_GetUserInfo";
        public const string SPGETUSERDETAILS = "usp_GetUserDetails";

        public const string LOCATIONID = "locationId";
        public const string LOCATIONNAME = "locationName";
        public const string CITY = "city";
        public const string STATE = "state";
        public const string COUNTRY = "country";
        public const string ROLEKEY = "roleKey";
        public const string USERROLENAME = "roleName";

        //ProductDL
        public const string SPGETPRODUCTLINEDETAILS = "usp_GetProductLineDetails";

        //ProjectsDL
        public const string SPGETLISTOFPROJECTSFORUSER = "usp_GetListOfProjectsForUser";
        public const string SPSEARCHUSER = "usp_SearchUser";

        //Group ConfigurationDL
        //public static readonly string SaveGoupConfigdetails = "usp_InsertGroupConfiguration";
        public const string SPGETGROUPCONFIGBYGROUPID = "usp_GetGroupConfigurationByGroupId";


        public const string SPGETGROUPCONFIGBYBUILDINGID = "usp_GetGroupConfigurationByBuildingId";
        public const string SPGETGROUPLAYOUTCONFIGURATIONID = "usp_GetGroupLayoutConfiguration_Conflicts";

        //Generate Building Name
        public const string SPGETNUMBEROFBUILDINGS = "usp_GetNumberOfBuildings";

        //Generate Group Name
        public const string SPGETNUMBEROFGROUPS = "usp_GetNumberOfGroups";

        //Generate Product Category
        public const string SPGETPRODUCTCATEGORY = "usp_GetProductCategory";

        //Get Floor designation and floor number by group Id

        public const string SPGETFLOORDESIGNATIONBYGROUPID = "usp_GetFloorDesignationByGroupId";

        public const string SPGETFIXTURESLIST = "usp_GetUnitHallFixtureTypes";

        public const string SPGETGROUPFIXTURESLIST = "usp_GetGroupHallFixtureTypes";


        //Unit ConfigurationDL
        public const string SPSAVECABINTERIORDETAILS = "usp_SaveCabInterior";
        public const string SPSAVEHOISTWAYTRACTIONEQUIPMENT = "usp_SaveHoistwayTractionEquipment";
        public const string SPSAVEENTRANCES = "usp_SaveEntrance";
        public const string SPEDITUNITDESIGNATION = "usp_EditUnitDesignation";

        public const string SPGETGENERALINFORMATIONBYGROUPID = "usp_GetGeneralInformationByGroupId";

        public const string SPGETCABINTERIORGROUPID = "usp_GetCabInteriorByGroupId";

        public const string SPGETHOSITWAYTRACTIONEQUIPMENTBYGROUPID = "usp_GetHositwayTractionEquipmentByGroupId";

        public const string SPGETENTRANCEBYGROUPID = "usp_GetEntranceByGroupId";

        public const string SPGETENTRANCEBYSETID = "usp_GetEntranceConfigurationBySetId";

        public const string SPSAVEGENERALINFORMATION = "usp_SaveGeneralInformation";

        public const string SPGETFIXTURESTRATEGY = "usp_GetFixtureStrategy";

        public const string SPGETHALLLANTERNCONFIGURATION = "usp_GetHallLanternConfiguration";
        public const string SPGETGROUPHALLFIXTURE = "usp_GetGroupHallFixture";
        public const string SPGETUNITDETAILSFORGROUPHALLFIXTURE = "usp_GetUnitAndFloorById";
        public const string GENERALINFORMATIONSAVEMESSAGE = "General Information Saved Successfully";
        public const string GENERALINFORMATIONUPDATEMESSAGE = "General Information Updated Successfully";
        public const string GENERALINFORMATIONERRORMESSAGE = "Error while Saving General Information";
        public const string GENERALINFORMATIONUPDATEERRORMESSAGE = "Error while Updating General Information";

        public const string SPSAVEUNITHALLFIXTURECONFIGURATION = "usp_SaveUnitHallFixtureConfiguration";
        public const string SPSPGETHALLLANTERNCONFIGURATION = "usp_GetHallLanternConfiguration";

        public const string SPDELETEENTRANCECONSOLE = "usp_DeleteEntranceConsole";

        public const string SPGETCARCALLCUTOUTKEYSWITCHOPENINGS = "usp_GetCarCallCutoutKeyswitchOpenings";
        public const string SPGETMAINEGRESS = "usp_GetMainEgress";
        public const string SPGETVALUESFORVIEWEXPORT = "usp_GetVariableValuesForVIEW";
        public const string SPGETVARIABLESBYQUOTEID = "usp_GetVariableByQuoteId";

         public static readonly string COORDINATIONQUESTIONSSTUBPATH = @"Templates\Integration\SendToCoordinationQuestions.json";

        public static readonly string STARTFIELDDRAWINGAUTOMATIONMAINTEMPLATEPATH = @"Templates\FieldDrawingAutomation\UIResponse.json";



        public const string USPSAVEPROJECTVALUES = "usp_CreateUpdateProject";
        public const string DELETEPROJECTIDINITIATE = "usp_DeleteProjectById";

        public static readonly string SYSTEMVALIDATERESPONSESTUB = @"Templates\Unit\{0}\SystemValidation\Parameters.json";

        public const string USPSETPRIMARYQUOTES = "usp_SetPrimaryQuotes";
        public const string DUPLICATEQUOTEBYQUOTEID = "usp_DuplicateQuoteByQuoteId";
        public const string SPGETBUILDINGCONFLICTS = "usp_SaveConflicts";


        #endregion

        #region Return Messages Related Constants
        public const string GROUPSAVEMESSAGE = "Group Saved Successfully";
        public const string BUILDINGSAVEMESSAGE = "Building Saved Successfully";
        public const string UPDATEMESSAGE = "Building Updated Successfully";
        public const string BUILDINGELEVATIONSUCCESSMESSAGE = "Building Elevation saved Successfully";
        public const string BUILDINGELEVATIONERRORMESSAGE = "Error occured while saving building elevation";
        public const string AUTOSAVEBUILDINGELESUCCESSMSG = "Auto Save Building Elevation  Successfull";
        public const string AUTOSAVEBUILDINGELEERRORMSG = "Error occured while saving building elevation";
        public const string BUILDINGELEVATIONUPDATEMSG = "Building Elevation Updated Successfully";
        public const string BUILDINGDELETIONSUCCESSMSG = "Building Deletion Successfull";
        public const string BUILDINGDELETIONERRORMSG = "Building Deletion not Successfull";
        public const string BUILDINGNOTFOUND = "Requested building is not found";
        public const string BUILDINGELEVATIONDELETIONSUCCESSMSG = "Building Elevation Deleted Successfully";
        public const string BUILDINGELEVATIONDELETIONERRORMSG = "Error Occured while deleting building elevation";
        public const string BUILDINGELEVATIONUPDATEERRORMSG = "Error occured while Update building elevation";
        public const string OPENINGLOCATIONUPDATEMESSAGE = "Opening Location Updated Successfully";
        public const string BUILDINGNAMEEXISTS = "Building Name already existing. Please provide a unique building Name";
        public const string BUILDINGSAVEISSUES = "Building Has EVO_200 Models. Please Change the Building Power Phase to 3";
        public const string AUTOSAVEMESSAGE = "Auto Save successful";
        public const string AUTOSAVEDELETEMESSAGE = "Auto Save details deleted successfully";
        public const string AUTOSAVEDELETEERRORMESSAGE = "No configuration exists for the given username";
        public const string ERRORSAVEMESSAGE = "Unit names should be unique";
        public const string ERRORREQUESTMESSAGE = "Requested SetId Not Found";

        public const string ERRORBUILDINGIDINCORRECTMESSAGE = "Building Id is Incorrect";
        public const string UNITNAMEREPEATING = "Unit Name/s = already used. Please enter unique names";
        public const string ERRORGROUPNAMEXISTS = "Group Name already existing. Please provide a unique group Name";
        public const string GROUPUPDATEMESSAGE = "Group Updated Successfully";
        public const string ERRORUPDATEMESSAGE = "Group Configuration doesnot exists";
        public const string GROUPLAYOUTSAVEMESSAGE = "Group Layout Saved Successfully";
        public const string GROUPLAYOUTUPDATEMESSAGE = "Group Layout Updated Successfully";
        public const string GROUPLAYOUTCONTROLROOMERRORMESSAGE = "Control Floor is mandatory";
        public const string CABINTERIORSAVEMESSAGE = "Cab Interior Saved Successfully";
        public const string CABINTERIORERRORMESSAGE = "Error while Saving Cab Interior";
        public const string CABINTERIORUPDATEMESSAGE = "Cab Interior Updated Successfully";
        public const string CABINTERIORUPDATEERRORMESSAGE = "Error while Updating Cab Interior";
        public const string HOISTWAYTRACTIONEQUIPMENTSAVEMESSAGE = "Hoistway Traction Equipment Saved Successfully";
        public const string HOISTWAYTRACTIONERRORMESSAGE = "Error while saving Hoistway Traction Equipment";
        public const string HOISTWAYTRACTIONEQUIPMENTUPDATEMESSAGE = "Hoistway Traction Equipment Updated Successfully";
        public const string HOISTWAYTRACTIONUPDATEERRORMESSAGE = "Error while Updating Hoistway Traction Equipment";
        public const string ENTRANCESAVEMESSAGE = "Entrances Saved Successfully";
        public const string ENTRANCESAVEERRORMESSAGE = "Error while Saving Entrances";
        public const string ENTRANCEUPDATEMESSAGE = "Entrances Updated Successfully";
        public const string ENTRANCEUPDATEERRORMESSAGE = "Error while Updating Entrances";
        public const string ADDITIONALINFORMATIONGROUPCONFIGURED = "Unit configured";
        public const string ADDITIONALINFORMATIONGROUPNOTCONFIGURED = "Unit not yet configured";
        public const string UNITCONFIGURATIONSAVEMESSAGE = "Unit Configuration Saved Successfully";
        public const string UNITCONFIGURATIONSAVEERRORMESSAGE = "Error while Saving Unit Configuration";
        public const string UNITCONFIGURATIONUPDATEMESSAGE = "Unit Configuration Updated Successfully";
        public const string GROUPHALLFIXTURESAVEMESSAGE = "GroupHallFixture Saved Successfully";
        public const string GROUPHALLFIXTURESAVEERRORMESSAGE = "Error while Saving GroupHallFixture";
        public const string GROUPHALLFIXTURESAVEFIRESERVICEERRORMESSAGE = "At least one opening should be selected for each unit or hall station";
        public const string GROUPHALLFIXTURESAVETRADITIONALAGILEERRORMESSAGE = "At least one hall station is mandatory for Traditional and Agile Hall stations each.";
        public const string UNITCONFIGURATIONUPDATEERRORMESSAGE = "Error while Updating Unit Configuration";
        public const string REQUESTGENERATEDSUCCESSFULLY = "Request Generated Sucessfully.";
        public const string USERNOTFOUND = "User Not Found.";
        public const string DELETEFLOORPLANSUCCESSMSG = "Floor plan is deleted successfully";
        public const string DELETEFLOORPLANERRORMSG = "Floor plan is not deleted";
        public const string SOMEFIELDSMISSINGMSG = "some fields missing";
        public const string SAVEFLOORPLANSUCCESSMSG = "FloorPlan saved Successfull";
        public const string SAVEFLOORPLANERRORMSG = "Error occured while saving floorplan";
        public const string UPDATEFLOORPLANSUCCESSMSG = "FloorPlan Updated Successfully";
        public const string UPDATEFLOORPLANERRORMSG = "Error occured while updating floorplan";
        public const string DELETEGROUPCONFIGSUCCESSMSG = "Group Configuration deleted successfully";
        public const string DELETEGROUPCONFIGERRORMSG = "Error occurred while deleting Group Configuration";
        public const string DELETEENTRANCECONSOLESUCCESSMSG = "Entrance console deleted successfully";
        public const string DELETEENTRANCECONSOLEERRORMSG = "Error occurred while deleting entrance console";

        public const string RELEASEINFOSAVEUPDATEMESSAGE = "Details saved successfully";
        public const string RELEASETOMANUFACTURINGMESSAGE = "Group released successfully";
        public const string RELEASEINFOSAVEUPDATEERRORMESSAGE = "Error occurred while saving release details";

        public const string DELETEUNITHALLFIXTURECONSOLESUCCESSMSG = "Fixture deleted successfully";
        public const string DELETEUNITHALLFIXTURECONSOLEERRORMSG = "Error occurred while deleting UnitHallFixture console";
        public const string RESETUNITHALLFIXTURECONSOLESUCCESSMSG = "UnitHallFixture console reset successfully";

        public const string DELETEGROUPHALLFIXTURECONSOLESUCCESSMSG = "Group Hall Fixture console deleted successfully";
        public const string DELETEGROUPHALLFIXTURECONSOLEERRORMSG = "Error occurred while deleting Group Hall Fixture console";
        public const string BUILDING = "Building ";
        public const string GROUP = "Group ";
        public const string CREATEDSUCCESSFULLY = " is created successfully";
        public const string VARIABLEKEY = "VariableKey";

        public const string FDASAVEMESSAGE = "Field Drawings Details Saved Successfully";
        public const string FDAERRORSAVEMESSAGE = "Something Went Wrong";
        public const string FDAUPDATELOCKSUCCESS = "Group Locked successfully";
        public const string FDAUPDATEUNLOCKSUCCESS = "Group Unlocked successfully";
        public const string SOMETHINGWENTWRONG = "Something Went Wrong";
        public const string SENDTOCOORDINATIONSAVEMESSAGE = "Group Details Saved Successfully";

        public const string PROJECTUPDATEMESSAGE = "Project save/updated successfully";
        public const string PROJECTERRORSAVEMESSAGE = "Project save/update was unsuccessful";

        public const string PRICEDETAILSSAVEMESSAGE = "Price Details saved successfully";
        public const string PRICEDETAILSSAVEERROR = "Error while saving price details";

        public const string SETPRIMARYMESSAGE = "Quote ID Successfully Set as Primary";

        public const string SETPRIMARYERRORMESSAGE = "Set Quote as Primary is Failed";



        public const string QUOTEDUPLICATESUCESSMESSAGE = "Duplicate Quotes created successfully";
        public const string QUOTEDUPLICATEERRORSAVEMESSAGE = "Duplicate Quotes creation was unsuccessful";
        public static readonly string DUPLICATEOVERWRITE = "Cannot Duplicate Existing Version Under This Project ";
        public const string VARIABLEIDVALUES = "VariableId";
        public const string BUILDINGCONFLICTSVARIABLES = "Building Conflicts Saved Successfull";
        public const string BUILDINGCONFLICTSVARIABLESISSUES = "Building Conflicts not Successfull";

        #endregion

        #region Tests Cases Related Constants

        //  public static readonly string APPGATEWAY_INPUTJSON_PATH = "../../../../TKE.CPQ.AppGateway.Test";
        public static readonly string APPGATEWAY_INPUTJSON_PATH = "../../../../UnitTests";
     //   public static readonly string INPUTJSONPATH = "../TKE.CPQ.AppGateway.Test/InputJson/";
        public static readonly string INPUTJSONPATH = "../UnitTests/InputJson/";
        public static readonly string VALIDATEUSERVALUES = INPUTJSONPATH + "UserInfoValidate.json";
        public static readonly string STARTGROUPCONFIGURATIONREQUESTBODY = INPUTJSONPATH + "StartGroupConfigRequestBody.json";
        public static readonly string GETGROUPCONFIGRESPONSE = INPUTJSONPATH + "GetConfigResponse.json";
        public static readonly string GETVARIABLEASSIGNMENTSOBOMRESPONSE = INPUTJSONPATH + "GetVariableAssignmentsObom.json";
        public static readonly string OZREQUESTBODY = "Templates/Integration/OzRequestPayload.json";

        public static readonly string GETBUILDINGCONFIGBYIDSTUBRESPONSEBODY = INPUTJSONPATH + "GetBuildingConfigByIdStubResponseBody.json";
        public static readonly string STARTGROUPCONFIGURATIONRESPONSEBODY = INPUTJSONPATH + "StartGroupConfigurationResponseBody.json";
        public static readonly string GETBUILDINGCONFIGBYIDSTUBRESPONSEBODY1 = INPUTJSONPATH + "GetBuildingConfigByIdStubResponseBody1.json";
        public static readonly string STARTBUILDINGCONFIGRESPONSEBODY = INPUTJSONPATH + "BuildingConfigurationResponse.json";
        public static readonly string CHANGEGROUPLAYOUTRESPONSEBODY = INPUTJSONPATH + "ChangeGroupConfigLayoutResponseBody.json";
        public static readonly string CHANGEPRODUCTSELECTIONSTUBRESPONSEBODY = INPUTJSONPATH + "ChangeProductSelectionStubResponse.json";
        public static readonly string UNITCONFIGURATIONRESPONSE = INPUTJSONPATH + "UnitConfigurationResponse.json";

        public static readonly string USERDETAILS = INPUTJSONPATH + "USERDETAILS.json";

        #endregion

        #region DB Column name constans
        public const string ENTRANCECONSOLEID = "EntranceConsoleId";

        public const string ENTRANCECONSOLENAME = "EntranceConsoleName";

        public const string ENTRANCECONSOLEWITHCONTROLLER = "EntranceConsoleWithController";

        public const string ENTRANCECONSOLEONE = "EntranceConsole1";
        public const string ASSIGNOPENINGS = "AssignOpenings";

        public const string VARIABLETYPE = "VariableType";

        public const string VARIABLEVALUE = "VariableValue";

        public const string FDATYPE = "FDAType";

        public const string FDAVALUE = "FDAValue";
        public const string LOBBYRECALLSWITCHVARAIBLES = "LOBBYRECALLSWITCHVARAIBLES";

        public const string FDALAYOUTGENERATIONSETTINGSID = "FDALayoutGenerationSettingsId";
        public const string FDALAYOUTNAME = "FDALayoutName";
        public const string FDALAYOUTGENERATIONSETTINGSTYPE = "FDALayoutGenerationSettingsType";
        public const string FDALAYOUTGENERATIONSETTINGSVALUE = "FDALayoutGenerationSettingsValue";


        public static readonly string BUILDINGIDFDA_CAMELCASE = "BuildingId";
        public static readonly string BUILDINGNAME_CAMELCASE = "BuildingName";
        public static readonly string GROUPIDRLSINFO_CAMELCASE = "GroupId";
        public static readonly string GROUPNAMERLSINFO_CAMELCASE = "GroupName";
        public static readonly string ALLSELECTEDCAMELCASE = "AllSelected";
        public static readonly string UNITIDLOWERCASE = "unitid";
        public static readonly string UNITNAMELOWERCASE = "unitname";
        public static readonly string SETIDLOWERCASE = "Setid";
        public static readonly string SETIDPASCALCASE = "setId";
        public static readonly string SYSTEMVARIABLESKEYS = "SystemVariableKeys";
        public static readonly string SYSTEMVARIABLESVALUES = "SystemVariableValues";


        public static readonly string RELEASECOMMENTSLOWERCASE = "ReleaseComments";
        public static readonly string DATAPOINTVAR_CAMELCASE = "ConfigureVariables";
        public static readonly string DATAPOINTVALUE_CAMELCASE = "ConfigureValues";
        public static readonly string ISACKNOWLEDGE_CAMELCASE = "isAcknowledge";
        public static readonly string SENDTOCOORDINATION = "SendToCoordination";
        public static readonly string ISPRIMARYQUOTE = "isPrimaryQuote";
        public static readonly string RELEASEINFO = "ReleaseInfo";
        public static readonly string FDA = "FDA";
        public static readonly string RELEASETOMANUFACTURING = "ReleaseToManufacturing";
        public static readonly string DISTINCTDATAPOINT_CAMELCASE = "distinctDataPoints";
        public static readonly string QUERYIDLOWERCASE = "queryId";
        public static readonly string QUERYNAMELOWERCASE = "queryName";
        public static readonly string ISACKNOWLEDGELOWERCASE = "isAcknowledge";


        public const string VARIABLENAME = "@VarName";
        public const string FLOORNUMBER = "FloorNumber";
        public const string ISSAVED = "isSaved";
        public const string NOOFFLOOR = "nooffloors";
        public const string FLOORDESIGNATION = "FloorDesignation";
        public const string FRONT = "Front";
        public const string QUANTITY = "Quantity";
        public const string ENABLESENDTOCOORDINATION = "enableSendToCoordination";
        public const string FLOOR = "Floor";
        public const string REAR = "Rear";
        public const string ELEVAIONFEET = "ElevationFeet";
        public const string OPENINGFRONT = "OpeningFront";
        public const string OPENINGREAR = "OpeningRear";
        public const string ZEROZEROONE = "001";
        public const string ZEROZEROTWO = "002";
        public const string ELEVAIONINCH = "ElevationInch";
        public const string ISCONTROLLER = "IsController";
        public const string PRODUCTNAME1 = "productName";
        public const string OPENINGREARS = "openingRear";
        public const string REARLOWER = "rear";
        public const string FRONTLOWER = "front";
        public const string SIDELOWER = "side";
        public const string FLOORNUMBERCAMELCASE = "floorNumber";
        public static readonly string OCUPPIEDSPACEBELOW = "OcuppiedSpaceBelow";

        public const string EVO200 = "EVO_200";
        public const string EVO100 = "EVO_100";
        public const string ENDURA200 = "ENDURA_200";
        public const string ENDURA100 = "ENDURA_100";
        public static readonly string EVOLUTION200 = "Evolution200";
        public const string UNITHALLFIXTURECONSOLEID = "UnitHallFixtureConsoleId";
        public const string UNITHALLFIXTURECONSOLENAME = "UnitHallFixturenConsoleName";
        public const string UNITHALLFIXTURETYPELIST = "UnitHallFixtureType";

        public const string GROUPHALLFIXTURECONSOLEID = "GroupHallFixtureConsoleId";
        public const string GROUPHALLFIXTURECONSOLENAME = "GroupHallFixturenConsoleName";
        public const string GROUPHALLFIXTURETYPELIST = "GroupHallFixtureType";

        public const string LOBBYRECALLSWITCHFLAG = "LOBBYRECALLSWITCHFLAG";
        public const string CARRIDINGLANTERNQUANT = "CARRIDINGLANTERNQUANT";

        public const string FIXTURESTRATEGYVARIABLE = "FixtureStrategy";
        public const string BECONSOLEID = "ConsoleId";
        public const string CONSOLENUMBER = "ConsoleNumber";
        public const string CONSOLENAME = "ConsoleName";
        public const string ISLOBBY = "IsLobby";
        public const string ASSIGNEDGROUPS = "AssignedGroups";
        public const string ASSIGNEDUNITS = "AssignedUnits";
        public const string GROUPIDLOWERCASE = "groupId";

        public const string GROUPSTATUSID = "grpStatusId";
        public const string GROUPSTATUSKEY = "grpStatusKey";
        public const string GROUPDISPLAYNAME = "grpDisplayName";
        public const string GROUPSTATUSNAME = "grpStatusName";
        public const string GROUPDESCRIPTION = "grpDescription";
        public const string NAMECOLUMN = "Name";

        public const string DRAWINGSTATUSID = "drawingStatusId";
        public const string DRAWINGSTATUSKEY = "drawingStatusKey";
        public const string DRAWINGDISPLAYNAME = "drawingDisplayName";
        public const string DRAWINGSTATUSNAME = "drawingStatusName";
        public const string DRAWINGDESCRIPTION = "drawingDescription";


        public const string GROUPNAMELOWERCASE = "GroupName";
        public const string MANUALINFOMESSAGE = "ManualInfoMessage";
        public const string PRODUCTKEY = "productKey";
        public const string UNITCOUNTLOWERCASE = "UnitCount";
        public const string IS_CHECKED = "is_Checked";
        public const string PRODUCTCATEGORY_CAMELCASE = "productCategory";
        public const string TOTALUNITS = "totalUnits";
        public const string TOTALGROUPS = "totalGroups";
        public const string GROUPCATEGORYID = "GroupCategoryId";
        public const string GROUPCATEGORYNAME = "groupCategoryName";
        public const string NOOFUNITS = "noOfUnits";
        public const string CONSOLENAMELOWERCASE = "consolename";
        public const string QUESTIONS = "questions";
        public const string IS_SAVED = "isSaved";
        public const string PROJECTOPPORTUNITYID = "OpportunityId";
        public const string PROJECTNAME = "Name";
        public const string PROJECTBRANCHNAME = "BranchName";
        public const string PROJECTSALESMAN = "SalesMan";
        public const string PROJECTSTATUSKEY = "StatusKey";
        public const string PROJECTPROJSTATUSNAME = "ProjStatusName";
        public const string PROJECTPROJSTATUSDESCRIPTION = "ProjStatusDescription";
        public const string PROJECTPROJSTATUSDISPLAYNAME = "ProjStatusDisplayName";
        public const string PROJECTCREATEDON = "CreatedOn";
        public const string PROJECTMODIFIEDON = "ModifiedOn";
        public const string PROJECTQUOTECOUNTS = "QuoteCounts";
        public const string PROJECTQUOTEID = "QuoteId";
        public const string PROJECTVERSIONID = "VersionId";
        public const string PROJECTDESCRIPTION = "Description";
        public const string PROJECTQUOTESTATUSKEY = "QuoteStatusKey";
        public const string PROJECTQUOTESTATUSNAME = "QuoteStatusName";
        public const string PROJECTQUOTESTATUSDESCRIPTION = "QuoteStatusDescription";
        public const string PROJECTQUOTESTATUSDISPLAYNAME = "QuoteStatusDisplayName";
        public const string PROJECTUSD = "USD";
        public const string PROJECTUNITSCOUNTINQUOTES = "UnitsCountInQuotes";
        public const char PROJECTSPACECHARATER = '\r';
        public const string PROJECTCOLUMNOPPORTUNITYID = "OpportunityId";
        public const string PROJECTCOLUMNNAME = "Name";
        public const string PROJECTCOLUMNBRANCHVALUE = "BranchValue";
        public const string PROJECTCOLUMNSALESID = "SalesId";
        public const string PROJECTCOLUMNQUOTESTATUS = "QuoteStatus";
        public const string PROJECTCOLUMNDESCRIPTION = "Description";
        public const string PROJECTCOLUMNBUSINESSLINE = "BusinessLine";
        public const string PROJECTCOLUMNMEASURINGUNIT = "MeasuringUnit";
        public const string PROJECTCOLUMNSALESMAN = "Salesman";
        public const string PROJECTCOLUMNSALESMANACTIVEDIRECTORYID = "SalesmanActiveDirectoryID";
        public const string PROJECTCOLUMNCOUNTRY = "country";
        public const string PROJECTCOLUMNCREATEDBY = "CreatedBy";
        public const string PROJECTCOLUMNPROJECTJSON = "ProjectJson";
        public const string PROJECTCOLUMNVERSIONID = "VersionId";
        public const string PROJECTCOLUMNPRIMARYQUOTE = "isPrimaryQuote";
        public const string PROJECTCOLUMNVALUEINCOMPLETE = "Incomplete";
        public const string PROJECTCOLUMNVALUEMETRIC = "Metric";
        public const string PROJECTCOLUMNVALUEUS = "US";
        public const string CREATEPROJECTCOLUMNOPPORTUNITYID = "OpportunityId";
        public const string CREATEPROJECTCOLUMNTYPE = "Type";
        public const string CREATEPROJECTCOLUMNADDRESSLINE1 = "AddressLine1";
        public const string CREATEPROJECTCOLUMNADDRESSLINE2 = "AddressLine2";
        public const string CREATEPROJECTCOLUMNCITY = "City";
        public const string CREATEPROJECTCOLUMNSTATE = "State";
        public const string CREATEPROJECTCOLUMNCOUNTRY = "Country";
        public const string CREATEPROJECTCOLUMNZIPCODE = "ZipCode";
        public const string CREATEPROJECTCOLUMNCUSTOMERNUMBER = "CustomerNumber";
        public const string CREATEPROJECTCOLUMNACCOUNTNAME = "AccountName";
        public const string CREATEPROJECTCOLUMNAWARDCLOSEDATE = "AwardCloseDate";
        public const string PROJECTSTATUSCOLUMNNAME = "ProjectStatus";
        public const string PRIMARYQUOTEID = "PrimaryQuoteId";
        public const string PRICEFORQUOTE = "PriceForQuote";

        public const string TOTALOPENINGS = "totalOpenings";
        public const string EMPIVARIABLE = "EMPI";
        public const string COMBOFVARIABLE = "COMBOF";
        public const string HALLFINMAT = "HALLFINMAT";
        public const string RETURNVARIABLE = "RETURN";
        public const string CARCALLLOCKOUTVARIABLEID = "ELEVATOR.Parameters_SP.carCallLockout_SP";
        public const string CARCALLCUTOUTCOLUMN = "totalCarCallCutoutSwitches";
        public const string NULLSTRING = "NULL";
        public const string TotalNoOpenings = "TotalOpenings";
        public const string CONFIGURATIONCOMPLETE = "Configuration complete";
        public const string COMPLETE = "Complete";
        public static readonly string CONFIGURATION = "Configuration";
        public const string RELEASEINFODATAPOINTS = "ReleaseInfoDataPoints";
        public static readonly string DOCUMENTGENERATION = "DocumentGeneration";
        public static readonly string WALLTHICKNESSFRONT = "WallThicknessFront";
        public static readonly string WALLTHICKNESSREAR = "WallThicknessRear";
        public static readonly string FRAMECONSTRUCTIONFRONT="FrameConstructionFront";
        public static readonly string FRAMECONSTRUCTIONREAR = "FrameConstructionRear";
        public static readonly string FACEOFFRAMEFRONT="FaceOfFrameFront";
        public static readonly string FACEOFFRAMEREAR = "FaceOfFrameRear";
        public static readonly string FRAMEFINISHFRONT = "FrameFinishFront";
        public static readonly string FRAMEFINISHREAR = "FrameFinishRear";
        public static readonly string HOISTWAYSILLSFRONT = "HoistwaySillsFront";
        public static readonly string HOISTWAYSILLSREAR = "HoistwaySillsRear";
        public static readonly string HALLLANTERNTYPEFRONT= "HallLanternTypeFront";
        public static readonly string BRAILLETYPE = "BRAILLETYPE";
        public static readonly string DESIGNATIONPLATETYPE = "DESIGNATIONPLATETYPE";
        public static readonly string ILLUMINATIONTYPE = "ILLUMINATIONTYPE";
        public static readonly string HALLLANTERNTYPEREAR = "HallLanternTypeRear";
        public static readonly string HALLPIFINISH="HallPIFinish";
        public static readonly string PICOLOR="PICOLOR";
        public static readonly string HALLFINMAT_SP="HALLFINMAT_SP";
        public static readonly string HALLTARGETINDICATOR="HALLTARGETINDICATOR";
        public static readonly string COMBOFINISHFRONT="COMBOFINISHFRONT";
        public static readonly string COMBOFINISHREAR = "COMBOFINISHREAR";
        public static readonly string LANTERNSHAPEFRONT = "LANTERNSHAPEFRONT";
        public static readonly string LANTERNSHAPEREAR = "LANTERNSHAPEREAR";
        public static readonly string INCONFFRONT = "INCONFFRONT";
        public static readonly string INCONFREAR = "INCONFREAR";
        public const string NUMBEROFFRONTOPENINGS = "NumberFrontOpenings";
        public const string NUMBEROFREAROPENINGS = "NumberRearOpenings";
        public const string FLRHTF = "FLRHTF";
        public const string TOTALOPENING = "TotalOpening";
        public const string NUMBEROFUNITSINGROUP = "NumberOfUnitsInGroup";








        #endregion


        #region Constant Files Path

        public static readonly string APPSETTINGS = "appsettings.json";
        public static readonly string LISTVARIABLEASSIGNMENTS = "Templates/Unit/Evolution200/TP2Summary/ListOfVariableAssignments.json";



        public static readonly string CARPOSITION = "UnitTable";
        public static readonly string CARPOSITIONFILE = "SampleStubData/GroupConfiguration/CarPosition.json";
        public const string BUILDINGVARIABLESLIST = "buildingVariablesList";
        public const string GROUPVARIABLESLIST = "groupVariablesList";
        public const string UNITVARIABLESLIST = "unitVariablesList";
        public const string HOISTWAYACCESS = "Hoistway_Access";
        public const string BRAILLE = "Braille";
        public const string HALLLANTERN = "Hall_Lantern";
        public const string ETA = "ETA";
        public const string ETD = "ETD";

        #endregion

        public static readonly string GETPROJECTINFODLSTARTED = "GetProjectInfo DL call Started";
        public static readonly string GETPROJECTINFODLCOMPLETED = "GetProjectInfo DL call Completed";
        public const string DATETIMESTRING = "yyyy/MM/dd HH:mm:ss.ff ";
        public const string DATETIMESTRINGFORVIEWEXPORT = "MM/dd/yyyy HH:mm:ss";
        public const string GETPROJECTINFO = "GetProjectInfo";
        public static readonly string POSTCONFIGURATIONTOVIEWSTARTED = "PostConfiguration to view DL call Started";
        public static readonly string POSTCONFIGURATIONTOVIEWCOMPLETED = "PostConfiguration to view DL call Completed";
        public static readonly string GENARETAREQUESTBODYFORVIEWEXPORTSTARTED = "generate request body for view export DL call Started";
        public static readonly string GENARETAREQUESTBODYFORVIEWEXPORTCOMPLETED = "generate request body for view export dl call Completed";
        public const string DATEFORMAT = "MM/dd/yyyy";
        public const string TIMEFORMAT = "hh:mm tt";
        #region VIEW JSON FIELDS
        public static readonly string FACTORYMATERIALCOST = "FactoryMaterialCost";
        public static readonly string MATERIALCOST = "MaterialCost";
        public const string TOTALPRICE = "totalPrice";
        #endregion
        #region VIEW ERROR MESSAGE

        public static readonly string VIEWERROEMESSAGE = "Error encountered while getting project details from VIEW.";
        #endregion
        #region DOCUMENT GENERATION ERROR MESSAGE

        public static readonly string DOCUMENTGENERATIONERROEMESSAGE = "Error encountered while generating document.";
        #endregion

        #region HTTP METHODTYPE ENUM
        public enum HTTPMethodType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        #endregion

        #region TOKEN TYPE ENUM
        public enum TokenType
        {
            Basic,
            Bearer
        }
        #endregion

        #region PROJECTS

        public static readonly string PROJECTCONSTANTMAPPER = @"Templates\Integration\ConstantMapper.json";        
        public static readonly string PRODUCTREE = "productree";
        public static readonly string INTEGRATIONCONSTANTMAPPER = @"Templates\Integration\Constantmapper.json";
        public static string INVALID="Invalid";
        public static readonly string VIEWENRICHMENTS = "enrichments";
        public static readonly string PROJECTCOMMONNAME = "commonName";
        #endregion

        #region MAPPER FILES
        public static readonly string UNITSVARIABLESMAPPERPATH = @"Templates\Unit\ConstantMapper.json";
        public static readonly string UNITMAPPER = "UnitCommonMapper";
        public static readonly string GROUPMAPPER = "GroupCommonMapper";
        public static readonly string PRODUCTSELECTIONCONSTANTTEMPLATEPATH = @"Templates\ProductSelection\ConstantMapper.json";

        #endregion
        #region Product Selection
        public static readonly string PRODUCTSELECTIONCONSTANTPATH = @"Templates\ProductSelection\ConstantMapper.json";
        public static readonly string VARIABLES="Variables";
        #endregion
    }

}