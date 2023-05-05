CREATE Procedure [dbo].[usp_GetOpeningLocationBygroupId] --1148
@groupConfigurationId int,
@VariableMapperDataTable AS VariableMapper READONLY

as

begin
BEGIN TRY
	DECLARE @Count int   --get the number of rows in openinglocation table 0 if no openings selected
	SET @Count = (Select count(*) from OpeningLocation where GroupConfigurationId = @groupConfigurationId)
	DECLARE @BuildingId int --get the building Id used to take the building details requred for openinglocation
	SET @BuildingId=(SELECT BuildingId FROM GroupConfiguration WHERE GroupId=@groupConfigurationId)
	declare @createdBy nvarchar(250) 
	set @createdBy=(select distinct CreatedBy from GroupConfiguration where GroupId=@groupConfigurationId)
	declare @createdOn dateTime
	set @CreatedOn=(select ( CreatedOn) from GroupConfiguration where GroupId=@groupConfigurationId)

	declare @BldgId int
	declare @CalcBuildingRise numeric(20,10)
	declare @CalculateTravelInch numeric(20,10)
	declare @CalcTravelFeet int
	DECLARE @BuildingLandings NVARCHAR(250)
	DECLARE @TotalBuildingFloorToFloorHeight NVARCHAR(250)
	SET @TotalBuildingFloorToFloorHeight = (select VariableType from @VariableMapperDataTable where variableKey = 'TOTALBUILDINGFLOORTOFLOORHEIGHT')
	SET @BuildingLandings = (select VariableType from @VariableMapperDataTable where variableKey = 'BLANDINGS')

	
	IF(@Count>1 )--and @Count!=@NumberOfRows )-- openinglocation already configured
	BEGIN
	--logic to add any new unit and floor
	
	
	insert into [dbo].[TempOpeningLocation]([GroupConfigurationId],[UnitId],[Travelfeet],[NoOfFloors],[TravelInch],[OcuppiedSpaceBelow],[FrontOpening],[RearOpening],[SideOpening],[FloorDesignation],[FloorNumber],[Front],[Side],[Rear],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn])
	select [GroupConfigurationId],[UnitId],[Travelfeet],[NoOfFloors],[TravelInch],[OcuppiedSpaceBelow],[FrontOpening],[RearOpening],[SideOpening],[FloorDesignation],[FloorNumber],[Front],[Side],[Rear],[CreatedBy],[CreatedOn],[ModifiedBy], [ModifiedOn]from OpeningLocation where GroupConfigurationId=@groupConfigurationId
	DELETE FROM OpeningLocation where GroupConfigurationId=@groupConfigurationId

	Set @CalcBuildingRise = (Select distinct BuildingRise from BuildingElevation where BuildingId = @BldgId)
	set @CalcTravelFeet = (@CalcBuildingRise/12)-((@CalcBuildingRise/12)%1)
	set @CalculateTravelInch = (@CalcBuildingRise%12)

	DECLARE @MinUnitId INT
	DECLARE @MaxUnitId INT
	DECLARE @FirstOpeningLocationId INT
	DECLARE @LastOpeningLocationId INT
	
	SET @MinUnitId=(select min(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
	set @MaxUnitId=(select max(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)

	declare @NoOfFloors int
	declare @travelfeet int
	declare @travelInch numeric(20,10)
	declare @occupiedSpaceBelow bit
	declare @frontOpening int
	declare @rearOpening int
	declare @sideOpening int
	declare @rearCheck int

	while @MinUnitId <=@MaxUnitId
	BEGIN
		
		set @NoOfFloors=(select distinct(noOfFloor) from BuildingElevation where BuildingId=@BuildingId)
		set @travelfeet=(select @CalcTravelFeet)
		set @travelInch=(select @CalculateTravelInch)
		set @occupiedSpaceBelow=0
		set @frontOpening=(select distinct(noOfFloor) from BuildingElevation where BuildingId=@BuildingId)
		set @rearOpening=case when exists(select * from doors where DoorType like '%rear%'  and DoorValue NOT IN ('','NR') and UnitId=@MinUnitId) then (select distinct(noOfFloor) from BuildingElevation where BuildingId=@BuildingId)  else 0 end
		set @sideOpening=case when exists(select * from doors where DoorType like '%side%' and UnitId=@MinUnitId) then (select distinct(noOfFloor) from BuildingElevation where BuildingId=@BuildingId)  else 0 end
		set @FirstOpeningLocationId=(SELECT MAX(OpeningLocationId) FROM OpeningLocation)
		INSERT INTO OpeningLocation ([FloorDesignation],[GroupConfigurationId],[FloorNumber]) 
		SELECT [FloorDesignation],@groupConfigurationId,[FloorNumber] FROM BuildingElevation where BuildingId=@BuildingId order by BuildingElevationId
		if(exists (select * from  tempopeninglocation where unitId=@MinUnitId))
		begin
			set @NoOfFloors= (select distinct(NoOfFloors) from TempOpeningLocation where UnitId=@MinUnitId)
			set @travelfeet=(select distinct(travelfeet) from TempOpeningLocation where UnitId=@MinUnitId)
			set @travelInch=(select distinct(travelInch) from TempOpeningLocation where UnitId=@MinUnitId)
			set @occupiedSpaceBelow=(select distinct([OcuppiedSpaceBelow]) from TempOpeningLocation where UnitId=@MinUnitId)
			set @frontOpening=(select distinct(frontOpening) from TempOpeningLocation where UnitId=@MinUnitId)
			set @rearOpening= case when exists(select * from doors where DoorType like '%rear%' and DoorValue NOT IN ('','NR') and UnitId=@MinUnitId)
							  then case when (select distinct(rearOpening) from TempOpeningLocation where UnitId=@MinUnitId)>0
							  then  (select distinct(rearOpening) from TempOpeningLocation where UnitId=@MinUnitId) 
							  else  (select distinct(noOfFloor) from BuildingElevation where BuildingId=@BuildingId) end
							  else 0 end
			set @sideOpening=(select distinct(sideOpening) from TempOpeningLocation where UnitId=@MinUnitId)
		end
		SET @LastOpeningLocationId=(SELECT MAX(OpeningLocationId) FROM OpeningLocation where [GroupConfigurationId]=@groupConfigurationId)
		UPDATE OpeningLocation SET unitId=@MinUnitId,
									NoOfFloors=@NoOfFloors,
									Travelfeet=@travelfeet,
									TravelInch=@travelInch,
									[OcuppiedSpaceBelow]=@occupiedSpaceBelow,
									FrontOpening=@frontOpening,
									RearOpening=@rearOpening,
									SideOpening=@sideOpening  
									where OpeningLocationId>@FirstOpeningLocationId and OpeningLocationId<=@LastOpeningLocationId and  [GroupConfigurationId]=@groupConfigurationId
		
		UPDATE OpeningLocation SET [Front]=T.[Front], [Side]=t.[Side],[Rear]= T.[Rear]
		FROM TempOpeningLocation AS T where t.[FloorDesignation]=OpeningLocation.[FloorDesignation] and
		t.unitId=OpeningLocation.unitId and OpeningLocation.[GroupConfigurationId]=@groupConfigurationId
		and OpeningLocation.UnitId=@MinUnitId

		update openinglocation set Front=1, rear = case when RearOpening>0 then 1 else 0 end, side = case when SideOpening>0 then 1 else 0 end
		where OpeningLocation.[GroupConfigurationId]=@groupConfigurationId and OpeningLocation.UnitId=@MinUnitId and @MinUnitId not in 
		( select distinct UnitId from TempOpeningLocation where GroupConfigurationId=@groupConfigurationId)

		---For selecting rear openings by default if a new rear door is added---
		if((select sum(cast(rear as int)) from TempOpeningLocation where UnitId=@MinUnitId) =0 and @rearOpening=@NoOfFloors)
		begin
		update OpeningLocation set rear = 1 where OpeningLocation.[GroupConfigurationId]=@groupConfigurationId and
		OpeningLocation.UnitId=@MinUnitId
		end

		SET @MinUnitId=@MinUnitId+1

	END
	update OpeningLocation SET CreatedBy=@createdBy, CreatedOn=@CreatedOn,ModifiedBy=@createdBy,ModifiedOn=getdate() where GroupConfigurationId=@groupConfigurationId
	DELETE FROM [TempOpeningLocation] where GroupConfigurationId=@groupConfigurationId
END
SET @Count = (Select count(*) from OpeningLocation where GroupConfigurationId = @groupConfigurationId)
if(@count=1)
begin

	Set @BldgId = (Select BuildingId from GroupConfiguration where GroupId= @groupConfigurationId)
	Set @CalcBuildingRise = (Select distinct BuildingRise from BuildingElevation where BuildingId = @BldgId)
	set @CalcTravelFeet = (@CalcBuildingRise/12)-((@CalcBuildingRise/12)%1)
	set @CalculateTravelInch = (@CalcBuildingRise%12)

	select
	gc.GroupId as GroupConfigurationId
	,gc.CreatedBy as userName
	,[dbo].[FnGetBuildingTableValueFromBldJson]( bg.id,@TotalBuildingFloorToFloorHeight) as buildingRise
	,isnull([dbo].[FnGetBuildingTableValueFromBldJson]( bg.id,@BuildingLandings),0) as [Floor]
	,ut.unitId
	,ut.[Designation] as [Name]
	,@CalcTravelFeet as TravelFeet
	,@CalculateTravelInch as TravelInch
	,0 as OccupiedSpaceBelow
	,be.noOfFloor as NumberOfFloors
	,be.MainEgress as MainEgress
	,be.alternateEgress as AlternateEgress
	,be.noOfFloor as FrontOpening
	,case when ( exists(select * from doors where UnitId=ut.UnitId and DoorType  like '%rear%' and DoorValue NOT IN ('','NR'))) then be.noOfFloor else 0 end as RearOpening
	,case when dr.DoorType like '%side%' then be.noOfFloor else 0 end as SideOpening
	,ISNULL(dr.DoorType,'') as DoorType
	,ISNULL(dr.DoorValue,'') as DoorValue
	,ISNULL(be.FloorDesignation,0) as FloorNumber
	,be.FloorDesignation
	,be.ElevationFeet
	,be.ElevationInch
	,1 as Front
	,case when ( exists(select * from doors where UnitId=ut.UnitId and DoorType  like '%rear%' and DoorValue NOT IN ('','NR'))) then 1 else 0 end as Rear
	,case when dr.DoorType like '%side%' then 1 else 0 end as Side
	from OpeningLocation ol
	left join GroupConfiguration gc on ol.GroupConfigurationId = gc.GroupId
	left join Units ut on ut.GroupConfigurationId = gc.GroupId
	left join Building bg on bg.Id = gc.BuildingId
	left join BuildingElevation be on be.BuildingId = gc.BuildingId
	left join Doors dr on ut.UnitId=dr.UnitId
	where gc.GroupId = @groupConfigurationId and ol.IsDeleted = 0
	ORDER BY Len(ol.OpeningLocationId),
	ol.OpeningLocationId ,be.BuildingElevationId


	
	
	-- insert defaults 

	-- save the default values
	
	
	declare @MinUnitIdValue int
	declare @MaxUnitIdValue int
	SET @MinUnitIdValue =(select min(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
	set @MaxUnitIdValue =(select max(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
	   	
	declare @noOfFloor int
	set @noOfFloor = (select distinct noOfFloor from BuildingElevation where BuildingId in 
	(select BuildingId from GroupConfiguration where GroupId = @groupConfigurationId))
	
	declare @MinFloorDesignationValue int

	declare @MaxFloorDesignationValue int
	set @MaxFloorDesignationValue = (select count(FloorDesignation) from BuildingElevation where BuildingId in 
	(select BuildingId from GroupConfiguration where GroupId = @groupConfigurationId) and IsDeleted = 0)

	DELETE FROM OpeningLocation where GroupConfigurationId=@GroupConfigurationId
	
	while (@MinUnitIdValue <= @MaxUnitIdValue)
	BEGIN
	set @MinFloorDesignationValue =(select min(floorNumber) from BuildingElevation where BuildingId in 
	(select BuildingId from GroupConfiguration where GroupId = @groupConfigurationId) and IsDeleted = 0)
	
	while (@MinFloorDesignationValue <= @MaxFloorDesignationValue)
		
		BEGIN
		declare @CurrentFloorDesignation nvarchar(50)
		set @CurrentFloorDesignation = 
			(select FloorDesignation from BuildingElevation where BuildingId in 
	(select BuildingId from GroupConfiguration where GroupId = @groupConfigurationId)
	and IsDeleted = 0 and floorNumber = @MinFloorDesignationValue)

		insert into OpeningLocation ([GroupConfigurationId],[UnitId],[Travelfeet],[NoOfFloors],
		[TravelInch],[OcuppiedSpaceBelow],[FrontOpening],[RearOpening],[SideOpening],
		[FloorDesignation],[FloorNumber],[Front],[Side],[Rear],[CreatedBy])	
		values (
		@GroupConfigurationId,
		@MinUnitIdValue,
		@CalcTravelFeet,
		@noOfFloor,
		@CalculateTravelInch,
		0,
		@noOfFloor,
		case when ( exists(select * from doors where UnitId=@MinUnitIdValue and DoorType  like '%rear%')) then @noOfFloor else 0 end,
		case when ( exists(select * from doors where UnitId=@MinUnitIdValue and DoorType  like '%side%')) then @noOfFloor else 0 end,
		ISNULL(@CurrentFloorDesignation,0),
		ISNULL(@MinFloorDesignationValue,0),
		1,
		case when ( exists(select * from doors where UnitId=@MinUnitIdValue and DoorType  like '%side%')) then 1 else 0 end,
		case when ( exists(select * from doors where UnitId=@MinUnitIdValue and DoorType  like '%rear%')) then 1 else 0 end,
		@createdBy)	
		
		set @MinFloorDesignationValue = @MinFloorDesignationValue + 1

		END	

		set @MinUnitIdValue = @MinUnitIdValue + 1
	END

	Update GroupConfiguration set WorkflowStatus='GRP_COM'
	Where groupid=@groupConfigurationId  and WorkflowStatus not in('GRP_CINV','GRP_LOC')
End



else
begin
	Set @BldgId = (Select BuildingId from GroupConfiguration where GroupId= @groupConfigurationId)
	Set @CalcBuildingRise = (Select distinct BuildingRise from BuildingElevation where BuildingId = @BldgId)
	set @CalcTravelFeet = (@CalcBuildingRise/12)-((@CalcBuildingRise/12)%1)
	set @CalculateTravelInch = (@CalcBuildingRise%12)

	select
	distinct
	ol.GroupConfigurationId as GroupConfigurationId
	,gc.CreatedBy as userName
	,ol.OpeningLocationId as OpeningLocationId
	,[dbo].[FnGetBuildingTableValueFromBldJson]( bg.id,@TotalBuildingFloorToFloorHeight) as buildingRise
	,isnull([dbo].[FnGetBuildingTableValueFromBldJson]( bg.id,@BuildingLandings),0) as [Floor]
	,ol.unitId
	,ut.[Designation] as [Name]
	,ISNULL(ol.Travelfeet,@CalcTravelFeet) as TravelFeet
	,ISNULL(ol.TravelInch,@CalculateTravelInch) as TravelInch
	,ISNULL(ol.OcuppiedSpaceBelow,0) as OccupiedSpaceBelow
	,ISNULL(ol.NoOfFloors,0) as NumberOfFloors
	,ISNULL(be.MainEgress,0) as MainEgress
	,ISNULL(be.alternateEgress,0) as AlternateEgress
	,ISNULL(ol.FrontOpening,0) as FrontOpening
	,ISNULL(ol.RearOpening,0) as RearOpening
	,ISNULL(ol.SideOpening,0) as SideOpening
	,ISNULL(dr.DoorType,'') as DoorType
	,ISNULL(dr.DoorValue,'') as DoorValue
	,ISNULL(ol.FloorNumber,0) as FloorNumber
	,ISNULL(ol.FloorDesignation,0) as FloorDesignation
	,ISNULL(be.ElevationFeet,0) as ElevationFeet
	,ISNULL(be.ElevationInch,0) as ElevationInch
	,ISNULL(ol.Front,0) as Front
	,ISNULL(ol.Rear,0) as Rear
	,ISNULL(ol.Side,0) as Side
	from OpeningLocation ol
	LEFT join GroupConfiguration gc on ol.GroupConfigurationId = gc.GroupId
	left join Units ut on ut.GroupConfigurationId = gc.GroupId and ut.UnitId=ol.UnitId
	left join Building bg on bg.Id = gc.BuildingId
	left join BuildingElevation be on be.BuildingId = gc.BuildingId AND be.FloorDesignation=ol.FloorDesignation
	left join Doors dr on ol.UnitId=dr.UnitId
	where ol.GroupConfigurationId = @groupConfigurationId and ol.IsDeleted = 0
	ORDER BY --Len(ol.OpeningLocationId),
	ol.GroupConfigurationId,ol.UnitId asc


end
	select isnull(MappedLocationJson,'') as MappedLocationJson from units where GroupConfigurationId=@groupConfigurationId

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

--select * from ProcedureLog

--select * from OpeningLocation where GroupConfigurationId = 1148 and unitid = 2523
