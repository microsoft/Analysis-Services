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

        private static List<int> _supportedCompatibilityLevels = new List<int>() { 1100, 1103, 1200, 1400 };

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
        public static Comparison CreateComparison(string bsmnFile)
        {
            ComparisonInfo comparisonInfo = ComparisonInfo.DeserializeBsmnFile(bsmnFile);
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
            if (comparisonInfo.SourceCompatibilityLevel != comparisonInfo.TargetCompatibilityLevel && !(comparisonInfo.SourceCompatibilityLevel == 1200 && comparisonInfo.TargetCompatibilityLevel == 1400))
            {
                throw new ConnectionException($"This combination of mixed compatibility levels is not supported.\nSource is {Convert.ToString(comparisonInfo.SourceCompatibilityLevel)} and target is {Convert.ToString(comparisonInfo.TargetCompatibilityLevel)}.");
            }

            if (comparisonInfo.SourceDirectQuery != comparisonInfo.TargetDirectQuery)
            {
                throw new ConnectionException($"Mixed DirectQuery settings are not supported.\nSource is {(comparisonInfo.SourceDirectQuery ? "On" : "Off")} and target is {(comparisonInfo.TargetDirectQuery ? "On" : "Off")}.");
            }

            //We know both models have same compatibility level, but is it supported?
            if (!_supportedCompatibilityLevels.Contains(comparisonInfo.SourceCompatibilityLevel))
            {
                throw new ConnectionException($"Models have compatibility level of {Convert.ToString(comparisonInfo.SourceCompatibilityLevel)}, which is not supported by this version of BISM Normalizer.\nPlease check http://bism-normalizer.com/purchase for other versions.");
            }

            if (comparisonInfo.SourceCompatibilityLevel >= 1200)
            {
                return new TabularMetadata.Comparison(comparisonInfo);
            }
            else
            {
                return new MultidimensionalMetadata.Comparison(comparisonInfo);
            }
        }

    }
}
