using System;
using CommandLine;
using CommandLine.Text;

namespace AsPartitionProcessing.SampleClient
{
    class ArgumentOptions
    {
        [Option('m', "ExecutionMode", HelpText = "Execution mode of SampleClient. Possible values are InitializeInline, InitializeFromDatabase, MergePartitions, DefragPartitionedTables.")]
        public string ExecutionMode { get; set; }

        [Option('t', "MergeTable", HelpText = "When ExecutionMode=MergePartitions, name of the partitioned table in the tabular model.")]
        public string MergeTable { get; set; }

        [Option('g', "TargetGranularity", HelpText = "When ExecutionMode=MergePartitions, granularity of the newly created partition. Possible values are Yearly or Monthly.")]
        public string TargetGranularity { get; set; }

        [Option('k', "MergePartitionKey", HelpText = "When ExecutionMode=MergePartitions, target partition key. If year, follow yyyy; if month follow yyyymm.")]
        public string MergePartitionKey { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
