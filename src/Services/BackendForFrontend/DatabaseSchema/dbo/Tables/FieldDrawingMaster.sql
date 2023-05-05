CREATE TABLE [dbo].[FieldDrawingMaster] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [GroupId]       INT            NOT NULL,
    [OpportunityId] NVARCHAR (100) NOT NULL,
    [StatusKey]     NVARCHAR (100) NULL,
    [DrawingMethod] INT            NULL,
    [CreatedBy]     NVARCHAR (250) NOT NULL,
    [CreatedOn]     DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]    NVARCHAR (250) NULL,
    [ModifiedOn]    DATETIME       NULL,
    [IsDeleted]     BIT            DEFAULT ((0)) NOT NULL,
    [QuoteId]       NVARCHAR (20)  NULL,
    [IsLocked]      BIT            DEFAULT ((0)) NOT NULL,
    [LockedDate]    DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([DrawingMethod]) REFERENCES [dbo].[FieldDrawingMethodMaster] ([Id]),
    FOREIGN KEY ([DrawingMethod]) REFERENCES [dbo].[FieldDrawingMethodMaster] ([Id])
);



