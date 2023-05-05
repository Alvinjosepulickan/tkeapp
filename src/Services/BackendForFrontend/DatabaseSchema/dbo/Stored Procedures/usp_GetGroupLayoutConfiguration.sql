CREATE Procedure [dbo].[usp_GetGroupLayoutConfiguration]-- 38
@id int
as
Begin 
	BEGIN TRY
		Select u.UnitId,ISNULL(u.UnitJson,'')as UnitJson,ISNULL(u.IsFutureElevator,0)as IsFutureElevator,ISNULL(u.MappedLocationJson,'') as DisplayUnitJson,
		ISNULL(u.Designation,'') as Designation,ISNULL(h.HallRiserJson,'') as HallRiserJson,MappedLocation,
		ISNULL(d.DoorJson,'') as DoorJson
		from units as u left join HallRiser as h on u.UnitId=h.UnitId left join Doors as d on d.UnitId=u.UnitId where u.GroupConfigurationId =@id 

		--Getting control location Variable assignments
		select ControlLocationJson from ControlLocation where GroupConfigurationId=@id
		--check numberoffloors
		declare @buildingId int
		set @buildingId=(select BuildingId from GroupConfiguration where GroupId=@id)
		declare @numberofFloors int
		set @numberofFloors=(select count(*) from BuildingElevation where BuildingId=@buildingId)
		declare @numberOfFloorsinopeninglocation int
		set @numberOfFloorsinopeninglocation=(select count(*) from OpeningLocation where GroupConfigurationId=@id)
		declare @numberofUnits int
		set @numberofUnits=(select count(*) from units where GroupConfigurationId=@id)
		if(@numberofUnits>0)
		begin
			set @numberOfFloorsinopeninglocation=@numberOfFloorsinopeninglocation/@numberofUnits
			if(@numberofFloors<>@numberOfFloorsinopeninglocation and @numberOfFloorsinopeninglocation>0)
			begin
				select @numberofFloors as TotalNumberOfFloors
			end
			else
			begin
				select 0 as TotalNumberOfFloors
			end
		end
		else
		begin
			select 0 as TotalNumberOfFloors
		end
		Select '[' +  
			SUBSTRING( 
			( 
				 SELECT ',{"VariableId":"'+k.GroupConfigurationType+'","Value":"'+k.GroupConfigurationValue+'"}'

			--GroupJson
			--gc.GroupId
			 -- ,gc.BuildingId
			-- GroupJson
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.Basic.GrpName') as groupName
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.Basic.ProdCat') as productCategory
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.Basic.CntrlLoc') as controlLocation
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.FloorPlan.B1P1') as floorPlanb1p1
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.FloorPlan.B1P2') as floorPlanb1p2
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.FloorPlan.B1P3') as floorPlanp3
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.FloorPlan.B1P4') as floorPlanp4
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.FloorPlan.B1P4') as floorPlanb1
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.FloorPlan.B1P4') as floorPlanb1
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.FloorPlan.B1P4') as floorPlanb3
			  --,[dbo].[FnGetGroupTableValueFromGroupJson](gc.GroupId,'GroupConfiguration.FloorPlan.B1P4') as floorPlanb4
		 

			 from [GroupConfiguration] g
			 Left join [GroupConfigurationDetails] k
			 on g.GroupId = k.GroupId
			 and g.BuildingId = k.BuildingId
			 where  g.GroupId = @id		 
			 and g.isDeleted  = 0 
			 FOR XML PATH('') 
			), 2 , 9999) + ']' As GroupJson
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
