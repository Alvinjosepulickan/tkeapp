CREATE TABLE [dbo].[FieldDrawingIntegrationMaster] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [FieldDrawingIntegrationId] INT            NULL,
    [IntegratedSystemId]        NVARCHAR (250) NULL,
    [IntegratedSystemRef]       NVARCHAR (250) NULL,
    [StatusKey]                 NVARCHAR (100) NOT NULL,
    [CreatedBy]                 NVARCHAR (250) NULL,
    [CreatedOn]                 DATETIME       CONSTRAINT [DF_FieldDrawingIntegrationMaster_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]                NVARCHAR (250) NULL,
    [ModifiedOn]                DATETIME       NULL,
    CONSTRAINT [PK_FDA_IntegrationMaster] PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([FieldDrawingIntegrationId]) REFERENCES [dbo].[FieldDrawingMaster] ([Id]),
    FOREIGN KEY ([FieldDrawingIntegrationId]) REFERENCES [dbo].[FieldDrawingMaster] ([Id]),
    CONSTRAINT [FK_FieldDrawingIntegrationMaster_FieldDrawingIntegrationMaster] FOREIGN KEY ([FieldDrawingIntegrationId]) REFERENCES [dbo].[FieldDrawingMaster] ([Id])
);



