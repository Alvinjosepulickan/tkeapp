CREATE TABLE [dbo].[GroupHallFixtureLocations] (
    [GroupHallFixtureLocationId] INT            IDENTITY (1, 1) NOT NULL,
    [GroupHallFixtureConsoleId]  INT            NOT NULL,
    [UnitId]                     INT            NOT NULL,
    [FloorDesignation]           NVARCHAR (3)   NOT NULL,
    [Front]                      BIT            NOT NULL,
    [Rear]                       BIT            NOT NULL,
    [CreatedBy]                  NVARCHAR (250) NOT NULL,
    [CreatedOn]                  DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]                 NVARCHAR (250) NULL,
    [ModifiedOn]                 DATETIME       NULL,
    [IsDeleted]                  BIT            DEFAULT ((0)) NULL,
    [HallStationName]            NVARCHAR (40)  NULL
);



