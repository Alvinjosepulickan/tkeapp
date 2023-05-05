CREATE TYPE [dbo].[tblBE] AS TABLE (
    [floorDesignation]       VARCHAR (50)     NOT NULL,
    [elevationFeet]          INT              NOT NULL,
    [elevationInch]          NUMERIC (7, 5)   NOT NULL,
    [floorToFloorHeightFeet] INT              NOT NULL,
    [floorToFloorHeightInch] NUMERIC (7, 5)   NOT NULL,
    [buildingId]             INT              NOT NULL,
    [mainEgress]             BIT              NOT NULL,
    [userId]                 VARCHAR (50)     NULL,
    [date]                   DATETIME         NULL,
    [AlternateEgress]        BIT              NOT NULL,
    [noOfFloor]              INT              NULL,
    [buildingRise]           NUMERIC (20, 10) NULL,
    [floorNumber]            INT              NOT NULL);

