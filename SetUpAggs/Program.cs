using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SetUpAggs
{
    #region JsonHelper Class

    public static class JsonHelper
    {
        public static string FromClass<T>(T data, bool isEmptyToNull = false,
                                          JsonSerializerSettings jsonSettings = null)
        {
            string response = string.Empty;

            if (!EqualityComparer<T>.Default.Equals(data, default(T)))
                response = JsonConvert.SerializeObject(data, jsonSettings);

            return isEmptyToNull ? (response == "{}" ? "null" : response) : response;
        }

        public static T ToClass<T>(string data, JsonSerializerSettings jsonSettings = null)
        {
            var response = default(T);

            if (!string.IsNullOrEmpty(data))
                response = jsonSettings == null
                    ? JsonConvert.DeserializeObject<T>(data)
                    : JsonConvert.DeserializeObject<T>(data, jsonSettings);

            return response;
        }
    }
    #endregion

    #region Json Configuration Classes

    public partial class AggsConfiguration
    {
        [JsonProperty("database")]
        public Database Database { get; set; }
    }

    public partial class Database
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tables")]
        public Table[] Tables { get; set; }
    }

    public partial class Table
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("aggregationRules", NullValueHandling = NullValueHandling.Ignore)]
        public AggregationRule[] AggregationRules { get; set; }
    }

    public partial class AggregationRule
    {
        [JsonProperty("aggTableColumn")]
        public string AggTableColumn { get; set; }

        [JsonProperty("summarization")]
        public string Summarization { get; set; }

        [JsonProperty("detailTable")]
        public string DetailTable { get; set; }

        [JsonProperty("detailTableColumn", NullValueHandling = NullValueHandling.Ignore)]
        public string DetailTableColumn { get; set; }
    }

    public partial class Object
    {
        [JsonProperty("database")]
        public string Database { get; set; }
    }

    public partial class AggsConfiguration
    {
        public static AggsConfiguration FromJson(string json) => JsonConvert.DeserializeObject<AggsConfiguration>(json, SetUpAggs.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this AggsConfiguration self) => JsonConvert.SerializeObject(self, SetUpAggs.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    #endregion

    #region Command Line Utility Arguments Handling

    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class AggConfigurationUtility
    {
        Microsoft.AnalysisServices.Tabular.Server server = new Microsoft.AnalysisServices.Tabular.Server();
        Microsoft.AnalysisServices.Tabular.Database database = new Microsoft.AnalysisServices.Tabular.Database();
        AggsConfiguration aggsConfig;

        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }


        /// <summary>
        /// Validate the configuration file against the model before applying changes
        /// </summary>
        /// <param name="tableConfig"></param>
        private void ValidateConfigurationOfTable(Table tableConfig)
        {
            var table = database.Model.Tables.Find(tableConfig.Name);

            if (table == null)
                throw new Exception($"Could not find table [{tableConfig.Name}]");

            try
            {
                Enum.Parse(typeof(ModeType), tableConfig.Mode);
            }
            catch(Exception)
            {
                throw new Exception($"Table [{tableConfig.Name}] has invalid mode type {tableConfig.Mode}, must be one of Default, DirectQuery, Dual, or Import");
            }

            if (table.Partitions.Count == 0)
                throw new Exception($"No partitions found for table [{table.Name}]");

            if(tableConfig.AggregationRules != null)
                foreach (AggregationRule rule in tableConfig.AggregationRules)
                {
                    if(rule.Summarization != "CountTableRows")
                        try
                        {
                            Enum.Parse(typeof(SummarizationType), rule.Summarization);
                        }
                        catch (Exception)
                        {
                            throw new Exception($"Aggregation rule summarization invalid [{rule.Summarization}], must be one of GroupBy, Sum, Min, Max, Count, or CountTableRows");
                        }

                    Microsoft.AnalysisServices.Tabular.Column aggColumn = table.Columns.Find(rule.AggTableColumn);

                    if (aggColumn == null)
                        throw new Exception($"Cound not find aggregation column [{table.Name}].[{rule.AggTableColumn}] in model");

                    var detailTable = database.Model.Tables.Find(rule.DetailTable);

                    if (detailTable == null)
                        throw new Exception($"Cound not find detail table [{rule.DetailTable}] in model");

                    var detailColumn = (rule.DetailTableColumn == null ? null : detailTable.Columns.Find(rule.DetailTableColumn));

                    bool isCountAggregate = rule.Summarization != "Count" || rule.Summarization != "CountTableRows";

                    if (detailColumn == null && !isCountAggregate)
                        throw new Exception($"Cound not find detail column [{rule.DetailTable}].[{rule.DetailTableColumn}] in model");

                    if (!isCountAggregate && aggColumn.DataType != detailColumn.DataType)
                        throw new Exception($"Data type mismatch for aggregation column [{table.Name}].[{rule.AggTableColumn}] and detail column [{rule.DetailTable}].[{rule.DetailTableColumn}]");

                }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Validated aggregation rules for [{table.Name}], mode currently set to {table.Partitions[0].Mode.ToString()}");
        }

        /// <summary>
        /// Applies the differences between the configuration file and the model
        /// </summary>
        /// <param name="tableConfig"></param>
        private int ApplyConfigurationOfTable(Table tableConfig)
        {
            // Update mode of each partition of the table if a difference is found between the model and the configuration
            var table = database.Model.Tables.Find(tableConfig.Name);
            int partitionModesChanged = 0;
            table.Partitions.ForEach(p =>
                {
                    var mode = (ModeType)Enum.Parse(typeof(ModeType), tableConfig.Mode);
                    if (p.Mode != mode)
                    {
                        p.Mode = mode;
                        partitionModesChanged++;
                    }
                }
            );

            // Remove all alternateOf definitions from the model that do not match an aggregation rule in the configuration
            int alternateOfRemoved = 0;
            table.Columns.ForEach(c =>
                {
                    int ruleMatch = 0;

                    if(tableConfig.AggregationRules != null && c.AlternateOf != null)
                        ruleMatch = tableConfig.AggregationRules.Where(r =>
                            r.Summarization != "CountTableRows" &&
                            r.AggTableColumn == c.Name &&
                            r.DetailTable == c.AlternateOf.BaseColumn.Table.Name &&
                            r.DetailTableColumn == c.AlternateOf.BaseColumn.Name &&
                            (SummarizationType)Enum.Parse(typeof(SummarizationType), r.Summarization) == c.AlternateOf.Summarization
                            ).Count() +
                        tableConfig.AggregationRules.Where(r =>
                            r.Summarization == "CountTableRows" &&
                            r.AggTableColumn == c.Name &&
                            r.DetailTable == c.AlternateOf.BaseTable.Name &&
                            c.AlternateOf.Summarization == SummarizationType.Count
                            ).Count();

                    if (ruleMatch == 0 && c.AlternateOf != null)
                    {
                        var baseObjectName = c.AlternateOf.BaseColumn == null ? 
                                             String.Format("[{0}]", c.AlternateOf.BaseTable.Name) :
                                             String.Format("[{0}].[{1}]", c.AlternateOf.BaseColumn.Table.Name, c.AlternateOf.BaseColumn.Name);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Removing aggregation rule from column [{c.Name}] with alternate of {baseObjectName} aggregation {c.AlternateOf.Summarization}");

                        c.AlternateOf = null;
                        alternateOfRemoved++;
                    }
                }
            );

            // Add all alternateOf definitions from the configuration that do not match an alternateOf definition in the model
            int alternateOfAdded = 0;
            if(tableConfig.AggregationRules != null)
                tableConfig.AggregationRules.ForEach(r =>
                    {
                        var column = table.Columns.First(c => c.Name == r.AggTableColumn);
                        var detailTable = database.Model.Tables.Find(r.DetailTable);
                        var detailColumn = (r.DetailTableColumn != null) ? detailTable.Columns.Find(r.DetailTableColumn) : null;

                        if (column.AlternateOf == null)
                        {
                            if (r.Summarization == "CountTableRows")
                                column.AlternateOf = new AlternateOf
                                {
                                    Summarization = SummarizationType.Count,
                                    BaseTable = detailTable
                                };
                            else
                                column.AlternateOf = new AlternateOf
                                {
                                    Summarization = (SummarizationType)Enum.Parse(typeof(SummarizationType), r.Summarization),
                                    BaseColumn = detailColumn
                                };

                            alternateOfAdded++;

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"Adding aggregation rule for column [{column.Name}] with alternate of [{detailTable.Name + (detailColumn == null ? string.Empty : "].[" + detailColumn.Name)}] aggregation {column.AlternateOf.Summarization}");
                        }
                    }
                );

            return partitionModesChanged + alternateOfRemoved + alternateOfAdded;
        }

        /// <summary>
        /// Connect to analysis services instance and database, throw if error is encountered
        /// </summary>
        /// <param name="serverName"></param>
        private void Connect(string serverName)
        {
            // Changed this line to force login dialogs
            //string connectionString = $"Provider=MSOLAP;Data Source={args.Server};";
            string connectionString = $"Provider=MSOLAP;Data Source={serverName};Persist Security Info=True;";
            server.Connect(connectionString);

            database = server.Databases.FindByName(aggsConfig.Database.Name);

            if (database == null)
            {
                throw new Microsoft.AnalysisServices.ConnectionException($"Could not find database [{aggsConfig.Database.Name}]");
            }
        }

        [ArgActionMethod, ArgDescription("Applies the Aggs configuration to the model specified")]
        public void Apply(ServerAndConfigurationArgs args)
        {
            List<Microsoft.AnalysisServices.Tabular.Table> updatedTableCollection = new List<Microsoft.AnalysisServices.Tabular.Table>();

            // Read configuration file
            aggsConfig = JsonHelper.ToClass<AggsConfiguration>(File.ReadAllText(args.ConfigFile));

            #region Connect

            Connect(args.Server);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Start: {DateTime.Now.ToString("hh:mm:ss tt")}");
            Console.WriteLine($"Server: {args.Server}");
            Console.WriteLine($"Database: {aggsConfig.Database.Name}");

            #endregion

            #region Validate Configuration Against Model

            aggsConfig.Database.Tables.ForEach(x => ValidateConfigurationOfTable(x));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Validation successful");

            #endregion

            #region Update Model Compatibility

            //Set database compat level
            if (database.CompatibilityLevel < 1465)
            {
                database.CompatibilityLevel = 1465;
                database.Update();
            }

            #endregion

            #region Apply Model Changes

            var changesApplied = 0;
            aggsConfig.Database.Tables.ForEach(x =>
                {

                    var delta = ApplyConfigurationOfTable(x);

                    if (delta > 0)
                        updatedTableCollection.Add(database.Model.Tables.Find(x.Name));

                    changesApplied += delta;
                }
                );

            if (changesApplied == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"No changes applied to model");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Applied model changes");
            }

            #endregion

            #region Expand Full

            if (changesApplied == 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Skipping write to database, no changes detected");
            }
            else
            {
                // Set database UpdateOptions to ExpandFull
                try
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Writing model changes to database ");
                    database.Update(Microsoft.AnalysisServices.UpdateOptions.ExpandFull);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("SUCCESS");
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAILED");

                    throw e;
                }
            }

            #endregion

            #region Refresh Tables as Required

            if (changesApplied == 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Skipping table refresh, no changes detected");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Refreshing tables - begin: " + DateTime.Now.ToString("hh:mm:ss tt"));

                foreach (var tableObj in updatedTableCollection)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"Refreshing table [{tableObj.Name}] ");
                    tableObj.RequestRefresh(RefreshType.Full);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("COMPLETE");
                }

                // Removed this line which was throwing errors on large models, and it seems to commit the changes without it 
                // database.Model.SaveChanges();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Refreshing tables - end: " + DateTime.Now.ToString("hh:mm:ss tt"));
            }

            #endregion
        }
    }

    public class ServerAndConfigurationArgs
    {
        [ArgRequired, ArgDescription("The URL to the Azure Analysis Services instance e.g.: asazure://host/instance"), ArgPosition(1)]
        public string Server { get; set; }

        [ArgRequired, ArgDescription("The file path to the JSON configuration file to apply to the database"), ArgPosition(2)]
        public string ConfigFile { get; set; }
    }

    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Args.InvokeAction<AggConfigurationUtility>(args);
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("");
                Console.WriteLine($"Exception occurred: {DateTime.Now.ToString("hh:mm:ss tt")}");
                Console.WriteLine($"Exception message: {exc.Message}");
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
