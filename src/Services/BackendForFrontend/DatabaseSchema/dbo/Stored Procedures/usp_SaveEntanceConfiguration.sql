
CREATE PROCEDURE [dbo].[usp_SaveEntanceConfiguration]
	@SetId INT,
	@ConsoleNumber INT,
	@EntranceConsoleVariables AS EntranceConsoleInfoDataTable READONLY,
	@EntranceConfigurationVariables AS EntranceConfigurationDataTable READONLY,
	@EntranceLocationVariables AS EntranceLocationDataTable READONLY,
	@isReset bit,
	@CreatedBy NVARCHAR(50),
	@Result INT OUTPUT,
	@historyTable as HistoryTable readonly
AS
BEGIN
	BEGIN TRY
		 declare @statusKey nvarchar
			set @statusKey = 'UNIT_INC'

		--declare @maxVersion nvarchar(50)
		--declare @versionNumber int
		IF(EXISTS (SELECT * FROM EntranceConsole WHERE ConsoleNumber=@ConsoleNumber AND SetId=@SetId))
		BEGIN
		 
			/****** Entrance Console******/

			UPDATE EntranceConsole 
			  SET Name = ecv.ConsoleName
				 ,IsController= ecv.IsController
				 ,ModifiedBy =@CreatedBy 
				 ,ModifiedOn = GETDATE()
				FROM @EntranceConsoleVariables ecv  
					WHERE ConsoleNumber = @ConsoleNumber AND SetId = @SetId

			/****** Entrance Configuration History******/
			--update EntranceConfigHistory set Islatest=0 
			--from EntranceCoEntranceConfigHistorynsole ech 
			--	 LEFT JOIN EntranceConsole ec on ech.EntranceConsoleId = ec.EntranceConsoleId
			--	 where ec.ConsoleNumber= @ConsoleNumber AND ec.SetId = @SetId
		
			--select @maxVersion=isnull(max(version),'') from EntranceConfigHistory ech
			--	JOIN EntranceConsole ec on ech.EntranceConsoleId = ec.EntranceConsoleId
			--where ec.ConsoleNumber= @ConsoleNumber AND ec.SetId = @SetId
			--SELECT @versionNumber=isnull(cast(value  as int),0)+1
			--FROM STRING_SPLIT(@maxVersion, 'V')  
			--WHERE RTRIM(value) <> '';
			--insert into EntranceConfigHistory
			--(	EntranceConsoleId
			--	,VariableType
			--	,VariableValue
			--	,CreatedBy
			--	,CreatedOn
			--	,ModifiedBy
			--	,ModifiedOn
			--	,Version

			--)
			--SELECT ec.EntranceConsoleId
			--		,VariableType
			--		,VariableValue
			--		,ec.CreatedBy
			--		,ec.CreatedOn
			--		,@CreatedBy
			--		,GETDATE()
			--		,CONCAT(N'V',cast(@versionNumber as nvarchar(50)))
			--	from EntranceConsole ec left join 
			--		 EntranceConfiguration ecn on ec.EntranceConsoleId=ecn.EntranceConsoleId
			--	where ConsoleNumber =@ConsoleNumber AND SetId=@SetId


			/****** Entrance Configuration******/
		
			DELETE FROM EntranceConfiguration 
			   WHERE EntranceConsoleId in
			   (
				  SELECT DISTINCT EntranceConsoleId 
					  FROM EntranceConsole 
						WHERE ConsoleNumber =@ConsoleNumber AND SetId=@SetId
				)


			INSERT INTO EntranceConfiguration
			   (
				 EntranceConsoleId
				,VariableType
				,VariableValue
				,CreatedBy
				,CreatedOn
				,ModifiedBy
				,ModifiedOn
				)

			 SELECT ec.EntranceConsoleId
					,VariableType
					,VariableValue
					,ec.CreatedBy
					,ec.CreatedOn
					,@CreatedBy
					,GETDATE()
				FROM @EntranceConfigurationVariables ecv 
					LEFT JOIN EntranceConsole ec on ecv.EntranceConsoleId = ec.ConsoleNumber
						WHERE ecv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId


			/****** Entrance Location******/
			--update EntranceLocationsHistory set Islatest=0 
			--from EntranceLocationsHistory elh 
			--	 LEFT JOIN EntranceConsole ec on elh.EntranceConsoleId = ec.EntranceConsoleId
			--	 where ec.ConsoleNumber= @ConsoleNumber AND ec.SetId = @SetId

			--select @maxVersion=isnull(max(version),'') from EntranceLocationsHistory elh
			--join EntranceConsole ec on elh.EntranceConsoleId = ec.EntranceConsoleId
			--where ec.ConsoleNumber= @ConsoleNumber AND ec.SetId = @SetId
			--SELECT @versionNumber=isnull(cast(value  as int),0)+1
			--FROM STRING_SPLIT(@maxVersion, 'V')  
			--WHERE RTRIM(value) <> '';

			--insert into EntranceLocationsHistory
			--(
			--	EntranceConsoleId
			--   ,FloorNumber
			--   ,Front
			--   ,Rear
			--   ,CreatedBy
			--   ,CreatedOn
			--   ,ModifiedBy
			--   ,ModifiedOn
			--   ,version
			--)
			--select ec.EntranceConsoleId
			--       ,el.FloorNumber
			--	   ,el.Front
			--	   ,el.Rear
			--	   ,ec.CreatedBy
			--	   ,ec.CreatedOn
			--	   ,@CreatedBy
			--	   ,GETDATE()
			--	   ,CONCAT(N'V',cast(@versionNumber as nvarchar(50)))
			--from EntranceConsole ec left join 
			--	 EntranceLocations el on ec.EntranceConsoleId=el.EntranceConsoleId
			--where ConsoleNumber =@ConsoleNumber AND SetId=@SetId

			DELETE FROM EntranceLocations 
			   WHERE EntranceConsoleId in
			   (
				  SELECT DISTINCT EntranceConsoleId 
					  FROM EntranceConsole 
						WHERE ConsoleNumber =@ConsoleNumber AND SetId=@SetId
				)


			INSERT INTO EntranceLocations
			  (
				EntranceConsoleId
			   ,FloorNumber
			   ,Front
			   ,Rear
			   ,CreatedBy
			   ,CreatedOn
			   ,ModifiedBy
			   ,ModifiedOn
			   )

			 SELECT ec.EntranceConsoleId
				   ,elv.FloorNumber
				   ,elv.Front
				   ,elv.Rear
				   ,ec.CreatedBy
				   ,ec.CreatedOn
				   ,@CreatedBy
				   ,GETDATE()
				FROM @EntranceLocationVariables elv 
				  LEFT JOIN EntranceConsole ec ON elv.EntranceConsoleId = ec.ConsoleNumber
					 WHERE elv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId
				  
		END
		ELSE
		BEGIN
		   if(@isReset=0)
		   begin
				/****** Entrance Console******/
			INSERT INTO EntranceConsole
			 (
			  ConsoleNumber
			 ,Name
			 ,IsController
			 ,SetId
			 ,CreatedBy
			 ,CreatedOn
			 )
			SELECT ConsoleId
				  ,ConsoleName
				  ,IsController
				  ,@SetId
				  ,@CreatedBy
				  ,getdate() 
				FROM @EntranceConsoleVariables  

			 /****** Entrance Configuration******/
			INSERT INTO EntranceConfiguration
			 (
			   EntranceConsoleId
			  ,VariableType
			  ,VariableValue
			  ,CreatedBy
			  ,CreatedOn
			  )
			 SELECT ec.EntranceConsoleId
				   ,ecv.VariableType
				   ,ecv.VariableValue
				   ,@CreatedBy
				   ,GETDATE()  
				FROM @EntranceConfigurationVariables ecv 
				  LEFT JOIN EntranceConsole ec ON ecv.EntranceConsoleId = ec.ConsoleNumber
				  WHERE ecv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId

			/****** Entrance Location******/
	    		INSERT INTO EntranceLocations
				 (
				   EntranceConsoleId
				  ,FloorNumber
				  ,Front
				  ,Rear
				  ,CreatedBy
				  ,CreatedOn
				 )
			 SELECT ec.EntranceConsoleId
				   ,elv.FloorNumber
				   ,elv.Front
				   ,elv.Rear
				   ,@CreatedBy
				   ,GETDATE()  
				FROM @EntranceLocationVariables elv 
				  LEFT JOIN EntranceConsole ec ON elv.EntranceConsoleId = ec.ConsoleNumber
					 WHERE elv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId
		   
		   end
       
		END

		--deselect the front openings which are overwritten
		update EntranceLocations 
			set Front = 0 
			where FloorNumber in(select distinct FloorNumber 
								from @EntranceLocationVariables
								where Front = 1 ) and
				  EntranceConsoleId in(select EntranceConsoleId 
									   from EntranceConsole 
									   where SetId = @SetId and ConsoleNumber <> @ConsoleNumber)

		--deselect the Rear openings which are overwritten
		update EntranceLocations 
			set Rear = 0 
			where FloorNumber in(select distinct FloorNumber 
								from @EntranceLocationVariables
								where Rear = 1 ) and
				  EntranceConsoleId in(select EntranceConsoleId 
									   from EntranceConsole 
									   where SetId = @SetId and ConsoleNumber <> @ConsoleNumber)


		--delete Entrance console which doesnt have no openings selected
		declare @entranceconsoleNumber int
		declare db_cursor cursor for
			select distinct consolenumber from EntranceConsole
			where EntranceConsoleId not in(
										select distinct el.EntranceConsoleId from EntranceLocations el 
										join EntranceConsole ec on ec.entranceconsoleid=el.entranceconsoleid
										where (Front=1 or Rear=1) and setId=@SetId
										group By el.EntranceConsoleId
									   )and SetId=@SetId
			order by consolenumber desc
		open db_cursor  
		fetch next from db_cursor into @entranceconsoleNumber
		while @@FETCH_STATUS = 0  
			begin  
				  declare @sp_result int
				  declare @history as HistoryTable
				  exec [dbo].[usp_DeleteEntranceConsole]@SetId,@entranceconsoleNumber,@history,@CreatedBy,@sp_result
				  fetch next from db_cursor into @entranceconsoleNumber
			end 

		close db_cursor  
		deallocate db_cursor
		/**HistoryTable**/

		insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @SetId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
		from @historyTable

		/**HistoryTable**/

	
	if(exists(select distinct StatusKey from Systems where SetId = @SetId))
	  begin
	  -- condition started 
		update Systems
		set StatusKey = @statusKey where SetId = @setId
		select StatusName  SystemStatus, DisplayName,Description from Status where StatusKey in (select StatusKey from Systems where SetId = @SetId)

		  --select Systems.SetId,Systems.SystemValidKeys,SystemsMasterValues.SystemsDescriptionKeys,SystemsMasterValues.SystemsDescriptionValues,
			 -- Status.StatusName,Systems.CreatedBy from Systems
			 -- inner join SystemsMasterValues on Systems.SystemValidValues = SystemsMasterValues.SystemsDescriptionKeys
			 -- inner join Status on Systems.StatusKey = Status.StatusKey
			 -- where Systems.SetId = @setId
		--select  from Systems where SetId = @setId

	  -- if condition ended 
	  end
	  else
	  begin  

		insert into Systems (SetId,StatusKey)
		values(@SetId,@statusKey)	
		select StatusName as SystemStatus, DisplayName, Description from Status where StatusKey = @statusKey
	  end

	  SET @Result=@SetId
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
