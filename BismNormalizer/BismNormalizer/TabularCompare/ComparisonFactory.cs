using Microsoft.AnalysisServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Class for instantiation of Core.Comparison objects using simple factory design pattern.
    /// </summary>
    public static class ComparisonFactory
    {
        // Factory pattern: https://msdn.microsoft.com/en-us/library/orm-9780596527730-01-05.aspx 

        private static int _minCompatibilityLevel = 1100;
        private static int _maxCompatibilityLevel = 1500;
        private static List<string> _supportedDataSourceVersions = new List<string> { "PowerBI_V3" };

        /// <summary>
        /// Uses factory design pattern to return an object of type Core.Comparison, which is instantiated using MultidimensionalMetadata.Comparison or TabularMeatadata.Comparison depending on SSAS compatibility level. Use this overload when running in Visual Studio.
        /// </summary>
        /// <param name="comparisonInfo">ComparisonInfo object for the comparison.</param>
        /// <param name="userCancelled">If use decides not to close .bim file(s) in Visual Studio, returns true.</param>
        /// <returns>Core.Comparison object</returns>
        public static Comparison CreateComparison(ComparisonInfo comparisonInfo, out bool userCancelled)
        {
            //This overload is for running in Visual Studio, so can set PromptForDatabaseProcessing = true
            comparisonInfo.PromptForDatabaseProcessing = true;

            // Need to ensure compatibility levels get initialized here (instead of comparisonInfo initialization properties). This also serves to prep databases on workspace server while finding compatibility levels
            comparisonInfo.InitializeCompatibilityLevels(out userCancelled);
            if (userCancelled)
            {
                return null;
            }

            return CreateComparisonInitialized(comparisonInfo);
        }

        /// <summary>
        /// Uses factory design pattern to return an object of type Core.Comparison, which is instantiated using MultidimensionalMetadata.Comparison or TabularMeatadata.Comparison depending on SSAS compatibility level.
        /// </summary>
        /// <param name="bsmnFile">Full path to the BSMN file.</param>
        /// <returns>Core.Comparison object</returns>
        public static Comparison CreateComparison(string bsmnFile, string appName)
        {
            ComparisonInfo comparisonInfo = ComparisonInfo.DeserializeBsmnFile(bsmnFile, appName);
            return CreateComparison(comparisonInfo);
        }

        /// <summary>
        /// Uses factory design pattern to return an object of type Core.Comparison, which is instantiated using MultidimensionalMetadata.Comparison or TabularMeatadata.Comparison depending on SSAS compatibility level.
        /// </summary>
        /// <param name="comparisonInfo">ComparisonInfo object for the comparison.</param>
        /// <returns>Core.Comparison object</returns>
        public static Comparison CreateComparison(ComparisonInfo comparisonInfo)
        {
            comparisonInfo.InitializeCompatibilityLevels();
            return CreateComparisonInitialized(comparisonInfo);
        }

        private static Comparison CreateComparisonInitialized(ComparisonInfo comparisonInfo)
        {
            Telemetry.TrackEvent("CreateComparisonInitialized", new Dictionary<string, string> { { "App", comparisonInfo.AppName.Replace(" ", "") } });

            ////Currently can't source from PBIP to AS because AS doesn't work with inline data sources:
            //if (comparisonInfo.ConnectionInfoSource.ServerName.StartsWith("powerbi://") && !comparisonInfo.ConnectionInfoTarget.ServerName.StartsWith("powerbi://"))
            //{
            //    throw new ConnectionException($"Source model is a Power BI dataset and the target is Analysis Services, which is not supported in the current version.");
            //}

            //If composite models not allowed on AS, check DQ/Import at model level matches:
            if (!comparisonInfo.ConnectionInfoSource.ServerName.StartsWith("powerbi://") && !Settings.Default.OptionCompositeModelsOverride && comparisonInfo.SourceDirectQuery != comparisonInfo.TargetDirectQuery)
            {
                throw new ConnectionException($"Mixed DirectQuery settings are not supported for AS skus.\nSource is {(comparisonInfo.SourceDirectQuery ? "On" : "Off")} and target is {(comparisonInfo.TargetDirectQuery ? "On" : "Off")}.");
            }

            #region Data-source versions check

            //If Power BI, check the default datasource version
            //Source
            bool sourceDataSourceVersionRequiresUpgrade = false;
            if (comparisonInfo.ConnectionInfoSource.ServerName.StartsWith("powerbi://") && !_supportedDataSourceVersions.Contains(comparisonInfo.SourceDataSourceVersion))
            {
                sourceDataSourceVersionRequiresUpgrade = true;
            }
            //Target
            bool targetDataSourceVersionRequiresUpgrade = false;
            if (comparisonInfo.ConnectionInfoTarget.ServerName.StartsWith("powerbi://") && !_supportedDataSourceVersions.Contains(comparisonInfo.TargetDataSourceVersion))
            {
                targetDataSourceVersionRequiresUpgrade = true;
            }
            //Check if user willing to upgrade the data-source version(s)
            if (sourceDataSourceVersionRequiresUpgrade && targetDataSourceVersionRequiresUpgrade)
            {
                string message = $"The source and target are Power BI datasets have default data-source versions of {comparisonInfo.SourceDataSourceVersion} and {comparisonInfo.TargetDataSourceVersion} respectively, which are not supported for comparison.";
                if (comparisonInfo.Interactive && System.Windows.Forms.MessageBox.Show(
                    message += $"\nDo you want to upgrade them both to {_supportedDataSourceVersions[0]} and allow the comparison?\n\nNOTE: this is a irreversible operation and you may not be able to download the PBIX file(s) to Power BI Desktop. You should only do this if you have the original PBIX as a backup.", comparisonInfo.AppName, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                {
                    throw new ConnectionException(message);
                }
            }
            else if (sourceDataSourceVersionRequiresUpgrade)
            {
                string message = $"The source is a Power BI dataset with default data-source version of {comparisonInfo.SourceDataSourceVersion}, which is not supported for comparison.";
                if (comparisonInfo.Interactive && System.Windows.Forms.MessageBox.Show(
                    message += $"\nDo you want to upgrade it to {_supportedDataSourceVersions[0]} and allow the comparison?\n\nNOTE: this is a irreversible operation and you may not be able to download the PBIX file(s) to Power BI Desktop. You should only do this if you have the original PBIX as a backup.", comparisonInfo.AppName, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                {
                    throw new ConnectionException(message);
                }
            }
            else if (targetDataSourceVersionRequiresUpgrade)
            {
                string message = $"The target is a Power BI datasets with default data-source version of {comparisonInfo.TargetDataSourceVersion}, which is not supported for comparison.";
                if (comparisonInfo.Interactive && System.Windows.Forms.MessageBox.Show(
                    message += $"\nDo you want to upgrade it to {_supportedDataSourceVersions[0]} and allow the comparison?\n\nNOTE: this is a irreversible operation and you may not be able to download the PBIX file(s) to Power BI Desktop. You should only do this if you have the original PBIX as a backup.", comparisonInfo.AppName, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                {
                    throw new ConnectionException(message);
                }
            }

            #endregion

            //Check if one of the supported compat levels:
            if (
                   !(comparisonInfo.SourceCompatibilityLevel >= _minCompatibilityLevel && comparisonInfo.SourceCompatibilityLevel <= _maxCompatibilityLevel &&
                     comparisonInfo.TargetCompatibilityLevel >= _minCompatibilityLevel && comparisonInfo.TargetCompatibilityLevel <= _maxCompatibilityLevel
                    )
               )
            {
                throw new ConnectionException($"This combination of mixed compatibility levels is not supported.\nSource is {Convert.ToString(comparisonInfo.SourceCompatibilityLevel)} and target is {Convert.ToString(comparisonInfo.TargetCompatibilityLevel)}.");
            }

            //Return the comparison object & offer upgrade of target if appropriate
            Comparison returnComparison = null;

            if (comparisonInfo.SourceCompatibilityLevel >= 1200 && comparisonInfo.TargetCompatibilityLevel >= 1200)
            {
                returnComparison = new TabularMetadata.Comparison(comparisonInfo);
                TabularMetadata.Comparison returnTabularComparison = (TabularMetadata.Comparison)returnComparison;

                //Upgrade default DATA-SOURCE versions if required
                if (sourceDataSourceVersionRequiresUpgrade)
                {
                    returnTabularComparison.SourceTabularModel.Connect();
                    returnTabularComparison.SourceTabularModel.TomDatabase.Model.DefaultPowerBIDataSourceVersion = Microsoft.AnalysisServices.Tabular.PowerBIDataSourceVersion.PowerBI_V3;
                    returnTabularComparison.SourceTabularModel.TomDatabase.Update();
                    returnTabularComparison.Disconnect();
                }
                if (targetDataSourceVersionRequiresUpgrade)
                {
                    returnTabularComparison.TargetTabularModel.Connect();
                    returnTabularComparison.TargetTabularModel.TomDatabase.Model.DefaultPowerBIDataSourceVersion = Microsoft.AnalysisServices.Tabular.PowerBIDataSourceVersion.PowerBI_V3;
                    returnTabularComparison.TargetTabularModel.TomDatabase.Update();
                    returnTabularComparison.Disconnect();
                }

                //Check if source has a higher compat level than the target and offer upgrade if appropriate.
                if (comparisonInfo.SourceCompatibilityLevel > comparisonInfo.TargetCompatibilityLevel)
                {
                    string message = $"Source compatibility level is { Convert.ToString(comparisonInfo.SourceCompatibilityLevel) } and target is { Convert.ToString(comparisonInfo.TargetCompatibilityLevel) }, which is not supported for comparison.\n";

                    if (comparisonInfo.Interactive && 
                        !comparisonInfo.ConnectionInfoTarget.UseProject && //Upgrade in SSDT not supported
                        System.Windows.Forms.MessageBox.Show(
                    message += $"\nDo you want to upgrade the target to {Convert.ToString(comparisonInfo.SourceCompatibilityLevel)} and allow the comparison?", comparisonInfo.AppName, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        returnTabularComparison.TargetTabularModel.Connect();
                        returnTabularComparison.TargetTabularModel.TomDatabase.CompatibilityLevel = comparisonInfo.SourceCompatibilityLevel;
                        returnTabularComparison.TargetTabularModel.TomDatabase.Update();
                        returnTabularComparison.Disconnect();
                    }
                    else
                    {
                        throw new ConnectionException(message + "\nUpgrade the target compatibility level and retry.");
                    }
                }
            }
            else
            {
                if (comparisonInfo.SourceCompatibilityLevel == comparisonInfo.TargetCompatibilityLevel)
                {
                    returnComparison = new MultidimensionalMetadata.Comparison(comparisonInfo);
                }
                else
                {
                    throw new ConnectionException($"This combination of mixed compatibility levels is not supported.\nSource is {Convert.ToString(comparisonInfo.SourceCompatibilityLevel)} and target is {Convert.ToString(comparisonInfo.TargetCompatibilityLevel)}.");
                }
            }

            return returnComparison;
        }
    }
}
