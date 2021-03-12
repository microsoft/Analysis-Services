using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using Metadata_Translator;

namespace MTCmd
{
    public enum Mode
    {
        Export,
        Import,
        Overwrite
    }

    class Program
    {
        static int Main(string[] args)
        {
            ///  
            /// Create a root command with the required options.
            /// 
            var rootCommand = new RootCommand(Strings.rootCmdDescription)
            {
                new Option<string>(
                    new string[]{ "--connection-string", "-cs" }, Strings.csDescription){ IsRequired = true },
                new Option<Mode>(
                    new string[]{ "--mode", "-m" },
                    getDefaultValue: () => Mode.Export,
                    description: Strings.modeDescription),
                new Option<DirectoryInfo>(
                    new string[]{ "--export-folder", "-ef" }, Strings.efDescription).ExistingOnly(),
                new Option<FileInfo>(
                    new string[]{ "--import-file", "-if" }, Strings.ifDescription).ExistingOnly(),
                new Option<string>(
                    new string[]{ "--locale-id", "-lcid" }, Strings.lcidDescription)
            };


            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string, Mode, DirectoryInfo, FileInfo, string>((connectionString, mode, exportFolder, importFile, localeId) =>
            {
                try
                {
                    DataModel model = DataModel.Connect(connectionString);
                    model.InitializeLanguages();

                    switch (mode)
                    {
                        case Mode.Export:
                            Export(model, exportFolder, localeId);
                            break;
                        case Mode.Import:
                            Import(model, importFile, false);
                            break;
                        case Mode.Overwrite:
                            Import(model, importFile, true);
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"{ex}");
                }
            });

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        static void Import(DataModel model, FileInfo importFile, bool overwriteMode)
        {
            if (importFile != null)
            {
                string lcid = Path.GetFileNameWithoutExtension(importFile.Name);
                if (model.SetLanguageFlags(lcid, true))
                {
                    model.ImportFromCsv(importFile.FullName, lcid, overwriteMode);
                    model.Update();
                    Console.WriteLine(Strings.importSuccess, importFile);
                }
                else
                {
                    Console.WriteLine(Strings.invalidLocale);
                }
            }
            else
            {
                Console.WriteLine(Strings.noImportFileSpecified);
            }
        }

        static void Export(DataModel model, DirectoryInfo exportFolder, string lcid)
        {
            if (exportFolder != null)
            {
                if(!string.IsNullOrEmpty(lcid))
                {
                    model.DeselectAllLanguages();
                    model.SetLanguageFlags(lcid, true, false);

                    model.ExportToCsv(exportFolder.FullName);
                    Console.WriteLine(Strings.singleLocalExportSuccess, lcid, exportFolder);
                }
                else if (model.HasTargetLanguages)
                {
                    model.ExportToCsv(exportFolder.FullName);
                    Console.WriteLine(Strings.exportSuccess, exportFolder);
                }
                else
                {
                    Console.WriteLine(Strings.noExportableTranslations);
                }
            }
            else
            {
                Console.WriteLine(Strings.noExportFolderSpecified);
            }
        }
    }
}
