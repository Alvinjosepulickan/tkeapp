CREATE TABLE [dbo].[BuildingConfiguration] (
    [BldConfigID]   INT            IDENTITY (1, 1) NOT NULL,
    [BuildingId]    INT            NOT NULL,
    [BuindingType]  NVARCHAR (250) NOT NULL,
    [BuindingValue] NVARCHAR (250) NOT NULL,
    [IsDeleted]     BIT            DEFAULT ((0)) NOT NULL,
    [CreatedBy]     NVARCHAR (50)  NOT NULL,
    [CreatedOn]     DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]    NVARCHAR (50)  NULL,
    [ModifiedOn]    DATETIME       NULL,
    CONSTRAINT [Pk_BuildingConfiguration] PRIMARY KEY CLUSTERED ([BldConfigID] ASC)
);

