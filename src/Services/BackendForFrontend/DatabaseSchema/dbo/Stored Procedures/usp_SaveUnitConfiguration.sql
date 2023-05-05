

CREATE PROCEDURE [dbo].[usp_SaveUnitConfiguration]
	-- Add the parameters for the stored procedure here
	@SetId int,
	@UnitVariables as unitConfigurationDataTable READONLY,
	@historyTable as HistoryTable readonly,
	@CreatedBy nvarchar(50),
	@IsEditFlows  nvarchar(30),
	@Result int OUTPUT
AS
BEGIN
	BEGIN TRY
		-- Adding unit status flags
		
		/*Changing Hall lantern openings value in unitHallFixtures with respect to changes in Car Riding Lantern Quantity value*/
		declare @CarRidingLanternQuantityOld int
		declare @CarRidingLanternQuantityNew int
		declare @CarRidingLanternOld NVARCHAR(20)
		declare @CarRidingLanternNew NVARCHAR(20)
		declare @rear int
		declare @minUnitId int
		declare @groupConfigurationId int
		declare @fixtureStrategy NVARCHAR(10)

		select @minUnitId=(select min(UnitId) from Units where SetId=@SetId)
		set @groupConfigurationId = (select distinct GroupConfigurationId from units where SetId = @SetId)
		set @rear=0
		set @fixtureStrategy = (select distinct ControlLocationValue from ControlLocation where GroupConfigurationId = @groupConfigurationId 
		and ControlLocationType = 'Parameters_SP.fixtureStrategy_SP')
 
		if(exists(select * from Doors where UnitId=@minUnitId and DoorType like '%rear%'))
		begin
		set @rear=1
		end

		  if (exists(select * from UnitHallFixtureConsole where SetId = @SetId)) and @fixtureStrategy = 'ETA/ETD'
		  begin
				select @CarRidingLanternQuantityOld = cast(ConfigureValues as int) from UnitConfiguration where ConfigureVariables like '%carRidingLantern%' and SetId=@SetId
				set @CarRidingLanternQuantityOld = case when @CarRidingLanternQuantityOld is null then 0 else @CarRidingLanternQuantityOld end
				if(exists(Select * from @UnitVariables where unitjson like '%carRidingLantern%'))
				begin
					select @CarRidingLanternQuantityNew = cast(value as int) from @UnitVariables where unitjson like '%carRidingLantern%' 
					if @CarRidingLanternQuantityOld <> @CarRidingLanternQuantityNew
					begin
						if @CarRidingLanternQuantityNew < 1
						begin
							update UnitHallFixtureLocations set  Front=1 where  UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where FixtureType like '%Hall_Lantern%' and SetId = @SetId) and 
							FloorNumber in (select  distinct FloorNumber from OpeningLocation where Front=1 and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
							if(@rear > 0)
							begin
								update UnitHallFixtureLocations set  Rear=1 where  UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where FixtureType like '%Hall_Lantern%' and SetId = @SetId) and 
							FloorNumber in (select distinct FloorNumber from OpeningLocation where Rear=1 and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
							end
						end
						else if @CarRidingLanternQuantityNew > 0 and @CarRidingLanternQuantityOld < 1
						begin
							update UnitHallFixtureLocations set  Front=0 where  UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where FixtureType like '%Hall_Lantern%' and SetId = @SetId) and 
							FloorNumber in (select  distinct FloorNumber from OpeningLocation where Front=1 and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
							if(@rear > 0)
							begin
								update UnitHallFixtureLocations set  Rear=0 where  UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where FixtureType like '%Hall_Lantern%' and SetId = @SetId) and 
							FloorNumber in (select distinct FloorNumber from OpeningLocation where Rear=1 and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
							end
						end
					end
				end	
				
		  end

		  if(exists(select * from UnitHallFixtureConsole where SetId = @SetId))
		  begin
			if(exists(select * from UnitConfiguration where ConfigureVariables like '%CRLTYPE%' and SetId=@setId))
			begin
				select @CarRidingLanternOld = ConfigureValues from UnitConfiguration where ConfigureVariables like '%CRLTYPE%' and SetId=@setId
			end
			else
			begin
				select @CarRidingLanternOld = ''
			end
			if(exists(Select * from @UnitVariables where unitjson like '%CRLTYPE%'))
			begin
				select @CarRidingLanternNew = value from @UnitVariables where unitjson like '%CRLTYPE%' 
				if @CarRidingLanternOld <> @CarRidingLanternNew and @CarRidingLanternNew='NR'
				begin
					update UnitHallFixtureLocations set  Front=1 where  UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where FixtureType like '%Hall_Lantern%' and SetId = @SetId) and 
					FloorNumber in (select  distinct FloorNumber from OpeningLocation where Front=1 and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
					if(@rear > 0)
					begin
						update UnitHallFixtureLocations set  Rear=1 where  UnitHallFixtureConsoleId in (select UnitHallFixtureConsoleId from UnitHallFixtureConsole where FixtureType like '%Hall_Lantern%' and SetId = @SetId) and 
					FloorNumber in (select distinct FloorNumber from OpeningLocation where Rear=1 and GroupConfigurationId=@groupConfigurationId and IsDeleted=0)
					end
				end
				
			end		
		  end
		

		if(exists(select * from @UnitVariables where UnitJson like '%hoistwayDimensionSelection%' and ([value]='Minimum' or [value]='Maximum'))
		and exists(select * from UnitConfiguration where ConfigureVariables like '%hoistwayDimensionSelection%' and 
		ConfigureValues='Custom' and setId=@SetId))
		begin
			
				delete from UnitConfiguration where setId=@setId and (ConfigureVariables like '%ELEVATOR.Parameters.HWYWID%' or
				ConfigureVariables like '%ELEVATOR.Parameters.PITDEPTH%' or 
				ConfigureVariables like '%ELEVATOR.Parameters.OVHEAD%' or 
				ConfigureVariables like '%ELEVATOR.Parameters.HWYDEP%')
				
			end
		
		



	 
	 
	 
		 declare @statusKey nvarchar(300)
			set @statusKey = 'UNIT_INC'
		--update units set ConflictStatus=@IsEditFlows where SetId=@SetId
		--updating workflow status
		if(@IsEditFlows = 'Valid')
		begin
		-- setting the unit flag by compare the pervious work flow status
			set @IsEditFlows ='UNIT_VAL'
			declare @perviousStatus nvarchar(100)
			set @perviousStatus = (select distinct WorkflowStatus from Units where SetId = @SetId)
			if(@perviousStatus = @IsEditFlows)
			begin
			-- setting the unit flag by compare the pervious work flow status
			update units set WorkflowStatus = 'UNIT_COM' where SetId =@SetId
			set @statusKey = 'UNIT_COM'
			end
		end
		else if(@IsEditFlows = 'InValid')
		begin
		-- setting the unit flag work flow status
			set @IsEditFlows ='UNIT_CINV'
			update units set WorkflowStatus = @IsEditFlows where SetId =@SetId
		end
		

		--update units set WorkflowStatus = @IsEditFlows where SetId =@SetId
		
		if(exists (select * from UnitConfiguration where SetId=@SetId))
		begin
			--update UnitConfiguration set  ConfigureVariables = (select UnitJson from @UnitVariables), 
			--ConfigureValues = (Select value from @UnitVariables), ConfigureJson = (Select UnitJsonData from @UnitVariables), ModifiedBy = @CreatedBy, 
			--ModifiedOn=GETDATE() where SetId=@SetId
			--inserting data to the history table
			if(exists(select * from @UnitVariables t join UnitConfiguration g 
			on t.UnitJson=g.ConfigureVariables and t.value<>g.ConfigureValues
			where g.SetId = @SetId))
			begin
				insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
				Select @SetId,t.UnitJson, t.value,g.ConfigureValues,@CreatedBy,GETDATE(),@CreatedBy,GETDATE()
				from @UnitVariables t join UnitConfiguration g 
				on t.UnitJson=g.ConfigureVariables and t.value<>g.ConfigureValues
				where g.SetId = @SetId
			end

			if(exists(Select * from @UnitVariables where unitjson not in(select ConfigureVariables from UnitConfiguration where SetId=@SetId)))
			begin
				insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
				select @SetId,UnitJson,value,'',@CreatedBy,getdate(),@CreatedBy,getdate()
				from @UnitVariables where unitjson not in(select ConfigureVariables from UnitConfiguration where SetId=@SetId)
			end

		

			--update already saved variables
			update UnitConfiguration set ConfigureValues=t.value,ConfigureJson=t.UnitJsonData,modifiedby=@CreatedBy,ModifiedOn=getdate()
			from @UnitVariables t join UnitConfiguration g on t.UnitJson=g.ConfigureVariables
			where g.SetId = @SetId

			--insert new variable assignments

			insert into UnitConfiguration(SetId,ConfigureVariables,ConfigureValues,ConfigureJson, CreatedBy,CreatedOn)
			select @SetId,UnitJson,value,UnitJsonData,@CreatedBy,getdate()
			from @UnitVariables where unitjson not in(select ConfigureVariables from UnitConfiguration where SetId=@SetId)


			set @Result=@SetId



		end
		else
		begin
			insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select @SetId,UnitJson,value,'',@CreatedBy,getdate(),@CreatedBy,getdate()
			from @UnitVariables 

			insert into UnitConfiguration(SetId,ConfigureVariables,ConfigureValues,ConfigureJson, CreatedBy,CreatedOn)
			select @SetId,UnitJson,value,UnitJsonData,@CreatedBy,getdate() from @UnitVariables
			set @Result=@SetId
		end
	

		---Deleting values in CarCallCutoutLocations ---- 

		if(EXISTS(select * from UnitConfiguration where ConfigureVariables like '%ELEVATOR.Parameters.LOCKREG%' and SetId = @SetId and ConfigureValues = 'NR'))
		begin
			/**HistoryTable**/

			insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select @SetId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
			from @historyTable

			/**HistoryTable**/
		  delete from CarCallCutoutLocations where SetId=@setId
		end

	if(exists(select StatusKey from Systems where SetId = @setId))
	  begin
	  -- condition started 
		update Systems
		set StatusKey = @statusKey where SetId = @setId
		--select StatusName  SystemStatus from Status where StatusKey = (select StatusKey from Systems where SetId = @setId)

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

		insert into Systems (SetId,StatusKey,CreatedBy,ModifiedBy)
		values(@setId,@statusKey,@CreatedBy,@CreatedBy)	

		--

		--select StatusName as SystemStatus from Status where StatusKey = @statusKey
	  end

	  declare @groupId int
	  select distinct @groupId= GroupConfigurationId from Units where SetId=@SetId
	  exec [dbo].[usp_UpdateWorkflowStatus]@groupId,'group'
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
