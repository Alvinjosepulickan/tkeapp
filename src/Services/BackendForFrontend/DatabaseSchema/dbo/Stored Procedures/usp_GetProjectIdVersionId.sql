CREATE PROCEDURE [dbo].[usp_GetProjectIdVersionId] 

@quoteId nvarchar(20)

AS


Begin
	BEGIN TRY
		Select OpportunityId,VersionId from Quotes where QuoteId=@quoteId 
		if(@@ROWCOUNT=0)
		begin
			RAISERROR('Invalid QuoteId',16,1)
		END
	END TRY
	BEGIN CATCH 
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo=@quoteId;
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
end   