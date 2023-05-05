CREATE PROCEDURE [dbo].[usp_CreateDefaultsForUHF]
@setId int,
@FixtureStrategy Nvarchar(100),
@userName Nvarchar(100),
@defaultUHFConfiguration DefaultConfigurationTable Readonly
AS
Begin
	BEGIN TRY
		declare @consoleId int
		declare @minUnitId int
		select @minUnitId=(select min(UnitId) from Units where SetId=@setId)
		declare @rear int
		set @rear=0
 
		if(exists(select * from Doors where UnitId=@minUnitId and DoorType like '%rear%'))
		begin
		set @rear=1
		end
		If(@FixtureStrategy='ETA')
		BEGIN
			/**Hoistway_Access**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hoistway_Access',@setId,N'Hoistway Access',1,@userName,getdate(),1)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hoistway Access'
			
			--Creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Hoistway_Access'
			
			--creating openings defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() from OpeningLocation 
			where UnitId=@minUnitId and IsDeleted=0
			
			--updating defaults for min and max landings

			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and IsDeleted=0) 
			and unitId=@minUnitId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId  and IsDeleted=0) 

			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId and IsDeleted=0) 
			and unitId=@minUnitId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId  and IsDeleted=0) 

			if(@rear > 0)
				begin
				update UnitHallFixtureLocations set  Rear= case when Front>0 then 0 else 1 end where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and IsDeleted=0) 
				update UnitHallFixtureLocations set  Rear= case when Front>0 then 0 else 1 end where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select max(FloorNumber) from OpeningLocation where  unitId=@minUnitId and IsDeleted=0)
				end
			/**Hoistway_Access**/

			/**Braille**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Braille',@setId,N'Braille',1,@userName,getdate(),2)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Braille'
			
			--creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Braille'
			
			--creating opening defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,Front,Rear,@userName,getdate() 
			from OpeningLocation where unitId=@minunitid and IsDeleted=0
			/**Braille**/

			/**Hall_Lantern**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Lantern',@setId,N'Hall Lantern',0,@userName,getdate(),3)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Lantern'
			
			--creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Hall_Lantern'
			
			--creating opening defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0
			/**Hall_Lantern**/

			/**Hall_PI**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_PI',@setId,N'Hall PI',0,@userName,getdate(),4)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall PI'
			
			--creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Hall_PI'
			
			--creating opening defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0
			/**Hall_PI**/

			/**Combo_Hall_Lantern**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Combo_Hall_Lantern/PI',@setId,N'Combo Hall Lantern/PI',0,@userName,getdate(),5)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Combo Hall Lantern/PI'
			
			--creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Combo_Hall_Lantern'
			
			--creating opening defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0
			/**Combo_Hall_Lantern**/
		END
		IF(@FixtureStrategy='ETD')
		BEGIN
			/**Hoistway_Access**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hoistway_Access',@setId,N'Hoistway Access',1,@userName,getdate(),1)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hoistway Access'
			
			--Creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Hoistway_Access'
			
			--creating openings defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0
			
			--updating defaults for min and max landings
			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and IsDeleted=0) 
			and unitId=@minUnitId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId  and IsDeleted=0) 

			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId and IsDeleted=0) 
			and unitId=@minUnitId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId  and IsDeleted=0)
			
			if(@rear > 0)
				begin
				update UnitHallFixtureLocations set  Rear= case when Front>0 then 0 else 1 end where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and IsDeleted=0) 
				update UnitHallFixtureLocations set  Rear= case when Front>0 then 0 else 1 end where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select max(FloorNumber) from OpeningLocation where  unitId=@minUnitId and IsDeleted=0)
				end
			/**Hoistway_Access**/

			/**Elevator_and_Floor_Designation_Braille**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Elevator_and_Floor_Designation_Braille',@setId,N'Elevator & Floor Designation Braille',1,@userName,getdate(),2)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Elevator & Floor Designation Braille'
				
			--Creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Elevator_and_Floor_Designation_Braille'
			
			--creating openings defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,Front,Rear,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0
			/**Elevator_and_Floor_Designation_Braille**/

			/**Hall_Elevator_Designation_Plate**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Elevator_Designation_Plate',@setId,N'Hall Elevator Designation Plate',1,@userName,getdate(),3)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Elevator Designation Plate'
				
			--Creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Hall_Elevator_Designation_Plate'
			
			--creating openings defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0
			/**Hall_Elevator_Designation_Plate**/
				
			/**Hall_Target_Indicator**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Target_Indicator',@setId,N'Hall Target Indicator',1,@userName,getdate(),4)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Target Indicator'
				
			--Creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Hall_Target_Indicator'
			
			--creating openings defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0
			/**Hall_Target_Indicator**/
		END
		IF(@FixtureStrategy='ETA/ETD')
		BEGIN
			/**Hoistway_Access**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hoistway_Access',@setId,N'Hoistway Access',1,@userName,getdate(),1)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hoistway Access'
			
			--Creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Hoistway_Access'
			
			--creating openings defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0
			
			--updating defaults for min and max landings
			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and IsDeleted=0) 
			and unitId=@minUnitId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId  and IsDeleted=0) 

			update UnitHallFixtureLocations set  
			Front= case when (select Front from OpeningLocation where 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId and IsDeleted=0) 
			and unitId=@minUnitId and IsDeleted=0)>0 then 1 else 0 end
			where  UnitHallFixtureConsoleId=@consoleId and 
			FloorNumber = (select max(FloorNumber) from OpeningLocation where unitId=@minUnitId  and IsDeleted=0) 

			if(@rear > 0)
				begin
				update UnitHallFixtureLocations set  Rear= case when Front>0 then 0 else 1 end where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select min(FloorNumber) from OpeningLocation where unitId=@minUnitId and IsDeleted=0) 
				update UnitHallFixtureLocations set  Rear= case when Front>0 then 0 else 1 end where  UnitHallFixtureConsoleId=@consoleId and 
				FloorNumber = (select max(FloorNumber) from OpeningLocation where  unitId=@minUnitId and IsDeleted=0)
				end
			/**Hoistway_Access**/

			/**Braille**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Braille',@setId,N'Braille',1,@userName,getdate(),2)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Braille'
			
			--creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Braille'
			
			--creating opening defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,Front,Rear,@userName,getdate() 
			from OpeningLocation where unitId=@minunitid  and IsDeleted=0
			/**Braille**/

			/**Hall_Lantern**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Lantern',@setId,N'Hall Lantern',0,@userName,getdate(),3)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Lantern'
			
			--creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Hall_Lantern'
			
			--creating opening defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0
			/**Hall_Lantern**/

			/**Hall_Elevator_Designation_Plate**/
			insert into UnitHallFixtureConsole(consolenumber,FixtureType,SetId,Name,IsController,CreatedBy,CreatedOn,ConsoleOrder)
			values(1,'Hall_Elevator_Designation_Plate',@setId,N'Hall Elevator Designation Plate',1,@userName,getdate(),3)
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hall Elevator Designation Plate'
				
			--Creating default configuration
			insert into UnitHallFixtureConfiguration(UnitHallFixtureConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @consoleId,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='Hall_Elevator_Designation_Plate'
			
			--creating openings defaults
			insert into UnitHallFixtureLocations(UnitHallFixtureConsoleId,FloorNumber,Front,Rear,CreatedBy,createdOn)
			select distinct @consoleId,FloorNumber,0,0,@userName,getdate() 
			from OpeningLocation where UnitId=@minUnitId and IsDeleted=0

			update UnitHallFixtureLocations set Front=1 where UnitHallFixtureConsoleId=@consoleId and
				FloorNumber in (select distinct FloorNumber from BuildingElevation where MainEgress=1 and BuildingId in  
				(select BuildingId from GroupConfiguration g join Units u on g.GroupId=u.GroupConfigurationId
								 WHere u.UnitId=@minUnitId))
			if(@rear>0)
			begin
				update UnitHallFixtureLocations set Rear=1 where UnitHallFixtureConsoleId=@consoleId and
				FloorNumber in (select distinct FloorNumber from BuildingElevation where MainEgress=1 and BuildingId= 
				(select BuildingId from GroupConfiguration g join Units u on g.GroupId=u.GroupConfigurationId
								 WHere u.UnitId=@minUnitId))
			end
			/**Hall_Elevator_Designation_Plate**/
		END
		
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