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
        Overwrite,
        ExportResx
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
                    new string[]{ "--locale-id", "-lcid" }, Strings.lcidDescription),
                new Option<bool>(
                    new string[]{ "--fallback-mode", "-fm" }, Strings.fallbackDescription),
                 new Option<string>(
                    new string[]{ "--key-prefix", "-kp" }, Strings.keyPrefixDescription)
           };


            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string, Mode, DirectoryInfo, FileInfo, string, bool, string>((connectionString, mode, exportFolder, importFile, localeId, fallbackMode, keyPrefix) =>
            {
                try
                {
                    DataModel model = DataModel.Connect(connectionString);
                    model.InitializeLanguages();

                    switch (mode)
                    {
                        case Mode.Export:
                        case Mode.ExportResx:
                            Export(mode, model, exportFolder, localeId, keyPrefix);
                            break;
                        case Mode.Import:
                            Import(model, importFile, false, fallbackMode);
                            break;
                        case Mode.Overwrite:
                            Import(model, importFile, true, fallbackMode);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"{ex}");
                }
            });

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        static void Import(DataModel model, FileInfo importFile, bool overwriteMode, bool fallbackDefault)
        {
            Func<string, bool> import = (lcid) => {
                if (Path.GetExtension(importFile.Name).Equals(".csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    model.ImportFromCsv(importFile.FullName, lcid, overwriteMode, fallbackDefault);
                    return true;
                }
                else if (Path.GetExtension(importFile.Name).Equals(".resx", StringComparison.InvariantCultureIgnoreCase))
                {
                    string referenceResx = $"{importFile.DirectoryName}\\{model.DefaultCulture}.resx";
                    if(File.Exists(referenceResx))
                    {
                        try
                        {
                            model.ImportFromResx(importFile.FullName, referenceResx, lcid, overwriteMode, fallbackDefault);
                            return true;
                        }
                        catch (NoResxMatchesException noMatch)
                        {
                            Console.Error.WriteLine(string.Format(Strings.NoResxMatches, noMatch.TranslationResx, noMatch.ReferenceResx));
                            return false;
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine(string.Format(Strings.noResxReferenceFile, importFile.FullName, referenceResx));
                        return false;
                    }
                }
                else
                {
                    Console.Error.WriteLine(Strings.invalidFileType);
                    return false;
                }
            };

            if (importFile != null)
            {
                string lcid = Path.GetFileNameWithoutExtension(importFile.Name);
                if (model.SetLanguageFlags(lcid, true))
                {
                    if (import(lcid))
                    {
                        model.Update();
                        Console.WriteLine(Strings.importSuccess, importFile);
                    }
                }
                else
                {
                    Console.Error.WriteLine(Strings.invalidLocale);
                }
            }
            else
            {
                Console.Error.WriteLine(Strings.noImportFileSpecified);
            }
        }

        static void Export(Mode mode, DataModel model, DirectoryInfo exportFolder, string lcid, string keyPrefix)
        {
            Action export = () => { if (mode == Mode.ExportResx) model.ExportToResx(exportFolder.FullName, keyPrefix); else model.ExportToCsv(exportFolder.FullName); };

            if (exportFolder != null)
            {
                if (!string.IsNullOrEmpty(lcid))
                {
                    model.DeselectAllLanguages();
                    model.SetLanguageFlags(lcid, true, false);

                    export();
                    Console.WriteLine(Strings.singleLocalExportSuccess, lcid, exportFolder);
                }
                else if (model.HasTargetLanguages || mode == Mode.ExportResx)
                {
                    export();
                    Console.WriteLine(Strings.exportSuccess, exportFolder);
                }
                else
                {
                    Console.Error.WriteLine(Strings.noExportableTranslations);
                }
            }
            else
            {
                Console.Error.WriteLine(Strings.noExportFolderSpecified);
            }
        }
    }
}
