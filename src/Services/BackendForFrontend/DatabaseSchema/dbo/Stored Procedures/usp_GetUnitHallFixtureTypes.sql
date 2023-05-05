
CREATE PROCEDURE [dbo].[usp_GetUnitHallFixtureTypes] 
	-- Add the parameters for the stored procedure here
	@FixtureStrategy as nvarchar(10)
AS
BEGIN
	BEGIN TRY
		if @FixtureStrategy='ETA'
		begin
			select UnitHallFixtureType from UnitHallFixtureTypes where ETA=1
		end
		if @FixtureStrategy='ETD'
		begin
			select UnitHallFixtureType from UnitHallFixtureTypes where ETD=1
		end
		if @FixtureStrategy='ETA/ETD'
		begin
			select UnitHallFixtureType from UnitHallFixtureTypes where ETA_ETD=1
		end
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

