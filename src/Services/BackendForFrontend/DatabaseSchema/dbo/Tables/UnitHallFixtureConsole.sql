CREATE TABLE [dbo].[UnitHallFixtureConsole] (
    [UnitHallFixtureConsoleId] INT            IDENTITY (1, 1) NOT NULL,
    [ConsoleNumber]            INT            NOT NULL,
    [FixtureType]              NVARCHAR (100) NOT NULL,
    [SetId]                    INT            NOT NULL,
    [Name]                     NVARCHAR (100) NOT NULL,
    [IsController]             BIT            NULL,
    [CreatedBy]                NVARCHAR (250) NOT NULL,
    [CreatedOn]                DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]               NVARCHAR (250) NULL,
    [ModifiedOn]               DATETIME       NULL,
    [IsDeleted]                BIT            DEFAULT ((0)) NULL,
    [ConsoleOrder]             INT            NULL
);

