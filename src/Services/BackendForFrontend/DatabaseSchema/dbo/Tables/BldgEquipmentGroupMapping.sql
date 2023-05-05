CREATE TABLE [dbo].[BldgEquipmentGroupMapping] (
    [GroupMappingId] INT            IDENTITY (1, 1) NOT NULL,
    [GroupId]        INT            NOT NULL,
    [GroupName]      NVARCHAR (MAX) NULL,
    [ConsoleId]      INT            NOT NULL,
    [is_Checked]     INT            DEFAULT ((0)) NOT NULL,
    [CreatedBy]      NVARCHAR (MAX) NOT NULL,
    [CreatedOn]      DATE           DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]     NVARCHAR (MAX) NULL,
    [ModifiedOn]     DATE           NULL
);

