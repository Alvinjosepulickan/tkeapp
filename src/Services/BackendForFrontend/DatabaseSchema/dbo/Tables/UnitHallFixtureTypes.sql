CREATE TABLE [dbo].[UnitHallFixtureTypes] (
    [UnitHallFixtureTypeId] INT           NOT NULL,
    [UnitHallFixtureType]   NVARCHAR (50) NOT NULL,
    [ETA]                   INT           NOT NULL,
    [ETD]                   INT           NOT NULL,
    [ETADefault]            INT           NULL,
    [ETDDefault]            INT           NULL,
    [ETA_ETD]               INT           NULL
);

