using System;
using System.Collections.Generic;
using Microsoft.AnalysisServices.Tabular;
using System.Diagnostics;

namespace AsPartitionProcessing.SampleClient
{
    #region enum ExecutionMode

    /// <summary>
    /// Execution mode of the SampleClient application.
    /// </summary>
    enum ExecutionMode
    {
        /// <summary>
        /// Initialize configuration inline using sample values.
        /// </summary>
        InitializeInline,
        
        /// <summary>
        /// Initialize from configuration and logging database.
        /// </summary>
        InitializeFromDatabase,

        /// <summary>
        /// Merge partitions in a table based on other parameters.
        /// </summary>
        MergePartitions,

        /// <summary>
        /// Defragment partitioned tables in the model. List of partitioned tables defined in the configuration and logging database.
        /// </summary>
        DefragPartitionedTables
    }

    #endregion

    class Program
    {
        //Set sample execution mode here:
        private static ExecutionMode _executionMode = ExecutionMode.InitializeFromDatabase;
        private static string _modelConfigurationIDs;

        static int Main(string[] args)
        {
            try
            {
                #region Set defaults for merging & read command-line arguments if provided

                string mergeTable = "Internet Sales";
                Granularity mergeTargetGranuarity = Granularity.Yearly;
                string mergePartitionKey = "2012";
                bool help;

                ParseArgs(args, ref mergeTable, ref mergeTargetGranuarity, ref mergePartitionKey, out help);
                if (help)
                    return 0; //ERROR_SUCCESS

                #endregion


                if (_executionMode == ExecutionMode.InitializeInline)
                {
                    //Perform Processing
                    PartitionProcessor.PerformProcessing(InitializeInline(), LogMessage);
                }
                else
                {
                    List<ModelConfiguration> modelsConfig = InitializeFromDatabase();

                    foreach (ModelConfiguration modelConfig in modelsConfig)
                    {
                        SetCredentials(modelConfig); //For Azure AS

                        switch (_executionMode)
                        {
                            case ExecutionMode.InitializeFromDatabase:
                                //Perform Processing
                                PartitionProcessor.PerformProcessing(modelConfig, LogMessage);
                                break;

                            case ExecutionMode.MergePartitions:
                                //Perform Merging
                                PartitionProcessor.MergePartitions(modelConfig, LogMessage, mergeTable, mergeTargetGranuarity, mergePartitionKey);
                                break;

                            case ExecutionMode.DefragPartitionedTables:
                                //Perform Defrag
                                PartitionProcessor.DefragPartitionedTables(modelConfig, LogMessage);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine(exc.Message);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                if (exc is ArgumentException)
                {
                    return 160; //ERROR_BAD_ARGUMENTS
                }
                else
                {
                    return 1360; //ERROR_GENERIC_NOT_MAPPED
                }
            }
            finally
            {
                if (Debugger.IsAttached)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                }
            }

            return 0; //ERROR_SUCCESS
        }

        private static ModelConfiguration InitializeInline()
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
                                maxDateIsNow: false,
                                maxDate: Convert.ToDateTime("2012-12-01"),
                                integerDateKey: true,
                                templateSourceQuery: "SELECT * FROM [dbo].[FactInternetSales] " +
                                    "WHERE OrderDateKey >= {0} AND OrderDateKey < {1} " +
                                    "ORDER BY OrderDateKey"
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
                                maxDateIsNow: false,
                                maxDate: Convert.ToDateTime("2012-12-01"),
                                integerDateKey: true,
                                templateSourceQuery: "SELECT * FROM [dbo].[FactResellerSales] " +
                                    "WHERE OrderDateKey >= {0} AND OrderDateKey < {1} " +
                                    "ORDER BY OrderDateKey"
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

            return partitionedModel;
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

            return ConfigDatabaseHelper.ReadConfig(connectionInfo, _modelConfigurationIDs);
        }

        private static void ParseArgs(string[] args, ref string mergeTable, ref Granularity mergeTargetGranuarity, ref string mergePartitionKey, out bool help)
        {
            help = false;
            if (args.Length > 0)
            {
                ArgumentOptions options = new ArgumentOptions();
                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    if (!String.IsNullOrEmpty(options.ModelConfigurationIDs))
                    {
                        Console.WriteLine($"ModelConfigurationIDs: {options.ModelConfigurationIDs}");
                        _modelConfigurationIDs = options.ModelConfigurationIDs;
                    }

                    Console.WriteLine($"Argument ExecutionMode: {options.ExecutionMode}");
                    switch (options.ExecutionMode)
                    {
                        case "InitializeInline":
                            _executionMode = ExecutionMode.InitializeInline;
                            break;

                        case "InitializeFromDatabase":
                            _executionMode = ExecutionMode.InitializeFromDatabase;
                            break;

                        case "MergePartitions":
                            _executionMode = ExecutionMode.MergePartitions;

                            if (options.MergeTable == null || options.TargetGranularity == null || options.MergePartitionKey == null)
                            {
                                throw new ArgumentException($"ExecutionMode MergePartitions additional arguments not provided or not recognized. Requires --MergeTable, --TargetGranularity, --MergePartitionKey.");
                            }

                            Console.WriteLine($"Argument MergeTable: {options.MergeTable}");
                            Console.WriteLine($"Argument TargetGranularity: {options.TargetGranularity}");
                            Console.WriteLine($"Argument MergePartitionKey: {options.MergePartitionKey}");

                            mergeTable = options.MergeTable;
                            mergeTargetGranuarity = options.TargetGranularity == "Yearly" ? Granularity.Yearly : Granularity.Monthly;
                            mergePartitionKey = options.MergePartitionKey;
                            break;

                        case "DefragPartitionedTables":
                            _executionMode = ExecutionMode.DefragPartitionedTables;
                            break;

                        default:
                            throw new ArgumentException($"Argument --ExecutionMode {options.ExecutionMode} not recognized.");
                            //break;
                    }
                }
                else
                {
                    if (args[0].ToLower() != "--help")
                    {
                        throw new ArgumentException($"Arguments provided not recognized.");
                    }

                    help = true;
                }
            }
        }

        private static void LogMessage(string message, ModelConfiguration partitionedModel)
        {
            //Can provide custom logger here

            try
            {
                if (!(_executionMode == ExecutionMode.InitializeInline))
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
                Console.ForegroundColor = ConsoleColor.White;
                Environment.Exit(0); //Avoid recursion if errored connecting to db
            }
        }

        private static void SetCredentials(ModelConfiguration modelConfig)
        {
            if (!modelConfig.IntegratedAuth) //For Azure AS
            {
                Console.WriteLine();
                Console.Write("User name for AS server: ");
                modelConfig.UserName = Console.ReadLine();
                Console.Write("Password for AS server: ");
                modelConfig.Password = ReadPassword();
            }
        }

        private static string ReadPassword()
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

