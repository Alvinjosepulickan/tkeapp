CREATE TABLE [dbo].[BuildingElevation] (
    [BuildingElevationId]    INT              IDENTITY (1, 1) NOT NULL,
    [BuildingId]             INT              NOT NULL,
    [MainEgress]             BIT              NOT NULL,
    [FloorDesignation]       VARCHAR (50)     NOT NULL,
    [ElevationFeet]          INT              NOT NULL,
    [ElevationInch]          NUMERIC (7, 5)   NULL,
    [FloorToFloorHeightFeet] INT              NOT NULL,
    [FloorToFloorHeightInch] NUMERIC (7, 5)   NOT NULL,
    [CreatedBy]              NVARCHAR (50)    NULL,
    [CreatedOn]              DATETIME         DEFAULT (getdate()) NULL,
    [ModifiedBy]             VARCHAR (50)     NULL,
    [ModifiedOn]             DATETIME         NULL,
    [IsDeleted]              BIT              DEFAULT ((0)) NULL,
    [alternateEgress]        BIT              NULL,
    [floorNumber]            INT              NULL,
    [noOfFloor]              INT              NULL,
    [buildingRise]           NUMERIC (20, 10) NULL,
    PRIMARY KEY CLUSTERED ([BuildingElevationId] ASC)
);

