-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetGroupVariableDetails]
	-- Add the parameters for the stored procedure here
	@VarName as NVARCHAR(50)
AS
BEGIN
	BEGIN TRY
			-- SET NOCOUNT ON added to prevent extra result sets from
			-- interfering with SELECT statements.
			SET NOCOUNT ON;
	
		Select g.IdValues
		from 
		GroupVariables g
		where g.VarName=@VarName
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
