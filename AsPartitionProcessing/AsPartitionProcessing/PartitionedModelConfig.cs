using System;
using System.Collections.Generic;

namespace AsPartitionProcessing
{
    /// <summary>
    /// Configuration information for a partitioned AS tabular model.
    /// </summary>
    public class PartitionedModelConfig
    {
        /// <summary>
        /// ID of the PartitionedModelConfig table.
        /// </summary>
        public int PartitionedModelConfigID { get; set; }

        /// <summary>
        /// Name of the Analysis Services instance. Can be SSAS or an Azure AS URL.
        /// </summary>
        public string AnalysisServicesServer { get; set; }

        /// <summary>
        /// Name of the Analysis Services database.
        /// </summary>
        public string AnalysisServicesDatabase { get; set; }

        /// <summary>
        /// True for initial set up to create partitions and process them sequentially. False for incremental processing.
        /// </summary>
        public bool InitialSetUp { get; set; }

        /// <summary>
        /// When initialSetUp=false, determines if processing is performed as an online operation, which may require more memory, but users can still query the model.
        /// </summary>
        public bool IncrementalOnline { get; set; }

        /// <summary>
        /// When initialSetUp=false, determines if separate tables are processed in parallel. Partitions within a table are always processed in parallel.
        /// </summary>
        public bool IncrementalParallelTables { get; set; }

        /// <summary>
        /// Should always set to true for SSAS implementations that will run under the current process account. For Azure AS, normally set to false.
        /// </summary>
        public bool IntegratedAuth { get; set; }

        /// <summary>
        /// Only applies when integratedAuth=false. Used for Azure AD UPNs to connect to Azure AS.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Only applies when integratedAuth=false. Used for Azure AD UPNs to connect to Azure AS.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Collection of partitioned tables containing configuration information.
        /// </summary>
        public List<PartitionedTableConfig> PartitionedTables { get; set; }

        /// <summary>
        /// Connection information to connect to the configuration and logging database.
        /// </summary>
        public ConfigDatabaseConnectionInfo ConfigDatabaseConnectionInfo { get; set; }

        /// <summary>
        /// GUID generated to the execution run.
        /// </summary>
        public string ExecutionID { get; set; }

        /// <summary>
        /// Parameters normally from configuration database to determine partitioning ranges and design.
        /// </summary>
        /// <param name="partitionedModelConfigID">ID of the PartitionedModelConfig table.</param>
        /// <param name="analysisServicesServer">Name of the Analysis Services instance. Can be SSAS or an Azure AS URL.</param>
        /// <param name="analysisServicesDatabase">Name of the Analysis Services database.</param>
        /// <param name="initialSetUp">True for initial set up to create partitions and process them sequentially. False for incremental processing.</param>
        /// <param name="incrementalOnline">When initialSetUp=false, determines if processing is performed as an online operation, which may require more memory, but users can still query the model.</param>
        /// <param name="incrementalParallelTables">When initialSetUp=false, determines if separate tables are processed in parallel. Partitions within a table are always processed in parallel.</param>
        /// <param name="integratedAuth">Should always set to true for SSAS implementations that will run under the current process account. For Azure AS, normally set to false.</param>
        /// <param name="userName">Only applies when integratedAuth=false. Used for Azure AD UPNs to connect to Azure AS.</param>
        /// <param name="password">Only applies when integratedAuth=false. Used for Azure AD UPNs to connect to Azure AS.</param>
        /// <param name="partitionedTables">Collection of partitioned tables containing configuration information.</param>
        public PartitionedModelConfig(
            int partitionedModelConfigID,
            string analysisServicesServer,
            string analysisServicesDatabase,
            bool initialSetUp,
            bool incrementalOnline,
            bool incrementalParallelTables,
            bool integratedAuth,
            string userName,
            string password,
            List<PartitionedTableConfig> partitionedTables
        )
        {
            PartitionedModelConfigID = partitionedModelConfigID;
            AnalysisServicesServer = analysisServicesServer;
            AnalysisServicesDatabase = analysisServicesDatabase;
            InitialSetUp = initialSetUp;
            IncrementalOnline = incrementalOnline;
            IncrementalParallelTables = incrementalParallelTables;
            IntegratedAuth = integratedAuth;
            UserName = userName;
            Password = password;
            PartitionedTables = partitionedTables;
            ExecutionID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PartitionedModelConfig()
        {
            ExecutionID = Guid.NewGuid().ToString();
        }
    }
}
