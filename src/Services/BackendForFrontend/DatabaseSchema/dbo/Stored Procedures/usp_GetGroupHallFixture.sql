
CREATE PROCEDURE [dbo].[usp_GetGroupHallFixture]--'ETA/ETD',450,'c2dUser'
	-- Add the parameters for the stored procedure here
@fixtureStrategy NVARCHAR(10),
@groupConfigurationId int,
@userName Nvarchar(100),
@hallStationVariables unitConfigurationDataTable Readonly
AS
Begin
	
	--declare @FixtureStrategy nvarchar(20)
	declare @setId int
	declare @buildingId int
	set @buildingId=(select BuildingId from GroupConfiguration where GroupId=@groupConfigurationId)
	declare @mainEgress nvarchar(3)
	set @mainEgress=(select FloorDesignation from BuildingElevation where BuildingId=@buildingId and MainEgress=1)
	set @setId=1
   --set @FixtureStrategy=(select ControlLocationValue from ControlLocation where GroupConfigurationId = @groupConfigurationId and ControlLocationType = 'Parameters_SP.fixtureStrategy_SP');
   
   declare @RearSelected bit
   if(Exists(select * from GroupHallFixtureConsole where GroupId=@groupConfigurationId and IsDeleted=0 ))
   begin
	delete from GroupHallFixtureLocations where [UnitId] in (select UnitId from Units where GroupConfigurationId=@groupConfigurationId) and [FloorDesignation] not in (select FloorDesignation from BuildingElevation where BuildingId=(@buildingId))
	/*Looping through consoles for updating IsController property for default consoles*/

   DECLARE @GroupHallFixtureConsoleId int
   DECLARE db_cursor CURSOR FOR SELECT GroupHallFixtureConsoleId FROM GroupHallFixtureConsole Where GroupId=@groupConfigurationId;
   OPEN db_cursor;
   FETCH NEXT FROM db_cursor INTO @GroupHallFixtureConsoleId;
   WHILE @@FETCH_STATUS = 0
   BEGIN
		declare @controller bit
		declare @FixtureType nvarchar(100)
		declare @CountConsole int
		select @controller = IsController from GroupHallFixtureConsole where GroupHallFixtureConsoleId=@GroupHallFixtureConsoleId	
		select @FixtureType = FixtureType from GroupHallFixtureConsole where GroupHallFixtureConsoleId=@GroupHallFixtureConsoleId
		select @CountConsole=count(*) from GroupHallFixtureConsole where FixtureType=@FixtureType and GroupId=@groupConfigurationId
		if(@FixtureStrategy='ETA')
		begin
			if(@CountConsole=1 and @controller=0)
			begin
				update GroupHallFixtureConsole set IsController=1 where GroupHallFixtureConsoleId=@GroupHallFixtureConsoleId and 
				FixtureType in ('Traditional_Hall_Stations','Appendix_H/O_Signage', 'Fire_Service', 'Emergency_Power')
			end
		end

		if(@FixtureStrategy='ETD')
		begin
			if(@CountConsole=1 and @controller=0)
			begin
				update GroupHallFixtureConsole set IsController=1 where GroupHallFixtureConsoleId=@GroupHallFixtureConsoleId and
				FixtureType in ('AGILE_Hall_Stations','Appendix_H/O_Signage', 'Fire_Service', 'Emergency_Power')
			end
		end

		if(@FixtureStrategy='ETA/ETD')
		begin
			if(@CountConsole=1 and @controller=0)
			begin
				update GroupHallFixtureConsole set IsController=1 where GroupHallFixtureConsoleId=@GroupHallFixtureConsoleId and
				FixtureType in ('Traditional_Hall_Stations','AGILE_Hall_Stations','Appendix_H/O_Signage', 'Fire_Service', 'Emergency_Power')
			end
		end
		
		
		FETCH NEXT FROM db_cursor INTO @GroupHallFixtureConsoleId;
   END
   CLOSE db_cursor;
   DEALLOCATE db_cursor;
   declare @minUnitId int
   declare @maxUnitId int
   declare @consoleId int
   set @minUnitId=(select min(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
   set @maxUnitId=(select max(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
   exec [dbo].[usp_CheckHallStationDataForGroupHallFixtures] @groupConfigurationId,@userName,@fixtureStrategy,@hallStationVariables,0

   while(@minUnitId<=@maxUnitId)
   begin
   if(not exists(select * from GroupHallFixtureLocations where UnitId=@minUnitId))
   begin
	
        set @consoleId=(select min(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where FixtureType=N'Appendix_H/O_Signage' and GroupId=@groupConfigurationId)
        insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
            select  @consoleId,UnitId,FloorDesignation,FrontOpening,RearOpening,@userName,getdate() from OpeningLocation where UnitId=@minUnitId
        
        set @consoleId=(select min(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where FixtureType=N'Fire_Service' and GroupId=@groupConfigurationId)
        insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
            select  @consoleId,UnitId,FloorDesignation,0,0,@userName,getdate() from OpeningLocation where UnitId=@minUnitId
        update GroupHallFixtureLocations set Front=1 where GroupHallFixtureConsoleId=@consoleId and FloorDesignation=@mainEgress

 

            set @consoleId=(select min(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where FixtureType=N'Emergency_Power' and GroupId=@groupConfigurationId)
        insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
            select  @consoleId,UnitId,FloorDesignation,0,0,@userName,getdate() from OpeningLocation where UnitId=@minUnitId
        update GroupHallFixtureLocations set Front=1 where GroupHallFixtureConsoleId=@consoleId and FloorDesignation=@mainEgress
	end
	else
	begin
		
		set @consoleId=(select min(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where FixtureType=N'Appendix_H/O_Signage' and GroupId=@groupConfigurationId)
		insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
			select  @consoleId,UnitId,FloorDesignation,FrontOpening,RearOpening,@userName,getdate() from OpeningLocation where UnitId=@minUnitId and [FloorDesignation] not in (select [FloorDesignation] from GroupHallFixtureLocations where GroupHallFixtureConsoleId=@consoleId )and UnitId=@minUnitId
		
		set @consoleId=(select min(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where FixtureType=N'Fire_Service' and GroupId=@groupConfigurationId)
		insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
			select  @consoleId,UnitId,FloorDesignation,FrontOpening,RearOpening,@userName,getdate() from OpeningLocation where UnitId=@minUnitId and [FloorDesignation] not in (select [FloorDesignation] from GroupHallFixtureLocations where GroupHallFixtureConsoleId=@consoleId )and UnitId=@minUnitId

			set @consoleId=(select min(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where FixtureType=N'Emergency_Power' and GroupId=@groupConfigurationId)
		insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
			select  @consoleId,UnitId,FloorDesignation,FrontOpening,RearOpening,@userName,getdate() from OpeningLocation where UnitId=@minUnitId and [FloorDesignation] not in (select [FloorDesignation] from GroupHallFixtureLocations where GroupHallFixtureConsoleId=@consoleId) and UnitId=@minUnitId

	end
	set @minUnitId+=1
	--else
	--begin
	--	set @consoleId=(select min(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where FixtureType=N'AGILE Hall Stations')
	--	insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
	--    select  @consoleId,UnitId,FloorDesignation,FrontOpening,RearOpening,@userName,getdate() from OpeningLocation where UnitId=@minUnitId
		
	--end
   end
   
   ---updating console locations (front) in case of change in opening locations for consoles with units---
	   update GroupHallFixtureLocations set GroupHallFixtureLocations.Front = OpeningLocation.Front from 
	GroupHallFixtureLocations left join OpeningLocation on GroupHallFixtureLocations.UnitId = OpeningLocation.UnitId
	and GroupHallFixtureLocations.FloorDesignation = OpeningLocation.FloorDesignation
	where OpeningLocation.GroupConfigurationid=@groupConfigurationId and OpeningLocation.Front=0
	 and 
	GroupHallFixtureLocations.GroupHallFixtureConsoleId in ( select GroupHallFixtureConsoleId from GroupHallFixtureConsole
	where GroupId=@groupConfigurationId and FixtureType not in ('Traditional_Hall_Stations','AGILE_Hall_Stations'))

	---updating console locations (rear) in case of change in opening locations for consoles with units---
	   update GroupHallFixtureLocations set 
	GroupHallFixtureLocations.Rear = OpeningLocation.Rear from 
	GroupHallFixtureLocations left join OpeningLocation on GroupHallFixtureLocations.UnitId = OpeningLocation.UnitId
	and GroupHallFixtureLocations.FloorDesignation = OpeningLocation.FloorDesignation
	where OpeningLocation.GroupConfigurationid=@groupConfigurationId and OpeningLocation.Rear=0
	 and 
	GroupHallFixtureLocations.GroupHallFixtureConsoleId in ( select GroupHallFixtureConsoleId from GroupHallFixtureConsole
	where GroupId=@groupConfigurationId and FixtureType not in ('Traditional_Hall_Stations','AGILE_Hall_Stations'))

	---updating console locations in case of change in opening locations for consoles with hall stations---
	update grouphallfixturelocations set GroupHallFixtureLocations.Front=c1.front from 
	(select *
		from (select floorDesignation, 
			case when sum(cast(Front as int))>0 then 1  else 0 end  as front, 
			case when sum(cast(rear as int))>0 then 1 else 0 end as rear, 
			hallstationname from (select distinct FloorDesignation,
            case when HallStationName like '%F_SP%' then case when Front=1 then 1 else 0 end else 0 end as Front,
            case when HallStationName like '%R_SP%' then case when Rear=1 then 1 else 0 end else 0 end as Rear,
            HallStationName
            from UnitHallStationMapping
            join @hallStationVariables hsv
            on HallStationName=hsv.UnitJson
            join Units on UnitPosition=Location Join OpeningLocation on OpeningLocation.UnitId= Units.UnitId
            where Units.GroupConfigurationId=@groupConfigurationId ) as c1 group by c1.floordesignation, HallStationName ) as c1 where c1.Rear 
			is not NULL and c1.Front is not NULL) as c1 where grouphallfixtureconsoleid in (select GroupHallFixtureConsoleId from 
			GroupHallFixtureConsole where GroupId=@groupConfigurationId and FixtureType in
			('Traditional_Hall_Stations','AGILE_Hall_Stations')) and 
			c1.Front=0 and c1.FloorDesignation=GroupHallFixtureLocations.FloorDesignation
			and c1.HallStationName=GroupHallFixtureLocations.HallStationName

	---updating console locations (rear) in case of change in opening locations for consoles with hall stations---
	update grouphallfixturelocations set GroupHallFixtureLocations.Rear=c1.rear from 
	(select *
		from (select floorDesignation, 
			case when sum(cast(Front as int))>0 then 1  else 0 end  as front, 
			case when sum(cast(rear as int))>0 then 1 else 0 end as rear, 
			hallstationname from (select distinct FloorDesignation,
            case when HallStationName like '%F_SP%' then case when Front=1 then 1 else 0 end else 0 end as Front,
            case when HallStationName like '%R_SP%' then case when Rear=1 then 1 else 0 end else 0 end as Rear,
            HallStationName
            from UnitHallStationMapping
            join @hallStationVariables hsv
            on HallStationName=hsv.UnitJson
            join Units on UnitPosition=Location Join OpeningLocation on OpeningLocation.UnitId= Units.UnitId
            where Units.GroupConfigurationId=@groupConfigurationId ) as c1 group by c1.floordesignation, HallStationName ) as c1 where c1.Rear 
			is not NULL and c1.Front is not NULL) as c1 where grouphallfixtureconsoleid in (select GroupHallFixtureConsoleId from 
			GroupHallFixtureConsole where GroupId=@groupConfigurationId and FixtureType in
			('Traditional_Hall_Stations','AGILE_Hall_Stations')) and 
			c1.Rear=0 and c1.FloorDesignation=GroupHallFixtureLocations.FloorDesignation
			and c1.HallStationName=GroupHallFixtureLocations.HallStationName

	
   select	distinct ec.ConsoleNumber UnitHallFixtureConsoleId,ec.Name UnitHallFixturenConsoleName, ec.FixtureType, ec.IsController,
		el.UnitId,isnull(ut.Designation,'') as Designation,el.FloorDesignation,openings.FloorDesignation,isnull(el.Front,0) Front,isnull(el.Rear,0) Rear,Openings.FloorNumber as FloorNumber,
		isnull(ecn.VariableType,'') as VariableType,isnull(ecn.VariableValue,'') as VariableValue,
		openings.OpeningFront,openings.openingRear,FrontOpening,RearOpening, el.HallStationName
		from GroupHallFixtureConsole ec left join GroupHallFixtureConfiguration ecn on ec.GroupHallFixtureConsoleId=ecn.GroupHallFixtureConsoleId
		left join GroupHallFixtureLocations el on ec.GroupHallFixtureConsoleId=el.GroupHallFixtureConsoleId
		left join Units ut on ut.UnitId=el.UnitId
		left join (select  FloorDesignation,FloorNumber,UnitId,Front as OpeningFront,Rear as openingRear,FrontOpening,RearOpening from OpeningLocation where UnitId = (select min(UnitId) from units where GroupConfigurationId=@groupConfigurationId and IsDeleted = 0)) as Openings
		on el.FloorDesignation=Openings.FloorDesignation --and el.UnitId=Openings.UnitId
		where ec.GroupId=@groupConfigurationId and ec.IsDeleted=0
		order by el.UnitId, ec.ConsoleNumber asc, ec.FixtureType asc
   select FloorDesignation, Front,Rear,HallStationName
			from (select floorDesignation, 
			case when sum(cast(Front as int))>0 then 1  else 0 end  as front, 
			case when sum(cast(rear as int))>0 then 1 else 0 end as rear, 
			hallstationname from (select distinct FloorDesignation,
            case when HallStationName like '%F_SP%' then case when Front=1 then 1 else 0 end else 0 end as Front,
            case when HallStationName like '%R_SP%' then case when Rear=1 then 1 else 0 end else 0 end as Rear,
            HallStationName
            from UnitHallStationMapping
            join @hallStationVariables hsv
            on HallStationName=hsv.UnitJson
            join Units on UnitPosition=Location Join OpeningLocation on OpeningLocation.UnitId= Units.UnitId
            where Units.GroupConfigurationId=@groupConfigurationId) as c1 group by c1.floordesignation, HallStationName ) as c1 where c1.Rear is not NULL and c1.Front is not NULL

   
   end
  else
  begin
  declare @nooffloor int
  --declare @minConsoleId int
  -- --select @nooffloor=isnull(Max(FloorNumber),0) from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
  -- select @chkJambMounted=cast(iif(ControlLocationValue=N'Jamb-Mounted',1,0) as bit)
  -- from ControlLocation where ControlLocationType=N'Parameters_SP.controllerLocation_SP' and
  -- GroupConfigurationId=@groupConfigurationId
  -- select @productName= ProductName from UnitSet where setid=@setId
   select @nooffloor=isnull(Max(FloorNumber),0) from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
   --declare @minUnitId int
   --declare @maxUnitId int
   set @minUnitId =(select min(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
   set @maxUnitId =(select max(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
   if(@FixtureStrategy='ETA')
   begin
		
		insert into GroupHallFixtureConsole(consolenumber,FixtureType,GroupId,Name,IsController,CreatedBy,CreatedOn)
		--values(1,'Traditional_Hall_Stations',@groupConfigurationId,N'Traditional Hall Stations 1',1,@userName,getdate())
		values(1,'Traditional_Hall_Stations',@groupConfigurationId,N'Traditional Hall Stations',1,@userName,getdate())
		select @consoleId=GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId and Name='Traditional Hall Stations'
		
		
			insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn, HallStationName)
			select @consoleId,0,FloorDesignation, Front,Rear,@userName,getdate(),HallStationName
			from (select floorDesignation, 
			case when sum(cast(Front as int))>0 then 1  else 0 end  as front, 
			case when sum(cast(rear as int))>0 then 1 else 0 end as rear, 
			hallstationname from (select distinct FloorDesignation,
            case when HallStationName like '%F_SP%' then case when Front=1 then 1 else 0 end else 0 end as Front,
            case when HallStationName like '%R_SP%' then case when Rear=1 then 1 else 0 end else 0 end as Rear,
            HallStationName
            from UnitHallStationMapping
            join @hallStationVariables hsv
            on HallStationName=hsv.UnitJson
            join Units on UnitPosition=Location Join OpeningLocation on OpeningLocation.UnitId= Units.UnitId
            where Units.GroupConfigurationId=@groupConfigurationId) as c1 group by c1.floordesignation, HallStationName ) as c1 where c1.Rear is not NULL and c1.Front is not NULL
		
		
	end
   else if(@FixtureStrategy='ETD')
	begin
		
		insert into GroupHallFixtureConsole(consolenumber,FixtureType,GroupId,Name,IsController,CreatedBy,CreatedOn)
		--values(1,'AGILE_Hall_Stations',@groupConfigurationId,N'AGILE Hall Stations 1',1,@userName,getdate())
		values(1,'AGILE_Hall_Stations',@groupConfigurationId,N'AGILE Hall Stations',1,@userName,getdate())
		select @consoleId=GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId and Name='AGILE Hall Stations'
		
		insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn, HallStationName)
		select @consoleId,0,FloorDesignation, Front,Rear,@userName,getdate(),HallStationName
			from (select floorDesignation, 
			case when sum(cast(Front as int))>0 then 1  else 0 end  as front, 
			case when sum(cast(rear as int))>0 then 1 else 0 end as rear, 
			hallstationname from (select distinct FloorDesignation,
            case when HallStationName like '%F_SP%' then case when Front=1 then 1 else 0 end else 0 end as Front,
            case when HallStationName like '%R_SP%' then case when Rear=1 then 1 else 0 end else 0 end as Rear,
            HallStationName
            from UnitHallStationMapping
            join @hallStationVariables hsv
            on HallStationName=hsv.UnitJson
            join Units on UnitPosition=Location Join OpeningLocation on OpeningLocation.UnitId= Units.UnitId
            where Units.GroupConfigurationId=@groupConfigurationId) as c1 group by c1.floordesignation, HallStationName ) as c1 where c1.Rear is not NULL and c1.Front is not NULL
		
	end
	else
	begin
		insert into GroupHallFixtureConsole(consolenumber,FixtureType,GroupId,Name,IsController,CreatedBy,CreatedOn)
		--values(1,'Traditional_Hall_Stations',@groupConfigurationId,N'Traditional Hall Stations 1',1,@userName,getdate())
		values(1,'Traditional_Hall_Stations',@groupConfigurationId,N'Traditional Hall Stations',1,@userName,getdate())
		select @consoleId=GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId and Name='Traditional Hall Stations' 
		
		insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn, HallStationName)
		select @consoleId,0,FloorDesignation, Front,Rear,@userName,getdate(),HallStationName
			from (select floorDesignation, 
			case when sum(cast(Front as int))>0 then 1  else 0 end  as front, 
			case when sum(cast(rear as int))>0 then 1 else 0 end as rear, 
			hallstationname from (select distinct FloorDesignation,
            case when HallStationName like '%F_SP%' then case when Front=1 then 1 else 0 end else 0 end as Front,
            case when HallStationName like '%R_SP%' then case when Rear=1 then 1 else 0 end else 0 end as Rear,
            HallStationName
            from UnitHallStationMapping
            join @hallStationVariables hsv
            on HallStationName=hsv.UnitJson
            join Units on UnitPosition=Location Join OpeningLocation on OpeningLocation.UnitId= Units.UnitId
            where Units.GroupConfigurationId=@groupConfigurationId) as c1 group by c1.floordesignation, HallStationName ) as c1 where c1.Rear is not NULL and c1.Front is not NULL

		update GroupHallFixtureLocations set Front=0,Rear=0 where GroupHallFixtureConsoleId=@consoleId and FloorDesignation=@mainEgress
		
		insert into GroupHallFixtureConsole(consolenumber,FixtureType,GroupId,Name,IsController,CreatedBy,CreatedOn)
		--values(1,'AGILE_Hall_Stations',@groupConfigurationId,N'AGILE Hall Stations 1',1,@userName,getdate())
		values(1,'AGILE_Hall_Stations',@groupConfigurationId,N'AGILE Hall Stations',1,@userName,getdate())
		
		select @consoleId=GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId and Name='AGILE Hall Stations'
		--set @minUnitId =(select min(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
		--set @maxUnitId =(select max(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
		
		insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn, HallStationName)
		select @consoleId,0,FloorDesignation, Front,Rear,@userName,getdate(),HallStationName
			from (select floorDesignation, 
			case when sum(cast(Front as int))>0 then 1  else 0 end  as front, 
			case when sum(cast(rear as int))>0 then 1 else 0 end as rear, 
			hallstationname from (select distinct FloorDesignation,
            case when HallStationName like '%F_SP%' then case when Front=1 then 1 else 0 end else 0 end as Front,
            case when HallStationName like '%R_SP%' then case when Rear=1 then 1 else 0 end else 0 end as Rear,
            HallStationName
            from UnitHallStationMapping
            join @hallStationVariables hsv
            on HallStationName=hsv.UnitJson
            join Units on UnitPosition=Location Join OpeningLocation on OpeningLocation.UnitId= Units.UnitId
            where Units.GroupConfigurationId=@groupConfigurationId) as c1 group by c1.floordesignation, HallStationName ) as c1 where c1.Rear is not NULL and c1.Front is not NULL
		
		update GroupHallFixtureLocations set Front=0,Rear=0 where GroupHallFixtureConsoleId=@consoleId and FloorDesignation<>@mainEgress
		
		
	end
	insert into GroupHallFixtureConsole(consolenumber,FixtureType,GroupId,Name,IsController,CreatedBy,CreatedOn)
		--values(1,'Appendix_H/O_Signage',@groupConfigurationId,N'Appendix H/O Signage 1',1,@userName,getdate())
		values(1,'Appendix_H/O_Signage',@groupConfigurationId,N'Appendix H/O Signage',1,@userName,getdate())
		select @consoleId=GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId and Name='Appendix H/O Signage'
		--set @minUnitId =(select min(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
		--set @maxUnitId =(select max(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)		
		insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
	    select  @consoleId,UnitId,FloorDesignation,Front,Rear,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId order by OpeningLocationId
		
	insert into GroupHallFixtureConsole(consolenumber,FixtureType,GroupId,Name,IsController,CreatedBy,CreatedOn)
		--values(1,'Fire_Service',@groupConfigurationId,N'Fire Service 1',1,@userName,getdate())
		values(1,'Fire_Service',@groupConfigurationId,N'Fire Service',1,@userName,getdate())
		select @consoleId=GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId and Name='Fire Service'
		--set @minUnitId =(select min(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
		--set @maxUnitId =(select max(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)		
		insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
	    select  @consoleId,UnitId,FloorDesignation,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId order by OpeningLocationId
		
		update GroupHallFixtureLocations set Front=1 where GroupHallFixtureConsoleId=@consoleId and FloorDesignation=@mainEgress
	
	insert into GroupHallFixtureConsole(consolenumber,FixtureType,GroupId,Name,IsController,CreatedBy,CreatedOn)
		--values(1,'Emergency_Power',@groupConfigurationId,N'Emergency Power 1',1,@userName,getdate())
		values(1,'Emergency_Power',@groupConfigurationId,N'Emergency Power',1,@userName,getdate())
		select @consoleId=GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId and 
		Name='Emergency Power'
		--set @minUnitId =(select min(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)
		--set @maxUnitId =(select max(UnitId) from Units where GroupConfigurationId=@groupConfigurationId)		
		insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,CreatedBy,createdOn)
	    select  @consoleId,UnitId,FloorDesignation,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId order by OpeningLocationId
		
		update GroupHallFixtureLocations set Front=1 where GroupHallFixtureConsoleId=@consoleId and FloorDesignation=@mainEgress
   
    select	distinct ec.ConsoleNumber UnitHallFixtureConsoleId,ec.Name UnitHallFixturenConsoleName, ec.FixtureType, ec.IsController,
		el.UnitId,ut.Designation,el.FloorDesignation,openings.FloorDesignation,el.Front,el.Rear,Openings.FloorNumber as FloorNumber,
		isnull(ecn.VariableType,'') as VariableType,isnull(ecn.VariableValue,'') as VariableValue,
		openings.OpeningFront,openings.openingRear,FrontOpening,RearOpening, el.HallStationName 
		from GroupHallFixtureConsole ec left join GroupHallFixtureConfiguration ecn on ec.GroupHallFixtureConsoleId=ecn.GroupHallFixtureConsoleId
		left join GroupHallFixtureLocations el on ec.GroupHallFixtureConsoleId=el.GroupHallFixtureConsoleId
		left join Units ut on ut.UnitId=el.UnitId
		join (select distinct FloorDesignation,FloorNumber,Front as OpeningFront,Rear as openingRear,FrontOpening,RearOpening from OpeningLocation where UnitId = (select min(UnitId) from units where GroupConfigurationId=@groupConfigurationId  and IsDeleted = 0))as Openings
		on el.FloorDesignation=Openings.FloorDesignation
		where ec.GroupId=@groupConfigurationId and ec.IsDeleted=0
		order by ec.ConsoleNumber asc, ec.FixtureType asc
	select FloorDesignation, Front,Rear,HallStationName
			from (select floorDesignation, 
			case when sum(cast(Front as int))>0 then 1  else 0 end  as front, 
			case when sum(cast(rear as int))>0 then 1 else 0 end as rear, 
			hallstationname from (select distinct FloorDesignation,
            case when HallStationName like '%F_SP%' then case when Front=1 then 1 else 0 end else 0 end as Front,
            case when HallStationName like '%R_SP%' then case when Rear=1 then 1 else 0 end else 0 end as Rear,
            HallStationName
            from UnitHallStationMapping
            join @hallStationVariables hsv
            on HallStationName=hsv.UnitJson
            join Units on UnitPosition=Location Join OpeningLocation on OpeningLocation.UnitId= Units.UnitId
            where Units.GroupConfigurationId=@groupConfigurationId ) as c1 group by c1.floordesignation, HallStationName ) as c1 where c1.Rear is not NULL and c1.Front is not NULL


	update Building set BuildingEquipmentStatus='BLDGEQP_AV'
	from Building b inner join GroupConfiguration g on b.id=g.BuildingId
	where g.GroupId=@groupConfigurationId

   end

  --	select ControlLocationValue from ControlLocation where GroupConfigurationId = @groupConfigurationId and ControlLocationType = 'Parameters_SP.fixtureStrategy_SP'
END 
 

