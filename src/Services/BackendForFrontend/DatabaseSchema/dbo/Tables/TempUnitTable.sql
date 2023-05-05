CREATE TABLE [dbo].[TempUnitTable] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [UnitName]             NVARCHAR (250) NOT NULL,
    [GroupConfigurationId] INT            NOT NULL,
    [OldUnitId]            INT            NULL,
    [NewUnitId]            INT            NULL,
    [SetId]                INT            DEFAULT ((0)) NULL,
    [UEID]                 NVARCHAR (16)  DEFAULT ('') NULL,
    [Designation]          NVARCHAR (50)  DEFAULT ('') NULL,
    [Description]          NVARCHAR (50)  DEFAULT ('') NULL,
    [WorkflowStatus]       NVARCHAR (100) NULL
);



