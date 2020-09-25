using System;
using System.Collections.Generic;
using Microsoft.AnalysisServices.Tabular;
using System.Threading;


//-----------
//ASSUMPTIONS
//Rolling window. Removes oldest partition on increment
//Depends on date keys in source to be integers formatted as yyyymmdd
//Source queries take the form "SELECT * FROM <SourceTable> WHERE FLOOR(<SourceColumn> / 100) = <yyyymm>" (monthly)
//Template partition exists with same name as table
//Non-template partitions have name of the format yyyy (yearly), yyyymm (monthly), yyyymmdd (daily)
//-----------


namespace AsPartitionProcessing
{
    /// <summary>
    /// Delegate to allow client to pass in a custom logging method
    /// </summary>
    /// <param name="message">The message to be logged</param>
    /// <param name="modelConfiguration">Configuration info for the model</param>
    public delegate void LogMessageDelegate(string message, MessageType messageType, ModelConfiguration modelConfiguration);

    /// <summary>
    /// Processor of partitions in AS tabular models
    /// </summary>
    public static class PartitionProcessor
    {
        #region Member Variables

        private static ModelConfiguration _modelConfiguration;
        private static LogMessageDelegate _messageLogger;
        private static int _retryAttempts;

        #endregion

        #region Public Methods

        /// <summary>
        /// Partitions tables in a tabular model based on configuration
        /// </summary>
        /// <param name="modelConfiguration">Configuration info for the model</param>
        /// <param name="messageLogger">Pointer to logging method</param>
        public static void PerformProcessing(ModelConfiguration modelConfiguration, LogMessageDelegate messageLogger)
        {
            _modelConfiguration = modelConfiguration;
            _messageLogger = messageLogger;
            _retryAttempts = modelConfiguration.RetryAttempts;

            PerformProcessing();
        }

