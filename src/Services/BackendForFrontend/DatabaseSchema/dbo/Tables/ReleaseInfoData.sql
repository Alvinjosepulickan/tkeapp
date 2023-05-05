CREATE TABLE [dbo].[ReleaseInfoData] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [SetId]              INT            NULL,
    [releaseComments]    NVARCHAR (MAX) NULL,
    [isAcknowledge]      BIT            NULL,
    [ConfigureVariables] NVARCHAR (200) NULL,
    [ConfigureValues]    NVARCHAR (50)  NULL,
    [CreatedOn]          DATETIME       DEFAULT (getdate()) NOT NULL,
    [CreatedBy]          NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

