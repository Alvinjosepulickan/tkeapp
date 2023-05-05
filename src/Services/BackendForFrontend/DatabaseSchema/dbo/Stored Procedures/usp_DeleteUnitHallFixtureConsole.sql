
CREATE PROCEDURE [dbo].[usp_DeleteUnitHallFixtureConsole]--96,2
	-- Add the parameters for the stored procedure here
	@SetId int,
	@consoleId int,
	@FixtureType nvarchar(50),
	@historyTable as HistoryTable readonly,
	@CreatedBy nvarchar(100),
	@Result int output
AS
BEGIN
	BEGIN TRY
		if exists(select * from UnitHallFixtureConsole where SetId=@SetId and ConsoleNumber=@consoleId and FixtureType=@FixtureType)
		begin
			--deleting entries for unit hall fixture configuration for the console
			delete from UnitHallFixtureConfiguration 
			where UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where 
			SetId=@SetId and ConsoleNumber=@consoleId and FixtureType=@FixtureType)

			--deleting entries for unit hall fixture locations for the console
			delete from UnitHallFixtureLocations
			where UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where 
			SetId=@SetId and ConsoleNumber=@consoleId and FixtureType=@FixtureType)

			--deleting entries to entrance console table
			delete from UnitHallFixtureConsole
			where SetId=@SetId and ConsoleNumber=@consoleId and FixtureType=@FixtureType

			--updating consolenumber of consoles which are after deleted consoles
			declare @NewName nvarchar(50)
			set @NewName = REPLACE(@FixtureType, '_',' ')
			declare @OldConsoleNum int
			declare @ConsoleCount int
			select @ConsoleCount = count(UnitHallFixtureConsoleId) from UnitHallFixtureConsole where SetId=@SetId and FixtureType=@FixtureType
			if(@ConsoleCount=1)
			begin
				select @OldConsoleNum = ConsoleNumber from UnitHallFixtureConsole where SetId=@SetId and FixtureType=@FixtureType
				if(@OldConsoleNum=2)
				begin
					update UnitHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
					Name = @NewName where SetId=@SetId and FixtureType=@FixtureType
					end
					else
					begin
					update UnitHallFixtureConsole set Name = @NewName where SetId=@SetId and FixtureType=@FixtureType
					end
		

			end
			else
			begin
				update UnitHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
				Name = concat(@NewName ,' ', cast(ConsoleNumber-1 as nvarchar(10)))
				where ConsoleNumber>@consoleId and SetId=@SetId and FixtureType=@FixtureType
			end

			--declare @defaultConsole int
			--select @defaultConsole=count(UnitHallFixtureConsoleId) from UnitHallFixtureConsole where SetId=@SetId and IsController=1 
			--and FixtureType = @FixtureType
			--update UnitHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
			--Name = case when @defaultConsole>0 then concat(@FixtureType ,cast(ConsoleNumber-2 as nvarchar(10)))
			--else concat(@FixtureType ,cast(ConsoleNumber-1 as nvarchar(10))) end
			--where ConsoleNumber>@consoleId and SetId=@SetId and FixtureType=@FixtureType


			----old code------
			--declare @columnCheck int
			--declare @FixtureStrategy nvarchar(50)
			--select @FixtureStrategy=ControlLocationValue from ControlLocation where ControlLocationType='Parameters_SP.fixtureStrategy_SP'
			--and GroupConfigurationId=(select GroupConfigurationId from Units where SetId=@SetId)
			--if(@FixtureStrategy='ETA')
			--begin 
			--	select @columnCheck = UnitHallFixtureTypeId from UnitHallFixtureTypes where UnitHallFixtureType=@FixtureType and ETADefault=1  
			--end
			--else
			--begin
			--	select @columnCheck = UnitHallFixtureTypeId from UnitHallFixtureTypes where UnitHallFixtureType=@FixtureType and ETDDefault=1
			--end
			--declare @OldConsoleNum int
	
			--declare @NewName nvarchar(50)
			--set @NewName = REPLACE(@FixtureType, '_',' ')
			--declare @countConsole int
			--select @countConsole=count(UnitHallFixtureConsoleId) from UnitHallFixtureConsole where SetId=@SetId and FixtureType=@FixtureType
			--if(@countConsole=1)
			--begin
			--	select @OldConsoleNum = ConsoleNumber from UnitHallFixtureConsole where SetId=@SetId and FixtureType=@FixtureType
			--	if(@columnCheck is not null)
			--	begin
			--		if(@OldConsoleNum=2)
			--		begin
			--		update UnitHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
			--		Name = @NewName, IsController=1 where SetId=@SetId and FixtureType=@FixtureType
			--		end
			--		else
			--		begin
			--		update UnitHallFixtureConsole set Name = @NewName, IsController=1 where SetId=@SetId and FixtureType=@FixtureType
			--		end
			--	end
			--	else
			--	begin
			--		if(@OldConsoleNum=2)
			--		begin
			--		--update UnitHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
			--		--Name = concat (@NewName,' ','1') where SetId=@SetId and FixtureType=@FixtureType
			--		update UnitHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
			--		Name = @NewName where SetId=@SetId and FixtureType=@FixtureType
			--		end
			--		else
			--		begin
			--		--update UnitHallFixtureConsole set Name = concat (@NewName,' ','1') where SetId=@SetId and FixtureType=@FixtureType
			--		update UnitHallFixtureConsole set Name = @NewName where SetId=@SetId and FixtureType=@FixtureType
			--		end
			--	end
		
			--end
			--else
			--begin
			--	if(@columnCheck is not null )
			--	begin
			--		declare @minConsoleNum int
			--		select @minConsoleNum =  min(ConsoleNumber) from UnitHallFixtureConsole where SetId=@SetId and FixtureType=@FixtureType
			--		if(@minConsoleNum =2)
			--		begin
			--			update UnitHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
			--			--Name = concat(@NewName ,' ', cast(ConsoleNumber-2 as nvarchar(10)))
			--			Name = concat(@NewName ,' ', cast(ConsoleNumber-1 as nvarchar(10)))
			--			where ConsoleNumber>@consoleId and SetId=@SetId and FixtureType=@FixtureType

			--			--update UnitHallFixtureConsole set 
			--			--Name = @NewName
			--			--where ConsoleNumber=1 and SetId=@SetId and FixtureType=@FixtureType
			--		end
			--		else
			--		begin
			--			update UnitHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
			--			--Name = concat(@NewName ,' ', cast(ConsoleNumber-2 as nvarchar(10)))
			--			Name = concat(@NewName ,' ', cast(ConsoleNumber-1 as nvarchar(10)))
			--			where ConsoleNumber>@consoleId and SetId=@SetId and FixtureType=@FixtureType
			--		end
			--	end
			--	else
			--	begin
			--		update UnitHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
			--		Name = concat(@NewName ,' ', cast(ConsoleNumber-1 as nvarchar(10)))
			--		where ConsoleNumber>@consoleId and SetId=@SetId and FixtureType=@FixtureType
			--	end
			--end
			------old code-------

			/**HistoryTable**/

			insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select @SetId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
			from @historyTable

			/**HistoryTable**/

			set @Result=1
		end
		else
		begin
			set @Result=0
		end
	END TRY
	BEGIN CATCH
	EXEC usp_Log_ProcedureCall
			@ObjectID = @@PROCID,
			@AdditionalInfo=@SetId;
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
END
