CREATE TYPE [dbo].[EntranceLocationDataTable] AS TABLE (
    [EntranceConsoleId] NVARCHAR (3) NULL,
    [FloorNumber]       INT          NULL,
    [Front]             BIT          NULL,
    [Rear]              BIT          NULL);

