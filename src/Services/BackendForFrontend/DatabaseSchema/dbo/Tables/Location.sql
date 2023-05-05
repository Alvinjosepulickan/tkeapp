CREATE TABLE [dbo].[Location] (
    [key]        INT           IDENTITY (1, 1) NOT NULL,
    [id]         VARCHAR (250) NOT NULL,
    [city]       VARCHAR (50)  NOT NULL,
    [state]      VARCHAR (50)  NOT NULL,
    [country]    VARCHAR (50)  NOT NULL,
    [createdBy]  INT           NULL,
    [createdOn]  DATETIME      NULL,
    [modifiedBy] INT           NULL,
    [modifiedOn] DATETIME      NULL,
    [isDeleted]  BIT           NULL
);

