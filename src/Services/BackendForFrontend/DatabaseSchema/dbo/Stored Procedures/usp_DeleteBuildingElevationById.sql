CREATE PROCEDURE [dbo].[usp_DeleteBuildingElevationById]
@buildingConfigurationId as int,@modifiedBy varchar(50)
as
BEGIN
	BEGIN TRY
       set nocount on;
       update BuildingElevation set isDeleted=1 ,modifiedBy=@modifiedBy,modifiedOn=getDate() where buildingId=@buildingConfigurationId
	   return 1 
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
