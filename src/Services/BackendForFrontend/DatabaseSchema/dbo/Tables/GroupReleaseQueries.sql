CREATE TABLE [dbo].[GroupReleaseQueries] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [GroupId]       NVARCHAR (50) NULL,
    [queryId]       NVARCHAR (50) NULL,
    [queryName]     NVARCHAR (50) NULL,
    [isAcknowledge] BIT           NULL,
    [createdBy]     NVARCHAR (50) NULL,
    [createdOn]     NVARCHAR (50) DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

