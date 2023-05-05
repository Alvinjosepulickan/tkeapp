CREATE TABLE [dbo].[BldgEquipmentCategoryCnfgn] (
    [GroupCategoryConfigurationId] INT            IDENTITY (1, 1) NOT NULL,
    [GroupCategoryId]              INT            NOT NULL,
    [GroupName]                    NVARCHAR (MAX) NULL,
    [NoOfUnits]                    INT            NOT NULL,
    [UnitName]                     NVARCHAR (MAX) NULL,
    [FactoryId]                    NVARCHAR (MAX) NULL,
    [ConsoleId]                    INT            NOT NULL,
    [CreatedBy]                    NVARCHAR (MAX) NOT NULL,
    [CreatedOn]                    DATE           DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]                   NVARCHAR (MAX) NULL,
    [ModifiedOn]                   DATE           NULL
);

