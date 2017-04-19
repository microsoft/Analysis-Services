using System;
using System.Collections.Generic;

namespace AsPartitionProcessing
{
    /// <summary>
    /// Configuration information for a partitioned AS tabular model.
    /// </summary>
    public class ModelConfiguration
    {
        /// <summary>
        /// ID of the ModelConfiguration table.
        /// </summary>
        public int ModelConfigurationID { get; set; }

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
        /// Sets the maximum number of threads on which to run processing commands in parallel. -1 will not set the value.
        /// </summary>
        public int MaxParallelism { get; set; }

        /// <summary>
        /// Set to override of CommitTimeout server property value for the connection. -1 will not override; the server value will be used.
        /// </summary>
        public int CommitTimeout { get; set; }

        /// <summary>
        /// Number of times a retry of the processing operation will be performed if an error occurs. Use for near-real time scenarios and environments with network reliability issues.
        /// </summary>
        public int RetryAttempts { get; set; }

        /// <summary>
        /// Number of seconds to wait before a retry attempt.
        /// </summary>
        public int RetryWaitTimeSeconds { get; set; }

        /// <summary>
        /// Collection of partitioned tables containing configuration information.
        /// </summary>
        public List<TableConfiguration> TableConfigurations { get; set; }

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
        /// <param name="modelConfigurationID">ID of the ModelConfiguration table.</param>
        /// <param name="analysisServicesServer">Name of the Analysis Services instance. Can be SSAS or an Azure AS URL.</param>
        /// <param name="analysisServicesDatabase">Name of the Analysis Services database.</param>
        /// <param name="initialSetUp">True for initial set up to create partitions and process them sequentially. False for incremental processing.</param>
        /// <param name="incrementalOnline">When initialSetUp=false, determines if processing is performed as an online operation, which may require more memory, but users can still query the model.</param>
        /// <param name="integratedAuth">Should always set to true for SSAS implementations that will run under the current process account. For Azure AS, normally set to false.</param>
        /// <param name="userName">Only applies when integratedAuth=false. Used for Azure AD UPNs to connect to Azure AS.</param>
        /// <param name="password">Only applies when integratedAuth=false. Used for Azure AD UPNs to connect to Azure AS.</param>
        /// <param name="maxParallelism">Sets the maximum number of threads on which to run processing commands in parallel. -1 will not set the value.</param>
        /// <param name="commitTimeout">Set to override of CommitTimeout server property value for the connection. -1 will not override; the server value will be used.</param>
        /// <param name="retryAttempts">Number of times a retry of the processing operation will be performed if an error occurs. Use for near-real time scenarios and environments with network reliability issues.</param>
        /// <param name="retryWaitTimeSeconds">Number of seconds to wait before a retry attempt.</param>
        /// <param name="tableConfigurations">Collection of partitioned tables containing configuration information.</param>
        public ModelConfiguration(
            int modelConfigurationID,
            string analysisServicesServer,
            string analysisServicesDatabase,
            bool initialSetUp,
            bool incrementalOnline,
            bool integratedAuth,
            string userName,
            string password,
            int maxParallelism,
            int commitTimeout,
            int retryAttempts,
            int retryWaitTimeSeconds,
            List<TableConfiguration> tableConfigurations
        )
        {
            ModelConfigurationID = modelConfigurationID;
            AnalysisServicesServer = analysisServicesServer;
            AnalysisServicesDatabase = analysisServicesDatabase;
            InitialSetUp = initialSetUp;
            IncrementalOnline = incrementalOnline;
            IntegratedAuth = integratedAuth;
            UserName = userName;
            Password = password;
            MaxParallelism = maxParallelism;
            CommitTimeout = commitTimeout;
            RetryAttempts = retryAttempts;
            RetryWaitTimeSeconds = retryWaitTimeSeconds;
            TableConfigurations = tableConfigurations;
            ExecutionID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ModelConfiguration()
        {
            ExecutionID = Guid.NewGuid().ToString();
        }
    }
}
