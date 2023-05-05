
CREATE PROCEDURE [dbo].[usp_SaveUnitHallFixtureConfiguration]
	-- Add the parameters for the stored procedure here

	@SetId INT,
	@ConsoleNumber INT,
	@UnitHallFixtureConsoleVariables AS UnitHallFixtureConsoleInfoDataTable READONLY,
	@UnitHallFixtureConfigurationVariables AS EntranceConfigurationDataTable READONLY,
	@UnitHallFixtureLocationVariables AS EntranceLocationDataTable READONLY,
	@CreatedBy NVARCHAR(50),
	@Result INT OUTPUT,
	@historyTable as HistoryTable readonly
AS
BEGIN
	BEGIN TRY
	declare @FixtureType nvarchar(100);

	 declare @statusKey nvarchar(300)
			set @statusKey = 'UNIT_INC'

	/****** Variables required for history  ******/
	--declare @maxVersion nvarchar(50)
	--declare @versionNumber int

	set @FixtureType= (select FixtureType from @UnitHallFixtureConsoleVariables)
		IF(EXISTS (SELECT * FROM UnitHallFixtureConsole ec WHERE ec.ConsoleNumber=@ConsoleNumber AND ec.SetId=@SetId AND ec.FixtureType = 
		@FixtureType))
		BEGIN
		 
			/****** Hall Lantern Console******/

			UPDATE UnitHallFixtureConsole 
			  SET 
				 ModifiedBy =@CreatedBy 
				 ,ModifiedOn = GETDATE()
			 
				FROM @UnitHallFixtureConsoleVariables ecv  
					WHERE ConsoleNumber = @ConsoleNumber AND SetId = @SetId AND UnitHallFixtureConsole.FixtureType = @FixtureType

			/****** Unit Hall Fixture Configuration History******/
			--update UnitHallFixtureConfigHistory set Islatest=0 
			--from UnitHallFixtureConfigHistory uhfch 
			--	 LEFT JOIN UnitHallFixtureConsole uhfc on uhfch.UnitHallFixtureConsoleId = uhfc.UnitHallFixtureConsoleId
			--	 where uhfc.ConsoleNumber= @ConsoleNumber AND uhfc.SetId = @SetId and FixtureType=@FixtureType
		
			--select @maxVersion=isnull(max(version),'') from UnitHallFixtureConfigHistory uch join
			--UnitHallFixtureConsole uc on uch.UnitHallFixtureConsoleId=uc.UnitHallFixtureConsoleId
			--where ConsoleNumber= @ConsoleNumber AND SetId = @SetId and FixtureType=@FixtureType
			--SELECT @versionNumber=isnull(cast(value  as int),0)+1
			--FROM STRING_SPLIT(@maxVersion, 'V')  
			--WHERE RTRIM(value) <> '';

			--insert into UnitHallFixtureConfigHistory
			--(	UnitHallFixtureConsoleId
			--	,VariableType
			--	,VariableValue
			--	,CreatedBy
			--	,CreatedOn
			--	,ModifiedBy
			--	,ModifiedOn
			--	,Version

			--)
			--SELECT ec.UnitHallFixtureConsoleId
			--		,VariableType
			--		,VariableValue
			--		,ec.CreatedBy
			--		,ec.CreatedOn
			--		,@CreatedBy
			--		,GETDATE()
			--		,CONCAT(N'V',cast(@versionNumber as nvarchar(50)))
			--	from UnitHallFixtureConsole ec left join 
			--		 UnitHallFixtureConfiguration ecn on ec.UnitHallFixtureConsoleId=ecn.UnitHallFixtureConsoleId
			--	where ConsoleNumber =@ConsoleNumber AND SetId=@SetId and FixtureType=@FixtureType



			/****** Hall Lantern Configuration******/

			DELETE FROM UnitHallFixtureConfiguration 
			   WHERE UnitHallFixtureConsoleId in
			   (
				  SELECT DISTINCT UnitHallFixtureConsoleId 
					  FROM UnitHallFixtureConsole 
						WHERE ConsoleNumber =@ConsoleNumber AND SetId=@SetId AND FixtureType = @FixtureType
				)


			INSERT INTO UnitHallFixtureConfiguration
			   (
				 UnitHallFixtureConsoleId
				,VariableType
				,VariableValue
				,CreatedBy
				,CreatedOn
				,ModifiedBy
				,ModifiedOn
				)

			 SELECT ec.UnitHallFixtureConsoleId
					,VariableType
					,VariableValue
					,ec.CreatedBy
					,ec.CreatedOn
					,@CreatedBy
					,GETDATE()
				FROM @UnitHallFixtureConfigurationVariables ecv 
					LEFT JOIN UnitHallFixtureConsole ec on ecv.EntranceConsoleId = ec.ConsoleNumber
						WHERE ecv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId AND FixtureType =@FixtureType
		
			/****** Entrance Location history******/
			--update UnitHallFixtureLocationsHistory set Islatest=0 
			--from UnitHallFixtureLocationsHistory uhfch 
			--	 LEFT JOIN UnitHallFixtureConsole uhfc on uhfch.UnitHallFixtureConsoleId = uhfc.UnitHallFixtureConsoleId
			--	 where uhfc.ConsoleNumber= @ConsoleNumber AND uhfc.SetId = @SetId and FixtureType=@FixtureType
		
			--select @maxVersion=isnull(max(version),'') from UnitHallFixtureLocationsHistory uch join
			--UnitHallFixtureConsole uc on uch.UnitHallFixtureConsoleId=uc.UnitHallFixtureConsoleId
			--where ConsoleNumber= @ConsoleNumber AND SetId = @SetId and FixtureType=@FixtureType
			--SELECT @versionNumber=isnull(cast(value  as int),0)+1
			--FROM STRING_SPLIT(@maxVersion, 'V')  
			--WHERE RTRIM(value) <> '';

			--insert into UnitHallFixtureLocationsHistory
			--(
			--	UnitHallFixtureConsoleId
			--   ,FloorNumber
			--   ,Front
			--   ,Rear
			--   ,CreatedBy
			--   ,CreatedOn
			--   ,ModifiedBy
			--   ,ModifiedOn
			--   ,version
			--)
			--select ec.UnitHallFixtureConsoleId
			--       ,el.FloorNumber
			--	   ,el.Front
			--	   ,el.Rear
			--	   ,ec.CreatedBy
			--	   ,ec.CreatedOn
			--	   ,@CreatedBy
			--	   ,GETDATE()
			--	   ,CONCAT(N'V',cast(@versionNumber as nvarchar(50)))
			--from  UnitHallFixtureConsole ec left join 
			--	 UnitHallFixtureLocations el on ec.UnitHallFixtureConsoleId=el.UnitHallFixtureConsoleId
			--where ConsoleNumber =@ConsoleNumber AND SetId=@SetId and FixtureType=@FixtureType


			/****** Entrance Location******/

			DELETE FROM UnitHallFixtureLocations 
			   WHERE UnitHallFixtureConsoleId in
			   (
				  SELECT DISTINCT UnitHallFixtureConsoleId 
					  FROM UnitHallFixtureConsole 
						WHERE ConsoleNumber =@ConsoleNumber AND SetId=@SetId AND FixtureType=@FixtureType
				)


			INSERT INTO UnitHallFixtureLocations
			  (
				UnitHallFixtureConsoleId
			   ,FloorNumber
			   ,Front
			   ,Rear
			   ,CreatedBy
			   ,CreatedOn
			   ,ModifiedBy
			   ,ModifiedOn
			   )

			 SELECT ec.UnitHallFixtureConsoleId
				   ,elv.FloorNumber
				   ,elv.Front
				   ,elv.Rear
				   ,ec.CreatedBy
				   ,ec.CreatedOn
				   ,@CreatedBy
				   ,GETDATE()
				FROM @UnitHallFixtureLocationVariables elv 
				  LEFT JOIN UnitHallFixtureConsole ec ON elv.EntranceConsoleId = ec.ConsoleNumber
					 WHERE elv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId AND FixtureType = @FixtureType
				  
			SET @Result=@SetId
		END
		ELSE
		BEGIN
	   
			/****** Hall Lantern Console******/

			declare @ConsoleOrder int
			if(exists(select * from UnitHallFixtureConsole where FixtureType=@FixtureType and SetId=@SetId))
			begin
				select @ConsoleOrder= ConsoleOrder from UnitHallFixtureConsole where FixtureType=@FixtureType and SetId=@SetId
			end
			else
			begin
				select @ConsoleOrder= max(ConsoleOrder) from UnitHallFixtureConsole where SetId=@SetId
				set @ConsoleOrder=@ConsoleOrder+1
			end

			declare @ConsoleNumberCheck int
			select @ConsoleNumberCheck =  ConsoleId from @UnitHallFixtureConsoleVariables

			INSERT INTO UnitHallFixtureConsole
			 (
			  ConsoleNumber
			 ,Name
			 ,FixtureType
			 ,IsController
			 ,SetId
			 ,CreatedBy
			 ,CreatedOn
			 ,ConsoleOrder
			 )
			SELECT ConsoleId
				  ,ConsoleName
				  ,FixtureType
				  ,IsController
				  ,@SetId
				  ,@CreatedBy
				  ,getdate() 
				  ,@ConsoleOrder
				FROM @UnitHallFixtureConsoleVariables 
		
			if(@ConsoleNumberCheck=2)
			begin
				update UnitHallFixtureConsole set Name = concat(REPLACE(@FixtureType,'_',' '),' ',1) where ConsoleNumber=1 and FixtureType=@FixtureType
				and SetId=@SetId
			end
	

	   ---------Updating IsController property for delete functionality-----------
			if(exists(select * from UnitHallFixtureConsole where FixtureType=@FixtureType and SetId=@SetId and IsController=1))
			begin
				update UnitHallFixtureConsole set isController=0 where FixtureType=@FixtureType and SetId=@SetId and IsController=1
			end
		
			 /****** Hall Lantern Configuration******/
			INSERT INTO UnitHallFixtureConfiguration
			 (
			   UnitHallFixtureConsoleId
			  ,VariableType
			  ,VariableValue
			  ,CreatedBy
			  ,CreatedOn
			  )
			 SELECT ec.UnitHallFixtureConsoleId
				   ,ecv.VariableType
				   ,ecv.VariableValue
				   ,@CreatedBy
				   ,GETDATE()  
				FROM @UnitHallFixtureConfigurationVariables ecv 
				  LEFT JOIN UnitHallFixtureConsole ec ON ecv.EntranceConsoleId = ec.ConsoleNumber
				  WHERE ecv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId AND FixtureType = @FixtureType


			/****** Hall Lantern Location******/
	    		INSERT INTO UnitHallFixtureLocations
				 (
				   UnitHallFixtureConsoleId
				  ,FloorNumber
				  ,Front
				  ,Rear
				  ,CreatedBy
				  ,CreatedOn
				 )
			 SELECT ec.UnitHallFixtureConsoleId
				   ,elv.FloorNumber
				   ,elv.Front
				   ,elv.Rear
				   ,@CreatedBy
				   ,GETDATE()  
				FROM @UnitHallFixtureLocationVariables elv 
				  LEFT JOIN UnitHallFixtureConsole ec ON elv.EntranceConsoleId = ec.ConsoleNumber
					 WHERE elv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId AND FixtureType = @FixtureType
		   
			--SET @Result=@SetId
	
		END
		--deselect the front openings which are overwritten
		if(@FixtureType in('Combo_Hall_Lantern/PI','Hall_PI','Hall_Lantern'))
		begin 
			if(@FixtureType in ('Combo_Hall_Lantern/PI', 'Hall_PI'))
			begin
					declare @unitHallFixtureConsoleId int
					declare @uhfConsoleNo int
					declare @grouphallfixtureconsoleNumber int
					select @unitHallFixtureConsoleId =UnitHallFixtureConsoleId from UnitHallFixtureConsole where 
					ConsoleNumber = @ConsoleNumber AND SetId = @SetId AND UnitHallFixtureConsole.FixtureType = @FixtureType
					--deselect the front openings which are overwritten
					update UnitHallFixtureLocations 
					set Front = 0 
					where FloorNumber in(select distinct FloorNumber 
										from @UnitHallFixtureLocationVariables
										where Front = 1 ) and
						  UnitHallFixtureConsoleId in(select UnitHallFixtureConsoleId 
											   from UnitHallFixtureConsole 
											   where SetId = @SetId and FixtureType in ('Combo_Hall_Lantern/PI','Hall_PI') and UnithallFixtureConsoleId<>@unitHallFixtureConsoleId)
					--deselect the Rear openings which are overwritten
					update UnitHallFixtureLocations 
					set Rear = 0 
					where FloorNumber in(select distinct FloorNumber 
										from @UnitHallFixtureLocationVariables
										where Rear = 1 ) and
						  UnitHallFixtureConsoleId in(select UnitHallFixtureConsoleId 
											   from UnitHallFixtureConsole 
											   where SetId = @SetId and FixtureType in ('Combo_Hall_Lantern/PI','Hall_PI') and UnithallFixtureConsoleId<>@unitHallFixtureConsoleId)

					declare db_cursor cursor for
					select distinct UnitHallFixtureConsoleId from UnitHallFixtureConsole
					where UnitHallFixtureConsoleId not in(
												select distinct el.UnitHallFixtureConsoleId from UnitHallFixtureLocations el 
												join UnitHallFixtureConsole ec on ec.UnitHallFixtureConsoleId=el.UnitHallFixtureConsoleId
												where (Front=1 or Rear=1) and SetId=@SetId and FixtureType in('Combo_Hall_Lantern/PI','Hall_PI')
												group By el.UnitHallFixtureConsoleId
											   )and  SetId=@SetId and FixtureType in('Combo_Hall_Lantern/PI','Hall_PI')
					order by UnitHallFixtureConsoleId desc
				open db_cursor  
				fetch next from db_cursor into @grouphallfixtureconsoleNumber
				while @@FETCH_STATUS = 0  
					begin  
						  declare @sp_result1 int
						  declare @history1 HistoryTable
						  declare @sp_fixtureType nvarchar(50)
						  select @uhfConsoleNo = ConsoleNumber, @sp_fixtureType=FixtureType from UnitHallFixtureConsole where UnitHallFixtureConsoleId=@grouphallfixtureconsoleNumber
						  if(@uhfConsoleNo <> 1)
						  begin
							exec [dbo].[usp_DeleteUnitHallFixtureConsole]@SetId,@uhfConsoleNo,@sp_fixtureType,@history1,@CreatedBy,@sp_result1
						  end
						  fetch next from db_cursor into @grouphallfixtureconsoleNumber
					end 

				close db_cursor  
				deallocate db_cursor
			end
			if(@FixtureType in ('Combo_Hall_Lantern/PI', 'Hall_Lantern'))
			begin
					declare @unitHallFixtureConsoleId1 int
					declare @uhfConsoleNo1 int
					declare @grouphallfixtureconsoleNumber1 int
					select @unitHallFixtureConsoleId1 =UnitHallFixtureConsoleId from UnitHallFixtureConsole where 
					ConsoleNumber = @ConsoleNumber AND SetId = @SetId AND UnitHallFixtureConsole.FixtureType = @FixtureType
					--deselect the front openings which are overwritten
					update UnitHallFixtureLocations 
					set Front = 0 
					where FloorNumber in(select distinct FloorNumber 
										from @UnitHallFixtureLocationVariables
										where Front = 1 ) and
						  UnitHallFixtureConsoleId in(select UnitHallFixtureConsoleId 
											   from UnitHallFixtureConsole 
											   where SetId = @SetId and FixtureType in ('Combo_Hall_Lantern/PI','Hall_Lantern') and UnithallFixtureConsoleId<>@unitHallFixtureConsoleId1)
					--deselect the Rear openings which are overwritten
					update UnitHallFixtureLocations 
					set Rear = 0 
					where FloorNumber in(select distinct FloorNumber 
										from @UnitHallFixtureLocationVariables
										where Rear = 1 ) and
						  UnitHallFixtureConsoleId in(select UnitHallFixtureConsoleId 
											   from UnitHallFixtureConsole 
											   where SetId = @SetId and FixtureType in ('Combo_Hall_Lantern/PI','Hall_Lantern') and UnithallFixtureConsoleId<>@unitHallFixtureConsoleId1)

					declare db_cursor cursor for
					select distinct UnitHallFixtureConsoleId from UnitHallFixtureConsole
					where UnitHallFixtureConsoleId not in(
												select distinct el.UnitHallFixtureConsoleId from UnitHallFixtureLocations el 
												join UnitHallFixtureConsole ec on ec.UnitHallFixtureConsoleId=el.UnitHallFixtureConsoleId
												where (Front=1 or Rear=1) and SetId=@SetId and FixtureType in('Combo_Hall_Lantern/PI','Hall_Lantern')
												group By el.UnitHallFixtureConsoleId
											   )and  SetId=@SetId and FixtureType in('Combo_Hall_Lantern/PI','Hall_Lantern')
					order by UnitHallFixtureConsoleId desc
				open db_cursor  
				fetch next from db_cursor into @grouphallfixtureconsoleNumber1
				while @@FETCH_STATUS = 0  
					begin  
						  declare @sp_result2 int
						  declare @history2 HistoryTable
						  declare @sp_fixtureType2 nvarchar(50)
						  select @uhfConsoleNo1 = ConsoleNumber, @sp_fixtureType2=FixtureType from UnitHallFixtureConsole where UnitHallFixtureConsoleId=@grouphallfixtureconsoleNumber1
						  if(@uhfConsoleNo <> 1)
						  begin
							exec [dbo].[usp_DeleteUnitHallFixtureConsole]@SetId,@uhfConsoleNo1,@sp_fixtureType2,@history2,@CreatedBy,@sp_result2
						  end
						  fetch next from db_cursor into @grouphallfixtureconsoleNumber1
					end 

				close db_cursor  
				deallocate db_cursor
			end
		end
		else
			update UnitHallFixtureLocations 
			set Front = 0 
			where FloorNumber in(select distinct FloorNumber 
								from @UnitHallFixtureLocationVariables
								where Front = 1 ) and
				  UnitHallFixtureConsoleId in(select UnitHallFixtureConsoleId 
									   from UnitHallFixtureConsole 
									   where SetId = @SetId and FixtureType=@FixtureType and ConsoleNumber <> @ConsoleNumber)

			--deselect the Rear openings which are overwritten
			update UnitHallFixtureLocations 
			set Rear = 0 
			where FloorNumber in(select distinct FloorNumber 
								from @UnitHallFixtureLocationVariables
								where Rear = 1 ) and
				  UnitHallFixtureConsoleId in(select UnitHallFixtureConsoleId 
									   from UnitHallFixtureConsole 
									   where SetId = @SetId and FixtureType=@FixtureType and ConsoleNumber <> @ConsoleNumber)


			--delete UnitHallFixture console which doesnt have no openings selected
			declare @uhfConsoleId int
			declare @uhfConsoleNumber int
			declare db_cursor cursor for
				select distinct UnitHallFixtureConsoleId from UnitHallFixtureConsole
				where UnitHallFixtureConsoleId not in(
											select distinct el.UnitHallFixtureConsoleId from UnitHallFixtureLocations el 
											join UnitHallFixtureConsole ec on ec.UnitHallFixtureConsoleId=el.UnitHallFixtureConsoleId
											where (Front=1 or Rear=1) and setId=@SetId  and FixtureType=@FixtureType
											group By el.UnitHallFixtureConsoleId
										   )and SetId=@SetId and FixtureType=@FixtureType
				order by UnitHallFixtureConsoleId desc
			open db_cursor  
			fetch next from db_cursor into @uhfConsoleId
			while @@FETCH_STATUS = 0  
			begin  
				  declare @sp_result int
				  declare @history HistoryTable
				  select @uhfConsoleNumber = ConsoleNumber from UnitHallFixtureConsole where UnitHallFixtureConsoleId=@uhfConsoleId
				  exec [dbo].[usp_DeleteUnitHallFixtureConsole]@SetId,@uhfConsoleNumber,@FixtureType,@history,@CreatedBy,@sp_result
				  fetch next from db_cursor into @uhfConsoleId
			end 

			close db_cursor  
			deallocate db_cursor

			/**HistoryTable**/

			insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select @SetId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
			from @historyTable

			/**HistoryTable**/



		
	if(exists(select StatusKey from Systems where SetId = @SetId))
	  begin
	  -- condition started 
		update Systems
		set StatusKey = @statusKey where SetId = @setId
		select StatusName  SystemStatus,DisplayName,Description from Status where StatusKey = (select distinct StatusKey from Systems where SetId = @SetId)

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
		select StatusName as SystemStatus,DisplayName,Description from Status where StatusKey = @statusKey
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
END
