CREATE TABLE [dbo].[DeviceSlotsConfiguration] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [UnitId]              INT            NOT NULL,
    [DeviceSlotVariables] NVARCHAR (MAX) NOT NULL,
    [Value]               NVARCHAR (MAX) NOT NULL,
    [CreatedBy]           NVARCHAR (250) NOT NULL,
    [CreatedOn]           DATETIME       NOT NULL,
    [ModifiedBy]          NVARCHAR (250) NULL,
    [ModifiedOn]          NVARCHAR (250) NULL,
    [IsDeleted]           BIT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

