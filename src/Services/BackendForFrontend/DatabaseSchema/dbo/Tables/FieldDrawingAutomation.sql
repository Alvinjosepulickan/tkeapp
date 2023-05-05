CREATE TABLE [dbo].[FieldDrawingAutomation] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [FieldDrawingId] INT            NOT NULL,
    [FDAType]        NVARCHAR (300) NOT NULL,
    [FDAValue]       NVARCHAR (500) NULL,
    [CreatedBy]      NVARCHAR (250) NOT NULL,
    [CreatedOn]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]     NVARCHAR (250) NULL,
    [ModifiedOn]     DATETIME       NULL,
    [IsDeleted]      BIT            DEFAULT ((0)) NOT NULL
);

