using System;

namespace AsPartitionProcessing
{
    /// <summary>
    /// Configuration information for partitioning of a table within an AS tabular model.
    /// </summary>
    public class PartitioningConfiguration : IComparable<PartitioningConfiguration>
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
        /// If MaxDateIsNow = false, the maximum date that needs to be accounted for in the partitioned table.
        /// </summary>
        public DateTime MaxDate { get; set; }

        /// <summary>
        /// Lower boundary for date range covered by partitioning configuration.
        /// </summary>
        public DateTime LowerBoundary { get; }

        /// <summary>
        /// Upper boundary for date range covered by partitioning configuration.
        /// </summary>
        public DateTime UpperBoundary { get; }

        /// <summary>
        /// Assumes date keys are integers. If false assumes date type.
        /// </summary>
        public bool IntegerDateKey { get; set; }

        /// <summary>
        /// Template query used for partition source queries.
        /// </summary>
        public string TemplateSourceQuery { get; set; }

        /// <summary>
        /// Initialize partitioning configuration for partitioned table. Normally populated from a configuration database.
        /// </summary>
        /// <param name="model">Parent model.</param>
        /// <param name="PartitioningConfigurationID">ID of the PartitioningConfiguration table.</param>
        /// <param name="granularity">Partition granularity, which can be Yearly, Monthly or Daily.</param>
        /// <param name="numberOfPartitionsFull">Count of all partitions in the rolling window. For example, a rolling window of 10 years partitioned by month would result in 120 partitions.</param>
        /// <param name="numberOfPartitionsForIncrementalProcess">Count of “hot partitions” where the data can change. For example, it may be necessary to refresh the most recent 3 months of data every day. This only applies to the most recent partitions.</param>
        /// <param name="maxDateIsNow">Assumes maximum date to be accounted for is today.</param>
        /// <param name="maxDate">The maximum date that needs to be accounted for in the partitioned table. Represents the upper boundary of the rolling window.</param>
        /// <param name="integerDateKey">Assumes date keys are integers. If false assumes date type.</param>
        /// <param name="templateSourceQuery">Template query used for partition source queries.</param>
        public PartitioningConfiguration(
            int partitioningConfigurationID,
            Granularity granularity,
            int numberOfPartitionsFull,
            int numberOfPartitionsForIncrementalProcess,
            bool maxDateIsNow,
            DateTime maxDate,
            bool integerDateKey,
            string templateSourceQuery
            )
        {
            PartitioningConfigurationID = partitioningConfigurationID;
            Granularity = granularity;
            NumberOfPartitionsFull = numberOfPartitionsFull;
            NumberOfPartitionsForIncrementalProcess = numberOfPartitionsForIncrementalProcess;
            if (maxDateIsNow)
            {
                MaxDate = DateTime.Today;
            }
            else
            {
                MaxDate = maxDate;
            }
            IntegerDateKey = integerDateKey;
            TemplateSourceQuery = templateSourceQuery;

            switch (granularity)
            {
                case Granularity.Daily:
                    LowerBoundary = MaxDate.AddDays(-numberOfPartitionsFull + 1);
                    UpperBoundary = MaxDate;
                    break;

                case Granularity.Monthly:
                    LowerBoundary = Convert.ToDateTime(MaxDate.AddMonths(-numberOfPartitionsFull + 1).ToString("yyyy-MMM-01"));
                    UpperBoundary = Convert.ToDateTime(MaxDate.AddMonths(1).ToString("yyyy-MMM-01")).AddDays(-1);
                    break;

                case Granularity.Yearly:
                    LowerBoundary = Convert.ToDateTime(MaxDate.AddYears(-numberOfPartitionsFull + 1).ToString("yyyy-01-01"));
                    UpperBoundary = Convert.ToDateTime(MaxDate.AddYears(1).ToString("yyyy-01-01")).AddDays(-1);
                    break;

                default:
                    break;
            }
        }

        public int CompareTo(PartitioningConfiguration other) => string.Compare(this.LowerBoundary.ToString("yyyy-MM-dd"), other.LowerBoundary.ToString("yyyy-MM-dd"));
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
