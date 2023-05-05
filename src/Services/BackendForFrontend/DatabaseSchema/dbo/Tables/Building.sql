CREATE TABLE [dbo].[Building] (
    [Id]                      INT            IDENTITY (1, 1) NOT NULL,
    [ProjectId]               INT            CONSTRAINT [DF_Building_ProjectId] DEFAULT ((0)) NOT NULL,
    [BldName]                 NVARCHAR (50)  NOT NULL,
    [IsDeleted]               BIT            DEFAULT ((0)) NOT NULL,
    [CreatedBy]               NVARCHAR (50)  NOT NULL,
    [CreatedOn]               DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]              NVARCHAR (50)  NULL,
    [ModifiedOn]              DATETIME       NULL,
    [OpportunityId]           NVARCHAR (100) NULL,
    [ConflictStatus]          NVARCHAR (25)  DEFAULT ('Valid') NULL,
    [QuoteId]                 NVARCHAR (20)  NULL,
    [workflowstatus]          NVARCHAR (100) NULL,
    [BuildingEquipmentStatus] NVARCHAR (100) NULL,
    FOREIGN KEY ([BuildingEquipmentStatus]) REFERENCES [dbo].[Status] ([StatusKey]),
    FOREIGN KEY ([workflowstatus]) REFERENCES [dbo].[Status] ([StatusKey])
);



