CREATE TYPE [dbo].[ReleaseInfoDataPoints] AS TABLE (
    [unitJsonVariables] NVARCHAR (MAX) NULL,
    [setId]             NVARCHAR (50)  NULL,
    [saveRelFlag]       NVARCHAR (MAX) NULL,
    [isAcknowledge]     BIT            NULL,
    [releaseComments]   NVARCHAR (MAX) NULL);

