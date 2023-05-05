CREATE TYPE [dbo].[ProjectsAccountDetails] AS TABLE (
    [OpportunityId]  NVARCHAR (250) NOT NULL,
    [Type]           NVARCHAR (250) NULL,
    [AddressLine1]   NVARCHAR (100) NULL,
    [AddressLine2]   NVARCHAR (100) NULL,
    [City]           NVARCHAR (250) NULL,
    [State]          NVARCHAR (250) NULL,
    [Country]        NVARCHAR (250) NULL,
    [ZipCode]        NVARCHAR (250) NULL,
    [CustomerNumber] NVARCHAR (100) NULL,
    [AccountName]    NVARCHAR (100) NULL,
    [AwardCloseDate] DATETIME       NULL);

