CREATE TABLE [dbo].[GroupConfiguration] (
    [GroupId]         INT            IDENTITY (1, 1) NOT NULL,
    [BuildingId]      INT            NULL,
    [GroupName]       NVARCHAR (250) NOT NULL,
    [GroupJson]       NVARCHAR (MAX) NULL,
    [CreatedBy]       NVARCHAR (50)  NOT NULL,
    [CreatedOn]       DATETIME       DEFAULT (getdate()) NULL,
    [ModifedBy]       NVARCHAR (50)  NULL,
    [ModifiedOn]      DATETIME       NULL,
    [IsDeleted]       BIT            DEFAULT ((0)) NULL,
    [NeedsValidation] BIT            DEFAULT ((0)) NULL,
    [ConflictStatus]  NVARCHAR (25)  DEFAULT ('Valid') NULL,
    [WorkflowStatus]  NVARCHAR (100) NULL,
    FOREIGN KEY ([WorkflowStatus]) REFERENCES [dbo].[Status] ([StatusKey])
);



