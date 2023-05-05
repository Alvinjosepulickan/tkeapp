CREATE TABLE [dbo].[UnitSet] (
    [SetId]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (20)  NOT NULL,
    [Description] NVARCHAR (50)  NOT NULL,
    [ProductName] NVARCHAR (MAX) NOT NULL,
    [CreatedBy]   NVARCHAR (250) NOT NULL,
    [CreatedOn]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]  NVARCHAR (250) NULL,
    [ModifiedOn]  NVARCHAR (250) NULL,
    [IsDeleted]   BIT            DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([SetId] ASC)
);

