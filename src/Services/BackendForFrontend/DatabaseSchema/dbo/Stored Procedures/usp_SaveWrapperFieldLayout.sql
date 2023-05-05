 
CREATE Procedure [dbo].[usp_SaveWrapperFieldLayout]
 @FDAProcessJson  NVarChar(max)
,@groupId int
,@IntegratedProcessId int
,@createdBy nvarchar(500)
,@statusId nvarchar(200)
AS

Begin
	BEGIN TRY
        IF NOT EXISTS(Select * from [dbo].[FieldDrawingAutomationProcessDetails] where IntegratedProcessId= @IntegratedProcessId)
		Begin
			  Insert into [dbo].[FieldDrawingAutomationProcessDetails]
			  (
				FDAProcessJson
				,IntegratedProcessId
				,CreatedBy
			   )
			  Values 
			  (
			     @FDAProcessJson
				,@IntegratedProcessId
				,@CreatedBy
			   )
		  End
		  Else
		  Begin
		    Update [dbo].[FieldDrawingAutomationProcessDetails]
			  Set FDAProcessJson=@FDAProcessJson
			     ,ModifiedBy = @createdBy
				 ,ModifiedOn = getdate()
				   Where IntegratedProcessId = @IntegratedProcessId 
		  End

		   

		  declare @fieldDrawingId int;
		  SET @fieldDrawingId = (Select FieldDrawingIntegrationId from FieldDrawingIntegrationMaster
		     where Id=@IntegratedProcessId)


		update FieldDrawingIntegrationMaster set StatusKey=@statusId where Id=@IntegratedProcessId;
		update FieldDrawingMaster set StatusKey=@statusId, IsLocked=0 where Id=@fieldDrawingId;

		--declare @status nvarchar(20)
		--set @status = (Select StatusKey from FieldDrawingMaster
		--              where Id=@fieldDrawingId)




		  
		--IF(@status = 'DWG_CMP')
		--Begin
			       
		--	UPDATE units
		--	SET WorkflowStatus= 'UNIT_LOC' where GroupConfigurationId=@groupId 

		--	UPDATE GroupConfiguration
		--	SET WorkflowStatus= 'GRP_LOC' where GroupId=@groupId 

		--	UPDATE Building
		--	SET WorkflowStatus= 'BLDG_LOC'
		--	from Building b inner join GroupConfiguration g on b.id=g.BuildingId
		--	where g.GroupId=@groupId
 
		--End

 		  





		--ELSE  IF(@layoutStatus = 'Pending' or @layoutStatus = 'Current' )
		--Begin
		--    update FieldDrawingIntegrationMaster set StatusId=2 where Id=@IntegratedProcessId;
		--    update FieldDrawingMaster set StatusId=2 where GroupId=@groupId;
		--End
		--ELSE  IF(@layoutStatus = 'Error')
		--Begin
		--    update FieldDrawingIntegrationMaster set StatusId=4 where Id=@IntegratedProcessId;
		--    update FieldDrawingMaster set StatusId=4 where GroupId=@groupId;
		--End
	END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
End
			   
		--End
	   
	  
				
				

	

		 
 
  
