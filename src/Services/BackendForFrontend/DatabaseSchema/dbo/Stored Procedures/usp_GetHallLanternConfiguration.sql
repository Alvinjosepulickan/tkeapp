
CREATE PROCEDURE [dbo].[usp_GetHallLanternConfiguration] 
	-- Add the parameters for the stored procedure here
@groupConfigurationId int,
@setId int,
@FixtureStrategy Nvarchar(100),
@userName Nvarchar(100)
AS
Begin
	BEGIN TRY
		declare @minUnitId int
		select @minUnitId=(select min(UnitId) from Units where SetId=@setId)
	   if(Exists(select * from UnitHallFixtureConsole where SetId=@setId and IsDeleted=0 ))
	   begin
   
	   ------new code------
	   DECLARE @UnitHallFixtureConsoleId int
	   DECLARE db_cursor CURSOR FOR SELECT UnitHallFixtureConsoleId FROM UnitHallFixtureConsole Where SetId=@setId;
	   OPEN db_cursor;
	   FETCH NEXT FROM db_cursor INTO @UnitHallFixtureConsoleId;
	   WHILE @@FETCH_STATUS = 0
	   BEGIN
			declare @controller bit
			select @controller = IsController from UnitHallFixtureConsole where UnitHallFixtureConsoleId=@UnitHallFixtureConsoleId
			if(@FixtureStrategy='ETA')
			begin
				declare @FixtureType nvarchar(100)
				declare @CountConsole int
			
				select @FixtureType = FixtureType from UnitHallFixtureConsole where UnitHallFixtureConsoleId=@UnitHallFixtureConsoleId
				select @CountConsole=count(*) from UnitHallFixtureConsole where FixtureType=@FixtureType and SetId=@setId
				if(@CountConsole=1 and @controller=0)
				begin
					update UnitHallFixtureConsole set IsController=1 where UnitHallFixtureConsoleId=@UnitHallFixtureConsoleId and 
					FixtureType in ('Hoistway_Access','Braille')--, 'Hall_Lantern', 'Hall_PI','Combo_Hall_Lantern/PI')
				end
			end

			if(@FixtureStrategy='ETD')
			begin
				declare @FixtureType1 nvarchar(100)
				declare @CountConsole1 int
				select @FixtureType1 = FixtureType from UnitHallFixtureConsole where UnitHallFixtureConsoleId=@UnitHallFixtureConsoleId
				select @CountConsole1=count(*) from UnitHallFixtureConsole where FixtureType=@FixtureType1 and SetId=@setId
				if(@CountConsole1=1 and @controller=0)
				begin
					update UnitHallFixtureConsole set IsController=1 where UnitHallFixtureConsoleId=@UnitHallFixtureConsoleId and
					FixtureType in ('Hoistway_Access','Elevator_and_Floor_Designation_Braille','Hall_Elevator_Designation_Plate','Hall_Target_Indicator')
				end
			end

			if(@FixtureStrategy='ETA/ETD')
			begin
				declare @FixtureType2 nvarchar(100)
				declare @CountConsole2 int
				select @FixtureType2 = FixtureType from UnitHallFixtureConsole where UnitHallFixtureConsoleId=@UnitHallFixtureConsoleId
				select @CountConsole2=count(*) from UnitHallFixtureConsole where FixtureType=@FixtureType2 and SetId=@setId
				if(@CountConsole2=1 and @controller=0)
				begin
					update UnitHallFixtureConsole set IsController=1 where UnitHallFixtureConsoleId=@UnitHallFixtureConsoleId and
					FixtureType in ('Hoistway_Access','Braille','Hall_Elevator_Designation_Plate','Hall_Lantern')
				end
			end

			FETCH NEXT FROM db_cursor INTO @UnitHallFixtureConsoleId;
	   END
	   CLOSE db_cursor;
	   DEALLOCATE db_cursor;
	   --------new code-------

	   select	distinct ec.ConsoleOrder as ConsoleOrder, ec.ConsoleNumber UnitHallFixtureConsoleId,ec.Name UnitHallFixturenConsoleName, ec.FixtureType, ec.IsController,
			ecn.VariableType,ecn.VariableValue,
			el.FloorNumber,openings.FloorDesignation,el.Front,el.Rear,
			openings.OpeningFront,openings.openingRear,FrontOpening,RearOpening
			from UnitHallFixtureConsole ec left join UnitHallFixtureConfiguration ecn on ec.UnitHallFixtureConsoleId=ecn.UnitHallFixtureConsoleId
			left join UnitHallFixtureLocations el on ec.UnitHallFixtureConsoleId=el.UnitHallFixtureConsoleId
			join (select distinct FloorDesignation,FloorNumber,Front as OpeningFront,Rear as openingRear,FrontOpening,RearOpening from 
			OpeningLocation where UnitId=@minUnitId)as Openings
			on el.FloorNumber=Openings.FloorNumber
			where ec.setId=@setId and ec.IsDeleted=0
			order by ec.ConsoleOrder asc,ec.ConsoleNumber asc, ec.FixtureType asc,el.FloorNumber desc

		select * from UnitConfiguration where ConfigureVariables like '%CRLTYPE%' and setId=@setId
   
	   end
	  else
	  begin
	  declare @nooffloor int
	  declare @consoleId int
	  -- --select @nooffloor=isnull(Max(FloorNumber),0) from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
	  -- select @chkJambMounted=cast(iif(ControlLocationValue=N'Jamb-Mounted',1,0) as bit)
	  -- from ControlLocation where ControlLocationType=N'Parameters_SP.controllerLocation_SP' and
	  -- GroupConfigurationId=@groupConfigurationId
	  -- select @productName= ProductName from UnitSet where setid=@setId
	  declare @rear int
	  declare @front int
  
	  set @rear=0
 
		if(exists(select * from Doors where UnitId=@minUnitId and DoorType like '%rear%'))
		begin
		set @rear=1
		end
  
		declare @CarRidingLantern nvarchar(15)
			if(exists(select * from UnitConfiguration where ConfigureVariables like '%CRLTYPE%' and SetId=@setId))
			begin
				select @CarRidingLantern = ConfigureValues from UnitConfiguration where ConfigureVariables like '%CRLTYPE%' and SetId=@setId
			end
			else
			begin
				select @CarRidingLantern = ''
			end
  
	   select @nooffloor=isnull(Max(FloorNumber),0) from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
	   if(@FixtureStrategy='ETA')
	   begin
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hoistway_Access',@setId,N'Hoistway Access',1,@userName,getdate(),1)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hoistway Access'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0

			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) and unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 

			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) and unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)

			if(@rear > 0)
				begin
				update UnitHallFixtureLocations set  Rear= case when Front>0 then 0 else 1 end where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				update UnitHallFixtureLocations set  Rear= case when Front>0 then 0 else 1 end where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select max(FloorNumber) from OpeningLocation where  unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
				end
		
			
			
   
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Braille',@setId,N'Braille',1,@userName,getdate(),2)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Braille'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
			update UnitHallFixtureLocations set  Front=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select  distinct FloorNumber from OpeningLocation where Front=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
			if(@rear >0)
				begin
				update UnitHallFixtureLocations set  Rear=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select distinct FloorNumber from OpeningLocation where Rear=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				end
		

				
		
   
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Lantern',@setId,N'Hall Lantern',0,@userName,getdate(),3)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Lantern'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0

			if(@CarRidingLantern='NR')
			begin
				update UnitHallFixtureLocations set  Front=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select  distinct FloorNumber from OpeningLocation where Front=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
			if(@rear >0)
				begin
				update UnitHallFixtureLocations set  Rear=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select distinct FloorNumber from OpeningLocation where Rear=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				end
			end
   
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_PI',@setId,N'Hall PI',0,@userName,getdate(),4)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall PI'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0

			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Combo_Hall_Lantern/PI',@setId,N'Combo Hall Lantern/PI',0,@userName,getdate(),5)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Combo Hall Lantern/PI'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
			end

		if(@FixtureStrategy='ETD')
		begin
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hoistway_Access',@setId,N'Hoistway Access',1,@userName,getdate(),1)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hoistway Access'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
			
			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) and unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 

			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) and unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)

			if(@rear > 0)
				begin
				update UnitHallFixtureLocations set  Rear= 1, Front=0 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				update UnitHallFixtureLocations set  Rear= 1, Front=0 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select max(FloorNumber) from OpeningLocation where  unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
				end
			
	
			
			

			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Elevator_and_Floor_Designation_Braille',@setId,N'Elevator & Floor Designation Braille',1,@userName,getdate(),2)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Elevator & Floor Designation Braille'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
			update UnitHallFixtureLocations set  Front=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select distinct FloorNumber from OpeningLocation where Front=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
			if(@rear >0)

				update UnitHallFixtureLocations set  Rear=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select distinct FloorNumber from OpeningLocation where Rear=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 

		

			

			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Elevator_Designation_Plate',@setId,N'Hall Elevator Designation Plate',1,@userName,getdate(),3)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Elevator Designation Plate'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0

			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Target_Indicator',@setId,N'Hall Target Indicator',1,@userName,getdate(),4)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Target Indicator'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
		end

		if(@FixtureStrategy='ETA/ETD')
		begin

			

			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hoistway_Access',@setId,N'Hoistway Access',1,@userName,getdate(),1)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hoistway Access'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
			
			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) and unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 

			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) and unitId=@minUnitId 
			and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)

			if(@rear > 0)
				begin
				update UnitHallFixtureLocations set  Rear= 1, Front=0 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				update UnitHallFixtureLocations set  Rear= 1, Front=0 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select max(FloorNumber) from OpeningLocation where  unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
				end

			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Braille',@setId,N'Braille',1,@userName,getdate(),2)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Braille'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
			update UnitHallFixtureLocations set  Front=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select  distinct FloorNumber from OpeningLocation where Front=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
			if(@rear >0)
				begin
				update UnitHallFixtureLocations set  Rear=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select distinct FloorNumber from OpeningLocation where Rear=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				end

			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Lantern',@setId,N'Hall Lantern',1,@userName,getdate(),3)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Lantern'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
			if(@CarRidingLantern='NR')
			begin
				update UnitHallFixtureLocations set  Front=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select  distinct FloorNumber from OpeningLocation where Front=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
			if(@rear >0)
				begin
				update UnitHallFixtureLocations set  Rear=1 where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber in (select distinct FloorNumber from OpeningLocation where Rear=1 and unitId=@minUnitId and GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				end
			end

			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Elevator_Designation_Plate',@setId,N'Hall Elevator Designation Plate',1,@userName,getdate(),4)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Elevator Designation Plate'
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
			update UnitHallFixtureLocations set Front=1 where UnitHallFixtureConsoleId=@consoleId and
				FloorNumber in (select distinct FloorNumber from BuildingElevation where MainEgress=1 and BuildingId= 
				(select BuildingId from GroupConfiguration where GroupId=@groupConfigurationId))
			if(@rear>0)
			begin
				update UnitHallFixtureLocations set Rear=1 where UnitHallFixtureConsoleId=@consoleId and
				FloorNumber in (select distinct FloorNumber from BuildingElevation where MainEgress=1 and BuildingId= 
				(select BuildingId from GroupConfiguration where GroupId=@groupConfigurationId))
			end

		end
   
	   /*Getting unit hall fixture data*/
		select	distinct ec.ConsoleOrder ConsoleOrder
						 ,ec.ConsoleNumber UnitHallFixtureConsoleId
						 ,ec.Name UnitHallFixturenConsoleName
						 ,ec.FixtureType
						 ,ec.IsController
						 ,el.FloorNumber,openings.FloorDesignation
						 ,el.Front
						 ,el.Rear
						 ,ecn.VariableType
						 ,ecn.VariableValue
						 ,openings.OpeningFront
						 ,openings.openingRear
						 ,FrontOpening,RearOpening
			from UnitHallFixtureConsole ec left join UnitHallFixtureConfiguration ecn on ec.UnitHallFixtureConsoleId=ecn.UnitHallFixtureConsoleId
			left join UnitHallFixtureLocations el on ec.UnitHallFixtureConsoleId=el.UnitHallFixtureConsoleId
			join (select distinct FloorDesignation,FloorNumber,Front as OpeningFront,Rear as openingRear,FrontOpening,RearOpening from OpeningLocation where UnitId=@minUnitId)as Openings
			on el.FloorNumber=Openings.FloorNumber
			where ec.SetId=@setId and ec.IsDeleted=0
			order by ec.ConsoleOrder asc,ec.ConsoleNumber asc, ec.FixtureType asc,el.FloorNumber desc

			select * from UnitConfiguration where ConfigureVariables like '%CRLTYPE%' and setId=@setId
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
 
