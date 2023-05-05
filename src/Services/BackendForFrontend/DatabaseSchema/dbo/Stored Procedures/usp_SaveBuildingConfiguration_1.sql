﻿
CREATE PROCEDURE [dbo].[usp_SaveBuildingConfiguration_1]

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
 @VariableMapperDataTable AS VariableMapper ReadOnly,
 @Result int OUTPUT

 AS

BEGIN
	BEGIN TRY
		--declare @QuoteId nvarchar(20)
		--Select @QuoteId = (Select QuoteId from Quotes where OpportunityId = @OpportunityId and VersionId = @VersionId)
		-- get product name 
		declare @isProductEvoValue nvarchar(200)
		declare @isProductEvo200 bit

		if exists(select * from Building where Id = @Id) 
		begin
		if exists((select ProductName  from UnitSet where SetId= (select top 1  SetId from Units where GroupConfigurationId =
			(select top 1 GroupId from GroupConfiguration where BuildingId = (select Id from Building where Id = 1)))))
			begin
		set @isProductEvoValue =(select ProductName  from UnitSet where SetId= (select top 1  SetId from Units where GroupConfigurationId =
			(select top 1 GroupId from GroupConfiguration where BuildingId = (select Id from Building where Id = 1))))
			end

			set @isProductEvo200 = (case when @isProductEvoValue = 'EVO_200' then 1 else 0 end)
			select @isProductEvoValue
			select @isProductEvo200
		end
		
		 IF EXISTS(SELECT * FROM Building WHERE QuoteId=@QuoteId AND BldName=@BldName AND IsDeleted=0)

		 BEGIN

		  SET @Result=-1

		  RETURN 0

		 END
		 ELSE
		 BEGIN
		  INSERT INTO [dbo].[Building]

					  (

								  QuoteId ,
								  OpportunityId,

				BldName ,

				--BldJson ,

				CreatedBy,

				ModifiedBy,
				ModifiedOn,
				WorkflowStatus,
				BuildingEquipmentStatus
					  )

			 VALUES

					  (

								  @QuoteId ,
								  @OpportunityId,
								  @BldName ,

								  --@BldJson ,

								  @CreatedBy,

									@ModifiedBy,
									GETDATE(),
									'BLDG_INC',
									'BLDGEQP_UA'

					  )
			SET @Result=SCOPE_IDENTITY()
		 --SET @Result=(SELECT (Id) FROM Building where QuoteId=@QuoteId and BldName=@BldName)

		 DECLARE @TempTable table (id int, VariableId nvarchar(255),[Value] nvarchar(255))
		 declare @getdate [datetime] = getdate()

		 insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
				SELECT @Result, 
					 JSON_VALUE(value,'$.variableId') as VariableId , 
					 JSON_VALUE(value,'$.value') as [Value] ,
					 '',
					 @CreatedBy,
					 getdate(),
					 @ModifiedBy,
					 @getdate
					 FROM OPENJSON(@BldJson);


		  insert into BuildingConfiguration (BuildingId, BuindingType, BuindingValue,CreatedBy, ModifiedBy, ModifiedOn)
					 SELECT @Result, 
					 JSON_VALUE(value,'$.variableId') as VariableId , 
					 JSON_VALUE(value,'$.value') as [Value] ,
					 @CreatedBy,
					 @ModifiedBy,
					 @getdate
					 FROM OPENJSON(@BldJson);

			declare @oppurtunityid nvarchar(20)
			select @oppurtunityid= q.OpportunityId from Quotes q inner join Building b on q.QuoteId=b.QuoteId where b.id=@Result
			 declare @powerPhaseValues nvarchar
			 set @powerPhaseValues = (case when exists (select BuindingValue from BuildingConfiguration where BuindingType ='Building_Configuration.Parameters.BPP' and BuildingId = @Id) then (select BuindingValue from BuildingConfiguration where BuindingType ='Building_Configuration.Parameters.BPP' and BuildingId = @Id) else '0' end)
			 if(@isProductEvo200 = 1 and @powerPhaseValues = '1' )
			 begin
				set @Result = -2
			 end
			exec [dbo].[usp_UpdateWorkflowStatus]@oppurtunityid,'project'
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
END