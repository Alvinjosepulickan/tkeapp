CREATE TABLE [dbo].[Status] (
    [StatusId]    INT            IDENTITY (1, 1) NOT NULL,
    [StatusKey]   NVARCHAR (100) NOT NULL,
    [StatusName]  NVARCHAR (MAX) NOT NULL,
    [DisplayName] NVARCHAR (500) NULL,
    [Step]        INT            NULL,
    [CreatedBy]   NVARCHAR (50)  NULL,
    [CreatedOn]   DATETIME       NULL,
    [ModifiedBy]  NVARCHAR (50)  NULL,
    [ModifiedOn]  DATETIME       NULL,
    [IsDeleted]   BIT            DEFAULT ((0)) NOT NULL,
    [EntityId]    INT            NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([StatusId] ASC),
    FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Entity] ([EntityId]),
    UNIQUE NONCLUSTERED ([StatusKey] ASC),
    UNIQUE NONCLUSTERED ([StatusKey] ASC)
);



