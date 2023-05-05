CREATE TABLE [dbo].[FieldDrawingMethodMaster] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [DrawingMethod] NVARCHAR (500) NULL,
    [CreatedBy]     NVARCHAR (250) NOT NULL,
    [CreatedOn]     DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]    NVARCHAR (250) NULL,
    [ModifiedOn]    DATETIME       NULL,
    [IsDeleted]     BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

