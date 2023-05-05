CREATE Procedure [dbo].[usp_GetTravelValueForUnitConfiguration]

@setId int

as

begin
	BEGIN TRY
	 select distinct Travelfeet,TravelInch from OpeningLocation where UnitId in (select unitId from Units where SetId=@setId)

	 select distinct GroupConfigurationId from units where setid = @setid

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
