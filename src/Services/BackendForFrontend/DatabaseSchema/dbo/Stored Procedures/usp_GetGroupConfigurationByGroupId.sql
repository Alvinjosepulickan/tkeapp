 
CREATE Procedure [dbo].[usp_GetGroupConfigurationByGroupId]
-- Add the parameters for the stored procedure here
@id int
as
Begin
	BEGIN TRY
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
END 
