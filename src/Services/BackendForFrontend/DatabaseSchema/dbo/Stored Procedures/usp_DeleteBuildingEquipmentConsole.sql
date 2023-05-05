
CREATE PROCEDURE [dbo].[usp_DeleteBuildingEquipmentConsole] 
	-- Add the parameters for the stored procedure here
	@buildingId INT,
	@consoleId INT,
	@userId nvarchar(max),
	@historyTable as HistoryTable readonly,
	@Result INT OUTPUT
AS
BEGIN
	BEGIN TRY
		if(exists(select * from BldgEquipmentConsole where BuildingId=@buildingId and ConsoleNumber=@consoleId and IsController=0))
		BEGIN
			delete from BldgEquipmentConsoleCnfgn 
		where ConsoleId in (select ConsoleId from BldgEquipmentConsole where 
		BuildingId=@buildingId and ConsoleNumber=@consoleId )

			delete from BldgEquipmentGroupMapping 
		where ConsoleId in (select ConsoleId from BldgEquipmentConsole where 
		BuildingId=@buildingId and ConsoleNumber=@consoleId )

			delete from BldgEquipmentCategoryCnfgn where ConsoleId in (select ConsoleId from BldgEquipmentConsole where 
		BuildingId=@buildingId and ConsoleNumber=@consoleId )

			delete from BldgEquipmentConsole where BuildingId=@buildingId and ConsoleNumber=@consoleId

			--updating consolenumber of consoles which are after deleted consoles
		declare @defaultConsole int
		select @defaultConsole=count(ConsoleId) from BldgEquipmentConsole where BuildingId=@buildingId and IsLobby=1 and IsController=1
		declare @countlobby int
		select @countlobby = count(IsLobby) from BldgEquipmentConsole where BuildingId = @buildingId and IsLobby=1
	
	  
		update BldgEquipmentConsole set ConsoleNumber= ConsoleNumber-1
 		where ConsoleNumber>@consoleId and BuildingId=@buildingId
 		

		--update BldgEquipmentConsole set  Name = replace(Name,Name, concat('Lobby Panel ', ROW_NUMBER()  over (order by  ConsoleNumber)))
		--from [dbo].[BldgEquipmentConsole]
		--   where BuildingId=100 and Name like 'Lobby Panel%%'
		declare @consoleCount int 
		set @consoleCount = (select IsController from BldgEquipmentConsole where BuildingId = @buildingId and Name = 'Lobby Panel 1')
		declare @value int
		set @value = case when @consoleCount = 1 then 4 else 5 end
		update BldgEquipmentConsole set Name = CONCAT('Lobby Panel ', ConsoleNumber-@value ) where
		BuildingId=@buildingId and ConsoleNumber>=@consoleId and IsLobby=1
	     

		/**HistoryTable**/

		insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @buildingId,variableId,Updatedvalue,PreviousValue,@userId,getdate(),@userId,getdate()
		from @historyTable

		/**HistoryTable**/


		SET @Result = 1
		
		END
	
		ELSE
		BEGIN
			SET @Result = 0
		END
	END TRY
	BEGIN CATCH
	EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo=@buildingId;
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
END
