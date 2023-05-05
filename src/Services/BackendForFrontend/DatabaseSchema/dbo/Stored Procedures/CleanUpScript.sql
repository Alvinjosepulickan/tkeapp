-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CleanUpScript] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   

--TODO: Sequencing of truccation to avoiding collision of Constraints

Begin /*Configuraion*/
--Truncate Order: Units-> Groups -> Building
	Begin /*Section:Unit*/
		truncate table    [dbo].[UnitConfiguration]
		truncate table    [dbo].[UnitConfigHistory]
		truncate table    [dbo].[UnitHallFixtureConfiguration]
		truncate table    [dbo].[UnitHallFixtureConsole]
		truncate table    [dbo].[UnitHallFixtureLocations]
		truncate table	  [dbo].[LeadTimeDetails]/*v3*/
		truncate table    [dbo].[PriceDetails] /*v3*/
		DELETE   From     [dbo].[UnitSet]
		DELETE   From     [dbo].[Units]
	End
	
	Begin /*Section:Group*/
		truncate table    [dbo].[GroupConfigHistory]
		truncate table    [dbo].[GroupConfiguration]
		truncate table    [dbo].[GroupConfigurationDetails]
		truncate table    [dbo].[GroupHallFixtureConfiguration]
		truncate table    [dbo].[GroupHallFixtureConsole]
		truncate table    [dbo].[GroupHallFixtureLocations]
		truncate table    [dbo].[HallRiser]
		truncate table    [dbo].[AutoSaveConfiguration]
	END
	
	Begin /*Section:Buidling*/
		truncate table    [dbo].[BldgEquipmentCategoryCnfgn]
		truncate table    [dbo].[BldgEquipmentConfiguration]
		truncate table    [dbo].[BldgEquipmentConsole]
		truncate table    [dbo].[BldgEquipmentConsoleCnfgn]
		truncate table    [dbo].[BldgEquipmentGroupMapping]
		truncate table    [dbo].[BuildingConfigHistory]
		truncate table    [dbo].[BuildingConfiguration]
		truncate table    [dbo].[BuildingElevation]
		truncate table    [dbo].[Building]
	End
		
	Begin /*Section:MISC*/
		truncate table    [dbo].[CarCallCutoutLocations]
		truncate table    [dbo].[ControlLocation]
		truncate table    [dbo].[CoordinationQuestions]
		truncate table    [dbo].[Doors]
		truncate table    [dbo].[EntranceConfiguration]
		truncate table    [dbo].[EntranceConsole]
		truncate table    [dbo].[EntranceLocations]
		truncate table    [dbo].[OpeningLocation]
		truncate table    [dbo].[ResetConsoleTable]
	End
End	
	
Begin /*Section:Drawing*/
	truncate table    [dbo].[FieldDrawingAutomation]
	truncate table    [dbo].[FieldDrawingAutomationProcessDetails]
	DELETE FROM    [dbo].[FieldDrawingIntegrationMaster] /*v2*/	
	DELETE FROM 	  [dbo].[FieldDrawingMaster] /*v2*/	
End
	
Begin /*Section:Project*/
	truncate table    [dbo].[TempSet]
	truncate table    [dbo].[TempUnit]
	truncate table    [dbo].[TempUnitHallFixture]
	truncate table    [dbo].[TempUnitTable]
	truncate table    [dbo].[Systems]
	truncate table    [dbo].[SystemsVariables] /*v4*/	
	truncate table    [dbo].[Projects]
	truncate table    [dbo].[Quotes]
	truncate table    [dbo].[AccountDetails]
	truncate table    [dbo].[PrimaryQuotes]
	truncate table    [dbo].[ProcedureLog]
End

DBCC CHECKIDENT (FieldDrawingIntegrationMaster, reseed, 1);
DBCC CHECKIDENT (FieldDrawingMaster, reseed, 1);
DBCC CHECKIDENT (UnitSet, reseed, 1);
DBCC CHECKIDENT (Units,reseed,1);


END