CREATE TABLE [dbo].[CoordinationQuestions] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [QuoteId]           NVARCHAR (100) NOT NULL,
    [OpportunityId]     NVARCHAR (100) NOT NULL,
    [GroupId]           INT            NOT NULL,
    [QuestionnaireJson] NVARCHAR (MAX) NOT NULL,
    [CreatedBy]         NVARCHAR (250) NOT NULL,
    [CreatedOn]         DATETIME       CONSTRAINT [DF_CoordinationQuestions_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]        NVARCHAR (250) NULL,
    [ModifiedOn]        DATETIME       NULL,
    [IsDeleted]         BIT            CONSTRAINT [DF_CoordinationQuestions_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CoordinationQuestions] PRIMARY KEY CLUSTERED ([Id] ASC)
);

