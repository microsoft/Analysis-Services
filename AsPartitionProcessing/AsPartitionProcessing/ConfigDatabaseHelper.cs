using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace AsPartitionProcessing
{
    /// <summary>
    /// Class containing helper methods for reading and writing to the configuration and logging database.
    /// </summary>
    public static class ConfigDatabaseHelper
    {
        /// <summary>
        /// Read configuration information from the database.
        /// </summary>
        /// <param name="connectionInfo">Information required to connect to the configuration and logging database.</param>
        /// <returns>Collection of partitioned models with configuration information.</returns>
        public static List<PartitionedModelConfig> ReadConfig(ConfigDatabaseConnectionInfo connectionInfo)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString(connectionInfo)))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"  
                        SELECT [PartitionedModelConfigID]
                              ,[AnalysisServicesServer]
                              ,[AnalysisServicesDatabase]
                              ,[InitialSetUp]
                              ,[IncrementalOnline]
                              ,[IncrementalParallelTables]
                              ,[IntegratedAuth]
                              ,[PartitionedTableConfigID]
                              ,[MaxDate]
                              ,[Granularity]
                              ,[NumberOfPartitionsFull]
                              ,[NumberOfPartitionsForIncrementalProcess]
                              ,[AnalysisServicesTable]
                              ,[SourceTableName]
                              ,[SourcePartitionColumn]
                          FROM [dbo].[vPartitionedTableConfig]
                          ORDER BY [PartitionedModelConfigID], [PartitionedTableConfigID];";

                    List<PartitionedModelConfig> models = new List<PartitionedModelConfig>();
                    PartitionedModelConfig modelConfig = null;
                    int currentPartitionedModelConfigID = -1;

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (modelConfig == null || currentPartitionedModelConfigID != Convert.ToInt32(reader["PartitionedModelConfigID"]))
                        {
                            modelConfig = new PartitionedModelConfig();
                            modelConfig.PartitionedTables = new List<PartitionedTableConfig>();
                            models.Add(modelConfig);

                            modelConfig.PartitionedModelConfigID = Convert.ToInt32(reader["PartitionedModelConfigID"]);
                            modelConfig.AnalysisServicesServer = Convert.ToString(reader["AnalysisServicesServer"]);
                            modelConfig.AnalysisServicesDatabase = Convert.ToString(reader["AnalysisServicesDatabase"]);
                            modelConfig.InitialSetUp = Convert.ToBoolean(reader["InitialSetUp"]);
                            modelConfig.IncrementalOnline = Convert.ToBoolean(reader["IncrementalOnline"]);
                            modelConfig.IncrementalParallelTables = Convert.ToBoolean(reader["IncrementalParallelTables"]);
                            modelConfig.IntegratedAuth = Convert.ToBoolean(reader["IntegratedAuth"]);
                            modelConfig.ConfigDatabaseConnectionInfo = connectionInfo;

                            currentPartitionedModelConfigID = modelConfig.PartitionedModelConfigID;
                        }

                        modelConfig.PartitionedTables.Add(
                            new PartitionedTableConfig(
                                Convert.ToInt32(reader["PartitionedTableConfigID"]),
                                Convert.ToDateTime(reader["MaxDate"]),
                                (Granularity)Convert.ToInt32(reader["Granularity"]),
                                Convert.ToInt32(reader["NumberOfPartitionsFull"]),
                                Convert.ToInt32(reader["NumberOfPartitionsForIncrementalProcess"]),
                                Convert.ToString(reader["AnalysisServicesTable"]),
                                Convert.ToString(reader["SourceTableName"]),
                                Convert.ToString(reader["SourcePartitionColumn"])
                            )
                        );
                    }

                    return models;
                }
            }
        }

        /// <summary>
        /// Delete all existing logs from the database. Useful in demo scenarios to initialize the database.
        /// </summary>
        /// <param name="connectionInfo">Information required to connect to the configuration and logging database.</param>
        public static void ClearLogTable(ConfigDatabaseConnectionInfo connectionInfo)
        {
            using (var connection = new SqlConnection(GetConnectionString(connectionInfo)))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM [dbo].[PartitionedModelLog];";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Log a message to the databsae.
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        /// <param name="partitionedModel">Partitioned model with configuration information.</param>
        public static void LogMessage(string message, PartitionedModelConfig partitionedModel)
        {
            using (var connection = new SqlConnection(GetConnectionString(partitionedModel.ConfigDatabaseConnectionInfo)))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
                        INSERT INTO [dbo].[PartitionedModelLog]
                               ([PartitionedModelConfigID]
                               ,[ExecutionID]
                               ,[LogDateTime]
                               ,[Message])
                         VALUES
                               (@PartitionedModelConfigID
                               ,@ExecutionID
                               ,@LogDateTime
                               ,@Message);";

                    SqlParameter parameter;

                    parameter = new SqlParameter("@PartitionedModelConfigID", SqlDbType.Int);
                    parameter.Value = partitionedModel.PartitionedModelConfigID;
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter("@ExecutionID", SqlDbType.Char, 36);
                    parameter.Value = partitionedModel.ExecutionID;
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter("@LogDateTime", SqlDbType.DateTime);
                    parameter.Value = DateTime.Now;
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter("@Message", SqlDbType.VarChar, 4000);
                    parameter.Value = message;
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                }
            }
        }

        private static string GetConnectionString(ConfigDatabaseConnectionInfo connectionInfo)
        {
            string connectionString;
            if (connectionInfo.IntegratedAuth)
            {
                connectionString = $"Server={connectionInfo.Server};Database={connectionInfo.Database};Integrated Security=SSPI;";
            }
            else
            {
                connectionString = $"Server={connectionInfo.Server};Database={connectionInfo.Database};User ID={connectionInfo.UserName};Password={connectionInfo.Password};";
            }

            return connectionString;
        }
    }
}