        private static void PerformProcessing()
        {
            Server server = new Server();
            try
            {
                Database database;
                Connect(server, out database);

                Console.ForegroundColor = ConsoleColor.White;
                LogMessage($"Start: {DateTime.Now.ToString("hh:mm:ss tt")}", MessageType.Informational, false);
                LogMessage($"Server: {_modelConfiguration.AnalysisServicesServer}", MessageType.Informational, false);
                LogMessage($"Database: {_modelConfiguration.AnalysisServicesDatabase}", MessageType.Informational, false);
                Console.ForegroundColor = ConsoleColor.Yellow;

                foreach (TableConfiguration tableConfiguration in _modelConfiguration.TableConfigurations)
                {
                    Table table = database.Model.Tables.Find(tableConfiguration.AnalysisServicesTable);
                    if (table == null)
                    {
                        throw new Microsoft.AnalysisServices.ConnectionException($"Could not connect to table {tableConfiguration.AnalysisServicesTable}.");
                    }

                    if (tableConfiguration.PartitioningConfigurations.Count == 0)
                    {
                        //Non-partitioned table. Process at table level.
                        LogMessage("", MessageType.Informational, false);
                        LogMessage($"Non-partitioned processing for table {tableConfiguration.AnalysisServicesTable}", MessageType.Informational, false);
                        LogMessage(new String('-', tableConfiguration.AnalysisServicesTable.Length + 37), MessageType.Informational, false);

                        if (_modelConfiguration.IncrementalOnline)
                        {
                            LogMessage($"Process table {tableConfiguration.AnalysisServicesTable} /Full", MessageType.Informational, true);
                            table.RequestRefresh(RefreshType.Full);
                        }
                        else
                        {
                            LogMessage($"Process table {tableConfiguration.AnalysisServicesTable} /DataOnly", MessageType.Informational, true);
                            table.RequestRefresh(RefreshType.DataOnly);
                        }
                    }
                    else
                    {
                        //Validate multiple granularity ranges.
                        tableConfiguration.ValidatePartitioningConfigurations();

                        //Find template partition.
                        Partition templatePartition = table.Partitions.Find(tableConfiguration.AnalysisServicesTable);
                        if (templatePartition == null)
                        {
                            throw new InvalidOperationException($"Table {tableConfiguration.AnalysisServicesTable} does not contain a partition with the same name to act as the template partition.");
                        }

                        //Process based on partitioning configuration(s).
                        foreach (PartitioningConfiguration partitioningConfiguration in tableConfiguration.PartitioningConfigurations)
                        {
                            LogMessage("", MessageType.Informational, false);
                            LogMessage($"Rolling-window partitioning for table {tableConfiguration.AnalysisServicesTable}", MessageType.Informational, false);
                            LogMessage(new String('-', tableConfiguration.AnalysisServicesTable.Length + 38), MessageType.Informational, false);

                            //Figure out what processing needs to be done
                            List<string> partitionKeysCurrent = GetPartitionKeysCurrent(table, partitioningConfiguration.Granularity);
                            List<string> partitionKeysNew = GetPartitionKeysTarget(false, partitioningConfiguration, partitioningConfiguration.Granularity);
                            List<string> partitionKeysForProcessing = GetPartitionKeysTarget(true, partitioningConfiguration, partitioningConfiguration.Granularity);
                            DisplayPartitionRange(partitionKeysCurrent, true, partitioningConfiguration.Granularity);
                            DisplayPartitionRange(partitionKeysNew, false, partitioningConfiguration.Granularity);
                            LogMessage("", MessageType.Informational, false);
                            LogMessage("=>Actions & progress:", MessageType.Informational, false);

                            //Check for old partitions that need to be removed
                            foreach (string partitionKey in partitionKeysCurrent)
                            {
                                if (Convert.ToInt32(partitionKey) < Convert.ToInt32(partitionKeysNew[0]))
                                {
                                    LogMessage($"Remove old partition       {DateFormatPartitionKey(partitionKey, partitioningConfiguration.Granularity)}", MessageType.Informational, true);
                                    table.Partitions.Remove(partitionKey);
                                }
                            }

                            //Process partitions
                            foreach (string partitionKey in partitionKeysForProcessing)
                            {
                                Partition partitionToProcess = table.Partitions.Find(partitionKey);

                                if (partitionToProcess == null)
                                {
                                    partitionToProcess = CreateNewPartition(table, templatePartition, partitioningConfiguration, partitionKey, partitioningConfiguration.Granularity);
                                    LogMessage($"Create new partition       {DateFormatPartitionKey(partitionKey, partitioningConfiguration.Granularity)}", MessageType.Informational, true);

                                    if (!_modelConfiguration.InitialSetUp)
                                    {
                                        IncrementalProcessPartition(partitionKey, partitionToProcess, partitioningConfiguration.Granularity);
                                    }
                                }
                                else if (!_modelConfiguration.InitialSetUp)
                                {
                                    //Existing partition for processing
                                    IncrementalProcessPartition(partitionKey, partitionToProcess, partitioningConfiguration.Granularity);
                                }

                                if (_modelConfiguration.InitialSetUp)
                                {
                                    if (partitionToProcess.State != ObjectState.Ready)
                                    {
                                        //Process new partitions sequentially during initial setup so don't run out of memory
                                        LogMessage($"Sequentially process       {DateFormatPartitionKey(partitionKey, partitioningConfiguration.Granularity)} /DataOnly", MessageType.Informational, true);
                                        partitionToProcess.RequestRefresh(RefreshType.DataOnly);
                                        database.Model.SaveChanges();
                                    }
                                    else
                                    {
                                        //Partition already exists during initial setup (and is fully processed), so ignore it
                                        LogMessage($"Partition {DateFormatPartitionKey(partitionKey, partitioningConfiguration.Granularity)} already exists and is processed", MessageType.Informational, true);
                                    }
                                }
                            }
                        }

                        //Ensure template partition doesn't contain any data
                        if (_modelConfiguration.InitialSetUp)
                        {
                            string beginParam = GetDateKey("19010102", Granularity.Daily, tableConfiguration.PartitioningConfigurations[0].IntegerDateKey, false, templatePartition.Source is MPartitionSource);
                            string endParam = GetDateKey("19010101", Granularity.Daily, tableConfiguration.PartitioningConfigurations[0].IntegerDateKey, false, templatePartition.Source is MPartitionSource);
                            //Query generated will always return nothing
                            string query = tableConfiguration.PartitioningConfigurations[0].TemplateSourceQuery.Replace("{0}", beginParam).Replace("{1}", endParam);

                            switch (templatePartition.Source)
                            {
                                case MPartitionSource mSource:
                                    mSource.Expression = query;
                                    break;
                                case QueryPartitionSource querySource:
                                    querySource.Query = query;
                                    break;
                            }
                            templatePartition.RequestRefresh(RefreshType.DataOnly);
                        }
                    }
                }

                //Commit the data changes, and bring model back online if necessary

                LogMessage("", MessageType.Informational, false);
                LogMessage("Final operations", MessageType.Informational, false);
                LogMessage(new String('-', 16), MessageType.Informational, false);

                //Save changes setting MaxParallelism if necessary
                if (_modelConfiguration.MaxParallelism == -1)
                {
                    LogMessage("Save changes ...", MessageType.Informational, true);
                    database.Model.SaveChanges();
                }
                else
                {
                    LogMessage($"Save changes with MaxParallelism={Convert.ToString(_modelConfiguration.MaxParallelism)}...", MessageType.Informational, true);
                    database.Model.SaveChanges(new SaveOptions() { MaxParallelism = _modelConfiguration.MaxParallelism });
                }

                //Perform recalc if necessary
                if (_modelConfiguration.InitialSetUp || (!_modelConfiguration.InitialSetUp && !_modelConfiguration.IncrementalOnline))
                {
                    LogMessage("Recalc model to bring back online ...", MessageType.Informational, true);

                    database.Model.RequestRefresh(RefreshType.Calculate);
                    database.Model.SaveChanges();
                }

                Console.ForegroundColor = ConsoleColor.White;
                LogMessage("", MessageType.Informational, false);
                LogMessage("Finish: " + DateTime.Now.ToString("hh:mm:ss tt"), MessageType.Informational, false);
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                LogMessage("", MessageType.Informational, false);
                LogMessage($"Exception occurred: {DateTime.Now.ToString("hh:mm:ss tt")}", MessageType.Error, false);
                LogMessage($"Exception message: {exc.Message}", MessageType.Error, false);
                if (exc.InnerException != null)
                {
                    LogMessage($"Inner exception message: {exc.InnerException.Message}", MessageType.Error, false);
                }
                LogMessage("", MessageType.Informational, false);
                Console.ForegroundColor = ConsoleColor.White;

                //Auto retry
                if (_retryAttempts > 0)
                {
                    LogMessage($"Retry attempts remaining: {Convert.ToString(_retryAttempts)}. Will wait {Convert.ToString(_modelConfiguration.RetryWaitTimeSeconds)} seconds and then attempt retry.", MessageType.Informational, false);
                    _retryAttempts -= 1;
                    Thread.Sleep(_modelConfiguration.RetryWaitTimeSeconds * 1000);
                    PerformProcessing();
                }
            }
            finally
            {
                try
                {
                    _modelConfiguration = null;
                    _messageLogger = null;
                    if (server != null) server.Disconnect();
                }
                catch { }
            }
        }

