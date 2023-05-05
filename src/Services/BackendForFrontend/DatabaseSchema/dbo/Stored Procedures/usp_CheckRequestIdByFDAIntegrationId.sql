 
CREATE Procedure [dbo].[usp_CheckRequestIdByFDAIntegrationId]
@fieldDrawingIntegrationMasterId int
As
Begin
	BEGIN TRY
    Select Id as IntegratedProcessId, IntegratedSystemRef from FieldDrawingIntegrationMaster
	  Where Id=@fieldDrawingIntegrationMasterId and StatusKey='DWG_PEN'
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

 