CREATE Procedure [dbo].[usp_GetBuildingElevationById]



-- Add the parameters for the stored procedure here



@id int,
@VariableMapperDataTable AS VariableMapper READONLY


as



Begin
	BEGIN TRY

		DECLARE @BuildingLandings NVARCHAR(250)
		DECLARE @TotalBuildingFloorToFloorHeight NVARCHAR(250)
		DECLARE @AverageRoofHeight NVARCHAR(250)
		SET @AverageRoofHeight = (select VariableType from @VariableMapperDataTable where variableKey = 'AVGHEIGHT')
		SET @TotalBuildingFloorToFloorHeight = (select VariableType from @VariableMapperDataTable where variableKey = 'TOTALBUILDINGFLOORTOFLOORHEIGHT')
		SET @BuildingLandings = (select VariableType from @VariableMapperDataTable where variableKey = 'BLANDINGS')
        
		Select bg.id



      ,be.floordesignation

   ,be.floorNumber



         -- ,[dbo].[FnGetBuildingTableValueFromBldJson](bg.id,'BldValidation.BLDGRISE') as buildingRise



         -- ,[dbo].[FnGetBuildingTableValueFromBldJson](bg.id,'BldValidation.FLOOR') as numberoffloor











    ,[dbo].[FnGetBuildingTableValueFromBldJson](bg.id,@TotalBuildingFloorToFloorHeight) as buildingRise

    ,isnull([dbo].[FnGetBuildingTableValueFromBldJson](bg.id,@BuildingLandings),2) as numberoffloor

	,isnull([dbo].[FnGetBuildingTableValueFromBldJson](bg.id,@AverageRoofHeight),0) as avgRoofHeight











            , be.mainegress



           ,be.alternateEgress



            ,be.elevationfeet



            ,be.elevationinch



            ,be.floortofloorheightfeet



            ,be.floortofloorheightinch



            ,be.createdby



            ,be.createdon



            ,be.modifiedby



            ,be.modifiedon



        FROM buildingelevation be



            right JOIN building bg



                ON be.buildingid = bg.id



        WHERE bg.id = @id and bg.[isDeleted]=0


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