        /// <summary>
        /// Merge months into a year, or days into a month.
        /// </summary>
        /// <param name="modelConfiguration">Configuration info for the model</param>
        /// <param name="messageLogger">Pointer to logging method</param>
        /// <param name="analysisServicesTable">Name of the partitioned table in the tabular model.</param>
        /// <param name="targetGranularity">Granularity of the newly created partition. Must be year or month.</param>
        /// <param name="partitionKey">Target partition key. If year, follow yyyy; if month follow yyyymm.</param>
        public static void MergePartitions(ModelConfiguration modelConfiguration, LogMessageDelegate messageLogger, string analysisServicesTable, Granularity targetGranularity, string partitionKey)
        {
            _modelConfiguration = modelConfiguration;
            _messageLogger = messageLogger;

            Server server = new Server();
            try
            {
                LogMessage("", MessageType.Informational, false);
                LogMessage($"Merge partitions into {partitionKey} for table {analysisServicesTable}", MessageType.Informational, false);
                LogMessage(new String('-', partitionKey.Length + analysisServicesTable.Length + 33), MessageType.Informational, false);
                LogMessage("", MessageType.Informational, false);
                LogMessage("=>Actions & progress:", MessageType.Informational, false);

                //Check target granularity
                if (targetGranularity == Granularity.Daily)
                {
                    throw new InvalidOperationException($"Target granularity for merging must be year or month.");
                }

                //Check new partition key is expected format
                int partitionKeyParsed;
                if ( !(
                        (partitionKey.Length == 4 && targetGranularity == Granularity.Yearly) || 
                        (partitionKey.Length == 6 && targetGranularity == Granularity.Monthly)
                      ) || !int.TryParse(partitionKey, out partitionKeyParsed)
                   )
                {
                    throw new InvalidOperationException($"Partition key {partitionKey} is not of expected format.");
                }

                //Check configuration contains the partitioned table
                bool foundMatch = false;
                PartitioningConfiguration partitionConfig = null;
                foreach (TableConfiguration tableConfig in modelConfiguration.TableConfigurations)
                {
                    if (tableConfig.AnalysisServicesTable == analysisServicesTable && tableConfig.PartitioningConfigurations.Count > 0)
                    {
                        partitionConfig = tableConfig.PartitioningConfigurations[0];
                        foundMatch = true;
                        break;
                    }
                }
                if (!foundMatch)
                {
                    throw new InvalidOperationException($"Table {analysisServicesTable} not found in configuration with at least one partitioning configuration defined.");
                }

                Database database;
                Connect(server, out database);

                Table table = database.Model.Tables.Find(analysisServicesTable);
                if (table == null)
                {
                    throw new Microsoft.AnalysisServices.ConnectionException($"Could not connect to table {analysisServicesTable}.");
                }

                //Find template partition
                Partition templatePartition = table.Partitions.Find(analysisServicesTable);
                if (templatePartition == null)
                {
                    throw new InvalidOperationException($"Table {analysisServicesTable} does not contain a partition with the same name to act as the template partition.");
                }

                //See if there is already a partition with same key - do not want to delete an existing partition in case of inadvertent data loss
                if (table.Partitions.Find(partitionKey) != null)
                {
                    throw new InvalidOperationException($"Table {analysisServicesTable} already contains a partition with key {partitionKey}. Please delete this partition and retry.");
                }


                //Check there are partitions to be merged
                Granularity childGranularity = targetGranularity == Granularity.Yearly ? Granularity.Monthly : Granularity.Daily;
                List<Partition> partitionsToBeMerged = GetPartitionsCurrent(table, childGranularity, partitionKey);
                if (partitionsToBeMerged.Count == 0)
                {
                    LogMessage($"No partitinos found in {analysisServicesTable} to be merged into {partitionKey}.", MessageType.Informational, false);
                }
                else
                {
                    //Done with validation, so go ahead ...
                    LogMessage("", MessageType.Informational, false);
                    LogMessage($"Create new merged partition {DateFormatPartitionKey(partitionKey, targetGranularity)} for table {analysisServicesTable}", MessageType.Informational, true);
                    Partition newPartition = CreateNewPartition(table, templatePartition, partitionConfig, partitionKey, targetGranularity);

                    foreach (Partition partition in partitionsToBeMerged)
                    {
                        LogMessage($"Partition {partition.Name} to be merged into {DateFormatPartitionKey(partitionKey, targetGranularity)}", MessageType.Informational, true);
                    }

                    newPartition.RequestMerge(partitionsToBeMerged);
                    LogMessage($"Save changes for table {analysisServicesTable} ...", MessageType.Informational, true);
                    database.Model.SaveChanges();

                    Console.ForegroundColor = ConsoleColor.White;
                    LogMessage("", MessageType.Informational, false);
                    LogMessage("Finish: " + DateTime.Now.ToString("hh:mm:ss tt"), MessageType.Informational, false);
                }
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                LogMessage("", MessageType.Informational, false);
                LogMessage($"Exception occurred: {DateTime.Now.ToString("hh:mm:ss tt")}", MessageType.Error, false);
                LogMessage($"Exception message: {exc.Message}", MessageType.Error, false);
                if (exc.InnerException != null)
                {
                    LogMessage($"Inner exception message: {exc.InnerException.Message}", MessageType.Error, false);
                }
                LogMessage("", MessageType.Informational, false);
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                try
                {
                    _modelConfiguration = null;
                    _messageLogger = null;
                    if (server != null) server.Disconnect();
                }
                catch { }
            }
        }

