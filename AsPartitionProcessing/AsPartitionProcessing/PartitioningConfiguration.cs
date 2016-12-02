using System;

namespace AsPartitionProcessing
{
    /// <summary>
    /// Configuration information for partitioning of a table within an AS tabular model.
    /// </summary>
    public class PartitioningConfiguration
    {
        /// <summary>
        /// ID of the PartitioningConfiguration table.
        /// </summary>
        public int PartitioningConfigurationID { get; set; }

        /// <summary>
        /// Partition granularity, which can be Yearly, Monthly or Daily.
        /// </summary>
        public Granularity Granularity { get; set; }

        /// <summary>
        /// Count of all partitions in the rolling window. For example, a rolling window of 10 years partitioned by month would result in 120 partitions.
        /// </summary>
        public int NumberOfPartitionsFull { get; set; }

        /// <summary>
        /// Count of “hot partitions” where the data can change. For example, it may be necessary to refresh the most recent 3 months of data every day. This only applies to the most recent partitions.
        /// </summary>
        public int NumberOfPartitionsForIncrementalProcess { get; set; }

        /// <summary>
        /// The maximum date that needs to be accounted for in the partitioned table. Represents the upper boundary of the rolling window.
        /// </summary>
        public DateTime MaxDate { get; set; }

        /// <summary>
        /// Name of the source table in the relational database.
        /// </summary>
        public string SourceTableName { get; set; }

        /// <summary>
        /// Name of the source column from the table in the relational database.
        /// </summary>
        public string SourcePartitionColumn { get; set; }

        /// <summary>
        /// Initialize partitioning configuration for partitioned table. Normally populated from a configuration database.
        /// </summary>
        /// <param name="model">Parent model.</param>
        /// <param name="PartitioningConfigurationID">ID of the PartitioningConfiguration table.</param>
        /// <param name="granularity">Partition granularity, which can be Yearly, Monthly or Daily.</param>
        /// <param name="numberOfPartitionsFull">Count of all partitions in the rolling window. For example, a rolling window of 10 years partitioned by month would result in 120 partitions.</param>
        /// <param name="numberOfPartitionsForIncrementalProcess">Count of “hot partitions” where the data can change. For example, it may be necessary to refresh the most recent 3 months of data every day. This only applies to the most recent partitions.</param>
        /// <param name="maxDate">The maximum date that needs to be accounted for in the partitioned table. Represents the upper boundary of the rolling window.</param>
        /// <param name="sourceTableName">Name of the source table in the relational database.</param>
        /// <param name="sourcePartitionColumn">Name of the source column from the table in the relational database.</param>
        public PartitioningConfiguration(
            int partitioningConfigurationID,
            Granularity granularity,
            int numberOfPartitionsFull,
            int numberOfPartitionsForIncrementalProcess,
            DateTime maxDate,
            string sourceTableName,
            string sourcePartitionColumn
            )
        {
            PartitioningConfigurationID = partitioningConfigurationID;
            Granularity = granularity;
            NumberOfPartitionsFull = numberOfPartitionsFull;
            NumberOfPartitionsForIncrementalProcess = numberOfPartitionsForIncrementalProcess;
            MaxDate = maxDate;
            SourceTableName = sourceTableName;
            SourcePartitionColumn = sourcePartitionColumn;
        }
    }

    /// <summary>
    /// Enumeration of supported partition granularities.
    /// </summary>
    public enum Granularity
    {
        Daily = 0,
        Monthly = 1,
        Yearly = 2
    }
}
