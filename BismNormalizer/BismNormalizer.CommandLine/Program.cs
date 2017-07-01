using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using BismNormalizer.TabularCompare;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.CommandLine
{
    class Program
    {
        private const int ERROR_SUCCESS = 0;
        private const int ERROR_FILE_NOT_FOUND = 2;
        private const int ERROR_BAD_ARGUMENTS = 160;
        private const int ERROR_GENERIC_NOT_MAPPED = 1360;
        private const int ERROR_NULL_REF_POINTER = 1780;

        static int Main(string[] args)
        {
            string bsmnFile = null;
            string logFile = null;
            string scriptFile = null;
            List<string> skipOptions = null;
            StreamWriter writer = null;
            Comparison _comparison = null;

            try
            {
                #region Argument validation / help message

                if (!(args?.Length > 0))
                {
                    Console.WriteLine("No arguments received. Exiting.");
                    return ERROR_BAD_ARGUMENTS;
                }
                else if (args[0].ToLower() == "help" || args[0].ToLower() == "?" || args[0].ToLower() == "/?" || args[0].ToLower() == "/h" || args[0].ToLower() == "/help" || args[0].ToLower() == "-help" || args[0].ToLower() == "-h")
                {
                    Console.WriteLine("");
                    Console.WriteLine("   BISM Normalizer Command-Line Utility");
                    Console.WriteLine("");
                    Console.WriteLine("   Executes BISM Normalizer in command-line mode, based on content of BSMN file");
                    Console.WriteLine("");
                    Console.WriteLine("   USAGE:");
                    Console.WriteLine("");
                    Console.WriteLine("   BismNormalizer.exe BsmnFile [/Log:LogFile] [/Script:ScriptFile] [/Skip:{MissingInSource | MissingInTarget | DifferentDefinitions}]");
                    Console.WriteLine("");
                    Console.WriteLine("   BsmnFile : Full path to the .bsmn file.");
                    Console.WriteLine("");
                    Console.WriteLine("   /Log:LogFile : All messages are output to the LogFile.");
                    Console.WriteLine("");
                    Console.WriteLine("   /Script:ScriptFile : Does not perform actual update to target database; instead, a deployment script is generated and stored to the ScriptFile.");
                    Console.WriteLine("");
                    Console.WriteLine("   /Skip:{MissingInSource | MissingInTarget | DifferentDefinitions} : Skip all objects that are missing in source/missing in target/with different definitions. Can pass a comma separated list of multiple skip options; e.g. 'MissingInSource,MissingInTarget,DifferentDefinitions'.");
                    Console.WriteLine("");

                    return ERROR_SUCCESS;
                }

                bsmnFile = args[0];

                const string logPrefix = "/log:";
                const string scriptPrefix = "/script:";
                const string skipPrefix = "/skip:";

                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].Length >= logPrefix.Length && args[i].Substring(0, logPrefix.Length).ToLower() == logPrefix)
                    {
                        logFile = args[i].Substring(logPrefix.Length, args[i].Length - logPrefix.Length).Replace("\"", "");
                    }
                    else if (args[i].Length >= scriptPrefix.Length && args[i].Substring(0, scriptPrefix.Length).ToLower() == scriptPrefix)
                    {
                        scriptFile = args[i].Substring(scriptPrefix.Length, args[i].Length - scriptPrefix.Length).Replace("\"", "");
                    }
                    else if (args[i].Length >= skipPrefix.Length && args[i].Substring(0, skipPrefix.Length).ToLower() == skipPrefix)
                    {
                        skipOptions = new List<string>(args[i].Substring(skipPrefix.Length, args[i].Length - skipPrefix.Length).Split(','));
                        foreach (string skipOption in skipOptions)
                        {
                            if (!(skipOption == ComparisonObjectStatus.MissingInSource.ToString() || skipOption == ComparisonObjectStatus.MissingInTarget.ToString() || skipOption == ComparisonObjectStatus.DifferentDefinitions.ToString()))
                            {
                                Console.WriteLine($"Argument '{args[i]}' is invalid. Valid skip options are '{ComparisonObjectStatus.MissingInSource.ToString()}', '{ComparisonObjectStatus.MissingInTarget.ToString()}' or '{ComparisonObjectStatus.DifferentDefinitions.ToString()}'");
                                return ERROR_BAD_ARGUMENTS;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"'{args[i]}' is not a valid argument.");
                        return ERROR_BAD_ARGUMENTS;
                    }
                }

                if (logFile != null)
                {
                    // Attempt to open output file.
                    writer = new StreamWriter(logFile);
                    // Redirect output from the console to the file.
                    Console.SetOut(writer);
                }

                #endregion

                if (!File.Exists(bsmnFile))
                {
                    throw new FileNotFoundException($"File not found {bsmnFile}");
                }
                Console.WriteLine($"About to deserialize {bsmnFile}");
                ComparisonInfo comparisonInfo = ComparisonInfo.DeserializeBsmnFile(bsmnFile);

                Console.WriteLine();
                if (comparisonInfo.ConnectionInfoSource.UseProject)
                {
                    Console.WriteLine($"Source Project File: {comparisonInfo.ConnectionInfoSource.ProjectFile}");
                }
                else
                {
                    Console.WriteLine($"Source Database: {comparisonInfo.ConnectionInfoSource.ServerName};{comparisonInfo.ConnectionInfoSource.DatabaseName}");
                }

                if (comparisonInfo.ConnectionInfoTarget.UseProject)
                {
                    Console.WriteLine($"Target Project: {comparisonInfo.ConnectionInfoTarget.ProjectName}");
                }
                else
                {
                    Console.WriteLine($"Target Database: {comparisonInfo.ConnectionInfoTarget.ServerName};{comparisonInfo.ConnectionInfoTarget.DatabaseName}");
                }

                Console.WriteLine();
                Console.WriteLine("--Comparing ...");
                _comparison = ComparisonFactory.CreateComparison(comparisonInfo);
                _comparison.ValidationMessage += HandleValidationMessage;
                _comparison.Connect();
                _comparison.CompareTabularModels();

                if (skipOptions != null)
                {
                    foreach (string skipOption in skipOptions)
                    {
                        SetSkipOptions(skipOption, _comparison.ComparisonObjects);
                    }
                }
                Console.WriteLine("--Done");
                Console.WriteLine();

                Console.WriteLine("--Validating ...");
                _comparison.ValidateSelection();
                Console.WriteLine("--Done");
                Console.WriteLine();

                if (scriptFile != null)
                {
                    Console.WriteLine("--Generating script ...");
                    //Generate script
                    File.WriteAllText(scriptFile, _comparison.ScriptDatabase());
                    Console.WriteLine($"Generated script '{scriptFile}'");
                }
                else
                {
                    Console.WriteLine("--Updating ...");
                    //Update target database/project
                    _comparison.Update();
                    if (comparisonInfo.ConnectionInfoTarget.UseProject)
                    {
                        Console.WriteLine($"Applied changes to project {comparisonInfo.ConnectionInfoTarget.ProjectName}.");
                    }
                    else
                    {
                        Console.WriteLine($"Deployed changes to database {comparisonInfo.ConnectionInfoTarget.DatabaseName}.");
                        Console.WriteLine("Passwords have not been set for impersonation accounts (setting passwords is not supported in command-line mode). Ensure the passwords are set before processing.");
                        if (comparisonInfo.OptionsInfo.OptionProcessingOption != ProcessingOption.DoNotProcess)
                        {
                            Console.WriteLine("No processing has been done (processing is not supported in command-line mode).");
                        }
                    }
                }
                Console.WriteLine("--Done");
            }
            catch (FileNotFoundException exc)
            {
                Console.WriteLine("The following exception occurred:");
                Console.WriteLine(exc.ToString());

                return ERROR_FILE_NOT_FOUND;
            }
            catch (ArgumentNullException exc)
            {
                Console.WriteLine("The following exception occurred. Try re-saving the BSMN file from Visual Studio using latest version of BISM Normalizer to ensure all necessary properties are deserialized and stored in the file.");
                Console.WriteLine();
                Console.WriteLine(exc.ToString());
                
                return ERROR_NULL_REF_POINTER;
            }
            catch (Exception exc)
            {
                Console.WriteLine("The following exception occurred:");
                Console.WriteLine(exc.ToString());

                return ERROR_GENERIC_NOT_MAPPED;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (_comparison != null)
                {
                    _comparison.Disconnect();
                }
            }

            return ERROR_SUCCESS;
        }

        private static void SetSkipOptions(string skipOption, List<ComparisonObject> comparisonObjects)
        {
            foreach (ComparisonObject comparisonObj in comparisonObjects)
            {
                if (   ((skipOption == ComparisonObjectStatus.MissingInSource.ToString() && comparisonObj.Status == ComparisonObjectStatus.MissingInSource) ||
                        (skipOption == ComparisonObjectStatus.MissingInTarget.ToString() && comparisonObj.Status == ComparisonObjectStatus.MissingInTarget) ||
                        (skipOption == ComparisonObjectStatus.DifferentDefinitions.ToString() && comparisonObj.Status == ComparisonObjectStatus.DifferentDefinitions)
                       ) && comparisonObj.MergeAction != MergeAction.Skip
                   )
                {
                    comparisonObj.MergeAction = MergeAction.Skip;
                    string objectName = (string.IsNullOrEmpty(comparisonObj.SourceObjectName) ? comparisonObj.TargetObjectName : comparisonObj.SourceObjectName).TrimStart();
                    Console.WriteLine($"Skip due to /Skip argument {skipOption} on {comparisonObj.ComparisonObjectType.ToString()} object {objectName}");
                }

                SetSkipOptions(skipOption, comparisonObj.ChildComparisonObjects);
            }
        }

        private static void HandleValidationMessage(object sender, ValidationMessageEventArgs e)
        {
            Console.WriteLine($"{e.ValidationMessageStatus.ToString()} Message for {e.ValidationMessageType.ToString()}: {e.Message}");
        }
    }
}
