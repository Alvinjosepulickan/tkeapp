CREATE TYPE [dbo].[unitDataTable] AS TABLE (
    [UnitId]             INT            NULL,
    [UnitJson]           NVARCHAR (MAX) NULL,
    [value]              BIT            NULL,
    [UnitJsonData]       NVARCHAR (MAX) NULL,
    [DisplayUnitJson]    NVARCHAR (MAX) NULL,
    [Location]           NVARCHAR (8)   NULL,
    [MappedLocationJson] NVARCHAR (8)   NULL,
    [UnitName]           NVARCHAR (20)  NULL,
    [UnitDesignation]    NVARCHAR (50)  NULL,
    [IsFutureElevator]   BIT            NOT NULL);

