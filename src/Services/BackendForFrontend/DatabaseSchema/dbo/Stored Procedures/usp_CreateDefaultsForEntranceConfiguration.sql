CREATE PROCEDURE [dbo].[usp_CreateDefaultsForEntranceConfiguration]
@setId int,
@controlLanding Nvarchar(100),
@userName Nvarchar(100),
@defaultUHFConfiguration DefaultConfigurationTable Readonly
AS
Begin
	BEGIN TRY
		declare @entranceConsoleID int
		declare @minUnitId int
		select @minUnitId=(select min(UnitId) from Units where SetId=@setId)
		IF(@controlLanding<>0)
		BEGIN
			/*Controller Console*/
			declare @front int,@rear int
			--entry Entrance console table
			insert into EntranceConsole(ConsoleNumber,SetId,Name,IsController,CreatedBy,CreatedOn)
			values(1,@setId,N'EntranceConsoleWithController',1,@username,getdate())
			select @entranceConsoleID =SCOPE_IDENTITY()

			--Entrance configuration table
			insert into EntranceConfiguration(EntranceConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @entranceConsoleID,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='ControllerConsole'
			
			--entry to opening location table
			insert into EntranceLocations(entranceConsoleId,FloorNumber,Front,Rear,CreatedBy,CreatedOn)
			select distinct @entranceConsoleID, FloorNumber,0,0,@username,GETDATE()
			from openinglocation where UnitId =@minUnitId

			Update EntranceLocations 
			Set Front=ol.Front,Rear=case when ol.Front=1 then 0 else ol.Rear end
			From OpeningLocation ol
			Where EntranceLocations.FloorNumber =ISNULL(@controlLanding,0)
			and EntranceConsoleId=@entranceConsoleID and
			ol.FloorNumber =ISNULL(@controlLanding,0) and
			ol.UnitId=@minUnitId

			select @front=Front,@rear=Rear
			From EntranceLocations 
			where EntranceConsoleId=@entranceConsoleID
			and FloorNumber =ISNULL(@controlLanding,0)
			/*Controller Console*/

			/*Non Controller Console*/
			insert into EntranceConsole(ConsoleNumber,SetId,Name,IsController,CreatedBy,CreatedOn)
			values(2,@setId,N'EntranceConsole1',0,@username,getdate())
			select @entranceConsoleID =SCOPE_IDENTITY()

			--Entrance configuration table
			insert into EntranceConfiguration(EntranceConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @entranceConsoleID,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='NonControllerConsole'
			
			--entry to opening location table
			insert into EntranceLocations(entranceConsoleId,FloorNumber,Front,Rear,CreatedBy,CreatedOn)
			select distinct @entranceConsoleID, FloorNumber,Front,Rear,@username,GETDATE()
			from openinglocation where UnitId =@minUnitId

			Update EntranceLocations
			Set Front=case when @front=1 then 0 else ol.Front end,
			Rear=case when @rear=1 then 0 else ol.Rear end
			From OpeningLocation ol
			Where ol.FloorNumber=ISNULL(@controlLanding,0) and ol.UnitId=@minUnitId
			and EntranceLocations.EntranceConsoleId=@entranceConsoleID
			and EntranceLocations.FloorNumber=ISNULL(@controlLanding,0)
			
			/*Non Controller Console*/
		END
		ELSE
		BEGIN
			/*Non Controller Console*/
			insert into EntranceConsole(ConsoleNumber,SetId,Name,IsController,CreatedBy,CreatedOn)
			values(2,@setId,N'EntranceConsole1',0,@username,getdate())
			select @entranceConsoleID =SCOPE_IDENTITY()

			--Entrance configuration table
			insert into EntranceConfiguration(EntranceConsoleId,VariableType,VariableValue,CreatedBy,CreatedOn)
			select  @entranceConsoleID,[VariableType],[VariableValue],@userName,getdate()
			From @defaultUHFConfiguration Where [ConsoleType]='NonControllerConsole'
			
			--entry to opening location table
			insert into EntranceLocations(entranceConsoleId,FloorNumber,Front,Rear,CreatedBy,CreatedOn)
			select distinct @entranceConsoleID, FloorNumber,Front,Rear,@username,GETDATE()
			from openinglocation where UnitId =@minUnitId
			/*Non Controller Console*/
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
END