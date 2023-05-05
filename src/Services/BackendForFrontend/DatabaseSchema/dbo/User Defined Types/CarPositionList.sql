CREATE TYPE [dbo].[CarPositionList] AS TABLE (
    [CarPositionID]       INT          NOT NULL,
    [CarPositionLocation] NVARCHAR (8) NULL,
    [UnitDesignation]     NVARCHAR (8) NULL,
    [UnitName]            NVARCHAR (8) NULL);

