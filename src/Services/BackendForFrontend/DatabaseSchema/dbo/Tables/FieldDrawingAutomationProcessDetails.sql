CREATE TABLE [dbo].[FieldDrawingAutomationProcessDetails] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [IntegratedProcessId] INT            NULL,
    [FDAProcessJson]      NVARCHAR (MAX) NULL,
    [CreatedBy]           NVARCHAR (250) NULL,
    [CreatedOn]           DATETIME       CONSTRAINT [DF_FieldDrawingAutomationProcessDetails_CreatedOn] DEFAULT (getdate()) NULL,
    [ModifiedBy]          NVARCHAR (250) NULL,
    [ModifiedOn]          DATETIME       NULL,
    CONSTRAINT [PK_FieldDrawingAutomationProcessDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([IntegratedProcessId]) REFERENCES [dbo].[FieldDrawingIntegrationMaster] ([Id])
);



