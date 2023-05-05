CREATE TABLE [dbo].[BldgEquipmentConsole] (
    [ConsoleId]     INT            IDENTITY (1, 1) NOT NULL,
    [ConsoleNumber] INT            NOT NULL,
    [BuildingId]    INT            NOT NULL,
    [Name]          NVARCHAR (MAX) NULL,
    [IsLobby]       BIT            DEFAULT ((0)) NULL,
    [IsController]  BIT            DEFAULT ((0)) NULL,
    [CreatedBy]     NVARCHAR (MAX) NOT NULL,
    [CreatedOn]     DATE           DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]    NVARCHAR (MAX) NULL,
    [ModifiedOn]    DATE           NULL
);

