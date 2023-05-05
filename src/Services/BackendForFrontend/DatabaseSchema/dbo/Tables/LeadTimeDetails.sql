CREATE TABLE [dbo].[LeadTimeDetails] (
    [LeadTimeId]    INT            IDENTITY (1, 1) NOT NULL,
    [SetId]         INT            NULL,
    [VariableId]    NVARCHAR (MAX) NOT NULL,
    [VariableValue] NVARCHAR (MAX) NOT NULL,
    [CreatedBy]     NVARCHAR (250) NOT NULL,
    [CreatedOn]     DATETIME       NOT NULL,
    [ModifiedBy]    NVARCHAR (250) NULL,
    [ModifiedOn]    NVARCHAR (250) NULL,
    [IsDeleted]     BIT            DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([LeadTimeId] ASC),
    FOREIGN KEY ([SetId]) REFERENCES [dbo].[UnitSet] ([SetId])
);

