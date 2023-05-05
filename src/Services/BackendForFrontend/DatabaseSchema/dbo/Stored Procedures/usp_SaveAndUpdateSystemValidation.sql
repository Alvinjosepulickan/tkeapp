CREATE  Procedure [dbo].[usp_SaveAndUpdateSystemValidation]-- 187

@setId int,
@systemsMetaData as SystemsMetaData readonly,
@systemsVariables as SystemVariables readonly,
@statusName nvarchar(200),
@createdBy nvarchar(250)
as

begin
BEGIN TRY
declare @ConflitCount int
set @ConflitCount = (select count(*) from @systemsMetaData)
--select @ConflitCount
declare @ConflictId int
set @ConflictId = 1

	if exists (select * from Systems where SetId = @setId)
	begin
			delete from Systems where SetId = @setId
	end

	if exists (select * from SystemsVariables where SetId = @setId)
	begin
			delete from SystemsVariables where SetId = @setId
	end

while (@ConflictId <= @ConflitCount)
begin

		declare @unitId int
		set @unitId = (select UnitId from @systemsMetaData where Id=@ConflictId)
		--declare @UnitSystemsMetaData  SystemsMetaData readonly
		--set @UnitSystemsMetaData = (select * from @systemsMetaData where Id = @ConflictId) 
	 --if (exists (select SystemValidValues from @systemsMetaData where Id =@ConflictId))
	 if not exists( select SystemValidValues from @systemsMetaData where Id = @ConflictId and SystemValidKeys  IS NULL or SystemValidKeys ='')
		 begin
			if(exists(select StatusKey from Systems where SetId = @setId))
			  begin
			  -- condition started 
				if(exists  (select * from Systems where SetId = @setId and UnitId IS NULL))
				begin					
					delete from Systems where SetId = @setId
					insert into Systems (SetId,SystemValidKeys,SystemValidValues,CreatedBy,ModifiedBy, UnitId, StatusKey)
					select @setId, SystemValidKeys,SystemValidValues , @createdby,@createdby, @unitId, StatusKey  from @systemsMetaData
					where Id = @ConflictId

					delete from SystemsVariables where SetId = @setId
					insert into SystemsVariables (SetId,SystemVariableKeys,SystemVariableValues,CreatedBy,ModifiedBy)
					select @setId, SystemVariableKeys,SystemVariableValues , @createdby,@createdby from @systemsVariables
					where SetId = @setId
				end
				else
					begin
					declare @systemsKeyData varchar(500)
					set @systemsKeyData = (select SystemValidKeys from @systemsMetaData where Id = @ConflictId )
						if exists(select * from Systems where SystemValidKeys = @systemsKeyData and UnitId = @unitId)
							begin						
								update Systems
								set SetId = @setId,
									SystemValidKeys = sysVal.SystemValidKeys,
									SystemValidValues = sysVal.SystemValidValues,
									ModifiedBy = @createdBy,
									ModifiedOn = GETDATE(),
									UnitId = @unitId,
									StatusKey = sysVal.StatusKey
									from @systemsMetaData sysVal
									where sysVal.Id = @ConflictId
									select 'update by unit'
							end
						else
							begin
									insert into Systems (SetId,SystemValidKeys,SystemValidValues,CreatedBy,ModifiedBy, UnitId, StatusKey)
									select @setId, SystemValidKeys,SystemValidValues , @createdby,@createdby, @unitId, StatusKey  from @systemsMetaData
									where Id = @ConflictId
									--select 0000

									insert into SystemsVariables (SetId,SystemVariableKeys,SystemVariableValues,CreatedBy,ModifiedBy)
									select @setId, SystemVariableKeys,SystemVariableValues , @createdby,@createdby from @systemsVariables
									where SetId = @setId

							end
					end
				--update Systems set SystemValidValues = (select SystemDescriptionValues from SystemsMasterValues where )
				--Update Systems
				--set StatusKey = @statusName where UnitId=@unitId
				--select * from Status
				--select StatusName from Status where StatusKey = (select StatusKey from Systems where SetId = @setId)
				update Units
				set WorkflowStatus = 'UNIT_INV' where SetId = @setId
				--select StatusName from Status where StatusKey = (select StatusKey from Systems where SetId = @setId)
			
				--select  from Systems where SetId = @setId
				--select 1010
			  -- if condition ended 
			  end
		  else
			  begin
			  --declare @statusKey nvarchar(20)
					--	set @statusKey = 'Unit_ValInp'
					--select 2222
				insert into Systems (SetId,SystemValidKeys,SystemValidValues,CreatedBy,ModifiedBy,UnitId,StatusKey)
				select @setId,SystemValidKeys,SystemValidValues, @createdby,@createdby ,@unitId, StatusKey from @systemsMetaData
				where Id = @ConflictId

				insert into SystemsVariables (SetId,SystemVariableKeys,SystemVariableValues,CreatedBy,ModifiedBy)
				select @setId, SystemVariableKeys,SystemVariableValues , @createdby,@createdby from @systemsVariables
				where SetId = @setId
				--Update Systems
				--set StatusKey = @statusName where UnitId=@unitId
				update Units
				set WorkflowStatus = 'UNIT_INV' where SetId = @setId
				--select StatusName from Status where StatusKey = (select StatusKey from Systems where SetId = @setId)
				--select 9999
			  end
		 end
	else
		  begin
			if(exists(select StatusKey from Systems where SetId = @setId and UnitId = @unitId))
				begin
					delete from Systems where SetId = @setId and UnitId = @unitId

					--select 3333
				end

			insert into Systems (SetId,SystemValidKeys,SystemValidValues,CreatedBy,ModifiedBy,UnitId,StatusKey)
				values(@setId,'','', @createdby,@createdby,@unitId,'UNIT_VAL')
				--Update Systems
				--set StatusKey = @statusName where SetId=@setId
				update Units
				set WorkflowStatus = 'UNIT_VAL' where SetId = @setId
				--select StatusName from Status where StatusKey = 'Unit_Val'
				--select 4444

				delete from SystemsVariables where SetId = @setId
				insert into SystemsVariables (SetId,SystemVariableKeys,SystemVariableValues,CreatedBy,ModifiedBy)
				select @setId, SystemVariableKeys,SystemVariableValues , @createdby,@createdby from @systemsVariables
				where SetId = @setId
		
		  end
		 -- select @ConflictId
	set @ConflictId = @ConflictId +1
	--select @ConflictId
end

	--select StatusName from Status where StatusKey = (select StatusKey from Systems where SetId = @setId)
	select Systems.UnitId, Status.StatusName,Status.DisplayName,Status.Description,Status.StatusKey from Systems
	inner join Status on Status.StatusKey =  Systems.StatusKey
	where Systems.SetId = @setId

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

end