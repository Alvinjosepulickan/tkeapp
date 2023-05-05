-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetFixtureStrategy]
	@GroupId int
	
AS
BEGIN
	BEGIN TRY
	select ControlLocationValue from ControlLocation where GroupConfigurationId = @GroupId 
	and ControlLocationType = 'Parameters_SP.fixtureStrategy_SP'
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
