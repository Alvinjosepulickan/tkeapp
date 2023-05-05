CREATE TYPE [dbo].[EntranceConfigurationDataTable] AS TABLE (
    [EntranceConsoleId] NVARCHAR (3)   NULL,
    [VariableType]      NVARCHAR (MAX) NULL,
    [VariableValue]     NVARCHAR (500) NULL);

