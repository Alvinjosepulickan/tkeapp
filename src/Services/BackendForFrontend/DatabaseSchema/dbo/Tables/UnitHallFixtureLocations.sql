CREATE TABLE [dbo].[UnitHallFixtureLocations] (
    [UnitHallFixtureLocationId] INT            IDENTITY (1, 1) NOT NULL,
    [UnitHallFixtureConsoleId]  INT            NOT NULL,
    [FloorNumber]               INT            NOT NULL,
    [Front]                     BIT            NOT NULL,
    [Rear]                      BIT            NOT NULL,
    [CreatedBy]                 NVARCHAR (250) NOT NULL,
    [CreatedOn]                 DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]                NVARCHAR (250) NULL,
    [ModifiedOn]                DATETIME       NULL,
    [IsDeleted]                 BIT            DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([UnitHallFixtureLocationId] ASC)
);