        /// <summary>
        /// Defragment all partitions tables in a tabular model based on configuration
        /// </summary>
        /// <param name="modelConfiguration">Configuration info for the model</param>
        /// <param name="messageLogger">Pointer to logging method</param>
        public static void DefragPartitionedTables(ModelConfiguration modelConfiguration, LogMessageDelegate messageLogger)
        {
            _modelConfiguration = modelConfiguration;
            _messageLogger = messageLogger;

            Server server = new Server();
            try
            {
                Database database;
                Connect(server, out database);

                Console.ForegroundColor = ConsoleColor.White;
                LogMessage($"Start: {DateTime.Now.ToString("hh:mm:ss tt")}", MessageType.Informational, false);
                LogMessage($"Server: {_modelConfiguration.AnalysisServicesServer}", MessageType.Informational, false);
                LogMessage($"Database: {_modelConfiguration.AnalysisServicesDatabase}", MessageType.Informational, false);
                Console.ForegroundColor = ConsoleColor.Yellow;

                LogMessage("", MessageType.Informational, false);
                LogMessage($"Defrag partitioned tables in database {_modelConfiguration.AnalysisServicesDatabase}", MessageType.Informational, false);
                LogMessage(new String('-', _modelConfiguration.AnalysisServicesDatabase.Length + 38), MessageType.Informational, false);
                LogMessage("", MessageType.Informational, false);
                LogMessage("=>Actions & progress:", MessageType.Informational, false);

                foreach (TableConfiguration tableConfiguration in _modelConfiguration.TableConfigurations)
                {
                    //Only interested in partitoned tables
                    if (tableConfiguration.PartitioningConfigurations.Count > 0)
                    {
                        Table table = database.Model.Tables.Find(tableConfiguration.AnalysisServicesTable);
                        if (table == null)
                        {
                            throw new Microsoft.AnalysisServices.ConnectionException($"Could not connect to table {tableConfiguration.AnalysisServicesTable}.");
                        }

                        LogMessage($"Defrag table {tableConfiguration.AnalysisServicesTable} ...", MessageType.Informational, true);
                        table.RequestRefresh(RefreshType.Defragment);
                        database.Model.SaveChanges();
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
                LogMessage("", MessageType.Informational, false);
                LogMessage("Finish: " + DateTime.Now.ToString("hh:mm:ss tt"), MessageType.Informational, false);
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                LogMessage("", MessageType.Informational, false);
                LogMessage($"Exception occurred: {DateTime.Now.ToString("hh:mm:ss tt")}", MessageType.Error, false);
                LogMessage($"Exception message: {exc.Message}", MessageType.Error, false);
                if (exc.InnerException != null)
                {
                    LogMessage($"Inner exception message: {exc.InnerException.Message}", MessageType.Error, false);
                }
                LogMessage("", MessageType.Informational, false);
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                try
                {
                    _modelConfiguration = null;
                    _messageLogger = null;
                    if (server != null) server.Disconnect();
                }
                catch { }
            }
        }

        #endregion

        #region Private Methods

        private static void IncrementalProcessPartition(string partitionKey, Partition partitionToProcess, Granularity granularity)
        {
            if (_modelConfiguration.IncrementalOnline)
            {
                LogMessage($"Parallel process partition {DateFormatPartitionKey(partitionKey, granularity)} /Full", MessageType.Informational, true);
                partitionToProcess.RequestRefresh(RefreshType.Full);
            }
            else
            {
                LogMessage($"Parallel process partition {DateFormatPartitionKey(partitionKey, granularity)} /DataOnly", MessageType.Informational, true);
                partitionToProcess.RequestRefresh(RefreshType.DataOnly);
            }
        }

        private static void LogMessage(string message, MessageType messageType, bool indented)
        {
            _messageLogger($"{(indented ? new String(' ', 3) : "")}{message}", messageType, _modelConfiguration);
        }

        private static string DateFormatPartitionKey(string partitionKey, Granularity granularity)
        {
            string returnVal = partitionKey.Substring(0, 4);

            try
            {
                if (granularity == Granularity.Monthly || granularity == Granularity.Daily || granularity == Granularity.Weekly)
                {
                    returnVal += "-" + partitionKey.Substring(4, 2);
                }

                if (granularity == Granularity.Daily || granularity == Granularity.Weekly)
                {
                    returnVal += "-" + partitionKey.Substring(6, 2);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException($"Failed to derive date from partition key. Check the key {partitionKey} matches {Convert.ToString(granularity)} granularity.");
            }

            return returnVal;
        }

        private static void DisplayPartitionRange(List<string> partitionKeys, bool current, Granularity granularity)
        {
            LogMessage("", MessageType.Informational, false);

            if (partitionKeys.Count > 0)
            {
                LogMessage($"=>{(current ? "Current" : "New")} partition range ({Convert.ToString(granularity)}):", MessageType.Informational, false);
                LogMessage($"MIN partition:   {DateFormatPartitionKey(partitionKeys[0], granularity)}", MessageType.Informational, true);
                LogMessage($"MAX partition:   {DateFormatPartitionKey(partitionKeys[partitionKeys.Count - 1], granularity)}", MessageType.Informational, true);
                LogMessage($"Partition count: {partitionKeys.Count}", MessageType.Informational, true);
            }
            else
            {
                LogMessage("=>Table not yet partitioned", MessageType.Informational, false);
            }
        }

        private static void Connect(Server server, out Database database)
        {
            //Connect and get main objects
            string serverConnectionString = $"Provider=MSOLAP;{(_modelConfiguration.CommitTimeout == -1 ? "" : $"CommitTimeout={Convert.ToString(_modelConfiguration.CommitTimeout)};")}Data Source={_modelConfiguration.AnalysisServicesServer};";
            if (_modelConfiguration.IntegratedAuth)
            {
                serverConnectionString += $"Integrated Security=SSPI;";
            }
            else
            {
                serverConnectionString += $"User ID={_modelConfiguration.UserName};Password={_modelConfiguration.Password};Persist Security Info=True;Impersonation Level=Impersonate;";
            }
            server.Connect(serverConnectionString);

            database = server.Databases.FindByName(_modelConfiguration.AnalysisServicesDatabase);
            if (database == null)
            {
                throw new Microsoft.AnalysisServices.ConnectionException($"Could not connect to database {_modelConfiguration.AnalysisServicesDatabase}.");
            }
        }

        private static List<string> GetPartitionKeysTarget(bool forProcessing, PartitioningConfiguration partitioningConfiguration, Granularity granularity) 
        {
            //forProcessing: false to return complete target set of partitions, true to return partitons to be processed (may be less if incremental mode).

            List<string> partitionKeys = new List<string>();
            int numberOfPartitions = (forProcessing && !_modelConfiguration.InitialSetUp ? partitioningConfiguration.NumberOfPartitionsForIncrementalProcess : partitioningConfiguration.NumberOfPartitionsFull);

            for (int i = numberOfPartitions - 1; i >= 0; i--)
            {
                DateTime periodToAdd;
                switch (granularity)
                {
                    case Granularity.Daily:
                        periodToAdd = partitioningConfiguration.MaxDate.AddDays(-i);
                        partitionKeys.Add(Convert.ToString((periodToAdd.Year * 100 + periodToAdd.Month) * 100 + periodToAdd.Day));
                        break;
                    case Granularity.Weekly:
                        periodToAdd = partitioningConfiguration.MaxDate.AddDays(-i*7);
                        partitionKeys.Add(Convert.ToString((periodToAdd.Year * 100 + periodToAdd.Month) * 100 + periodToAdd.Day));
                        break;
                    case Granularity.Monthly:
                        periodToAdd = partitioningConfiguration.MaxDate.AddMonths(-i);
                        partitionKeys.Add(Convert.ToString(periodToAdd.Year * 100 + periodToAdd.Month));
                        break;
                    default: //Granularity.Yearly:
                        periodToAdd = partitioningConfiguration.MaxDate.AddYears(-i);
                        partitionKeys.Add(Convert.ToString(periodToAdd.Year));
                        break;
                }
            }
            partitionKeys.Sort();

            return partitionKeys;
        }

        private static List<string> GetPartitionKeysCurrent(Table table, Granularity granularity, string filter = "")
        {
            List<string> partitionKeysExisting = new List<string>();

            foreach (Partition partition in table.Partitions)
            {
                int partitionKey = -1;
                if (IncludePartition(granularity, filter, partition, ref partitionKey))
                {
                    partitionKeysExisting.Add(Convert.ToString(partitionKey));
                }
            }
            partitionKeysExisting.Sort();

            return partitionKeysExisting;
        }

        private static List<Partition> GetPartitionsCurrent(Table table, Granularity granularity, string filter = "")
        {
            List<Partition> partitionsExisting = new List<Partition>();

            foreach (Partition partition in table.Partitions)
            {
                int partitionKey = -1;
                if (IncludePartition(granularity, filter, partition, ref partitionKey))
                {
                    partitionsExisting.Add(partition);
                }
            }

            return partitionsExisting;
        }

        private static Partition CreateNewPartition(Table table, Partition templatePartition, PartitioningConfiguration partitioningConfiguration, string partitionKey, Granularity granularity)
        {
            string beginParam = GetDateKey(partitionKey, granularity, partitioningConfiguration.IntegerDateKey, false, templatePartition.Source is MPartitionSource);
            string endParam = GetDateKey(partitionKey, granularity, partitioningConfiguration.IntegerDateKey, true, templatePartition.Source is MPartitionSource);

            Partition newPartition;
            newPartition = new Partition();
            templatePartition.CopyTo(newPartition);
            newPartition.Name = partitionKey;
            //string query = String.Format(partitioningConfiguration.TemplateSourceQuery, beginParam, endParam);
            string query = partitioningConfiguration.TemplateSourceQuery.Replace("{0}", beginParam).Replace("{1}", endParam);
            switch (newPartition.Source)
            {
                case MPartitionSource mSource:
                    mSource.Expression = query;
                    break;
                case QueryPartitionSource querySource:
                    querySource.Query = query;
                    break;
            }
            table.Partitions.Add(newPartition);
            return newPartition;
        }

        private static string GetDateKey(string partitionKey, Granularity granularity, bool integerDateKey, bool addPeriod, bool mExpression)
        {
            DateTime dateVal = new DateTime();

            switch (granularity)
            {
                case Granularity.Daily:
                    dateVal = new DateTime(Convert.ToInt32(partitionKey.Substring(0, 4)), Convert.ToInt32(partitionKey.Substring(4, 2)), Convert.ToInt32(partitionKey.Substring(6, 2)));
                    if (addPeriod)
                    {
                        dateVal = dateVal.AddDays(1);
                    }
                    break;
                case Granularity.Weekly:
                    dateVal = new DateTime(Convert.ToInt32(partitionKey.Substring(0, 4)), Convert.ToInt32(partitionKey.Substring(4, 2)), Convert.ToInt32(partitionKey.Substring(6, 2)));
                    if (addPeriod)
                    {
                        dateVal = dateVal.AddDays(7);
                    }
                    break;
                case Granularity.Monthly:
                    dateVal = new DateTime(Convert.ToInt32(partitionKey.Substring(0, 4)), Convert.ToInt32(partitionKey.Substring(4, 2)), 1);
                    if (addPeriod)
                    {
                        dateVal = dateVal.AddMonths(1);
                    }
                    break;
                case Granularity.Yearly:
                    dateVal = new DateTime(Convert.ToInt32(partitionKey.Substring(0, 4)), 1, 1);
                    if (addPeriod)
                    {
                        dateVal = dateVal.AddYears(1);
                    }
                    break;
                default:
                    break;
            }

            if (integerDateKey)
            {
                return dateVal.ToString("yyyyMMdd");
            }
            else
            {
                if (mExpression)
                {
                    return $"#datetime({dateVal.ToString("yyyy, MM, dd")}, 0, 0, 0)";
                }
                else
                {
                    return $"'{dateVal.ToString("yyyy-MM-dd")}'";
                }
            }
        }

        private static bool IncludePartition(Granularity granularity, string filter, Partition partition, ref int partitionKey)
        {
            //Ignore partitions that don't follow the convention yyyy, yyyymm or yyyymmdd, or are not included in the filter expression
            return ( (partition.Name.Length == 4 && granularity == Granularity.Yearly) ||
                     (partition.Name.Length == 6 && granularity == Granularity.Monthly) ||
                     (partition.Name.Length == 8 && (granularity == Granularity.Daily || granularity == Granularity.Weekly))
                     
                   ) && int.TryParse(partition.Name, out partitionKey) &&
                   partition.Name.StartsWith(filter);
        }

        #endregion
    }
}

