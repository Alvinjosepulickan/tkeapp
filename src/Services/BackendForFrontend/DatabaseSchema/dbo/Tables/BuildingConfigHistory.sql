﻿CREATE TABLE [dbo].[BuildingConfigHistory] (
    [BuildingConfigHistoryId] INT            IDENTITY (1, 1) NOT NULL,
    [BuildingId]              INT            NOT NULL,
    [VariableId]              NVARCHAR (MAX) NOT NULL,
    [CurrentValue]            NVARCHAR (MAX) NULL,
    [PreviousValue]           NVARCHAR (MAX) NULL,
    [CreatedBy]               NVARCHAR (250) NOT NULL,
    [CreatedOn]               DATETIME       NOT NULL,
    [ModifiedBy]              NVARCHAR (250) NULL,
    [ModifiedOn]              NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([BuildingConfigHistoryId] ASC)
);



