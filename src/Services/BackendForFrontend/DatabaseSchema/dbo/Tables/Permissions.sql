CREATE TABLE [dbo].[Permissions] (
    [PermissionId]   INT            IDENTITY (1, 1) NOT NULL,
    [PermissionKey]  NVARCHAR (100) NOT NULL,
    [PermissionName] NVARCHAR (MAX) NOT NULL,
    [EntityId]       INT            NULL,
    [CreatedBy]      VARCHAR (MAX)  NULL,
    [CreatedOn]      DATETIME       NULL,
    [ModifiedBy]     VARCHAR (MAX)  NULL,
    [ModifiedOn]     DATETIME       NULL,
    [IsDeleted]      BIT            DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([PermissionId] ASC),
    FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Entity] ([EntityId]),
    UNIQUE NONCLUSTERED ([PermissionKey] ASC)
);



