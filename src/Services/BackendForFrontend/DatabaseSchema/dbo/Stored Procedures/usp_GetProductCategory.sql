
CREATE PROCEDURE [dbo].[usp_GetProductCategory]
	-- Add the parameters for the stored procedure here
	@ProductCategoryId as int
AS
BEGIN
	BEGIN TRY
		SELECT ProductCategory from [dbo].[ProductCategory] where ProductCategoryId=@ProductCategoryId
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
