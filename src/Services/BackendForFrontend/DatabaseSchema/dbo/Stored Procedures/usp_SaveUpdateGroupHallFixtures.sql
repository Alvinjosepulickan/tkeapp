
create PROCEDURE [dbo].[usp_SaveUpdateGroupHallFixtures]
	@GroupId int,
	@ConsoleNumber int,
	@GroupHallFixtureConsoleVariables as UnitHallFixtureConsoleInfoDataTable READONLY,
	@GroupHallFixtureConfigurationVariables as EntranceConfigurationDataTable READONLY,
	@GroupHallFixtureLocationVariables as GroupHallFixtureLocationDataTable READONLY,
	@historyTable as HistoryTable readonly,
	@CreatedBy nvarchar(50),
	@Result int OUTPUT
as

Begin
	BEGIN TRY
		declare @groupHallFixtureConsoleId int
		set @groupHallFixtureConsoleId=(select GroupHallFixtureConsoleId from GroupHallFixtureConsole where GroupId=@GroupId and Name in (select ConsoleName from  @GroupHallFixtureConsoleVariables));
		declare @FixtureStrategy nvarchar(100);
		set @FixtureStrategy=(select ControlLocationValue from ControlLocation where GroupConfigurationId = @GroupId and ControlLocationType = 'Parameters_SP.fixtureStrategy_SP');
   
		declare @FixtureType nvarchar(100);
		set @FixtureType= (select FixtureType from @GroupHallFixtureConsoleVariables)

		---Check At least one hall station is mandatory for Traditional and Agile Hall stations each.
		if(@FixtureStrategy='ETA/ETD')
		begin
			declare @grphallfixturelocation GroupHallFixtureLocationDataTable
		
			if( @FixtureType in('Traditional_Hall_Stations'))
			begin
				insert into @grphallfixturelocation
				select GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,HallStationName 
				from GroupHallFixtureLocations where GroupHallFixtureConsoleId 
				in(select GroupHallFixtureConsoleId from GroupHallFixtureConsole 
				where GroupId=@GroupId and FixtureType in ('AGILE_Hall_Stations'))
			end
			else if(@FixtureType in('AGILE_Hall_Stations'))
			begin
				insert into @grphallfixturelocation
				select GroupHallFixtureConsoleId,UnitId,FloorDesignation,Front,Rear,HallStationName
				from GroupHallFixtureLocations where GroupHallFixtureConsoleId 
				in(select GroupHallFixtureConsoleId from GroupHallFixtureConsole 
				where GroupId=@GroupId and FixtureType in ('Traditional_Hall_Stations'))
			end
		
			if( @FixtureType in('Traditional_Hall_Stations','AGILE_Hall_Stations'))
			begin
			--deselect the front openings which are overwritten
			update @grphallfixturelocation 
			set Front = 0
			from 
			GroupHallFixtureConsole gc inner join
			@grphallfixturelocation gl on gc.[GroupHallFixtureConsoleId]=gl.GroupHallFixtureConsoleId
			inner join  @GroupHallFixtureLocationVariables gv
			on gl.FloorDesignation=gv.FloorDesignation and gl.HallStationName=gv.HallStationName
			where gv.Front=1 and gl.Front=1 
			--deselect the Rear openings which are overwritten
			update @grphallfixturelocation 
			set Rear = 0
			from 
			GroupHallFixtureConsole gc inner join
			@grphallfixturelocation gl on gc.[GroupHallFixtureConsoleId]=gl.GroupHallFixtureConsoleId
			inner join  @GroupHallFixtureLocationVariables gv
			on gl.FloorDesignation=gv.FloorDesignation and gl.HallStationName=gv.HallStationName
			where gv.Rear=1 and gl.Rear=1

			---To Check At least one opening is selected for Traditional and Agile HallStations
			declare @countEmptyConsole int
		
			select @countEmptyConsole= isnull(count(distinct el.HallStationName),0) from @grphallfixturelocation el 
									 where el.HallStationName != '' 
										group by el.hallstationname having sum(cast(el.Rear as INT))= 0 and sum(cast(el.Front as INT))=0 
			
										--isnull(count(distinct el.GroupHallFixtureConsoleId),0) from @grphallfixturelocation el 
										--join GroupHallFixtureConsole ec on ec.GroupHallFixtureConsoleId=el.GroupHallFixtureConsoleId
										--where (Front=1 or Rear=1) 
			if(@countEmptyConsole>0)
			begin
				set @Result=-1
				Return 0
			end
			end
		end

		
		---To Check At least one opening is selected for all Fixtures---
        if( @FixtureType in('Fire_Service','Emergency_Power','Appendix_H/O_Signage','Code_Blue','VIP_Service_Switch','Medical_Emergency',
					 'Elevator_Communication_Failure_Jewel','Fire_Key_Box','Emergency_Key_Box','Fire_Phone_Jack','Card_Reader_Provision',
					 'Hall_Call_Lockout','Hall_Call_Registration'))
        begin       
            declare @selectedOpeningCountFront int   
            select @selectedOpeningCountFront= isnull(count(distinct el.UnitId),0) from @GroupHallFixtureLocationVariables el    
                                        group by el.UnitId having  sum(cast(el.Front as INT))=0 and sum(cast(el.Rear as INT))=0

            if(@selectedOpeningCountFront>0)
            begin
                set @Result=-2
                Return 0
            end
        end

		---To Check At least one opening is selected for Traditional_Hall_Stations and AGILE_Hall_Stations---
		if( @FixtureType in('Traditional_Hall_Stations','AGILE_Hall_Stations'))
        begin       
            declare @selectedOpeningCount int   
            select @selectedOpeningCount= isnull(count(distinct el.HallStationName),0) from @GroupHallFixtureLocationVariables el    
                                        group by el.HallStationName having  sum(cast(el.Front as INT))=0 and sum(cast(el.Rear as INT))=0
            
            if(@selectedOpeningCount>0)
            begin
                set @Result=-2
                Return 0
            end
        end

		IF(EXISTS (SELECT * FROM GroupHallFixtureConsole gc WHERE gc.ConsoleNumber=@ConsoleNumber 
		AND gc.GroupId=@GroupId and gc.Name in (select ConsoleName from  @GroupHallFixtureConsoleVariables)))
		BEGIN

		/****** GroupHallFixture Console******/

			UPDATE GroupHallFixtureConsole 
			  SET Name = gcv.ConsoleName
				 ,IsController= gcv.IsController
				 ,FixtureType=gcv.FixtureType
				 ,ModifiedBy =@CreatedBy 
				 ,ModifiedOn = GETDATE()
			 
				FROM @GroupHallFixtureConsoleVariables gcv  
					WHERE ConsoleNumber = @ConsoleNumber --AND GroupHallFixtureConsole.FixtureType = @FixtureType 
														 AND GroupHallFixtureConsole.GroupId = @GroupId and Name in(select ConsoleName from  @GroupHallFixtureConsoleVariables)


		--/****** GroupHallFixture Configuration******/

			DELETE FROM GroupHallFixtureConfiguration 
			   WHERE GroupHallFixtureConsoleId =@groupHallFixtureConsoleId

			INSERT INTO GroupHallFixtureConfiguration
			   (
				 GroupHallFixtureConsoleId
				,VariableType
				,VariableValue
				,CreatedBy
				,CreatedOn
				,ModifiedBy
				,ModifiedOn
				)

			 SELECT @groupHallFixtureConsoleId
					,VariableType
					,VariableValue
					,@CreatedBy
					,gc.CreatedOn
					,@CreatedBy
					,GETDATE()
				FROM @GroupHallFixtureConfigurationVariables gcv 
					LEFT JOIN GroupHallFixtureConsole gc on gcv.EntranceConsoleId = gc.ConsoleNumber and gc.GroupId=@GroupId and gc.Name in ((select ConsoleName from  @GroupHallFixtureConsoleVariables ))
						WHERE gcv.EntranceConsoleId = @ConsoleNumber --AND FixtureType =@FixtureType 
						 AND gc.GroupId = @GroupId                 --AND gc.SetId = @SetId
	
		--/****** GroupHallFixture Location******/

		if( @FixtureType in('Traditional_Hall_Stations','AGILE_Hall_Stations'))
		begin

			delete from GroupHallFixtureLocations where GroupHallFixtureConsoleId =@groupHallFixtureConsoleId

			INSERT INTO GroupHallFixtureLocations
			  (
				GroupHallFixtureConsoleId
				,UnitId
			   ,FloorDesignation
			   ,Front
			   ,Rear
			   ,HallStationName
			   ,CreatedBy
			   ,CreatedOn
			   ,ModifiedBy
			   ,ModifiedOn
			   )

			 SELECT @groupHallFixtureConsoleId
					,glv.UnitId
				   ,glv.[FloorDesignation]
				   ,glv.Front
				   ,glv.Rear
				   ,glv.HallStationName
				   ,@CreatedBy
				   ,gc.CreatedOn
				   ,@CreatedBy
				   ,GETDATE()
				FROM @GroupHallFixtureLocationVariables glv 
				  LEFT JOIN GroupHallFixtureConsole gc ON glv.[GroupHallFixtureConsoleId] = gc.ConsoleNumber and gc.GroupId=@GroupId and gc.Name=((select ConsoleName from  @GroupHallFixtureConsoleVariables ))
					 WHERE glv.[GroupHallFixtureConsoleId] = @ConsoleNumber --AND FixtureType = @FixtureType
					  AND gc.GroupId = @GroupId
		end
		else
		begin
			DELETE FROM GroupHallFixtureLocations 
			   WHERE GroupHallFixtureConsoleId =@groupHallFixtureConsoleId


			INSERT INTO GroupHallFixtureLocations
			  (
				GroupHallFixtureConsoleId
				,UnitId
			   ,FloorDesignation
			   ,Front
			   ,Rear
			   ,CreatedBy
			   ,CreatedOn
			   ,ModifiedBy
			   ,ModifiedOn
			   )

			 SELECT @groupHallFixtureConsoleId
					,glv.UnitId
				   ,glv.[FloorDesignation]
				   ,glv.Front
				   ,glv.Rear
				   ,@CreatedBy
				   ,gc.CreatedOn
				   ,@CreatedBy
				   ,GETDATE()
				FROM @GroupHallFixtureLocationVariables glv 
				  LEFT JOIN GroupHallFixtureConsole gc ON glv.[GroupHallFixtureConsoleId] = gc.ConsoleNumber and gc.GroupId=@GroupId and gc.Name=((select ConsoleName from  @GroupHallFixtureConsoleVariables ))
					 WHERE glv.[GroupHallFixtureConsoleId] = @ConsoleNumber --AND FixtureType = @FixtureType
					  AND gc.GroupId = @GroupId                  -- AND gc.SetId = @SetId
		end
				  
			SET  @Result= @GroupId
		END
		ELSE
		BEGIN
	   
			 ---------------------------Insert in GroupHallFixtureConsole-------------------------------

			DECLARE @ConsoleNumberCheck int
			select @ConsoleNumberCheck = ConsoleId from @GroupHallFixtureConsoleVariables

			INSERT INTO GroupHallFixtureConsole
			 (
			  ConsoleNumber
			 ,Name
			 ,FixtureType
			 ,IsController
			 ,GroupId
			 ,CreatedBy
			 ,CreatedOn
			 )
			SELECT ConsoleId
				  ,ConsoleName
				  ,FixtureType
				  ,IsController
				  ,@GroupId
				  ,@CreatedBy
				  ,getdate() 
				FROM @GroupHallFixtureConsoleVariables  

			set @groupHallFixtureConsoleId=scope_identity()

			if(@ConsoleNumberCheck=2)
			begin
				update GroupHallFixtureConsole set Name = concat(REPLACE(@FixtureType,'_',' '),' ',1) where ConsoleNumber=1 and FixtureType=@FixtureType
				and GroupId=@GroupId
			end

			 ---------Updating IsController property for delete functionality-----------
			if(exists(select * from GroupHallFixtureConsole where FixtureType=@FixtureType and GroupId=@GroupId and IsController=1))
			begin
				update GroupHallFixtureConsole set isController=0 where FixtureType=@FixtureType and GroupId=@GroupId and IsController=1
			end
	
			 ---------------------------Insert in GroupHallFixtureConfiguration-----------------------------------------------

			INSERT INTO GroupHallFixtureConfiguration
			 (
			   GroupHallFixtureConsoleId
			  ,VariableType
			  ,VariableValue
			  ,CreatedBy
			  ,CreatedOn
			  )
			 SELECT @groupHallFixtureConsoleId
				   ,VariableType
				   ,VariableValue
				   ,@CreatedBy
				   ,getdate()  
			   FROM @GroupHallFixtureConfigurationVariables gcv
		   
			
	    		INSERT INTO GroupHallFixtureLocations
				 (
				   GroupHallFixtureConsoleId
				   ,UnitId
				  ,FloorDesignation
				  ,Front
				  ,Rear
				  ,CreatedBy
				  ,CreatedOn
				  ,HallStationName
				 )
			 SELECT @groupHallFixtureConsoleId
					,UnitId
				   ,[FloorDesignation]
				   ,Front
				   ,Rear
				   ,@CreatedBy
				   ,getdate()
				   ,HallStationName
				FROM @GroupHallFixtureLocationVariables glv
			  
			SET @Result = @GroupId
		END
		declare @grouphallfixtureconsoleNumber int
		if(@FixtureStrategy='ETA/ETD' and @FixtureType in('Traditional_Hall_Stations','AGILE_Hall_Stations'))
		begin
			--deselect the front openings which are overwritten
			update GroupHallFixtureLocations 
			set Front = 0
			from 
			GroupHallFixtureConsole gc inner join
			GroupHallFixtureLocations gl on gc.[GroupHallFixtureConsoleId]=gl.GroupHallFixtureConsoleId
			inner join  @GroupHallFixtureLocationVariables gv
			on gl.FloorDesignation=gv.FloorDesignation and gl.HallStationName=gv.HallStationName
			where gc.GroupHallFixtureConsoleId <> @groupHallFixtureConsoleId and gv.Front=1 and gl.Front=1
			and gc.GroupId=@GroupId and gc.FixtureType in ('Traditional_Hall_Stations','AGILE_Hall_Stations')
			--deselect the Rear openings which are overwritten
			update GroupHallFixtureLocations 
			set Rear = 0
			from 
			GroupHallFixtureConsole gc inner join
			GroupHallFixtureLocations gl on gc.[GroupHallFixtureConsoleId]=gl.GroupHallFixtureConsoleId
			inner join  @GroupHallFixtureLocationVariables gv
			on gl.FloorDesignation=gv.FloorDesignation and gl.HallStationName=gv.HallStationName
			where gc.GroupHallFixtureConsoleId <> @groupHallFixtureConsoleId and gv.Rear=1 and gl.Rear=1
			and gc.GroupId=@GroupId and gc.FixtureType in ('Traditional_Hall_Stations','AGILE_Hall_Stations')

			declare db_cursor cursor for
			select distinct GroupHallFixtureConsoleId from GroupHallFixtureConsole
			where GroupHallFixtureConsoleId not in(
										select distinct el.GroupHallFixtureConsoleId from GroupHallFixtureLocations el 
										join GroupHallFixtureConsole ec on ec.GroupHallFixtureConsoleId=el.GroupHallFixtureConsoleId
										where (Front=1 or Rear=1) and GroupId=@GroupId and FixtureType in('Traditional_Hall_Stations','AGILE_Hall_Stations')
										group By el.GroupHallFixtureConsoleId
									   )and  GroupId=@GroupId and FixtureType in('Traditional_Hall_Stations','AGILE_Hall_Stations')
			order by GroupHallFixtureConsoleId desc
		open db_cursor  
		fetch next from db_cursor into @grouphallfixtureconsoleNumber
		while @@FETCH_STATUS = 0  
			begin  
				  declare @sp_result int
				  declare @sp_consoleNumber int
				  declare @sp_fixtureType nvarchar(50)
				  select @sp_consoleNumber=ConsoleNumber,@sp_fixtureType=FixtureType
				  from GroupHallFixtureConsole where GroupHallFixtureConsoleId=@grouphallfixtureconsoleNumber
				  exec [dbo].[usp_DeleteGroupHallFixtureConsole]@GroupId,@sp_consoleNumber,@sp_fixtureType,@historyTable,@createdBy,@sp_result
				  fetch next from db_cursor into @grouphallfixtureconsoleNumber
			end 

		close db_cursor  
		deallocate db_cursor
		end
		else
		begin
			--deselect the front openings which are overwritten
			if(@FixtureType in('Traditional_Hall_Stations','AGILE_Hall_Stations'))
				begin
					update GroupHallFixtureLocations 
					set Front = 0
					from 
					GroupHallFixtureConsole gc inner join
					GroupHallFixtureLocations gl on gc.[GroupHallFixtureConsoleId]=gl.GroupHallFixtureConsoleId
					inner join  @GroupHallFixtureLocationVariables gv
					on gl.FloorDesignation=gv.FloorDesignation and gl.HallStationName=gv.HallStationName
					where ConsoleNumber <> @ConsoleNumber and gv.Front=1 and gl.Front=1
					and gc.GroupId=@GroupId and gc.FixtureType=@FixtureType
					--deselect the Rear openings which are overwritten
					update GroupHallFixtureLocations 
					set Rear = 0
					from 
					GroupHallFixtureConsole gc inner join
					GroupHallFixtureLocations gl on gc.[GroupHallFixtureConsoleId]=gl.GroupHallFixtureConsoleId
					inner join  @GroupHallFixtureLocationVariables gv
					on gl.FloorDesignation=gv.FloorDesignation and gl.HallStationName=gv.HallStationName
					where ConsoleNumber <> @ConsoleNumber and gv.Rear=1 and gl.Rear=1
					and gc.GroupId=@GroupId and gc.FixtureType=@FixtureType
				end
			else
				begin
					update GroupHallFixtureLocations 
			set Front = 0
			from 
			GroupHallFixtureConsole gc inner join
			GroupHallFixtureLocations gl on gc.[GroupHallFixtureConsoleId]=gl.GroupHallFixtureConsoleId
			inner join  @GroupHallFixtureLocationVariables gv
			on gl.FloorDesignation=gv.FloorDesignation and gl.UnitId=gv.UnitId
			where ConsoleNumber <> @ConsoleNumber and gv.Front=1 and gl.Front=1
			and gc.GroupId=@GroupId and gc.FixtureType=@FixtureType
					--deselect the Rear openings which are overwritten
					update GroupHallFixtureLocations 
			set Rear = 0
			from 
			GroupHallFixtureConsole gc inner join
			GroupHallFixtureLocations gl on gc.[GroupHallFixtureConsoleId]=gl.GroupHallFixtureConsoleId
			inner join  @GroupHallFixtureLocationVariables gv
			on gl.FloorDesignation=gv.FloorDesignation and gl.UnitId=gv.UnitId
			where ConsoleNumber <> @ConsoleNumber and gv.Rear=1 and gl.Rear=1
			and gc.GroupId=@GroupId and gc.FixtureType=@FixtureType
				end
			--delete Entrance console which doesnt have no openings selected
			declare db_cursor cursor for
			select distinct consolenumber from GroupHallFixtureConsole
			where GroupHallFixtureConsoleId not in(
										select distinct el.GroupHallFixtureConsoleId from GroupHallFixtureLocations el 
										join GroupHallFixtureConsole ec on ec.GroupHallFixtureConsoleId=el.GroupHallFixtureConsoleId
										where (Front=1 or Rear=1) and GroupId=@GroupId and FixtureType=@FixtureType
										group By el.GroupHallFixtureConsoleId
									   )and  GroupId=@GroupId and FixtureType=@FixtureType
			order by consolenumber desc
		open db_cursor  
		fetch next from db_cursor into @grouphallfixtureconsoleNumber
		while @@FETCH_STATUS = 0  
			begin  
				  declare @sp_result1 int
				  exec [dbo].[usp_DeleteGroupHallFixtureConsole]@GroupId,@grouphallfixtureconsoleNumber,@FixtureType,@historyTable,@createdBy,@sp_result1
				  fetch next from db_cursor into @grouphallfixtureconsoleNumber
			end 

		close db_cursor  
		deallocate db_cursor

	

		end
	
		/**HistoryTable**/

			insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select @GroupId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
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



	
