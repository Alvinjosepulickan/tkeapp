CREATE Procedure [dbo].[usp_CheckHallStationDataForGroupHallFixtures] --1133,'c2dUser','ETA/ETD' declare @hallStationVariables insert @hallStationVariables values('Parameters_SP.HS2R_SP','','')
@groupConfigurationId int,
@userName Nvarchar(100),
@fixtureStrategy NVARCHAR(10),
@hallStationVariables unitConfigurationDataTable Readonly,
@result int
AS 
BEGIN
	---Getting Main Egress Floor---
	declare @buildingId int
	set @buildingId=(select BuildingId from GroupConfiguration where GroupId=@groupConfigurationId)
	declare @mainEgress nvarchar(3)
	set @mainEgress=(select FloorDesignation from BuildingElevation where BuildingId=@buildingId and MainEgress=1)

	DECLARE @hallStation varchar(max)
	DECLARE @consoleId int
	DECLARE db_cursor CURSOR FOR select GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@groupConfigurationId 
		and fixtureType in('Traditional_Hall_Stations','AGILE_Hall_Stations')
   OPEN db_cursor;
   FETCH NEXT FROM db_cursor INTO @consoleId;
   WHILE @@FETCH_STATUS = 0
   BEGIN

		DECLARE @minConsoleId int
		select @minConsoleId = min(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where FixtureType = (select FixtureType
		from GroupHallFixtureConsole where GroupHallFixtureConsoleId=@consoleId) and GroupId=@groupConfigurationId

		IF(EXISTS(
		select distinct  UnitJson from @hallStationVariables  
		EXCEPT
		select distinct HallStationName from GroupHallFixtureLocations where GroupHallFixtureConsoleId=@consoleId))
		BEGIN
			insert into GroupHallFixtureLocations(GroupHallFixtureConsoleId,UnitId,HallStationName,FloorDesignation,Front,Rear,CreatedBy,CreatedOn)
			select distinct hallstation.consoleId,
							0,
							hallstation.UnitJson,
							ghl.FloorDesignation,
							case when @fixtureStrategy='ETA/ETD' then 
								case when @minconsoleId=@consoleId 
									then
									case when ghfc.FixtureType = 'Traditional_Hall_Stations' 
										 then case when  ghl.FloorDesignation <> @mainEgress 
													then ol.Front 
													else 0
											  end
									else
										case when  ghl.FloorDesignation = @mainEgress 
											 then ol.Front 
											 else 0
										end
									end

								else 0 
								
								end 
							else 
								case when @minconsoleId=@consoleId 
									 then ol.Front 
								else 0 
								end
							end,
							case when @fixtureStrategy='ETA/ETD' then 
								case when @minconsoleId=@consoleId 
									then
									case when ghfc.FixtureType = 'Traditional_Hall_Stations' 
										 then case when  ghl.FloorDesignation <> @mainEgress 
													then ol.Rear 
													else 0
											  end
									else
										case when  ghl.FloorDesignation = @mainEgress 
											 then ol.Rear 
											 else 0
										end
									end

								else 0 
								
								end 
							else 
								case when @minconsoleId=@consoleId 
									 then ol.Rear
								else 0 
								end
							end,
							@userName,
							GETDATE()
			FROM
			(select distinct  @consoleId consoleId,UnitJson from @hallStationVariables  
			 EXCEPT
			 select distinct @consoleId consoleId,HallStationName from GroupHallFixtureLocations where GroupHallFixtureConsoleId=@consoleId) as hallstation
			 join GroupHallFixtureLocations ghl on hallstation.consoleId=ghl.GroupHallFixtureConsoleId
			 join OpeningLocation ol on ghl.FloorDesignation = ol.FloorDesignation and ol.GroupConfigurationId=@groupConfigurationId
			 join GroupHallFixtureConsole ghfc on ghl.GroupHallFixtureConsoleId=ghfc.GroupHallFixtureConsoleId
			 

		END
		IF(EXISTS(
		select distinct HallStationName from GroupHallFixtureLocations where GroupHallFixtureConsoleId=@consoleId
		EXCEPT
		select distinct  UnitJson from @hallStationVariables  
		))
		BEGIN
			delete from GroupHallFixtureLocations
			where GroupHallFixtureConsoleId =@consoleId and
			HallStationName in(select distinct HallStationName from GroupHallFixtureLocations where GroupHallFixtureConsoleId=@consoleId
								EXCEPT
								select distinct  UnitJson from @hallStationVariables )
		END

	  FETCH NEXT FROM db_cursor INTO @consoleId;
   END
   CLOSE db_cursor;
   DEALLOCATE db_cursor;
   


   declare @grouphallfixtureconsoleId int 
   declare db_cursor cursor for
			select distinct GroupHallFixtureConsoleId from GroupHallFixtureConsole
			where GroupHallFixtureConsoleId not in(
										select distinct el.GroupHallFixtureConsoleId from GroupHallFixtureLocations el 
										join GroupHallFixtureConsole ec on ec.GroupHallFixtureConsoleId=el.GroupHallFixtureConsoleId
										where (Front=1 or Rear=1) and GroupId=@groupConfigurationId and fixtureType in ('Traditional_Hall_Stations','AGILE_Hall_Stations')
										group By el.GroupHallFixtureConsoleId 
									   )and  GroupId=@groupConfigurationId and fixtureType in ('Traditional_Hall_Stations','AGILE_Hall_Stations')
			order by GroupHallFixtureConsoleId desc
		open db_cursor  
		fetch next from db_cursor into @grouphallfixtureconsoleId
		while @@FETCH_STATUS = 0  
			begin  
				  declare @sp_result1 int
				  declare @historyTable as HistoryTable
				  declare @FixtureType as nvarchar(50)
				  declare @consolenumber int
				  select @consolenumber = consoleNumber from GroupHallFixtureConsole where GroupHallFixtureConsoleId=@grouphallfixtureconsoleId
				  select @FixtureType = fixtureType from GroupHallFixtureConsole where GroupId=@groupConfigurationId
				  and ConsoleNumber=@consolenumber
				  select @consolenumber = consoleNumber from GroupHallFixtureConsole where GroupHallFixtureConsoleId=@grouphallfixtureconsoleId
				  exec [dbo].[usp_DeleteGroupHallFixtureConsole]@groupConfigurationId,@consolenumber,@FixtureType,@historyTable,'',@sp_result1
				  fetch next from db_cursor into @grouphallfixtureconsoleId
			end 

		close db_cursor  
		deallocate db_cursor



   set @result=0
   return @result
END