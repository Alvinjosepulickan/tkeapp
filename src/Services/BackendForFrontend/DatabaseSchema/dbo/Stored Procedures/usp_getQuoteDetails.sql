CREATE procedure [dbo].[usp_getQuoteDetails]
(
 @quoteId as nvarchar(30)
)
as
begin
	BEGIN TRY
		select * from Quotes where quoteId=@quoteId
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