

CREATE PROCEDURE [dbo].[usp_SaveCarCallOutKeyswitchLocations]
	@SetId INT,
	@ConsoleNumber INT,
	@EntranceLocationVariables AS EntranceLocationDataTable READONLY,
	@historyTable as HistoryTable readonly,
	@CreatedBy NVARCHAR(50),
	@Result INT OUTPUT
AS
BEGIN
	BEGIN TRY
		IF(EXISTS (SELECT * FROM CarCallCutoutLocations WHERE ConsoleId=@ConsoleNumber AND SetId=@SetId))
		BEGIN


			DELETE FROM CarCallCutoutLocations 
			   WHERE ConsoleId = @ConsoleNumber AND SetId=@SetId


			INSERT INTO CarCallCutoutLocations
			  (
				ConsoleId
			   ,SetId	
			   ,FloorNumber
			   ,Front
			   ,Rear
			   ,CreatedBy
			   ,CreatedOn
			   ,ModifiedBy
			   ,ModifiedOn
			   )

			 SELECT elv.EntranceConsoleId
				   ,@SetId
				   ,elv.FloorNumber
				   ,elv.Front
				   ,elv.Rear
				   ,@CreatedBy
				   ,GETDATE()
				   ,@CreatedBy
				   ,GETDATE()
				FROM @EntranceLocationVariables elv 
			  

			SET @Result=@SetId	
		
		END
		ELSE
		BEGIN

			INSERT INTO CarCallCutoutLocations
				(
				ConsoleId
			   ,SetId	
			   ,FloorNumber
			   ,Front
			   ,Rear
			   ,CreatedBy
			   ,CreatedOn
				)
			SELECT elv.EntranceConsoleId
				,@SetId
				,elv.FloorNumber
				,elv.Front
				,elv.Rear
				,@CreatedBy
				,GETDATE()  
			FROM @EntranceLocationVariables elv
		   
	   
	    

		   SET @Result=@SetId
		END
		/**HistoryTable**/

			insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select @SetId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
			from @historyTable

		/**HistoryTable**/
	END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
End
