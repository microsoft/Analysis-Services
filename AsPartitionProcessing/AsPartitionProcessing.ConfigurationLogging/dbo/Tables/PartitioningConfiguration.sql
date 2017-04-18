CREATE TABLE [dbo].[PartitioningConfiguration] (
    [PartitioningConfigurationID]             INT           NOT NULL,
    [TableConfigurationID]                    INT           NOT NULL,
    [Granularity]                             TINYINT       NOT NULL,
    [NumberOfPartitionsFull]                  INT           NOT NULL,
    [NumberOfPartitionsForIncrementalProcess] INT           NOT NULL,
    [MaxDateIsNow]                            BIT           NOT NULL,
    [MaxDate]                                 DATE          NULL,
    [IntegerDateKey]                          BIT           NOT NULL,
    [TemplateSourceQuery]                     VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_PartitioningConfiguration] PRIMARY KEY CLUSTERED ([PartitioningConfigurationID] ASC),
    CONSTRAINT [CK_PartitioningConfiguration_Granularity] CHECK ([Granularity]=(2) OR [Granularity]=(1) OR [Granularity]=(0)),
    CONSTRAINT [CK_PartitioningConfiguration_NumberOfPartitionsForIncrementalProcess] CHECK ([NumberOfPartitionsForIncrementalProcess]<=[NumberOfPartitionsFull]),
    CONSTRAINT [FK_PartitioningConfiguration_TableConfiguration] FOREIGN KEY ([TableConfigurationID]) REFERENCES [dbo].[TableConfiguration] ([TableConfigurationID])
);

