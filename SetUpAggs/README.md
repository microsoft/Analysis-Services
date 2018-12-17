# SetUpAggs

This utility is designed to assist in updating an Analysis Services and/or Power BI data model to utilize the Composite Model and Aggregations features.  The utility will connect to a specified Azure Analysis Services and/or Power BI instance and read in a configuration file, this configuration file will specify the database and associated tables to update the Composite Model and Aggregations properties.

When running this utility against an Azure Analysis Services model, it will upgrade the model to Compatibility Level 1465, this change is not reversible via this utility.  The utility will only update the metadata of tables listed in the configuration file, but it will both add and remove Aggregation column definitions, so that the model metadata will match the configuration file.  Partition mode changes will be applied to all the partitions defined on the table.

## Example Usage
SetUpAggs.exe apply -Server asazure://server/instance -ConfigFile AggsConfig.json

## Steps
* Parse command line arguments, show basic help  
* Connect to model  
* Validate configuration file against the model, exit with relevant error message if issue is found in configuration or model  
* Update model compatibility level if current compatibility level is below 1465  
* Apply configuration changes to model  
   * For each table  
      * For each partition, update partition mode to Dual, DirectQuery, Import, or Default as specified in config  
      * For each column, remove any AlternateOf definitions that do not match an aggregation rule in the config  
      * For each rule, add the AlternateOf definition to the model if it does not already exist  
* If changes were made to the model, run the ExpandFull on the model  
* If changes were made to the model, refresh the tables that were modified  

## Configuration File Example

```json
{
	"database": {
		"name": "AdventureWorksAggsProvider",
		"tables": [
			{   
				"name": "DimGeography",
				"mode": "Dual"
			},   
			{   
				"name": "DimCustomer",
				"mode": "Dual"
			},
			{   
				"name": "DimDate",
				"mode": "Dual"
			},
			{   
				"name": "DimProductSubcategory",
				"mode": "Dual"
			},
			{   
				"name": "DimProductCategory",
				"mode": "Dual"
			},
			{   
				"name": "FactInternetSalesAgg",
				"mode": "Import",
				"aggregationRules": [
					{
						"aggTableColumn": "OrderDateKey",
						"summarization": "GroupBy",
						"detailTable": "FactInternetSales",
						"detailTableColumn": "OrderDateKey"
					},
					{
						"aggTableColumn": "CustomerKey",
						"summarization": "GroupBy",
						"detailTable": "FactInternetSales",
						"detailTableColumn": "CustomerKey"
					},
					{
						"aggTableColumn": "ProductSubcategoryKey",
						"summarization": "GroupBy",
						"detailTable": "DimProduct",
						"detailTableColumn": "ProductSubcategoryKey"
					},
					{
						"aggTableColumn": "SalesAmount_Sum",
						"summarization": "Sum",
						"detailTable": "FactInternetSales",
						"detailTableColumn": "SalesAmount"
					},
					{
						"aggTableColumn": "UnitPrice_Sum",
						"summarization": "Sum",
						"detailTable": "FactInternetSales",
						"detailTableColumn": "UnitPrice"
					},
					{
						"aggTableColumn": "FactInternetSales_Count",
						"summarization": "CountTableRows",
						"detailTable": "FactInternetSales"
					}
				]
			}
		]		
	}
}
```




