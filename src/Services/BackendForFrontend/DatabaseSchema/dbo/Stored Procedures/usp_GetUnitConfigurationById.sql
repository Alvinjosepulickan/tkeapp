



CREATE Procedure [dbo].[usp_GetUnitConfigurationById]
-- Add the parameters for the stored procedure here
 @groupConfigurationId int
,@setId nvarchar(500)
as
Begin
	BEGIN TRY
		Declare @count int
		Declare @fixtureStrategy nvarchar(50)
    
		SET @count = (SELECT CASE WHEN (SELECT top 1 DoorType FROM [dbo].[Doors]
		  WHERE GroupConfigurationId=@groupConfigurationId and UnitId in(select distinct UnitId from Units where SetId=@setId) and DoorType like '%Parameters_SP.rearDoorTypeAndHand_SP%') is null
			THEN 0 ELSE 1 END)

		SET @fixtureStrategy = (SELECT distinct(ControlLocationValue) FROM ControlLocation WHERE GroupConfigurationId = @groupConfigurationId 
		and ControlLocationType = 'Parameters_SP.fixtureStrategy_SP')

		IF(@count =1)
		Begin
		   Select uc.ConfigureVariables,uc.ConfigureValues from UnitConfiguration uc
			 Left Join Units us on us.SetId = uc.SetId
			 Left Join UnitSet ust on us.SetId = ust.SetId
				where uc.IsDeleted=0 and us.IsDeleted=0 and ust.IsDeleted=0 
					and us.GroupConfigurationId=@groupConfigurationId and us.SetId=@setId
					Union all
			 Select 'ELEVATOR.Parameters.REAROPEN', 'TRUE'
					union all
			select 'ELEVATOR.Parameters_SP.fixtureStrategy_SP', @fixtureStrategy

		End
		ELSE
		Begin
			Select  uc.ConfigureVariables,uc.ConfigureValues from UnitConfiguration uc
			 Left Join Units us on us.SetId = uc.SetId
			 Left Join UnitSet ust on us.SetId = ust.SetId
				where uc.IsDeleted=0 and us.IsDeleted=0 and ust.IsDeleted=0 
					and us.GroupConfigurationId=@groupConfigurationId and us.SetId=@setId
					UNION all
				Select 'ELEVATOR.Parameters_SP.fixtureStrategy_SP', @fixtureStrategy
		End


		Select uc.ConfigureVariables,uc.ConfigureValues from UnitConfiguration uc
			 Left Join Units us on us.SetId = uc.SetId
			 Left Join UnitSet ust on us.SetId = ust.SetId
				where uc.IsDeleted=0 and us.IsDeleted=0 and ust.IsDeleted=0 
					and us.GroupConfigurationId=@groupConfigurationId and us.SetId=@setId

       

		--Select ConfigureVariables,ConfigureValues from UnitConfiguration
		--       Where SetId=13 and ConfigureVariables like '%COPAUX%'


			 Select UnitId as unitid ,[Designation] as unitname,UEID AS ueid  from Units  
				 Where IsDeleted=0 and SetId=@setId and GroupConfigurationId=@groupConfigurationId

	 Select UnitId as unitid ,[Designation] as unitname,UEID AS ueid  from Units  
				 Where IsDeleted=0 and SetId=@setId and GroupConfigurationId=@groupConfigurationId
	declare @buildingId int
		set @buildingId=(select distinct BuildingId from GroupConfiguration where GroupId=@groupConfigurationId)
		declare @numberofFloors int
		set @numberofFloors=(select count(*) from BuildingElevation where BuildingId=@buildingId)
		declare @numberOfFloorsinopeninglocation int
		set @numberOfFloorsinopeninglocation=(select count(*) from OpeningLocation where GroupConfigurationId=@groupConfigurationId)
		declare @numberofUnits int
		set @numberofUnits=(select count(*) from units where GroupConfigurationId=@groupConfigurationId)
		set @numberOfFloorsinopeninglocation=@numberOfFloorsinopeninglocation/@numberofUnits
		if(@numberofFloors<>@numberOfFloorsinopeninglocation and @numberOfFloorsinopeninglocation>0)
		begin
			select @numberofFloors as TotalNumberOfFloors
		end
		else
		begin
			select 0 as TotalNumberOfFloors
		end

		select FrontOpening as FrontOpening, RearOpening as RearOpening from OpeningLocation where GroupConfigurationId=@groupConfigurationId
		
			 if(exists(select StatusKey from Systems where SetId = @setId))
	  begin
	  -- condition started 
		--update Systems
		--set StatusKey = @statusKey where SetId = @setId
		select StatusName  SystemStatus,DisplayName , Description from Status where StatusKey = (select distinct WorkflowStatus from Units where SetId = @setId)

		  select Systems.SetId,Systems.SystemValidKeys,SystemsMasterValues.SystemsDescriptionKeys,SystemsMasterValues.SystemsDescriptionValues,
			  Status.StatusName,Systems.CreatedBy, Status.DisplayName,Status.Description from Systems
			  inner join SystemsMasterValues on Systems.SystemValidValues = SystemsMasterValues.SystemsDescriptionKeys
			  inner join Status on Systems.StatusKey = Status.StatusKey
			  where Systems.SetId = @setId
		--select  from Systems where SetId = @setId

	  -- if condition ended 
	  end
	  else
	  begin
  
		 declare @statusKey nvarchar(300)
			set @statusKey = 'UNIT_INC'
		insert into Systems (SetId,StatusKey)
		values(@setId,@statusKey)	
		select StatusName as SystemStatus, DisplayName , Description from Status where StatusKey = @statusKey
	  end
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



 

