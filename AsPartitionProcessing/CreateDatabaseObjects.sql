CREATE DATABASE [AsPartitionProcessing]
GO
USE [AsPartitionProcessing]
GO

CREATE TABLE [dbo].[ModelConfiguration](
    [ModelConfigurationID] [int] NOT NULL,
    [AnalysisServicesServer] [varchar](255) NOT NULL,
    [AnalysisServicesDatabase] [varchar](255) NOT NULL,
    [InitialSetUp] [bit] NOT NULL,
    [IncrementalOnline] [bit] NOT NULL,
    [IntegratedAuth] [bit] NOT NULL,
    [MaxParallelism] [int] NOT NULL,
    [CommitTimeout] [int] NOT NULL,
 CONSTRAINT [PK_ModelConfiguration] PRIMARY KEY CLUSTERED 
(
    [ModelConfigurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PartitioningConfiguration](
    [PartitioningConfigurationID] [int] NOT NULL,
    [TableConfigurationID] [int] NOT NULL,
    [Granularity] [tinyint] NOT NULL,
    [NumberOfPartitionsFull] [int] NOT NULL,
    [NumberOfPartitionsForIncrementalProcess] [int] NOT NULL,
    [MaxDateIsNow] [bit] NOT NULL,
    [MaxDate] [date] NULL,
    [IntegerDateKey] [bit] NOT NULL,
    [TemplateSourceQuery] [varchar](max) NOT NULL,
 CONSTRAINT [PK_PartitioningConfiguration] PRIMARY KEY CLUSTERED 
(
    [PartitioningConfigurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ProcessingLog](
    [PartitioningLogID] [int] IDENTITY(1,1) NOT NULL,
    [ModelConfigurationID] [int] NOT NULL,
    [ExecutionID] [char](36) NOT NULL,
    [LogDateTime] [datetime] NOT NULL,
    [Message] [varchar](8000) NOT NULL,
 CONSTRAINT [PK_ProcessingLog] PRIMARY KEY CLUSTERED 
(
    [PartitioningLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TableConfiguration](
    [TableConfigurationID] [int] NOT NULL,
    [ModelConfigurationID] [int] NOT NULL,
    [AnalysisServicesTable] [varchar](255) NOT NULL,
    [DoNotProcess] [bit] NOT NULL,
 CONSTRAINT [PK_TableConfiguration] PRIMARY KEY CLUSTERED 
(
    [TableConfigurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PartitioningConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_PartitioningConfiguration_TableConfiguration] FOREIGN KEY([TableConfigurationID])
REFERENCES [dbo].[TableConfiguration] ([TableConfigurationID])
GO

ALTER TABLE [dbo].[PartitioningConfiguration] CHECK CONSTRAINT [FK_PartitioningConfiguration_TableConfiguration]
GO

ALTER TABLE [dbo].[ProcessingLog]  WITH CHECK ADD  CONSTRAINT [FK_ProcessingLog_ModelConfiguration] FOREIGN KEY([ModelConfigurationID])
REFERENCES [dbo].[ModelConfiguration] ([ModelConfigurationID])
GO

ALTER TABLE [dbo].[ProcessingLog] CHECK CONSTRAINT [FK_ProcessingLog_ModelConfiguration]
GO

ALTER TABLE [dbo].[TableConfiguration] ADD  CONSTRAINT [DF_TableConfiguration_DoNotProcess]  DEFAULT ((0)) FOR [DoNotProcess]
GO

ALTER TABLE [dbo].[TableConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_TableConfiguration_ModelConfiguration] FOREIGN KEY([ModelConfigurationID])
REFERENCES [dbo].[ModelConfiguration] ([ModelConfigurationID])
GO

ALTER TABLE [dbo].[TableConfiguration] CHECK CONSTRAINT [FK_TableConfiguration_ModelConfiguration]
GO

ALTER TABLE [dbo].[PartitioningConfiguration]  WITH CHECK ADD  CONSTRAINT [CK_PartitioningConfiguration_Granularity] CHECK  (([Granularity]=(2) OR [Granularity]=(1) OR [Granularity]=(0)))
GO

ALTER TABLE [dbo].[PartitioningConfiguration] CHECK CONSTRAINT [CK_PartitioningConfiguration_Granularity]
GO

ALTER TABLE [dbo].[PartitioningConfiguration]  WITH CHECK ADD  CONSTRAINT [CK_PartitioningConfiguration_NumberOfPartitionsForIncrementalProcess] CHECK  (([NumberOfPartitionsForIncrementalProcess]<=[NumberOfPartitionsFull]))
GO

ALTER TABLE [dbo].[PartitioningConfiguration] CHECK CONSTRAINT [CK_PartitioningConfiguration_NumberOfPartitionsForIncrementalProcess]
GO



CREATE VIEW [dbo].[vPartitioningConfiguration]
AS
SELECT m.[ModelConfigurationID]
      ,m.[AnalysisServicesServer]
      ,m.[AnalysisServicesDatabase]
      ,m.[InitialSetUp]
      ,m.[IncrementalOnline]
      ,m.[IntegratedAuth]
      ,m.[MaxParallelism]
      ,m.[CommitTimeout]
      ,t.[TableConfigurationID]
      ,t.[AnalysisServicesTable]
      ,t.[DoNotProcess]
      ,CASE
        WHEN p.[TableConfigurationID] IS NULL THEN 0
        ELSE 1
       END [Partitioned]
      ,p.[PartitioningConfigurationID]
      ,p.[Granularity]
      ,p.[NumberOfPartitionsFull]
      ,p.[NumberOfPartitionsForIncrementalProcess]
	  ,p.[MaxDateIsNow]
      ,p.[MaxDate]
      ,p.[IntegerDateKey]
      ,p.[TemplateSourceQuery]
  FROM [dbo].[ModelConfiguration] m
INNER JOIN [dbo].[TableConfiguration] t ON m.[ModelConfigurationID] = t.[ModelConfigurationID]
LEFT JOIN [dbo].[PartitioningConfiguration] p ON t.[TableConfigurationID] = p.[TableConfigurationID]
GO


