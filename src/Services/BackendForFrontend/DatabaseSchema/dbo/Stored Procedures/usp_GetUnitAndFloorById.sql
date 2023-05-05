CREATE Procedure [dbo].[usp_GetUnitAndFloorById]-- 269
-- Add the parameters for the stored procedure here
 @GroupConfigurationId int
as
Begin
	BEGIN TRY
		DECLARE @BuildingId int,@mainEgress nvarchar(10),@alternateEgress nvarchar(10),@noOfFloor int
		set  @BuildingId=(select BuildingId from GroupConfiguration where GroupId=@groupConfigurationId)

		--select FloorNumber,FloorDesignation from BuildingElevation where BuildingId=@BuildingId

		--select Units.UnitId,Designation,Occupiedspacebelow,DoorType from Units left join Doors on Units.unitId=Doors.UnitId where Units.GroupConfigurationId=@groupConfigurationId
	
		select @mainEgress=FloorDesignation   from buildingElevation where BuildingId=@BuildingId and MainEgress=1
		select @alternateEgress=FloorDesignation  from buildingElevation where BuildingId=@BuildingId and alternateEgress=1
		select distinct @noOfFloor=count(floorNumber) from BuildingElevation where BuildingId=@BuildingId
		select @mainEgress MainEgress,@alternateEgress AlternateEgress, @noOfFloor NumberOfFloors
		--exec [dbo].[usp_GetOpeningLocationBygroupId] @GroupConfigurationId

		--Unit openingDetails
		Select distinct u.UnitId,
		u.Designation,
		u.OccupiedSpaceBelow OccupiedSpaceBelow,
		be.FloorDesignation,
		be.FloorNumber,
		isnull(ol.FrontOpening,-1) NoOfFrontOpening,
		isnull(ol.RearOpening,-1)  NoOfRearOpening,
		0 NoOfSideOpening,
		cast(1 as bit) FrontOpening,
		case when ( exists(select * from doors where UnitId=ol.UnitId and DoorType  like '%rear%' and DoorValue<>'')) then 1 else 0 end RearOpening,
		cast (0 as bit) SideOpening,
		cast(isnull(ol.Front,0) as bit) Front,
		cast(isnull(ol.Rear,0) as bit) Rear,
		cast (0 as bit) Side
		From OpeningLocation ol right join 
		BuildingElevation be on ol.FloorDesignation=be.FloorDesignation
		 left join Doors d on ol.unitID=d.unitid
		 join Units u on ol.unitId=u.unitid
		where ol.GroupConfigurationId=@GroupConfigurationId  and buildingid=@BuildingId
		order by ol.UnitId, ol.FloorNumber

		--Hallstation Details
		--select distinct HallStationName,
		--		be.FloorDesignation,
		--		be.FloorNumber,
		--		OccupiedSpaceBelow,
		--		Max(ol.FrontOpening) NoOfFrontOpening,
		--		max(ol.RearOpening) NoOfRearOpening,
		--		case when HallStationName like '%F_SP%' then 1 else 0 end as FrontDoorSelected,
		--		case when HallStationName like '%R_SP%' then 1 else 0 end as RearDoorSelected,
		--		cast(MAX(cast(ol.Front as int)) as bit)Front,
		--		cast(MAX(cast(ol.Rear as int)) as bit)Rear
		--	from UnitHallStationMapping
		--	join ControlLocation
		--	on HallStationName=ControlLocationType
		--	join Units on UnitPosition=Location 
		--	Join OpeningLocation ol on ol.UnitId= Units.UnitId
		--	right join BuildingElevation be on be.floorDesignation=ol.FloorDesignation
		--	where Units.GroupConfigurationId=@GroupConfigurationId and ControlLocation.GroupConfigurationId=@GroupConfigurationId
		--	and ControlLocationValue ='TRUE' and buildingid=@BuildingId
		--	Group By HallStationName,
		--		be.FloorDesignation,
		--		be.FloorNumber,
		--		OccupiedSpaceBelow
		--	order by HallStationName,FloorNumber


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
