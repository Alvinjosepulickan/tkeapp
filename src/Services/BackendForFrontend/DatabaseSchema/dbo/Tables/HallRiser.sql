CREATE TABLE [dbo].[HallRiser] (
    [HallRiserId]          INT            IDENTITY (1, 1) NOT NULL,
    [GroupConfigurationId] INT            NOT NULL,
    [UnitId]               INT            NOT NULL,
    [HallRiserType]        NVARCHAR (300) NOT NULL,
    [HallRiserValue]       NVARCHAR (300) NOT NULL,
    [HallRiserJson]        NVARCHAR (MAX) NULL,
    [CreatedBy]            NVARCHAR (250) CONSTRAINT [DF_HallRiser_CreatedBy] DEFAULT (getdate()) NOT NULL,
    [CreatedOn]            DATETIME       CONSTRAINT [DF_HallRiser_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]           NVARCHAR (250) NULL,
    [ModifiedOn]           DATETIME       NULL,
    [IsDeleted]            BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([HallRiserId] ASC)
);

