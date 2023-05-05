CREATE PROCEDURE [dbo].[usp_GetGroupHallFixtureTypes]
	-- Add the parameters for the stored procedure here
	@FixtureStrategy as nvarchar(10)
AS
BEGIN
	BEGIN TRY
		if @FixtureStrategy='ETA'
		begin
			select GroupHallFixtureType from GroupHallFixtureTypes where [ETA]=1
		end
		else if(@FixtureStrategy='ETD')
		begin
			select GroupHallFixtureType from GroupHallFixtureTypes where [ETD]=1
		end
		else
		begin
		select GroupHallFixtureType from GroupHallFixtureTypes where [ETA/ETD] =1
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



