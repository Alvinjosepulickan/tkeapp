CREATE TYPE [dbo].[HistoryTable] AS TABLE (
    [VariableId]    NVARCHAR (MAX) NULL,
    [UpdatedValue]  NVARCHAR (MAX) NULL,
    [PreviousValue] NVARCHAR (MAX) NULL);

