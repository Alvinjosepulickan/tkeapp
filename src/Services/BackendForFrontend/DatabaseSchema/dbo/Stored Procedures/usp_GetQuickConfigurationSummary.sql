

CREATE PROCEDURE [dbo].[usp_GetQuickConfigurationSummary] --'',0,0,1331
 @QuoteId nvarchar(25)
 ,@buildingConfigurationId int = 0
 ,@groupConfigurationId int =0
 ,@setId int=0
AS
BEGIN
	BEGIN TRY
		--declare @QuoteId nvarchar(20)
		--Select @QuoteId = (Select QuoteId from Quotes where OpportunityId = @OpportunityId and VersionId = @VersionId)


		  IF(@QuoteId <> '')
		   BEGIN
		   select 
   					   distinct(bg.QuoteId)
					  ,(select count(*) from building where QuoteId=@QuoteId and IsDeleted=0) as [NumberOfBuildings]
					  from Building bg
				   Where bg.QuoteId=@QuoteId
		   END

		   ELSE IF(@buildingConfigurationId > 0)
		   BEGIN
			  Select  bg.Id as Id
					 ,bg.BldName as BuildingName
					 ,bg.QuoteId
					 ,(Select Count(*) from GroupConfiguration 
							where IsDeleted=0 and BuildingId in (select distinct(BuildingId) from GroupConfiguration 
									where BuildingId=@buildingConfigurationId)) as [NumberOfGroups]
					 ,(select Count(*) from building 
						where IsDeleted=0 and QuoteId in (select QuoteId from Building 
							where Id=@buildingConfigurationId)) as [NumberOfBuildings]
					  from Building bg
				   Where bg.Id = @buildingConfigurationId
		   END

		   ELSE IF(@groupConfigurationId > 0)
		   BEGIN
			  Select bg.Id as Id
					,bg.BldName as BuildingName
					,(select Count(QuoteId) from building 
						where IsDeleted=0 and QuoteId= (select distinct QuoteId from Building 
							where Id= (select BuildingId from GroupConfiguration where GroupId=@groupConfigurationId))) as [NumberOfBuildings]
					,bg.QuoteId 
					,(Select Count(*) from GroupConfiguration 
						where IsDeleted=0 and BuildingId in (select distinct(BuildingId) from GroupConfiguration gc 
							where gc.GroupId=@groupConfigurationId)) AS [NumberOfGroups]
					,gc.GroupId 
					,gc.GroupName
					,(select count(*) from Units 
						where IsDeleted=0 and GroupConfigurationId in (select distinct(GroupconfigurationId) from Units 
							where GroupConfigurationId=@groupConfigurationId)) as [NumberOfUnits]
					 from Building bg
				 Left Join GroupConfiguration gc on bg.Id = gc.BuildingId
				   Where gc.GroupId = @groupConfigurationId
		   END

		   ELSE IF(@setId > 0)
		   BEGIN
			DECLARE @grpId int
			DECLARE @UnitStatus nvarchar(30)
			DECLARE @StatusCheck nvarchar(30)
		   set @grpId = (select distinct GroupConfigurationId from Units us where us.setId=@setId)
		   set @UnitStatus = (select distinct WorkflowStatus from Units where SetId=@setId)
		   set @StatusCheck = case when(select Step from Status where StatusKey = @UnitStatus )>=(select Step from Status where StatusKey = 'UNIT_VAL') 
		   then 'UNITVALID' else 'UNITNOTVALID' end

				--select * from 
				select bg.Id as Id
					,bg.BldName as BuildingName
					,(select Count(QuoteId) from building 
						where IsDeleted=0 and QuoteId= (select distinct QuoteId from Building 
							where Id= (select distinct BuildingId from GroupConfiguration 
								where GroupId= (select MAX(GroupConfigurationId) from Units us where us.setId=@setId)))) as [NumberOfBuildings]
					,bg.QuoteId
					,(Select Count(*) from GroupConfiguration 
						where IsDeleted=0 and GroupId in (select distinct GroupConfigurationId from Units us where us.SetId=@setId)) as NumberOfGroups
					,us.GroupConfigurationId as GroupId
					,gc.GroupName
					,(select count(*) from Units 
						where IsDeleted=0 and GroupConfigurationId in (select distinct GroupconfigurationId from Units us where us.SetId=@setId)) as NumberOfUnits
					,us.UEID
					,us.UnitId
					,us.SetId
					,us.[Designation] as UnitName
					,ust.ProductName as Model
					,Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](@setId,'CAPACITY'), '') AS Capacity
					,Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](@setId,'SPEED'), '') AS Speed
					,Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](@setId,'HWYWID'), '') AS Width
					,Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](@setId,'HWYDEP'), '') AS Depth
					,Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](@setId,'PITDEPTH'), '') AS PitDepth
					,Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](@setId,'OVHEAD'), '') AS Overhead
					,(CASE WHEN ust.ProductName <> 'EVO_100' 
								THEN Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](@setId,'hoistwayDimensionSelection_SP'), '') 
						   WHEN ust.ProductName = 'EVO_100' THEN 'NoHoistwayDimensions' END) AS DimensionSelection
					--,Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](@setId,'hoistwayDimensionSelection_SP'), '') AS DimensionSelection
					--,Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](@setId,'MAXRAILBRACSPACING'), '') AS MaximumRailbracketSpacing
					,'' AS [Status]
					,us.[Designation] as [UnitDesignation]
					,us.MappedLocation as [DisplayCarPosition]
					,(CASE WHEN us.SetId=@setId THEN 1
											 WHEN us.SetId<>@setId THEN 0 END
											) as UnitCurrentlyConfigured
					,Isnull( [dbo].[Fn_GetDoorVariableFromDoors](us.UnitId,'frontDoorTypeAndHand_SP'), '') AS Front 
					,Isnull( [dbo].[Fn_GetDoorVariableFromDoors](us.UnitId,'rearDoorTypeAndHand_SP'), '') AS Rear
					,@StatusCheck as StatusCheck
					,(CASE WHEN @StatusCheck = 'UNITVALID' 
								THEN Isnull([dbo].[Fn_GetSystemValidationForQuickSummaryScreen](@setId,'MACHFAM'), '') 
						   WHEN @StatusCheck = 'UNITNOTVALID' THEN '' END) AS MachineType

					,(CASE WHEN @StatusCheck = 'UNITVALID' 
								THEN Isnull([dbo].[Fn_GetSystemValidationForQuickSummaryScreen](@setId,'MACHPN'), '') 
						   WHEN @StatusCheck = 'UNITNOTVALID' THEN '' END) AS MotorTypeSize 
					,(CASE WHEN @StatusCheck = 'UNITVALID' 
								THEN Isnull([dbo].[Fn_GetSystemValidationForQuickSummaryScreen](@setId,'ECARWT'), '') 
						   WHEN @StatusCheck = 'UNITNOTVALID' THEN '' END) AS AvailableFinishWeight 
				from Building bg
				 Left Join GroupConfiguration gc on bg.Id = gc.BuildingId
				 Left Join Units us on us.GroupConfigurationId = gc.GroupId
				 Left Join UnitSet ust on ust.SetId= @setId

				 Where us.GroupConfigurationId= @grpId
		 
				(select * from (select distinct 
								SUM(CAST(Front AS INT)) as FrontOpenings,
								SUM(CAST(Rear AS INT)) as RearOpenings,						
								unitId 
								from OpeningLocation where UnitId in (select UnitId from
											 Units where SetId=@setId ) group by unitId)  c1  left Join  
								 (select distinct TravelFeet,TravelInch,NoOfFloors AS FloorServed,UnitId from OpeningLocation where UnitId in (select UnitId from
											 Units where SetId=@setId ))  c2 on c1.UnitId = c2.UnitId) 
								
				
		
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
end
 
