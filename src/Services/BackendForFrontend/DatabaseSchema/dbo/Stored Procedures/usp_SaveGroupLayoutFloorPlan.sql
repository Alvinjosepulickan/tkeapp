
CREATE PROCEDURE [dbo].[usp_SaveGroupLayoutFloorPlan]
@GroupConfigurationId int,
@UnitVariables as unitDataTable READONLY,
@UnitVariablesHallRiser as layoutDataTable READONLY,
@UnitVariablesDoor as layoutDataTable READONLY, 
@UnitVariableControlLocation as layoutDataTable READONLY,
--@UnitFutureElevatorValue as unitTableFutureElevatorAssignment READONLY,
@IsEditFlows as nvarchar(30),
@CreatedBy nvarchar(50),
@fdaJsonData nvarchar(max),
@Result INT OUTPUT
AS
BEGIN
	BEGIN TRY
		BEGIN
			--get number of units
			declare @numberOfUnits int
			declare @buildingId int
			declare @numberOfDuplicates int
			set @buildingId=(select BuildingId from GroupConfiguration where GroupId=@GroupConfigurationId)
			set @numberOfUnits=(select count(*) from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId = (select BuildingId from GroupConfiguration where GroupId=@GroupConfigurationId) and isDeleted=0))

			delete from units where GroupConfigurationId=@GroupConfigurationId and [Location] in (select Location from @UnitVariables where UnitDesignation='')
			--autogenerate unitname
			declare @UnitData as unitDataTable

			declare @DuplicateUnitData as unitDataTable
			insert into @UnitData(
				[UnitId],
				[UnitJson] ,
				[value] ,
				[UnitJsonData] ,
				[DisplayUnitJson] ,
				[Location] ,
				[MappedLocationJson] ,
				[UnitName] ,
				[UnitDesignation] ,
				[IsFutureElevator] 
					) 
					select 
					[UnitId],
					[UnitJson] ,
					[value] ,
					[UnitJsonData] ,
					[DisplayUnitJson] ,
					[Location] ,
					[MappedLocationJson] ,
					[UnitName] ,
					[UnitDesignation] ,
					[IsFutureElevator] 
					from @UnitVariables
			set @numberOfUnits=(select count(*) from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId))
			declare @minUnitId int
			declare @maxUnitId int
			set @minUnitId=(Select min([UnitId]) from @UnitData)
			set @maxUnitId=(Select max([UnitId]) from @UnitData)
			declare @UnitName nvarchar(10)
			declare @unitNameExists bit
			declare @unitNameCheck bit
			while @minUnitId<=@maxUnitId
			begin
				set @unitNameCheck=1
				while(@unitNameCheck=1)
				begin
					if(not exists(select * from units where [Location]=(select [Location] from @UnitData where UnitId =@minUnitId and UnitDesignation<>'') and GroupConfigurationId=@GroupConfigurationId))
					begin
						set @unitNameExists =0
						set @numberOfUnits+=1
						set @UnitName='U'+CAST(@numberOfUnits as nvarchar(3))
						if(exists (select * from units where [Designation]=@UnitName and GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@BuildingId)))
						begin
							set @unitNameExists=1
						end
						if(exists (select * from @UnitData where [UnitDesignation]=@UnitName))
						begin
							set @unitNameExists=1
						end
						if(@unitNameExists=0)
						begin
							update @UnitData set [UnitDesignation]=@UnitName where [UnitId]=@minUnitId AND UnitDesignation=''
							set @unitNameCheck=0
							set @minUnitId+=1
						end
					end
					else
					begin
						set @minUnitId+=1
					end
				end
			end
		
			--update @UnitData set [UnitDesignation]=unitTable.[UnitDesignation] from @UnitVariables as unitTable inner join @UnitData as unitData on unitData.[Location]=unitTable.[Location] where unitTable.UnitDesignation<>''
			if(Exists(SELECT [UnitDesignation]
				FROM @UnitData
				GROUP BY [UnitDesignation]
				HAVING COUNT([UnitDesignation]) > 1))
			begin
				set @Result=-1
			end
			if((select count(*) from (select [UnitDesignation],[Location] from @UnitData
									  where [UnitDesignation] in (select [Designation] from units
										where GroupConfigurationId in (select GroupId from GroupConfiguration 
											where IsDeleted=0 and BuildingId=@buildingId and GroupId<>@GroupConfigurationId)))as duplicate)>0)
			begin
				set @Result=-1 
			end
			if(@Result=-1)
			begin
			select @Result as Result
		
				select uts.[UnitDesignation],uts.MappedLocationJson as [Location]
				from @UnitData as uts 
					join 
				Units as ut on 
					 ut.[Designation]=uts.[UnitDesignation] where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@buildingId) and GroupConfigurationId<>@GroupConfigurationId
			union
			select uts.[UnitDesignation],uts.MappedLocationJson as [Location]
				from @UnitData as uts 
					join 
				Units as ut on 
					 ut.[Designation]=uts.[UnitDesignation]and ut.[Location]<>uts.[Location] where GroupConfigurationId =@GroupConfigurationId
			union 
			select [UnitDesignation],[Location] from @UnitData where [UnitDesignation] in (SELECT [UnitDesignation]
				FROM @UnitData
				GROUP BY [UnitDesignation]
				HAVING COUNT([UnitDesignation]) > 1)

			return 0
			end
			--select * from @UnitData
			 --as units
			--unit designation validation
			/** History Table **/
						--Units 
						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,createdby,createdon,modifiedby,modifiedon)
						select @GroupConfigurationId,Location,'TRUE','',@CreatedBy,getdate(),@CreatedBy,getdate()
						from @UnitData where Location not in (select distinct Location from Units where GroupConfigurationId=@GroupConfigurationId)

						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,createdby,createdon,modifiedby,modifiedon)
						select @GroupConfigurationId,Location,'FALSE','TRUE',@CreatedBy,getdate(),@CreatedBy,getdate()
						from Units where GroupConfigurationId=@GroupConfigurationId and Location not in(select Location from @UnitData)

						--Unit Designation

						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,createdby,createdon,modifiedby,modifiedon)
						select @GroupConfigurationId,'Unit Designation',ud.UnitDesignation,u.Designation,@CreatedBy,getdate(),@CreatedBy,getdate()
						from Units u inner join @UnitData ud on u.Name=ud.UnitName and u.Location=ud.Location and u.Designation<>ud.UnitDesignation
						where u.GroupConfigurationId=@GroupConfigurationId 

						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,createdby,createdon,modifiedby,modifiedon)
						select @GroupConfigurationId,'Unit Designation',UnitDesignation,'',@CreatedBy,getdate(),@CreatedBy,getdate()
						from @UnitData where Location not in (select distinct Location from Units where GroupConfigurationId=@GroupConfigurationId)

					
						--Future Elevator
						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,createdby,createdon,modifiedby,modifiedon)
						select @GroupConfigurationId,concat(UnitDesignation,' - Future'),'TRUE','',@CreatedBy,getdate(),@CreatedBy,getdate()
						from @UnitData where Location not in (select distinct Location from Units where GroupConfigurationId=@GroupConfigurationId) and IsFutureElevator=1

						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,createdby,createdon,modifiedby,modifiedon)
						select @GroupConfigurationId,concat(UnitDesignation,' - Future'),case when ud.IsFutureElevator=0 then 'FALSE' else 'TRUE' end,
							   case when u.IsFutureElevator=0 then 'FALSE' else 'TRUE' end,@CreatedBy,getdate(),@CreatedBy,getdate()
						from Units u inner join @UnitData ud on u.Name=ud.UnitName and u.Location=ud.Location and u.IsFutureElevator<>ud.IsFutureElevator
						where u.GroupConfigurationId=@GroupConfigurationId 

						--Control Location
						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
						Select @GroupConfigurationId,ucl.UnitJson, ucl.value,cl.ControlLocationValue,@CreatedBy,GETDATE(),@CreatedBy,GETDATE()
						from @UnitVariableControlLocation ucl join ControlLocation cl 
						on ucl.UnitJson=cl.ControlLocationType and ucl.value<>cl.ControlLocationValue
						where cl.GroupConfigurationId = @GroupConfigurationId and 
						ucl.UnitJson not in(select ControlLocationType from ControlLocation where ControlLocationType like '%controlRoomQuad_SP%')

						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
						Select @GroupConfigurationId,'Control Position Changed','YES','',@CreatedBy,GETDATE(),@CreatedBy,GETDATE()
						from @UnitVariableControlLocation ucl join ControlLocation cl 
						on ucl.UnitJson=cl.ControlLocationType and ucl.value<>cl.ControlLocationValue
						where cl.GroupConfigurationId = @GroupConfigurationId and 
						ucl.UnitJson  like '%controlRoomQuad_SP%'

						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
						Select @GroupConfigurationId,ucl.UnitJson, ucl.value,'',@CreatedBy,GETDATE(),@CreatedBy,GETDATE()
						from @UnitVariableControlLocation ucl 
						where ucl.UnitJson not in (select distinct ControlLocationType From ControlLocation where GroupConfigurationId=@GroupConfigurationId)
						and ucl.UnitJson not like '%controlRoomQuad_SP%'--in(select ControlLocationType from ControlLocation where ControlLocationType like '%controlRoomQuad_SP%')
						--Doors
						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
						Select distinct @GroupConfigurationId,
									case when ud.UnitJson like '%Parameters_SP.rearDoorTypeAndHand_SP%' then Concat(u.Designation,' - Rear opening') else Concat(u.Designation,' - Front opening') end,
									ud.value,d.DoorValue,@CreatedBy,GETDATE(),@CreatedBy,GETDATE()
						from @UnitVariablesDoor ud join Doors d
						on ud.UnitJson=d.DoorType and ud.value<>d.DoorValue
						join Units u on u.UnitId=d.UnitId
						where d.GroupConfigurationId = @GroupConfigurationId

						insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
						Select @GroupConfigurationId,
							   case when ud.UnitJson like '%Parameters_SP.rearDoorTypeAndHand_SP%' then Concat(u.Designation,' - Rear opening') else Concat(u.Designation,' - Front opening') end,
							   ud.value,'',@CreatedBy,GETDATE(),@CreatedBy,GETDATE()
						from @UnitVariablesDoor ud join units u on u.Name=ud.unitName 
						where ud.UnitJson not in (select distinct DoorType From Doors where GroupConfigurationId=@GroupConfigurationId)
						and u.GroupConfigurationId=@GroupConfigurationId

						insert into GroupConfigHistory(GroupId,VariableId,PreviousValue,CurrentValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
						Select @GroupConfigurationId,
							   case when d.DoorType like '%Parameters_SP.rearDoorTypeAndHand_SP%' then Concat(u.Designation,' - Rear opening') else Concat(u.Designation,' - Front opening') end,
							   d.DoorValue,'',@CreatedBy,GETDATE(),@CreatedBy,GETDATE()
						FROM Doors d join Units u on u.UnitId=d.UnitId
						where d.GroupConfigurationId=@GroupConfigurationId and
						d.DoorType not in(select distinct UnitJson from @UnitVariablesDoor)

						/** History Table **/
			
		
			if(exists (select * from units where groupconfigurationid=@GroupConfigurationId))
				begin
					exec [dbo].[usp_InsertUnitDetails] @GroupConfigurationId
					update TempUnitTable set oldunitId=Units.Unitid,setId=Units.SetId,Designation=Units.Designation,[Description]=Units.[Description],[UEID]=units.UEID, WorkflowStatus = Units.WorkflowStatus
					from TempUnitTable inner join units on TempUnitTable.GroupConfigurationId=Units.GroupConfigurationId and Units.[Name]=TempUnitTable.UnitName
					if(exists (select * from @UnitData))
					begin
						delete from units where groupconfigurationid=@GroupConfigurationId
					    if (exists(select * from @UnitVariablesDoor))
						begin
							delete from doors where groupconfigurationid=@GroupConfigurationId
						end	
						if (exists(select * from @UnitVariablesHallRiser))
						begin
							delete from hallriser where groupconfigurationid=@GroupConfigurationId
						end
						if (exists(select * from @UnitVariableControlLocation))
						begin
							delete from ControlLocation where groupconfigurationid=@GroupConfigurationId
						end
					end

				
					--DELETE FROM UnitConfiguration where SetId in (select setid from units where GroupConfigurationId=@GroupConfigurationId)
					set @Result=1
			end
			if(@fdaJsonData <>'[]')
			begin
				DECLARE @TempTable table (id int, VariableId nvarchar(255),[Value] nvarchar(255))

				INSERT INTO @TempTable
				SELECT @GroupConfigurationId id, 
				JSON_VALUE(value,'$.variableId') as VariableId , 
				JSON_VALUE(value,'$.value') as [Value]
				FROM OPENJSON(@fdaJsonData);
				/*Basic Log*/
				insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
				Select @GroupConfigurationId,gc.GroupConfigurationType,temp.Value,gc.GroupConfigurationValue,@CreatedBy,getdate(),@CreatedBy,getdate()
				from GroupConfigurationDetails gc join @TempTable temp
				on gc.GroupConfigurationType=temp.VariableId and gc.GroupConfigurationValue<>temp.Value
				where GroupId=@GroupConfigurationId

				insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
				Select @GroupConfigurationId,VariableId,Value,'',@CreatedBy,getdate(),@CreatedBy,getdate()
				from @TempTable
				where VariableId not in (select GroupConfigurationType from GroupConfigurationDetails 
											 where GroupId=@GroupConfigurationId and (GroupConfigurationType like '%OFF%' or GroupConfigurationType like '%DIST%')) 
				/*Basic Log*/
				delete from GroupConfigurationDetails where GroupId=@GroupConfigurationId and GroupConfigurationType like '%OFF%' 
				delete from GroupConfigurationDetails where GroupId=@GroupConfigurationId and GroupConfigurationType like '%DIST%' 
				INSERT INTO [dbo].[GroupConfigurationDetails]
					(
					GroupId,
					BuildingId,
					GroupConfigurationType,
					GroupConfigurationValue,
					CreatedBy
					)
					Select
					@GroupConfigurationId,
					@BuildingId,
					VariableId ,
					Value,
					@CreatedBy
					FROM @TempTable

	
			end
			insert into units([Name],[Location],[UnitJson],[GroupConfigurationId],SetId,[CreatedBy],[CreatedOn],[Designation],[Description],[MappedLocation],[MappedLocationJson],OccupiedSpaceBelow,IsFutureElevator)
				select [UnitName],[Location],[UnitJsonData],@GroupConfigurationId,0,@CreatedBy,GETDATE(),[UnitDesignation],[UnitDesignation],[MappedLocationJson],[DisplayUnitJson],0 ,IsFutureElevator from @UnitData	
			UPDATE GroupConfiguration SET ConflictStatus=@IsEditFlows WHERE GroupId =@GroupConfigurationId

		
		
		
			update TempUnitTable set newunitId=Units.Unitid
			from TempUnitTable inner join units on TempUnitTable.GroupConfigurationId=Units.GroupConfigurationId and Units.[Name]=TempUnitTable.UnitName
			--update TempUnitTable set Designation=Units.Designation from TempUnitTable inner join units on TempUnitTable.GroupConfigurationId=Units.GroupConfigurationId and Units.UnitId=TempUnitTable.NewUnitId where TempUnitTable.Designation=''
			--update TempUnitTable set [Description]=Units.[Description] from TempUnitTable inner join units on TempUnitTable.GroupConfigurationId=Units.GroupConfigurationId and Units.UnitId=TempUnitTable.NewUnitId where TempUnitTable.[Description]=''
			--select * from TempUnitTable
			update Units set setId=TempUnitTable.SetId,UEID=TempUnitTable.UEID,
			[Description] = case when TempUnitTable.Description != '' then TempUnitTable.Description else units.Description end 
			,WorkflowStatus = TempUnitTable.WorkflowStatus from TempUnitTable inner join units on TempUnitTable.GroupConfigurationId=Units.GroupConfigurationId and Units.[Name]=TempUnitTable.UnitName
			update OpeningLocation set UnitId=TempUnitTable.newUnitId from OpeningLocation inner join TempUnitTable on OpeningLocation.GroupConfigurationId=TempUnitTable.groupconfigurationId and OpeningLocation.UnitId=TempUnitTable.oldunitId and TempUnitTable.NewUnitId>0
			update GroupHallFixtureLocations set UnitId=TempUnitTable.newUnitId from GroupHallFixtureLocations inner join TempUnitTable on GroupHallFixtureLocations.UnitId=TempUnitTable.OldUnitId and TempUnitTable.NewUnitId>0
			update Doors set UnitId=TempUnitTable.newUnitId from Doors inner join TempUnitTable on Doors.GroupConfigurationId=TempUnitTable.groupconfigurationId and Doors.UnitId=TempUnitTable.oldunitId and TempUnitTable.NewUnitId>0
			delete from TempUnitTable where GroupConfigurationId=@GroupConfigurationId

			---deleting units from GroupHallFixtureLocations table which are deselected---
			--delete from GroupHallFixtureLocations where UnitId in (
			--select distinct UnitId from GroupHallFixtureLocations where UnitId not in (select UnitId from Units where GroupConfigurationId=@GroupConfigurationId)
			--and GroupHallFixtureConsoleId in (select GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@GroupConfigurationId and
			--FixtureType not in ('Traditional_Hall_Stations','AGILE_Hall_Stations'))) and GroupHallFixtureConsoleId in 
			--(select GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@GroupConfigurationId and
			--FixtureType not in ('Traditional_Hall_Stations','AGILE_Hall_Stations'))
			   		 	 
			--opening selected
			declare @openingSelected int
			select distinct  @openingSelected= count(*) from OpeningLocation where GroupConfigurationId=@GroupConfigurationId
			-- need to check this flag
			IF(@IsEditFlows='InValid')-- or @IsEditFlows='NeedValidation')
			begin
				update GroupConfiguration set  WorkflowStatus='GRP_CINV' where GroupId=@GroupConfigurationId
				update units set WorkflowStatus='UNIT_CNV' where GroupConfigurationId=@GroupConfigurationId
			end
			else
			begin


			declare @perviousStatus nvarchar(100)
			set @perviousStatus = (select distinct WorkflowStatus from GroupConfiguration where GroupId = @GroupConfigurationId)
			-- setting the unit flag by compare the pervious work flow status
			if(@perviousStatus = 'GRP_VAL')
			begin
			update GroupConfiguration set WorkflowStatus = 'GRP_COM'  where GroupId = @GroupConfigurationId
			update Units set WorkflowStatus = 'UNIT_COM' where GroupConfigurationId = @GroupConfigurationId
			update Systems set StatusKey = 'UNIT_COM' where SetId in (select distinct SetId from Units where GroupConfigurationId = @GroupConfigurationId)
			end
			else
			begin
				IF(@openingSelected>1)
				BEGIN
					If exists (select distinct count(ol.UnitId) opening from OpeningLocation ol
									right join Units u on u.UnitId=ol.UnitId
									where u.GroupConfigurationId=@GroupConfigurationId
									Group By u.UnitId having count( ol.UnitId)=0)
					begin

					UPDATE GroupConfiguration set WorkflowStatus='GRP_INC' where GroupId=@GroupConfigurationId
					end
					else
						begin
							UPDATE GroupConfiguration set WorkflowStatus='GRP_COM' where GroupId=@GroupConfigurationId
							
						end
				END
				ELSE
				BEGIN
					UPDATE GroupConfiguration set WorkflowStatus='GRP_INC' where GroupId=@GroupConfigurationId
				END

				update units set WorkflowStatus='UNIT_COM' 
				where GroupConfigurationId=@GroupConfigurationId and WorkflowStatus = 'UNIT_VAL'		

				
				update Systems set StatusKey = 
				case when  StatusKey='UNIT_VAL' then 'UNIT_COM'else StatusKey end
				where SetId in (select distinct SetId from Units where GroupConfigurationId = @GroupConfigurationId)
		
			end
			
			end
			update units set WorkflowStatus='UNIT_NCFG' 
			where GroupConfigurationId=@GroupConfigurationId and SetId = 0
			exec [dbo].[usp_UpdateWorkflowStatus]@buildingId,'building'
			declare @oppurtunityid nvarchar(20)
			select @oppurtunityid=q.OpportunityId from Quotes q inner join building b on q.QuoteId=b.QuoteId
			inner join GroupConfiguration g on g.Buildingid=b.id
			where g.groupid=@GroupConfigurationId
			exec [dbo].[usp_UpdateWorkflowStatus]@oppurtunityid,'project'


			INSERT INTO Doors([GroupConfigurationId], [UnitId], [DoorType], [DoorValue], [DoorJson], [CreatedBy], [CreatedOn])
				select @GroupConfigurationId,unit.UnitId,tblDoors.UnitJson, value, tblDoors.UnitJsonData, @CreatedBy, GETDATE()
					from @UnitVariablesDoor tblDoors join Units unit on tblDoors.UnitName =unit.[Name] where GroupConfigurationId in (@GroupConfigurationId)
			INSERT INTO HallRiser([GroupConfigurationId], [UnitId], [HallRiserType], [HallRiserValue], [HallRiserJson], [CreatedBy], [CreatedOn])
				select @GroupConfigurationId,unit.UnitId,tblHallRiser.UnitJson, value, tblHallRiser.UnitJsonData, @CreatedBy, GETDATE()
					from @UnitVariablesHallRiser tblHallRiser join Units unit on tblHallRiser.UnitName =unit.[Name] where GroupConfigurationId in (@GroupConfigurationId)
			
			if(exists(select UnitJson from @UnitVariableControlLocation where UnitJson like '%controllerLocation_SP%' and [value] in ('Remote','Adjacent')))
			begin
				if(not Exists( select UnitJson from @UnitVariableControlLocation where UnitJson like '%controlRoomFloor_SP%'))
				begin 
					set @Result=-2
					select @Result as returnValue
				end
			end
			
			INSERT INTO ControlLocation([GroupConfigurationId], [ControlLocationType], [ControlLocationValue], [ControlLocationJson], [CreatedBy], [CreatedOn])
				select @GroupConfigurationId,tblControlLocation.UnitJson, value, tblControlLocation.UnitJsonData, @CreatedBy, GETDATE()
					from @UnitVariableControlLocation tblControlLocation
			SET @Result = @GroupConfigurationId
			select @Result

			--DELETE  g from GroupHallFixtureLocations g inner join Units u on g.UnitId<>u.UnitId and u.GroupConfigurationId=@GroupConfigurationId
			--where GroupHallFixtureConsoleId not in(select GroupHallFixtureConsoleId from GroupHallFixtureConsole 
			--									where FixtureType not  in('Traditional_Hall_Stations','AGILE_Hall_Stations') and GroupId=@GroupConfigurationId)
			--/*deleteing the hallstation which are set false*/
			--DELETE g FROM GroupHallFixtureLocations g inner join ControlLocation c 
			--on g.HallStationName<>c.ControlLocationType and ControlLocationType like'%HS%'
			--WHERE GroupHallFixtureConsoleId  in(SELECT GroupHallFixtureConsoleId FROM GroupHallFixtureConsole 
			--									WHERE FixtureType in('Traditional_Hall_Stations','AGILE_Hall_Stations') and GroupId=@GroupConfigurationId)

			RETURN @Result
		END
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
