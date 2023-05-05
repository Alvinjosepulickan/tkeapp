CREATE TABLE [dbo].[TempOpeningLocation] (
    [OpeningLocationId]    INT              IDENTITY (1, 1) NOT NULL,
    [GroupConfigurationId] INT              NULL,
    [UnitId]               NVARCHAR (50)    DEFAULT ((0)) NULL,
    [Travelfeet]           INT              DEFAULT ((0)) NULL,
    [TravelInch]           NUMERIC (20, 10) DEFAULT ((0)) NULL,
    [OcuppiedSpaceBelow]   BIT              DEFAULT ((0)) NULL,
    [NoOfFloors]           INT              DEFAULT ((0)) NULL,
    [FrontOpening]         INT              DEFAULT ((0)) NULL,
    [RearOpening]          INT              DEFAULT ((0)) NULL,
    [SideOpening]          INT              DEFAULT ((0)) NULL,
    [FloorDesignation]     VARCHAR (50)     DEFAULT ('') NULL,
    [FloorNumber]          INT              DEFAULT ((0)) NULL,
    [Front]                BIT              DEFAULT ((0)) NULL,
    [Side]                 BIT              DEFAULT ((0)) NULL,
    [Rear]                 BIT              DEFAULT ((0)) NULL,
    [CreatedBy]            NVARCHAR (250)   DEFAULT ('') NULL,
    [CreatedOn]            DATETIME         DEFAULT ('') NULL,
    [ModifiedBy]           NVARCHAR (250)   NULL,
    [ModifiedOn]           DATETIME         NULL,
    [IsDeleted]            BIT              DEFAULT ((0)) NULL
);

