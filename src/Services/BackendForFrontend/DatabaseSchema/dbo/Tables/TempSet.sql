CREATE TABLE [dbo].[TempSet] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [BuildingId] INT           NULL,
    [GroupName]  NVARCHAR (25) NULL,
    [OldGroupId] INT           NULL,
    [NewGroupId] INT           NULL,
    [OldSetId]   INT           NULL,
    [NewSetid]   INT           NULL
);

