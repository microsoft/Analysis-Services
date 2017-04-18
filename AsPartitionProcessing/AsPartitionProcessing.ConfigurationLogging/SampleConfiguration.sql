INSERT INTO [dbo].[ModelConfiguration]
VALUES(
     1                          --[ModelConfigurationID]
    ,'localhost'                --[AnalysisServicesServer]
    ,'AdventureWorks'           --[AnalysisServicesDatabase]
    ,1                          --[InitialSetUp]
    ,1                          --[IncrementalOnline]
    ,1                          --[IntegratedAuth]
    ,-1                         --[MaxParallelism]
    ,-1                         --[CommitTimeout]
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
    ,0                          --[MaxDateIsNow]
    ,'2012-12-01'               --[MaxDate]
    ,1                          --[IntegerDateKey]
    ,'SELECT * FROM [dbo].[FactInternetSales] WHERE OrderDateKey >= {0} AND OrderDateKey < {1} ORDER BY OrderDateKey' --[TemplateSourceQuery]
),
(
     2                          --[PartitioningConfigurationID]
    ,2                          --[TableConfigurationID]
    ,2                          --[Granularity]   2=Yearly
    ,3                          --[NumberOfPartitionsFull]
    ,1                          --[NumberOfPartitionsForIncrementalProcess]
    ,0                          --[MaxDateIsNow]
    ,'2012-12-01'               --[MaxDate]
    ,1                          --[IntegerDateKey]
    ,'SELECT * FROM [dbo].[FactResellerSales] WHERE OrderDateKey >= {0} AND OrderDateKey < {1} ORDER BY OrderDateKey' --[TemplateSourceQuery]
);
