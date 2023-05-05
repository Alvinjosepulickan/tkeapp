CREATE TABLE [dbo].[AzureGuidBranchMapping] (
    [AzureGuid]    NVARCHAR (36) NOT NULL,
    [BranchNumber] INT           NOT NULL,
    CONSTRAINT [PK_AzureGuidBranchMapping] PRIMARY KEY CLUSTERED ([AzureGuid] ASC)
);

