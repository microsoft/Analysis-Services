


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
      ,m.[RetryAttempts]
      ,m.[RetryWaitTimeSeconds]
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
