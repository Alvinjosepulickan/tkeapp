CREATE TABLE [dbo].[SystemsMasterValues] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [SystemsDescriptionKeys]   NVARCHAR (500) NULL,
    [SystemsDescriptionValues] NVARCHAR (500) NULL,
    [IsDeleted]                BIT            DEFAULT ((0)) NULL,
    [CreatedBy]                NVARCHAR (20)  NULL,
    [CreatedOn]                DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]               NVARCHAR (20)  NULL,
    [ModifiedOn]               DATETIME       DEFAULT (getdate()) NOT NULL,
    [FlagName]                 NVARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



