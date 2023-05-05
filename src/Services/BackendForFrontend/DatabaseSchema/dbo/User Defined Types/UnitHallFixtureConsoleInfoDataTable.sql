CREATE TYPE [dbo].[UnitHallFixtureConsoleInfoDataTable] AS TABLE (
    [ConsoleId]    NVARCHAR (3)   NULL,
    [ConsoleName]  NVARCHAR (500) NULL,
    [IsController] BIT            NULL,
    [FixtureType]  NVARCHAR (100) NULL);

