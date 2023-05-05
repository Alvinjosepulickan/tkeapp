CREATE TABLE [dbo].[GroupHallFixtureTypes] (
    [GroupHallFixtureTypeId] INT           NOT NULL,
    [GroupHallFixtureType]   NVARCHAR (50) NOT NULL,
    [ETA]                    BIT           NOT NULL,
    [ETD]                    BIT           NOT NULL,
    [ETA/ETD]                BIT           NOT NULL
);

