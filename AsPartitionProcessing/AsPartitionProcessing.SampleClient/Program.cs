using System;
using System.Collections.Generic;
using Microsoft.AnalysisServices.Tabular;


namespace AsPartitionProcessing.SampleClient
{
    enum SampleExecutionMode
    {
        InitializeInline,
        InitializeFromDatabase,
        MergePartitions,
        DefragPartitionedTables
    }

    class Program
    {
        //Set sample execution mode here:
        const SampleExecutionMode ExecutionMode = SampleExecutionMode.InitializeInline;

        static void Main(string[] args)
        {
            try
            {
                List<ModelConfiguration> modelsConfig;
                if (ExecutionMode == SampleExecutionMode.InitializeInline)
                {
                    modelsConfig = InitializeInline();
                }
                else
                {
                    modelsConfig = InitializeFromDatabase();
                }

                foreach (ModelConfiguration modelConfig in modelsConfig)
                {
                    if (!modelConfig.IntegratedAuth) //For Azure AS
                    {
                        Console.WriteLine();
                        Console.Write("User name for AS server: ");
                        modelConfig.UserName = Console.ReadLine();
                        Console.Write("Password for AS server: ");
                        modelConfig.Password = ReadPassword();
                    }

                    if (ExecutionMode == SampleExecutionMode.MergePartitions)
                    {
                        PartitionProcessor.MergePartitions(modelConfig, LogMessage, "Internet Sales", Granularity.Yearly, "2012");
                    }
                    else if (ExecutionMode == SampleExecutionMode.DefragPartitionedTables)
                    {
                        PartitionProcessor.DefragPartitionedTables(modelConfig, LogMessage);
                    }
                    else
                    {
                        PartitionProcessor.PerformProcessing(modelConfig, LogMessage);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine(exc.Message, null);
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static List<ModelConfiguration> InitializeInline()
        {
            ModelConfiguration partitionedModel = new ModelConfiguration(
                modelConfigurationID: 1,
                analysisServicesServer: "localhost",
                analysisServicesDatabase: "AdventureWorks",
                initialSetUp: true,
                incrementalOnline: true,
                integratedAuth: true,
                userName: "",
                password: "",
                maxParallelism: -1,
                commitTimeout: -1,
                tableConfigurations:
                new List<TableConfiguration>
                {
                    new TableConfiguration(
                        tableConfigurationID: 1,
                        analysisServicesTable: "Internet Sales",
                        partitioningConfigurations:
                        new List<PartitioningConfiguration>
                        {
                            new PartitioningConfiguration(
                                partitioningConfigurationID: 1,
                                granularity: Granularity.Monthly,
                                numberOfPartitionsFull: 12,
                                numberOfPartitionsForIncrementalProcess: 3,
                                maxDate: Convert.ToDateTime("2012-12-01"),
                                sourceTableName: "[dbo].[FactInternetSales]",
                                sourcePartitionColumn: "OrderDateKey"
                            )
                        }
                    ),
                    new TableConfiguration(
                        tableConfigurationID: 2,
                        analysisServicesTable: "Reseller Sales",
                        partitioningConfigurations:
                        new List<PartitioningConfiguration>
                        {
                            new PartitioningConfiguration(
                                partitioningConfigurationID: 2,
                                granularity: Granularity.Yearly,
                                numberOfPartitionsFull: 3,
                                numberOfPartitionsForIncrementalProcess: 1,
                                maxDate: Convert.ToDateTime("2012-12-01"),
                                sourceTableName: "[dbo].[FactResellerSales]",
                                sourcePartitionColumn: "OrderDateKey"
                            )
                        }
                    )
                }
            );

            #region Not needed as sample includes pre-prepared version of AdventureWorks

            ////This section not to be used normally - just to get started with AdventureWorks. It removes existing partitions that come in AdventureWorks and creates a template one to align with assumptions listed at top of PartitionProcessor.cs file.
            //if (partitionedModel.InitialSetUp)
            //{
            //    Console.WriteLine("Initialize AdventureWorks template partitions? [y/n]");
            //    if (Console.ReadLine().ToLower() == "y")
            //        InitializeAdventureWorksDatabase(partitionedModel);
            //}

            #endregion

            return new List<ModelConfiguration> { partitionedModel };
        }

        private static List<ModelConfiguration> InitializeFromDatabase()
        {
            ConfigDatabaseConnectionInfo connectionInfo = new ConfigDatabaseConnectionInfo();

            connectionInfo.Server = Settings.Default.ConfigServer;
            connectionInfo.Database = Settings.Default.ConfigDatabase;
            connectionInfo.IntegratedAuth = Settings.Default.ConfigDatabaseIntegratedAuth;

            if (!Settings.Default.ConfigDatabaseIntegratedAuth)
            {
                Console.Write("User name for config database: ");
                connectionInfo.UserName = Console.ReadLine();
                Console.Write("Password for config database: ");
                connectionInfo.Password = ReadPassword();
            }

            return ConfigDatabaseHelper.ReadConfig(connectionInfo);
        }

        private static void LogMessage(string message, ModelConfiguration partitionedModel)
        {
            //Can provide custom logger here

            try
            {
                if (!(ExecutionMode == SampleExecutionMode.InitializeInline))
                {
                    ConfigDatabaseHelper.LogMessage(message, partitionedModel);
                }

                Console.WriteLine(message);
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exc.Message);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0); //Avoid recursion if errored connecting to db
            }
        }

        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of password
            Console.WriteLine();
            return password;
        }

        #region Not needed as sample includes pre-prepared version of AdventureWorks

        private static void InitializeAdventureWorksDatabase(ModelConfiguration parameters)
        {
            //In order to align with assumptions listed in PartitionProcessor.cs, need to:
            //1. Delete existing partitions in InternetSales and ResellerSales
            //2. Create template partition (again, see comments at top of PartitionProcessor.cs)

            Console.WriteLine("Initializing AdventureWorks ...");

            using (Server server = new Server())
            {
                //Connect and get main objects
                string serverConnectionString;
                if (parameters.IntegratedAuth)
                    serverConnectionString = $"Provider=MSOLAP;Data Source={parameters.AnalysisServicesServer};";
                else
                {
                    serverConnectionString = $"Provider=MSOLAP;Data Source={parameters.AnalysisServicesServer};User ID={parameters.UserName};Password={parameters.Password};Persist Security Info=True;Impersonation Level=Impersonate;";
                }
                server.Connect(serverConnectionString);

                Database database = server.Databases.FindByName(parameters.AnalysisServicesDatabase);
                if (database == null)
                {
                    throw new Microsoft.AnalysisServices.ConnectionException($"Could not connect to database {parameters.AnalysisServicesDatabase}.");
                }

                InitializeAdventureWorksTable(database, "Internet Sales", "[dbo].[FactInternetSales]");
                InitializeAdventureWorksTable(database, "Reseller Sales", "[dbo].[FactResellerSales]");

                database.Update(Microsoft.AnalysisServices.UpdateOptions.ExpandFull);
                server.Disconnect();
            }
        }

        private static void InitializeAdventureWorksTable(Database database, string analysisServicesTableName, string sourceFactTableName)
        {
            Table table = database.Model.Tables.Find(analysisServicesTableName);
            if (table == null)
            {
                throw new Microsoft.AnalysisServices.ConnectionException($"Could not connect to table {analysisServicesTableName}.");
            }
            table.Partitions.Clear();
            Partition templatePartition = new Partition();
            templatePartition.Name = analysisServicesTableName;
            table.Partitions.Add(templatePartition);
            templatePartition.Source = new QueryPartitionSource()
            {
                DataSource = database.Model.DataSources[0],  //Assuming this is only data source (SqlServer localhost)
                Query = $"SELECT * FROM {sourceFactTableName}"
            };
        }

        #endregion
    }
}

