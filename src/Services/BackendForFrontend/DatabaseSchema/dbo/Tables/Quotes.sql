CREATE TABLE [dbo].[Quotes] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [OpportunityId] NVARCHAR (100) NULL,
    [VersionId]     INT            NULL,
    [QuoteId]       NVARCHAR (100) NULL,
    [QuoteStatusId] NVARCHAR (100) NULL,
    [Description]   NVARCHAR (250) NULL,
    [IsDeleted]     INT            DEFAULT ((0)) NOT NULL,
    [CreatedBy]     NVARCHAR (100) NULL,
    [CreatedOn]     DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]    NVARCHAR (100) NULL,
    [ModifiedOn]    DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



