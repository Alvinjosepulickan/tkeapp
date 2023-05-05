
CREATE Procedure [dbo].[usp_GetFloorDesignationByGroupId] --745

-- Add the parameters for the stored procedure here

@id int

as

Begin

 BEGIN TRY       
		select FloorDesignation,
		isnull([dbo].[FnGetBuildingTableValueFromBldJson](BuildingId,'Building_Configuration.Parameters.BLANDINGS'),2) as numberoffloor
		from BuildingElevation where BuildingId in (select distinct BuildingId from GroupConfiguration where groupid=@id and IsDeleted=0)
		order by buildingelevationid desc
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

