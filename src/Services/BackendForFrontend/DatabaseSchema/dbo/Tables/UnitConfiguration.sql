CREATE TABLE [dbo].[UnitConfiguration] (
    [UnitConfigurationId] INT            IDENTITY (1, 1) NOT NULL,
    [SetId]               INT            NULL,
    [ConfigureVariables]  NVARCHAR (MAX) NOT NULL,
    [ConfigureValues]     NVARCHAR (MAX) NOT NULL,
    [ConfigureJson]       NVARCHAR (MAX) NOT NULL,
    [CreatedBy]           NVARCHAR (250) NOT NULL,
    [CreatedOn]           DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]          NVARCHAR (250) NULL,
    [ModifiedOn]          NVARCHAR (250) NULL,
    [IsDeleted]           BIT            DEFAULT ((0)) NULL,
    [isAcknowledge]       BIT            NULL,
    PRIMARY KEY CLUSTERED ([UnitConfigurationId] ASC)
);



