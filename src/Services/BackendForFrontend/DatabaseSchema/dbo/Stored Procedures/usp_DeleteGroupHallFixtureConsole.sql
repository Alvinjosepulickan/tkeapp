
CREATE PROCEDURE [dbo].[usp_DeleteGroupHallFixtureConsole]
	-- Add the parameters for the stored procedure here
	@GroupId int,
	@consoleId int,
	@FixtureType nvarchar(1000),
	@historyTable as HistoryTable readonly,
	@CreatedBy nvarchar(100),
	@Result int output
AS
BEGIN
	BEGIN TRY
	if exists(select * from GroupHallFixtureConsole where GroupId=@GroupId and ConsoleNumber=@consoleId and FixtureType=@FixtureType and IsController=0)
	begin
		--deleting entries for group hall fixture configuration for the console
		delete from GroupHallFixtureConfiguration 
		where GroupHallFixtureConsoleId in (select GroupHallFixtureConsoleId from GroupHallFixtureConsole where 
		GroupId=@GroupId and ConsoleNumber=@consoleId and FixtureType=@FixtureType)
	
		--deleting entries for group hall fixture locations for the console
		delete from GroupHallFixtureLocations
		where GroupHallFixtureConsoleId in (select GroupHallFixtureConsoleId from GroupHallFixtureConsole where 
		GroupId=@GroupId and ConsoleNumber=@consoleId and FixtureType=@FixtureType)


		declare @fixtureName nvarchar(1000)
		set @fixtureName=REPLACE(@FixtureType ,'_',' ')
	
		--deleting entries to group hall fixture console table
		delete from GroupHallFixtureConsole
	
		where GroupId=@GroupId and ConsoleNumber=@consoleId and FixtureType=@FixtureType

	
		--updating consolenumber of consoles which are after deleted consoles
		--declare @defaultConsole int
		--select @defaultConsole=count(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where GroupId=@GroupId and IsController=1 
		--and FixtureType = @FixtureType
		--update GroupHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
		--Name = case when @defaultConsole>0 then concat(@fixtureName,' ' ,cast(ConsoleNumber-1 as nvarchar(10)))
		--else concat(@fixtureName ,cast(ConsoleNumber-1 as nvarchar(10))) end
		--where ConsoleNumber>@consoleId and GroupId=@GroupId and FixtureType=@FixtureType

		declare @ConsoleCount int
		declare @OldConsoleNumber int
		select @ConsoleCount=count(GroupHallFixtureConsoleId) from GroupHallFixtureConsole where GroupId=@GroupId and FixtureType=@FixtureType
		if(@ConsoleCount=1)
		begin
			select @OldConsoleNumber = ConsoleNumber from GroupHallFixtureConsole where GroupId=@GroupId and FixtureType=@FixtureType
			if(@OldConsoleNumber=2)
			begin
				update GroupHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
				Name = @fixtureName where GroupId=@GroupId and FixtureType=@FixtureType
			end
			else
			begin
				update GroupHallFixtureConsole set Name = @fixtureName where GroupId=@GroupId and FixtureType=@FixtureType
			end
		end
		else
		begin
			update GroupHallFixtureConsole set ConsoleNumber=ConsoleNumber-1,
			Name = concat(@fixtureName ,' ', cast(ConsoleNumber-1 as nvarchar(10)))
			where ConsoleNumber>@consoleId and GroupId=@GroupId and FixtureType=@FixtureType
		end

		/**HistoryTable**/

		insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @GroupId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
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
		@AdditionalInfo=@GroupId;
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH

END
