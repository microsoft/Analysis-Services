# DeltaAnalyzer

    Instructions:
    - Connect Notebook to Lakehouse (does not have to be in same Workspace)
    - Update deltaTable parameter immedately below instructions with name of table to analyze
    - Review other parameters (append vs overwrite) 
    - Run
    - Review four output tables that have "zz_n_DeltaAnalyzerOutput"

    zz_1_DeltaAnalyzerOutput_parquetFiles
        This table has one row per Parquet file
        Ideally, there should not be thousands of these
        This table only uses parquet file metadata and should be quick to populate

    zz_2_DeltaAnalyzerOutput_rowRowgroups
        This table has one row per rowgroup and shows rowgroups for every parquet file
        Look for the number of rows per rowgroup.  Ideally this should be 1M to 16M rows (higher the better)
        This table only uses parquet file metadata and should be quick to populate

    zz_3_DeltaAnalyzerOutput_columnChunks
        One row per column/chunk within rowgroups
        Large number of output and has much more detail about dictionaries and compression
        This table only uses parquet file metadata and should be quick to populate
    
    zz_4_DeltaAnalyzerOutput_columns
        One row per column of the table
        Look to see how many unique values per column.  If using floating point, consider modifying parquet file to use DECIMAL(17,4)
        This table runs a compute query against the Detla table so may take time depending on size of Delta table

    Run 
    %%sql
    OPTIMIZE tablename vorder 


    Footnote:
        Useful doc
        https://www.javadoc.io/doc/org.apache.parquet/parquet-hadoop/latest/index.html