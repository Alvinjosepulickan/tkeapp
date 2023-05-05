CREATE  Procedure [dbo].[usp_GetdetailsForOBOMXMLGeneration]--194--107

 

@setId int

as

begin
	BEGIN TRY
		declare @groupConfigurationId int
		declare @buildingId int
		declare @createdBy nvarchar(250)
		declare @fixtureStrategy NVARCHAR(10)

		set @groupConfigurationId=@setId
		set @buildingId=(select distinct(BuildingId) from GroupConfiguration where GroupId=@groupConfigurationId)
		set @createdBy=(select distinct(CreatedBy) from Building where id=@buildingId)
		set @FixtureStrategy=(select ControlLocationValue from ControlLocation where ControlLocationId in  (select min(ControlLocationId) from ControlLocation where GroupConfigurationId = @groupConfigurationId and ControlLocationType = 'Parameters_SP.fixtureStrategy_SP'));
		if(@fixtureStrategy is null)
		begin
			set @fixtureStrategy='ETA'
		end

	
		--building configuration data
		select * from BuildingConfiguration where BuildingId=@buildingId
		--building elevation data
		select * from BuildingElevation where BuildingId=@buildingId order by floorNumber
		--group configuration data
		select * from GroupConfigurationDetails where GroupId=@groupConfigurationId
		--group layout data
		select * from units where GroupConfigurationId=@groupConfigurationId order by [Location]
		--control location data
		select * from ControlLocation where GroupConfigurationId=@groupConfigurationId
		--hall riser data
		select * from HallRiser where GroupConfigurationId=@groupConfigurationId
		--doors data
		select * from Doors where GroupConfigurationId=@groupConfigurationId
		--opening location data
		select * from OpeningLocation where GroupConfigurationId=@groupConfigurationId order by FloorNumber,UnitId
		--group hall fixture console data
		select * from GroupHallFixtureConfiguration where GroupHallFixtureConsoleId in (select GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId)
		--unit configuration data
		select * from UnitConfiguration where SetId in (select SetId from Units where GroupConfigurationId=@setId)
		--entrance configuration data
		select * from EntranceConfiguration where EntranceConsoleId in (select EntranceConsoleId from EntranceConsole where SetId in (select SetId from Units where GroupConfigurationId=@setId))
		--unit hall fixture console data
		select * from UnitHallFixtureConfiguration where UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId in (select SetId from Units where GroupConfigurationId=@setId))
		
		select * from GroupHallFixtureLocations where GroupHallFixtureConsoleId in (select GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId)
		
		select * from EntranceLocations where EntranceConsoleId in (select EntranceConsoleId from EntranceConsole where SetId in (select SetId from Units where GroupConfigurationId=@setId))
		
		select * from UnitHallFixtureLocations where UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId in (select SetId from Units where GroupConfigurationId=@setId))
		
		select * from EntranceConsole where SetId in (select SetId from Units where GroupConfigurationId=@setId)
		select * from UnitHallFixtureConsole where SetId in (select SetId from Units where GroupConfigurationId=@setId)
		if( exists (select * from [SpecMemoVersion] where GroupId=@setId))
		begin
			declare @specMemovalue int
			set @specMemovalue=(select max(specMemoVersion) from [SpecMemoVersion] where GroupId=@setId)
			update SpecMemoVersion set specMemoVersion=@specMemovalue+1 where GroupId=@setId
		end
		else
		begin
			insert into [SpecMemoVersion] (specMemoVersion,GroupId) values(1,@setId)
		end
		select * from [SpecMemoVersion] where GroupId=@setId
		--exec [dbo].[usp_GetBuildingConfigurationById] @buildingId
		--exec [dbo].[usp_GetBuildingElevationById] @buildingId
		--exec [dbo].[usp_GetGroupLayoutConfiguration] @groupConfigurationId
		--exec [dbo].[usp_GetOpeningLocationBygroupId]  @groupConfigurationId
		--exec [dbo].[usp_GetGroupHallFixture] @fixtureStrategy, @groupConfigurationId ,@createdBy
		--exec [dbo].[usp_GetHallLanternConfiguration] @groupConfigurationId,@setId,@fixtureStrategy,@createdBy
		--exec [dbo].[usp_GetEntranceConfigurationBySetId] @groupConfigurationId,@setId
		--exec [dbo].[usp_GetBuildingEquipmentConfigurationById] @buildingId
		--select * from units where GroupConfigurationId=@groupConfigurationId
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
