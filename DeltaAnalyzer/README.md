# DeltaAnalyzer

`Delta Analyzer.ipynb` inspects Delta Lake table layout using Parquet footer metadata plus optional table scans for cardinality analysis. It is designed to help identify layout issues that can affect Fabric, Spark, and semantic model performance.

## How to use

1. Connect the notebook to a Lakehouse. It does not need to be in the same workspace as the table being analyzed.
2. Update the `deltaTable` parameter with the target table name.
3. Optionally set `deltaTableSchema` for schema-enabled lakehouses.
4. Review the other parameters such as overwrite/append and whether to skip cardinality scans.
5. Run the notebook.
6. Review the output tabs and optional Delta/CSV exports.

For best row group quality, consider running:

```sql
OPTIMIZE tablename vorder
```

## Outputs

The notebook can write the following Delta tables:

- `zz_1_DeltaAnalyzerOutput_parquetFiles`
    - One row per Parquet file
    - Useful for file count, V-ORDER metadata, and file-level row counts

- `zz_2_DeltaAnalyzerOutput_rowGroups`
    - One row per row group across all files
    - Useful for checking row group density. A rough target is 1M to 16M rows per row group, higher is usually better

- `zz_3_DeltaAnalyzerOutput_columnChunks`
    - One row per column chunk within each row group
    - Includes compression, dictionary usage, estimated page counts, and dictionary overflow indicators

- `zz_4_DeltaAnalyzerOutput_columns`
    - One row per logical column in the Delta table
    - Includes size contribution, multiple cardinality estimates, and dictionary overflow rollups

The notebook can also export CSV files to the Lakehouse `Files` area for easier download from the Fabric portal.

## Highlights in the current version

- Faster cardinality analysis using a single-pass `approx_count_distinct` query across all columns
- Optional cached precise `COUNT(DISTINCT)` results for comparison
- Metadata-derived cardinality estimates from Parquet min/max and dictionary page information
- Dictionary overflow detection to identify columns likely to have poor dictionary compression
- Rich HTML output with tabs and downloadable CSV for each section
- Optional Delta table export and CSV export for downstream analysis

## Notes

- `columnChunks` analysis uses Parquet metadata only and is typically fast
- Column cardinality scans can still be expensive on very large tables
- If you are working with floating point data and poor compression, consider testing a `DECIMAL(17,4)` representation where appropriate

## Reference

- Parquet Hadoop Javadocs: https://www.javadoc.io/doc/org.apache.parquet/parquet-hadoop/latest/index.html