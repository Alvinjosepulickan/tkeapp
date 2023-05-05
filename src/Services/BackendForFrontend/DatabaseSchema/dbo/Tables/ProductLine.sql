CREATE TABLE [dbo].[ProductLine] (
    [key]         INT           IDENTITY (1, 1) NOT NULL,
    [id]          VARCHAR (250) NOT NULL,
    [name]        VARCHAR (250) NOT NULL,
    [materialId]  VARCHAR (50)  NOT NULL,
    [technology]  VARCHAR (250) NULL,
    [type]        VARCHAR (250) NULL,
    [fit]         VARCHAR (250) NULL,
    [travel]      VARCHAR (250) NULL,
    [description] VARCHAR (MAX) NULL,
    [createdBy]   INT           NULL,
    [createdOn]   DATETIME      NULL,
    [modifiedBy]  INT           NULL,
    [modifiedOn]  DATETIME      NULL,
    [isDeleted]   BIT           NULL
);

