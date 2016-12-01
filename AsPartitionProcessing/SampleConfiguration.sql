INSERT INTO [dbo].[PartitionedModelConfig]
VALUES(
     1                          --[PartitionedModelConfigID]
    ,'localhost'                --[AnalysisServicesServer]
    ,'AdventureWorks'           --[AnalysisServicesDatabase]
    ,1                          --[InitialSetUp]
    ,1                          --[IncrementalOnline]
    ,1                          --[IncrementalParallelTables]
    ,1                          --[IntegratedAuth]
);

INSERT INTO [dbo].[PartitionedTableConfig]
VALUES(
     1                          --[PartitionedTableConfigID]
    ,1                          --[PartitionedModelConfigID]
    ,'2012-12-01'               --[MaxDate]
    ,1                          --[Granularity]   1=Monthly
    ,12                         --[NumberOfPartitionsFull]
    ,3                          --[NumberOfPartitionsForIncrementalProcess]
    ,'Internet Sales'           --[AnalysisServicesTable]
    ,'[dbo].[FactInternetSales]'--[SourceTableName]
    ,'OrderDateKey'             --[SourcePartitionColumn]
),
(
     2                          --[PartitionedTableConfigID]
    ,1                          --[PartitionedModelConfigID]
    ,'2012-12-01'               --[MaxDate]
    ,2                          --[Granularity]   2=Yearly
    ,3                          --[NumberOfPartitionsFull]
    ,1                          --[NumberOfPartitionsForIncrementalProcess]
    ,'Reseller Sales'           --[AnalysisServicesTable]
    ,'[dbo].[FactResellerSales]'--[SourceTableName]
    ,'OrderDateKey'             --[SourcePartitionColumn]
);
