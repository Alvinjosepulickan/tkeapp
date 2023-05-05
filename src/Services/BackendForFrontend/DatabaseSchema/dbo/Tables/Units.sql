CREATE TABLE [dbo].[Units] (
    [UnitId]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]                 NVARCHAR (20)  NULL,
    [Location]             NVARCHAR (8)   NOT NULL,
    [UnitJson]             NVARCHAR (MAX) NULL,
    [GroupConfigurationId] INT            NOT NULL,
    [SetId]                INT            DEFAULT ((0)) NULL,
    [CreatedBy]            NVARCHAR (250) NOT NULL,
    [CreatedOn]            DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]           NVARCHAR (250) NULL,
    [ModifiedOn]           DATETIME       NULL,
    [IsDeleted]            BIT            DEFAULT ((0)) NULL,
    [Designation]          NVARCHAR (50)  NULL,
    [Description]          NVARCHAR (50)  NULL,
    [UEID]                 NVARCHAR (16)  DEFAULT ('') NULL,
    [OccupiedSpaceBelow]   BIT            DEFAULT ((0)) NULL,
    [IsFutureElevator]     BIT            NULL,
    [MappedLocation]       NVARCHAR (8)   NULL,
    [MappedLocationJson]   NVARCHAR (MAX) NULL,
    [ConflictStatus]       NVARCHAR (25)  DEFAULT ('Valid') NULL,
    [WorkflowStatus]       NVARCHAR (100) NULL,
    [releaseComments]      NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([UnitId] ASC),
    FOREIGN KEY ([WorkflowStatus]) REFERENCES [dbo].[Status] ([StatusKey])
);



