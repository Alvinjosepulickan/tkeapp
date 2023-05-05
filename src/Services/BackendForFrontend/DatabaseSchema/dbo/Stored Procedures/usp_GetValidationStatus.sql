CREATE  Procedure [dbo].[usp_GetValidationStatus] --187

@setId int,
@systemsMetaData as SystemsMetaData readonly,
@statusName nvarchar(250),
@createdBy nvarchar(250)
as
BEGIN
	BEGIN TRY
		 declare @StatusFlagValue varchar(300)
		 set @StatusFlagValue = (select distinct StatusKey from Systems where SetId = @setId)
		  if(exists(select distinct StatusKey from Systems where SetId = @setId))
		  begin
		  if(@StatusFlagValue = 'UNIT_VAL')
			  begin
				select Systems.SetId,Systems.SystemValidKeys, '' as SystemsDescriptionKeys,'' as SystemsDescriptionValues,
				  Status.StatusKey,Status.StatusName,Status.DisplayName,Status.Description,Systems.CreatedBy, '' as FlagName, Systems.UnitId as UnitId  from Systems
				  inner join Status on Systems.StatusKey = Status.StatusKey
				  where Systems.SetId = @setId
			  end
		  else if(exists(
			  select Systems.SetId,Systems.SystemValidKeys,SystemsMasterValues.SystemsDescriptionKeys,SystemsMasterValues.SystemsDescriptionValues,
			  Status.StatusKey,Status.StatusName,Systems.CreatedBy from Systems
			  inner join SystemsMasterValues on Systems.SystemValidValues = SystemsMasterValues.SystemsDescriptionKeys
			  inner join Status on Systems.StatusKey = Status.StatusKey
			  where Systems.SetId = @setId))
				  begin
					  select Systems.SetId,Systems.SystemValidKeys,SystemsMasterValues.SystemsDescriptionKeys,SystemsMasterValues.SystemsDescriptionValues,
					  Status.StatusKey,Status.StatusName,Status.DisplayName,Status.Description ,Systems.CreatedBy,SystemsMasterValues.FlagName as FlagName, Systems.UnitId as UnitId  from Systems
					  inner join SystemsMasterValues on Systems.SystemValidValues = SystemsMasterValues.SystemsDescriptionKeys
					  inner join Status on Systems.StatusKey = Status.StatusKey
					  where Systems.SetId = @setId

					  --select 1111
				  end
			else
				  begin
				  declare @descriptionMessage nvarchar(300)
				  set @descriptionMessage = (select SystemValidValues from Systems where SetId = 195)
				  --declare @flagName nvarchar(20)
				  --set @flagName = ''
					  select Systems.SetId,Systems.SystemValidKeys, Systems.SystemValidValues as SystemsDescriptionKeys,@descriptionMessage  as SystemsDescriptionValues, 
					  Status.StatusKey,Status.StatusName,Systems.CreatedBy, Status.DisplayName, Status.Description ,''as FlagName from Systems
					  inner join Status on Systems.StatusKey = Status.StatusKey
					  where Systems.SetId =@setId
					  --select * from Systems
					-- select 2222
				  end

			end
			else
			begin
				
				insert into Systems (SetId,SystemValidKeys,SystemValidValues,CreatedBy,ModifiedBy,UnitId,StatusKey)
				select @setId,SystemValidKeys,SystemValidValues, @createdby,@createdby, UnitId , 'UNIT_VAL'
				from @systemsMetaData

				Update Systems
				set StatusKey = (select StatusId from Status where Status.StatusName = 'UNIT_VAL')where SetId=@setId
				select Status.StatusKey,StatusName, DisplayName,Description from Status where StatusKey  = 'UNIT_VAL'
				--select 3333
			end

			--Update Workflowstatus
			declare @groupId int
			set @groupId = (select distinct GroupConfigurationId from Units where SetId = @setId)
			exec [dbo].[usp_UpdateWorkflowStatus]@groupId,'group'
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