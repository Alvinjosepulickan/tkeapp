
CREATE PROCEDURE [dbo].[usp_UpdateBuildingConfiguration]
	 @Id int = 0,

    --@projectId int ,

@OpportunityId nvarchar(100),

	--@VersionId nvarchar(100),

	@quoteId nvarchar(25),

 @BldName nvarchar(50) ,

 @BldJson nvarchar(max) = NULL,

 @CreatedBy nvarchar(50),

 @ModifiedBy nvarchar(50) ,
 @IsEditFlow nvarchar(20),
 @VariableMapperDataTable AS VariableMapper READONLY,
 @Result int OUTPUT
AS
BEGIN
	BEGIN TRY
		--declare @QuoteId nvarchar(20)
		--Select @QuoteId = (Select QuoteId from Quotes where OpportunityId = @OpportunityId and VersionId = @VersionId)

		IF EXISTS(SELECT * FROM BUILDING WHERE BldName=@BldName AND QuoteId=@QuoteId AND Id!=@Id and isDeleted=0)
	BEGIN
		SET @Result=-1
		RETURN 0
	END
	ELSE
	BEGIN
		DECLARE @BuildingNameVariableId NVARCHAR(250)
		DECLARE @TotalBuildingFloorToFloorHeight NVARCHAR(250)
		SET @TotalBuildingFloorToFloorHeight = (select VariableType from @VariableMapperDataTable where variableKey = 'TOTALBUILDINGFLOORTOFLOORHEIGHT')
		SET @BuildingNameVariableId = (select VariableType from @VariableMapperDataTable where variableKey = 'BUILDINGNAMEVARIABLEID')
		DECLARE @NewBuildingNameJson NVARCHAR(MAX)
		set @NewBuildingNameJson='[{"variableId":@BuildingNameVariableId,"value":"'+(@BldName )+'"}]'
		IF(@BldJson = @NewBuildingNameJson)
		BEGIN
			DECLARE @buildingJsonString NVARCHAR(MAX)
			DECLARE @OldBuildingName NVARCHAR(MAX)
			DECLARE @OldBuildingNameJson NVARCHAR(MAX)
			set @buildingJsonString =(select '['+ SUBSTRING( 
			( 
		     SELECT ',{"variableId":"'+k.BuindingType+'","value":"'+k.BuindingValue+'"}'
			from building b
			Left Join BuildingConfiguration k
			on b.Id = k.BuildingId
			where b.id = @id
			and b.isDeleted=0
			FOR XML PATH('') 
			), 2 , 9999) + ']')

			set @OldBuildingName=(select BldName from Building where id=@Id)
			set @OldBuildingNameJson='{"variableId":@BuildingNameVariableId,"value":"'+@OldBuildingName+'"},'
			set @NewBuildingNameJson='{"variableId":@BuildingNameVariableId,"value":"'+(@BldName )+'"},'
			set @buildingJsonString= replace(@buildingJsonString,@OldBuildingNameJson,@NewBuildingNameJson)
			if(@OldBuildingName<>@BldName)
			begin
				insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
				select	@Id,
						'Building_Configuration.Parameters.BLDGNAME',
						@BldName,
						@OldBuildingName,
						@ModifiedBy,
						getdate(),
						@ModifiedBy,
						getdate()

			end
			update Building set BldName=@BldName/*,BldJson=@buildingJsonString*/ where id=@Id

			update BuildingConfiguration set BuindingValue = @BldName where BuildingId=@Id and BuindingType = @BuildingNameVariableId

			SET @Result=1
		END
		ELSE
		BEGIN
			UPDATE [dbo].[Building]
				SET BldName=@BldName
				,QuoteId =@QuoteId
				--,BldJson =@BldJson
				,ModifiedBy=@ModifiedBy
				,ModifiedOn=GETDATE()
				--,HasConflictsFlag=@HasConflictsFlag
			OUTPUT inserted.Id
			WHERE Id=@Id

			DECLARE @TempTable table (id int, VariableId nvarchar(255),[Value] nvarchar(255))
			 
			 INSERT INTO @TempTable
			 SELECT @Id id, 
			 JSON_VALUE(value,'$.variableId') as VariableId , 
			 JSON_VALUE(value,'$.value') as [Value]
			 FROM OPENJSON(@BldJson);

		insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @Id,VariableId,tmp.Value,bldConfig.BuindingValue,@ModifiedBy,getdate(),@ModifiedBy,getdate()
		from BuildingConfiguration bldConfig
		join @TempTable tmp
		on	tmp.id = bldConfig.BuildingId
		and	tmp.VariableId = bldConfig.BuindingType
		and tmp.Value<> bldConfig.BuindingValue
		where bldConfig.BuildingId = @Id and bldConfig.BuindingType<>@TotalBuildingFloorToFloorHeight

		insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @Id,VariableId,tmp.Value,bldConfig.BuindingValue,@ModifiedBy,getdate(),@ModifiedBy,getdate()
		from BuildingConfiguration bldConfig
		join @TempTable tmp
		on	tmp.id = bldConfig.BuildingId
		and	tmp.VariableId = bldConfig.BuindingType
		and cast(tmp.Value as numeric(20,10))<> cast(bldConfig.BuindingValue as numeric(20,10))
		where bldConfig.BuildingId = @Id and bldConfig.BuindingType=@TotalBuildingFloorToFloorHeight

		update bldConfig
		set bldConfig.BuindingValue = tmp.[Value],
			IsDeleted = 0,
			ModifiedBy = ModifiedBy,
			ModifiedOn = getdate()
		from BuildingConfiguration bldConfig
		join @TempTable tmp
		on	tmp.id = bldConfig.BuildingId
		and	tmp.VariableId = bldConfig.BuindingType
		where bldConfig.BuildingId = @Id

		If exists (Select '*' From BuildingConfiguration bldConfig
				where not exists (select '*' from @TempTable tmp
								where	tmp.id = bldConfig.BuildingId
								and	tmp.VariableId = bldConfig.BuindingType
								)
				and bldConfig.BuildingId = @Id
				)
		Begin
			--insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			--select @Id,BuindingType,'',BuindingValue,@ModifiedBy,getdate(),@ModifiedBy,getdate()
			--from BuildingConfiguration
			--where BuildingId=@Id and BuindingType not in (select VariableId from @TempTable)


			update bldConfig
			set IsDeleted = 1
			from BuildingConfiguration bldConfig
			where not exists (select '*' from @TempTable tmp
									where tmp.id = bldConfig.BuildingId
									and	tmp.VariableId = bldConfig.BuindingType
									)
			and bldConfig.BuildingId = @Id
		End

		If exists (Select '*' from @TempTable 
					where VariableId not in(select BuindingType 
											from BuildingConfiguration 
											where BuildingId = @Id
											)
					)
		Begin
			insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select  @Id,VariableId,[Value],'',@ModifiedBy,getdate(),@ModifiedBy,getdate()
			from @TempTable where VariableId not in(select BuindingType 
													from BuildingConfiguration
													where BuildingId = @Id
													)

			insert into BuildingConfiguration(BuildingId,BuindingType,BuindingValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select  @Id,VariableId,[Value],@ModifiedBy,getdate(),@ModifiedBy,getdate()
			from @TempTable where VariableId not in(select BuindingType 
													from BuildingConfiguration
													where BuildingId = @Id
													)
		End
		
		SET @Result=1
		END


		IF(@IsEditFlow='InValid' or @IsEditFlow='NeedValidation')
		BEGIN
			IF(@IsEditFlow='InValid')
			begin
				update Building set Workflowstatus ='BLDG_CINV' where id=@Id
			end
			UPDATE GroupConfiguration SET ConflictStatus='GRP_CNV' WHERE BuildingId=@Id --and ConflictCheck=0
			UPDATE Units SET ConflictStatus='UNIT_CNV' WHERE GroupConfigurationId IN (SELECT GroupId FROM GroupConfiguration WHERE BuildingId=@Id) --and ConflictCheck=0
		END
		else
		begin
			declare @perviousStatus nvarchar(100)
			set @perviousStatus = (select distinct WorkflowStatus from Building where Id = @Id)
			-- setting the unit flag by compare the pervious work flow status
			if(@perviousStatus = 'BLDG_VAL')
			begin
				update Building set workflowstatus ='BLDG_COM' where Id = @Id
				update GroupConfiguration set WorkflowStatus = 'GRP_COM'  where BuildingId = @Id
				update Units set WorkflowStatus = 'UNIT_COM' where GroupConfigurationId IN (select distinct GroupConfigurationId from GroupConfiguration where BuildingId = @Id)
				update Systems set StatusKey = 'UNIT_COM' where SetId IN
				(select distinct SetId from Units where GroupConfigurationId IN
				(select distinct GroupConfigurationId from GroupConfiguration where BuildingId = @Id))
				end
			else
			begin
				declare @elevation int
				set @elevation=(select count(*) from BuildingElevation where BuildingId=@Id)
				if(@elevation>0)
				begin
					update Building set workflowstatus='BLDG_COM' where id=@Id
					
				end	
				else
				begin
					update Building set workflowstatus='BLDG_INC' where id=@Id
				end
				update GroupConfiguration set WorkflowStatus = 
				case when WorkflowStatus = 'GRP_VAL' then 'GRP_COM' else WorkflowStatus end
				where BuildingId = @Id
				update Units set WorkflowStatus = 
				case when WorkflowStatus= 'UNIT_VAL' then 'UNIT_COM' else WorkflowStatus end
				where GroupConfigurationId IN (select distinct GroupConfigurationId from GroupConfiguration where BuildingId = @Id)
				update Systems set StatusKey = 
				case when StatusKey='UNIT_VAL' then 'UNIT_COM' else StatusKey end
				where SetId IN
				(select distinct SetId from Units where GroupConfigurationId IN
				(select distinct GroupConfigurationId from GroupConfiguration where BuildingId = @Id))
			end
			
		end
		exec [dbo].[usp_UpdateWorkflowStatus]@quoteId,'quote'

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
end



