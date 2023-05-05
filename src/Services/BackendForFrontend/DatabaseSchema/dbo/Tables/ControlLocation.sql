CREATE TABLE [dbo].[ControlLocation] (
    [ControlLocationId]    INT            IDENTITY (1, 1) NOT NULL,
    [GroupConfigurationId] INT            NOT NULL,
    [ControlLocationType]  NVARCHAR (300) NOT NULL,
    [ControlLocationValue] NVARCHAR (300) NOT NULL,
    [ControlLocationJson]  NVARCHAR (MAX) NULL,
    [CreatedBy]            NVARCHAR (250) NOT NULL,
    [CreatedOn]            DATETIME       NOT NULL,
    [ModifiedBy]           NVARCHAR (250) NULL,
    [ModifiedOn]           DATETIME       NULL,
    [IsDeleted]            BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ControlLocationId] ASC)
);

