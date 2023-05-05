CREATE TABLE [dbo].[GroupConfigurationDetails] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [GroupId]                 INT            NOT NULL,
    [BuildingId]              INT            NULL,
    [GroupConfigurationType]  NVARCHAR (250) NULL,
    [GroupConfigurationValue] NVARCHAR (250) NULL,
    [CreatedBy]               NVARCHAR (50)  NOT NULL,
    [CreatedOn]               DATETIME       DEFAULT (getdate()) NULL,
    [ModifedBy]               NVARCHAR (50)  NULL,
    [ModifiedOn]              DATETIME       NULL,
    [IsDeleted]               BIT            DEFAULT ((0)) NULL
);

