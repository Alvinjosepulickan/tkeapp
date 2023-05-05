CREATE TABLE [HangFire].[Set] (
    [Key]      NVARCHAR (100) NOT NULL,
    [Score]    FLOAT (53)     NOT NULL,
    [Value]    NVARCHAR (256) NOT NULL,
    [ExpireAt] DATETIME       NULL,
    CONSTRAINT [PK_HangFire_Set] PRIMARY KEY CLUSTERED ([Key] ASC, [Value] ASC)
);




GO



GO


