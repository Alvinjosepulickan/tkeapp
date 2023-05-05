CREATE TABLE [dbo].[BldgEquipmentGroupCategoryMaster] (
    [GroupCategoryId]   INT            IDENTITY (1, 1) NOT NULL,
    [GroupCategoryName] NVARCHAR (MAX) NOT NULL,
    [createdBy]         NVARCHAR (MAX) NOT NULL,
    [createdon]         DATE           DEFAULT (getdate()) NOT NULL
);

