﻿CREATE TABLE [HangFire].[Hash] (
    [Key]      NVARCHAR (100) NOT NULL,
    [Field]    NVARCHAR (100) NOT NULL,
    [Value]    NVARCHAR (MAX) NULL,
    [ExpireAt] DATETIME2 (7)  NULL,
    CONSTRAINT [PK_HangFire_Hash] PRIMARY KEY CLUSTERED ([Key] ASC, [Field] ASC)
);




GO

