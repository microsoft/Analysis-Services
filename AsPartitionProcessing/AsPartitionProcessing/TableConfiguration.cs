using System;
using System.Collections.Generic;

namespace AsPartitionProcessing
{
    /// <summary>
    /// Configuration information for a table within an AS tabular model.
    /// </summary>
    public class TableConfiguration
    {
        /// <summary>
        /// ID of the TableConfiguration table.
        /// </summary>
        public int TableConfigurationID { get; set; }

        /// <summary>
        /// Name of the partitioned table in the tabular model.
        /// </summary>
        public string AnalysisServicesTable { get; set; }

        /// <summary>
        /// Collection of partitioning configurations.
        /// </summary>
        public List<PartitioningConfiguration> PartitioningConfigurations { get; set; }

        /// <summary>
        /// Initialize configuration info for table. Normally populated from a configuration database.
        /// </summary>
        /// <param name="model">Parent model.</param>
        /// <param name="tableConfigurationID">ID of the TableConfiguration table.</param>
        /// <param name="analysisServicesTable">Name of the partitioned table in the tabular model.</param>
        /// <param name="partitioningConfigurations">Collection of partitioning configurations.</param>
        public TableConfiguration(
            int tableConfigurationID,
            string analysisServicesTable,
            List<PartitioningConfiguration> partitioningConfigurations
            )
        {
            TableConfigurationID = tableConfigurationID;
            AnalysisServicesTable = analysisServicesTable;
            PartitioningConfigurations = partitioningConfigurations;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TableConfiguration()
        {
        }

        /// <summary>
        /// Validate multiple granularities to ensure no overlapping ranges, etc.
        /// </summary>
        public void ValidatePartitioningConfigurations()
        {
            if (this.PartitioningConfigurations.Count > 1)
            {
                this.PartitioningConfigurations.Sort(); //Sorts by LowerBoundary value
                DateTime previousUpperBoundary = DateTime.MinValue;
                Granularity previousGranularity = Granularity.Daily;
                bool foundDaily = false, foundMonthly = false, foundYearly = false;

                foreach (PartitioningConfiguration partitioningConfiguration in this.PartitioningConfigurations)
                {
                    #region Check don't have multiple partitioning configurations with same granularity

                    string multiSameGrainErrorMessage = $"Table {this.AnalysisServicesTable} contains multiple {{0}} partitioning configurations, which is not allowed.";
                    switch (partitioningConfiguration.Granularity)
                    {
                        case Granularity.Daily:
                            if (foundDaily)
                            {
                                throw new InvalidOperationException(string.Format(multiSameGrainErrorMessage, "daily"));
                            }
                            else
                            {
                                foundDaily = true;
                            }
                            break;
                        case Granularity.Monthly:
                            if (foundMonthly)
                            {
                                throw new InvalidOperationException(string.Format(multiSameGrainErrorMessage, "monthly"));
                            }
                            else
                            {
                                foundMonthly = true;
                            }
                            break;
                        case Granularity.Yearly:
                            if (foundYearly)
                            {
                                throw new InvalidOperationException(string.Format(multiSameGrainErrorMessage, "yearly"));
                            }
                            else
                            {
                                foundYearly = true;
                            }
                            break;
                        default:
                            break;
                    }

                    #endregion

                    #region Check don't have overlapping date ranges

                    if (partitioningConfiguration.LowerBoundary <= previousUpperBoundary)
                    {
                        throw new InvalidOperationException($"Table {this.AnalysisServicesTable} contains partitioning configurations with overlapping date ranges, which is not allowed. {previousGranularity.ToString()} upper boundary is {previousUpperBoundary.ToString("yyyy-MM-dd")}; {partitioningConfiguration.Granularity.ToString()} lower boundary is {partitioningConfiguration.LowerBoundary.ToString("yyyy-MM-dd")}.");
                    }
                    else
                    {
                        previousUpperBoundary = partitioningConfiguration.UpperBoundary;
                        previousGranularity = partitioningConfiguration.Granularity;
                    }

                    #endregion
                }
            }
        }
    }
}
