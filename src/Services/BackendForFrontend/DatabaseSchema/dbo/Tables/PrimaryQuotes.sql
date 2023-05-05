CREATE TABLE [dbo].[PrimaryQuotes] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [OpportunityId]  NVARCHAR (100) NULL,
    [PrimaryQuoteId] NVARCHAR (100) NULL,
    [CreatedBy]      NVARCHAR (100) NULL,
    [CreatedOn]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]     NVARCHAR (100) NULL,
    [ModifiedOn]     DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

