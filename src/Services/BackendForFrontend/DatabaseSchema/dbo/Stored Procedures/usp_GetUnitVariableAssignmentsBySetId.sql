 
CREATE  Procedure [dbo].[usp_GetUnitVariableAssignmentsBySetId] --1253
 @setId int
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
as
Begin
	BEGIN TRY
	  
	   declare @productCategory nvarchar(200);
	   declare @numOfFloorsServed nvarchar(200);
	   declare @numOfFrontOpenings nvarchar(200);
	   declare @numOfRearOpenings nvarchar(200);
	   declare @netTravelparam nvarchar(200);
	   declare @blandingsParam nvarchar(200);
	   declare @floorToFloorHeight nvarchar(200);
	   SET @productCategory = (Select VariableType from @ConstantMapperList where VariableKey ='PRODUCTCATEGORY')
	   SET @numOfFloorsServed = (Select VariableType from @ConstantMapperList where VariableKey ='NOOFFLOORSSERVED')
	   SET @numOfFrontOpenings = (Select VariableType from @ConstantMapperList where VariableKey ='FRONTOPENINGS')
	   SET @numOfRearOpenings = (Select VariableType from @ConstantMapperList where VariableKey ='REAROPENINGS')
	   SET @netTravelparam = (Select VariableType from @ConstantMapperList where VariableKey ='NETTRAVEL')
	   SET @blandingsParam = (Select VariableType from @ConstantMapperList where VariableKey ='BLANDINGS')
	   SET @floorToFloorHeight = (Select VariableType from @ConstantMapperList where VariableKey ='FLOORTOFLOORHEIGHT')



		--Getting no: floors,frontOpenings,RearOpenings,Net travel
		declare @GroupId int,@nooffloor int, @frontOpening int,@rearOpening int,@netTravel decimal,@productType nvarchar(100)
		select distinct @GroupId=GroupConfigurationId from Units where SetId=@setId
		select distinct @productType= GroupConfigurationValue From GroupConfigurationDetails 
		where GroupConfigurationType like '%'+@productCategory+'%' and GroupId=@GroupId
		select distinct @nooffloor=Count(floornumber) from  OpeningLocation 
		where unitId in(Select UnitId From Units Where SetId=@setId)
		select distinct @frontOpening=FrontOpening from  OpeningLocation 
			where unitId in(Select UnitId From Units Where SetId=@setId)
		select distinct @rearOpening=RearOpening from  OpeningLocation 
			where unitId in(Select UnitId From Units Where SetId=@setId)
		select distinct @netTravel= Travelfeet*12+TravelInch
		from OpeningLocation where UnitId in (select unitId from Units where SetId=@setId)
		IF(@productType='Elevator')
		BEGIN
			If exists(select * from UnitConfiguration where SetId= @setId and ConfigureVariables like '%Parameters.Num_FrontOpenings%')
			Begin
			Update UnitConfiguration Set IsDeleted = 1 where SetId= @setId and ConfigureVariables like '%Parameters.Num_FrontOpenings%'
			end 
			If exists(select * from UnitConfiguration where SetId= @setId and ConfigureVariables like '%Parameters.Num_FloorServed%')
			Begin
			Update UnitConfiguration Set IsDeleted = 1 where SetId= @setId and ConfigureVariables like '%Parameters.Num_FloorServed%'
			end 
			If exists(select * from UnitConfiguration where SetId= @setId and ConfigureVariables like '%Parameters.Num_RearOpenings%')
			Begin
			Update UnitConfiguration Set IsDeleted = 1 where SetId= @setId and ConfigureVariables like '%Parameters.Num_RearOpenings%'
			end 
			If exists(select * from UnitConfiguration where SetId= @setId and ConfigureVariables like '%Parameters.NetTravel%')
			Begin
			Update UnitConfiguration Set IsDeleted = 1 where SetId= @setId and ConfigureVariables like '%Parameters.NetTravel%'
			end 
			Select ConfigureVariables as VariableId
			,ConfigureValues as [Value]
				from UnitConfiguration
			where SetId=@setId and IsDeleted = 0
			union
			Select @numOfFloorsServed as VariableId, Cast(@nooffloor as nvarchar) [Value]
			union
			Select @numOfFrontOpenings as VariableId, Cast(@frontOpening as nvarchar) [Value]
			union
			Select @numOfRearOpenings as VariableId, Cast(@rearOpening as nvarchar) [Value]
			union
			Select @netTravelparam as VariableId, Cast(@netTravel as nvarchar) [Value]
		END
		ELSE
		BEGIN
		  
		   	declare @blandings nvarchar(20);
			SET @blandings = (Select BuindingValue from BuildingConfiguration
			   where BuildingId = (Select distinct BuildingId from GroupConfiguration
			   where GroupId= (Select distinct GroupConfigurationId from units where setid=@setId))
				  and BuindingType=@blandingsParam)

			  declare @buildingRise nvarchar(20);
			SET @buildingRise = (Select BuindingValue from BuildingConfiguration
				where BuildingId = (Select distinct BuildingId from GroupConfiguration
				where GroupId= (Select distinct GroupConfigurationId from units where setid=@setId))
					and BuindingType=@floorToFloorHeight)


			Select ConfigureVariables as VariableId
			,ConfigureValues as [Value]
				from UnitConfiguration 
			where SetId=@setId and IsDeleted = 0
			union all
			Select @numOfFloorsServed as VariableId, Cast(@blandings as nvarchar) [Value]
			union
			Select @numOfFrontOpenings VariableId, Cast(@blandings as nvarchar) [Value]
			union
			Select @numOfRearOpenings VariableId, Cast(@blandings as nvarchar) [Value]
			union
			Select @netTravelparam VariableId, Cast(@buildingRise as nvarchar) [Value]


		END
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