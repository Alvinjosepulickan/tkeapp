CREATE Procedure [dbo].[usp_GetFDAStatusForRefresh] --125,'In-2021-000002'
 @groupId int
,@quoteId nvarchar(200)
as 
Begin
	BEGIN TRY
	declare @fieldDrawingId int;
	  SET @fieldDrawingId = (Select top 1 Id from FieldDrawingMaster
			 where GroupId=@groupId and QuoteId=@quoteId  order by CreatedOn desc)


		 Select StatusKey as statusId
			   ,IntegratedSystemRef as referenceId
			   ,Id as fieldDrawingIntegrationMasterId from FieldDrawingIntegrationMaster
		   Where FieldDrawingIntegrationId=@fieldDrawingId
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