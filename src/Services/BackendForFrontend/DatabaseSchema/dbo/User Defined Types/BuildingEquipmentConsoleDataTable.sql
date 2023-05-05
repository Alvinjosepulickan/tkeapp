CREATE TYPE [dbo].[BuildingEquipmentConsoleDataTable] AS TABLE (
    [ConsoleId]    NVARCHAR (3)   NULL,
    [ConsoleName]  NVARCHAR (500) NULL,
    [IsController] BIT            NULL,
    [IsLobbyPanel] BIT            NULL);

