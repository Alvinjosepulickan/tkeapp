-- =============================================
-- Author:		Jesna
-- Create date: 26-Nov-2020
-- Description:	SP to delete the Entrance console
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteEntranceConsole]--96,2
	-- Add the parameters for the stored procedure here
	@SetId int,
	@consoleId int,
	@historyTable as HistoryTable readonly,
	@CreatedBy nvarchar(100),
	@Result int output
AS
BEGIN
	BEGIN TRY
	if exists(select * from EntranceConsole where SetId=@SetId and ConsoleNumber=@consoleId)
	begin
		--deleting entries for entrance configuration for the console
		delete from EntranceConfiguration 
		where EntranceConsoleId in (select EntranceConsoleId from EntranceConsole where SetId=@SetId and ConsoleNumber=@consoleId)

		--deleting entries for entrance locations for the console
		delete from EntranceLocations
		where EntranceConsoleId in (select EntranceConsoleId from EntranceConsole where SetId=@SetId and ConsoleNumber=@consoleId)

		--deleting entries to entrance console table
		delete from EntranceConsole
		where SetId=@SetId and ConsoleNumber=@consoleId


		--updating consolenumber of consoles which are after deleted consoles
		declare @countControllerConsole int
		select @countControllerConsole=count(EntranceConsoleId) from EntranceConsole
		where SetId=@SetId and IsController=1
		update EntranceConsole set ConsoleNumber=ConsoleNumber-1,
		name= case when @countControllerConsole>0 then concat('EntranceConsole',cast(ConsoleNumber-2 as nvarchar(10)))
		else concat('EntranceConsole',cast(ConsoleNumber-1 as nvarchar(10))) end
		where ConsoleNumber>@consoleId and SetId=@SetId

		/**HistoryTable**/

		insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @SetId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
		from @historyTable

		/**HistoryTable**/

		set @Result=@SetId
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
