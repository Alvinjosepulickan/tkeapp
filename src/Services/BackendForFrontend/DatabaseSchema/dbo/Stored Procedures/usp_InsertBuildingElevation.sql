





CREATE PROCEDURE [dbo].[usp_InsertBuildingElevation]

@elevationData as tblBE readonly

,@Result int OUTPUT

as

BEGIN

     BEGIN TRY

	  set nocount on;

	  declare @buildingId int


	  IF exists(select * from BuildingElevation where BuildingId=(select distinct([buildingId]) from @elevationData))
	  begin
		delete from BuildingElevation where BuildingId=(select distinct([buildingId]) from @elevationData)
	  end



	  insert into BuildingElevation([buildingId],[floorDesignation],[elevationFeet],[elevationInch],[floorToFloorHeightFeet],[floorToFloorHeightInch],[mainEgress],[createdBy],[createdOn],[AlternateEgress],[noOfFloor],[buildingRise],[floorNumber])

	  select [buildingId],[floorDesignation],[elevationFeet],[elevationInch],[floorToFloorHeightFeet],[floorToFloorHeightInch],[mainEgress],[userId],[date],[AlternateEgress],[noOfFloor],[buildingRise],[floorNumber] from @elevationData;

	  --Updating workflow status when building elevation is updated
	  UPDATE Building SET workflowstatus='BLDG_COM' where id in(select distinct([buildingId]) from @elevationData)

	  SET @Result=1

    END TRY

    BEGIN CATCH

	SET @Result=0
	EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
    declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH

END

