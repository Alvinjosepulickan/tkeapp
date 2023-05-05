CREATE TYPE [dbo].[GroupHallFixtureLocationDataTable] AS TABLE (
    [GroupHallFixtureConsoleId] INT            NULL,
    [UnitId]                    INT            NULL,
    [FloorDesignation]          NVARCHAR (3)   NULL,
    [Front]                     BIT            NULL,
    [Rear]                      BIT            NULL,
    [HallStationName]           NVARCHAR (MAX) NULL);



