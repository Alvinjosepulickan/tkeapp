CREATE TYPE [dbo].[MiniProjectsDetailsListValuesData1] AS TABLE (
    [OpportunityId]             NVARCHAR (100) NULL,
    [Name]                      NVARCHAR (250) NULL,
    [BranchValue]               NVARCHAR (100) NULL,
    [SalesId]                   NVARCHAR (100) NULL,
    [QuoteStatus]               NVARCHAR (100) NULL,
    [description]               NVARCHAR (500) NULL,
    [BusinessLine]              NVARCHAR (100) NULL,
    [MeasuringUnit]             NVARCHAR (100) NULL,
    [Salesman]                  NVARCHAR (250) NULL,
    [SalesmanActiveDirectoryID] INT            NULL,
    [country]                   NVARCHAR (100) NULL,
    [CreatedBy]                 NVARCHAR (100) NULL,
    [ProjectJson]               NVARCHAR (MAX) NULL,
    [VersionId]                 INT            NULL,
    [isPrimaryQuote]            BIT            NULL);

