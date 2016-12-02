Microsoft BI technical whitepaper: [Automated Partition Management for Analysis Services Tabular Models](Automated%20Partition%20Management%20for%20Analysis%20Services%20Tabular%20Models.pdf)

Analysis Services tabular models can store data in a highly-compressed, in-memory cache for optimized query performance. This provides fast user interactivity over large data sets. Large datasets normally require table partitioning to accelerate and optimize the data-load process. Partitioning enables incremental loads, increases parallelization, and reduces memory consumption. The Tabular Object Model (TOM) serves as an API to create and manage partitions. Model Compatibility Level 1200 or above is required.

The AsPartitionProcessing TOM code sample is
* Intended to be generic and configuration driven requiring minimal code changes.
* Works for both Azure Analysis Services and SQL Server Analysis Services tabular models.
* Can be leveraged in many ways including from an SSIS script task, Azure Functions and others.
