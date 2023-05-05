CREATE TYPE [dbo].[OpeningLocationDataTableType] AS TABLE (
    [GroupConfigurationId] INT              NULL,
    [UnitId]               NVARCHAR (50)    NULL,
    [Travelfeet]           INT              NULL,
    [TravelInch]           NUMERIC (20, 10) NULL,
    [OcuppiedSpaceBelow]   BIT              NULL,
    [NoOfFloors]           INT              NULL,
    [FrontOpening]         INT              NULL,
    [RearOpening]          INT              NULL,
    [SideOpening]          INT              NULL,
    [FloorDesignation]     VARCHAR (50)     NULL,
    [FloorNumber]          INT              NULL,
    [Front]                BIT              NULL,
    [Side]                 BIT              NULL,
    [Rear]                 BIT              NULL,
    [UserName]             NVARCHAR (250)   NULL);

