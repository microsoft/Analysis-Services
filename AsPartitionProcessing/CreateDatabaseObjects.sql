CREATE DATABASE [AsPartitionProcessing]
GO
USE [AsPartitionProcessing]
GO

CREATE TABLE [dbo].[PartitionedModelConfig](
    [PartitionedModelConfigID] [int] NOT NULL,
    [AnalysisServicesServer] [varchar](255) NOT NULL,
    [AnalysisServicesDatabase] [varchar](255) NOT NULL,
    [InitialSetUp] [bit] NOT NULL,
    [IncrementalOnline] [bit] NOT NULL,
    [IncrementalParallelTables] [bit] NOT NULL,
    [IntegratedAuth] [bit] NOT NULL,
 CONSTRAINT [PK_PartitionedDatabaseConfig] PRIMARY KEY CLUSTERED 
(
    [PartitionedModelConfigID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PartitionedTableConfig](
    [PartitionedTableConfigID] [int] NOT NULL,
    [PartitionedModelConfigID] [int] NOT NULL,
    [MaxDate] [date] NOT NULL,
    [Granularity] [tinyint] NOT NULL,
    [NumberOfPartitionsFull] [int] NOT NULL,
    [NumberOfPartitionsForIncrementalProcess] [int] NOT NULL,
    [AnalysisServicesTable] [varchar](255) NOT NULL,
    [SourceTableName] [varchar](255) NOT NULL,
    [SourcePartitionColumn] [varchar](255) NOT NULL,
 CONSTRAINT [PK_PartitionedTablesConfig] PRIMARY KEY CLUSTERED 
(
    [PartitionedTableConfigID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PartitionedModelLog](
    [PartitionedModelLogID] [int] IDENTITY(1,1) NOT NULL,
    [PartitionedModelConfigID] [int] NOT NULL,
    [ExecutionID] [char](36) NOT NULL,
    [LogDateTime] [datetime] NOT NULL,
    [Message] [varchar](8000) NOT NULL,
 CONSTRAINT [PK_PartitionedModelLog] PRIMARY KEY CLUSTERED 
(
    [PartitionedModelLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PartitionedTableConfig]  WITH CHECK ADD  CONSTRAINT [FK_PartitionedTableConfig_PartitionedDatabaseConfig] FOREIGN KEY([PartitionedModelConfigID])
REFERENCES [dbo].[PartitionedModelConfig] ([PartitionedModelConfigID])
GO

ALTER TABLE [dbo].[PartitionedTableConfig] CHECK CONSTRAINT [FK_PartitionedTableConfig_PartitionedDatabaseConfig]
GO

ALTER TABLE [dbo].[PartitionedModelLog]  WITH CHECK ADD  CONSTRAINT [FK_PartitionedModelLog_PartitionedModelConfig] FOREIGN KEY([PartitionedModelConfigID])
REFERENCES [dbo].[PartitionedModelConfig] ([PartitionedModelConfigID])
GO

ALTER TABLE [dbo].[PartitionedModelLog] CHECK CONSTRAINT [FK_PartitionedModelLog_PartitionedModelConfig]
GO



CREATE VIEW [dbo].[vPartitionedTableConfig]
AS
SELECT m.[PartitionedModelConfigID]
      ,m.[AnalysisServicesServer]
      ,m.[AnalysisServicesDatabase]
      ,m.[InitialSetUp]
      ,m.[IncrementalOnline]
      ,m.[IncrementalParallelTables]
      ,m.[IntegratedAuth]
      ,t.[PartitionedTableConfigID]
      ,t.[MaxDate]
      ,t.[Granularity]
      ,t.[NumberOfPartitionsFull]
      ,t.[NumberOfPartitionsForIncrementalProcess]
      ,t.[AnalysisServicesTable]
      ,t.[SourceTableName]
      ,t.[SourcePartitionColumn]
  FROM [dbo].[PartitionedTableConfig] t
INNER JOIN [dbo].[PartitionedModelConfig] m ON t.[PartitionedModelConfigID] = m.[PartitionedModelConfigID]
GO

