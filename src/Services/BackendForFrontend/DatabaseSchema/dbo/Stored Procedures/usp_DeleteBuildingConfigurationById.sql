

CREATE PROCEDURE [dbo].[usp_DeleteBuildingConfigurationById]
@buildingConfigurationId int
,@modifiedBy varchar(50)
,@Result int output

AS
BEGIN
	BEGIN TRY
	set nocount on;
	IF EXISTS (SELECT id from Building where id = @buildingConfigurationId and isDeleted=0) 
		BEGIN 
	
			update Building set modifiedOn=getdate(),isDeleted=1,modifiedBy=@modifiedBy where id = @buildingConfigurationId;
			update BuildingConfiguration set modifiedOn=getdate(),isDeleted=1,modifiedBy=@modifiedBy where BuildingId = @buildingConfigurationId and isDeleted=0

			IF EXISTS (SELECT buildingId from BuildingElevation where buildingId = @buildingConfigurationId) 
				BEGIN
					update BuildingElevation set isDeleted=1,modifiedBy=@modifiedBy,modifiedOn=getdate() where buildingId=@buildingConfigurationId;
				END
			IF EXISTS (SELECT buildingId from GroupConfiguration where buildingId = @buildingConfigurationId) 
				BEGIN
					update GroupConfiguration set isDeleted=1,ModifedBy=@modifiedBy,modifiedOn=getdate() where buildingId=@buildingConfigurationId;
					UPDATE Units set IsDeleted=1,ModifiedBy=@modifiedBy,ModifiedOn=getdate() where GroupConfigurationId in(select distinct Groupid from GroupConfiguration where BuildingId=@buildingConfigurationId)
				END
			--update workflow status
			declare @Quoteid nvarchar(20), @opportunityid nvarchar(20)
			select @Quoteid= QuoteId from Building where id=@buildingConfigurationId
			--select @opportunityid= OpportunityId from Quotes where QuoteId=@Quoteid
			--exec [dbo].[usp_UpdateWorkflowStatus]@opportunityid,'project'
			exec [dbo].[usp_UpdateWorkflowStatus]@Quoteid,'quote'
			
			set @Result= 1;
		END
	ELSE
		BEGIN
		set @Result= 0;;
		END
	END TRY
	BEGIN CATCH
	EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo=@buildingConfigurationId;
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH

END


