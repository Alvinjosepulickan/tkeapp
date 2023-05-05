CREATE TABLE [dbo].[AutoSaveConfiguration] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [RequestMessage] NVARCHAR (MAX) NOT NULL,
    [CreatedBy]      NVARCHAR (250) NOT NULL,
    [CreatedOn]      DATETIME       DEFAULT (getdate()) NULL,
    [ModifiedBy]     NVARCHAR (250) NULL,
    [ModifiedOn]     DATETIME       NULL,
    [IsDeleted]      BIT            DEFAULT ((0)) NULL
);

