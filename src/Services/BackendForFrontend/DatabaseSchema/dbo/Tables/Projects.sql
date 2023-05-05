CREATE TABLE [dbo].[Projects] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [OpportunityId]             NVARCHAR (100) NULL,
    [Name]                      NVARCHAR (500) NULL,
    [BranchId]                  INT            NOT NULL,
    [Salesman]                  NVARCHAR (250) NULL,
    [SalesmanActiveDirectoryID] INT            NULL,
    [BusinessLineId]            INT            NULL,
    [MeasuringUnitId]           INT            NULL,
    [IsDeleted]                 BIT            DEFAULT ((0)) NULL,
    [CreatedBy]                 NVARCHAR (100) NULL,
    [CreatedOn]                 DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]                NVARCHAR (100) NULL,
    [ModifiedOn]                DATETIME       DEFAULT (getdate()) NOT NULL,
    [ProjectJson]               NVARCHAR (MAX) NULL,
    [ProjectSource]             INT            NULL,
    [WorkflowStage]             NVARCHAR (100) NULL,
    [BranchNumber]              NVARCHAR (20)  NULL,
    FOREIGN KEY ([WorkflowStage]) REFERENCES [dbo].[Status] ([StatusKey])
);



