
CREATE proc [dbo].[usp_DuplicateBuildingByBuildingid] 
(
	@BuildingList as BuildingIDList Readonly,
	--@OpportunityId nvarchar(25),--@QuoteId nvarchar(25),
	--@VersionId nvarchar(25),
	@QuoteId nvarchar(25),
	@VariableMapperDataTable AS VariableMapper READONLY,
	@Result int output
)
as
begin
	BEGIN TRY

	--declare @QuoteId nvarchar(20)
	--select @QuoteId = (Select QuoteId from Quotes where OpportunityId = @OpportunityId and VersionId = @VersionId)

	declare @BuildingName nvarchar(10)
	declare @buildingId int
	declare @ProjectId nvarchar(25)
	declare @NumberOfBuildings int
	declare @OldBldJson nvarchar(max)
	declare @BldJson nvarchar(max)
	declare @NewBldJsion nvarchar(max)
	declare @NewBldJson nvarchar(max)
	declare @MaxUnitId int
	declare @MaxControlLocationId int
	declare @MaxDoorsId int
	declare @MaxHallRiserId int
	declare @MaxOpeningLocationId int
	DECLARE @MaxGroupHallFixtureConsoleId int
	declare @BusinessLine nvarchar(2)
	declare @country nvarchar(2) 
	declare @supplyingFactory nvarchar(2)
	declare @ueid nvarchar(16)
	declare @minSetId int
	declare @maxSetId int
	declare @maxConsoleId int
	declare @maxConfigurationId int
	declare @maxLocationId int
	declare @condition bit
	set @condition=1
	set @buildingId=0

	
	while(@condition=1)
	begin
		set @buildingId=(select min(buildingId) from @BuildingList where buildingId>@buildingId)
	


		--get the name of building to be duplicated
		set @BldJson=(select BldName from building where id=@BuildingId)
		--get the complete json string of the building to be duplicated
		set @OldBldJson=('['+ SUBSTRING( 
			( 
		     SELECT ',{"VariableId":"'+k.BuindingType+'","Value":"'+k.BuindingValue+'"}'
			from building b
			Left Join BuildingConfiguration k
			on b.Id = k.BuildingId
			where b.id = @BuildingId
			and b.isDeleted=0
					 FOR XML PATH('') 
		), 2 , 9999) + ']' --As BldJson
			)
		--generate substring from bldjson for replacing the name
		set @BldJson='[{"VariableId":"'+(select VariableType  from @VariableMapperDataTable where VariableKey='BuildingName') +'","Value":"'+(@BldJson )+'"}'
		--get the opportunityid for that building
		set @ProjectId=(select QuoteId from Building where Id=@BuildingId)
		declare @BuildingNameCheck int
		declare @BuildingCount int
		set @BuildingCount=(SELECT count(*)  from [dbo].[Building]  where QuoteId = @QuoteId)
		set @BuildingNameCheck=1
		while(@BuildingNameCheck=1)
		begin
			set @BuildingNameCheck =0
			set @BuildingCount+=1
			set @BuildingName='B'+CAST(@BuildingCount as nvarchar(3))
			--select @BuildingName
			if(exists (select * from Building where BldName=@BuildingName and QuoteId = @QuoteId))
			begin
				set @BuildingNameCheck=1
			end
			if(@BuildingNameCheck=0)
			begin
				set @BuildingName=@BuildingName
			end 
		end
		
		--set @BuildingName='B'+Cast(@NumberOfBuildings as nvarchar(9))
		--generate substring with new building name
		set @NewBldJsion='[{"VariableId":"'+(select VariableType  from @VariableMapperDataTable where VariableKey='BuildingName') +'","Value":"'+(@BuildingName )+'"}'
		--replace old bldjson with new bldjson
		set @NewBldJson= replace(@OldBldJson,@BldJson,@NewBldJsion)


		--Copy the building data
		insert into Building (ProjectId,BldName,/*BldJson,*/IsDeleted,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,OpportunityId,QuoteId,workflowstatus,BuildingEquipmentStatus)
		select ProjectId,@BuildingName,/*@NewBldJson,*/IsDeleted,CreatedBy,getdate(),ModifiedBy,getdate(),OpportunityId,@QuoteId,workflowstatus,BuildingEquipmentStatus from building where id=@BuildingId
		--Copy Building Elevation data
		--Get the new BuildingId

		set @Result=SCOPE_IDENTITY()
		
		insert into BuildingConfiguration (BuildingId, BuindingType, BuindingValue, CreatedBy,CreatedOn, ModifiedBy, ModifiedOn)
		select @Result, BuindingType, BuindingValue,CreatedBy,getdate(), ModifiedBy, ModifiedOn
		from BuildingConfiguration
		where BuildingId=@BuildingId

		update BuildingConfiguration
		set BuindingValue = @BuildingName
		where BuildingId=@Result
		and BuindingType in (select VariableType  from @VariableMapperDataTable where VariableKey='BuildingName')

		-- Building Status Update BLDGEQP_AV

					update Building
					set WorkflowStatus = case when (select Step from Status where StatusKey = workflowstatus )>(select Step from Status where StatusKey = 'BLDG_COM') 
					then 'BLDG_COM' else workflowstatus end
					where Id = @Result

			-- Quote Status Update 

					update Quotes
					set QuoteStatusId = 'QT_INC' where QuoteId = @QuoteId

		/*
		update bldConfig
		set BuindingValue = @BuildingName,
			ModifiedBy = ModifiedBy,
			ModifiedOn = getdate()
		from BuildingConfiguration bldConfig
		where id = @BuildingId
		and BuindingType = 'Building_Configuration.Parameters.Basic_Info.BLDGNAME'
		*/
		
		select Id as BuildingId,BldName as BuildingName from Building where id=@Result
		insert into BuildingElevation(BuildingId,MainEgress,FloorDesignation,ElevationFeet,ElevationInch,FloorToFloorHeightFeet,FloorToFloorHeightInch,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted,alternateEgress,floorNumber,noOfFloor,buildingRise)
									select @Result,MainEgress,FloorDesignation,ElevationFeet,ElevationInch,FloorToFloorHeightFeet,FloorToFloorHeightInch,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted,alternateEgress,floorNumber,noOfFloor,buildingRise from BuildingElevation where BuildingId=@BuildingId order by BuildingElevationId
	
	
	
		declare @GroupList as BuildingIDList
		-- Added delete @group List
		delete from @GroupList
		--select @buildingId as buildinId
		insert into @GroupList select GroupId from GroupConfiguration where BuildingId=@buildingId AND IsDeleted = 0 order by GroupId
		--select * from @GroupList as GroupList
		exec [dbo].[usp_DuplicateGroupToDifferentBuilding] @GroupList,@Result,@VariableMapperDataTable,-1
		update GroupConfiguration set NeedsValidation=0 where BuildingId=@Result
		
	----Copy Group and UnitId of the existing building
	--insert into TempUnit(Buildingid,GroupName,OldGroupId,OldUnitId,UnitName) 
	--								select @BuildingId,GroupName,GroupConfigurationId,UnitId,[Name] from GroupConfiguration join Units on GroupConfiguration.GroupId=Units.GroupConfigurationId where GroupConfiguration.BuildingId=@BuildingId
	
	--insert into [TempSet]([BuildingId] ,[GroupName],[OldGroupId],[OldSetId] )
	--								select @BuildingId,GroupName,GroupConfigurationId,[SetId] from GroupConfiguration join Units on GroupConfiguration.GroupId=Units.GroupConfigurationId where GroupConfiguration.BuildingId=@BuildingId and SetId>0
	
	----Copy group data to GroupConfiguration table
	--insert into GroupConfiguration(BuildingId,GroupName,GroupJson,CreatedBy,CreatedOn,ModifedBy,ModifiedOn) 
	--								select @Result,GroupName,GroupJson,CreatedBy,GetDate(),ModifiedOn,Getdate() from GroupConfiguration where BuildingId=@BuildingId and IsDeleted=0 order by GroupId
	--set @MaxUnitId=(select max(UnitId) from Units where GroupConfigurationId in(select GroupId from GroupConfiguration where BuildingId=@BuildingId))
	
	--update TempUnit set NewGroupId=GroupConfiguration.GroupId from GroupConfiguration inner Join TempUnit on GroupConfiguration.GroupName=TempUnit.Groupname where TempUnit.BuildingId=@BuildingId and GroupConfiguration.BuildingId=@Result
	--update TempSet set NewGroupId=GroupConfiguration.GroupId from GroupConfiguration inner Join TempSet on GroupConfiguration.GroupName=TempSet.Groupname where TempSet.BuildingId=@BuildingId and GroupConfiguration.BuildingId=@Result
	----Copy Unit data to Units table
	-- set @minSetId=(select min(Id) from TempSet where BuildingId=@BuildingId)
	--set @maxSetId=(select max(Id) from TempSet where BuildingId=@BuildingId)
	
	 
	 
	-- insert into units([Name],[Location],UnitJson,GroupConfigurationId,SetId,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted,Designation,Description,UEID,OccupiedSpaceBelow,MappedLocation,MappedLocationJson)
	-- select [Name],[Location],UnitJson,GroupConfigurationId,SetId,CreatedBy,getdate(),ModifiedBy,getdate(),IsDeleted,Description,Description,UEID,OccupiedSpaceBelow,MappedLocation,MappedLocationJson from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId and IsDeleted=0)
	--update Units set GroupConfigurationId=TempUnit.NewGroupId from TempUnit inner join Units ON TempUnit.OldGroupId =Units.GroupConfigurationId and TempUnit.UnitName=Units.[Name] where UnitId>@MaxUnitId
	-- while @minSetId<=@maxSetId 
	--begin
	--	insert into UnitSet(Name,Description,ProductName,CreatedBy,CreatedOn) select Name,Description,ProductName,CreatedBy,getdate() from UnitSet where SetId in (select OldSetId from TempSet where Id=@minSetId)
	--	update TempSet set [NewSetid]=SCOPE_IDENTITY() where id=@minSetId
	--	insert into UnitConfiguration(SetId,ConfigureVariables,ConfigureValues,ConfigureJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted) select SCOPE_IDENTITY(),ConfigureVariables,ConfigureValues,ConfigureJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from UnitConfiguration where SetId in (select OldSetId from TempSet where Id=@minSetId )and IsDeleted=0
	--	set @minSetId=@minSetId+1
	--end
	--update units set SetId=TempSet.NewSetid from TempSet join Units on TempSet.OldSetId=Units.SetId where Units.UnitId>@MaxUnitId
		
	-- if(Year(@yearCheck)<>@year)
	--begin
	--	truncate table UEIDMaster
	--end
	-- insert into UEIDMaster(UnitId,GroupId,CreatedBy,CreatedOn) select UnitId,GroupConfigurationId,CreatedBy,Getdate() from units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@Result)
	 
	-- update Units set UEID =replace(uts.UEID,SUBSTRING(uts.UEID, 5, 14),cast(@year as nvarchar(4))+
	-- (
	--					case when LEN(UEIDMaster.UEIDNumber) = 1 then '00000'+CAST(UEIDMaster.UEIDNumber as varchar(6)) 
	--					when LEN(UEIDMaster.UEIDNumber) = 2 then '0000'+CAST(UEIDMaster.UEIDNumber as varchar(6))
	--					when LEN(UEIDMaster.UEIDNumber) = 3 then '000'+CAST(UEIDMaster.UEIDNumber as varchar(6))
	--					when LEN(UEIDMaster.UEIDNumber) = 4 then '00'+CAST(UEIDMaster.UEIDNumber as varchar(6))
	--					when LEN(UEIDMaster.UEIDNumber) = 5 then '0'+CAST(UEIDMaster.UEIDNumber as varchar(6))
	--					when LEN(UEIDMaster.UEIDNumber) = 6 then +CAST(UEIDMaster.UEIDNumber as varchar(6))
	--					end)+SUBSTRING(uts.UEID, 15, 16)) 
						
	--					from Units join Units uts on units.UnitId=uts.UnitId join UEIDMaster on UEIDMaster.UnitId=Uts.UnitId where uts.setId>0 and uts.UnitId>@MaxUnitId --and uts.GroupConfigurationId in(select GroupId from GroupConfiguration where BuildingId=@Result)

	-- update TempUnit set NewUNitid=Units.UnitId from Units inner join TempUnit ON TempUnit.NewGroupId =Units.GroupConfigurationId and TempUnit.UnitName=Units.[Name] where UnitId>@MaxUnitId
	 

	-- --Copy Control Location Data
	-- set @MaxControlLocationId=(select max(ControlLocationId) from ControlLocation)
	-- insert into ControlLocation(GroupConfigurationId,ControlLocationType,ControlLocationValue,ControlLocationJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
	-- select GroupConfigurationId,ControlLocationType,ControlLocationValue,ControlLocationJson,CreatedBy,getdate(),ModifiedBy,GETDATE(),IsDeleted from ControlLocation where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId and IsDeleted=0)
	-- update ControlLocation set GroupConfigurationId=TempUnit.NewGroupId from TempUnit inner join ControlLocation ON TempUnit.OldGroupId =ControlLocation.GroupConfigurationId  where ControlLocationId>@MaxControlLocationId

	-- --Copy Doors data
	--set @MaxDoorsId=(select max(DoorId) from Doors)
	--insert into Doors(GroupConfigurationId,UnitId,DoorType,DoorValue,DoorJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
	--select GroupConfigurationId,UnitId,DoorType,DoorValue,DoorJson,CreatedBy,getdate(),ModifiedBy,getdate(),IsDeleted from Doors where  GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId and IsDeleted=0)
	--update Doors set GroupConfigurationId=TempUnit.NewGroupId,UnitId=TempUnit.NewUNitid from TempUnit inner join Doors ON TempUnit.OldGroupId =Doors.GroupConfigurationId and Doors.UnitId=TempUnit.OldUnitId where DoorId>@MaxDoorsId

	----Copy Hall Riser data
	--set @MaxHallRiserId=(select max(HallRiserId) from HallRiser)
	--insert into HallRiser(GroupConfigurationId, UnitId, HallRiserType,HallRiserValue, HallRiserJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
	--select GroupConfigurationId, UnitId, HallRiserType,HallRiserValue, HallRiserJson,CreatedBy,getdate(),ModifiedBy,getdate(),IsDeleted from HallRiser where  GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId and IsDeleted=0)
	--update HallRiser set GroupConfigurationId=TempUnit.NewGroupId,UnitId=TempUnit.NewUNitid from TempUnit inner join HallRiser ON TempUnit.OldGroupId =HallRiser.GroupConfigurationId and HallRiser.UnitId=TempUnit.OldUnitId where HallRiserId>@MaxHallRiserId





	----Copy openiniglocation data
	--set @MaxOpeningLocationId=(select max(OpeningLocationId) from OpeningLocation)
	--INSERT INTO [dbo].[OpeningLocation]
 --          ([GroupConfigurationId]
 --          ,[UnitId]
 --          ,[Travelfeet]
 --          ,[TravelInch]
 --          ,[OcuppiedSpaceBelow]
 --          ,[NoOfFloors]
 --          ,[FrontOpening]
 --          ,[RearOpening]
 --          ,[SideOpening]
 --          ,[FloorDesignation]
 --          ,[FloorNumber]
 --          ,[Front]
 --          ,[Side]
 --          ,[Rear]
 --          ,[CreatedBy]
 --          ,[CreatedOn]
 --          ,[ModifiedBy]
 --          ,[ModifiedOn]
 --          ,[IsDeleted])
	--select [GroupConfigurationId]
 --          ,[UnitId]
 --          ,[Travelfeet]
 --          ,[TravelInch]
 --          ,[OcuppiedSpaceBelow]
 --          ,[NoOfFloors]
 --          ,[FrontOpening]
 --          ,[RearOpening]
 --          ,[SideOpening]
 --          ,[FloorDesignation]
 --          ,[FloorNumber]
 --          ,[Front]
 --          ,[Side]
 --          ,[Rear]
 --          ,[CreatedBy]
 --          ,getdate()
 --          ,[ModifiedBy]
 --          ,getdate()
 --          ,[IsDeleted] from openinglocation where GroupConfigurationId in
	--							(select GroupId from GroupConfiguration where BuildingId=@BuildingId and IsDeleted=0)


	--update OpeningLocation set GroupConfigurationId=TempUnit.NewGroupId,UnitId=TempUnit.NewUNitid from TempUnit inner join OpeningLocation ON TempUnit.OldGroupId =OpeningLocation.GroupConfigurationId and OpeningLocation.UnitId=TempUnit.OldUnitId where OpeningLocationId>@MaxOpeningLocationId

	--insert into TempUnitHallFixture([OldSetId],[ConsoleName],[OldConsoleId])
	--select GroupId,[Name],GroupHallFixtureConsoleId from GrouphallfixtureConsole where GroupId in(select GroupId from GroupConfiguration where GroupId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId))
	--set @maxConsoleId=(select max(GroupHallFixtureConsoleId) from GroupHallFixtureConsole )
	-- set @maxConfigurationId =(select max(GroupHallFixtureConfigurationId) from GroupHallFixtureConfiguration )
	-- set @maxLocationId =(select max(GroupHallFixtureLocationId) from GroupHallFixtureLocations )

	-- insert into GroupHallFixtureConfiguration(GroupHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
	--	select GroupHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,getdate() from GroupHallFixtureConfiguration where GroupHallFixtureConsoleId in (select GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId in(select GroupId from GroupConfiguration where GroupId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId)))
	--insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,front,Rear,CreatedBy,CreatedOn)
	--	select GroupHallFixtureConsoleId,UnitId,FloorDesignation,front,Rear,CreatedBy,getdate() from GroupHallFixtureLocations where GroupHallFixtureConsoleId in (select GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId in(select GroupId from GroupConfiguration where GroupId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId)))
	--insert into GroupHallFixtureConsole(ConsoleNumber,FixtureType,GroupId,[Name],IsController,CreatedBy,CreatedOn,IsDeleted) 
	--	select ConsoleNumber,FixtureType,GroupId,[Name],IsController,CreatedBy,getDate(),IsDeleted from GroupHallFixtureConsole where GroupId in(select GroupId from GroupConfiguration where BuildingId =@BuildingId)
	
	--update TempUnitHallFixture set [newConsoleId]=GroupHallFixtureConsoleId from GroupHallFixtureConsole join TempUnitHallFixture on GroupHallFixtureConsole.GroupId=TempUnitHallFixture.OldSetId and TempUnitHallFixture.ConsoleName= GroupHallFixtureConsole.Name where GroupHallFixtureConsole.GroupHallFixtureConsoleId>@maxConsoleId
	--update GroupHallFixtureConsole set GroupId=TempUnit.NewGroupId from TempUnit inner join GroupHallFixtureConsole on TempUnit.OldGroupId=GroupHallFixtureConsole.GroupId where GroupHallFixtureConsole.GroupHallFixtureConsoleId>@maxConsoleId
	--update GroupHallFixtureConfiguration set GroupHallFixtureConsoleId=newConsoleId from TempUnitHallFixture join GroupHallFixtureConfiguration on TempUnitHallFixture.OldConsoleId=GroupHallFixtureConfiguration.GroupHallFixtureConsoleId where GroupHallFixtureConfiguration.GroupHallFixtureConfigurationId>@maxConfigurationId
	--update GroupHallFixtureLocations set GroupHallFixtureConsoleId=TempUnitHallFixture.newConsoleId from TempUnitHallFixture join GroupHallFixtureLocations on TempUnitHallFixture.OldConsoleId=GroupHallFixtureLocations.GroupHallFixtureConsoleId where GroupHallFixtureLocations.GroupHallFixtureLocationId>@maxLocationId
	--update GroupHallFixtureLocations set UnitId=TempUnit.NewUNitid from TempUnit inner join GroupHallFixtureLocations ON TempUnit.OldUnitId =GroupHallFixtureLocations.UnitId  where GroupHallFixtureLocationId>@maxLocationId


	--delete from TempUnitHallFixture where OldSetId in (select setId from units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId))

	--insert into TempUnitHallFixture([OldSetId],[ConsoleName],[OldConsoleId])
	--select SetId,[Name],UnitHallFixtureConsoleId from unithallfixtureConsole where SetId in(select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId))

	--set @maxConsoleId=(select max(UnitHallFixtureConsoleId) from UnitHallFixtureConsole )
	-- set @maxConfigurationId =(select max(UnitHallFixtureConfigurationId) from UnitHallFixtureConfiguration )
	-- set @maxLocationId =(select max(UnitHallFixtureLocationId) from UnitHallFixtureLocations )

	--insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
	--	select UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,getdate() from UnitHallFixtureConfiguration where UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId in(select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId)))
	--insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,front,Rear,CreatedBy,CreatedOn)
	--	select UnitHallFixtureConsoleId,FloorNumber,front,Rear,CreatedBy,getdate() from UnitHallFixtureLocations where UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId in(select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId)))
	--insert into UnitHallFixtureConsole(ConsoleNumber,FixtureType,SetId,[Name],IsController,CreatedBy,CreatedOn,IsDeleted) 
	--	select ConsoleNumber,FixtureType,SetId,[Name],IsController,CreatedBy,getDate(),IsDeleted from UnitHallFixtureConsole where SetId in(select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId))
	
	--update TempUnitHallFixture set [newConsoleId]=UnitHallFixtureConsoleId from UnitHallFixtureConsole join TempUnitHallFixture on UnitHallFixtureConsole.SetId=TempUnitHallFixture.OldSetId and TempUnitHallFixture.ConsoleName= UnitHallFixtureConsole.Name where UnitHallFixtureConsole.UnitHallFixtureConsoleId>@maxConsoleId
	--update UnitHallFixtureConsole set SetId=TempSet.NewSetid from TempSet inner join UnitHallFixtureConsole on TempSet.OldSetId=UnitHallFixtureConsole.SetId where UnitHallFixtureConsole.UnitHallFixtureConsoleId>@maxConsoleId
	--update UnitHallFixtureConfiguration set UnitHallFixtureConsoleId=newConsoleId from TempUnitHallFixture join UnitHallFixtureConfiguration on TempUnitHallFixture.OldConsoleId=UnitHallFixtureConfiguration.UnitHallFixtureConsoleId where UnitHallFixtureConfiguration.UnitHallFixtureConfigurationId>@maxConfigurationId
	--update UnitHallFixtureLocations set UnitHallFixtureConsoleId=newConsoleId from TempUnitHallFixture join UnitHallFixtureLocations on TempUnitHallFixture.OldConsoleId=UnitHallFixtureLocations.UnitHallFixtureConsoleId where UnitHallFixtureLocations.UnitHallFixtureLocationId>@maxLocationId

	--set @maxConsoleId=(select max(EntranceConsoleId) from EntranceConsole )
	-- set @maxConfigurationId =(select max(EntranceConfigurationId) from EntranceConfiguration )
	-- set @maxLocationId =(select max(EntranceLocationId) from EntranceLocations )

	-- delete from TempUnitHallFixture where OldSetId in (select setId from units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId))

	-- insert into TempUnitHallFixture([OldSetId],[ConsoleName],[OldConsoleId])
	--select SetId,[Name],EntranceConsoleId from EntranceConsole where SetId in(select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId))

	-- insert into EntranceConfiguration(EntranceConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
	--	select EntranceConsoleId,VariableType,VariableValue,CreatedBy,getdate() from EntranceConfiguration where EntranceConsoleId in (select EntranceConsoleId from EntranceConsole where SetId in(select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId)))
	--insert into EntranceLocations(EntranceConsoleId,FloorNumber,front,Rear,CreatedBy,CreatedOn)
	--	select EntranceConsoleId,FloorNumber,front,Rear,CreatedBy,getdate() from EntranceLocations where EntranceConsoleId in (select EntranceConsoleId from EntranceConsole where SetId in(select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId)))
	--insert into EntranceConsole(ConsoleNumber,SetId,[Name],IsController,CreatedBy,CreatedOn,IsDeleted) 
	--	select ConsoleNumber,SetId,[Name],IsController,CreatedBy,getDate(),IsDeleted from EntranceConsole where SetId in(select SetId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId))
	--update TempUnitHallFixture set [newConsoleId]=EntranceConsoleId from EntranceConsole join TempUnitHallFixture on EntranceConsole.SetId=TempUnitHallFixture.OldSetId and TempUnitHallFixture.ConsoleName= EntranceConsole.Name where EntranceConsole.EntranceConsoleId>@maxConsoleId
	--update EntranceConsole set SetId=TempSet.NewSetid from TempSet inner join EntranceConsole on TempSet.OldSetId=EntranceConsole.SetId where EntranceConsole.EntranceConsoleId>@maxConsoleId
	--update EntranceConfiguration set EntranceConsoleId=newConsoleId from TempUnitHallFixture join EntranceConfiguration on TempUnitHallFixture.OldConsoleId=EntranceConfiguration.EntranceConsoleId where EntranceConfiguration.EntranceConfigurationId>@maxConfigurationId
	--update EntranceLocations set EntranceConsoleId=newConsoleId from TempUnitHallFixture join EntranceLocations on TempUnitHallFixture.OldConsoleId=EntranceLocations.EntranceConsoleId where EntranceLocations.EntranceLocationId>@maxLocationId

	if(exists (select * from @BuildingList where buildingId>@buildingId))
	begin
		set @condition=1
	end
	else
	begin
		set @condition=0
	end
	--DELETE FROM TempUnit WHERE BuildingId=@BuildingId
	--delete from TempSet where BuildingId=@BuildingId
	--delete from TempUnitHallFixture where OldSetId in (select setId from units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId))
	end


	END TRY
	BEGIN CATCH 
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
end

