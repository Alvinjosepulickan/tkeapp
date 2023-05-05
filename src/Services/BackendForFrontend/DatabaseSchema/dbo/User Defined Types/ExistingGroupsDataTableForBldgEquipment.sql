CREATE TYPE [dbo].[ExistingGroupsDataTableForBldgEquipment] AS TABLE (
    [ConsoleId]       NVARCHAR (3)   NULL,
    [GroupCategoryId] INT            NULL,
    [GroupName]       NVARCHAR (500) NULL,
    [NoOfUnits]       INT            NULL,
    [GroupFactoryId]  NVARCHAR (500) NULL);

