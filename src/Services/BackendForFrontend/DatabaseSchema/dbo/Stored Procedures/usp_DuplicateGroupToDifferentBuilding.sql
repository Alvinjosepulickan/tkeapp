CREATE PROCEDURE [dbo].[usp_DuplicateGroupToDifferentBuilding]
	-- Add the parameters for the stored procedure here
	@GroupList as BuildingIDList Readonly,
	@BuildingId int,
	@VariableMapperDataTable AS VariableMapper READONLY,
	@Result int output
AS
BEGIN
	BEGIN TRY
		if(@Result is null)
		begin
			set @Result=0
		end
		declare @unitCount int
		set @unitCount =(select count(*) from units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=80))
		declare @duplicateBuilding int
		set @duplicateBuilding=@Result
		declare @GroupName nvarchar(50)
		declare @OldGrpJson nvarchar(max)
		declare @NewGrpJson nvarchar(max)
		declare @NewGrpName nvarchar(max)
		declare @GrpJson nvarchar(max)
		declare @NoOfGroups int
		declare @MaxUnitId int
		declare @MaxDoorsId int
		declare @MaxOpeningLocationId int
		declare @MaxConsoleId int
		declare @MaxConfigurationId int
		declare @MaxLocationId int
		declare @maxsetid int
		declare @maxUnitConfigurationId int
		declare @GroupId int	
		declare @BusinessLine nvarchar(2)
		declare @country nvarchar(2) 
		declare @supplyingFactory nvarchar(2)
		declare @ueid nvarchar(16)
		declare @MaxHallRiserid int
		declare @NumberOfUnits int
		declare @newConsoleId int
		declare @minSetId int
		declare @minConsole int
		declare @maxConsole int
		set @NumberOfUnits=(select count(*) from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId = @BuildingId))
		declare @condition bit
		set @condition=1
		set @GroupId=0
		while(@condition=1)
		begin
			set @GroupId=(select min(buildingId) from @GroupList where buildingId>@GroupId)
			declare @building int
			set @building=(select BuildingId from GroupConfiguration where GroupId=(select min(buildingId) from @GroupList where buildingId>@GroupId))			
			
			--get name of group to be duplicated--
			set @GrpJson=(select GroupName from GroupConfiguration where GroupId=@GroupId)
			--get complete Json string of group to be duplicated--
			set @OldGrpJson=(select GroupJson from GroupConfiguration where GroupId=@GroupId)
			--generate substring from groupJson for replacing the name--
			set @GrpJson='[{"variableId":"'+(select VariableType  from @VariableMapperDataTable where VariableKey='GroupName') +'","value":"'+(@GrpJson)+'"}'
			declare @groupNameCheck int
			declare @GroupCount int
			set @GroupCount=(SELECT count(*)  from [dbo].[GroupConfiguration]  where BuildingId=@BuildingId)
			set @groupNameCheck=1
			while(@groupNameCheck=1)
			begin
				set @groupNameCheck =0
				set @GroupCount+=1
				set @GroupName='G'+CAST(@GroupCount as nvarchar(3))
				if(exists (select * from GroupConfiguration where GroupName=@GroupName and BuildingId=@BuildingId))
				begin
					set @groupNameCheck=1
				end
			end
	
			--generate substring with new group name--
			set @NewGrpName='[{"variableId":"'+(select VariableType  from @VariableMapperDataTable where VariableKey='GroupName')+',"value":"'+(@GroupName)+'"}' 
			--replace old grpJson with new grpJson--
			set @NewGrpJson=replace(@OldGrpJson,@GrpJson,@NewGrpName)
			INSERT INTO [dbo].[GroupConfiguration]([BuildingId] ,[GroupName],[CreatedBy],CreatedOn,ModifedBy,ModifiedOn,NeedsValidation,WorkflowStatus)
				select @BuildingId,@GroupName,CreatedBy,GetDate(),ModifedBy,Getdate(),NeedsValidation,WorkflowStatus from GroupConfiguration where 
					GroupId=@GroupId
			--insert into GroupConfiguration(BuildingId,GroupName,GroupJson,CreatedBy,CreatedOn,ModifedBy,ModifiedOn,NeedsValidation) 
			--	select @BuildingId,@GroupName,@NewGrpJson,CreatedBy,GetDate(),ModifiedOn,Getdate(),NeedsValidation from GroupConfiguration where 
			--		GroupId=@GroupId
			set @Result=SCOPE_IDENTITY()
			declare @createdBy nvarchar(50)
			set @createdBy=(select CreatedBy from GroupConfiguration where GroupId=@Result)
			INSERT INTO [dbo].[GroupConfigurationDetails]

	(
		GroupId,
	
		BuildingId,

		GroupConfigurationType,

		GroupConfigurationValue,

		CreatedBy
	)
	select 
	@Result,@BuildingId,GroupConfigurationType,GroupConfigurationValue,CreatedBy
	from GroupConfigurationDetails where GroupId=@GroupId

	update [GroupConfigurationDetails] set GroupConfigurationValue=@GroupName where GroupId=@Result and GroupConfigurationType in (select VariableType  from @VariableMapperDataTable where VariableKey='GroupName')
	--Select

	--	@Result,
	
	--	@BuildingId,

	--	JSON_VALUE(value,'$.variableId') as VariableId , 
			 
	--	JSON_VALUE(value,'$.value') as [Value],
	
	--	@CreatedBy

	--	FROM OPENJSON(@NewGrpJson);




			if(@duplicateBuilding>-1)
			begin
				select GroupId, GroupName from GroupConfiguration where GroupId=@Result
			end
			if(@BuildingId NOT in (select BuildingId from GroupConfiguration  where GroupId=@GroupId))
			begin
				update GroupConfiguration set NeedsValidation=1 where GroupId=@Result
			end
	
	
			insert into DuplicateUnit(OldGroupId,NewGroupId,OldUnitId,[UnitName] )
				select GroupConfigurationId,@Result,UnitId,[Name] from units where GroupConfigurationId=@GroupId
	
			set @MaxUnitId=(select max(UnitId) from Units where GroupConfigurationId = @GroupId)
	
			insert into Units([Name],[Location],UnitJson,GroupConfigurationId,SetId,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted,Designation,Description,UEID,OccupiedSpaceBelow,MappedLocation,MappedLocationJson,ConflictStatus,WorkflowStatus)
				select [Name],[Location],UnitJson,@Result,SetId,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted,Description,Description,UEID,
				OccupiedSpaceBelow,MappedLocation,MappedLocationJson,ConflictStatus,WorkflowStatus from Units where GroupConfigurationId =@GroupId ORDER BY [Location]
	
			update DuplicateUnit set newUnitId=Units.UnitId from Units inner join DuplicateUnit on DuplicateUnit.UnitName=units.[Name] where Units.GroupConfigurationId=@Result and NewGroupId=@Result

	
	

			insert into ControlLocation(GroupConfigurationId,ControlLocationType,ControlLocationValue,ControlLocationJson,CreatedBy,CreatedOn,
				ModifiedBy,ModifiedOn,IsDeleted)
				select @Result,ControlLocationType,ControlLocationValue,ControlLocationJson,CreatedBy,getdate(),ModifiedBy,GETDATE(),IsDeleted 
					from ControlLocation where GroupConfigurationId=@GroupId



			set @MaxDoorsId=(select max(DoorId) from Doors)
			insert into Doors(GroupConfigurationId,UnitId,DoorType,DoorValue,DoorJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
				select @Result,UnitId,DoorType,DoorValue,DoorJson,CreatedBy,getdate(),ModifiedBy,ModifiedOn,IsDeleted from Doors
				where GroupConfigurationId=@GroupId
			update Doors set Doors.UnitId = DuplicateUnit.NewUNitId from DuplicateUnit INNER JOIN Doors on DuplicateUnit.OldUnitId=Doors.UnitId where DoorId>@MaxDoorsId and GroupConfigurationId=@Result


			set @MaxHallRiserid=(select max(DoorId) from Doors)
			insert into HallRiser(GroupConfigurationId,UnitId,HallRiserType,HallRiserValue,HallRiserJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
				select @Result,UnitId,HallRiserType,HallRiserValue,HallRiserJson,CreatedBy,getdate(),ModifiedBy,ModifiedOn,IsDeleted from HallRiser
					where GroupConfigurationId=@GroupId
			update HallRiser set HallRiser.UnitId = DuplicateUnit.NewUNitId from DuplicateUnit INNER JOIN HallRiser on DuplicateUnit.OldUnitId=HallRiser.UnitId where HallRiserId>@MaxHallRiserId and GroupConfigurationId=@Result





			set @MaxOpeningLocationId=(select max(OpeningLocationId) from OpeningLocation)
			INSERT INTO [dbo].[OpeningLocation]([GroupConfigurationId],[UnitId],[Travelfeet]
			   ,[TravelInch],[OcuppiedSpaceBelow],[NoOfFloors],[FrontOpening],[RearOpening]
			   ,[SideOpening],[FloorDesignation],[FloorNumber],[Front],[Side],[Rear],[CreatedBy]
			   ,[CreatedOn],[ModifiedBy],[ModifiedOn]
			   ,[IsDeleted])
			select @Result,[UnitId],[Travelfeet],[TravelInch],[OcuppiedSpaceBelow],[NoOfFloors]
			   ,[FrontOpening],[RearOpening],[SideOpening],[FloorDesignation],[FloorNumber]
			   ,[Front],[Side],[Rear],[CreatedBy],getdate(),[ModifiedBy],getdate()
			   ,[IsDeleted] from openinglocation 
				where GroupConfigurationId = @GroupId
								
	
			update OpeningLocation set UnitId=DuplicateUnit.NewUNitid from DuplicateUnit inner join OpeningLocation ON  
				OpeningLocation.UnitId=DuplicateUnit.OldUnitId where OpeningLocationId>@MaxOpeningLocationId and [GroupConfigurationId]=@Result
			delete from DuplicateUnit where NewGroupId=@Result
	
			set @minSetId=(select min(SetId) from Units where GroupConfigurationId=@Result and SetId>0)
			set @maxSetId=(select max(SetId) from Units where GroupConfigurationId=@Result and SetId>0)
			declare @newSetId int

			declare @isDifferentBuilding bit
			set @isDifferentBuilding = case when (select BuildingId from GroupConfiguration where GroupId = @GroupId)= @BuildingId
			then 0 else 1 end
			if(@isDifferentBuilding = 0)
			begin 
			
			-- max set id 
					declare @perviousStatus nvarchar(100)
					set @perviousStatus = (select distinct WorkflowStatus from GroupConfiguration where GroupId = @GroupId)		
					
					-- Group Status Flag
					update GroupConfiguration
					set WorkflowStatus = case when (select Step from Status where StatusKey = @perviousStatus)>(select Step from Status where StatusKey = 'GRP_COM') 
					then 'GRP_COM' else @perviousStatus end
					where GroupId = @Result

						-- Unit  Status flag
						update Units
						set WorkflowStatus = case when (select Step from Status where StatusKey = WorkflowStatus)>(select Step from Status where StatusKey = 'UNIT_COM') 
						then 'UNIT_COM' else WorkflowStatus end
						where GroupConfigurationId=@Result 
			end 
			else 
			begin 
				-- Group Status Flag
					update GroupConfiguration
					set WorkflowStatus = 'GRP_CNV' where GroupId = @Result

					-- Unit  Status flag
						update Units
						set WorkflowStatus = 'UNIT_CNV'
						where GroupConfigurationId=@Result
			end

			
			if(@maxsetid>0)
			begin
			while @minSetId<=@maxSetId 
			begin
				if(exists (select * from units where GroupConfigurationId=@Result and SetId=@minSetId))
				begin
					insert into UnitSet(Name,Description,ProductName,CreatedBy,CreatedOn) select Name,Description,ProductName,CreatedBy,getdate() from UnitSet where SetId =@minSetId
					--update TempSet set [NewSetid]=SCOPE_IDENTITY() where OldSetId=@minSetId
					set @newSetId=SCOPE_IDENTITY()				
				
					insert into UnitConfiguration(SetId,ConfigureVariables,ConfigureValues,ConfigureJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted) 
					select @newSetId,ConfigureVariables,ConfigureValues,ConfigureJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from UnitConfiguration where SetId =@minSetId
			
					update units set SetId=@newSetId where SetId=@minSetId and GroupConfigurationId=@Result					
						select UnitId,GroupConfigurationId,CreatedBy,CreatedOn from units where SetId=@newSetId
					declare @minUnit int
					set @minUnit =(select min(Unitid) from units where SetId=@newSetId)
					declare @maxUnit int
					set @maxUnit =(select max(Unitid) from units where SetId=@newSetId)
					declare @numberOfUnitsCreatedInTheGivenYear int
					if(@minUnit is not null)
					begin
						while(@minUnit<=@maxUnit)
						begin
							EXEC usp_GenerateUEID @businessLine, @country, @supplyingFactory, @UEID = @UEID OUTPUT 
							update units set ueid=@UEID where UnitId=@minUnit and SetId=@newSetId
							set @minUnit+=1
						end
					end
			
					set @minConsole=(select min(UnitHallFixtureConsoleId) from UnitHallFixtureConsole where SetId=@minSetId)
					set @maxConsole=(select max(UnitHallFixtureConsoleId) from UnitHallFixtureConsole where SetId=@minSetId)
					if(@minConsole is not null)
					begin
						while (@minConsole<=@maxConsole)
						begin
							if(exists (select * from UnitHallFixtureConsole where SetId=@minSetId and UnitHallFixtureConsoleId=@minConsole))
							begin
								insert into UnitHallFixtureConsole(ConsoleNumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted,ConsoleOrder)
									select ConsoleNumber,FixtureType,@newSetId,Name,IsController,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted,ConsoleOrder from UnitHallFixtureConsole where SetId=@minSetId and UnitHallFixtureConsoleId=@minConsole
								set  @newConsoleId=SCOPE_IDENTITY()
								insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
									select @newConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from UnitHallFixtureConfiguration where UnitHallFixtureConsoleId=@minConsole
								insert into UnitHallFixtureLocations (UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
									select @newConsoleId,FloorNumber,Front,Rear,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from UnitHallFixtureLocations where UnitHallFixtureConsoleId=@minConsole
							
							end
							set @minConsole+=1
						end
					end
					set @minConsole=(select min(EntranceConsoleId) from EntranceConsole where SetId=@minSetId)
					set @maxConsole=(select max(EntranceConsoleId) from EntranceConsole where SetId=@minSetId)
					if(@minConsole is not null)
					begin
						while (@minConsole<=@maxConsole)
						begin
							if(exists (select * from EntranceConsole where SetId=@minSetId and EntranceConsoleId=@minConsole))
							begin
								insert into EntranceConsole(ConsoleNumber,SetId,Name,IsController,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
									select ConsoleNumber,@newSetId,Name,IsController,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from EntranceConsole where SetId=@minSetId and EntranceConsoleId=@minConsole
								set  @newConsoleId=SCOPE_IDENTITY()
								insert into EntranceConfiguration(EntranceConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
									select @newConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from EntranceConfiguration where EntranceConsoleId=@minConsole
								insert into EntranceLocations(EntranceConsoleId,FloorNumber,Front,Rear,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
									select @newConsoleId,FloorNumber,Front,Rear,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from EntranceLocations where EntranceConsoleId=@minConsole
							
							end
							set @minConsole+=1
						end
					end
				end
				set @minSetId=@minSetId+1
			end
		end	

		
		declare @maxConsoleNumber int
		set @maxConsoleNumber=(select max(GroupHallFixtureConsoleId) from GroupHallFixtureConsole)

	
		set @minConsole=(select min(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where GroupId=@GroupId)
		set @maxConsole=(select max(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where GroupId=@GroupId)
		if(@minConsole is not null)
		begin
			while (@minConsole<=@maxConsole)
			begin
				if(exists (select * from GroupHallFixtureConsole where GroupId=@GroupId and GroupHallFixtureConsoleId=@minConsole))
				begin
					insert into GroupHallFixtureConsole(ConsoleNumber,GroupId,Name,FixtureType,IsController,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
						select ConsoleNumber,@Result,Name,FixtureType,IsController,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from GroupHallFixtureConsole where GroupHallFixtureConsoleId=@minConsole and GroupId=@GroupId
					set @newConsoleId=SCOPE_IDENTITY()
					insert into GroupHallFixtureConfiguration (GroupHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted)
						select @newConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from GroupHallFixtureConfiguration where GroupHallFixtureConsoleId=@minConsole
					insert into GroupHallFixtureLocations (GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted, HallStationName)
						select @newConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted, HallStationName from GroupHallFixtureLocations where GroupHallFixtureConsoleId=@minConsole
					update GroupHallFixtureLocations set UnitId=ut.UnitId from Units ut join units uts on ut.[Name]=uts.[Name] where ut.GroupConfigurationId=@Result and uts.GroupConfigurationId=@GroupId
					update GroupHallFixtureLocations set UnitId=0 where HallStationName<>NULL and GroupHallFixtureConsoleId in 
					(select GroupHallFixtureConsoleId from GroupHallFixtureConsole where groupid= @Result)
				end
				set @minConsole+=1
			end
		end
	
	
		declare @minUnitId int
		set @minUnitId=(select min(unitid) from units where GroupConfigurationId=@Result)
		declare @maxId int
		set @maxId=(select max(unitid) from units where GroupConfigurationId=@Result)
		declare @numberOfUnitsInBuilding int
		declare @unitNameCheck int
		declare @unitNameExists int
		set @unitNameExists=0
		declare @UnitName nvarchar(10)
		set @numberOfUnits=(select count(*) from Units where  GroupConfigurationId<>@Result and  GroupConfigurationId in (select GroupId from groupconfiguration where BuildingId=(select BuildingId from GroupConfiguration where GroupId=@Result)))
		while @minUnitId<=@maxId
		begin
			set @unitNameCheck=1
			if(exists (select * from units where GroupConfigurationId=@Result and UnitId=@minUnitId))
			begin
				while(@unitNameCheck=1)
				begin
					set @unitNameExists =0
					set @numberOfUnits+=1
					set @UnitName='U'+CAST(@numberOfUnits as nvarchar(3))
					if(exists (select * from units where [Designation]=@UnitName and GroupConfigurationId<>@Result and GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId) and UnitId<>@minUnitId))
					begin
						set @unitNameExists=1
					end
					if(@unitNameExists=0)
					begin
						update Units set [Description]=@UnitName,[Designation]=@UnitName where UnitId=@minUnitId and GroupConfigurationId=@Result
						set @unitNameCheck=0
						set @minUnitId+=1
					end
				end
			end
		end
	
		DELETE FROM TempUnit WHERE NewGroupId=@Result
		delete from TempSet where NewGroupid=@Result
		delete from TempUnitHallFixture where OldSetId in (select setId from units where GroupConfigurationId = @GroupId)
		if(exists (select * from @GroupList where buildingId>@GroupId))
		begin
			set @condition=1
		end
		else
		begin
			set @condition=0
		end
	
		exec [dbo].[usp_UpdateWorkflowStatus]@BuildingId,'building'

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
END

