
CREATE PROCEDURE [dbo].[usp_GetBuildingEquipmentConfigurationById]    
-- Add the parameters for the stored procedure here
 @buildingId INT
,@BuildingEquipmentVariablesList as [dbo].[VariableMapper] READONLY
AS
BEGIN
	BEGIN TRY
	
	   declare @VariableControlLocationType nvarchar(200);
	   declare @VariableFixtureStatergy nvarchar(200);
	   SET @VariableControlLocationType = (Select VariableType from @BuildingEquipmentVariablesList where VariableKey ='CONTROLLOCATIONTYPE')
	   SET @VariableFixtureStatergy = (Select VariableType from @BuildingEquipmentVariablesList where VariableKey ='FIXTURESTETERGY')


	  declare @CountETD int
	  declare @fixtureStrategy nvarchar(100)
	  select @CountETD=isnull(Count(ControlLocationValue),0) from ControlLocation 
	  where GroupConfigurationId in(select groupid from GroupConfiguration where BuildingId=@buildingId ) 
	  and ControlLocationType = @VariableControlLocationType and ControlLocationValue in('ETD','ETA/ETD');
	  select @fixtureStrategy=case when @CountETD=0 then N'ETA' else N'ETD' end
	  SELECT  VariableType
			 ,[Value]  
				 FROM [dbo].[BldgEquipmentConfiguration]
					 WHERE BuildingId = @buildingId
	  union All
	  select @VariableFixtureStatergy, @fixtureStrategy

	  --Building Equipment status to complete when user is visiting the screen
	  update Building set BuildingEquipmentStatus='BLDGEQP_COM'
	  where id=@buildingId
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
