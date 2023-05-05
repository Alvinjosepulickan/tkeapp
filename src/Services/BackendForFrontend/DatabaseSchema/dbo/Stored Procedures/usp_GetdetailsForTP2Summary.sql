CREATE  Procedure [dbo].[usp_GetdetailsForTP2Summary] --4

@setId int

as

begin

	BEGIN TRY
		declare @groupConfigurationId int

		declare @buildingId int

		declare @buildingEquipmentStatus nvarchar(20)

		set @groupConfigurationId=(select distinct(GroupConfigurationId) from units where setid=@setId)

		set @buildingId=(select distinct(BuildingId) from GroupConfiguration where GroupId=@groupConfigurationId)



		select isnull('['+ SUBSTRING( 
					( 
					 SELECT ',{"VariableId":"'+k.BuindingType+'","Value":"'+k.BuindingValue+'"}'
					from building b
					Left Join BuildingConfiguration k
					on b.Id = k.BuildingId
					where b.id = @buildingId
					and b.isDeleted=0
							 FOR XML PATH('') 
				), 2 ,99999) + ']','') As BldJson 

 

		Select u.UnitId,ISNULL(u.UnitJson,'')as UnitJson,

			   ISNULL(h.HallRiserJson,'') as HallRiserJson,

			   ISNULL(d.DoorJson,'') as DoorJson,

			   ISNULL(cl.ControlLocationJson,'') as ControlLocationJson

			   from units as u left join HallRiser as h on u.UnitId=h.UnitId left join Doors as d on d.UnitId=u.UnitId left join ControlLocation cl on cl.GroupConfigurationId=u.GroupConfigurationId where u.SetId=@setId and u.IsDeleted=0

		select * from (select ConfigureJson  from UnitConfiguration where SetId=@setId and IsDeleted=0) as c2 cross join
		(select sum(cast(Front as int))+sum(cast(Rear as int)) as totalOpenings from 
		OpeningLocation where UnitId in (select UnitId from units where SetId=@setId)) as c1

		select UnitId,UEID,Designation,ProductName from units join UnitSet on Units.SetId=UnitSet.SetId where units.SetId=@setId

		 select distinct(GroupConfigurationId) from units where SetId=@setId
		  select distinct Travelfeet,TravelInch from OpeningLocation where UnitId in (select unitId from Units where SetId=@setId)

		---- Group Hall Fixture Parameters ----
		select VariableType, VariableValue,locations.UnitId,Units.Name, FloorDesignation, Front, Rear  from GroupHallFixtureConsole console join 
		GroupHallFixtureConfiguration config on 
		console.GroupHallFixtureConsoleId = config.GroupHallFixtureConsoleId join GroupHallFixtureLocations locations on
		console.GroupHallFixtureConsoleId =  locations.GroupHallFixtureConsoleId join Units on Units.UnitId = locations.UnitId where
		GroupId=@groupConfigurationId


		---- Unit Hall Fixture Parameters ----
		select VariableType, VariableValue, FloorNumber, Front, Rear  from UnitHallFixtureConsole console join 
		UnitHallFixtureConfiguration config on 
		console.UnitHallFixtureConsoleId = config.UnitHallFixtureConsoleId join UnitHallFixtureLocations locations on
		console.UnitHallFixtureConsoleId =  locations.UnitHallFixtureConsoleId where SetId=@setId

		---- Entrance Console Parameters ----
		select VariableType, VariableValue, FloorNumber, Front, Rear  from EntranceConsole console join 
		EntranceConfiguration config on 
		console.EntranceConsoleId = config.EntranceConsoleId join EntranceLocations locations on
		console.EntranceConsoleId =  locations.EntranceConsoleId where SetId=@setId

		select distinct pjs.Name,pjs.OpportunityId,pjSe.Source,bchs.Branch,bld.BldName,Qts.VersionId,qts.QuoteId,status1.StatusName as ProjectStatus
		,grps.GroupName,uts.Designation,[Status].StatusName as [Status],ol.FrontOpening,ol.RearOpening, (ol.Travelfeet*12) +ol.TravelInch as Travel
			from Projects pjs left join 
				ProjectSource pjSe on pjse.id=pjs.ProjectSource left join
					Quotes qts on qts.OpportunityId=pjs.OpportunityId left join
						Branch bchs on bchs.BranchNumber=pjs.BranchNumber left join
							Building bld on bld.QuoteId=qts.QuoteId left join
								GroupConfiguration grps on grps.BuildingId=bld.id left join
									Units uts on uts.GroupConfigurationId=grps.GroupId left join
										OpeningLocation ol on ol.UnitId=uts.UnitId left join
										[status] status1 on status1.StatusKey=pjs.WorkflowStage left join
										[Status] on uts.WorkflowStatus=[Status].StatusKey 
										where uts.SetId=@setId and uts.IsDeleted=0

		--- Building Equipment Parameters ---
		select VariableType, [Value] from BldgEquipmentConfiguration where BuildingId=@buildingId

		--- Car Call Cutout KeySwitches ---
		select sum(cast(Front as INT))+sum(cast(REAR as INT)) as totalCarCallCutoutSwitches from CarCallCutoutLocations
		where SetId=@setId

		select BuildingId,VariableId,isnull(CurrentValue,'') as CurrentValue,isnull(PreviousValue,'') as PreviousValue,CreatedBy,CreatedOn from BuildingConfigHistory where BuildingId=@buildingId
		select GroupId,VariableId,isnull(CurrentValue,'') as CurrentValue,isnull(PreviousValue,'') as PreviousValue,CreatedBy,CreatedOn from GroupConfigHistory where GroupId=@groupConfigurationId
		select SetId,VariableId,isnull(CurrentValue,'') as CurrentValue,isnull(PreviousValue,'') as PreviousValue,CreatedBy,CreatedOn from UnitConfigHistory where SetId=@setId

		select distinct bg.Id as BuildingId,bg.bldname as BuildingName,gc.GroupId as GroupConfigurationId, gc.GroupName,ut.Designation,uts.ProductName,ut.SetId 
			from Building bg left join 
				GroupConfiguration gc on gc.BuildingId=bg.Id and gc.IsDeleted=0 left join
					units ut on ut.GroupConfigurationId=gc.GroupId and ut.IsDeleted=0 left join
						UnitSet uts on uts.SetId=ut.SetId and uts.IsDeleted=0
					where bg.QuoteId in ( select distinct quoteId from Building where Id=@buildingId )and bg.isDeleted =0
		select distinct pjs.Name,pjs.OpportunityId,qts.VersionId,qts.CreatedOn,act.AccountName as AccountName,act.AddressLine1 ,act.AddressLine2,act.City,act.State , act.County as Country,act.ZipCode,act.CustomerNumber,Status.StatusName as QuoteStatusKey,status1.StatusName ,pjSe.Source,bchs.Branch,bld.BldName,Qts.VersionId,qts.QuoteId,status1.StatusName as ProjectStatus
		
			from Projects pjs left join 
				ProjectSource pjSe on pjse.id=pjs.ProjectSource left join
					Quotes qts on qts.OpportunityId=pjs.OpportunityId left join
						Branch bchs on bchs.BranchNumber=pjs.BranchNumber left join
							AccountDetails act on act.opportunityid=pjs.OpportunityId left join
							Building bld on bld.QuoteId=qts.QuoteId left join
								GroupConfiguration grps on grps.BuildingId=bld.id left join
									Units uts on uts.GroupConfigurationId=grps.GroupId left join
										OpeningLocation ol on ol.UnitId=uts.UnitId left join
										[status] status1 on status1.StatusKey=pjs.WorkflowStage left join
										[Status] on uts.WorkflowStatus=[Status].StatusKey 
										where uts.SetId=@setId and uts.IsDeleted=0

		--- for getting all the units from the group id corresponding to the input setId ---
		select UnitId, Name, SetId, UEID  from Units where GroupConfigurationid=(select distinct groupconfigurationId from units where SetId=@setId)

		--- getting building equipment status ---
		select BuildingEquipmentStatus from Building where Id=@buildingId

		select FloorDesignation,FloorNumber,MainEgress,alternateEgress from BuildingElevation where (MainEgress=1 or alternateEgress=1) and buildingId=@buildingId 
		select FloorDesignation,floorNumber,FloorDesignation,FloorToFloorHeightFeet,FloorToFloorHeightInch from BuildingElevation where buildingId=@buildingId 
		select isnull(econsole.EntranceConsoleId,0) as EntranceConsoleId,isnull([Name],'') as [Name],isnull(VariableType,'')as VariableType,isnull(VariableValue,'') as VariableValue,isnull(FloorNumber,0) as FloorNumber,isnull(Front,0) as Front,isnull(Rear,0) as Rear from [dbo].[EntranceConsole] econsole left join [dbo].[EntranceConfiguration] econfig on econsole.EntranceConsoleId =econfig.EntranceConsoleId left join [dbo].[EntranceLocations] elocations on econsole.EntranceConsoleId =elocations.EntranceConsoleId where setid=@setId
		select isnull(econsole.UnitHallFixtureConsoleId,0) as EntranceConsoleId,isnull([Name],'') as [Name],isnull(VariableType,'')as VariableType,isnull(VariableValue,'') as VariableValue,isnull(FloorNumber,0) as FloorNumber,isnull(Front,0) as Front,isnull(Rear,0) as Rear 
	from [dbo].[UnitHallFixtureConsole] econsole 
	left join [dbo].UnitHallFixtureConfiguration econfig on econsole.UnitHallFixtureConsoleId =econfig.UnitHallFixtureConsoleId 
	left join [dbo].UnitHallFixtureLocations elocations on econsole.UnitHallFixtureConsoleId =elocations.UnitHallFixtureConsoleId where setid=@setId

		--- getting the price details for the units in the set ---
		select UnitId, VariableId, VariableValue,CreatedBy,CreatedOn from PriceDetails where UnitId in (select distinct UnitId from units where SetId=@setId)
		select front, rear,FloorNumber from OpeningLocation where UnitId in (select distinct UnitId from units where SetId=@setId)
		select ElevationFeet,floorNumber from BuildingElevation where BuildingId =@buildingId
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