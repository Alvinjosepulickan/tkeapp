
CREATE PROCEDURE [dbo].[usp_DuplicateUnitToGroup]
	-- Add the parameters for the stored procedure here
	@UnitList as [dbo].[UnitIDList] readonly,
	@GroupId int,
	--@OperationType nvarchar(100),
	@CarPositionArray as CarPositionList readOnly,
	@Result int output

AS
BEGIN
	BEGIN TRY
		declare @MaxOpeningLocationId int
		declare @MappedLocation nvarchar(max)
		declare @MappedLocationJson nvarchar(max)
		declare @MappedLocationJsonz nvarchar(max)
		set @MappedLocationJsonz=(SELECT distinct(MappedLocationJson) from units where groupConfigurationId=@GroupId and [Location]='B1P1')
		--delete from units where [Location] in (select CarPositionLocation from @CarPositionArray) AND GroupConfigurationId=@GroupId
		delete from units where [MappedLocation] in (select CarPositionLocation from @CarPositionArray) AND GroupConfigurationId=@GroupId
		declare @buildingId int
		declare @UnitId int
		declare @CarPositionList as CarPositionList
		insert into @CarPositionList select * from @CarPositionArray
		set @UnitId=(select min(UnitID) from @UnitList)
		declare @NewUnitName nvarchar(10)
		declare @NoOfUnits int
		declare @NoOfSets int
		declare @NewSetName nvarchar(10)
		declare @MaxUnitId int
		declare @PositionName nvarchar(8)
		declare @NewLocation nvarchar(8)
		declare @NewUnitJson nvarchar(max)
		declare @BusinessLine nvarchar(2)
		declare @country nvarchar(2) 
		declare @supplyingFactory nvarchar(2)
		declare @ueid nvarchar(16)
		declare @yearStartdate datetime
		declare @year int 
		declare @yearCheck datetime
		declare @MaxControlLocationId int
		declare @MaxDoorsId int
		--declare @OldDoorType nvarchar(max)
		--declare @DoorValue nvarchar(300)
		--declare @NewDoorJson nvarchar(max)
		--declare @NewDoorType nvarchar(max)
		declare @MaxHallRiserId int
		declare @OldHallRiserType nvarchar(max)
		declare @HallRiserValue nvarchar(300)
		declare @NewHallRiserType nvarchar(max)
		declare @NewHallRiserJson nvarchar(max)
		declare @maxSetId int
		DECLARE @LocationId INT
		declare @UnitName nvarchar(6)
		declare @UnitDesgination nvarchar(6)
		declare @UnitDescription nvarchar(6)
		--declare @MappedLocation nvarchar(max)
		declare @CarPosition nvarchar(8)
		declare @NumberOfUnits int
		declare @condition bit
		set @condition=1
		set @LocationId=0
		set @CarPosition=(select Location from Units where UnitId=@UnitId)
		--set @MappedLocation=(SELECT distinct(MappedLocationJson) from units where groupConfigurationId=@GroupId and [Name]='U1')
		set @buildingId=(select max(BuildingId) from GroupConfiguration where GroupId=@GroupId)
		set @numberOfUnits=(select count(*) from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId in (select BuildingId from GroupConfiguration where GroupId=@GroupId)))
		declare @minId int
		declare @maxId int
		set @minId=(Select min([CarPositionID]) from @CarPositionList)
		set @maxId=(Select max([CarPositionID]) from @CarPositionList)
		declare @unitNameExists bit
		declare @unitNameCheck bit
		--inserying oldsetid into Temp set
		insert into TempSet(OldSetId)
		select distinct setid from Units where UnitId in(@UnitId)
		while @minId<=@maxId
		begin
			set @unitNameCheck=1
			while(@unitNameCheck=1)
			begin
				set @unitNameExists =0
				set @numberOfUnits+=1
				set @UnitName='U'+CAST(@numberOfUnits as nvarchar(3))
				if(exists (select * from units where [Designation]=@UnitName and GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId)))
				begin
					set @unitNameExists=1
				end
				if(exists (select * from @CarPositionList where [UnitDesignation]=@UnitName))
				begin
					set @unitNameExists=1
				end
					if(@unitNameExists=0)
					begin
						update @CarPositionList set [UnitDesignation]=@UnitName where [CarPositionID]=@minId  AND [UnitDesignation]=''
						set @unitNameCheck=0
						set @minId+=1
					end
				end
			end
		if(Exists(SELECT [UnitDesignation]
			FROM @CarPositionList
			GROUP BY [UnitDesignation]
			HAVING COUNT([UnitDesignation]) > 1))
		begin
			set @Result=-1
		
		end
			-- validating unit name within the building
		if((select count(*) from (select [UnitDesignation],[CarPositionLocation] from @CarPositionList
									  where [UnitDesignation] in (select [Designation] from units
										where GroupConfigurationId in (select GroupId from GroupConfiguration 
											where BuildingId=@buildingId )))as duplicate)>0)
		BEGIN
			set @Result=-1
		
		end
		if(@Result=-1)
		begin
			select @Result as Result

			select uts.[UnitDesignation],uts.CarPositionLocation
				from @CarPositionList as uts 
					join 
				Units as ut on 
					 ut.[Designation]=uts.[UnitDesignation] where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@buildingId) --and GroupConfigurationId<>@GroupId
		

			return 0
		end
		while(@condition=1)
		begin
			--set @NewDoorJson=(select DoorJson from Doors where DoorId=(select max(DoorId) from Doors where unitId=@UnitId))--
			set @LocationId=(select min([CarPositionID]) from @CarPositionList where [CarPositionID]>@LocationId)
			--get CarpositionName--
			set @PositionName= (select [CarPositionLocation] from @CarPositionList where [CarPositionID]=@LocationId) 
			-- get unit name 
			set @UnitName= (select [UnitName] from @CarPositionList where [CarPositionID]=@LocationId)
		

			--get number of units for generating Unit name--
			set @NoOfUnits =(select count(*) from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@buildingId))+1
			--get number of Sets for generating Set name--
			set @NoOfSets = (select Count(distinct setId) from units where GroupConfigurationId = @GroupId and setId>0)+1
			--generate new group name--
			set @NewUnitName = (select [UnitDesignation] from @CarPositionList where CarPositionLocation=@PositionName)
			--generate new group name--
			set @NewSetName ='Set'+Cast(@NoOfSets as nvarchar(10))
			--generate new Location--
			set @PositionName= (select [CarPositionLocation] from @CarPositionList where [CarPositionID]=@LocationId) 
			set @NewLocation= @PositionName
			declare @MappedLocations nvarchar(8)
			set @MappedLocations=@NewLocation
			set @MappedLocations=@NewLocation
			declare @location nvarchar(10)

			if(@PositionName like '%B2P%')
				begin
					set @MappedLocations='B2P'
					declare @position int
					set @position=1
					declare @unitNumber int
					set @unitNumber=4
					while @position<5
					begin
						set @MappedLocations=@MappedLocations+cast(@position as nvarchar(3))
						set @unitNumber+=1
						if( exists(select * from Units where GroupConfigurationId=@GroupId and [Location]=@MappedLocations))
						begin
							set @MappedLocations='B2P'
							set @position=@position+1
						end
						else
						begin
							--set @MappedLocations=@MappedLocations+cast(@position as nvarchar(3))
							set @UnitName='U'+(cast(@unitNumber as nvarchar(3)))
							set @position=5
						end
					
					end
				end
			declare @OldLocation nvarchar(8)
			set @OldLocation=(select location from units where unitId= @UnitId)
			--generate new UnitJson--
			set @NewUnitJson = (select replace((select UnitJson from units where UnitId =@UnitId),@OldLocation,@NewLocation))
			--get old Door type--
			--set @OldDoorType = (select distinct DoorType from Doors where UnitId=@UnitId)
			----get Door Value--
			--set @DoorValue= (select distinct DoorValue from Doors where UnitId=@UnitId)
			----generate new Door type--
			--set @NewDoorType = (select replace(@OldDoorType,@CarPosition,@NewLocation))
			----generate New DoorJson--
			--set @NewDoorJson = (select replace(@NewDoorJson,@CarPosition,@NewLocation))
			--get old Door type--
			--Copy GroupId of new Unit to Units table--
			insert into Units ([Name],
							[Location],
							[UnitJson],
							[GroupConfigurationId],
							[SetId],
							[CreatedBy],
							[CreatedOn],
							[ModifiedBy],
							[ModifiedOn],
							[IsDeleted],
							[Designation],
							[Description],
							[UEID],
							[OccupiedSpaceBelow],
							[IsFutureElevator],
							[MappedLocation],
							[MappedLocationJson],
							[WorkflowStatus]
						 )
			select @UnitName,
					@MappedLocations,
					replace([UnitJson],@OldLocation,@MappedLocations),
					@GroupId,
					SetId,
					[CreatedBy],
					getdate(),
					[ModifiedBy],
					getdate(),
					[IsDeleted],
					@NewUnitName,
					@NewUnitName,
					[UEID],
					[OccupiedSpaceBelow],
					[IsFutureElevator],
					@PositionName,
					@MappedLocationJsonz ,
					[WorkflowStatus] 
					from Units where UnitId =@UnitId

			/**get new UnitId**/
			set @Result =scope_Identity()
			select UnitId as UnitId,[Designation] as UnitName from units where unitId=@Result
			set @OldLocation=(select location from units where unitId= @UnitId)
			  insert into Doors(GroupConfigurationId,
							UnitId,
							DoorType,
							DoorValue,
							DoorJson,
							CreatedBy,
							CreatedOn,
							ModifiedBy,
							ModifiedOn,
							IsDeleted)
			select @GroupId,
				@Result,
				 Replace(DoorType,@OldLocation,@MappedLocations) ,
				DoorValue,
				 Replace(DoorJson,@OldLocation,@MappedLocations) ,
				CreatedBy,
				getdate(),
				ModifiedBy,
				getdate(),
				IsDeleted from Doors where Doors.UnitId =@UnitId and Doors.IsDeleted=0
			INSERT INTO [dbo].[OpeningLocation]
			   ([GroupConfigurationId]
			   ,[UnitId]
			   ,[Travelfeet]
			   ,[TravelInch]
			   ,[OcuppiedSpaceBelow]
			   ,[NoOfFloors]
			   ,[FrontOpening]
			   ,[RearOpening]
			   ,[SideOpening]
			   ,[FloorDesignation]
			   ,[FloorNumber]
			   ,[Front]
			   ,[Side]
			   ,[Rear]
			   ,[CreatedBy]
			   ,[CreatedOn]
			   ,[ModifiedBy]
			   ,[ModifiedOn]
			   ,[IsDeleted])
			select @GroupId
			   ,@Result
			   ,[Travelfeet]
			   ,[TravelInch]
			   ,[OcuppiedSpaceBelow]
			   ,[NoOfFloors]
			   ,[FrontOpening]
			   ,[RearOpening]
			   ,[SideOpening]
			   ,[FloorDesignation]
			   ,[FloorNumber]
			   ,[Front]
			   ,[Side]
			   ,[Rear]
			   ,[CreatedBy]
			   ,getdate()
			   ,[ModifiedBy]
			   ,getdate()
			   ,[IsDeleted] from openinglocation where openinglocation.UnitId= @UnitId and openinglocation.IsDeleted=0 order by OpeningLocationId
		   
			
			

				Select @Result,@GroupId,CreatedBy,getdate() from units where UnitId=@Result and GroupConfigurationId= @GroupId
			declare @ueidValue nvarchar(30)
			set @ueidValue=(select distinct(ueid) from units where unitId=@Result)
			declare @countryValue nvarchar(10)
			set @countryValue=SUBSTRING(@ueidValue, 3,2)
			declare @type nvarchar(10)
			set @type=SUBSTRING(@ueidValue, 1,2)
			declare @factoryCountry nvarchar(10)
			set @factoryCountry=SUBSTRING(@ueidValue, 15,2)
			EXEC usp_GenerateUEID @type,@countryValue,@factoryCountry, @UEID = @UEID OUTPUT 
			update Units set UEID =@UEID  where setId>0 and unitId=@Result --and uts.GroupConfigurationId in(select GroupId from GroupConfiguration where BuildingId=@Result)

			--insert into HallRiser(GroupConfigurationId,
			--				 UnitId,
			--				 HallRiserType,
			--				 HallRiserValue,
			--				 HallRiserJson,
			--				 CreatedBy,
			--				 CreatedOn,
			--				 ModifiedBy,
			--				 ModifiedOn,
			--				 IsDeleted)
			--select @GroupId,
			--	@Result,
			--	replace(HallRiserType,(select substring(HallRiserType,0,5)),@NewLocation) ,
			--	HallRiserValue,
			--	'{"VariableId":"'+(replace(HallRiserType,(select substring(HallRiserType,0,5)),@NewLocation))+'","Value":"'+HallRiserValue+'"}',
			--	CreatedBy,
			--	getdate(),
			--	ModifiedBy,
			--	getdate(),
			--	IsDeleted from HallRiser where HallRiser.UnitId = @UnitId and HallRiser.IsDeleted=0
	
			if(exists(select * from @CarPositionList where [CarPositionID]>@LocationId))
			begin
				set @condition=1
			
			end
			else
			begin
				set @condition=0
			
			end

			end
		
		update Units set MappedLocationJson=@MappedLocationJsonZ where GroupConfigurationId=@GroupId
			set @maxSetId=(select max(setId) from units where UnitId in (select UnitId from @UnitList))
		
		
			if(@maxSetId>0)
			begin
				if (@GroupId in (Select distinct(GroupConfigurationId) from Units where UnitId in (select UnitId from @UnitList) ))
				begin 	
					update units set SetId=@maxSetId where GroupConfigurationId=@GroupId and  [Location] in (select Location from Units where UnitId in (select UnitId from @UnitList))
					--update Units set MappedLocationJson=@MappedLocation where setId=@maxSetId
					
					declare @perviousStatus nvarchar(100)
					set @perviousStatus = (select distinct WorkflowStatus from Units where UnitId = @UnitId)		
					update Units
					set WorkflowStatus = case when (select Step from Status where StatusKey = @perviousStatus)>(select Step from Status where StatusKey = 'UNIT_COM') 
					then 'UNIT_COM' else @perviousStatus end
					where SetId = @maxSetId
					update Systems
					set StatusKey = case when (select Step from Status where StatusKey = @perviousStatus)>(select Step from Status where StatusKey = 'UNIT_COM') 
					then 'UNIT_COM' else @perviousStatus end
					where SetId = @maxSetId
				end
				else 
				begin
					insert into UnitSet (Name,Description,ProductName,CreatedBy,CreatedOn) 
					select @NewSetName,@NewSetName,ProductName,CreatedBy,getdate() from UnitSet where SetId in (select SetId from Units where UnitId =@UnitId)
					--update Units set MappedLocationJson=@MappedLocation where setId=@maxSetId
					update TempSet set NewSetid = SCOPE_IDENTITY() where [OldSetId] = (select SetId from Units where UnitId =@UnitId)
					update units set setId=scope_identity() where [Location] in (select CarPositionLocation from @CarPositionList) and GroupConfigurationId=@GroupId
					set @maxSetId = SCOPE_IDENTITY()

					update Units
					set WorkflowStatus = 'UNIT_CNV' where SetId =  SCOPE_IDENTITY()

					--update units set setId=scope_identity() where unitId=@Result
					--Copy the unitConfiguration data--
					insert into UnitConfiguration
						(
							SetId,
							ConfigureVariables,
							ConfigureValues,
							ConfigureJson,
							CreatedBy,
							CreatedOn,
							ModifiedBy,
							ModifiedOn,
							IsDeleted
						) 
					select	
						@maxSetId,
						[ConfigureVariables],
						[ConfigureValues],
						ConfigureJson,
						CreatedBy,
						GetDate(),
						ModifiedBy,
						Getdate(),
						[IsDeleted] from [UnitConfiguration] 
						where [UnitConfiguration].SetId in (Select SetId from Units where UnitId=@UnitId and IsDeleted=0) 
				insert into TempUnitHallFixture([OldSetId],[ConsoleName],[OldConsoleId])
				select SetId,[Name],UnitHallFixtureConsoleId from unithallfixtureConsole where SetId in(select SetId from Units where unitId=@UnitId)
				declare @maxConsoleId int
			declare @maxConfigurationId int
			declare @maxLocationId int
			set @maxConsoleId=(select max(UnitHallFixtureConsoleId) from UnitHallFixtureConsole )
			set @maxConfigurationId =(select max(UnitHallFixtureConfigurationId) from UnitHallFixtureConfiguration )
			set @maxLocationId =(select max(UnitHallFixtureLocationId) from UnitHallFixtureLocations )

			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,getdate() from UnitHallFixtureConfiguration where UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId  in(select SetId from Units where unitId=@UnitId))
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,front,Rear,CreatedBy,CreatedOn)
			select UnitHallFixtureConsoleId,FloorNumber,front,Rear,CreatedBy,getdate() from UnitHallFixtureLocations where UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId  in(select SetId from Units where unitId=@UnitId))
			insert into UnitHallFixtureConsole(ConsoleNumber,FixtureType,SetId,[Name],IsController,CreatedBy,CreatedOn,IsDeleted) 
			select ConsoleNumber,FixtureType,SetId,[Name],IsController,CreatedBy,getDate(),IsDeleted from UnitHallFixtureConsole where SetId in(select SetId from Units where unitId=@UnitId)
	
			update TempUnitHallFixture set [newConsoleId]=UnitHallFixtureConsoleId from UnitHallFixtureConsole join TempUnitHallFixture on UnitHallFixtureConsole.SetId=TempUnitHallFixture.OldSetId and TempUnitHallFixture.ConsoleName= UnitHallFixtureConsole.Name where UnitHallFixtureConsole.UnitHallFixtureConsoleId>@maxConsoleId
			update UnitHallFixtureConsole set SetId=TempSet.NewSetid from TempSet inner join UnitHallFixtureConsole on TempSet.OldSetId=UnitHallFixtureConsole.SetId where UnitHallFixtureConsole.UnitHallFixtureConsoleId>@maxConsoleId
			update UnitHallFixtureConfiguration set UnitHallFixtureConsoleId=newConsoleId from TempUnitHallFixture join UnitHallFixtureConfiguration on TempUnitHallFixture.OldConsoleId=UnitHallFixtureConfiguration.UnitHallFixtureConsoleId where UnitHallFixtureConfiguration.UnitHallFixtureConfigurationId>@maxConfigurationId
			update UnitHallFixtureLocations set UnitHallFixtureConsoleId=newConsoleId from TempUnitHallFixture join UnitHallFixtureLocations on TempUnitHallFixture.OldConsoleId=UnitHallFixtureLocations.UnitHallFixtureConsoleId where UnitHallFixtureLocations.UnitHallFixtureLocationId>@maxLocationId
																							--and [UnitConfiguration].IsDeleted=0
	
			set @maxConsoleId=(select max(EntranceConsoleId) from EntranceConsole )
			set @maxConfigurationId =(select max(EntranceConfigurationId) from EntranceConfiguration )
			set @maxLocationId =(select max(EntranceLocationId) from EntranceLocations )

			delete from TempUnitHallFixture where OldSetId in (select SetId from Units where unitId=@UnitId)

			insert into TempUnitHallFixture([OldSetId],[ConsoleName],[OldConsoleId])
			select SetId,[Name],EntranceConsoleId from EntranceConsole where SetId in(select SetId from Units where unitId=@UnitId)
			--select EntranceConsoleId,VariableType,VariableValue,CreatedBy,getdate() from EntranceConfiguration where EntranceConsoleId in (select EntranceConsoleId from EntranceConsole where SetId in(select SetId from Units where unitId=@UnitId))
			insert into EntranceLocations(EntranceConsoleId,FloorNumber,front,Rear,CreatedBy,CreatedOn)
			select EntranceConsoleId,FloorNumber,front,Rear,CreatedBy,getdate() from EntranceLocations where EntranceConsoleId in (select EntranceConsoleId from EntranceConsole where SetId in (select SetId from Units where unitId=@UnitId))
			insert into EntranceConsole(ConsoleNumber,SetId,[Name],IsController,CreatedBy,CreatedOn,IsDeleted) 
			select ConsoleNumber,SetId,[Name],IsController,CreatedBy,getDate(),IsDeleted from EntranceConsole where SetId in(select SetId from Units where unitId=@UnitId)
			update TempUnitHallFixture set [newConsoleId]=EntranceConsoleId from EntranceConsole join TempUnitHallFixture on EntranceConsole.SetId=TempUnitHallFixture.OldSetId and TempUnitHallFixture.ConsoleName= EntranceConsole.Name where EntranceConsole.EntranceConsoleId>@maxConsoleId
			update EntranceConsole set SetId=TempSet.NewSetid from TempSet inner join EntranceConsole on TempSet.OldSetId=EntranceConsole.SetId where EntranceConsole.EntranceConsoleId>@maxConsoleId
			update EntranceConfiguration set EntranceConsoleId=newConsoleId from TempUnitHallFixture join EntranceConfiguration on TempUnitHallFixture.OldConsoleId=EntranceConfiguration.EntranceConsoleId where EntranceConfiguration.EntranceConfigurationId>@maxConfigurationId
			update EntranceLocations set EntranceConsoleId=newConsoleId from TempUnitHallFixture join EntranceLocations on TempUnitHallFixture.OldConsoleId=EntranceLocations.EntranceConsoleId where EntranceLocations.EntranceLocationId>@maxLocationId

	end
	exec [dbo].[usp_UpdateWorkflowStatus]@GroupId,'group'
end
	END TRY
		BEGIN CATCH
		declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
			 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	END CATCH
END


