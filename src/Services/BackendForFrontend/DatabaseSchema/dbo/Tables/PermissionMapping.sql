CREATE TABLE [dbo].[PermissionMapping] (
    [PermissionMappingId] INT            IDENTITY (1, 1) NOT NULL,
    [EntityId]            INT            NULL,
    [PermissionKey]       NVARCHAR (100) NULL,
    [ProjectStage]        NVARCHAR (100) NULL,
    [BuildingStatus]      NVARCHAR (100) NULL,
    [GroupStatus]         NVARCHAR (100) NULL,
    [UnitStatus]          NVARCHAR (100) NULL,
    [CreatedBy]           NVARCHAR (100) NULL,
    [CreatedOn]           DATETIME       NULL,
    [ModifiedBy]          NVARCHAR (100) NULL,
    [ModifiedOn]          DATETIME       NULL,
    [IsDeleted]           BIT            DEFAULT ((0)) NULL,
    [RoleKey]             NVARCHAR (20)  NULL,
    PRIMARY KEY CLUSTERED ([PermissionMappingId] ASC),
    FOREIGN KEY ([BuildingStatus]) REFERENCES [dbo].[Status] ([StatusKey]),
    FOREIGN KEY ([BuildingStatus]) REFERENCES [dbo].[Status] ([StatusKey]),
    FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Entity] ([EntityId]),
    FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Entity] ([EntityId]),
    FOREIGN KEY ([GroupStatus]) REFERENCES [dbo].[Status] ([StatusKey]),
    FOREIGN KEY ([GroupStatus]) REFERENCES [dbo].[Status] ([StatusKey]),
    FOREIGN KEY ([PermissionKey]) REFERENCES [dbo].[Permissions] ([PermissionKey]),
    FOREIGN KEY ([PermissionKey]) REFERENCES [dbo].[Permissions] ([PermissionKey]),
    FOREIGN KEY ([ProjectStage]) REFERENCES [dbo].[Status] ([StatusKey]),
    FOREIGN KEY ([ProjectStage]) REFERENCES [dbo].[Status] ([StatusKey]),
    FOREIGN KEY ([UnitStatus]) REFERENCES [dbo].[Status] ([StatusKey]),
    FOREIGN KEY ([UnitStatus]) REFERENCES [dbo].[Status] ([StatusKey])
);



