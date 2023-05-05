CREATE procedure [dbo].[usp_GetVariableValuesForVIEW] --N'US-2021-00000085'
(
 @quoteId nvarchar(50),
 @buildingVariables as [VariableList] readonly ,
 @buildingEqmntVariables as [VariableList] readonly,
 @buildingEqmntConsoleVariables as [VariableList] readonly,
 --@groupVariables as [VariableList] readonly ,
 --@groupLayoutVariables as [VariableList] readonly,
 @controlLocationVariables as [VariableList] readonly,
 --@hallRiserVariables as [VariableList] readonly,
 @unitConfigurationVariables as [VariableList] readonly
 --@toBeCalculated as [VariableList] readonly
)
as
begin
	begin try
	 select BuildingId,BuindingType,BuindingValue from BuildingConfiguration
	  --where BuindingType in (select [VariableId] from @buildingVariables) and
	  where BuildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0) and IsDeleted=0
	 select BuildingId,Variabletype,[Value] from BldgEquipmentConfiguration
	  --where Variabletype in (select [VariableId] from @buildingEqmntVariables) and 
	  where BuildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0)
	   select BuildingId,bcc.ConsoleId,Variabletype,[Value] from BldgEquipmentConsoleCnfgn bcc Left Join BldgEquipmentConsole bc on bcc.ConsoleId= bc.ConsoleId
	  where --Variabletype in (select [VariableId] from @buildingEqmntConsoleVariables) and 
	  bcc.ConsoleId in (select ConsoleId from BldgEquipmentConsole where BuildingId in (select Id from Building
	  where QuoteId=@quoteId and IsDeleted=0))
	 --select BuildingId,GroupId,GroupConfigurationType,GroupConfigurationValue from GroupConfigurationDetails
	 -- where GroupConfigurationType in (select [VariableId] from @groupVariables)
	 -- and GroupId in (select GroupId from GroupConfiguration where BuildingId in (select BuildingId from Building
	 -- where OpportunityId=@opportunityId))
	 --select GroupConfigurationId,UnitId,UnitJson,SetId from Units
	 -- where GroupConfigurationId in (select GroupId from GroupConfiguration
	 -- where BuildingId in (select BuildingId from Building where OpportunityId=@opportunityId))
	 select BuildingId,GroupId,GroupName from GroupConfiguration where IsDeleted=0 and BuildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0)
	 select GroupConfigurationId,GroupName,BuildingId,ControlLocationType,ControlLocationValue from ControlLocation cl Left Join GroupConfiguration gc
	  on cl.GroupConfigurationId= gc.GroupId where --ControlLocationType in (select [VariableId] from @controlLocationVariables) and 
	  GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0)and IsDeleted=0)
	 --select GroupConfigurationId,UnitId,HallRiserType,HallRiserJson from HallRiser
	 -- where HallRiserType in (select [VariableId] from @hallRiserVariables)
	 -- and GroupConfigurationId in (select GroupId from GroupConfiguration
	 -- where BuildingId in (select BuildingId from Building where OpportunityId=@opportunityId))
	 select distinct BuildingId,us.GroupConfigurationId,isnull(us.CreatedOn,getdate()) as CreatedOn,us.UnitId,isnull(UEID,'') as UEID,isnull(us.SetId,'')as SetId,isnull(ust.ProductName,'')as ProductName ,isnull(us.designation,'') as designation,isnull(ol.TravelFeet,0) as TravelFeet,isnull(ol.TravelInch,0) as TravelInch, isnull(ol.FrontOpening,0) as FrontOpening,isnull(ol.RearOpening ,0) as RearOpening
	 ,isnull(ust.Description,'') as SetDescription ,Status.DisplayName as StatusName
	 from Units us
	 Left Join UnitSet ust on us.SetId= ust.SetId
	 Left Join GroupConfiguration gc on us.GroupConfigurationId=gc.GroupId
	 LEFT JOIN openinglocation ol ON ol.unitid = us.unitid AND ol.isdeleted = 0
	 left join  Status on us.WorkflowStatus=Status.StatusKey
	 where --ConfigureVariables in (select [VariableId] from @unitConfigurationVariables)
	  us.GroupConfigurationId in (select GroupId from GroupConfiguration
	 where BuildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0)and IsDeleted=0)
	 select distinct us.UnitId,ConfigureVariables,ConfigureValues from UnitConfiguration uc
	 Left Join Units us on uc.SetId = us.SetId
	 Left Join UnitSet ust on uc.SetId= ust.SetId
	 Left Join GroupConfiguration gc on us.GroupConfigurationId=gc.GroupId
	 LEFT JOIN openinglocation ol ON ol.unitid = us.unitid AND ol.isdeleted = 0 
	 where --ConfigureVariables in (select [VariableId] from @unitConfigurationVariables)and 
	 uc.SetId in (select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration
	 where BuildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0))and IsDeleted=0)
	   --select distinct BuildingId,GroupConfigurationId,UnitId,TravelFeet,TravelInch, FrontOpening,RearOpening from OpeningLocation ol
	   --Left Join GroupConfiguration gc on ol.GroupConfigurationId= gc.GroupId
	   --where ol.GroupConfigurationId in (select GroupId from GroupConfiguration
	   --where BuildingId in (select BuildingId from Building
	   --where OpportunityId=@opportunityId))
	   select OpportunityId,VersionId,Quotes.CreatedBy,isnull(Quotes.CreatedOn ,getdate()) as CreatedOn,isnull(Quotes.ModifiedOn,'') as ModifiedOn,Status.DisplayName as StatusName from quotes join Status on quotes.QuoteStatusId=Status.StatusKey where quoteId=@quoteId

	   select UnitId,FloorNumber,front,Rear from openinglocation 

	   select * from [dbo].[CoordinationQuestions] where QuoteId=@quoteId
	   select PriceId,UnitId,VariableId,VariableValue from PriceDetails where unitId in (select UnitId from units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId in (select id from Building where QuoteId=@quoteId)))
		select SetId,VariableId,VariableValue from LeadTimeDetails where SetId in (select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration
	 where BuildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0))and IsDeleted=0)

	 select SetId,SystemVariableKeys as VariableId,SystemVariableValues as VariableValue from SystemsVariables where SetId in 
		(select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration
			where BuildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0))and IsDeleted=0)
	select bldConsole.ConsoleId,bldConsole.ConsoleNumber,isnull(bldMap.GroupId,0) as GroupId from [dbo].[BldgEquipmentConsole] bldConsole left join [BldgEquipmentGroupMapping] bldMap on bldConsole.ConsoleId=bldMap.ConsoleId  where Name like '%Lobby%' and buildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0)
	end try
	begin catch
	EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo=@quoteId;
	end catch
end