CREATE TABLE [dbo].[Entity] (
    [EntityId]   INT           IDENTITY (1, 1) NOT NULL,
    [EntityName] VARCHAR (100) NOT NULL,
    [CreatedBy]  VARCHAR (MAX) NULL,
    [CreatedOn]  DATETIME      NULL,
    [ModifiedBy] VARCHAR (MAX) NULL,
    [ModifiedOn] DATETIME      NULL,
    [IsDeleted]  BIT           DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([EntityId] ASC)
);

