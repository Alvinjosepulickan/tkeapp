CREATE TABLE [dbo].[ProcedureLog] (
    [LogDate]        SMALLDATETIME  DEFAULT (getdate()) NOT NULL,
    [DatabaseID]     INT            NULL,
    [ObjectID]       INT            NULL,
    [ProcedureName]  NVARCHAR (400) NULL,
    [ErrorLine]      INT            NULL,
    [ErrorMessage]   NVARCHAR (MAX) NULL,
    [AdditionalInfo] NVARCHAR (MAX) NULL
);

