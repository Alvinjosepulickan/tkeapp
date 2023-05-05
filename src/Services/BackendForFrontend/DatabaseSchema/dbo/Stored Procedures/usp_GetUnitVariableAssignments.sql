
CREATE Procedure [dbo].[usp_GetUnitVariableAssignments]
@unitList as UnitIDList readonly
as
begin

BEGIN TRY
		--select distinct UnitId ,Travelfeet,TravelInch from OpeningLocation where UnitId in (select UnitID from @unitList) order by OpeningLocation.UnitId

		select distinct OpeningLocation.UnitId, Location,OpeningLocation.OcuppiedSpaceBelow as OccupiedSpaceBelow, UnitJson, Travelfeet,TravelInch, NoOfFloors, FloorNumber, Front,Rear from OpeningLocation inner join Units on 
		OpeningLocation.UnitId = Units.UnitId
		where OpeningLocation.UnitId in (select UnitId from @unitList) order by OpeningLocation.UnitId

		--Getting assignments for Jamp mount default control
		declare @groupId int
		IF OBJECT_ID('tempdb..#tempControlLocation') IS NOT NULL 
		BEGIN 
			DROP TABLE #TempTable 
		END
		declare @variableAssignmentTOPF nvarchar(50),@variableAssignmentTOPR nvarchar(50)
		SET @variableAssignmentTOPF=N'Parameters.TOPF'
		SET @variableAssignmentTOPR=N'Parameters.TOPR'
		declare @valueTOPF int,@valueTOPR int
		select distinct @groupId= GroupConfigurationId from OpeningLocation where UnitId in (select UnitID from @unitList)
		select  @valueTOPF=Max(FloorNumber) from OpeningLocation where GroupConfigurationId=@groupId and Front=1
		select  @valueTOPR=isnull(Max(FloorNumber),1) from OpeningLocation where GroupConfigurationId=@groupId and Rear=1
		create table #tempControlLocation(groupid int, variableAssignment nvarchar(max), value nvarchar(max))
		insert into #tempControlLocation values(@groupId,@variableAssignmentTOPF,@valueTOPF)
		insert into #tempControlLocation values(@groupId,@variableAssignmentTOPR,@valueTOPR)
		insert into #tempControlLocation
		select GroupConfigurationId,ControlLocationType,ControlLocationValue from controllocation where GroupConfigurationId=@groupId
		select * from #tempControlLocation

		select distinct UnitId, DoorJson from Doors where UnitId in (select UnitID from @unitList) order by Doors.UnitId

		select distinct UnitId, Location, UnitJson from Units
		where GroupConfigurationId = (select distinct(GroupConfigurationId) from Units where UnitId in (select UnitId from @unitList))
	
		select * from BuildingConfiguration where BuildingId in (select BuildingId from GroupConfiguration where GroupId in (select distinct GroupConfigurationId from Units where UnitId in (select UnitId from @unitList )))
		select distinct GroupConfigurationId from Units where UnitId in (select UnitId from @unitList ) 
		select BuildingId,MainEgress,alternateEgress,FloorNumber,ElevationFeet,ElevationInch,FloorToFloorHeightFeet,FloorToFloorHeightInch from BuildingElevation where BuildingId in (select BuildingId from GroupConfiguration where GroupId in (select distinct GroupConfigurationId from Units where UnitId in (select UnitId from @unitList )))
		select * from unitset where SetId in (select SetId from units where GroupConfigurationId in (select GroupConfigurationId from units where unitid in (select unitId from @unitList)))
		declare @buildingId int
		set @buildingId=(select distinct BuildingId  from GroupConfiguration where GroupId in (select distinct GroupConfigurationId from units where unitId in (select UnitId from @unitList)))
		select  OpeningLocation.FloorNumber,OpeningLocation.FloorDesignation,Front,Rear,FloorToFloorHeightFeet,FloorToFloorHeightInch from OpeningLocation inner join BuildingElevation on 
		BuildingElevation.BuildingId =@buildingId and BuildingElevation.floorNumber=OpeningLocation.FloorNumber
		where OpeningLocation.UnitId in (select UnitId from @unitList) order by OpeningLocation.UnitId
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
