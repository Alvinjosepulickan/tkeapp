-- =============================================
-- Author:		Harshada
-- Create date: 14/4/2021
-- Description:	Get Product Type using set Id
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetProductType] 
	-- Add the parameters for the stored procedure here
	@setId int
AS
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		-- Insert statements for procedure here
		SELECT ProductName from UnitSet where SetId = @setId
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
