CREATE TABLE [dbo].[UnitHallFixtureConfiguration] (
    [UnitHallFixtureConfigurationId] INT            IDENTITY (1, 1) NOT NULL,
    [UnitHallFixtureConsoleId]       INT            NULL,
    [VariableType]                   NVARCHAR (MAX) NOT NULL,
    [VariableValue]                  NVARCHAR (100) NOT NULL,
    [CreatedBy]                      NVARCHAR (250) NOT NULL,
    [CreatedOn]                      DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]                     NVARCHAR (250) NULL,
    [ModifiedOn]                     DATETIME       NULL,
    [IsDeleted]                      BIT            DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([UnitHallFixtureConfigurationId] ASC)
);

