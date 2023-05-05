
CREATE PROCEDURE [dbo].[usp_ResetUnitHallFixtureConsoleNew]
	-- Add the parameters for the stored procedure here
	@SetId int,
	@FixtureType nvarchar(100),
	@userName nvarchar(100),
	@ConsoleNumber int

AS
BEGIN
	BEGIN TRY
		--delete from UnitHallFixtureLocations where UnitHallFixtureConsoleId = (select UnitHallFixtureConsoleId from UnitHallFixtureConsole
		--where ConsoleNumber=@ConsoleNumber and FixtureType=@FixtureType and SetId=@SetId)
		declare @groupConfigurationId int
		set @groupConfigurationId = (select distinct GroupConfigurationId from units where SetId=@SetId)
		declare @minUnitId int
		select @minUnitId=(select min(UnitId) from Units where SetId=@setId)

		declare @rear int
		declare @consoleId int
  
		set @rear=0
 
		if(exists(select * from Doors where UnitId=@minUnitId and DoorType like '%rear%'))
		begin
		set @rear=1
		end

		if(@FixtureType='Hoistway_Access' and @ConsoleNumber=1)
		begin
				select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Hoistway Access'
		insert into ResetConsoleTable(ConsoleId,FloorNumber,Front,Rear)
			select distinct @consoleId,FloorNumber,0,0 from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0

			update ResetConsoleTable set  Front=1 where  ConsoleId=@consoleId and 
				FloorNumber = (select min(FloorNumber) from OpeningLocation where  GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				update ResetConsoleTable set  Front=1 where  ConsoleId=@consoleId and 
				FloorNumber = (select max(FloorNumber) from OpeningLocation where  GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
			if(@rear > 0)
				begin
				update ResetConsoleTable set  Rear=1 where ConsoleId=@consoleId and 
				FloorNumber = (select min(FloorNumber) from OpeningLocation where  GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				update ResetConsoleTable set  Rear=1 where ConsoleId=@consoleId and 
				FloorNumber = (select max(FloorNumber) from OpeningLocation where  GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
				end
		end

		else if(@FixtureType='Braille' and @ConsoleNumber=1)
		begin 
		select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@setId and Name='Braille'
			insert into ResetConsoleTable(ConsoleId,FloorNumber,Front,Rear)
			select distinct @consoleId,FloorNumber,0,0 from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
		
			update ResetConsoleTable set  Front=1 where  ConsoleId=@consoleId and 
				FloorNumber in (select  distinct FloorNumber from OpeningLocation where  GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
			if(@rear >0)
				begin
				update ResetConsoleTable set  Rear=1 where  ConsoleId=@consoleId and 
				FloorNumber in (select distinct FloorNumber from OpeningLocation where  GroupConfigurationId=@groupConfigurationId and IsDeleted=0) 
				end
		end

		else
		begin
			select @consoleId=UnitHallFixtureConsoleId from UnitHallFixtureConsole where SetId=@SetId and FixtureType=@FixtureType and ConsoleNumber=@ConsoleNumber
			insert into ResetConsoleTable(ConsoleId,FloorNumber,Front,Rear)
			select distinct @consoleId,FloorNumber,0,0 from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
		end

		select	distinct ec.ConsoleNumber UnitHallFixtureConsoleId,ec.Name UnitHallFixturenConsoleName, ec.FixtureType, ec.IsController,
			el.FloorNumber,openings.FloorDesignation,el.Front,el.Rear,
			ecn.VariableType,ecn.VariableValue,
			openings.OpeningFront,openings.openingRear,FrontOpening,RearOpening
			from UnitHallFixtureConsole ec left join UnitHallFixtureConfiguration ecn on ec.UnitHallFixtureConsoleId=ecn.UnitHallFixtureConsoleId
			left join ResetConsoleTable el on ec.UnitHallFixtureConsoleId=el.ConsoleId
			join (select distinct FloorDesignation,FloorNumber,Front as OpeningFront,Rear as openingRear,FrontOpening,RearOpening from OpeningLocation where UnitId=@minUnitId)as Openings
			on el.FloorNumber=Openings.FloorNumber
			where ec.SetId=@SetId and ec.IsDeleted=0 and ec.ConsoleNumber=@ConsoleNumber and FixtureType=@FixtureType
			order by ec.ConsoleNumber asc, ec.FixtureType asc,el.FloorNumber desc
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


