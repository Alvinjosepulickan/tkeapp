


CREATE PROCEDURE [dbo].[usp_UpdateBuildingElevation]


@elevationData as tblBE readonly,
@VariableMapperDataTable AS VariableMapper READONLY

,@Result int OUTPUT

as

BEGIN
	BEGIN TRY
		 set nocount on;

		DECLARE @BuildingLandings NVARCHAR(250)
		DECLARE @TotalBuildingFloorToFloorHeight NVARCHAR(250)
		SET @TotalBuildingFloorToFloorHeight = (select VariableType from @VariableMapperDataTable where variableKey = 'TOTALBUILDINGFLOORTOFLOORHEIGHT')
		SET @BuildingLandings = (select VariableType from @VariableMapperDataTable where variableKey = 'BLANDINGS')

		  declare @buildingId int,@workflowstatus nvarchar(100)

		  set @buildingId=(select distinct buildingId from @elevationData)
		  UPDATE GroupConfiguration SET WorkflowStatus='GRP_CNV' WHERE BuildingId=@buildingId --and ConflictCheck=0
		  UPDATE Units SET WorkflowStatus='UNIT_CNV' WHERE GroupConfigurationId IN (SELECT GroupId FROM GroupConfiguration WHERE BuildingId=@buildingId) --and ConflictCheck=0
		

		  declare @noOfFloor int

		  set @noOfFloor=(select distinct([noOfFloor]) from @elevationData)

		  declare @buildingelevation [numeric](20, 10)

		  set @buildingelevation=(select distinct([buildingRise]) from @elevationData)

		 if exists (select buildingId from BuildingElevation where buildingId=@buildingId)

		 begin

		  declare @userId varchar(50)

		  declare @date datetime

		  declare @buildingJsonString nvarchar(max)

		  declare @numberOfFloorsOld nvarchar(max)

		  declare @numberOfFloorsNew nvarchar(max)

		  declare @buildingriseOld nvarchar(max)

		  declare @buildingriseNew nvarchar(max)

		  declare @numberOfFloors nvarchar(max)

		  declare @buildingRise nvarchar(max)

		  set @userID=(select distinct userId from @elevationData);

		  set @buildingJsonString =(select '['+ SUBSTRING( 
					( 
					 SELECT ',{"VariableId":"'+k.BuindingType+'","Value":"'+k.BuindingValue+'"}'
					from building b
					Left Join BuildingConfiguration k
					on b.Id = k.BuildingId
					where b.id = @buildingId
					and b.isDeleted=0
							 FOR XML PATH('') 
				), 2 , 9999) + ']' As BldJson)

		  set @numberOfFloors=isnull([dbo].[FnGetBuildingTableValueFromBldJson](@buildingId,@BuildingLandings),2)



  
		  set @numberOfFloorsOld=',{"VariableId":@BuildingLandings,"Value":'+@numberOfFloors+'},'

		  set @numberOfFloorsNew=',{"VariableId":@BuildingLandings,"Value":'+CAST(@noOfFloor as nvarchar(max))+'},'

		  --update number of floors

		  set @buildingJsonString= replace(@buildingJsonString,@numberOfFloorsOld,@numberOfFloorsNew)
  
		  set @numberOfFloorsOld=',{"VariableId":@BuildingLandings,"Value":"'+@numberOfFloors+'"},'

		  set @numberOfFloorsNew=',{"VariableId":@BuildingLandings,"Value":"'+CAST(@noOfFloor as nvarchar(max))+'"},'

		  --update number of floors

		  set @buildingJsonString= replace(@buildingJsonString,@numberOfFloorsOld,@numberOfFloorsNew)
  
		  if(@numberOfFloors<>@noOfFloor)
		  begin
			insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select	@BuildingId,
					@BuildingLandings,
					@noOfFloor,
					@numberOfFloors,
					@userId,
					GETDATE(),
					@userId,
					GETDATE()

		  end
		  if(exists (select * from BuildingConfiguration where BuildingId=@buildingId and  BuindingType=@BuildingLandings) )
		  begin
			update bldConfig
			set BuindingValue = CAST(@noOfFloor as nvarchar(max)),
			ModifiedBy = ModifiedBy,
			ModifiedOn = getdate()
			from BuildingConfiguration bldConfig
			where BuildingId = @BuildingId
			and BuindingType = @BuildingLandings
			end
			else
			begin
				insert into BuildingConfiguration (BuildingId,BuindingType,BuindingValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
				values(@buildingId,@BuildingLandings,CAST(@noOfFloor as nvarchar(max)),@userID,getdate(),@userID,getdate())
			end
		  set @buildingRise=[dbo].[FnGetBuildingTableValueFromBldJson](@buildingId,@TotalBuildingFloorToFloorHeight)
  

		  set @buildingriseOld=',{"VariableId":@TotalBuildingFloorToFloorHeight,"Value":"'+@buildingRise+'"}'

		  set @buildingriseNew=',{"VariableId":@TotalBuildingFloorToFloorHeight,"Value":"'+CAST(@buildingelevation as nvarchar(max))+'"}'
		  --update building Rise

		  set @buildingJsonString= replace(@buildingJsonString,@buildingriseOld,@buildingriseNew)
  
		  set @buildingriseOld=',{"VariableId":@TotalBuildingFloorToFloorHeight,"Value":'+@buildingRise+'},'

		  set @buildingriseNew=',{"VariableId":@TotalBuildingFloorToFloorHeight,"Value":'+CAST(@buildingelevation as nvarchar(max))+'},'
		  --update building Rise

		  set @buildingJsonString= replace(@buildingJsonString,@buildingriseOld,@buildingriseNew)
  
		  /*update Building set BldJson=@buildingJsonString where id=@buildingId*/
		  if(@buildingRise<>@buildingelevation)
		  begin
			insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select	@BuildingId,
					@TotalBuildingFloorToFloorHeight,
					@buildingelevation,
					@buildingRise,
					@userId,
					GETDATE(),
					@userId,
					GETDATE()

		  end
		  update bldConfig
		  set BuindingValue = CAST(@buildingelevation as nvarchar(max)),
  			ModifiedBy = ModifiedBy,
  			ModifiedOn = getdate()
		  from BuildingConfiguration bldConfig
		  where BuildingId = @BuildingId
		  and BuindingType = @TotalBuildingFloorToFloorHeight

  
		  set @date=(select distinct createdOn from BuildingElevation where buildingId = (select distinct buildingId from @elevationData));

		  --inserting Modified entries of Floor Designation to History table
		  insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		  select  ble.BuildingId,
				  concat('Floor Designation - ',cast(ed.floorNumber as nvarchar(10))),
				  ed.floorDesignation,
				  ble.FloorDesignation,
				  @userId,getdate(),@userId,getdate()
		  from BuildingElevation ble
		  join @elevationData ed on ble.floorNumber=ed.floorNumber and ble.BuildingId=ed.buildingId
		  where ble.FloorDesignation<>ed.floorDesignation


		  --inserting Modified entries of Floor to floor Height to History table
		  insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		  select  ble.BuildingId,
				  concat('Floor to Floor Height - ',cast(ed.floorNumber as nvarchar(10))),
				  concat(cast(ed.floorToFloorHeightFeet as nvarchar(50)),concat(',',cast(ed.floorToFloorHeightInch as nvarchar(50)))),
				  concat(cast(ble.floorToFloorHeightFeet as nvarchar(50)),concat(',',cast(ble.floorToFloorHeightInch as nvarchar(50)))),
				  @userId,getdate(),@userId,getdate()
		  from BuildingElevation ble
		  join @elevationData ed on ble.floorNumber=ed.floorNumber and ble.BuildingId=ed.buildingId
		  where ble.FloorToFloorHeightFeet<>ed.floorToFloorHeightFeet or ble.FloorToFloorHeightInch<>ed.floorToFloorHeightInch

		  --inserting modified entries of editable elevation to History table
		  insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		  select  ble.BuildingId,
				  N'Elevation - 1',
				  concat(cast(ed.elevationFeet as nvarchar(50)),concat(',',cast(ed.elevationInch as nvarchar(50)))),
				  concat(cast(ble.elevationFeet as nvarchar(50)),concat(',',cast(ble.elevationInch as nvarchar(50)))),
				  @userId,getdate(),@userId,getdate()
		  from BuildingElevation ble
		  join @elevationData ed on ble.floorNumber=ed.floorNumber and ble.BuildingId=ed.buildingId
		  where (ble.elevationFeet<>ed.elevationFeet or ble.elevationInch<>ed.elevationInch)
		  and ble.floorNumber=1
		  --inserting modified entries of Main egress to History table
		  declare @egressnew int,@egressold int
		  set @egressnew=(select floornumber from @elevationData where mainEgress=1)
		  set @egressold=(select floornumber from BuildingElevation where Buildingid=@buildingId and mainEgress=1)
		  if(@egressnew<>@egressold)
		  begin
			insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select  @buildingId,'Main Egress',@egressnew,@egressold,@userId,getdate(),@userId,getdate()
		  end
		  --inserting modified entries of Alternate egress to History table
		  set @egressnew=(select floornumber from @elevationData where AlternateEgress=1)
		  set @egressold=(select floornumber from BuildingElevation where Buildingid=@buildingId and alternateEgress=1)
		  if(@egressnew<>@egressold)
		  begin
			insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select  @buildingId,'Alternate Egress',@egressnew,@egressold,@userId,getdate(),@userId,getdate()
		  end
		  delete from BuildingElevation where buildingId = (select distinct buildingId from @elevationData);

		  insert into BuildingElevation([buildingId],
										[floorDesignation],
										[elevationFeet],
										[elevationInch],
										[floorToFloorHeightFeet],
										[floorToFloorHeightInch]
										,[mainEgress],
										[modifiedBy],
										[modifiedOn],
										[AlternateEgress],
										[noOfFloor],
										[buildingRise],
										[floorNumber])

		  select [buildingId],[floorDesignation],[elevationFeet],[elevationInch],[floorToFloorHeightFeet],[floorToFloorHeightInch],[mainEgress],[userId],[date],[AlternateEgress],[noOfFloor],[buildingRise],[floorNumber] from @elevationData;

		  update BuildingElevation set createdBy=@userId,createdOn=@date where buildingId =(select distinct buildingId from @elevationData);

		  SET @Result= 1;

		 end

		 else

		 begin



			   insert into BuildingElevation([buildingId],[floorDesignation],[elevationFeet],[elevationInch],[floorToFloorHeightFeet],[floorToFloorHeightInch],[mainEgress],[createdBy],[createdOn])

			   select [buildingId],[floorDesignation],[elevationFeet],[elevationInch],[floorToFloorHeightFeet],[floorToFloorHeightInch],[mainEgress],[userId],[date] from @elevationData;

			SET @Result =2;

		 end
		 
		 --update workflowstatus to complete if the status is incomplete
		 UPDATE Building SET workflowstatus='BLDG_COM' WHERE id=@buildingId and workflowstatus='BLDG_INC'

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




