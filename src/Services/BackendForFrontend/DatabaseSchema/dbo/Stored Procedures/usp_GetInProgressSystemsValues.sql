CREATE  Procedure [dbo].[usp_GetInProgressSystemsValues] --195,'c2dUser'

@setId int,
@createdBy nvarchar(250)
as
-- called before hang fire call
begin
	BEGIN TRY
	declare @statusKey nvarchar(200)

	set @statusKey = 'UNIT_SVINP'

	  if(exists(select StatusKey from Systems where SetId = @setId))
	  begin
	  -- condition started 
		update Systems
		set StatusKey = @statusKey where SetId = @setId
		-- update in the unit
		update Units
		set WorkflowStatus = @statusKey where SetId = @setId
		-- modfied name
		update Systems
		set ModifiedBy = @createdBy
		where SetId = @setId

		select distinct StatusName  SystemStatus, DisplayName , Description from Status where StatusKey in (select distinct StatusKey from Systems where SetId = @setId)


	  -- if condition ended 
	  end
	  else
	  begin

		insert into Systems (SetId,StatusKey,CreatedBy,ModifiedBy)
		values(@setId,@statusKey,@createdBy,@createdBy)	
		select StatusName as SystemStatus,DisplayName , Description from Status where StatusKey = @statusKey
		
		-- update in the unit
		update Units
		set WorkflowStatus = @statusKey where SetId = @setId

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


--select * from Systems