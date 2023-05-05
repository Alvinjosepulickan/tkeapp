CREATE TABLE [dbo].[Doors] (
    [DoorId]               INT            IDENTITY (1, 1) NOT NULL,
    [GroupConfigurationId] INT            NOT NULL,
    [UnitId]               INT            NOT NULL,
    [DoorType]             NVARCHAR (300) NOT NULL,
    [DoorValue]            NVARCHAR (300) NOT NULL,
    [DoorJson]             NVARCHAR (MAX) NULL,
    [CreatedBy]            NVARCHAR (250) CONSTRAINT [DF_Doors_CreatedBy] DEFAULT (getdate()) NOT NULL,
    [CreatedOn]            DATETIME       NOT NULL,
    [ModifiedBy]           NVARCHAR (250) NULL,
    [ModifiedOn]           DATETIME       NULL,
    [IsDeleted]            BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([DoorId] ASC)
);

