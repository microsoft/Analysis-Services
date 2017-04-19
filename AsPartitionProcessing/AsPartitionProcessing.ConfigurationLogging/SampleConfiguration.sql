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
	,0							--[RetryAttempts]
	,0							--[RetryWaitTimeSeconds]
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
    ,
'let
    Source = #"AdventureWorks",
    dbo_FactInternetSales = Source{[Schema="dbo",Item="FactInternetSales"]}[Data],
    #"Filtered Rows" = Table.SelectRows(dbo_FactInternetSales, each [OrderDateKey] >= {0} and [OrderDateKey] < {1}),
    #"Sorted Rows" = Table.Sort(#"Filtered Rows",{{"OrderDateKey", Order.Ascending}})
in
    #"Sorted Rows"'             --[TemplateSourceQuery]
),
(
     2                          --[PartitioningConfigurationID]
    ,2                          --[TableConfigurationID]
    ,2                          --[Granularity]   2=Yearly
    ,3                          --[NumberOfPartitionsFull]
    ,1                          --[NumberOfPartitionsForIncrementalProcess]
    ,0                          --[MaxDateIsNow]
    ,'2012-12-01'               --[MaxDate]
    ,0                          --[IntegerDateKey]
    ,
'let
    Source = #"AdventureWorks",
    dbo_FactResellerSales = Source{[Schema="dbo",Item="FactResellerSales"]}[Data],
    #"Filtered Rows" = Table.SelectRows(dbo_FactResellerSales, each [OrderDate] >= {0} and [OrderDate] < {1}),
    #"Sorted Rows" = Table.Sort(#"Filtered Rows",{{"OrderDate", Order.Ascending}})
in
    #"Sorted Rows"'             --[TemplateSourceQuery]
);
