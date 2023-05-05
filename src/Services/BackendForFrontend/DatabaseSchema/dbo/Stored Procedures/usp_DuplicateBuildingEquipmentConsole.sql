
CREATE PROCEDURE [dbo].[usp_DuplicateBuildingEquipmentConsole]
	-- Add the parameters for the stored procedure here
	@buildingId INT,
	@consoleId INT,
	@userId nvarchar(max),
	@Result INT OUTPUT
AS
BEGIN
	BEGIN TRY
		IF(EXISTS(Select * from BldgEquipmentConsole bec where bec.BuildingId = @BuildingId and bec.ConsoleNumber = @consoleId))
		BEGIN
		DECLARE @NextConsoleNum INT
		select @NextConsoleNum = (Select max(ConsoleNumber) from BldgEquipmentConsole where BuildingId = @buildingId)
		DECLARE @Lobby INT
		select @Lobby = (select count(IsLobby) from BldgEquipmentConsole where BuildingId = @buildingId and IsLobby=1)
		DECLARE @OldName Nvarchar(max)
		SET @OldName = (select Name from BldgEquipmentConsole bec where bec.BuildingId = @BuildingId and bec.ConsoleNumber = @consoleId)
		DECLARE @NewName Nvarchar(max)
		DECLARE @LastChar Char
		SET @LastChar = right((@OldName), 1)
		if @LastChar like '%[0-9]' 
		begin
			set @NewName = concat(left(@OldName, len(@OldName)-1), cast((cast (@Lobby as int) +1) as nvarchar(10)))
		end
		else
		begin
			set @NewName = concat(@OldName, cast(1 as nvarchar(10)))
		end
			INSERT INTO BldgEquipmentConsole
			 (
			  ConsoleNumber
			  ,BuildingId
			 ,Name
			 ,IsController
			 ,IsLobby 
			 ,CreatedBy
			 ,CreatedOn
			 )
			SELECT @NextConsoleNum+1
				  ,@BuildingId
				  ,@NewName
				  ,0
				  ,IsLobby
				  ,@userId
				  ,getdate() 
				FROM BldgEquipmentConsole where BuildingId = @buildingId and ConsoleNumber = @consoleId

			DECLARE @ConsoleIdFromConsoleTable INT
			SET @ConsoleIdFromConsoleTable = (select ConsoleId from BldgEquipmentConsole where BuildingId = @BuildingId and ConsoleNumber = @NextConsoleNum+1)

			INSERT INTO BldgEquipmentConsoleCnfgn
			   (
				 ConsoleId
				,VariableType
				,Value
				,CreatedBy
				,CreatedOn
				,ModifiedBy
				,ModifiedOn
				)

			 SELECT @ConsoleIdFromConsoleTable
					,VariableType
					,Value
					,@userId
					,GETDATE()
					,@userId
					,GETDATE()
				FROM BldgEquipmentConsoleCnfgn where ConsoleId = 
				(select ConsoleId from BldgEquipmentConsole where BuildingId = @buildingId and ConsoleNumber = @consoleId)
			SET @Result=1
		END
		ELSE
		BEGIN
			SET @Result=0
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
