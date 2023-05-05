CREATE TABLE [dbo].[BldgEquipmentConsoleCnfgn] (
    [ConfigurationConsoleId] INT            IDENTITY (1, 1) NOT NULL,
    [ConsoleId]              INT            NOT NULL,
    [VariableType]           NVARCHAR (MAX) NULL,
    [Value]                  NVARCHAR (MAX) NULL,
    [CreatedBy]              NVARCHAR (MAX) NOT NULL,
    [CreatedOn]              DATE           DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]             NVARCHAR (MAX) NULL,
    [ModifiedOn]             DATE           NULL
);

