CREATE PROCEDURE [dbo].[usp_UpdateLockGroupPropertyForFDA] 
 @islock int,
 @groupId int
,@OpportunityId nvarchar(25)

AS
BEGIN
	BEGIN TRY
	 IF EXISTS (select * from FieldDrawingMaster where GroupId = @groupId and QuoteId=@OpportunityId )
	begin
			update FieldDrawingMaster
			set IsLocked = @islock, LockedDate = getdate()
			where Id in (Select top 1 Id from  [dbo].[FieldDrawingMaster] 
		    Where GroupId = @groupId and QuoteId=@OpportunityId
	        order by createdOn desc)

			if (@islock=1)
			begin
				UPDATE units
				SET WorkflowStatus= 'UNIT_LOC' where GroupConfigurationId=@groupId 

				UPDATE GroupConfiguration
				SET WorkflowStatus= 'GRP_LOC' where GroupId=@groupId 

				UPDATE Building
				SET WorkflowStatus= 'BLDG_LOC'
				from Building b inner join GroupConfiguration g on b.id=g.BuildingId
				where g.GroupId=@groupId
				EXEC [usp_UpdateWorkflowStatus]@OpportunityId,'quote'
			end
			else
			begin
				
				UPDATE GroupConfiguration SET WorkflowStatus='GRP_VAL' WHERE GroupId=@groupId
				UPDATE Units SET WorkflowStatus='UNIT_VAL' WHERE GroupConfigurationId=@groupId
				declare @countLockGroup int
				select @countLockGroup=Count(*) from GroupConfiguration where WorkflowStatus='GRP_LOC' 
										and BuildingId=(select buildingid from GroupConfiguration where GroupId=@groupId) and IsDeleted=0
				declare @buildingid int
				select @buildingid =buildingid from GroupConfiguration where GroupId=@groupId
				if(@countLockGroup>0)
				begin
					
					UPDATE Building
					SET WorkflowStatus= 'BLDG_LOC'
					from Building b inner join GroupConfiguration g on b.id=g.BuildingId
					where g.GroupId=@groupId
					 
				end
				else
				begin
					EXEC [usp_UpdateWorkflowStatus]@buildingid,'building'
				end
			end
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
