CREATE TABLE [dbo].[SystemsVariables] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [SetId]                INT            NULL,
    [StatusKey]            NVARCHAR (20)  NULL,
    [SystemVariableKeys]   NVARCHAR (500) NULL,
    [SystemVariableValues] NVARCHAR (500) NULL,
    [IsDeleted]            BIT            DEFAULT ((0)) NULL,
    [CreatedBy]            NVARCHAR (100) NULL,
    [CreatedOn]            DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]           NVARCHAR (100) NULL,
    [ModifiedOn]           DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

