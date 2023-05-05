CREATE TABLE [dbo].[UserRoleMaster] (
    [RoleId]     INT            IDENTITY (1, 1) NOT NULL,
    [RoleName]   NVARCHAR (MAX) NULL,
    [CreatedBy]  NVARCHAR (MAX) NULL,
    [CreatedOn]  DATE           DEFAULT (getdate()) NULL,
    [ModifiedBy] NVARCHAR (MAX) NULL,
    [ModifiedOn] DATE           NULL,
    [IsDeleted]  BIT            NULL,
    [RoleKey]    NVARCHAR (20)  NULL,
    PRIMARY KEY CLUSTERED ([RoleId] ASC),
    UNIQUE NONCLUSTERED ([RoleKey] ASC),
    UNIQUE NONCLUSTERED ([RoleKey] ASC)
);



