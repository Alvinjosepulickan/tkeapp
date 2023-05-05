CREATE TABLE [dbo].[AccountDetails] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Type]           NVARCHAR (20)  NULL,
    [AccountName]    NVARCHAR (500) NULL,
    [AddressLine1]   NVARCHAR (100) NULL,
    [City]           NVARCHAR (100) NULL,
    [County]         NVARCHAR (100) NULL,
    [ZipCode]        NVARCHAR (50)  NULL,
    [CustomerNumber] NVARCHAR (100) NULL,
    [AddressLine2]   NVARCHAR (100) NULL,
    [opportunityid]  NVARCHAR (40)  NOT NULL,
    [isDeleted]      BIT            DEFAULT ((0)) NOT NULL,
    [State]          NVARCHAR (100) NULL,
    [AwardCloseDate] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



