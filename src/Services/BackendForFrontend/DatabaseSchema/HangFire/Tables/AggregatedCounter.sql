CREATE TABLE [HangFire].[AggregatedCounter] (
    [Key]      NVARCHAR (100) NOT NULL,
    [Value]    BIGINT         NOT NULL,
    [ExpireAt] DATETIME       NULL,
    CONSTRAINT [PK_HangFire_CounterAggregated] PRIMARY KEY CLUSTERED ([Key] ASC)
);




GO


