INSERT INTO [dbo].[ModelConfiguration]
VALUES(
     1                          --[ModelConfigurationID]
    ,'localhost'                --[AnalysisServicesServer]
    ,'AdventureWorks'           --[AnalysisServicesDatabase]
    ,1                          --[InitialSetUp]
    ,1                          --[IncrementalOnline]
    ,1                          --[IncrementalParallelTables]
    ,1                          --[IntegratedAuth]
);

INSERT INTO [dbo].[TableConfiguration]
VALUES(
     1                          --[TableConfigurationID]
    ,1                          --[ModelConfigurationID]
    ,'Internet Sales'           --[AnalysisServicesTable]
    ,0                          --[DoNotProcess]
),
(
     2                          --[TableConfigurationID]
    ,1                          --[ModelConfigurationID]
    ,'Reseller Sales'           --[AnalysisServicesTable]
    ,0                          --[DoNotProcess]
);

INSERT INTO [dbo].[PartitioningConfiguration]
VALUES(
     1                          --[PartitioningConfigurationID]
    ,1                          --[TableConfigurationID]
    ,1                          --[Granularity]   1=Monthly
    ,12                         --[NumberOfPartitionsFull]
    ,3                          --[NumberOfPartitionsForIncrementalProcess]
    ,'2012-12-01'               --[MaxDate]
    ,'[dbo].[FactInternetSales]'--[SourceTableName]
    ,'OrderDateKey'             --[SourcePartitionColumn]
),
(
     2                          --[PartitioningConfigurationID]
    ,2                          --[TableConfigurationID]
    ,2                          --[Granularity]   2=Yearly
    ,3                          --[NumberOfPartitionsFull]
    ,1                          --[NumberOfPartitionsForIncrementalProcess]
    ,'2012-12-01'               --[MaxDate]
    ,'[dbo].[FactResellerSales]'--[SourceTableName]
    ,'OrderDateKey'             --[SourcePartitionColumn]
);
