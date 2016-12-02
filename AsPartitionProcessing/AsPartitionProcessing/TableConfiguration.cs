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
    }
}
