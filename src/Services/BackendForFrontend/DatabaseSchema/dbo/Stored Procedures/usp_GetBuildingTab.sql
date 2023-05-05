 
CREATE PROCEDURE [dbo].[usp_GetBuildingTab]
@buildingId int
AS
BEGIN
	BEGIN TRY
	   declare @buildingequipment int;
	   declare @isDisabled int;
 
		 SET @buildingequipment = (SELECT dbo.[Fn_GetBuildingEquipmentBybuildingId](@buildingId))
		 SET @isDisabled = (Select case when @buildingequipment is null then 0 else @buildingequipment end )
		 Select (case when @isDisabled = 0 then 1 else 0 end) as isDisabled
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
